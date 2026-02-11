using Application.Dtos;

namespace Application.Services.Onboarding;

public interface IOnboardingService
{
    Task<OnboardingSnapshotDto?> GetSnapshotAsync(string userId);
    Task<OnboardingProgressDto> UpdatePersonalDetailsAsync(string userId, PersonalDetailsDto dto);
    Task<OnboardingProgressDto> AddQualificationAsync(string userId, QualificationDto dto);
    Task<OnboardingProgressDto> SaveNextOfKinAsync(string userId, NextOfKinDto dto);
    Task<OnboardingProgressDto> SaveHrInfoAsync(string userId, HrInfoDto dto);
    Task<OnboardingProgressDto> UpdateDocumentsProgressAsync(string userId);
    Task<bool> IsOnboardingCompleteAsync(string userId);
}
