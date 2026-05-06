# 🍽️ TasteTribe

**App for Foodies with Bot Integration**  
ISM6225 – App Dev for Analytics | USF M.S. in Artificial Intelligence & Business Analytics

🔗 **Live Site:** [https://tastetribe-awcza6hug2fqcjb5.canadacentral-01.azurewebsites.net/](https://tastetribe-awcza6hug2fqcjb5.canadacentral-01.azurewebsites.net/)

[![Live Demo](https://img.shields.io/badge/Demo-TasteTribe-green)](https://tastetribe-awcza6hug2fqcjb5.canadacentral-01.azurewebsites.net/)



---

## 📡 API Endpoints Used

TasteTribe integrates the **Restaurants Near Me USA** API via RapidAPI to allow users to search for restaurants by zip code.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/restaurantapi/by-location?zip={zipcode}` | Fetches restaurants near a given zip code from RapidAPI |

**How it works:**  
The frontend calls our backend endpoint (`/api/restaurantapi/by-location?zip=60616`), which acts as a secure server-side proxy. The ASP.NET controller builds an HTTP request to RapidAPI using `IHttpClientFactory`, attaches the API key server-side, and returns the JSON response to the frontend. The API key is never exposed to the browser.

**External API used:** `https://restaurants-near-me-usa.p.rapidapi.com/restaurants/location/zipcode/{zip}/0`

---

## 🗃️ Data Model (ERD)

```
User
├── UserId (PK)
├── FirstName
└── LastName

Restaurant
├── RestaurantId (PK)
├── Name
└── CuisineType

Dish
├── DishId (PK)
├── Name
└── RestaurantId (FK → Restaurant)

Review
├── ReviewId (PK)
├── Rating
├── ReviewText
├── UserId (FK → User)
├── RestaurantId (FK → Restaurant)
└── DishId (FK → Dish)
```

**Relationships:**
- A `Restaurant` has many `Dishes`
- A `User`, `Restaurant`, and `Dish` can each have many `Reviews`
- A `Review` belongs to one `User`, one `Restaurant`, and one `Dish`
- Foreign keys use `ON DELETE NO ACTION` to avoid cascade conflicts in SQL Server

Tables are managed by **Entity Framework Core** and were created automatically via migrations against an Azure SQL Database (Basic tier).

---

## ⚙️ CRUD Implementation Overview

All CRUD operations are implemented in `HomeController.cs` using Entity Framework Core through the injected `AppDbContext` (`_context`).

**Create**
- GET: Loads dropdown data for Users, Restaurants, and Dishes into `ViewBag` and returns the blank form
- POST: Accepts form data, checks if free-text was entered for user/restaurant/dish and creates new records if needed, validates the model, saves the review, and redirects to Read

**Read**
- Queries all Reviews using `.Include()` to eagerly load related User, Restaurant, and Dish navigation properties
- Passed to `Read.cshtml` as `IEnumerable<Review>` and rendered in a table

**Update**
- GET: Loads the review by ID with all related data pre-filled in the form using `asp-for` tag helpers
- POST: Checks if a restaurant/dish with the submitted name already exists before creating a new one to avoid duplicates, then calls `_context.Update()` and `SaveChangesAsync()`

**Delete**
- GET: Loads the review with all related data and shows a confirmation page
- POST (`DeleteConfirmed`): Removes the review from the database and redirects to Read

**Dynamic Charts (Data Page)**
- Data is pulled from the database using LINQ in `HomeController.Data()`
- Aggregated with `.GroupBy()`, `.Average()`, and `.Count()` and stored in `ViewBag`
- Serialized to JSON in `Data.cshtml` using `@Html.Raw(JsonSerializer.Serialize(...))`
- Rendered on the frontend using **Chart.js** (bar, pie, and line charts)

---

## 🏗️ MVC Architecture

| Layer | Technology | Role |
|-------|-----------|------|
| **Model** | C# classes + EF Core | Defines data structure; maps to Azure SQL tables |
| **View** | Razor (.cshtml) + Bootstrap | Renders HTML with `@model`, `ViewBag`, and tag helpers |
| **Controller** | ASP.NET Core MVC | Handles GET/POST requests, queries database, passes data to views |

`AppDbContext` (in `/Data/AppDbContext.cs`) defines `DbSet<T>` properties for each model and configures foreign key relationships in `OnModelCreating`.

---

## ⚠️ Notable Technical Challenges & Solutions

**1. Azure SQL tier defaulted to General Purpose instead of Basic**  
When creating the database, Azure defaulted to General Purpose (vCore-based) which cost ~$16 in two days. We manually switched to the Basic DTU tier ($4.90/month) after deployment.

**2. Cascade delete conflict on Reviews table**  
EF Core's default cascade delete behavior caused a SQL Server error when creating the Reviews table because multiple cascade paths existed (User → Review, Restaurant → Review, Dish → Review). Fixed by setting `.OnDelete(DeleteBehavior.NoAction)` for all Review foreign keys in `OnModelCreating`.

**3. Malformed `appsettings.json` caused app crash on Azure**  
A missing closing `}` in `appsettings.json` caused a `JsonReaderException` on startup. The app showed HTTP 500.30. Fixed by correcting the JSON and republishing.

**4. Azure subscription suspended mid-development**  
The student subscription ran out of credits during development, freezing all write operations. Resolved by upgrading to pay-as-you-go and reactivating the subscription.

**5. App Service region quota conflict**  
The Free F1 tier was unavailable in West US 2 (same region as the database). App Service was deployed to Canada Central instead, which had available quota — the database connection still works across regions.

**6. Publish credentials expiring after subscription reactivation**  
After reactivating the subscription, Visual Studio's saved publish profile had stale credentials. Fixed by downloading a fresh publish profile from the Azure portal and re-importing it in Visual Studio.

**7. EF Core package version mismatch**  
Running `Add-Migration` failed with a `MissingMethodException` due to a version conflict between EF Core packages. Fixed by pinning all three packages (`SqlServer`, `Tools`, `Design`) to version 9.0.4.

---

## 🚀 Deployment

- **Hosting:** Azure App Service (Free F1 tier, Canada Central)
- **Database:** Azure SQL Database (Basic tier, 5 DTUs, West US 2)
- **Deployment method:** Visual Studio Web Deploy (right-click → Publish → Azure App Service Windows)
- **Tables created via:** EF Core migrations (`Add-Migration` + `Update-Database`)