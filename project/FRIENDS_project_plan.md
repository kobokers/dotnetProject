# F.R.I.E.N.D.S – Project Planning Document
**Course:** BIW 20503 DOTNET Programming | **Sem:** 2025/2026/2  
**Group size:** 5 members | **Total marks:** 20%

---

## Overview

A social networking web app and mobile app built on the F.R.I.E.N.D.S theme.  
Users can register, manage profiles, make friends, and post content.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Web (Project 1) | ASP.NET Core Web App (MVC), C# |
| Mobile (Project 2) | .NET MAUI |
| Database | SQL Server LocalDB (built-in with Visual Studio) |
| ORM | Entity Framework Core |
| Auth | ASP.NET Core Identity |
| Server | IIS Express (localhost) |

> **Why LocalDB:** It ships with Visual Studio, requires zero setup, and EF Core migrations work identically to full SQL Server. No connection string changes needed across group members.

---

## Project 1 — ASP.NET Core MVC Web App (Weeks 8–14)

### 4 Required Modules (all with full CRUD)

#### Module 1 — User / Profile
- Register, Login, Logout (via ASP.NET Identity)
- View & Edit own profile (name, bio, profile photo, location)
- View other users' profiles
- Delete account

**Model fields:** `UserId`, `DisplayName`, `Bio`, `ProfilePhoto`, `Location`, `CreatedAt`

#### Module 2 — Posts / Feed
- Create a post (text, optional image)
- View all posts in a feed (newest first)
- Edit own post
- Delete own post
- View a single post detail page

**Model fields:** `PostId`, `UserId`, `Content`, `ImageUrl`, `CreatedAt`, `UpdatedAt`

#### Module 3 — Friends
- Send a friend request
- Accept / Reject a friend request
- View your friends list
- Remove a friend (unfriend)

**Model fields:** `FriendRequestId`, `SenderId`, `ReceiverId`, `Status` (Pending/Accepted/Rejected), `CreatedAt`

#### Module 4 — Comments
- Add a comment on any post
- Read all comments under a post
- Edit own comment
- Delete own comment

**Model fields:** `CommentId`, `PostId`, `UserId`, `Content`, `CreatedAt`

---

### MVC Structure

```
/Controllers
    AccountController.cs      ← Module 1 (auth + profile)
    PostController.cs         ← Module 2
    FriendController.cs       ← Module 3
    CommentController.cs      ← Module 4

/Models
    ApplicationUser.cs        ← extends IdentityUser
    Post.cs
    FriendRequest.cs
    Comment.cs
    ApplicationDbContext.cs

/Views
    Account/   (Login, Register, Profile, Edit)
    Post/      (Index/Feed, Create, Edit, Details, Delete)
    Friend/    (Index, Requests, Remove)
    Comment/   (partial view inside Post/Details)
```

---

### Database (LocalDB via EF Core)

- Use `ApplicationDbContext : IdentityDbContext<ApplicationUser>`
- Run migrations: `Add-Migration Init` → `Update-Database`
- Connection string (appsettings.json):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FriendsDb;Trusted_Connection=True;"
}
```

---

### Marks Checklist — Project 1

| Rubric | Requirement | Target |
|---|---|---|
| P3 /2 | No runtime errors | All 4 modules working cleanly |
| P4 /4 | CRUD modules | 4 modules = full 4 marks |
| P4 /1 | Creativity/value added | See suggestions below |
| C3 /2 | Coherent system proposal | In report: intro + module diagram |
| C4 /2 | Requirements modelled | In report: use case / ER diagram |

**Creativity ideas (value added +1):**
- Profile photo upload
- Like button on posts
- Search/filter users by name
- Responsive UI with Bootstrap 5

---

## Project 2 — .NET MAUI Mobile App (Weeks 11–14)

### Goal
Replicate the web app's UI in mobile form. **Full CRUD is not required** — but more CRUD = more marks.

### Marks target (aim for 3–4 marks on P4)
| Level | Requirement |
|---|---|
| 1 mark | Static screens, menu links only |
| 2 marks | Navigation between all pages works |
| 3 marks | 2 modules with CRUD |
| 4 marks | 3 modules with CRUD |

### Recommended MAUI approach
- Use the same LocalDB via a **REST API layer** (add a minimal API project or Web API controllers to your MVC project)
- MAUI calls the API with `HttpClient`
- Alternatively: use SQLite locally in MAUI for standalone CRUD (simpler, no API needed)

### MAUI Pages to build

```
MainPage        ← Login / Register
FeedPage        ← View posts (list)
PostDetailPage  ← View single post + comments
ProfilePage     ← View & edit profile
FriendsPage     ← Friends list
```

### MAUI Module priority (for marks)
1. Posts — Create + Read (easiest to demo)
2. Profile — Read + Edit
3. Friends — Read list

---

## Report Structure

```
1. Cover Page
   - Project title: "F.R.I.E.N.D.S Social Network System"
   - Group number, member names, matric numbers, section

2. Introduction
   - What the system does
   - Why it's relevant to the F.R.I.E.N.D.S theme

3. Proposed System Modules & Submodules
   - Table: module name, description, CRUD operations
   - ER Diagram (draw with draw.io or dbdiagram.io)
   - Use Case Diagram (draw.io)

4. Discussion / Conclusion
   - What worked, what was challenging, what you learned

5. Attachment
   - Print and attach the Project Assessment Rubric
```

---

## Submission Checklist

```
Group1_Report.pdf
Group1_Video.mp4       ← demo of running app, ~5 min
Group1_Source.zip      ← full solution folder, must run on IIS
```

---

## Week-by-week Plan

| Week | Task |
|---|---|
| 8 | Setup project, DB, Identity (login/register), Module 1 Profile |
| 9 | Module 2 Posts (feed, CRUD) |
| 10 | Module 3 Friends + Module 4 Comments |
| 11 | Polish web app, start MAUI setup + navigation |
| 12 | MAUI: Feed + Profile pages with CRUD |
| 13 | MAUI: Friends page, fix bugs, record video |
| 14 | Final submission + presentation (10–12 min) |

---

## Notes for the Agent

- **Do not** use Razor Pages or Blazor — the project requires MVC specifically
- **Do not** scaffold generic CRUD — keep controllers thin, put logic in services if needed
- All views must use the shared `_Layout.cshtml` with a consistent navbar
- Identity is already a module (Profile = Module 1); do not add a 5th module unnecessarily
- LocalDB connection string uses `(localdb)\\mssqllocaldb` — double backslash in JSON
- EF Core package: `Microsoft.EntityFrameworkCore.SqlServer` + `Microsoft.EntityFrameworkCore.Tools`
- For MAUI, target `.NET 8` or `.NET 9` (match the MVC project's target framework)
- MAUI does **not** run on IIS — only the web app does; keep them as separate projects in the same solution
