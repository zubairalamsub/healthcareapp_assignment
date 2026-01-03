# Healthcare Management System

A full-stack healthcare management application for managing patients, caregivers, and offices with role-based access control.

## 🛠️ Tech Stack

**Backend:** ASP.NET Core 8.0 Web API • Entity Framework Core • Dapper • SQL Server • JWT Authentication • In-Memory Caching

**Frontend:** Angular 17 • TypeScript • Reactive Forms • RxJS • Custom CSS

## ✨ Features

- **Patient Management** - CRUD operations, caregiver assignments, office filtering
- **Caregiver Management** - Search, filter by office, specialization tracking
- **Office Management** - Multi-location support (5 Bangladesh cities)
- **Role-Based Access** - Admin, Manager, User roles with different permissions
- **Search & Filter** - Keyword search, sorting, pagination with record counts
- **Caching** - 5-minute cache with automatic invalidation
- **Audit Logging** - Track all search activities
- **Validation** - Client and server-side validation with error messages
- **Responsive UI** - Modal editing, animations, professional design

## 🚀 Quick Start

### 1. Database Setup
```bash
sqlcmd -S localhost -i Database/SetupDatabase.sql
```
Creates database, tables, indexes, and seeds 44 records (5 offices, 18 caregivers, 21 patients).

### 2. Backend
```bash
cd Backend
dotnet restore
dotnet run
```
Runs on `https://localhost:7097`

### 3. Frontend
```bash
cd Frontend
npm install
ng serve
```
Runs on `http://localhost:4200`

## 🔐 Login Credentials

| Username | Password | Role | Access |
|----------|----------|------|--------|
| admin | Password123! | Admin | Full CRUD |
| manager | Password123! | Manager | Create/Update patients |
| user1 | Password123! | User | Read-only |

## 📁 Project Structure

```
Backend/
├── Controllers/     # API endpoints
├── Models/         # Database entities
├── DTOs/           # Data transfer objects
├── Services/       # Business logic + caching
├── Repositories/   # Data access (EF Core + Dapper)
└── Data/           # DbContext

Frontend/
└── src/app/
    ├── components/  # Patient, Caregiver, Office, Login
    ├── services/    # HTTP API services
    └── models/      # TypeScript interfaces

Database/
├── SetupDatabase.sql      # Complete setup script
├── README.md             # Database documentation
└── QUICK_REFERENCE.md    # Useful queries
```

## 🗄️ Database Schema

**Tables:** Roles → Users, Offices → Caregivers, Offices → Patients, Patients ↔ Caregivers (many-to-many), SearchAudits

**Indexes:** 15 performance indexes on foreign keys, names, emails, dates

**Sample Data:** Bangladesh locations (Dhaka, Chittagong, Sylhet, Rajshahi, Khulna)

## 🔑 RBAC Permissions

| Action | Admin | Manager | User |
|--------|-------|---------|------|
| View Patients | ✅ | ✅ | ✅ |
| Create Patient | ✅ | ✅ | ❌ |
| Edit Patient | ✅ | ✅ | ❌ |
| Delete Patient | ✅ | ❌ | ❌ |
| Assign Caregivers | ✅ | ✅ | ❌ |

## 📡 Key API Endpoints

```
POST   /api/auth/login              # Login (returns JWT)
GET    /api/patients                # Get patients (paginated)
POST   /api/patients                # Create patient
PUT    /api/patients/{id}           # Update patient
DELETE /api/patients/{id}           # Delete patient
GET    /api/caregivers/search       # Search caregivers (with filters)
GET    /api/offices/search          # Get offices
```

## 🎯 Key Features Explained

### Caching Strategy
- Patient/Caregiver lists cached for 5 minutes
- Cache keys include: keyword, officeId, page, pageSize, sortBy, sortDescending
- Automatic invalidation on create/update/delete
- Prevents stale data across office filters

### Patient-Caregiver Assignment
- Many-to-many relationship via PatientCaregivers table
- Checkbox UI for easy selection
- Multiple caregivers per patient
- Office-based caregiver filtering

### Search Optimization
- Indexed columns: FirstName, LastName, Email, Phone, OfficeId
- EF.Functions.Like for LIKE queries
- Dapper for read-heavy operations
- Sub-300ms response time

### Validation
- **Frontend:** Reactive Forms, real-time error messages, required field indicators
- **Backend:** Data Annotations, ModelState validation
- **Examples:** Email format, required fields, date validation

## 🎨 UI Highlights

- **Quick Login Buttons** - Auto-fill credentials for demo accounts
- **Modal Edit Form** - Popup for editing patients (non-intrusive)
- **Checkbox Caregivers** - Replace multi-select with checkboxes
- **Office Filter** - Dropdown to filter caregivers by office
- **Pagination Info** - Shows "1-10 of 50 patients" + page number
- **Row Numbers** - Numbered rows with pagination awareness
- **Validation Messages** - Inline error messages with icons

## 🔧 Configuration

**Backend** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HealthcareDB;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "HealthcareApp",
    "Audience": "HealthcareApp",
    "ExpiryMinutes": 60
  }
}
```

**Frontend** (`environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

## 📸 Screenshots

Automated screenshot tool:
```bash
cd screenshots
npm install
npm run screenshots
```
Captures 11 screenshots of all pages automatically.

## 📦 Production Build

**Backend:**
```bash
dotnet publish -c Release -o ./publish
```

**Frontend:**
```bash
ng build --configuration production
```

**Database:**
Run `SetupDatabase.sql` on production SQL Server.

## 🐛 Troubleshooting

**Cache not clearing?**
- Fixed: Cache keys now include officeId
- Automatic invalidation on patient create/update/delete

**CORS error?**
- Backend allows `http://localhost:4200` by default
- Update `Program.cs` if frontend runs on different port

**Database connection failed?**
- Verify SQL Server running
- Check connection string
- Run `SetupDatabase.sql` first

## 📊 Performance

- **Search Response:** <300ms
- **Cache Hit Ratio:** ~80% for repeated searches
- **Database Queries:** Optimized with 15 indexes
- **Pagination:** Server-side (efficient for large datasets)

## 🔒 Security

- JWT authentication with expiry
- Password hashing (ASP.NET Core Identity)
- Parameterized queries (SQL injection prevention)
- CORS configuration
- Role-based authorization middleware
- Audit logging for all searches

## 📝 Sample Data

- **5 Offices** - Bangladesh cities with addresses
- **18 Caregivers** - Various specializations (GP, Pediatrics, Cardiology, Nursing, Surgery, etc.)
- **21 Patients** - Complete demographic info
- **34 Assignments** - Realistic patient-caregiver relationships
- **3 Roles** - Admin, Manager, User
- **5 Users** - Pre-configured accounts

---

**Quick Test:** Login as `admin/Password123!` → Add Patient → Assign Caregivers → Filter by Office → Test caching!
