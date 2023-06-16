using System.Collections.Generic;
using Assets.Sun.Base;
using Assets.Sun.Task01;
using NUnit.Framework;

namespace Assets.Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void TestRangeWindow()
		{
			var set = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var window = RangeWindow.CreateFrom(set);

			window.Rebuild(1, 1, 2);
			window.GetEnumeratorRangeAll().ToEnumerable().ToText().Log();

			window.Rebuild(0, 0, 2);
			window.GetEnumeratorRangeAll().ToEnumerable().ToText().Log();
		}
	}
}
