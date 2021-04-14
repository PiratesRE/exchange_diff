using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tracking
{
	internal class MessageTrackingSharedResultSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition MessageTrackingReportId = new SimpleProviderPropertyDefinition("MessageTrackingReportId", ExchangeObjectVersion.Exchange2010, typeof(MessageTrackingReportId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SubmittedDateTime = new SimpleProviderPropertyDefinition("SubmittedDateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.TaskPopulated, DateTime.Today, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Subject = new SimpleProviderPropertyDefinition("Subject", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FromAddress = new SimpleProviderPropertyDefinition("FromAddress", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FromDisplayName = new SimpleProviderPropertyDefinition("FromDisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientAddresses = new SimpleProviderPropertyDefinition("RecipientAddresses", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientDisplayNames = new SimpleProviderPropertyDefinition("RecipientDisplayNames", ExchangeObjectVersion.Exchange2010, typeof(string[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
