using AdventOfCSharp.CodeAnalysis.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCSharp.SourceGenerators.Utilities;

#nullable enable

// It is unknown to me whether an ISyntaxReceiver would be more appropriate
// Further exploration of source generators will determine this
public sealed class ProblemClassIdentifier
{
    private static readonly Regex dayPattern = new(@"Day(?'day'\d+)");
    private static readonly Regex yearPattern = new(@"Year(?'year'\d+)");

    private static readonly Dictionary<Compilation, ProblemClassIdentifier> cachedIdentifiers = new();

    private readonly Compilation compilation;
    private readonly Lazy<ProblemClassDeclarationCorrelationCollection> correlations;

    public ProblemClassIdentifier(Compilation compilation)
    {
        this.compilation = compilation;
        correlations = new(CalculateCorrelations);
    }

    private ProblemClassDeclarationCorrelationCollection CalculateCorrelations()
    {
        var globalNamespaces = compilation.GetAllAssemblySymbols().Select(IAssemblyOrModuleSymbolExtensions.GetGlobalNamespace).Where(gn => gn is not null) as IEnumerable<INamespaceSymbol>;
        var allTypes = globalNamespaces.SelectMany(gn => gn.GetAllContainedTypes());
#nullable disable
        return new(allTypes.Select(CorrelateProblemClass).Where(v => v is not null));
#nullable enable
    }
    public ProblemClassDeclarationCorrelationCollection GetProblemClassCorrelations()
    {
        return correlations.Value;
    }

    public static ProblemClassDeclarationCorrelationCollection GetProblemClassCorrelations(Compilation compilation)
    {
        bool contained = cachedIdentifiers.TryGetValue(compilation, out var identifier);
        if (!contained)
        {
            identifier = new(compilation);
            cachedIdentifiers.Add(compilation, identifier);
        }
        return identifier.GetProblemClassCorrelations();
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
}

public sealed class ProblemClassDeclarationCorrelationCollection
{
    private readonly Dictionary<ProblemDate, ProblemClassDeclarationCorrelation> dictionary = new();

    public IEnumerable<ProblemDate> Dates => dictionary.Keys;

    public IEnumerable<ProblemClassDeclarationCorrelation> Correlations => dictionary.Values;
    
    public ProblemClassDeclarationCorrelationCollection() { }
    public ProblemClassDeclarationCorrelationCollection(IEnumerable<ProblemClassDeclarationCorrelation> correlations)
    {
        AddRange(correlations);
    }

    public void Add(ProblemClassDeclarationCorrelation correlation)
    {
        var date = new ProblemDate(correlation.Year, correlation.Day);
        dictionary.Add(date, correlation);
    }
    public void AddRange(IEnumerable<ProblemClassDeclarationCorrelation> correlations)
    {
        foreach (var correlation in correlations)
            Add(correlation);
    }

    // For filtering, directly filter all the discovered declarations
    public IEnumerable<ProblemClassDeclarationCorrelation> OfDay(int day)
    {
        return Correlations.Where(correlation => correlation.Day == day);
    }
    public IEnumerable<ProblemClassDeclarationCorrelation> OfYear(int year)
    {
        return Correlations.Where(correlation => correlation.Year == year);
    }

    public ProblemClassDeclarationCorrelation this[int year, int day] => this[new(year, day)];
    public ProblemClassDeclarationCorrelation this[ProblemDate date]
    {
        get => dictionary[date];
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

    public IMethodSymbol? PartSolverMethodSymbol(int part)
    {
        return ClassSymbol?.GetAllMembersIncludingInherited().First(member => member.Name == $"SolvePart{part}") as IMethodSymbol;
    }

    public bool HasQualityAssertableParts(out bool part1, out bool part2)
    {
        part1 = IsQualityAssertablePart(1);
        part2 = IsQualityAssertablePart(2);
        return part1 || part2;
    }

    public bool IsQualityAssertablePart(int part)
    {
        if (Day is 25 && part is 2)
            return false;

        return IsValidPartSolution(part);
    }
    public bool IsValidPartSolution(int part)
    {
        var method = PartSolverMethodSymbol(part);
        if (method is null)
            return false;

        var partSolutionAttribute = method.FirstOrDefaultAttributeNamed<PartSolutionAttribute>();
        // No attribute defaults to a valid solution
        if (partSolutionAttribute is null)
            return true;

        var status = (PartSolutionStatus)partSolutionAttribute.ConstructorArguments[0].Value!;
        return status.IsValidSolution();
    }

    internal sealed class FileNameComparer : IComparer<ProblemClassDeclarationCorrelation>
    {
        public static readonly FileNameComparer Default = new();

        public int Compare(ProblemClassDeclarationCorrelation left, ProblemClassDeclarationCorrelation right)
        {
            int yearComparison = left.Year.CompareTo(right.Year);
            if (yearComparison is not 0)
                return yearComparison;

            return left.Day.ToString().CompareTo(right.Day.ToString());
        }
    }
}
