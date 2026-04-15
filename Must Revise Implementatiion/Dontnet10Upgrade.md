/*
1. Exposing the Existing ASP.NET MVC MUI Project as a Web API
Goal:

Make the business/data logic of MUI available via RESTful endpoints (Web API), so it can be consumed by new UIs or other systems.

Steps:

Identify Core Business Logic

Find controllers/actions in MUI that contain real business/data logic.

Move this logic into separate service classes (if not already done).

This makes it easier to call from both MVC and Web API.

Create a New ASP.NET Core Web API Project (on .NET 10)

Scaffold a new project:
dotnet new webapi -n MUI.WebApi -f net10.0

Add references to your business/data logic projects (migrated to .NET 10 or netstandard).

Expose Endpoints

For each key feature, create a controller and expose endpoints (e.g., /api/bookings, /api/users).

Return data as JSON.

Migrate/Adapt Logic

Move or adapt logic from MVC controllers to API controllers.

Use dependency injection for services.

Authentication/Authorization

Implement JWT or OAuth2 if needed (ASP.NET Core has built-in support).

Test

Use Swagger (OpenAPI) for easy testing and documentation.

Tip:
You can run the new Web API alongside the old MVC app during migration (strangler pattern).

2. .NET 10 Alternatives for Crystal Reports
Crystal Reports is not supported in .NET Core/.NET 5+ (including .NET 10).
You cannot use the old Crystal Reports runtime in .NET 10.

Alternatives:

Option

Description

Notes

Stimulsoft Reports

Modern, .NET Core/.NET 10 compatible, web-based designer and viewer

Commercial, feature-rich, similar to Crystal

DevExpress XtraReports

Full-featured, .NET Core/.NET 10 support, web and desktop

Commercial, strong support

Syncfusion Reporting

.NET Core/.NET 10 support, web-based, good for dashboards

Commercial, free for small companies

ActiveReports.NET

.NET Core/.NET 10 support, web and desktop

Commercial

FastReport.NET

.NET Core/.NET 10 support, lightweight, web and desktop

Commercial, affordable

Roll your own (PDF/Excel)

Use libraries like QuestPDF, iText7, or OpenXML to generate reports programmatically

More work, but full control

Migration Approach:

For each Crystal Report, decide if you need to:

Rebuild it in a new reporting tool (above).

Replace with a new UI/dashboard.

Export to PDF/Excel using code.

3. .NET 10 Alternatives for Telerik Components
Telerik UI for ASP.NET MVC/WebForms does not work on .NET 10.
Telerik UI for ASP.NET Core is the supported path.

Alternatives:

Option

Description

Notes

Telerik UI for ASP.NET Core

Official Telerik library for .NET Core/.NET 10, supports Razor Pages, MVC, TagHelpers

Commercial, similar API to old Telerik

DevExpress ASP.NET Core

Competing commercial suite, full .NET 10 support

Commercial

Syncfusion ASP.NET Core

Large set of controls, .NET 10 support

Commercial, free for small companies

Kendo UI for jQuery/React/Angular

Telerik’s JS-based controls, can be used with ASP.NET Core APIs

Commercial, more SPA-oriented

Open Source (Bootstrap, jQuery UI, etc.)

Use standard web controls, integrate with APIs

Free, but less feature-rich

Migration Approach:

For each Telerik control/page:

Find the equivalent in Telerik UI for ASP.NET Core (or another suite).

Replace old markup/components with new ones.

For grids, charts, etc., wire up to new Web API endpoints.

4. Summary Table
Legacy Tech

.NET 10 Alternative(s)

Migration Notes

Crystal Reports

Stimulsoft, DevExpress, Syncfusion, FastReport, ActiveReports, or custom PDF

Must rebuild reports in new tool; no direct port

Telerik MVC/WebForms

Telerik UI for ASP.NET Core, DevExpress, Syncfusion, Kendo UI, or open source

Replace controls with .NET Core compatible ones*/
