using System;
using System.Data.SqlTypes;
using System.Net;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTraceSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ClientMessageIdProperty = new HygienePropertyDefinition("ClientMessageId", typeof(string));

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition DirectionProperty = new HygienePropertyDefinition("Direction", typeof(MailDirection));

		internal static readonly HygienePropertyDefinition FromEmailPrefixProperty = new HygienePropertyDefinition("FromEmailPrefix", typeof(string));

		internal static readonly HygienePropertyDefinition FromEmailDomainProperty = new HygienePropertyDefinition("FromEmailDomain", typeof(string));

		internal static readonly HygienePropertyDefinition IPAddressProperty = new HygienePropertyDefinition("IPAddress", typeof(IPAddress));

		internal static readonly HygienePropertyDefinition IP1Property = new HygienePropertyDefinition("IP1", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP2Property = new HygienePropertyDefinition("IP2", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP3Property = new HygienePropertyDefinition("IP3", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP4Property = new HygienePropertyDefinition("IP4", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP5Property = new HygienePropertyDefinition("IP5", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP6Property = new HygienePropertyDefinition("IP6", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP7Property = new HygienePropertyDefinition("IP7", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP8Property = new HygienePropertyDefinition("IP8", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP9Property = new HygienePropertyDefinition("IP9", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP10Property = new HygienePropertyDefinition("IP10", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP11Property = new HygienePropertyDefinition("IP11", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP12Property = new HygienePropertyDefinition("IP12", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP13Property = new HygienePropertyDefinition("IP13", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP14Property = new HygienePropertyDefinition("IP14", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP15Property = new HygienePropertyDefinition("IP15", typeof(byte?));

		internal static readonly HygienePropertyDefinition IP16Property = new HygienePropertyDefinition("IP16", typeof(byte?));

		internal static readonly HygienePropertyDefinition ReceivedTimeProperty = new HygienePropertyDefinition("ReceivedTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartTimeQueryProperty = new HygienePropertyDefinition("startTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndTimeQueryProperty = new HygienePropertyDefinition("endTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
