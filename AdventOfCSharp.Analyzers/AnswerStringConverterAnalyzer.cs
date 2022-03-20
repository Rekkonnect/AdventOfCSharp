using AdventOfCSharp.CodeAnalysis.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AnswerStringConverterAnalyzer : ProblemAoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeAnswerStringConverter, SymbolKind.NamedType);
    }

    private void AnalyzeAnswerStringConverter(SymbolAnalysisContext context)
    {
        var classSymbol = context.Symbol as INamedTypeSymbol;

        if (classSymbol.TypeKind is not TypeKind.Class)
            return;

        var baseType = classSymbol.BaseType;
        if (baseType is null)
            return;

        var declaringSyntax = classSymbol.DeclaringSyntaxReferences.First().GetSyntax() as ClassDeclarationSyntax;
        var baseList = declaringSyntax.BaseList!;

        var baseTypeNode = baseList.Types.First();

        // Do not report on CommonAnswerStringConverter or AnswerStringConverter<TSource>
        var classFullSymbolName = classSymbol.GetFullSymbolName();
        if (classFullSymbolName.Matches(KnownFullSymbolNames.CommonAnswerStringConverter, SymbolNameMatchingLevel.Namespace))
            return;
        if (classFullSymbolName.Matches(KnownFullSymbolNames.GenericAnswerStringConverter, SymbolNameMatchingLevel.Namespace))
            return;

        var baseFullSymbolName = baseType.GetFullSymbolName();
        if (baseFullSymbolName.Matches(KnownFullSymbolNames.NonGenericAnswerStringConverter, SymbolNameMatchingLevel.Namespace))
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0016(baseTypeNode));
            return;
        }

        // If the base type is irrelevant to AnswerStringConverter<TSource>, ignore
        if (!baseFullSymbolName.Matches(KnownFullSymbolNames.GenericAnswerStringConverter, SymbolNameMatchingLevel.Namespace))
            return;

        var typeArgument = baseType.TypeArguments.Single();
        var specialType = typeArgument.SpecialType;
        switch (specialType)
        {
            case SpecialType.System_Object:
                context.ReportDiagnostic(Diagnostics.CreateAoCS0017(baseTypeNode));
                break;

            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_SByte:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_String:
                context.ReportDiagnostic(Diagnostics.CreateAoCS0018(baseTypeNode));
                break;
        }
    }
}
