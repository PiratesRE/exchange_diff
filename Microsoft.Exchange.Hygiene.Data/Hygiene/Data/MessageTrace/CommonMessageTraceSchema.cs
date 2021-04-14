using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class CommonMessageTraceSchema
	{
		internal static readonly HygienePropertyDefinition PhysicalInstanceKeyProp = DalHelper.PhysicalInstanceKeyProp;

		internal static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		internal static readonly HygienePropertyDefinition HashBucketProperty = DalHelper.HashBucketProp;

		internal static readonly HygienePropertyDefinition EventIdProperty = new HygienePropertyDefinition("EventId", typeof(Guid));

		internal static readonly HygienePropertyDefinition ClassificationIdProperty = new HygienePropertyDefinition("ClassificationId", typeof(Guid));

		internal static readonly HygienePropertyDefinition RecipientIdProperty = new HygienePropertyDefinition("RecipientId", typeof(Guid));

		internal static readonly HygienePropertyDefinition EventRuleIdProperty = new HygienePropertyDefinition("EventRuleId", typeof(Guid));

		internal static readonly HygienePropertyDefinition EventRuleClassificationIdProperty = new HygienePropertyDefinition("EventRuleClassificationIdProperty", typeof(Guid));

		internal static readonly HygienePropertyDefinition SourceItemIdProperty = new HygienePropertyDefinition("SourceItemId", typeof(Guid));

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = new HygienePropertyDefinition("ExMessageId", typeof(Guid));

		internal static readonly HygienePropertyDefinition EventHashKeyProperty = new HygienePropertyDefinition("EventHashKey", typeof(byte[]));

		internal static readonly HygienePropertyDefinition EmailHashKeyProperty = new HygienePropertyDefinition("EmailHashKey", typeof(byte[]));

		internal static readonly HygienePropertyDefinition EmailDomainHashKeyProperty = new HygienePropertyDefinition("EmailDomainHashKey", typeof(byte[]));

		internal static readonly HygienePropertyDefinition IPHashKeyProperty = new HygienePropertyDefinition("IPHashKey", typeof(byte[]));

		internal static readonly HygienePropertyDefinition RuleIdProperty = new HygienePropertyDefinition("RuleId", typeof(Guid));

		internal static readonly HygienePropertyDefinition DataClassificationIdProperty = new HygienePropertyDefinition("DataClassificationId", typeof(Guid));
	}
}
