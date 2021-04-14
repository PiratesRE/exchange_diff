using System;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageCommonSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition EventIdProperty = CommonMessageTraceSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition ClientMessageIdProperty = new HygienePropertyDefinition("ClientMessageId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ReceivedProperty = new HygienePropertyDefinition("Received", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SenderAddressProperty = new HygienePropertyDefinition("SenderAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RecipientAddressProperty = new HygienePropertyDefinition("RecipientAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSubjectProperty = new HygienePropertyDefinition("MessageSubject", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSizeProperty = new HygienePropertyDefinition("MessageSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MailDirectionProperty = new HygienePropertyDefinition("MailDirection", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition QuarantineTypeProperty = new HygienePropertyDefinition("QuarantineType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PartNameProperty = new HygienePropertyDefinition("PartName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MimeNameProperty = new HygienePropertyDefinition("MimeName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ExpiresProperty = new HygienePropertyDefinition("Expires", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NotifiedProperty = new HygienePropertyDefinition("Notified", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition QuarantinedProperty = new HygienePropertyDefinition("Quarantined", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ReleasedProperty = new HygienePropertyDefinition("Released", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ReportedProperty = new HygienePropertyDefinition("Reported", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartDateQueryProperty = new HygienePropertyDefinition("StartDate", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndDateQueryProperty = new HygienePropertyDefinition("EndDate", typeof(DateTime), SqlDateTime.MaxValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainListQueryProperty = new HygienePropertyDefinition("tvp_DomainList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition MailDirectionListQueryProperty = new HygienePropertyDefinition("tvp_MailDirectionList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition ClientMessageListQueryProperty = new HygienePropertyDefinition("tvp_ClientMessageIdList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition SenderAddressListQueryProperty = new HygienePropertyDefinition("tvp_SenderAddressList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition RecipientAddressListQueryProperty = new HygienePropertyDefinition("tvp_RecipientAddressList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition TransportRuleListQueryProperty = new HygienePropertyDefinition("tvp_TransportRuleList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition QuarantinedUserAddressListQueryProperty = new HygienePropertyDefinition("tvp_QuarantinedUserAddressList", typeof(DataTable));

		internal static readonly HygienePropertyDefinition StartExpireDateQueryProperty = new HygienePropertyDefinition("StartExpireDate", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndExpireDateQueryProperty = new HygienePropertyDefinition("EndExpireDate", typeof(DateTime), SqlDateTime.MaxValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PageNumberQueryProperty = new HygienePropertyDefinition("PageNumber", typeof(int), 1, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = new HygienePropertyDefinition("PageSize", typeof(int), 100, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
