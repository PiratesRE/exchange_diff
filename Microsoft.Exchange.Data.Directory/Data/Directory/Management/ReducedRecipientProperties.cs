using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal static class ReducedRecipientProperties
	{
		public static ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return ReducedRecipientProperties.recipientSchema.AllProperties;
			}
		}

		private static ReducedRecipientSchema recipientSchema = ObjectSchema.GetInstance<ReducedRecipientSchema>();
	}
}
