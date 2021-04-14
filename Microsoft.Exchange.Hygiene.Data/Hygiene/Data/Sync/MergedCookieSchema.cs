using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class MergedCookieSchema : BaseCookieSchema
	{
		public static readonly HygienePropertyDefinition ContextIdProperty = new HygienePropertyDefinition("ContextId", typeof(ADObjectId));
	}
}
