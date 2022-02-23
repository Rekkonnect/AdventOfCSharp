using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.CodeFixes.Tests.FinalDayUsage;

[TestClass]
public class AoCS0011_CodeFixTests : FinalDayUserCodeFixTests
{
    [DataTestMethod]
    [DataRow("int")]
    [DataRow("string")]
    [DataRow("ulong")]
    public void SingleFinalDayCodeFix(string type)
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day25 : {{|*:Problem<{type}, string>|}}
{{
    public override {type} SolvePart1() => default;
    public override string SolvePart2() => default;
}}
";

        var fixedCode =
$@"
namespace AoC.Year2021;

public class Day25 : FinalDay<{type}>
{{
    public override {type} SolvePart1() => default;
}}
";
        TestCodeFixWithUsings(testCode, fixedCode);
    }

    [DataTestMethod]
    [DataRow("int")]
    [DataRow("string")]
    [DataRow("ulong")]
    public void MultipleYearsCodeFix(string type)
    {
        var testCode =
$@"
namespace AoC.Year2020
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
namespace AoC.Year2021
{{
    public class Day25 : {{|*:Problem<{type}, string>|}}
    {{
        public override {type} SolvePart1() => default;
        public override string SolvePart2() => default;
    }}
}}
namespace AoC.Year2022
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
";

        var fixedCode =
$@"
namespace AoC.Year2020
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
namespace AoC.Year2021
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
namespace AoC.Year2022
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
";
        TestCodeFixWithUsings(testCode, fixedCode);
    }

    [DataTestMethod]
    [DataRow("int")]
    [DataRow("string")]
    [DataRow("ulong")]
    public void MultipleYearsBatchCodeFix(string type)
    {
        var testCode =
$@"
namespace AoC.Year2020
{{
    public class Day25 : {{|*:Problem<{type}, string>|}}
    {{
        public override {type} SolvePart1() => default;
        public override string SolvePart2() => default;
    }}
}}
namespace AoC.Year2021
{{
    public class Day25 : {{|*:Problem<{type}, string>|}}
    {{
        public override {type} SolvePart1() => default;
        public override string SolvePart2() => default;
    }}
}}
namespace AoC.Year2022
{{
    public class Day25 : {{|*:Problem<{type}, string>|}}
    {{
        public override {type} SolvePart1() => default;
        public override string SolvePart2() => default;
    }}
}}
";

        var fixedCode =
$@"
namespace AoC.Year2020
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
namespace AoC.Year2021
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
namespace AoC.Year2022
{{
    public class Day25 : FinalDay<{type}>
    {{
        public override {type} SolvePart1() => default;
    }}
}}
";
        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
