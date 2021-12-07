using System.IO;

namespace AdventOfCSharp.Utilities;

// TODO: Migrate to Garyon
public static class AppDomainHelpers
{
    public static void ForceLoadAllAssemblies(this AppDomain domain)
    {
        // Graciously copied from some helpful guy on the internet
        // This function should exist in the BCL
        var staticLoadedAssemblies = domain.GetAssemblies().Where(assembly => !assembly.IsDynamic);
        var staticLoadedPaths = staticLoadedAssemblies.Select(a => a.Location).ToArray();

        var referencedPaths = Directory.GetFiles(domain.BaseDirectory, "*.dll");
        var toLoad = referencedPaths.Except(staticLoadedPaths, StringComparer.InvariantCultureIgnoreCase).ToArray();

        toLoad.Select(AssemblyName.GetAssemblyName).ForEach(domain.NoReturnLoad);
    }
    public static void ForceLoadAllAssembliesCurrent()
    {
        ForceLoadAllAssemblies(AppDomain.CurrentDomain);
    }

    public static void NoReturnLoad(this AppDomain domain, AssemblyName name)
    {
        domain.Load(name);
    }
}
