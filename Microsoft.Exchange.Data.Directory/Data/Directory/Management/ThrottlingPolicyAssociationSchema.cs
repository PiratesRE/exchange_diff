using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ThrottlingPolicyAssociationSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADRecipientSchema>();
		}

		public static readonly ADPropertyDefinition ObjectId = ADObjectSchema.Id;

		public static readonly ADPropertyDefinition ThrottlingPolicyId = ADRecipientSchema.ThrottlingPolicy;
	}
}
