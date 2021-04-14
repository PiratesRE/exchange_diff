using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class HygieneFilterRuleFacadeSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition Enabled = new HygienePropertyDefinition("Enabled", typeof(bool), true, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition Priority = new HygienePropertyDefinition("Priority", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition Comments = new HygienePropertyDefinition("Comments", typeof(string));

		public static readonly HygienePropertyDefinition SentToMemberOf = new HygienePropertyDefinition("SentToMemberOf", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition SentTo = new HygienePropertyDefinition("SentTo", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition RecipientDomainIs = new HygienePropertyDefinition("RecipientDomainIs", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition ExceptIfRecipientDomainIs = new HygienePropertyDefinition("ExceptIfRecipientDomainIs", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition ExceptIfSentTo = new HygienePropertyDefinition("ExceptIfSentTo", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition ExceptIfSentToMemberOf = new HygienePropertyDefinition("ExceptIfSentToMemberOf", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);
	}
}
