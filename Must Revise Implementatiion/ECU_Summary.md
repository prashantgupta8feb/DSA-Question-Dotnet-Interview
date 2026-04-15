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
 