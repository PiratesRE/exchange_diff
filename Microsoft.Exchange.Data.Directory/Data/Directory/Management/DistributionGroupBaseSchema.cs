using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal abstract class DistributionGroupBaseSchema : MailEnabledRecipientSchema
	{
		public static readonly ADPropertyDefinition ExpansionServer = ADGroupSchema.ExpansionServer;

		public static readonly ADPropertyDefinition ReportToManagerEnabled = ADGroupSchema.ReportToManagerEnabled;

		public static readonly ADPropertyDefinition ReportToOriginatorEnabled = ADGroupSchema.ReportToOriginatorEnabled;

		public static readonly ADPropertyDefinition SendOofMessageToOriginatorEnabled = ADGroupSchema.SendOofMessageToOriginatorEnabled;

		public static readonly ADPropertyDefinition DefaultDistributionListOU = ADRecipientSchema.DefaultDistributionListOU;
	}
}
