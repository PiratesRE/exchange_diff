using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailContactSchema : MailEnabledOrgPersonSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADContactSchema>();
		}

		public static readonly ADPropertyDefinition ExternalEmailAddress = ADRecipientSchema.ExternalEmailAddress;

		public static readonly ADPropertyDefinition UsePreferMessageFormat = ADRecipientSchema.UsePreferMessageFormat;

		public static readonly ADPropertyDefinition MessageFormat = ADRecipientSchema.MessageFormat;

		public static readonly ADPropertyDefinition MessageBodyFormat = ADRecipientSchema.MessageBodyFormat;

		public static readonly ADPropertyDefinition MacAttachmentFormat = ADRecipientSchema.MacAttachmentFormat;

		public static readonly ADPropertyDefinition MaxRecipientPerMessage = ADRecipientSchema.RecipientLimits;

		public static readonly ADPropertyDefinition UseMapiRichTextFormat = ADRecipientSchema.UseMapiRichTextFormat;

		public static readonly ADPropertyDefinition UserCertificate = ADRecipientSchema.Certificate;

		public static readonly ADPropertyDefinition UserSMimeCertificate = ADRecipientSchema.SMimeCertificate;
	}
}
