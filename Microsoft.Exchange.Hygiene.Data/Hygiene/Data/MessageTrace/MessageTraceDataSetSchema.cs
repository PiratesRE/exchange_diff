using System;
using System.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTraceDataSetSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition MessagesTableProperty = new HygienePropertyDefinition("tvp_Messages", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessagePropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageActionTableProperty = new HygienePropertyDefinition("tvp_MessageActions", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageActionPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageActionProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageClassificationsTableProperty = new HygienePropertyDefinition("tvp_MessageClassifications", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageClassificationPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageClassificationProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageClientInformationTableProperty = new HygienePropertyDefinition("tvp_MessageClientInformation", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageClientInformationPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageClientInformationProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventsTableProperty = new HygienePropertyDefinition("tvp_MessageEvents", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageEventProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventRulesTableProperty = new HygienePropertyDefinition("tvp_MessageEventRules", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventRulePropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageEventRuleProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventRuleClassificationsTableProperty = new HygienePropertyDefinition("tvp_MessageEventRuleClassifications", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventRuleClassificationPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageEventRuleClassificationProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventSourceItemsTableProperty = new HygienePropertyDefinition("tvp_MessageEventSourceItems", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageEventSourceItemPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageEventSourceItemProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageRecipientsTableProperty = new HygienePropertyDefinition("tvp_MessageRecipients", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageRecipientPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageRecipientProperties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageRecipientStatusTableProperty = new HygienePropertyDefinition("tvp_MessageRecipientStatus", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageRecipientStatusPropertiesTableProperty = new HygienePropertyDefinition("tvp_MessageRecipientStatusProperties", typeof(DataTable));
	}
}
