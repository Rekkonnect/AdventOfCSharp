using System.IO;

namespace AdventOfCSharp.Utilities;

// TODO: Migrate to Garyon
public static class AppDomainHelpers
{
    public static void ForceLoadAllAssemblies(this AppDomain domain)
    {
        // Graciously copied from some helpful guy on the internet
        // https://stackoverflow.com/a/2384679/11438007
        // This function should exist in the BCL
        var staticLoadedAssemblies = domain.GetAssemblies().Where(assembly => !assembly.IsDynamic);
        var staticLoadedPaths = staticLoadedAssemblies.Select(a => a.Location).ToArray();

        var referencedPaths = Directory.GetFiles(domain.BaseDirectory, "*.dll");
        var toLoad = referencedPaths.Except(staticLoadedPaths, StringComparer.InvariantCultureIgnoreCase).ToArray();

        var names = toLoad.Select(AssemblyName.GetAssemblyName);
        foreach (var assemblyName in names)
            domain.Load(assemblyName);
    }
    public static void ForceLoadAllAssembliesCurrent()
    {
        ForceLoadAllAssemblies(AppDomain.CurrentDomain);
    }
}
