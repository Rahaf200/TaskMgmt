Task Management API : 

Proje Açıklaması : 

Task Management API, kullanıcıların projeler, görevler (tasks) ve yorumlar (comments) üzerinde işlem yapabildiği bir RESTful Web API’dir.
Proje ASP.NET Core, Entity Framework Core, JWT Authentication ve SQLite kullanılarak geliştirilmiştir.
-Kimlik Doğrulama & Yetkilendirme : 
  * JWT (JSON Web Token) tabanlı authentication
  * Rol bazlı yetkilendirme : 
                               - Admin
                               - User

-Erişim Kuralları: 
  * Controller API (/api/...) :
       ** Users Controller (/api/users) --> sadece Admin
       ** Diğer Controller endpoint’leri --> Giriş yapmış tüm kullanıcılar (Admin + User)
  * Minimal API (/minimal/...) : Giriş yapmış tüm kullanıcılar (Admin + User) erişebilir

-Özellikler:
  * Kullanıcı yönetimi
  * Proje yönetimi
  * Proje bazlı görev (task) yönetimi
  * Görevler için yorum sistemi
  * Soft delete (IsDeleted)
  * Global exception middleware
  * Standart API response yapısı
  * Swagger dokümantasyonu

Mimari Diagram : 

Client[Client / Swagger / Postman]
Auth[JWT Authentication]
Controllers[Controllers API]
Minimal[Minimal API]
Services[Service Layer]
DbContext[AppDbContext ] 
Database[(SQLite Database)]

Client --> Auth
Auth --> Controllers
Auth --> Minimal
Controllers --> Services 
Minimal --> Services
Services --> DbContext 
DbContext --> Database

Endpoint listesi : 

* Authentication :
 method   Endpoint          Description
 POST     /api/auth/login   Login and get JWT token

* Users-Controller API (Admin Only) :
Method          Endpoint         Description 
GET             /api/users       Get all users
GET             /api/users/{id}  Get user by ID
POST            /api/users       Create new user
PUT             /api/users/{id}  Update user
DELETE          /api/users/{id}  Soft delete user

* Users-Minimal API (Admin + User) :
Method          Endpoint             Description 
GET             /minimal/users       Get all users
GET             /minimal/users/{id}  Get user by ID
POST            /minimal/users       Create new user
PUT             /minimal/users/{id}  Update user
DELETE          /minimal/users/{id}  Delete user

* Projects-Minimal API (Authenticated Users) :
Method          Endpoint                Description 
GET             /minimal/projects       Get all projects
GET             /minimal/projects/{id}  Get project by ID
POST            /minimal/projects       Create new project
PUT             /minimal/projects/{id}  Update project
DELETE          /minimal/projects/{id}  Delete project

* Tasks-Minimal API :
Method          Endpoint                                   Description 
GET             /minimal/projects/{projectId}/tasks        Get tasks by project 
GET             /minimal/projects/{projectId}/tasks/{id}   Get task by ID
POST            /minimal/projects/{projectId}/tasks        Create new task
PUT             /minimal/projects/{projectId}/tasks/{id}   Update task
DELETE          /minimal/projects/{projectId}/tasks/{id}   Delete task

* Comments-Minimal API :
Method          Endpoint                                Description 
GET             /minimal/tasks/{taskId}/comments        Get task comments  
GET             /minimal/tasks/{taskId}/comments/{id}   Get comment
POST            /minimal/tasks/{taskId}/comments        Create new comment
PUT             /minimal/tasks/{taskId}/comments/{id}   Update comment
DELETE          /minimal/tasks/{taskId}/comments/{id}   Delete comment

* Access Rules Summary : * Controller APIs (/api/*) : 
                                  * Users → Admin only
                                  * Projects / Tasks / Comments → Any authenticated user
                         * Minimal APIs (/minimal/*) :
                                  * Any authenticated user


API Response Örnekleri : 

* Başarılı Response : 
{
  "success": true,
  "message": "Project created",
  "data": {
    "id": 1,
    "name": "Task Management System",
    "description": "Demo project",
    "userId": 1,
    "createdAt": "2024-01-01T10:00:00",
    "updatedAt": "2024-01-01T10:00:00"
  }
}

* Validation Error : 
{
  "success": false,
  "message": "Validation failed",
  "data": [
    "Name is required"
  ]
}

* Unauthorized :
{
  "success": false,
  "message": "Unauthorized",
  "data": null
}

* Not Found : 
{
  "success": false,
  "message": "Project not found",
  "data": null
}

Kurulum talimatları : 
* Repository’yi Klonla : 
   git clone https://github.com/Rahaf200/TaskMgmt.git
   cd TaskMgmt
   git checkout dev-handling

* Gerekli Araçlar : 
  .NET 9 SDK (or compatible .NET 7+ SDK)
  SQLite
  Visual Studio or VS Code

* Connection String : 
  The SQLite connection string is configured in appsettings.json:
  {
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=taskmgmt.db"
  }
}

* Uygulamayı Çalıştır :
  dotnet run

* Swagger UI :
   https://localhost:{port}/swagger
   1.Login via /api/auth/login
   2.Copy JWT token
   3.Click Authorize
   4.Paste the token in the following format : Bearer {token}
   (inside the value bar only past the token)

* Default Seed Users : 
 Username  Passwords    Role 
 admin     admin123     Admin 
 john      john123      User


