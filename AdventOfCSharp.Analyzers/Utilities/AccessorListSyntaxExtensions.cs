using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class AccessorListSyntaxExtensions
{
    public static AccessorDeclarationSyntax? GetAccessor(this AccessorListSyntax? accessorList, AccessorKind accessorKind)
    {
        if (accessorList is null)
            return null;

        var targetSyntaxKind = accessorKind.GetRepresentingKeywordSyntaxKind();

        if (targetSyntaxKind is SyntaxKind.None)
            return null;

        foreach (var accessor in accessorList.Accessors)
        {
            if (accessor.Keyword.IsKind(targetSyntaxKind))
                return accessor;
        }

        return null;
    }
}

public enum AccessorKind
{
    None,

    Get,
    Set,
    Init,

    Add,
    Remove,
}

public static class AccessorKindExtensions
{
    public static SyntaxKind GetRepresentingKeywordSyntaxKind(this AccessorKind kind)
    {
        return kind switch
        {
            AccessorKind.None => SyntaxKind.None,

            AccessorKind.Get => SyntaxKind.GetKeyword,
            AccessorKind.Set => SyntaxKind.SetKeyword,
            AccessorKind.Init => SyntaxKind.InitKeyword,

            AccessorKind.Add => SyntaxKind.AddKeyword,
            AccessorKind.Remove => SyntaxKind.RemoveKeyword,
        };
    }
}
