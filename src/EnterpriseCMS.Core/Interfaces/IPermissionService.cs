namespace EnterpriseCMS.Core.Interfaces;

public static class Permissions
{
    public static class Content
    {
        public const string View   = "content.view";
        public const string Create = "content.create";
        public const string Edit   = "content.edit";
        public const string Delete = "content.delete";
        public const string Publish = "content.publish";
    }
    public static class Media
    {
        public const string View   = "media.view";
        public const string Upload = "media.upload";
        public const string Delete = "media.delete";
    }
    public static class Users
    {
        public const string View   = "users.view";
        public const string Create = "users.create";
        public const string Edit   = "users.edit";
        public const string Delete = "users.delete";
    }
    public static class Settings
    {
        public const string View = "settings.view";
        public const string Edit = "settings.edit";
    }
}

public static class RoleNames
{
    public const string SuperAdmin    = "SuperAdmin";
    public const string Administrator = "Administrator";
    public const string Editor        = "Editor";
    public const string Author        = "Author";
    public const string Subscriber    = "Subscriber";
}
