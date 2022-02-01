using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class ExampleTest
{
    [Test]
    public void ExampleTestSimplePasses()
    {
    }

    [UnityTest]
    public IEnumerator ExampleTestWithEnumeratorPasses()
    {
        yield return null;
    }
}
