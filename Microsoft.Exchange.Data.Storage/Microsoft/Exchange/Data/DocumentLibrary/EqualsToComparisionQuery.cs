using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EqualsToComparisionQuery : ComparisionQuery<object>
	{
		internal EqualsToComparisionQuery(int index, object propValue) : base(index, propValue)
		{
		}

		public override bool IsMatch(object[] row)
		{
			return object.Equals(this.PropValue, row[this.Index]);
		}
	}
}
