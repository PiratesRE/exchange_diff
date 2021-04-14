using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageRecipientSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = QuarantinedMessageCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = QuarantinedMessageCommonSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition EmailPrefixProperty = new HygienePropertyDefinition("EmailPrefix", typeof(string));

		internal static readonly HygienePropertyDefinition EmailDomainProperty = new HygienePropertyDefinition("EmailDomain", typeof(string));

		internal static readonly HygienePropertyDefinition QuarantinedProperty = QuarantinedMessageCommonSchema.QuarantinedProperty;

		internal static readonly HygienePropertyDefinition NotifiedProperty = QuarantinedMessageCommonSchema.NotifiedProperty;

		internal static readonly HygienePropertyDefinition ReportedProperty = QuarantinedMessageCommonSchema.ReportedProperty;

		internal static readonly HygienePropertyDefinition ReleasedProperty = QuarantinedMessageCommonSchema.ReleasedProperty;

		public static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;
	}
}
