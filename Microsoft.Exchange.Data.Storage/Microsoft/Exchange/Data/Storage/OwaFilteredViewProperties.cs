using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class OwaFilteredViewProperties
	{
		private const string PropertyNamePrefix = "http://schemas.microsoft.com/exchange/";

		private static readonly Guid publicStringsGuid = new Guid("00020329-0000-0000-C000-000000000046");

		public static readonly PropertyDefinition FilteredViewLabel = GuidNamePropertyDefinition.CreateCustom("FilteredViewLabel", typeof(string[]), OwaFilteredViewProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fldfltr", PropertyFlags.None);

		public static readonly PropertyDefinition FilteredViewAccessTime = GuidNamePropertyDefinition.CreateCustom("FilteredViewAccessTime", typeof(ExDateTime), OwaFilteredViewProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fltract", PropertyFlags.None);
	}
}
