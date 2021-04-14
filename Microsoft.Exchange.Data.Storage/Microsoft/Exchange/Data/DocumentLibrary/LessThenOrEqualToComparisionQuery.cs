using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LessThenOrEqualToComparisionQuery : ComparisionQuery<IComparable>
	{
		internal LessThenOrEqualToComparisionQuery(int index, IComparable propValue) : base(index, propValue)
		{
		}

		public override bool IsMatch(object[] row)
		{
			return Utils.CompareValues(this.PropValue, row[this.Index]) >= 0;
		}
	}
}
