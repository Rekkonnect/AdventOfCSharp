using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;
using System.Resources;

namespace AdventOfCSharp.Analyzers;

internal sealed class AoCSDiagnosticDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static readonly AoCSDiagnosticDescriptorStorage Instance = new();

    protected override string BaseRuleDocsURI => "https://github.com/AlFasGD/AdventOfCSharp/blob/master/docs/analyzers/rules";
    protected override string DiagnosticIDPrefix => "AoCS";
    protected override ResourceManager ResourceManager => DiagnosticStringResources.ResourceManager;

    #region Category Constants
    public const string BrevityCategory = "Brevity";
    public const string ConventionCategory = "Convention";
    public const string DesignCategory = "Design";
    public const string InformationCategory = "Information";
    public const string ValidityCategory = "Validity";
    public const string BestPracticeCategory = "Best Practice";
    #endregion

    #region Rules
    private AoCSDiagnosticDescriptorStorage()
    {
        SetDefaultDiagnosticAnalyzer<PartSolutionAnalyzer>();

        CreateDiagnosticDescriptor(0001, ValidityCategory);
        CreateDiagnosticDescriptor(0002, ValidityCategory);

        SetDefaultDiagnosticAnalyzer<ProblemInheritanceAnalyzer>();

        CreateDiagnosticDescriptor(0003, BrevityCategory);

        SetDefaultDiagnosticAnalyzer<ProblemClassNamingAnalyzer>();

        CreateDiagnosticDescriptor(0004, ConventionCategory);
        CreateDiagnosticDescriptor(0005, ConventionCategory);
        CreateDiagnosticDescriptor(0006, ConventionCategory);
        CreateDiagnosticDescriptor(0007, ConventionCategory);

        SetDefaultDiagnosticAnalyzer<SecretsContainerAnalyzer>();

        CreateDiagnosticDescriptor(0008, ValidityCategory);

        SetDefaultDiagnosticAnalyzer<FinalDayAnalyzer>();

        CreateDiagnosticDescriptor(0011, DesignCategory);
        CreateDiagnosticDescriptor(0012, ValidityCategory);

        SetDefaultDiagnosticAnalyzer<PartSolverAttributeAnalyzer>();

        CreateDiagnosticDescriptor(0013, ValidityCategory);
        CreateDiagnosticDescriptor(0014, ValidityCategory);
        CreateDiagnosticDescriptor(0015, InformationCategory);

        SetDefaultDiagnosticAnalyzer<AnswerStringConverterAnalyzer>();

        CreateDiagnosticDescriptor(0016, ValidityCategory);
        CreateDiagnosticDescriptor(0017, ValidityCategory);
        CreateDiagnosticDescriptor(0018, ValidityCategory);

        SetDefaultDiagnosticAnalyzer<SecretStringPropertyAnalyzer>();

        CreateDiagnosticDescriptor(0080, ValidityCategory);
        CreateDiagnosticDescriptor(0081, DesignCategory);
        CreateDiagnosticDescriptor(0082, BestPracticeCategory);
        CreateDiagnosticDescriptor(0083, ValidityCategory);
        CreateDiagnosticDescriptor(0084, ValidityCategory);
    }

    protected override DiagnosticSeverity? GetDefaultSeverity(string category)
    {
        return category switch
        {
            ValidityCategory => DiagnosticSeverity.Error,
            ConventionCategory => DiagnosticSeverity.Error,
            DesignCategory => DiagnosticSeverity.Warning,
            BrevityCategory => DiagnosticSeverity.Info,
            InformationCategory => DiagnosticSeverity.Warning,
            BestPracticeCategory => DiagnosticSeverity.Info,
        };
    }
    #endregion
}
