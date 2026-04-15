---

MUI — Deep Dive (codebase-driven)

Authoritative review of the current codebase (C# .NET Framework 4.8). Use this to explain the system end-to-end in interviews.

Quick pointers

•	Main web project: MUI (MUI\MUI.Web.csproj) — hybrid ASP.NET MVC + WebForms.

•	Business layer: AccountService.cs (MUI.BusinessService\*.cs).

•	Data layer: ICTSMessagesRepository.cs (repository classes calling stored procedures).

•	Domain: AirportDomain.cs (domain DTOs).

•	DI: Unity container wired in UnityRegistration.cs + UnityControllerFactory.cs.

•	Mapper: AutoMapper helper MapperHelper.cs.

•	Auth: Forms authentication (cookie-based) in Global.asax.cs + AccountController.cs.

•	Reports/UI libs: Crystal Reports in MUI\Reports\*.rpt, Telerik in views.

•	Important controller example used throughout: AirportController.cs.

---

1. Architecture & Project Structure (what it actually is)

Summary: Monolith with layered organization (Presentation → BusinessService → DataAccess → Domain). Not a strict Clean Architecture: layers exist but dependencies are mixed and some inversion-of-control is ad-hoc.

Projects (top-level visible)

•	MUI — web app: controllers, views (.aspx/.ascx), Global.asax, web.config.

•	Files of interest:

•	Global.asax.cs — application lifecycle and authentication hook.

•	BaseController.cs — common controller helpers.

•	UnityControllerFactory.cs — Unity-based controller activation.

•	AirportController.cs — airport CRUD/search flow (example).

•	MUI\Views\... — many .aspx/.ascx pages (WebForms).

•	AccountService.cs — service classes and BusinessServiceFactory.

•	BusinessServiceFactory.cs maps Domain types to concrete service types and provides GetService<T>().

•	Example: AccountService.cs shows service delegating to AccountRepository.

•	ICTSMessagesRepository.cs — repository classes (ADO.NET wrappers).

•	Examples: AccountRepository.cs, AirportRepository.cs (pattern: build parameters -> call SP using DataBlockHelper -> read DataSet/IDataReader -> populate Domain DTOs).

•	AirportDomain.cs — domain DTOs/entities (e.g., AirportDomain.cs, AirportCodeList.cs).

•	AssemblyInfo.cs & MUI.Helpers — logging/audit/database helpers.

•	Test projects: App.config, AssemblyInfo.cs (presence only; content unknown).

Responsibilities and coupling

•	Controllers: UI interaction, forms handling, read/write TempData/Session, invoke business services.

•	Business Services: orchestrate business logic, validate, call repositories (but often instantiate repos directly).

•	Repositories: execute stored procedures via DataBlockHelper and create domain objects.

•	Mapping: AutoMapper configured through MapperHelper.

•	DI: Unity container used to register and resolve some services/repositories and to set a controller factory. However, not all concrete types are registered or resolved via DI — many services still New repositories. This results in mixed coupling (DI + direct instantiation).

How to explain in interview

•	"It’s a layered monolith. Layers exist, but the enforcement of inversion of control is inconsistent — Unity is configured, but many services still instantiate repositories directly, which increases coupling."

---

2. End-to-End Request Flow — concrete example (AirportCodes)

I’ll trace a real request (the common Airport Codes screen), step-by-step, showing files and methods that run.

High-level endpoint: user opens Airport Codes screen (MVC route → AirportController.AirportCodes())

Step-by-step execution trace (request → response):

1.	Browser request

•	URL: e.g., /Airport/AirportCodes (conventional routing in Global.asax.cs).

•	Routing defined: Global.asax.cs RegisterRoutes (default route {controller}/{action}/{id}).

2.	Controller activation

•	MVC requests are routed to controller types resolved by UnityControllerFactory:

•	UnityRegistration.Register() sets up a UnityContainer and stores it in Container.

•	ControllerBuilder.Current.SetControllerFactory(typeof(UnityControllerFactory)) ensures Unity is used.

•	GetControllerInstance(RequestContext, Type) resolves the controller instance via MvcUnityContainer.Container.Resolve(controllerType).

3.	Controller method: AirportController.AirportCodes() (file: AirportController.cs)

•	The action constructs an AirportModelView:

•	Reads TempData keys SearchAirportCode / SearchAirportName if present.

•	Calls model.AirportList = this.SearchAirports(model.SearchAirportCode, model.SearchAirportName);

4.	Controller → BusinessService call

•	AirportController constructor obtains business service:

•	AirportService = BusinessServiceFactory.GetService<AirportDomain>(); (calls into BusinessServiceFactory).

•	SearchAirports() calls GetAllAirportCodes() which calls AirportService.GetAll().

•	GetAllAirportCodes():

•	IList<AirportDomain> airportDomains = AirportService.GetAll();

•	ViewBag.RecordCount = airportDomains.Count;

•	Uses AutoMapper MapperHelper.CreateMapper<AirportDomain, Airport>() to map domain objects to view models (Airport view model).

5.	BusinessService to DataAccess

•	BusinessServiceFactory.GetService<AirportDomain>() returns a concrete service type (mapping registered in BusinessServiceFactory).

•	Inside the concrete service (e.g., AirportService), typical pattern:

•	Many services create repository instances directly, e.g., new AirportRepository() or call a repository through DI if registered.

•	The repository executes database logic:

•	Repositories live in MUI.DataAccess\*Repository.cs.

•	They build List<InputParameter> and call ExecuteDatasetFromSP(string, List<InputParameter>, string, string, int) or ExecuteReaderFromSP(string, List<InputParameter>, string) with a stored procedure constant (from StoredProcedures).

•	Example: AccountRepository.Validate() uses DataBlockHelper.ExecuteReaderFromSP(StoredProcedures.ValidateLogin, inputParameters, out errorMessage); — airport repository follows the same pattern.

6.	Data mapping and response assembly

•	AirportRepository reads IDataReader or DataSet, populates AirportDomain objects.

•	AirportService returns domain objects to controller.

•	Controller maps AirportDomain → Airport view model via AutoMapper (MapperHelper.CreateMapper<AirportDomain, Airport>()).

•	Controller returns View(model) or JSON for AJAX endpoints (SubmitAirportForm(Airport) returns Json()).

7.	Browser receives HTML + embedded data (or JSON for Ajax).

•	Many pages are WebForms Index.aspx that rely on server-side rendering and Telerik widgets; AirportCodes.aspx view is used to present the AirportModelView.

Files referenced in trace

•	Routing + auth: Global.asax.cs

•	Controller: AirportController.cs

•	Business services factory: BusinessServiceFactory.cs

•	Example service: AccountService.cs (shows pattern of new repository instantiation)

•	Repository: AccountRepository.cs (serves as canonical pattern)

•	Mapping: MapperHelper.cs

•	DB helper & constants: MUI.DataAccess\Common\DataBlockHelper (not fully displayed here) and StoredProcedures constants.

How data transforms

•	UI model (view model) Airport ↔ mapped via AutoMapper to/from AirportDomain (domain DTO).

•	AirportDomain is populated by repository from raw ADO.NET results.

•	Controller maps domain → view model and renders view.

How to explain in interview

•	"Start at controller action -> it asks BusinessServiceFactory for a service -> the service uses a repository that calls stored procedures with a custom DataBlockHelper -> repository returns domain DTOs -> controller maps domain -> view model via AutoMapper and returns .aspx view or JSON for Ajax."

---

3. Database Access — what is used (deep dive)

Concrete findings from code:

•	No Entity Framework / DbContext pattern encountered in the repository code inspected.

•	Data access is custom ADO.NET with stored procedures.

•	Pattern:

•	Build parameter list: List<InputParameter> (in repository).

•	Call helper: ExecuteDatasetFromSP(string, List<InputParameter>, string, string, int) or ExecuteReaderFromSP(string, List<InputParameter>, string).

•	Read DataSet/IDataReader and map columns to domain properties.

•	Examples:

•	AccountRepository.cs:

•	var inputParameters = new List<InputParameter>();

•	IDataReader reader = DataBlockHelper.ExecuteReaderFromSP(StoredProcedures.ValidateLogin, inputParameters, out errorMessage);

•	while (reader.Read()) { entity.AgentId = reader.GetInt32(...); ... }

Mapping strategy

•	Manual mapping inside repository methods: reading columns and setting domain properties.

•	AutoMapper used at controller boundary to map domain → view models (MapperHelper). But repository → domain mapping is manual.

Connection & transaction management

•	Connection creation and execution are encapsulated in DataBlockHelper (not shown fully). The code pattern indicates centralized helper for executing SPs.

•	There is no visible UnitOfWork/DbContext or explicit TransactionScope usage in the snippets inspected.

•	Conclusion: transactions are either handled inside stored procedures or not centralized in code. (If transaction needed, likely implemented inside stored procedures or in DataBlockHelper — inspect that helper for details; in our inspected files, no explicit UoW found.)

How queries are written

•	Stored procedure names are constants (probably in StoredProcedures).

•	The code relies on SPs for both reads and writes — the repository forms parameters and invokes SPs.

How to explain in interview

•	"This project uses a classic ADO.NET Stored-Proc repository approach. Repositories assemble parameter lists, call a shared DataBlockHelper to execute the SP, and manually map IDataReader/DataSet records to domain DTOs. No EF Core or DbContext is present."

---

4. Authentication & Authorization (exact behavior)

Authentication mechanism in codebase:

•	Cookie-based Forms Authentication (ASP.NET FormsAuth), not JWT.

•	AccountController.SetAuthenticationTicket(...) creates FormsAuthenticationTicket and writes encrypted cookie via FormsAuthentication.Encrypt.

•	Global.asax.cs Application_AuthenticateRequest(Object, EventArgs) reads the cookie: FormsAuthentication.Decrypt(authCookie.Value); and rebuilds GenericPrincipal using roles from the ticket's UserData (split by comma).

•	Roles are stored in authTicket.UserData at issuance time.

•	Session is heavily used:

•	Session["LoginUserID"], Session["UserEnvironment"], Session["Password"], Session["UserRoels"], Session["RoleMatrix"], etc.

Authorization:

•	Attribute-based role checks:

•	Example: AirportController decorated with [MUIAuthorize(Roles = "airportcodes (mgmt)")] (custom attribute).

•	Application_AuthenticateRequest(Object, EventArgs) sets Context.User = new GenericPrincipal(...), which standard [Authorize] attributes will act upon.

Login flow (concrete)

•	AccountController.LogOn(LogOnModel model):

•	Map to domain: MapperHelper.CreateMapper<LogOnModel, AccountDomain>().Map(...).

•	Calls this.acountService.Validate(agent); — service calls repository to invoke ValidateLogin SP.

•	On success: sets Session values and calls SetAuthenticationTicket(model.UserName, agent.AgentSecurityLevelCode) to create forms-auth cookie.

How to explain in interview

•	"Auth is classic ASP.NET FormsAuth with roles embedded into the auth ticket. On each request Global.asax reads the ticket and sets HttpContext.User. The app relies on server Session for user state and role matrix."

Missing features / Not implemented

•	Not implemented: token-based / JWT auth. No refresh-token mechanism. Not implemented: centralized auth middleware typical in ASP.NET Core.

---

5. Exception Handling (what exists)

What is implemented:

•	MVC-level error handling via HandleErrorAttribute:

•	BaseController is decorated with [HandleError].

•	Global.asax.cs RegisterGlobalFilters adds HandleErrorAttribute() to GlobalFilters.Filters.

•	Many controllers perform IsValid(object, ValidationContext) checks and set ModelState.AddModelError.

•	Repositories often set ValidationErrorMessage / DomainError on domain objects instead of throwing exceptions — services propagate these back to controllers and controllers convert into ModelState errors or JSON responses.

•	Session_End() hook in Global.asax.cs used for cleanup logging on session end.

What is not implemented:

•	No global exception-handling middleware (ASP.NET Core-style) is present. There is no central code that serializes all unhandled exceptions into consistent JSON responses (the framework's custom errors / HandleError view pipeline is used for MVC views).

•	No consistent API error wrapper for JSON endpoints (some actions return Json(airportDomain.DomainError...) ad-hoc).

How to explain in interview

•	"Exception handling is handled by MVC's HandleError(DataSet, Dictionary<string, List<int>>, DataRow, List<string>, List<string>, List<string>, List<string>) and individual controller/service patterns where domain errors are captured in domain objects and surfaced to views. There is no centralized JSON error middleware for API-style responses."

Interview tip

•	Emphasize the difference between handling HTML view errors (HandleError) versus a modern API global exception middleware.

---

6. Logging strategy (actual code)

What is present:

•	Custom DB-based logging and instrumentation:

•	AssemblyInfo.cs and classes like DatabaseLogger, LoggingInstrumentationDomain, AgentAccountChangeAuditLogger (files in MUI.DataAccess\Common\*).

•	Repositories call into logging/instrumentation helpers directly, e.g., var loggingInstrumentationDomain = DatabaseLogger.LogInstrumentationStart(...); and later DatabaseLogger.LogInstrumentationEnd(...).

•	Example: Update(Domain.FlightManifestDomain) uses DatabaseLogger and AgentAccountChangeAuditLogger.LogAgentAccountChange(...) for audit logging.

•	No Serilog / Microsoft.Extensions.Logging usage (this is a .NET Framework MVC app). The code uses enterprise-style database logging (custom).

Usage patterns

•	Logging is synchronous and performed inline in repositories/services.

•	Audit logging is done for sensitive events (password change, user actions) and appears to persist into DB audit tables via repositories / logging helper.

Correlation & request tracking

•	No explicit correlation ID middleware or structured logging framework present. Some informal GUIDs used for user session tracking (Session["UserGUID"]) and UserLogGUID are logged.

How to explain in interview

•	"Logging is DB/audit oriented and embedded in repository/service methods via AssemblyInfo.cs helpers. There is no standard structured logging pipeline like Serilog/ILogger in current code."

Improvement note

•	Suggest moving to structured logging and centralized sink (Serilog, ELK), and adding correlation ID in request pipeline.

---

7. Middleware / Request pipeline (classic ASP.NET)

What exists:

•	Classic ASP.NET pipeline (Global.asax events).

•	Application_Start() registers areas, global filters and routes, and calls UnityRegistration.Register().

•	Application_AuthenticateRequest(Object, EventArgs) reconstructs principal from FormsAuth cookie.

•	Session_End() handles session end cleanup.

MVC filters used

•	Global filter: HandleErrorAttribute.

•	Custom action filters / attributes:

•	SessionExpireFilterAttribute — used on controllers/actions to check session expiration.

•	NoCacheAttribute — sets HTTP cache headers (file: NoCache.cs).

•	PopulateSiteMap attribute (used for building SiteMap/Telerik Nav).

•	Controller factory: UnityControllerFactory replaces default controller activation (Dependency Injection integration).

Execution order (for a request)

1.	IIS / ASP.NET pipeline constructs HttpContext.

2.	Application_AuthenticateRequest(Object, EventArgs) runs early to set User.

3.	Routing maps to controller/action.

4.	Controller instance resolved via UnityControllerFactory.

5.	Action filters and attributes run (e.g., SessionExpireFilterAttribute, custom attributes).

6.	Controller action executes.

7.	HandleError(DataSet, Dictionary<string, List<int>>, DataRow, List<string>, List<string>, List<string>, List<string>) and MVC exception pipeline handle errors to show error pages.

How to explain in interview

•	"Because this is classic ASP.NET MVC, the pipeline is event-driven with Global.asax hooks. Unity replaces the controller factory so controllers are created from the container. Action filters provide cross-cutting concerns (session, caching)."

---

8. Design Patterns observed (concrete)

Patterns used (with where they appear)

•	Repository Pattern

•	MUI.DataAccess\*Repository.cs implement IDataRepository<T> interface.

•	Concrete repos (e.g., AccountRepository) encapsulate data-access to DB/SPs.

•	Service / Facade Pattern

•	Business services MUI.BusinessService\*Service.cs implement IBusinessService<T>.

•	Factory

•	BusinessServiceFactory maps Domain type -> Service type and returns service via Activator.CreateInstance.

•	Dependency Injection (partial)

•	Unity container (Microsoft.Practices.Unity) used; UnityRegistration.Register() registers many types and sets the UnityControllerFactory.

•	But DI is inconsistent: many services New repositories directly.

•	Mapper / Adapter

•	MapperHelper centralizes AutoMapper mapping configurations.

•	Not used / Not implemented

•	CQRS: Not implemented.

•	Unit of Work: Not implemented (no DbContext/UoW found).

•	Modern middleware pipeline: Not applicable (classic ASP.NET).

How to explain in interview

•	"This system mixes patterns — repo + service + factory are present, but enforcement of DI is inconsistent. The factory uses reflection to return services, which is simple but bypasses container-based lifecycle control."

Interview tip

•	Be ready to discuss the pros/cons: factory + new -> simple but hard to unit test; DI container -> testability but requires consistent usage.

---

9. Dependency Injection — concrete wiring

Where DI is set up

•	UnityRegistration.cs:

•	Creates new UnityContainer() and calls obj.RegisterType<...> for many repositories and services.

•	Registers IBusinessServiceFactory (twice actually in that file).

•	Sets MvcUnityContainer.Container = container and ControllerBuilder.Current.SetControllerFactory(typeof(UnityControllerFactory)).

•	UnityControllerFactory resolves controllers via MvcUnityContainer.Container.Resolve(controllerType).

Limitations in the codebase

•	Not all services are registered in Unity; BusinessServiceFactory continues to create services via reflection (Activator.CreateInstance(serviceType)).

•	Unable to control lifetimes or use constructor injection consistently due to mixed instantiation patterns.

How to explain in interview

•	"Unity is used to register many concrete types and to resolve controllers. But because BusinessServiceFactory uses Activator.CreateInstance, many services are not controlled by the DI container. In an interview, highlight that full DI adoption would improve testability and lifecycle control."

---

10. Configuration & environment handling (actual)

Where config lives

•	web.config and MUI\DataAccess\app.config hold connection strings and appSettings.

•	Code uses System.Web.Configuration.WebConfigurationManager.AppSettings[...] (seen in SetAuthenticationTicket(string, string) for AuthCookieTimeout) and WebConfigurationManager.ConnectionStrings (in GetEnvironments()).

Secrets management

•	Not implemented: no Azure Key Vault or similar; secrets and connection strings live in config files.

Environment-based configs

•	Not implemented as a structured pattern (no appsettings.{env}.json etc.) — classic web.config approach.

How to explain in interview

•	"Configuration is classic web.config/app.config usage. For cloud deployment we'd propose migrating to environment-based appsettings + secrets management."

---

11. Performance & Code Quality (observations & suggestions)

Current issues (observed in code)

•	Synchronous ADO.NET usage and blocking ExecuteReaderFromSP(string, List<InputParameter>, string) patterns — blocking threads under load.

•	Heavy server-side Session usage reduces scale (session-affine server requirements).

•	Many DataSet/DataTable usages and ExecuteDatasetFromSP(string, List<InputParameter>, string, string, int) which is memory-heavy for large results.

•	Direct New of repositories prevents tuning lifetimes and replacing implementations easily.

•	No centralized caching layer detected (no IMemoryCache/Redis calls).

•	No async controller/service methods (no async/await observed).

Potential bottlenecks

•	Database calls made for each data-bound request (no caching).

•	Telerik grids with server-side paging may cause frequent full data reads depending on implementation.

Immediate improvements for production-readiness

•	Introduce async ADO.NET / Dapper or EF Core async calls.

•	Reduce server Session dependence: move to stateless tokens (JWT) and client storage.

•	Add caching for read-heavy lists (IMemoryCache or Redis).

•	Replace DataSet-based heavy reads with streaming/IDataReader->DTO pipelines or Dapper.

•	Adopt consistent DI usage and enable unit testing.

How to explain in interview

•	"This app predates modern async and stateless patterns; the fastest wins are migrating to async DB calls, removing server Session for stateless tokens, and adding caching for read-heavy endpoints."

---

12. Testing — what exists

What we found

•	Test projects exist: App.config and AssemblyInfo.cs (project files present).

•	No systematic test coverage analysis done here; content not inspected.

Observations / gaps

•	Because services create repositories with New, unit testing those services is harder (cannot easily inject mocks).

•	Tests will be easier if BusinessServiceFactory and repositories are refactored to rely on interfaces and container injection.

Interview explanation

•	"There are test projects present, but the codebase pattern (direct New inside services) hampers easy mocking. For true unit/integration testing, refactor to constructor DI for repositories."

---

13. Gaps & Improvements (very important)

List of concrete missing / weak areas (prioritized)

1.	Consistent Dependency Injection

•	Problem: BusinessServiceFactory + direct New in services.

•	Fix: Register all services/repositories in DI container and remove Activator.CreateInstance use.

2.	Centralized error handling for APIs

•	Problem: HandleError(DataSet, Dictionary<string, List<int>>, DataRow, List<string>, List<string>, List<string>, List<string>) is for HTML views; no global JSON error middleware.

•	Fix: Add centralized exception middleware (in a new .NET 10 API) that returns standard API error responses.

3.	Modern Authentication for API (for PoC & cloud)

•	Problem: FormsAuth + Session not suitable for stateless API.

•	Fix: New API should issue JWTs; legacy adapter can keep FormsAuth for the existing UI.

4.	Asynchronous data access

•	Problem: blocking DB calls (DataSet/IDataReader).

•	Fix: Use async ADO.NET or Dapper/EF Core async operations.

5.	Logging / observability

•	Problem: Custom DB logging is fine for audit, but no structured centralized logs (e.g., Serilog).

•	Fix: Add structured logging + correlation id + external sink (ELK, Seq).

6.	Transaction / Unit of Work

•	Problem: No UoW abstraction.

•	Fix: If porting to EF, use DbContext and transactions; otherwise provide a transactional wrapper.

7.	Config & secrets

•	Problem: web.config-based secrets.

•	Fix: Move to environment-based appsettings and secret store.

8.	Reporting and UI

•	Risk: Crystal Reports and Telerik are tightly integrated; moving UI to React requires server export endpoints or partial server-side report generation.

9.	Caching and performance tuning

•	Implement IMemoryCache/Redis and tune queries; reduce DataSet usage.

10.	Tests & CI

•	Make DI consistent to allow unit tests with mocks; add integration tests for DB calls.

How to explain in interview

•	"This project is a classic monolith with reasonable layering but requires a modernization plan — make DI consistent, move to stateless auth for APIs, add async I/O and structured logging, and treat Crystal/Telerik as UI migration risks."

---

Quick migration / PoC advice (concise)

•	For PoC (AirportCodes) you can:

•	Option A (recommended): Port small data access pieces (AirportRepository & required helpers) into .NET 10 API (requires replacing DataBlockHelper / web.config usage with Microsoft.Data.SqlClient and config).

•	Option B: Create .NET Framework adapter service that references existing AirportService and exposes REST endpoints. New .NET 10 API calls that adapter via HTTP. (No code changes to data layer.)

•	Because MapperHelper centralizes mapping, reuse its mappings in the PoC if you move code into new API.

Files to copy / inspect for PoC B (minimum):

•	AirportService.cs (if exists) or BusinessServiceFactory mapping for AirportDomain.

•	AirportRepository.cs

•	AirportDomain.cs and any dependencies (AirportCodeList.cs).

•	MapperHelper.cs

•	AirportCodes.aspx (for UI reference)

---

Interview Coaching — short scripts

Use these short, precise lines when asked.

•	Architecture summary

•	"It's a layered monolith (Presentation → BusinessService → DataAccess → Domain). DI via Unity exists, but not enforced across the codebase — some services still New repositories."

•	Request lifecycle (in one line)

•	"Request routes to an MVC controller (resolved via Unity), the controller obtains a BusinessService from BusinessServiceFactory, the service calls a repository which executes stored procedures through DataBlockHelper, returns domain objects, controller maps domain → viewmodel with AutoMapper and returns the view or JSON."

•	Data access rationale

•	"This codebase favors stored procedures for DB logic and uses ADO.NET wrappers for performance/policy reasons. Modernizing can preserve stored procs while replacing the client with Dapper/EFCore for convenience and async support."

•	Security explanation

•	"Uses FormsAuth cookie and server-side Session. For an API-first approach, recommend JWT-based stateless tokens; for PoC we can build a thin adapter to reuse current domain/data layers."

•	Design patterns succinctly

•	"Repository, Service, Factory, AutoMapper. DI via Unity is used but applied partially. No UnitOfWork or CQRS is present."

---

Appendix — concrete code references (quick list)

•	Global / app lifecycle: Global.asax.cs

•	Controller factory (DI): UnityControllerFactory.cs, UnityRegistration.cs

•	Base controller: BaseController.cs

•	Airport UI controller: AirportController.cs

•	Business factory: BusinessServiceFactory.cs

•	Example service: AccountService.cs

•	Example repository: AccountRepository.cs (pattern)

•	Mapper: MapperHelper.cs

•	Custom attributes: NoCache.cs

•	Reports & assets: MUI\Reports\*.rpt, MUI\aspnet_client\system_web\...

•	Views: LogOn.aspx, AirportCodes.aspx

---

Final Notes & How to present this in an interview

1.	Start by naming the architecture: "Layered monolith with partial DI (Unity) and stored-proc-based data access."

2.	Walk a single request (AirportCodes) end-to-end using the step-by-step trace above — mention the exact files.

3.	Highlight two strengths: clear separation (controller/service/repo) and mature DB stored-proc usage; and two weaknesses: inconsistent DI and session-coupled auth.

4.	Finish with concrete modernization steps: make DI consistent, add async data access, use stateless token auth for new API, centralize logging, and isolate Crystal/Telerik as separate UI migration risks.

---
 