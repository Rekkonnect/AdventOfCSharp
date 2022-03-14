using AdventOfCSharp.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCSharp.SourceGenerators.Utilities;

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

    public static IEnumerable<ProblemClassDeclarationCorrelation> GetProblemClassCorrelationsSymbols(Compilation compilation)
    {
        var globalNamespaces = GetAllAssemblySymbols(compilation).Select(GetGlobalNamespace).Where(gn => gn is not null) as IEnumerable<INamespaceSymbol>;
        var allTypes = globalNamespaces.SelectMany(gn => gn.GetAllContainedTypes());
        // Safe to ignore the current assembly's global namespace; it should not 
        return allTypes.Select(CorrelateProblemClass).Where(v => v is not null) as IEnumerable<ProblemClassDeclarationCorrelation>;
    }

    private static IEnumerable<ISymbol> GetAllAssemblySymbols(Compilation compilation)
    {
        return compilation.SourceModule.ReferencedAssemblySymbols.Concat(new[] { compilation.Assembly });
    }
    private static INamespaceSymbol? GetGlobalNamespace(ISymbol? symbol)
    {
        return symbol switch
        {
            IAssemblySymbol assemblySymbol => assemblySymbol.GlobalNamespace,
            IModuleSymbol moduleSymbol => moduleSymbol.GlobalNamespace,

            _ => null,
        };
    }

    private static ProblemClassDeclarationCorrelation? CorrelateProblemClass(ClassDeclarationSyntax declarationSyntax)
    {
        var className = declarationSyntax.Identifier.Text;
        var namespaceDeclaration = declarationSyntax.GetNearestParentOfType<BaseNamespaceDeclarationSyntax>();
        var rightmostIdentifier = namespaceDeclaration.Name.GetRightmostIdentifier();
        var namespaceLeafName = rightmostIdentifier.Text;

        return CorrelateProblemClass(declarationSyntax, className, namespaceLeafName);
    }
    private static ProblemClassDeclarationCorrelation? CorrelateProblemClass(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.GetIdentifiableSymbolKind() is not IdentifiableSymbolKind.Class)
            return null;

        var className = typeSymbol.Name;
        var namespaceLeafName = typeSymbol.ContainingNamespace.Name;

        return CorrelateProblemClass(typeSymbol, className, namespaceLeafName);
    }
    private static ProblemClassDeclarationCorrelation? CorrelateProblemClass(INamedTypeSymbol classSymbol, string className, string namespaceLeafName)
    {
        var dayMatch = dayPattern.Match(className);
        if (!dayMatch.Success)
            return null;

        var yearMatch = yearPattern.Match(namespaceLeafName);
        if (!yearMatch.Success)
            return null;

        int day = int.Parse(dayMatch.Groups["day"].Value);
        int year = int.Parse(yearMatch.Groups["year"].Value);
        return new(classSymbol, year, day);
    }
    private static ProblemClassDeclarationCorrelation? CorrelateProblemClass(ClassDeclarationSyntax declarationSyntax, string className, string namespaceLeafName)
    {
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
    public ClassDeclarationSyntax? DeclarationSyntax { get; }
    public INamedTypeSymbol? ClassSymbol { get; }

    public int Year { get; }
    public int Day { get; }

    public string FullSymbolName
    {
        get
        {
            if (DeclarationSyntax is not null)
                return DeclarationSyntax.FullDeclaredSymbolName();

            return ClassSymbol!.GetFullSymbolName()!.FullNameString;
        }
    }

    internal ProblemClassDeclarationCorrelation(ClassDeclarationSyntax? declarationSyntax, INamedTypeSymbol? classSymbol, int year, int day)
    {
        DeclarationSyntax = declarationSyntax;
        ClassSymbol = classSymbol;
        Year = year;
        Day = day;
    }
    internal ProblemClassDeclarationCorrelation(ClassDeclarationSyntax declarationSyntax, int year, int day)
        : this(declarationSyntax, null, year, day) { }
    internal ProblemClassDeclarationCorrelation(INamedTypeSymbol classSymbol, int year, int day)
        : this(classSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as ClassDeclarationSyntax, classSymbol, year, day)
    {
    }

    public string FullNamespace(string baseNamespace)
    {
        return $"{baseNamespace}.Year{Year}";
    }

    internal sealed class Comaprer : IComparer<ProblemClassDeclarationCorrelation>
    {
        public static readonly Comaprer Default = new();

        public int Compare(ProblemClassDeclarationCorrelation x, ProblemClassDeclarationCorrelation y)
        {
            int yearComparison = x.Year.CompareTo(y.Year);
            if (yearComparison is not 0)
                return yearComparison;

            return x.Day.ToString().CompareTo(y.Day.ToString());
        }
    }
}
