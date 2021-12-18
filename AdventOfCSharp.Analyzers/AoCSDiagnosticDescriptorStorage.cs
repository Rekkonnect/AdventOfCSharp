using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;
using System.Resources;

namespace AdventOfCSharp.Analyzers;

internal sealed class AoCSDiagnosticDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static readonly AoCSDiagnosticDescriptorStorage Instance = new();

    protected override string BaseRuleDocsURI => "https://github.com/AlFasGD/AdventOfCSharp/blob/master/docs/rules";
    protected override string DiagnosticIDPrefix => "AoCS";
    protected override ResourceManager ResourceManager => DiagnosticStringResources.ResourceManager;

    #region Category Constants
    public const string APIRestrictionsCategory = "API Restrictions";
    public const string BrevityCategory = "Brevity";
    public const string DesignCategory = "Design";
    public const string InformationCategory = "Information";
    public const string ValidityCategory = "Validity";
    #endregion

    #region Rules
    private AoCSDiagnosticDescriptorStorage()
    {
        //SetDefaultDiagnosticAnalyzer<PartSolutionAnalyzer>();

        CreateDiagnosticDescriptor(0001, ValidityCategory, DiagnosticSeverity.Error);
        CreateDiagnosticDescriptor(0002, ValidityCategory, DiagnosticSeverity.Error);
    }
    #endregion
}
