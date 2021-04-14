using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExistsQuery : SinglePropertyQuery
	{
		internal ExistsQuery(int index) : base(index)
		{
		}

		public override bool IsMatch(object[] row)
		{
			PropertyError propertyError = row[this.Index] as PropertyError;
			return propertyError == null || (propertyError.PropertyErrorCode != PropertyErrorCode.NotFound && propertyError.PropertyErrorCode != PropertyErrorCode.NotSupported);
		}
	}
}
