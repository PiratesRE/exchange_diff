using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Constants
	{
		public const string IgnoreSubmitTimePrefix = "ed590c4ca1674effa0067475ab2b93b2_";

		public static readonly Guid ConsumerTenantGuid = new Guid("4af1e291-e4c1-419c-b986-3ccddb2fd556");
	}
}
