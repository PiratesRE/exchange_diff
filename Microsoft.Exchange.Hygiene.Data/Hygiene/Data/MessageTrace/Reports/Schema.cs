using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class Schema : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		internal static readonly HygienePropertyDefinition ActionListQueryDefinition = new HygienePropertyDefinition("tvp_ActionList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition SummarizeByQueryDefinition = new HygienePropertyDefinition("tvp_SummarizeBy", typeof(DataTable));

		internal static readonly HygienePropertyDefinition AggregateByQueryDefinition = new HygienePropertyDefinition("AggregateBy", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClientMessageIdQueryDefinition = new HygienePropertyDefinition("ClientMessageId", typeof(string));

		internal static readonly HygienePropertyDefinition DirectionListQueryDefinition = new HygienePropertyDefinition("tvp_MailDirectionList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition DomainListQueryDefinition = new HygienePropertyDefinition("tvp_DomainList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition DataSourceListQueryDefinition = new HygienePropertyDefinition("tvp_DataSourceList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition EndDateQueryDefinition = new HygienePropertyDefinition("EndDate", typeof(DateTime), DateTime.MaxValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndDateKeyQueryDefinition = new HygienePropertyDefinition("EndDateKey", typeof(int), int.MaxValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndDatetimeQueryDefinition = new HygienePropertyDefinition("EndDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition EndHourKeyQueryDefinition = new HygienePropertyDefinition("EndHourKey", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventListQueryDefinition = new HygienePropertyDefinition("tvp_EventList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition EventTypeListQueryDefinition = new HygienePropertyDefinition("tvp_EventTypeList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition FromIPAddressQueryDefinition = new HygienePropertyDefinition("FromIPAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdQueryDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MalwareListQueryDefinition = new HygienePropertyDefinition("tvp_MalwareList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MessageIdListQueryDefinition = new HygienePropertyDefinition("tvp_ClientMessageIdList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition OrganizationQueryDefinition = new HygienePropertyDefinition("organizationalUnitRoot", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PageQueryDefinition = new HygienePropertyDefinition("PageNumber", typeof(int), 1, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PageSizeQueryDefinition = new HygienePropertyDefinition("PageSize", typeof(int), 1000, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyListQueryDefinition = new HygienePropertyDefinition("tvp_PolicyList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition RecipientAddressQueryDefinition = new HygienePropertyDefinition("RecipientAddress", typeof(string));

		internal static readonly HygienePropertyDefinition RecipientAddressListQueryDefinition = new HygienePropertyDefinition("tvp_RecipientAddressList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition RuleListQueryDefinition = new HygienePropertyDefinition("tvp_RuleList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition SenderAddressQueryDefinition = new HygienePropertyDefinition("SenderAddress", typeof(string));

		internal static readonly HygienePropertyDefinition SenderAddressListQueryDefinition = new HygienePropertyDefinition("tvp_SenderAddressList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition StartDateQueryDefinition = new HygienePropertyDefinition("StartDate", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartDateKeyQueryDefinition = new HygienePropertyDefinition("StartDateKey", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartDatetimeQueryDefinition = new HygienePropertyDefinition("StartDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition StartHourKeyQueryDefinition = new HygienePropertyDefinition("StartHourKey", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ToIPAddressQueryDefinition = new HygienePropertyDefinition("ToIPAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MailDeliveryStatusListDefinition = new HygienePropertyDefinition("tvp_MailDeliveryStatusList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition TransportRuleListQueryDefinition = new HygienePropertyDefinition("tvp_TransportRuleList", typeof(DataTable));
	}
}
