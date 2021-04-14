using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GreaterThenComparisionQuery : ComparisionQuery<IComparable>
	{
		internal GreaterThenComparisionQuery(int index, IComparable propValue) : base(index, propValue)
		{
		}

		public override bool IsMatch(object[] row)
		{
			return Utils.CompareValues(this.PropValue, row[this.Index]) < 0;
		}
	}
}
