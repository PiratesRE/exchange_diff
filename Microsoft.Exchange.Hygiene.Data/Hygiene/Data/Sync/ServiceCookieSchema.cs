using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class ServiceCookieSchema : BaseCookieSchema
	{
		public static readonly HygienePropertyDefinition LastCompletedTimeProperty = new HygienePropertyDefinition("LastCompletedTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
