using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class HostedContentFilterRuleFacadeSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition Enabled = HygieneFilterRuleFacadeSchema.Enabled;

		public static readonly HygienePropertyDefinition Priority = HygieneFilterRuleFacadeSchema.Priority;

		public static readonly HygienePropertyDefinition Comments = HygieneFilterRuleFacadeSchema.Comments;

		public static readonly HygienePropertyDefinition SentToMemberOf = HygieneFilterRuleFacadeSchema.SentToMemberOf;

		public static readonly HygienePropertyDefinition SentTo = HygieneFilterRuleFacadeSchema.SentTo;

		public static readonly HygienePropertyDefinition RecipientDomainIs = HygieneFilterRuleFacadeSchema.RecipientDomainIs;

		public static readonly HygienePropertyDefinition ExceptIfRecipientDomainIs = HygieneFilterRuleFacadeSchema.ExceptIfRecipientDomainIs;

		public static readonly HygienePropertyDefinition ExceptIfSentTo = HygieneFilterRuleFacadeSchema.ExceptIfSentTo;

		public static readonly HygienePropertyDefinition ExceptIfSentToMemberOf = HygieneFilterRuleFacadeSchema.ExceptIfSentToMemberOf;

		public static readonly HygienePropertyDefinition HostedContentFilterPolicy = new HygienePropertyDefinition("HostedContentFilterPolicy", typeof(string));
	}
}
