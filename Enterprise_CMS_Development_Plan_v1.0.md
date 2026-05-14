# ENTERPRISE CMS — Comprehensive Development Plan v1.0

**.NET 9 | ASP.NET MVC | SQL Server | jQuery | Clean Architecture**  
**Repository Pattern | Plugin Architecture | 15 Pillars | 100+ Features**

> **Intended Audience:** AI Development Agent  
> **Document Version:** 1.0 | **Date:** May 2026

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Technology Stack](#2-technology-stack)
3. [Solution Structure](#3-solution-structure-clean-architecture)
4. [Database Design Principles](#4-database-design-principles)
5. [Plugin Architecture](#5-plugin-architecture)
6. [The 15 Feature Pillars](#6-the-15-feature-pillars)
7. [Development Phases](#7-development-phases)
8. [Coding Standards & Conventions](#8-coding-standards--conventions-for-ai-agent)
9. [Configuration Reference](#9-configuration-reference)
10. [Deliverables Checklist](#10-deliverables-checklist-per-phase)

---

## 1. Project Overview

The Enterprise CMS is a fully featured, WordPress-equivalent content management platform built on .NET 9 and ASP.NET MVC. It is designed to allow non-technical users to build, manage, and publish professional websites without writing a single line of code, while simultaneously providing enterprise-grade security, compliance, scalability, and developer extensibility.

The system targets three audiences simultaneously:

- **End users / site editors** — who need a simple, intuitive block-based page builder and content workflow.
- **Site owners / administrators** — who need multi-tenancy, role management, SEO tools, and analytics.
- **Developers / integrators** — who need a clean REST API, plugin hooks, CLI tooling, and extensible architecture.

### Design Philosophy

| Principle | Description |
|---|---|
| **Flexibility** | Every feature is pluggable. Core services are swappable via interfaces. No hard coupling to any vendor. |
| **Usability** | Any general user should be able to build a site in under 30 minutes with zero code. |
| **Enterprise** | Security, compliance (GDPR, PDPA), audit trails, MFA, SSO, and WCAG are first-class concerns, not afterthoughts. |
| **Extensibility** | Plugin architecture mirrors WordPress hooks/filters pattern, translated to .NET events and middleware pipelines. |
| **Performance** | Output caching, Redis, minification, lazy loading, and SQL Server query optimisation are built in from day one. |

---

## 2. Technology Stack

### 2.1 Core Platform (Backend)

| Layer | Technology | Version | Purpose |
|---|---|---|---|
| Runtime | .NET | 9.0 LTS | Application host, DI container, middleware pipeline |
| Web Framework | ASP.NET Core MVC | 9.0 | Controllers, Razor Views, Tag Helpers, Model Binding |
| ORM | Entity Framework Core | 9.0 | Database access, migrations, change tracking |
| Database | Microsoft SQL Server | 2022 / Azure SQL | Primary relational data store |
| Caching | Redis (StackExchange.Redis) | latest | Distributed cache, session, output cache |
| Background Jobs | Hangfire | 1.8+ | Scheduled tasks, recurring jobs, retry queues |
| Validation | FluentValidation | 11+ | Command/model validation with fluent rules |
| Mapping | Mapster | 7+ | Fast DTO/entity mapping |
| Mediator | MediatR | 12+ | CQRS commands, queries, domain event pipeline |
| Logging | Serilog + Seq | latest | Structured logging with queryable log server |
| API Docs | Swashbuckle (Swagger) | latest | OpenAPI 3.0 documentation auto-generation |
| Testing | xUnit + Moq + FluentAssertions | latest | Unit, integration, and assertion framework |
| Containerisation | Docker + docker-compose | latest | Reproducible environments from Sprint 1 |

### 2.2 Front-End Stack

| Layer | Technology | Version | Purpose |
|---|---|---|---|
| Core JS | jQuery | 3.7+ | DOM manipulation, AJAX, event handling |
| UI Components | jQuery UI | 1.14+ | Drag-and-drop, sortable, dialogs, accordions |
| Block Builder DnD | jQuery UI Sortable | bundled | Block drag-and-drop canvas in page builder |
| Rich Text Editor | TinyMCE | 7+ | WYSIWYG content editor |
| Code Editor | CodeMirror | 6+ | Custom CSS editor and HTML block editor |
| File Uploads | Dropzone.js | 6+ | Drag-and-drop file upload with preview |
| Data Tables | DataTables.js | 2+ | Server-side paginated admin tables |
| Date/Time | Flatpickr | 4+ | Lightweight date/time picker for scheduling |
| Notifications | Toastr.js | latest | Non-blocking toast notifications |
| Charts (Admin) | Chart.js | 4+ | Analytics dashboards in admin panel |
| Modals/Dialogs | jQuery UI Dialog | bundled | Confirmation dialogs, media picker modals |
| Select | Select2 | 4+ | Enhanced select with search and tagging |
| Icon Set | Tabler Icons (SVG) | 3+ | Consistent icon system (SVG sprites) |
| CSS Framework | Bootstrap | 5.3 | Responsive grid and utility classes for admin UI |

### 2.3 Architecture Patterns

- **Clean Architecture** — Core, Application, Infrastructure, Presentation layers with strict dependency rules (outer layers depend on inner; never reverse).
- **Repository Pattern** — `IRepository<T>` generic base with entity-specific repository interfaces. Unit of Work wraps repositories per request.
- **CQRS via MediatR** — Commands mutate state; Queries read state. Separate models for reads and writes.
- **Plugin Architecture** — `IPlugin` interface, `PluginRegistry`, event hooks (`IEventDispatcher`), and admin UI injection points. NuGet-based distribution.
- **Domain Events** — Published via MediatR notifications after aggregate changes. Handlers are decoupled and async.
- **Specification Pattern** — `ISpecification<T>` for composable query logic. Avoids leaking business rules into repositories.

---

## 3. Solution Structure (Clean Architecture)

```
EnterpriseCMS.sln
├── src/
│   ├── EnterpriseCMS.Core              # Domain entities, interfaces, domain events, value objects
│   ├── EnterpriseCMS.Application       # Use cases: Commands, Queries, Handlers, DTOs, Validators
│   ├── EnterpriseCMS.Infrastructure    # EF Core DbContext, Repositories, SQL Server, Email, Storage, Cache
│   ├── EnterpriseCMS.Web               # ASP.NET MVC Controllers, Razor Views, ViewModels, Middleware
│   ├── EnterpriseCMS.Plugins.Core      # Plugin contracts: IPlugin, IPluginHook, PluginMetadata, HookRegistry
│   └── EnterpriseCMS.Api               # Headless REST API controllers (Phase 2)
├── tests/
│   ├── EnterpriseCMS.UnitTests         # Core + Application layer unit tests
│   ├── EnterpriseCMS.IntegrationTests  # Repository, Service, Controller integration tests
│   └── EnterpriseCMS.E2ETests          # Playwright end-to-end browser tests
├── plugins/                            # Sample and built-in plugin assemblies
└── scripts/                            # Database seed scripts, migration helpers, dev tooling
```

### 3.1 Core Layer (`EnterpriseCMS.Core`)

Contains **only**: domain entities, domain events, value objects, repository interfaces, service interfaces, and common exceptions. Zero dependencies on EF Core, ASP.NET, or any infrastructure library.

- **Entities:** `BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `TenantId`, `IsDeleted` — soft delete on all entities)
- **Domain Events:** `IDomainEvent`, raised inside aggregates, dispatched after persistence via MediatR notifications
- **Interfaces:** `IRepository<T>`, `IUnitOfWork`, `ICurrentUserService`, `IDateTimeService`, `ICacheService`, `IEmailService`, `IStorageService`, `IPluginRegistry`
- **Enums/Constants:** `ContentStatus`, `UserRole`, `MediaType`, `PermissionLevel`, `PublishState`
- **Exceptions:** `NotFoundException`, `ValidationException`, `UnauthorizedException`, `DuplicateException`

### 3.2 Application Layer (`EnterpriseCMS.Application`)

Contains commands, queries, handlers, DTOs, validators. References **only** Core. No EF, no HTTP.

- **Commands:** `CreatePageCommand`, `PublishContentCommand`, `UploadMediaCommand`, `CreateUserCommand`, etc.
- **Queries:** `GetPageBySlugQuery`, `GetContentListQuery`, `GetMediaByIdQuery`, `GetUserByIdQuery`, etc.
- **Handlers:** Implement `IRequestHandler<TCommand, TResult>` from MediatR
- **Validators:** One `FluentValidation AbstractValidator<T>` per command
- **DTOs:** `PageDto`, `ContentDto`, `MediaDto`, `UserDto` — flat read models for views and API
- **Behaviours:** `LoggingBehaviour`, `ValidationBehaviour`, `CachingBehaviour`, `AuditBehaviour` (MediatR pipeline)

### 3.3 Infrastructure Layer (`EnterpriseCMS.Infrastructure`)

Implements all interfaces defined in Core. References Core and Application.

- **`CmsDbContext : DbContext`** — all `DbSet`s, global query filters (`TenantId`, `IsDeleted`), `SaveChanges` audit hook
- **Repositories:** `GenericRepository<T> : IRepository<T>`, entity-specific repositories (`IContentRepository`, `IMediaRepository`, etc.)
- **UnitOfWork:** Wraps `DbContext` transaction, commits all repositories atomically
- **Migrations:** EF Core code-first migrations, versioned, all tables include `TenantId` from day one
- **Services:** `EmailService` (SMTP), `StorageService` (local disk / Azure Blob switchable), `CacheService` (Redis)
- **PluginLoader:** Scans plugin directory, loads assemblies, registers `IPlugin` implementations into DI

### 3.4 Web Layer (`EnterpriseCMS.Web`)

ASP.NET MVC presentation layer. References Application layer only.

- **Areas:** `Admin` (backend CMS), `Site` (front-end rendered pages), `Api` (Phase 2 headless)
- **Controllers:** `AdminController`, `ContentController`, `MediaController`, `ThemeController`, `UserController`, `SettingsController`, `PluginController`
- **ViewModels:** Separate from DTOs — contain display logic, validation attributes for forms
- **Tag Helpers:** `BlockRendererTagHelper`, `MediaPickerTagHelper`, `PermissionTagHelper`, `TenantTagHelper`
- **Middleware:** `TenantResolutionMiddleware`, `CurrentUserMiddleware`, `PerformanceLoggingMiddleware`, `MaintenanceModeMiddleware`
- **Filters:** `AuditActionFilter`, `PermissionFilter`, `ThrottleFilter`

---

## 4. Database Design Principles

### 4.1 Universal Column Rules

> **Every table in the system must include the following columns. No exceptions.**

| Column | Type | Rule |
|---|---|---|
| `Id` | `UNIQUEIDENTIFIER` (`NEWSEQUENTIALID()`) | Primary key, sequential GUID for index performance |
| `TenantId` | `UNIQUEIDENTIFIER NULL` | NULL = root/system tenant. Phase 2 activates multi-tenancy without schema changes |
| `CreatedAt` | `DATETIME2 NOT NULL DEFAULT GETUTCDATE()` | UTC timestamps always |
| `UpdatedAt` | `DATETIME2 NOT NULL DEFAULT GETUTCDATE()` | Auto-updated via EF Core `SaveChanges` override |
| `CreatedBy` | `UNIQUEIDENTIFIER NULL FK Users.Id` | Nullable: system/seed operations have no user |
| `UpdatedBy` | `UNIQUEIDENTIFIER NULL FK Users.Id` | Last modifier |
| `IsDeleted` | `BIT NOT NULL DEFAULT 0` | Soft delete. Global EF Core query filter excludes `IsDeleted=1` |
| `DeletedAt` | `DATETIME2 NULL` | Set when `IsDeleted` flipped to 1 |
| `RowVersion` | `ROWVERSION` | Optimistic concurrency on all entities |

### 4.2 Core Tables Summary

| Table | Purpose | Key Columns |
|---|---|---|
| `Tenants` | Multi-tenant registry | Name, Slug, Domain, PlanId, IsActive |
| `Users` | All user accounts | Email, PasswordHash, DisplayName, AvatarUrl, IsLocked |
| `UserRoles` | M:M Users to Roles | UserId, RoleId, TenantId |
| `Roles` | Permission roles | Name, Slug, IsSystemRole, Permissions (JSON) |
| `Contents` | Pages, posts, all content types | Title, Slug, ContentType, Status, PublishAt, ExpiresAt, Blocks (JSON), AuthorId |
| `ContentVersions` | Full version history | ContentId, VersionNumber, Blocks (JSON), ChangeSummary, CreatedBy |
| `ContentMeta` | SEO & custom fields per content | ContentId, Key, Value, MetaType |
| `Media` | All uploaded files | FileName, StoragePath, MimeType, Size, Width, Height, AltText, FolderId |
| `MediaFolders` | Folder hierarchy for DAM | Name, ParentFolderId, Path |
| `Themes` | Installed theme registry | Name, Slug, Version, IsActive, ScreenshotPath |
| `ThemeSettings` | Per-site theme config | ThemeId, Key, Value, TenantId |
| `Menus` | Navigation menu registry | Name, Slug, Location, TenantId |
| `MenuItems` | Menu item tree | MenuId, ParentId, Title, Url, ContentId, Order, Target |
| `Widgets` | Widget instances in areas | AreaSlug, PluginWidgetId, Settings (JSON), Order, TenantId |
| `Plugins` | Installed plugin registry | AssemblyName, Version, IsEnabled, Settings (JSON), TenantId |
| `AuditLogs` | Tamper-proof audit trail | EntityType, EntityId, Action, OldValues (JSON), NewValues (JSON), UserId, IpAddress |
| `Settings` | Site-wide key-value settings | Key, Value, Group, IsEncrypted, TenantId |
| `Tags` | Taxonomy tags | Name, Slug, Description, TenantId |
| `ContentTags` | M:M Contents to Tags | ContentId, TagId |
| `Categories` | Hierarchical categories | Name, Slug, ParentId, Description, TenantId |
| `Comments` | Content comments | ContentId, AuthorName, AuthorEmail, Body, Status, ParentId |
| `RefreshTokens` | JWT refresh tokens | UserId, Token, ExpiresAt, IsRevoked, DeviceFingerprint |
| `EmailTemplates` | Transactional email templates | Slug, Subject, HtmlBody, TextBody, Variables (JSON) |
| `Notifications` | User notifications | UserId, Type, Title, Body, IsRead, Link |
| `ScheduledJobs` | Hangfire-tracked custom jobs | JobName, Payload (JSON), ScheduledAt, Status |

### 4.3 Repository Pattern Implementation

**Generic Repository Interface (Core Layer):**

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<T?> GetSingleAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);      // Soft delete: sets IsDeleted=true
    Task HardDeleteAsync(T entity, CancellationToken ct = default);  // Physical delete (admin only)
}
```

**Unit of Work Interface (Core Layer):**

```csharp
public interface IUnitOfWork
{
    IContentRepository Contents { get; }
    IMediaRepository Media { get; }
    IUserRepository Users { get; }
    IMenuRepository Menus { get; }
    IPluginRepository Plugins { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

**Generic Repository Implementation (Infrastructure Layer):**

```csharp
public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CmsDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(CmsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec, CancellationToken ct = default)
        => await SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec).ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}
```

### 4.4 Global EF Core Query Filters

```csharp
protected override void OnModelCreating(ModelBuilder mb)
{
    // Apply soft-delete and tenant filter to ALL BaseEntity-derived types
    foreach (var entityType in mb.Model.GetEntityTypes())
    {
        if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
        {
            // 1. Soft delete filter — never return deleted records
            mb.Entity(entityType.ClrType)
              .HasQueryFilter(e => !EF.Property<bool>(e, "IsDeleted"));
        }
    }
    // 2. Tenant filter activated in Phase 2 when TenantResolutionMiddleware
    //    sets ITenantContext.CurrentTenantId per request.
}
```

---

## 5. Plugin Architecture

The plugin system is inspired by WordPress hooks/filters but implemented as a type-safe .NET event pipeline. Plugins are .NET class libraries distributed as NuGet packages or DLL drops into a `/plugins` directory. The infrastructure is wired in from **Sprint 1** even with zero plugins installed.

### 5.1 Core Plugin Interfaces

```csharp
// EnterpriseCMS.Plugins.Core

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    string Author { get; }
    string Description { get; }
    void ConfigureServices(IServiceCollection services);
    void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    void OnActivate(IPluginContext context);    // Called when plugin is enabled
    void OnDeactivate(IPluginContext context);  // Called when plugin is disabled
}

public interface IPluginHook<TEvent> where TEvent : IPluginEvent
{
    int Priority { get; }  // Lower = runs earlier. Default = 10 (same as WordPress)
    Task HandleAsync(TEvent evt, CancellationToken ct);
}

// Built-in events plugins can hook into:
// IContentPublishedEvent, IContentSavedEvent, IMediaUploadedEvent,
// IUserCreatedEvent, IUserLoginEvent, IPageRenderingEvent (allows HTML injection)
```

### 5.2 Admin UI Injection

Plugins can register the following objects to extend the admin UI without modifying core code:

- **`AdminMenuItem`** — adds items to the admin sidebar navigation
- **`DashboardWidget`** — renders Razor partial views in the admin dashboard
- **`SettingsSection`** — injects configuration panels into Settings pages
- **`BlockType`** — adds custom blocks to the page builder block palette

### 5.3 Plugin Loader

```csharp
// EnterpriseCMS.Infrastructure

public class PluginLoader
{
    public IEnumerable<IPlugin> LoadPlugins(string pluginDirectory)
    {
        // 1. Scan /plugins directory for *.dll files
        // 2. Load each assembly into a PluginLoadContext (AssemblyLoadContext) for isolation
        // 3. Find types implementing IPlugin via reflection
        // 4. Instantiate and register in DI container
        // 5. Call ConfigureServices() then Configure()
        // 6. Persist plugin record to Plugins table in database
    }
}
```

### 5.4 Priority `int` Reference

| Priority Value | Meaning |
|---|---|
| 1 | Runs first (highest priority) |
| 10 | Default — same as WordPress default hook priority |
| 100 | Runs last (lowest priority) |

---

## 6. The 15 Feature Pillars

> **[MVP]** = built in Phases 1–3 (Months 1–6).  
> **[Phase N]** = built in the specified post-MVP phase.

---

### Pillar 1: Content Studio `[MVP]`

**Purpose:** Complete content creation, editing, versioning, and publishing workflow.

#### Feature 1.1 — Block-Based Page Builder `[MVP]`

A drag-and-drop visual page editor where all content is composed of typed blocks. Blocks are stored as a JSON array in `Contents.Blocks` (`NVARCHAR(MAX)`). The renderer converts JSON to HTML at render time, enabling both server-side Razor rendering and future headless API delivery.

- **Implementation:** jQuery UI Sortable on a `#block-canvas` div. Each block is a `<div data-block-id>` card. Dragging reorders the JSON array server-side on save.
- **Block Storage Format:**
  ```json
  [
    { "id": "uuid", "type": "richtext", "order": 1, "data": { "html": "<p>...</p>", "alignment": "left" } },
    { "id": "uuid", "type": "image",    "order": 2, "data": { "mediaId": "uuid", "alt": "...", "caption": "..." } }
  ]
  ```
- **Block Renderer:** `IBlockRenderer` interface with a concrete renderer per block type. Registered in DI. Called via `<cms-blocks blocks="@Model.Blocks" />` Tag Helper.
- **Block Palette:** Left sidebar listing all registered block types (built-in + plugin-contributed). Click or drag to canvas to insert.
- **Block Settings Panel:** Right sidebar populated via AJAX-loaded Razor partial per block type when a block is selected.
- **Save Strategy:** Auto-save every 30 seconds via jQuery AJAX POST to `/admin/content/autosave`. Full save on Ctrl+S or Save button.

#### Feature 1.2 — 8 Core Block Types `[MVP]`

| Block Type | Key Properties | Rendered Output |
|---|---|---|
| **Rich Text** | HTML content (TinyMCE), alignment | Sanitised HTML paragraph, headings, lists |
| **Image** | MediaId, AltText, Caption, Link URL, Width, Alignment | Responsive `<img>` with lazy loading, optional `<figure><figcaption>` |
| **Columns** | ColumnCount (1–4), GapSize, VerticalAlign, Children (nested blocks) | Bootstrap grid row/col wrapping child blocks |
| **Button / CTA** | Label, URL, Style (primary/secondary/outline), Size, Target | Styled `<a>` or `<button>` with configurable CSS classes |
| **Video** | URL (YouTube/Vimeo/upload), AutoPlay, Muted, Loop, Poster | Responsive iframe embed or HTML5 `<video>` |
| **HTML** | Raw HTML string | Unescaped HTML output — power user block |
| **Divider** | Height (px), Style (solid/dashed/dotted), Color, MarginTop/Bottom | Styled `<hr>` element |
| **Gallery** | MediaIds[], Columns (2–4), LightboxEnabled, CaptionMode | Responsive image grid with optional Fancybox lightbox |

#### Feature 1.3 — WYSIWYG Rich Text Editor `[MVP]`

TinyMCE 7 embedded in the Rich Text block and standalone for excerpts and widget text areas.

- **Toolbar:** H2/H3/H4, bold, italic, underline, strikethrough, blockquote, ordered/unordered lists, link, image (opens Media Picker modal), table, horizontal rule, code, remove formatting.
- **Image insertion** triggers the Media Picker jQuery UI Dialog — no direct URL input.
- **Output sanitisation:** Server-side `HtmlSanitizer` NuGet package before storage. Allowed tags: `p, h2, h3, h4, strong, em, a, ul, ol, li, blockquote, code, pre, table, tr, td, th, img, hr, br`.

#### Feature 1.4 — Content Versioning `[MVP]`

- Every save (auto or manual) creates a `ContentVersion` row with full Blocks JSON snapshot.
- Version list shows: version number, timestamp, author, change summary (diff of block count/types).
- Visual diff: side-by-side block-level comparison, client-side jQuery DOM diff.
- Restore: copy version's Blocks JSON back to `Contents.Blocks`, create new version labelled `Restored from v{N}`.
- Retention: Keep last 50 versions per content item. Hangfire weekly prune job.

#### Feature 1.5 — Publishing Workflow `[MVP]`

| Status | Who Can Set | Description |
|---|---|---|
| Draft | Author, Editor, Admin | Saved but not visible on front-end |
| Pending Review | Author | Submitted for review — triggers email to Editors |
| Scheduled | Editor, Admin | Will publish at `Contents.PublishAt` (UTC) |
| Published | Editor, Admin | Visible on front-end |
| Unpublished | Editor, Admin | Was published, now hidden. Does not delete |
| Archived | Admin | Soft-archived — no longer in active content lists |

- **Scheduled publishing:** Hangfire recurring job every 1 minute checks `Status='Scheduled' AND PublishAt <= GETUTCDATE()`.
- **Content expiry:** Same job checks `ExpiresAt` and sets `Status='Unpublished'`.
- **Email notifications:** Workflow state changes trigger `EmailService` using template slug `content-status-changed`.

#### Feature 1.6 — Autosave and Recovery `[MVP]`

- Client: `setInterval(autoSave, 30000)`. POSTs block JSON to `/admin/content/{id}/autosave`.
- Server: Writes to `ContentVersions` with `IsAutosave=true`. Does not change `Contents.Status`.
- **Unsaved changes detection:** `beforeunload` event warns if `isDirty` jQuery flag is true.
- **Recovery prompt:** On open, if autosave version is newer than `Contents.UpdatedAt`, jQuery UI Dialog offers restore.

#### Feature 1.7 — Quick Edit and Bulk Edit `[MVP]`

- **Inline quick edit:** DataTables row click enables inline editing of: Title, Slug, Status, Author, PublishAt, Categories.
- **Bulk actions:** Checkbox select + dropdown: Publish, Unpublish, Move to Trash, Change Author, Add Tag.
- Bulk operations via single AJAX POST with `ContentId[]` array and operation type.

#### Feature 1.8 — Custom Fields `[MVP]`

- Stored in `ContentMeta` table as key-value pairs with `MetaType`: `text, number, date, boolean, url, color, select, media`.
- Admin defines field groups per `ContentType`. Groups stored as `Settings` JSON.
- Rendered as a "Custom Fields" meta box below the block builder canvas.
- `ContentDto.CustomFields` exposes as `Dictionary<string, object>` in the headless API (Phase 2).

#### Feature 1.9 — Multi-Language / i18n `[Phase 4]`

- `ContentTranslations` table: `ContentId, LanguageCode, Title, Slug, Blocks (JSON), Status, PublishAt`.
- Language switcher in admin content editor. Language selector on front-end.
- URL strategy: Subdirectory (`/en/`, `/bn/`) or subdomain — configurable per site.
- RTL support: `dir="rtl"` applied at `<html>` level for RTL languages.

#### Feature 1.10 — Content Import / Export `[Phase 4]`

- **Export:** Contents + ContentMeta + Media references as ZIP (XML manifest + media files).
- **Import:** Upload ZIP, validate manifest, resolve media conflicts, create contents in Draft status.
- **WordPress migration:** WordPress WXR XML import support.

---

### Pillar 2: Media & Assets `[MVP]`

#### Feature 2.1 — File Upload Service `[MVP]`

- **Dropzone.js** for drag-and-drop uploads in Media Library and inline Media Picker modal.
- Accepted types configurable: images (`jpg, png, gif, webp, svg`), documents (`pdf, docx, xlsx`), video (`mp4, webm`), audio (`mp3, wav`).
- Max file size configurable per tenant in Settings (default: 50 MB images, 500 MB video).
- **Storage:** `IStorageService` abstraction. Default: local disk at `/wwwroot/uploads/{tenantId}/{year}/{month}/`. Phase 4: Azure Blob Storage implementation.
- On upload: create `Media` record, trigger `IMediaUploadedEvent`.

#### Feature 2.2 — Image Processing Pipeline `[MVP]`

- **Library:** `SixLabors.ImageSharp` (MIT, pure .NET, no native dependencies).
- On every image upload, generate: Original, Large (1920px), Medium (800px), Thumbnail (300×300 crop center), WebP variants of each size.
- Processing via **Hangfire background job** triggered by `IMediaUploadedEvent` handler.
- Storage paths: `/uploads/{tenantId}/{year}/{month}/{filename}-{size}.{ext}` and matching `.webp` versions.
- `<picture>` Tag Helper in theme renders WebP with JPEG/PNG fallback automatically.

#### Feature 2.3 — Media Library UI `[MVP]`

- Grid view and list view toggle (jQuery controlled). Sort by: date, name, size, type.
- Folder tree sidebar: drag media to folders. Folder CRUD.
- **Media detail panel** (slide-in jQuery): filename, dimensions, size, alt text (editable), title, caption, copy URL, delete.
- **Media Picker modal:** jQuery UI Dialog. Supports single and multi-select. Invoked from block settings and TinyMCE.

#### Feature 2.4 — CDN Integration `[Phase 4]`

- `StorageService` CDN mode: uploads to Azure Blob + returns CDN URL (Azure CDN / Cloudflare).
- CDN URL rewriting: all image URLs in rendered content rewritten to CDN origin via `IContentRenderer` middleware.

#### Feature 2.5 — AI Alt-Text Generation `[Phase 5]`

- On image upload, if AI features enabled, call Azure Computer Vision or OpenAI Vision API.
- Generated alt text pre-populated in `Media.AltText` but remains user-editable.

---

### Pillar 3: Theme & Design `[MVP]`

#### Feature 3.1 — Razor-Based Theme Engine `[MVP]`

Themes are Razor class libraries packaged as NuGet packages or folder drops into `/themes`.

```
/themes/{ThemeName}/
├── Theme.json              # Manifest (name, version, author, screenshot, areas, slots)
├── Views/
│   ├── _Layout.cshtml      # Master layout
│   └── Partials/
│       ├── _Header.cshtml
│       ├── _Footer.cshtml
│       ├── _Sidebar.cshtml
│       └── _Navigation.cshtml
├── wwwroot/
│   ├── css/theme.css
│   └── js/theme.js
├── BlockViews/             # Block-specific render templates
│   ├── RichText.cshtml
│   ├── Image.cshtml
│   └── Columns.cshtml
└── WidgetViews/            # Widget render templates
    ├── RecentPosts.cshtml
    └── Categories.cshtml
```

- `ThemeLoader` service scans `/themes`, reads `Theme.json`, registers into `Themes` table.
- Theme activation sets `Settings['ActiveThemeSlug']` for the tenant. `ThemeViewLocationExpander` injects theme's Views path into Razor view discovery.
- **Non-destructive switching:** activating a new theme does not modify content.

#### Feature 3.2 — Three Starter Themes `[MVP]`

| Theme | Target | Key Design |
|---|---|---|
| **CMS-Clean** | Corporate / business | White background, minimal nav, sidebar option |
| **CMS-Portfolio** | Creative / agency | Full-bleed hero, masonry grid, dark header option |
| **CMS-Blog** | Blog / magazine | Reading-optimised typography, category sidebar, featured post hero |

#### Feature 3.3 — Custom Menu Builder `[MVP]`

- **Admin UI:** Drag-and-drop tree using jQuery UI Sortable nested lists. Add items from: Pages/Posts picker, Custom URL, Category, Tag, External Link.
- Menu locations defined in `Theme.json`. Admin assigns menus to locations.
- Front-end: `_Navigation.cshtml` renders active menu. Active state applied by comparing current URL.
- Mobile menu: Hamburger toggle via Bootstrap collapse + jQuery click handler.

#### Feature 3.4 — Widget Areas `[MVP]`

- Widget areas defined in `Theme.json`: `[{ "slug": "sidebar-primary", "name": "Primary Sidebar" }, ...]`
- Widget types: Recent Posts, Categories, Tags, Text/HTML, Search, Custom Menu, Media/Image.
- Admin: drag widgets from palette into areas; configure via jQuery UI Dialog settings form.
- Rendering: `@await Html.PartialAsync("_WidgetArea", "sidebar-primary")` in theme Razor files.

#### Feature 3.5 — Custom CSS Editor `[MVP]`

- Per-site CSS stored in `Settings['CustomCSS']`. Edited via CodeMirror with CSS syntax highlighting.
- Injected into `<head>` via `_Layout.cshtml` after theme CSS.

#### Feature 3.6 — Live Theme Customiser `[Phase 4]`

- Split-pane: settings form (colours, fonts, spacing) + live preview iframe.
- Settings POSTed via jQuery AJAX. Preview iframe reloads with preview CSS injected.
- Persisted to `ThemeSettings` table on Save.

---

### Pillar 4: Identity & Authentication `[MVP]`

#### Feature 4.1 — ASP.NET Core Identity + JWT `[MVP]`

- `ApplicationUser : IdentityUser<Guid>` extended with: `DisplayName, AvatarUrl, Bio, TenantId, IsActive, LastLoginAt, FailedLoginAttempts, LockoutReason`.
- **Password hashing:** Override `IPasswordHasher<ApplicationUser>` with Argon2id (`Konscious.Security.Cryptography`).
- **JWT:** `ITokenService` generates access tokens (15-min expiry) and refresh tokens (7-day expiry, stored in `RefreshTokens` table with device fingerprint).
- **Refresh token rotation:** Old token revoked on each use. Concurrent use detection flags the account.
- **Hybrid auth:** Admin panel uses `.AddCookie()`. API uses `.AddJwtBearer()`. Dual scheme in `Program.cs`.

#### Feature 4.2 — Role-Based Access Control (RBAC) `[MVP]`

| Built-in Role | Capabilities |
|---|---|
| **Super Admin** | Full system access including multi-tenancy, plugins, system settings |
| **Administrator** | Full site access: content CRUD, user management, theme, settings, plugins |
| **Editor** | Create/edit/publish/delete any content. Manage media. No user/settings management |
| **Author** | Create/edit own content only. Submit for review. Upload media |
| **Contributor** | Create/edit own content. Cannot publish (submits for review only) |
| **Subscriber** | Front-end login only. View gated content. No admin access |

- **Custom roles:** Admin creates custom roles with capability checkboxes from permissions matrix.
- **Permissions:** Defined as const strings in `PermissionConstants` (e.g. `content.publish`, `media.delete`, `users.manage`).
- **Authorization:** `[Authorize(Policy = "RequirePermission")]` + `PermissionRequirement` + `PermissionAuthorizationHandler`.
- **Claims:** User permissions embedded in JWT at login time. Refresh token re-issues JWT with current permissions.

#### Feature 4.3 — Login Security `[MVP]`

- **Rate limiting:** ASP.NET Core `RateLimiter` `SlidingWindowLimiter` on `/account/login` — 5 attempts per 10 minutes per IP.
- **Account lockout:** After 10 failed logins, locked for 30 minutes. Admin can unlock manually.
- **CAPTCHA:** Google reCAPTCHA v3 on login and registration. Score threshold configurable in Settings.
- **Password reset:** Secure token in `AspNetUserTokens`, expires 2 hours, single-use.
- **Login audit:** Every attempt logged to `AuditLogs` with IpAddress, UserAgent, Timestamp, Success flag.

#### Feature 4.4 — SSO (OAuth2 / OIDC / SAML) `[Phase 4]`

- OAuth2/OIDC: `Microsoft.AspNetCore.Authentication.OpenIdConnect`. Providers: Azure AD, Google Workspace, Okta.
- SAML 2.0: `Sustainsys.Saml2.AspNetCore` for enterprise SSO.
- JIT provisioning: First SSO login auto-creates user account with Subscriber role (configurable).

#### Feature 4.5 — MFA `[Phase 4]`

- **TOTP:** RFC 6238. QR code via `QRCoder` package for authenticator app setup.
- **Email OTP:** 6-digit code, expires 10 minutes.
- **SMS OTP:** Via configurable SMS gateway (Twilio adapter).
- **Backup codes:** 10 single-use recovery codes generated on MFA setup.

---

### Pillar 5: Security & Core Operations `[MVP]`

#### Feature 5.1 — HTTP Security Headers `[MVP]`

Implemented as `SecurityHeadersMiddleware` added to the ASP.NET Core pipeline in `Program.cs`:

```
Content-Security-Policy: default-src 'self'; script-src 'self' 'nonce-{nonce}';
                          style-src 'self' 'unsafe-inline'; img-src 'self' data: blob:;
                          font-src 'self'; frame-ancestors 'none'
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: camera=(), microphone=(), geolocation=()
Strict-Transport-Security: max-age=63072000; includeSubDomains; preload
```

> **CSP nonce:** Generated per-request via `INonceService`. Injected into `ViewData` for inline scripts in Razor views.

#### Feature 5.2 — OWASP Top 10 Protections `[MVP]`

| OWASP Risk | Mitigation |
|---|---|
| A01 Broken Access Control | `PermissionAuthorizationHandler` on all admin routes. Tests verify 403 per role. |
| A02 Cryptographic Failures | Argon2id passwords, AES-256 for encrypted settings, TLS 1.3, no sensitive data in logs |
| A03 Injection | All DB access via EF Core parameterised queries only. Raw SQL forbidden. `HtmlSanitizer` for content. |
| A04 Insecure Design | Clean Architecture enforces layered boundaries. No business logic in controllers. |
| A05 Security Misconfiguration | Secrets in environment variables / Azure Key Vault (never `appsettings.json`). HSTS enforced. |
| A06 Vulnerable Components | Dependabot alerts. Monthly NuGet package audit as Hangfire job. |
| A07 Auth Failures | Rate limiting, account lockout, CAPTCHA, refresh token rotation, concurrent session detection |
| A08 Software Integrity | Signed NuGet packages for plugins. Plugin loading validates assembly signature. |
| A09 Logging Failures | Structured Serilog to Seq. All security events logged with UserId, IpAddress, Timestamp, Action. |
| A10 SSRF | `HttpClient` factory with allowed-domain whitelist. No user-controlled URL parameters to server. |

#### Feature 5.3 — GDPR & Compliance `[MVP]`

- **Cookie consent banner:** `CookieConsent.js`. Categories: Necessary, Functional, Analytics, Marketing. Recorded in `ConsentLog` table.
- **GDPR settings page:** Admin configures consent categories, cookie descriptions, privacy policy URL.
- **Data erasure:** Account deletion hard-deletes personal data and anonymises audit trail entries.
- **Data export:** All user data exported as JSON/ZIP (GDPR data portability).

#### Feature 5.4 — Automated Backup `[MVP]`

- Hangfire recurring job: Daily at 02:00 UTC. `BACKUP DATABASE` T-SQL command to BAK file.
- Rotation: Keep last 7 daily, 4 weekly, 3 monthly. Older backups purged automatically.
- Storage: Configurable path (local or Azure Blob via `IStorageService`).
- Health check: `/health` endpoint includes last successful backup timestamp.

#### Feature 5.5 — Health Checks & Monitoring `[MVP]`

- **ASP.NET Core HealthChecks:** `/health` endpoint — SQL Server, Redis, disk space, last backup age.
- **Admin dashboard widget:** Health status. Unhealthy checks highlighted in red.
- **Serilog + Seq:** All requests logged with `RequestId, TenantId, UserId, Duration, StatusCode`.
- **Maintenance Mode:** `Settings['MaintenanceMode'] = true` serves maintenance page to non-admin users.

#### Feature 5.6 — WAF & Advanced Security `[Phase 4]`

- Azure Application Gateway WAF v2 or Cloudflare WAF in front of application.
- IP allowlist/blocklist stored in Settings. Checked in `SecurityMiddleware`.
- **Malware scanning:** ClamAV integration via ClamAV .NET client. Files quarantined pending scan result.

---

### Pillar 6: SEO & Discoverability `[MVP]`

#### Feature 6.1 — Meta & SEO Manager `[MVP]`

- Per-content SEO tab: Meta Title (60 char limit with live counter), Meta Description (160 char), Canonical URL override, robots meta.
- **Open Graph:** `og:title, og:description, og:image, og:type, og:url, og:site_name`. Rendered in `_Layout.cshtml <head>`.
- **Twitter Card:** `twitter:card, twitter:title, twitter:description, twitter:image`.
- **Fallbacks:** Empty SEO fields fall back to `Content.Title` and first 160 chars of first Rich Text block.

#### Feature 6.2 — XML Sitemap Generation `[MVP]`

- Auto-generated at `/sitemap.xml`. Includes all Published pages/posts with `LastModifiedDate` and `ChangeFrequency`.
- Sitemap index for large sites: splits into `/sitemap-pages-1.xml`, `/sitemap-posts-1.xml` (max 1000 URLs per file).
- Regenerated on content publish via `IContentPublishedEvent` handler. Cached in Redis with 1-hour TTL.

#### Feature 6.3 — Robots.txt Manager `[MVP]`

- `/robots.txt` served dynamically from `Settings['Robotstxt']`. Editable via Settings > SEO > Robots.txt.
- Default: `User-agent: *\nAllow: /\nSitemap: https://{domain}/sitemap.xml`

#### Feature 6.4 — Permalink Structure `[MVP]`

- URL patterns in `Settings['PermalinkStructure']`. Options: `/{slug}/`, `/posts/{slug}/`, `/blog/{year}/{month}/{slug}/`, custom.
- Slug auto-generated from title (lowercase, hyphenated, unique). Duplicate slugs get `-2`, `-3` suffix.
- **301 redirect manager:** Slug changes automatically add old slug to `Redirects` table. `RedirectMiddleware` checks all requests.

#### Feature 6.5 — Schema.org Structured Data `[MVP]`

- JSON-LD blocks injected into `<head>` by `ISchemaOrgBuilder` service.
- Content type determines schema: `Article`, `BlogPosting`, `WebPage`, `Product` (Phase 3), `Course` (Phase 5).
- Breadcrumb schema: `BreadcrumbList` generated from URL hierarchy.

---

### Pillar 7: User & Member Management `[MVP]`

#### Feature 7.1 — Admin User Management `[MVP]`

- DataTables.js user list: search, filter by role, sort by name/date/status. Server-side pagination.
- Add user form: Email, Display Name, Role, Send welcome email checkbox.
- Edit user panel: Change role, reset password, force logout (revoke all refresh tokens), lock/unlock.
- User activity log: Last 20 actions from `AuditLogs` per user.

#### Feature 7.2 — Front-End Member Portal `[MVP]`

- Registration: Email, Password, Display Name, optional profile fields. Email confirmation required.
- Login: Email + password. Remember me (30-day cookie). Forgot password link.
- Profile page: Edit display name, avatar upload, bio, change password, notification preferences.
- Account deletion: Self-service with GDPR data erasure.

#### Feature 7.3 — Social Login `[Phase 4]`

- Providers: Google, Microsoft (Azure AD), GitHub. Configured via Settings (ClientId, ClientSecret).
- On first social login: create Subscriber account. Associate with existing account if email matches.

---

### Pillar 8: Analytics & Reporting `[Phase 4]`

#### Feature 8.1 — Built-in Analytics `[Phase 4]`

- Track: page views, unique visitors (hashed IP), referrer, device type, browser, country (MaxMind GeoLite2).
- Storage: `PageViews` table partitioned by month.
- Admin widgets: Page views (Chart.js line), top pages (bar), referrers (table), devices (pie).
- Respects cookie consent (analytics category). Bot detection filters common crawlers.

#### Feature 8.2 — Google Analytics Integration `[Phase 4]`

- Settings: GA4 Measurement ID. Injected as `gtag.js` snippet only when enabled AND analytics cookie consent given.

#### Feature 8.3 — SEO Content Score `[Phase 4]`

- Per-content SEO health check: Title length, meta description, H1 in blocks, images have alt text, internal links, canonical set.
- Score stored in `ContentMeta` key `seo_score`. Recomputed on save.

---

### Pillar 9: Headless API & Developer Platform `[Phase 2]`

> The entire Headless API pillar is Phase 2. CQRS queries built in MVP are the foundation; API endpoints exposed in Phase 2.

#### Feature 9.1 — REST API `[Phase 2]`

- Base URL: `/api/v1/`. Area: `EnterpriseCMS.Web/Areas/Api/`
- Content endpoints: `GET /api/v1/contents`, `GET /api/v1/contents/{slug}`, `POST/PUT/DELETE` (authenticated).
- Auth: `X-Api-Key` header OR OAuth2 Client Credentials flow.
- Response envelope: `{ success: bool, data: T, errors: [], meta: { page, pageSize, totalCount } }`.

#### Feature 9.2 — GraphQL API `[Phase 2]`

- **Hot Chocolate** GraphQL library (.NET native). Schema-first design.
- Types: `ContentType`, `MediaType`, `MenuType`, `TaxonomyType`.
- Queries: `content(slug: String)`, `contentList(filter: ContentFilter)`, `media(id: ID)`.
- DataLoader pattern for N+1 prevention.

#### Feature 9.3 — Webhook System `[Phase 2]`

- `WebhookSubscriptions` table: `TenantId, Url, Events (JSON), Secret, IsActive`.
- On each event, `IWebhookDispatcher` queues delivery via Hangfire.
- Delivery: POST with `HMAC-SHA256` signature header (`X-CMS-Signature`). Retry 3× with exponential backoff.

#### Feature 9.4 — CLI Tool `[Phase 2]`

```bash
dotnet tool install -g enterprisecms-cli

cms content list
cms content publish {id}
cms user create
cms plugin install {name}
cms backup now
cms settings set {key} {value}
```

---

### Pillar 10: Plugin & Extension System `[Phase 2]`

> Core infrastructure (`IPlugin`, `IEventDispatcher`, `PluginLoader`) built in MVP Sprint 1. Plugin admin UI and marketplace in Phase 2.

#### Feature 10.1 — Plugin Admin UI `[Phase 2]`

- Installed plugins list: Name, Version, Author, Status, Settings, Deactivate/Activate toggle.
- Plugin upload: ZIP/DLL upload. Validates signature. Extracts to `/plugins`. Reloads `PluginLoader`.
- Plugin settings: Plugins implementing `IPluginSettings` render a Razor partial in their settings page.

#### Feature 10.2 — Built-in Extended Block Plugins `[Phase 2]`

| Plugin Block | Description |
|---|---|
| Hero Section | Full-width hero with background image/video, headline, subtitle, CTA |
| Testimonials | Quotes with author, title, avatar. Slider or grid layout |
| Pricing Table | 3-column pricing cards with feature lists and CTA buttons |
| Contact Form | Configurable form builder with email delivery |
| Google Maps | Embedded map with configurable marker and zoom |
| Recent Posts | Dynamic block pulling latest posts by category/tag |
| FAQ Accordion | jQuery UI Accordion-based collapsible Q&A list |
| Social Share | Share buttons for Facebook, Twitter/X, LinkedIn, WhatsApp |

---

### Pillar 11: Multi-Tenancy `[Phase 2]`

> Database schema is tenant-aware from day one (`TenantId` on all tables). Phase 2 activates `TenantResolutionMiddleware` and tenant query filter.

#### Feature 11.1 — Tenant Resolution `[Phase 2]`

- `TenantResolutionMiddleware`: Resolves tenant from subdomain (`tenant.cms.com`) or custom domain by querying `Tenants` table.
- `ITenantContext`: Scoped service. All repositories read `CurrentTenantId`. All inserts set `TenantId` via EF Core interceptor.

#### Feature 11.2 — Tenant Onboarding Wizard `[Phase 2]`

- Step 1: Site name, subdomain/domain.
- Step 2: Admin user email + password.
- Step 3: Choose starter theme.
- Step 4: Import sample content (optional).
- Creates: `Tenant` record, admin `User` with Administrator role, default `Settings`, installs selected theme.

#### Feature 11.3 — Network Admin `[Phase 2]`

- Super Admin only. Lists all tenants with status, plan, usage stats.
- Actions: Suspend tenant, reset admin password, view tenant audit log, clone site to new tenant.

#### Feature 11.4 — Staging & Blue/Green `[Phase 4]`

- Per-tenant staging environment. Content synced from production to staging via `StagingService`.
- Blue/green deploy: swap staging to production with one click. Old production preserved for 24 hours for rollback.

---

### Pillar 12: Integrations `[Phase 4]`

#### Feature 12.1 — SMTP Email Configuration `[Phase 4]`

- Settings > Email: SMTP host, port, username, password (AES-256 encrypted in `Settings` table), From Name, From Email.
- Test email button validates current config.
- **Email template system:** `EmailTemplates` table. `{{variable}}` Handlebars syntax replaced by `EmailService` at send time.

#### Feature 12.2 — Payment Gateway Integration `[Phase 3]`

- `IPaymentGateway` abstraction. Stripe and PayPal implementations.
- Used by eCommerce (Pillar 13) and LMS (Pillar 15).

#### Feature 12.3 — CRM Integration `[Phase 4]`

- HubSpot and Salesforce adapters via REST API.
- Form submission handler can push data to configured CRM.

#### Feature 12.4 — Email Marketing `[Phase 4]`

- Mailchimp and SendGrid integration. Subscribe/unsubscribe hooks on user registration.

---

### Pillar 13: eCommerce `[Phase 3]`

> Implemented as a built-in plugin. Activating adds product management, cart, checkout to the CMS.

#### Feature 13.1 — Product Management `[Phase 3]`

- Products are a `ContentType` with additional fields: `Price, SalePrice, SKU, StockQuantity, Weight, Dimensions, ProductImages, ProductStatus`.
- Product variations: `ProductVariants` table — each variant has own SKU, price, stock.
- Product-specific category taxonomy using existing `Categories` system.

#### Feature 13.2 — Cart & Checkout `[Phase 3]`

- Cart stored in Redis session per visitor (30-min TTL). `CartItem`: `ProductId, VariantId, Quantity, Price snapshot`.
- Checkout flow: Cart review → Shipping address → Shipping method → Payment → Confirmation.
- `Orders` table: `OrderItems (JSON), CustomerInfo, ShippingAddress, BillingAddress, PaymentStatus, FulfillmentStatus, Total, TaxAmount, DiscountCode`.

#### Feature 13.3 — Digital Downloads `[Phase 3]`

- Products can have downloadable files stored in MediaLibrary with access control.
- After payment confirmed (`IOrderPaidEvent`), generate signed temporary download URL (HMAC signed, 48-hour expiry).

#### Feature 13.4 — Coupons & Discounts `[Phase 3]`

- `Coupons` table: Code, DiscountType (percent/fixed), DiscountValue, MinOrderAmount, UsageLimit, ExpiresAt.
- Applied at checkout. Server-side validation only — never trust client-side coupon calculations.

#### Feature 13.5 — Multi-Currency & Tax `[Phase 3]`

- Currency configured per tenant. Exchange rates fetched from external API, cached in Redis.
- Tax rules: TaxZones table with country/region rules. Applied server-side at checkout.

---

### Pillar 14: Community & Forms `[Phase 4]`

#### Feature 14.1 — Comments System `[Phase 4]`

- `Comments` table: `ContentId, AuthorName, AuthorEmail, Body, Status (pending/approved/spam/trash), ParentId`.
- Moderation queue in admin. Approve/spam/trash actions.
- Akismet spam checking via API.
- Notifications: Author emailed on new comment. Admin notified on pending comment.

#### Feature 14.2 — Form Builder `[Phase 4]`

- Drag-and-drop form builder via jQuery UI Sortable. Field types: Text, Email, Phone, Textarea, Select, Checkbox, Radio, File Upload, Date, Hidden.
- Forms stored as `FormDefinitions` JSON. Submissions in `FormSubmissions` table.
- Email notification on submission. Configurable recipients and email template.
- Spam protection: Honeypot field + reCAPTCHA.
- GDPR: Consent checkbox. `FormSubmissions` auto-deleted per data retention settings.

#### Feature 14.3 — Notifications `[Phase 4]`

- `Notifications` table. Admin notification bell icon with unread count badge.
- Types: workflow events, new comments, system health alerts, plugin updates available.

---

### Pillar 15: LMS / eLearning `[Phase 5]`

> Implemented as a plugin. Adds Courses, Lessons, Quizzes, Enrolments as content types.

#### Feature 15.1 — Course & Lesson Management `[Phase 5]`

- Course is a `ContentType`. Lessons are child `Content` items linked via `CourseId`.
- Curriculum builder: Drag-and-drop lesson ordering within course (jQuery UI Sortable).
- Drip content: `LessonAvailableAfterDays` — lesson unlocks N days after enrolment.

#### Feature 15.2 — Progress Tracking `[Phase 5]`

- `LearnerProgress` table: `UserId, CourseId, LessonId, CompletedAt, QuizScore`.
- Progress bar on course page and lesson sidebar.
- Certificate: PDF generated via `QuestPDF` on course completion.

#### Feature 15.3 — Quizzes & Assessments `[Phase 5]`

- Question types: Multiple choice, true/false, short answer, file submission.
- Auto-grading for objective questions. Manual grading for subjective.
- SCORM 1.2 package import via custom SCORM runtime.

#### Feature 15.4 — Virtual Classroom `[Phase 5]`

- Zoom and Microsoft Teams meeting link integration per lesson.
- Meeting scheduled via Zoom/Teams API. Link embedded in lesson page.

---

## 7. Development Phases

---

### Phase 1 — Foundation & Infrastructure
**Months 1–2 | Sprints 1–4**  
**Goal:** Working .NET 9 application with authentication, basic content CRUD, media upload, one theme, and security baseline. Deployable to Docker from day one.

---

#### Sprint 1 — Architecture & Identity (Weeks 1–2)

1. Create .NET 9 solution with 6 projects: Core, Application, Infrastructure, Web, Plugins.Core, Tests.
2. Install NuGet packages: EF Core + SQL Server provider, FluentValidation, MediatR, Mapster, Serilog, Hangfire, StackExchange.Redis, Argon2id, HtmlSanitizer.
3. Design and create all database tables (Section 4). Every table includes `TenantId`, `IsDeleted`, audit columns.
4. Implement `GenericRepository<T>`, `UnitOfWork`, `CmsDbContext` with global soft-delete query filter.
5. Implement ASP.NET Identity with custom `ApplicationUser`. Configure Argon2id password hasher.
6. Implement JWT token service with refresh token rotation. Access: 15-min, Refresh: 7-day.
7. Create Admin area shell: `_AdminLayout.cshtml` with Bootstrap 5 sidebar + top nav. Tabler Icons SVG sprite.
8. Implement 5 default user roles with permission constants. Seed Super Admin on first run.
9. Wire `IPlugin`, `IEventDispatcher` infrastructure. Empty `PluginLoader`. Zero plugins — system is ready.
10. Docker: `Dockerfile` (multi-stage), `docker-compose.yml` (app + SQL Server 2022 + Redis).
11. GitHub Actions CI: build + run unit tests on push.

---

#### Sprint 2 — Content Core (Weeks 3–4)

1. Content CRUD: `CreateContentCommand`, `UpdateContentCommand`, `DeleteContentCommand`, `GetContentListQuery`, `GetContentByIdQuery` — all via MediatR.
2. Content list admin page: DataTables.js server-side with filters (status, author, type, category).
3. Content editor page: Left — editor canvas (block builder placeholder). Right — meta box panel (status, author, publish date, categories, tags, SEO, custom fields).
4. TinyMCE 7 integration: rich text block editor. Configure toolbar per Feature 1.3.
5. Version history: Every save creates `ContentVersion`. Version list sidebar. Restore button.
6. Autosave: jQuery `setInterval` AJAX POST every 30 seconds. Recovery prompt on open.
7. Custom fields: `ContentMeta` key-value storage. Admin-defined field groups per `ContentType`.
8. FluentValidation validators for all content commands. MediatR `ValidationBehaviour` pipeline.

---

#### Sprint 3 — Media Service & Theme Engine (Weeks 5–6)

1. Media upload: Dropzone.js + `/admin/media/upload` endpoint. `IStorageService` local disk implementation.
2. ImageSharp processing pipeline as Hangfire background job. Generate Original, Large, Medium, Thumbnail, WebP variants.
3. Media Library admin page: jQuery grid/list toggle. Folder tree (jQuery UI nested sortable). DataTables list view.
4. Media Picker modal: jQuery UI Dialog. Invoked from block settings and TinyMCE image plugin.
5. Razor Theme Engine: `ThemeLoader`, `ThemeViewLocationExpander`. `Theme.json` manifest spec.
6. **CMS-Clean** starter theme: Bootstrap 5 responsive layout. Header, footer, sidebar slots. Navigation partial.
7. `BlockRendererTagHelper`: `<cms-blocks blocks="@Model.Blocks" />` renders JSON blocks to HTML via `IBlockRenderer` registry.
8. Block renderers for all 8 MVP block types (Feature 1.2).

---

#### Sprint 4 — Security Baseline & First Deploy (Weeks 7–8)

1. `SecurityHeadersMiddleware`: All headers per Feature 5.1. CSP nonce generation.
2. Anti-CSRF: `AntiForgeryToken` on all POST forms + `ValidateAntiForgeryToken` on all POST actions.
3. Login rate limiting: ASP.NET Core `RateLimiter` `SlidingWindowLimiter` on `/account/login`.
4. CAPTCHA: reCAPTCHA v3 on login, registration, and all public forms.
5. Automated backup: Hangfire recurring job. SQL Server `BACKUP DATABASE` to local `/backups` folder.
6. Health check endpoint: `/health` — SQL Server, Redis, disk space, last backup age.
7. Maintenance mode: Settings-driven middleware. Admin bypass via role claim.
8. Serilog to Seq: All requests logged with `RequestId, UserId, TenantId, Duration, Status`.
9. Integration test suite: Auth flow, content CRUD, media upload, role access checks.

---

### Phase 2 — Core CMS Features
**Months 3–4 | Sprints 5–8**  
**Goal:** Full block builder, menus, widgets, publishing workflow, SEO module, 2 more themes. Usable by non-technical users to build complete websites.

---

#### Sprint 5 — Block Builder (Weeks 9–10)

1. Block builder canvas: jQuery UI Sortable on `#block-canvas`. Each block is a card with move handle, settings button, delete button.
2. Block palette sidebar: categorised list of block types. Click to append; drag to position.
3. Block settings panel: right sidebar. Populated via AJAX loading block-specific Razor partial by block type slug.
4. Add block: AJAX POST to `/admin/content/blocks/add` returns new block card HTML + updated JSON.
5. Reorder: Sortable `stop` event POSTs updated order array to `/admin/content/blocks/reorder`.
6. Delete block: confirm dialog (jQuery UI Dialog). AJAX DELETE. Canvas updates.
7. Columns block: nested Sortable instances within column containers. Recursive block rendering.
8. Gallery block: Dropzone.js + Media Picker for selecting multiple images. Fancybox lightbox.
9. Block storage: all operations maintain canonical JSON format (Feature 1.1).
10. Preview mode: `/admin/content/{id}/preview` opens front-end render in new tab with preview token.

---

#### Sprint 6 — Navigation & Widget Areas (Weeks 11–12)

1. Menu builder: jQuery UI Sortable nested list for drag-and-drop menu tree. Add items from content picker, custom URL, category.
2. Menu assignment: Assign menus to theme-defined locations via select dropdowns.
3. `_Navigation.cshtml` partial: renders assigned menu as Bootstrap 5 nav. Active class on current page.
4. Widget area admin: drag widgets from palette to registered theme areas. jQuery UI Sortable.
5. 8 built-in widget types: Recent Posts, Categories, Tags, Text/HTML (TinyMCE), Search, Custom Menu, Image, Archives.
6. Widget settings: jQuery UI Dialog form per widget type. Settings persisted as JSON in `Widgets.Settings`.
7. Custom CSS editor: Settings > Appearance > Custom CSS. CodeMirror 6 with CSS syntax highlighting.
8. Responsive preview pane: mobile/tablet/desktop buttons toggle iframe width in preview.

---

#### Sprint 7 — Publishing Workflow & Bulk Operations (Weeks 13–14)

1. Approval workflow: state machine via MediatR commands. Status transitions per role matrix (Feature 1.5).
2. Email notification on workflow state change: Hangfire + `EmailService` + `EmailTemplate` `content-status-changed`.
3. Scheduled publishing: Hangfire `RecurringJob` every 1 minute. `CheckAndPublishScheduledContent` job.
4. Content expiry: same job checks `ExpiresAt`.
5. Inline quick edit: DataTables editor mode on row click. Editable: Title, Slug, Status, PublishAt.
6. Bulk actions: checkbox select + dropdown. Operations: Publish, Unpublish, Delete, Move to Category, Add Tag.
7. Quick duplicate: Clone content item (new ID, title `(Copy)` suffix, Status=Draft).

---

#### Sprint 8 — SEO Module & Two More Themes (Weeks 15–16)

1. SEO meta box in content editor: Title, Description, Canonical, Robots, OG image picker.
2. XML sitemap: `SitemapController`. Redis cache 1-hour TTL. Invalidated on content publish.
3. `robots.txt`: dynamic from Settings. Admin editor page.
4. 301 redirect manager: admin UI for creating/managing redirects. `RedirectMiddleware` (runs before routing).
5. Schema.org JSON-LD: `ISchemaOrgBuilder` service generates Article / BlogPosting / WebPage per content type.
6. Breadcrumb component. Schema `BreadcrumbList` injected.
7. SEO-friendly permalinks: `ContentSlugService` handles generation, uniqueness, and redirect creation on slug change.
8. **CMS-Portfolio** theme: full-bleed hero, masonry gallery CSS layout, dark header, portfolio grid.
9. **CMS-Blog** theme: reading-optimised font stack (Georgia serif), category sidebar, featured image hero, estimated read time.

---

### Phase 3 — Polish, Performance & MVP Launch
**Months 5–6 | Sprints 9–12**

---

#### Sprint 9 — Performance Optimisation (Weeks 17–18)

1. .NET 9 Output Cache middleware: cache all public pages with 5-minute TTL. Cache tags per `ContentId` for invalidation on publish.
2. Redis: `IDistributedCache` for session, sitemap cache, rate limiter state.
3. CSS + JS bundling: `WebOptimizer` package. Fingerprinted URLs for cache busting.
4. GZIP + Brotli: `app.UseResponseCompression()` with both providers. Brotli preferred where supported.
5. Lazy loading: all `<img>` tags get `loading="lazy"` via Tag Helper. Placeholder blur-up technique.
6. SQL indexes: `Contents(TenantId, Status, PublishAt)`, `Contents(Slug)`, `Media(TenantId, FolderId)`, `AuditLogs(TenantId, CreatedAt)`.
7. EF Core no-tracking queries: all GET queries use `.AsNoTracking()` where entities are not mutated.
8. k6 load test: 500 concurrent users. P95 response < 500ms. Fix bottlenecks.

---

#### Sprint 10 — Compliance & Ops (Weeks 19–20)

1. GDPR consent banner: `CookieConsent.js`. Categories: Necessary, Analytics, Marketing. Server-side consent recording.
2. Cookie policy page: auto-generated from registered cookies.
3. Data erasure: `DeleteAccountCommand` anonymises personal data from Users, AuditLogs, Comments, FormSubmissions.
4. Data export: `ExportUserDataQuery` returns ZIP of all user data as JSON files.
5. Global exception handler middleware. User-friendly error pages (404, 500, 503). Errors logged, never leak stack traces.
6. Site migration export: ZIP of full site (Contents + Media + Settings + Theme config). Import wizard.
7. Debug mode: `Settings['DebugMode'] = true` shows Serilog output at `/admin/debug` (Super Admin only).
8. WCAG 2.1 AA audit: Axe accessibility scanner on all admin and front-end pages. Fix critical issues.

---

#### Sprint 11 — Admin UX & Onboarding (Weeks 21–22)

1. Onboarding wizard: 5-step wizard for new site setup. Step 1: Site name/URL. Step 2: Admin user. Step 3: Theme selection. Step 4: Sample content import. Step 5: Launch checklist.
2. Admin dashboard: customisable widget grid. Default widgets: Recent Content, Media Usage, Quick Draft, System Health, Traffic Overview.
3. Mobile-responsive admin: sidebar collapses to hamburger. DataTables scroll horizontally on small screens.
4. Admin dark mode: CSS custom properties. Toggle saved in user profile. Persists across sessions.
5. Notification system: Toastr.js toasts for admin actions. Database notification bell for workflow events.
6. User tour: Intro.js guided tour on first admin login. Highlights block builder, media library, publish button.

---

#### Sprint 12 — QA, Testing & Beta Release (Weeks 23–24)

1. Unit tests: 80%+ coverage on Application and Core layers. xUnit + Moq + FluentAssertions.
2. Integration tests: repository tests against SQL Server LocalDB. Controller tests with `WebApplicationFactory`.
3. E2E tests: Playwright .NET. Scenarios: create page, publish, check front-end. Create user. Upload media. Install theme.
4. Load test: k6 — 1000 concurrent users, 10-minute soak. Target: < 600ms P95, zero errors.
5. Security scan: OWASP ZAP automated scan. Fix all High/Medium findings.
6. Pen-test checklist: SQL injection (parameterised check), XSS (sanitiser check), CSRF (all forms), broken auth (role bypass), sensitive data exposure (log review).
7. Docker production build: multi-stage `Dockerfile`. `docker-compose.prod.yml` with environment variable secrets. No `appsettings` secrets in image.
8. Deployment runbook: pull image, run migrations (`dotnet-ef database update`), start containers, health check.
9. Beta release: internal beta with 3–5 pilot users. Feedback issue tracker. Hotfix window.

---

### Phase 4 — Enterprise Platform
**Months 7–10 | Sprints 13–20**

| Sprint | Focus | Key Deliverables |
|---|---|---|
| 13 | Headless REST API | Area/Api controllers, API key auth, pagination envelope, Swagger UI, rate limiting |
| 14 | GraphQL API | Hot Chocolate setup, Content/Media/Menu types, DataLoader, playground UI |
| 15 | Plugin Admin UI & Marketplace | Plugin list, upload, activate/deactivate, built-in 8 extended block plugins |
| 16 | Multi-Tenancy Activation | TenantResolutionMiddleware, EF tenant query filter, tenant admin dashboard, site cloning |
| 17 | Onboarding + White-Label | Tenant onboarding wizard, custom domain, white-label branding, per-tenant settings |
| 18 | SSO / MFA / Social Login | OIDC (Azure AD, Google), SAML, TOTP MFA, email OTP, social login (Google, GitHub) |
| 19 | Advanced Security + Compliance | WAF integration, IP allowlist, malware scan (ClamAV), full GDPR tooling, SOC2 audit log |
| 20 | Analytics + Form Builder + Comments | Built-in analytics, Chart.js admin dashboard, form builder, comments + moderation |

---

### Phase 5 — Ecosystem Expansion
**Months 11–15 | Sprints 21–30**

| Sprint Range | Feature Area | Key Deliverables |
|---|---|---|
| 21–22 | eCommerce Plugin | Product management, cart, checkout, Stripe/PayPal, digital downloads, orders |
| 23 | Subscriptions & Memberships | Recurring billing, membership tiers, content gating by membership level |
| 24–25 | LMS Plugin | Courses, lessons, quizzes, progress tracking, certificates, drip content |
| 26 | SCORM & Virtual Classroom | SCORM 1.2 import, Zoom/Teams meeting integration per lesson |
| 27 | PWA & Mobile Optimisation | Service worker, web manifest, offline cache, push notifications |
| 28 | AI Content Features | Claude/GPT content generation, AI SEO suggestions, semantic search (vector embeddings) |
| 29 | Multi-Region & CDN | Azure Blob + CDN asset delivery, geo-routing, high availability setup |
| 30 | Marketplace Launch | Public plugin and theme marketplace. Submission workflow, review, versioning |

---

## 8. Coding Standards & Conventions for AI Agent

> ⚠️ **MANDATORY — These rules apply to ALL generated code without exception.**

1. **NEVER** put connection strings, API keys, or secrets in `appsettings.json`. Use environment variables or `IConfiguration` bound from environment.
2. **NEVER** write raw SQL strings. All queries via EF Core LINQ or parameterised `SqlCommand`. No string interpolation in SQL.
3. **NEVER** catch `Exception` directly without rethrowing or logging. All catches use specific exception types.
4. **NEVER** use `DateTime.Now` — always `DateTime.UtcNow`. The application operates in UTC throughout.
5. **NEVER** skip cancellation tokens on async methods. All async methods accept `CancellationToken ct = default`.
6. **NEVER** add business logic to Controllers. Controllers: validate model state → call `_mediator.Send()` → return View/JSON.
7. **NEVER** make Infrastructure reference Application or Web layers. Dependency direction: `Web → Application → Core ← Infrastructure`.
8. **ALWAYS** use `ILogger<T>` for logging. **NEVER** use `Console.WriteLine` in production code.
9. **ALWAYS** use Mapster for entity↔DTO mapping. **NEVER** manually map properties in handlers.
10. **ALWAYS** validate commands with FluentValidation. **NEVER** validate in handlers or controllers directly.

### 8.1 Correct Controller Pattern

```csharp
[Authorize(Policy = Policies.CanEditContent)]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreateContentViewModel vm, CancellationToken ct)
{
    if (!ModelState.IsValid) return View(vm);

    var command = vm.Adapt<CreateContentCommand>(); // Mapster
    command.AuthorId = _currentUser.UserId;

    var result = await _mediator.Send(command, ct);

    TempData["Success"] = "Content created.";
    return RedirectToAction(nameof(Edit), new { id = result.Id });
}
```

### 8.2 jQuery Conventions

- All jQuery code goes in `$(document).ready(function(){ ... })` or `$(function(){ ... })`.
- Use event delegation for dynamically added elements: `$(document).on('click', '.delete-btn', handler)`.
- All AJAX calls use `$.ajax()` with explicit `type`, `url`, `data`, `contentType`, and `error` handler.
- CSRF token in all AJAX POST requests:
  ```javascript
  headers: {
      'RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
  }
  ```
- No inline `onclick=` attributes in HTML. All events bound in JS files.
- JS files: one file per admin page/feature (e.g. `/wwwroot/js/admin/content-editor.js`). Loaded via `@section Scripts { <script src="..."> }` in Razor views.

### 8.3 Razor View Conventions

- All views use strongly-typed models (`@model PageViewModel`). No `ViewBag`/`ViewData` for primary data.
- Shared admin layout: `Areas/Admin/Views/Shared/_AdminLayout.cshtml`. All admin views: `@{ Layout = "_AdminLayout"; }`.
- Partial views for reusable fragments. Convention: `_PartialName.cshtml` (leading underscore).
- Tag Helpers preferred over HTML helpers: `<asp:label>`, `<input asp-for="">`, `<select asp-for="">`.
- Razor syntax: `@Html.DisplayFor()` for display, `asp-for` attributes for forms. Never string-concatenate HTML in code.

### 8.4 Error Handling

- Domain errors: `NotFoundException`, `ValidationException`, `UnauthorizedException` in Core layer. Thrown from handlers.
- `ExceptionHandlerMiddleware`: Catches all unhandled exceptions. Maps to 404, 400, 403, 500. Logs to Serilog.
- **Never** return stack traces to the user. Log to Seq; serve friendly error page.
- Validation errors: FluentValidation returns `ValidationException` with `Errors` dictionary. MediatR `ValidationBehaviour` converts to `ModelState` errors.

### 8.5 Database Migration Rules

- One migration per sprint minimum. More for large schema changes.
- Naming: `YYYYMMDD_Description` (e.g. `20260515_AddTagsTable`).
- **NEVER** modify existing migrations that have been applied. Only add new ones.
- Seed data in `HasData()` only for: roles, permissions, default settings, email templates.
- All foreign keys have explicit cascade behaviour (prefer `Restrict` over `Cascade` to prevent accidental mass deletes).

---

## 9. Configuration Reference

### 9.1 appsettings.json Structure (Non-Secret Keys Only)

```json
{
  "ConnectionStrings": {
    "Default": "#{SQL_CONNECTION_STRING}#"
  },
  "Redis": {
    "ConnectionString": "#{REDIS_CONNECTION}#"
  },
  "Jwt": {
    "Issuer": "enterprisecms",
    "Audience": "enterprisecms-users",
    "ExpiryMinutes": 15
  },
  "Storage": {
    "Provider": "local",
    "BasePath": "/uploads",
    "MaxFileSizeMb": 50
  },
  "Hangfire": {
    "Dashboard": "/admin/jobs",
    "RequireAuth": true
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "SeqUrl": "#{SEQ_URL}#"
  },
  "Cms": {
    "DefaultLocale": "en-US",
    "AllowRegistration": true,
    "RequireEmailConfirmation": true
  }
}
```

### 9.2 Required Environment Variables

| Variable | Description |
|---|---|
| `SQL_CONNECTION_STRING` | Full SQL Server connection string including credentials |
| `REDIS_CONNECTION` | Redis connection string (`host:port,password=xxx`) |
| `JWT_SECRET` | 256-bit random string for JWT signing key |
| `SMTP_HOST` / `SMTP_USER` / `SMTP_PASSWORD` | Email server credentials |
| `RECAPTCHA_SITE_KEY` / `RECAPTCHA_SECRET` | Google reCAPTCHA v3 keys |
| `STORAGE_AZURE_CONNECTION` | Azure Blob Storage connection string (if using Azure) |
| `SEQ_URL` / `SEQ_API_KEY` | Structured logging server endpoint |
| `ENCRYPTION_KEY` | AES-256 key for encrypting sensitive Settings values |

---

## 10. Deliverables Checklist per Phase

### Phase 1 & 2 — MVP Deliverables (Months 1–6)

- [ ] Working .NET 9 ASP.NET MVC CMS running in Docker
- [ ] SQL Server database with all tables, indexes, and initial migrations
- [ ] JWT authentication with refresh token rotation and Argon2id password hashing
- [ ] 5 default roles with permission-based authorization
- [ ] Block-based page builder with 8 core block types and drag-and-drop
- [ ] TinyMCE rich text editor with media picker integration
- [ ] Media library with ImageSharp processing (resize, crop, WebP) and Dropzone.js
- [ ] Version history, autosave with recovery prompt, quick edit, bulk edit
- [ ] Publishing workflow: Draft → Review → Scheduled → Published → Archived
- [ ] 3 starter themes: CMS-Clean, CMS-Portfolio, CMS-Blog
- [ ] Custom menu builder and widget areas
- [ ] SEO module: Meta, OG, sitemap, robots.txt, redirects, Schema.org, breadcrumbs
- [ ] OWASP-hardened: CSP, HSTS, CSRF, rate limiting, CAPTCHA, brute-force protection
- [ ] GDPR consent banner, cookie manager, data erasure, data export
- [ ] Automated backup, health check endpoint, maintenance mode
- [ ] Output caching (Redis), minification, GZIP/Brotli, lazy loading
- [ ] Admin onboarding wizard for new site setup
- [ ] 80%+ unit test coverage, Playwright E2E tests, k6 load test passing
- [ ] Docker production image with deployment runbook

### Phase 4 Deliverables (Months 7–10)

- [ ] Headless REST + GraphQL API with Swagger docs
- [ ] Webhook delivery system with HMAC signatures
- [ ] CLI management tool
- [ ] Plugin admin UI with 8 extended block plugins
- [ ] Multi-tenancy: tenant isolation, onboarding wizard, custom domains, network admin
- [ ] SSO: Azure AD, Google Workspace (OIDC), SAML 2.0
- [ ] MFA: TOTP, email OTP with backup codes
- [ ] Social login: Google, Microsoft, GitHub
- [ ] Built-in analytics dashboard with Chart.js
- [ ] Comment system with moderation and Akismet
- [ ] Form builder with email notifications and GDPR controls

### Phase 5 Deliverables (Months 11–15)

- [ ] eCommerce plugin: products, cart, checkout, Stripe, digital downloads, subscriptions
- [ ] LMS plugin: courses, lessons, quizzes, progress tracking, certificates, SCORM import
- [ ] PWA: service worker, offline cache, push notifications
- [ ] AI features: content generation, SEO suggestions, semantic search
- [ ] CDN integration and multi-region support
- [ ] Public plugin and theme marketplace

---

*End of Enterprise CMS Development Plan v1.0*  
*15 Pillars | 100+ Features | 5 Phases | 30 Sprints | 15 Months*
