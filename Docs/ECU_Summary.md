Condensed Level Summary for interview:
Based on your ECU summary, here's your pro-level walkthrough:

Opening line — set the context:

"This is a legacy enterprise ASP.NET MVC application built on .NET Framework 4.8, following a classic layered architecture 
with a hybrid UI, service layer, and stored-procedure based data access."

The application follows a classic 3-tier N-tier architecture. The presentation tier handles HTTP requests via MVC controllers 
and Web Forms. The business logic tier encapsulates domain rules in service classes. The data access tier abstracts database 
operations through repositories using ADO.NET and stored procedures. Each tier has a single responsibility and only communicates 
with the tier directly below it.  
Strictly speaking it's N-tier rather than just 3-tier because there's an additional domain/DTO layer — AirportDomain — that acts 
as a shared contract across all tiers, and a separate mapping layer via AutoMapper. So the separation is more granular than a 
basic 3-tier.


The flow — UI to Database:
1. UI Layer

"The UI is a hybrid of ASP.NET MVC controllers and legacy Web Forms pages. Incoming requests hit the Global.asax pipeline where 
Application_AuthenticateRequest reconstructs the user principal from a Forms Authentication ticket and roles are loaded into session. 
Requests then route conventionally to controllers like AccountController or AirportController, all inheriting from 
BaseController which provides shared session and HttpContext helpers."

2. Controller Layer

"Controllers perform model binding and ModelState validation first. Before calling downstream, they set environment context via 
CallContext.SetData('MuiEnvironment') so the data layer knows the execution context without needing it passed explicitly through 
every method. Custom attributes like SessionExpireFilterAttribute and NoCache handle cross-cutting concerns at this level."

3. Service Layer

"Controllers delegate business logic to service classes in AccountService. These services orchestrate the business rules and 
coordinate between multiple repositories when needed. DI is handled partially via Unity container through UnityControllerFactory,
though some services still instantiate repositories directly using new — a technical debt we'd address in any modernization effort."

4. Repository / Data Access Layer

"Repositories implement IDataRepository<T> and use a custom ADO.NET helper — DataBlockHelper — to execute stored procedures. 
The pattern is: build input parameters → call stored proc → read IDataReader or DataSet → map to domain DTOs. There's no ORM like
Entity Framework — all data access is stored-procedure driven, which gives tight DB control but makes the layer verbose."

5. Domain & Mapping

"Domain objects from AirportDomain flow across all layers as DTOs. AutoMapper via MapperHelper handles mapping between domain 
objects and view models centrally, keeping controllers clean."

6. Logging & Audit

"Logging is DB-coupled — audit events like password changes are written directly to audit tables via DatabaseLogger and 
specialized audit loggers like AgentAccountChangeAuditLogger. This is tightly integrated into the data access helpers themselves."


Closing — show architect thinking:

"The architecture works well for a stable enterprise system but has clear modernization opportunities — inconsistent DI, server-side session dependency, no middleware pipeline, and DB-coupled logging. If migrating to .NET Core, the priorities would be: normalize DI with Microsoft.Extensions.DependencyInjection, replace Forms Auth with JWT, introduce a proper middleware pipeline, and adapt logging to Microsoft.Extensions.Logging while keeping the stored-proc layer intact initially to reduce risk."
Below is a concise technical summary of the MUI project (current .NET Framework 4.8) covering architecture, major features and how they are implemented. Use this as a quick reference when planning the PoC migration.

1.	Projects and high-level architecture

•	Projects in the solution

•	MUI (web app) — hybrid ASP.NET MVC + Web Forms web UI (controllers under MUI\Controllers, views under MUI\Views).

•	AccountService.cs — service layer / business logic (many *Service classes).

•	ICTSMessagesRepository.cs — data access repositories (stored-proc / ADO.NET wrappers).

•	AirportDomain.cs — domain models / DTOs used across layers.

•	AssemblyInfo.cs — logging / DB helper / instrumentation helpers.

•	app.config — external integrations.

•	App.config, AssemblyInfo.cs — test projects.

•	Layering / dependencies

•	MUI → AccountService.cs, AirportDomain.cs, ICTSMessagesRepository.cs, AssemblyInfo.cs

•	AccountService.cs → ICTSMessagesRepository.cs, AirportDomain.cs

•	ICTSMessagesRepository.cs → AirportDomain.cs, AssemblyInfo.cs

•	Composition patterns

•	Mixed DI + factory pattern: Unity used for many registrations (see UnityRegistration.cs + UnityControllerFactory.cs), but BusinessServiceFactory still uses Activator.CreateInstance and many services New repositories directly.

2.	Web/UI layer (how it’s built)

•	Hybrid UI:

•	MVC controllers in MUI\Controllers\* (e.g., AccountController, AirportController).

•	Many legacy Web Forms pages / user controls under MUI\Views\* (Index.aspx, AddAlternateAirportPairEditor.ascx) — e.g., LogOn.aspx.

•	Routing and app lifecycle:

•	Global.asax.cs handles Application_Start(), registers areas, global filters and routes (conventional {controller}/{action}/{id}).

•	Application_AuthenticateRequest(Object, EventArgs) reads forms-auth cookie and builds GenericPrincipal.

•	Important base types & filters:

•	BaseController (BaseController.cs) — common helpers that read Session and HttpContext.

•	Custom attributes: SessionExpireFilterAttribute, NoCache (in MUI.CustomAttributes), PopulateSiteMap, etc.

•	Global HandleError(DataSet, Dictionary<string, List<int>>, DataRow, List<string>, List<string>, List<string>, List<string>) attribute is applied (RegisterGlobalFilters(GlobalFilterCollection)).

3.	Data access (ORM / patterns)

•	No EF/ORM detected — repository + ADO.NET stored-proc model:

•	Repositories implement IDataRepository<T> (many classes under MUI.DataAccess\*, e.g., AccountRepository, AirportRepository).

•	Data access uses custom helpers: DataBlockHelper (ExecuteReader/ExecuteDatasetFromSP), InputParameter wrappers, StoredProcedures constants.

•	Typical repository code: build param list → call stored proc → read IDataReader / DataSet → populate domain DTOs.

•	Transaction / unit-of-work

•	No DbContext/UoW pattern present; each repository manages SP calls directly.

•	How repositories are obtained

•	Mixed: some repos/services are registered in Unity and resolved; many services call new SomeRepository() directly (partial DI).

4.	Mapping & DTOs

•	AutoMapper used:

•	MapperHelper builds many MapperConfiguration instances and exposes generic CreateMapper<T,K>() — mapping between domain objects and view models is centralized there.

5.	Logging, instrumentation and auditing

•	Logging components:

•	AssemblyInfo.cs contains custom logging helpers / DatabaseLogger, LoggingInstrumentationDomain, AgentAccountChangeAuditLogger and other audit loggers.

•	Repositories and services call these helpers to log instrumentation and audit events (e.g., password-change logging in Update(Domain.FlightManifestDomain)).

•	Where logs go

•	Predominantly DB-based logging / audit tables (via Enterprise-style helpers), plus any file/other sinks implemented in AssemblyInfo.cs.

•	Notes

•	Logging is tightly coupled to data-access helpers and stored-proc-based logging; replace/redirecting to Microsoft.Extensions.Logging will require adapter work.

6.	Authentication / Authorization / Session

•	Authentication

•	Forms authentication: FormsAuthenticationTicket created in SetAuthenticationTicket(string, string).

•	Application_AuthenticateRequest(Object, EventArgs) reconstructs the principal (roles in authTicket.UserData).

•	Session state

•	Heavy use of Session[...] for user/context state (LoginUserId, UserEnvironment, UserGUID, PasswordExpireFlag).

•	CallContext.SetData("MuiEnvironment", ...) is used in controllers before calling data layer.

•	Authorization

•	Roles stored in session and used by RoleBasedAccessHelper and UI role-matrix checks.

7.	Exception handling & validation

•	Controller-level

•	HandleErrorAttribute on BaseController and registered globally provides MVC error handling.

•	Many controller actions perform ModelState checks and add model errors.

•	Repository/service-level

•	DataAccess methods often do their own try/catch or set ValidationErrorMessage on domain objects rather than throwing.

•	Some methods still throw NotImplementedException for unimplemented paths.

•	Session end / global cleanup

•	Session_End() in Global.asax.cs records logout information using a utility.

8.	Middleware / pipeline

•	Classic ASP.NET pipeline (no ASP.NET Core middleware):

•	App lifecycle hooks in Global.asax.cs (Application_Start(), AuthenticateRequest, Session_End()).

•	Controller factory replacement: UnityControllerFactory replaces default controller activator, resolving controllers with Unity container.

•	No middleware-style pipeline (no IApplicationBuilder, no built-in middleware chain) — if migrating to .NET 10, middleware replacements must be implemented.

9.	Third-party UI/reporting components

•	Telerik

•	Telerik.Web.Mvc is used in views (Html.Telerik() helpers, tWindow, grids, editors). Many AddAlternateAirportPairEditor.ascx editor templates use Telerik controls.

•	Crystal Reports

•	MUI\Reports\*.rpt files and viewer assets under aspnet_client — server-side report generation integrated in controllers/reports pages.

10.	Testing, CI & build notes

•	Test projects present: App.config, AssemblyInfo.cs.

•	Projects are legacy (non-SDK csproj) targeting .NET Framework 4.8 — solution modernization will require converting csproj and adding multi-targeting / migration steps.

11.	Practical implications for PoC / migration (concise)

•	Data access is safe to reuse if you can host it from a .NET Framework process (or port repository helpers to .NET 10 and replace ADO.NET driver usage). No EF conversion required for PoC, but stored-proc calls must be validated with Microsoft.Data.SqlClient if porting.

•	Authentication & session: current flow uses FormsAuth + server session; PoC API should issue tokens (JWT) and avoid server session, or provide a compatibility adapter.

•	Logging and audit are DB-coupled; to preserve behavior keep AssemblyInfo.cs available to adapter or plan a migration to Microsoft.Extensions.Logging later.

•	DI is inconsistent: add small adapter/service layer to normalize DI for the new Web API rather than trying to inject legacy classes directly into .NET 10.

If you want, I can:

•	Produce a compact diagram or a one-page md describing the AirportCode flow from UI → Controller → BusinessService → Repository → DB, listing exact files and DTOs to copy for plan B.

•	Scaffold the PoC API controller skeleton and a minimal adapter mapping to the existing AirportRepository/AirportService.
 