using Application.Dtos;
using Application.Services.UploadImage;
using Data.Context;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Document;

public class DocumentService(EmployeeAppDbContext _context, IImageService _imageService) : IDocumentService
{
    public async Task<EmployeeDocumentDto> UploadDocumentAsync(UploadDocumentDto dto)
    {
        var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "image/jpeg",
            "image/png"
        };

        if (dto.File == null || dto.File.Length == 0)
        {
            throw new InvalidOperationException("No file provided.");
        }

        if (dto.File.Length > 10 * 1024 * 1024)
        {
            throw new InvalidOperationException("File size exceeds 10 MB limit.");
        }

        if (string.IsNullOrWhiteSpace(dto.File.ContentType) || !allowedContentTypes.Contains(dto.File.ContentType))
        {
            throw new InvalidOperationException("Unsupported file type.");
        }

        // Upload file to Cloudinary
        var fileUrl = await _imageService.UploadFileAsync(dto.File, "documents");

        var document = new EmployeeDocument
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            DocumentType = dto.DocumentType,
            FileName = dto.File.FileName,
            FileUrl = fileUrl,
            UploadedDate = DateTime.UtcNow,
            ExpiryDate = dto.ExpiryDate,
            Notes = dto.Notes
        };

        await _context.EmployeeDocuments.AddAsync(document);
        await _context.SaveChangesAsync();

        var employee = await _context.Employees.FindAsync(dto.EmployeeId);

        return new EmployeeDocumentDto
        {
            Id = document.Id,
            EmployeeId = document.EmployeeId,
            EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown",
            DocumentType = document.DocumentType,
            FileName = document.FileName,
            FileUrl = document.FileUrl,
            UploadedDate = document.UploadedDate,
            ExpiryDate = document.ExpiryDate,
            Notes = document.Notes
        };
    }

    public async Task<List<EmployeeDocumentDto>> GetEmployeeDocumentsAsync(Guid employeeId)
    {
        var documents = await _context.EmployeeDocuments
            .Include(d => d.Employee)
            .Where(d => d.EmployeeId == employeeId)
            .OrderByDescending(d => d.UploadedDate)
            .ToListAsync();

        return documents.Select(d => new EmployeeDocumentDto
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            EmployeeName = $"{d.Employee.FirstName} {d.Employee.LastName}",
            DocumentType = d.DocumentType,
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            UploadedDate = d.UploadedDate,
            ExpiryDate = d.ExpiryDate,
            Notes = d.Notes
        }).ToList();
    }

    public async Task<bool> DeleteDocumentAsync(Guid documentId)
    {
        var document = await _context.EmployeeDocuments.FindAsync(documentId);
        if (document == null) return false;

        if (!string.IsNullOrEmpty(document.FileUrl))
        {
            var publicId = _imageService.ExtractPublicIdFromUrl(document.FileUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                await _imageService.DeleteImageAsync(publicId);
            }
        }

        _context.EmployeeDocuments.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<EmployeeDocumentDto>> GetExpiringDocumentsAsync(int daysAhead = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
        var documents = await _context.EmployeeDocuments
            .Include(d => d.Employee)
            .Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value <= cutoffDate && d.ExpiryDate.Value >= DateTime.UtcNow)
            .OrderBy(d => d.ExpiryDate)
            .ToListAsync();

        return documents.Select(d => new EmployeeDocumentDto
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            EmployeeName = $"{d.Employee.FirstName} {d.Employee.LastName}",
            DocumentType = d.DocumentType,
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            UploadedDate = d.UploadedDate,
            ExpiryDate = d.ExpiryDate,
            Notes = d.Notes
        }).ToList();
    }
}
