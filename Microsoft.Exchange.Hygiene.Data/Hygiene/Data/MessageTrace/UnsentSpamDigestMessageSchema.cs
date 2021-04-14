using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnsentSpamDigestMessageSchema
	{
		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition SenderNameProperty = new HygienePropertyDefinition("SenderName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FromEmailPrefixProperty = new HygienePropertyDefinition("FromEmailPrefix", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FromEmailDomainProperty = new HygienePropertyDefinition("FromEmailDomain", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RecipientNameProperty = new HygienePropertyDefinition("RecipientName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ToEmailPrefixProperty = new HygienePropertyDefinition("ToEmailPrefix", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ToEmailDomainProperty = new HygienePropertyDefinition("ToEmailDomain", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SubjectProperty = new HygienePropertyDefinition("Subject", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSizeProperty = new HygienePropertyDefinition("MessageSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DateReceivedProperty = new HygienePropertyDefinition("DateReceived", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastNotifiedProperty = new HygienePropertyDefinition("LastNotifiedDateTime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition UpperBoundaryQueryProperty = new HygienePropertyDefinition("UpperBoundaryDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DefaultESNFrequencyQueryProperty = new HygienePropertyDefinition("DefaultESNFrequency", typeof(int), 3, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
