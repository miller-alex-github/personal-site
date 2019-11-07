using System.Reflection;

namespace Ma.Web.Services
{
    /// <summary>
    /// Represents a service to manage the version of app.
    /// </summary>
    public interface IAppVersionService
    {
        string Version { get; }
    }

    public class AppVersionService : IAppVersionService
    {
        public string Version =>
            Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
