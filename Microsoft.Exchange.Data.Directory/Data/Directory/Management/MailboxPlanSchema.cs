using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailboxPlanSchema : MailboxSchema
	{
		public static readonly ADPropertyDefinition IsDefault = ADRecipientSchema.IsDefault;

		public static readonly ADPropertyDefinition IsDefault_R3 = ADRecipientSchema.IsDefault_R3;

		public static readonly ADPropertyDefinition MailboxPlanIndex = ADRecipientSchema.MailboxPlanIndex;

		public static readonly ADPropertyDefinition MailboxPlanRelease = ADRecipientSchema.MailboxPlanRelease;

		public static readonly ADPropertyDefinition IsPilotMailboxPlan = ADUserSchema.IsPilotMailboxPlan;
	}
}
