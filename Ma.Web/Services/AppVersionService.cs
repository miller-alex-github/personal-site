using System.Reflection;

namespace Ma.Web.Services
{
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
