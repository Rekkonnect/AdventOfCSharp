using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCSharp.CodeAnalysis.Core;

#nullable enable

// It is unknown to me whether an ISyntaxReceiver would be more appropriate
// Further exploration of source generators will determine this
public static class ProblemClassIdentifier
{
    private static readonly Regex dayPattern = new(@"Day(?'day'\d+)");
    private static readonly Regex yearPattern = new(@"Year(?'year'\d+)");

    public static IEnumerable<ProblemClassDeclarationCorrelation> GetProblemClassCorrelations(SyntaxTree syntaxTree)
    {
        return GetProblemClassCorrelations(syntaxTree.NodesOfType<ClassDeclarationSyntax>());
    }
    public static IEnumerable<ProblemClassDeclarationCorrelation> GetProblemClassCorrelations(Compilation compilation)
    {
        return GetProblemClassCorrelations(compilation.NodesOfType<ClassDeclarationSyntax>());
    }
    public static IEnumerable<ProblemClassDeclarationCorrelation> GetProblemClassCorrelations(IEnumerable<ClassDeclarationSyntax> allClasses)
    {
        return allClasses.Select(CorrelateProblemClass).Where(v => v is not null) as IEnumerable<ProblemClassDeclarationCorrelation>;
    }

    private static ProblemClassDeclarationCorrelation? CorrelateProblemClass(ClassDeclarationSyntax declarationSyntax)
    {
        var className = declarationSyntax.Identifier.Text;
        var namespaceDeclaration = declarationSyntax.GetNearestParentOfType<BaseNamespaceDeclarationSyntax>();
        var rightmostIdentifier = namespaceDeclaration.Name.GetRightmostIdentifier();
        var namespaceLeafName = rightmostIdentifier.Text;
        
        var dayMatch = dayPattern.Match(className);
        if (!dayMatch.Success)
            return null;

        var yearMatch = yearPattern.Match(namespaceLeafName);
        if (!yearMatch.Success)
            return null;

        int day = int.Parse(dayMatch.Groups["day"].Value);
        int year = int.Parse(yearMatch.Groups["year"].Value);
        return new(declarationSyntax, year, day);
    }
}

public class ProblemClassDeclarationCorrelation
{
    public ClassDeclarationSyntax DeclarationSyntax { get; }

    public int Year { get; }
    public int Day { get; }

    internal ProblemClassDeclarationCorrelation(ClassDeclarationSyntax declarationSyntax, int year, int day)
    {
        DeclarationSyntax = declarationSyntax;
        Year = year;
        Day = day;
    } 

    public string FullNamespace(string baseNamespace)
    {
        return $"{baseNamespace}.Year{Year}";
    }
}
