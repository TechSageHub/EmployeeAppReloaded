# StaffHubSystem - Comprehensive HR Management Solution

StaffHubSystem is a modern, enterprise-grade HR management system built with .NET 9. It streamlines organizational workflows by centralizing employee data, attendance tracking, leave management, and internal communications.

## 🚀 Features

### ✅ Implemented Modules
- **Employee Directory**: Centralized management of staff profiles, departments, and roles.
- **Leave & Absence Management**: Full request/approval workflow with automated balance tracking.
- **Attendance & Time Tracking**: Real-time clock-in/out system with monthly work-hour statistics.
- **Internal Communication (Notice Board)**: Role-based announcement system with pinned high-priority updates.
- **Digital Document Vault**: Secure cloud storage for employee IDs, contracts, and certifications via Cloudinary.
- **Dynamic Dashboard**: Real-time organizational statistics and quick action panels.

### 🛠️ Technology Stack
- **Backend**: ASP.NET Core MVC ( .NET 9)
- **Database**: Microsoft SQL Server (via EF Core)
- **Identity**: Microsoft Identity for secure Authentication & Authorization
- **Storage**: Cloudinary for profile images and secure document storage
- **UI/UX**: Bootstrap 5 with custom modern aesthetics and bi-icons

## ⚙️ Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB supported)

### Setup Instructions
1. **Clone the Repository**
   ```bash
   git clone https://github.com/TechSageHub/EmployeeAppReloaded
   cd EmployeeAppReloaded
   ```

2. **Configure Database**
   Update the `DefaultConnection` in `Presentation/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=StaffHubSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Configure Cloudinary** (Optional for Image/Docs)
   Add your Cloudinary credentials in `appsettings.json`.

4. **Apply Migrations**
   ```bash
   dotnet ef database update --project Data --startup-project Presentation
   ```

5. **Run the Project**
   ```bash
   dotnet run --project Presentation
   ```

## 🔐 Credentials (Initial Seed)
- **Admin**: `admin@staffhub.com` / `Admin@123`

---
*Developed by TechSageHub - Transforming HR Workflows.*
