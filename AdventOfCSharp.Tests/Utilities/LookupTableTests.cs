using AdventOfCSharp.Utilities;
using NUnit.Framework;

namespace AdventOfCSharp.Tests.Utilities;

public class LookupTableTests
{
    [Test]
    public void TestValueContainer()
    {
        var table = new LookupTable<int>(1, 10);
        Assert.AreEqual(10, table.Count);

        SetAssert(1, 3);
        SetAssert(10, 4);
        SetAssert(5, 6);

        void SetAssert(int index, int value)
        {
            table[index] = value;
            Assert.AreEqual(value, table[index]);
        }
    }
}
