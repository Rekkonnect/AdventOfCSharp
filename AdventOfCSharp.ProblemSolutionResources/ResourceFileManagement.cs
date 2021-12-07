using System.IO;

namespace AdventOfCSharp.ProblemSolutionResources;

public static class ResourceFileManagement
{
    public static string GetProjectBaseDirectory()
    {
        var directoryName = $@"{nameof(AdventOfCSharp)}.{nameof(ProblemSolutionResources)}";
        // This solution will work for as long as the project structure does not include any deeper nesting
        // No idea how abstractable this solution can be for other projects
        // TODO: Explore resource storages
        var solutionDirectory = Directory.GetParent(ResourceFileHelpers.GetBaseCodeDirectory())!.FullName;
        return $@"{solutionDirectory}\{directoryName}";
    }

    public static void SetResourceProjectAsBaseProblemFileDirectory()
    {
        ProblemFiles.CustomBaseDirectory = GetProjectBaseDirectory();
    }
}
