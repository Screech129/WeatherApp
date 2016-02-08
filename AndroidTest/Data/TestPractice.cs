using System;
using NUnit.Framework;

namespace AndroidTest
{
	[TestFixture]
	public class TestPractice
	{
		
        
		[SetUp]
		public void Setup ()
		{
				
		}


		[TearDown]
		public void Tear ()
		{
				
		}

		[Test]
		public void Test_Demonstrate_Assertions ()
		{
			int a = 5;
			int b = 3;
			int c = 5;
			int d = 10;

			Assert.AreEqual (a, c, "X should be equal");
			Assert.IsTrue (d > a, "Y should be true");
			Assert.IsFalse (a == b, "Z should be false");

			if (b > d) {
				Assert.Fail ("XX should never happen");
			}
		}
	}
    
}

