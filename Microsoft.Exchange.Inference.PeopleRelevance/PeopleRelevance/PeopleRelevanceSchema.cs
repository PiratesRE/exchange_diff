using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal class PeopleRelevanceSchema : DocumentSchema
	{
		public static readonly PropertyDefinition ContactList = new SimplePropertyDefinition("ContactList", typeof(IDictionary<string, IInferenceRecipient>), PropertyFlag.None);

		public static readonly PropertyDefinition SentItemsTrainingSubDocument = new SimplePropertyDefinition("SentItemsTrainingSubDocument", typeof(IDocument), PropertyFlag.None);

		public static readonly PropertyDefinition CurrentTimeWindowStartTime = new SimplePropertyDefinition("CurrentTimeWindowStartTime", typeof(ExDateTime), PropertyFlag.None);

		public static readonly PropertyDefinition CurrentTimeWindowNumber = new SimplePropertyDefinition("CurrentTimeWindowNumber", typeof(long), PropertyFlag.None);

		public static readonly PropertyDefinition TimeWindowNumberAtLastRun = new SimplePropertyDefinition("TimeWindowNumberAtLastRun", typeof(long), PropertyFlag.None);

		public static readonly PropertyDefinition TopRankedContacts = new SimplePropertyDefinition("TopRankedContacts", typeof(List<string>), PropertyFlag.None);

		public static readonly PropertyDefinition PeopleModelVersion = new SimplePropertyDefinition("PeopleModelVersion", typeof(Version), PropertyFlag.None);

		public static readonly PropertyDefinition LastRecipientCacheValidationTime = new SimplePropertyDefinition("LastRecipientCacheValidationTime", typeof(ExDateTime), PropertyFlag.None);

		public static readonly PropertyDefinition MailboxOwner = new SimplePropertyDefinition("MailboxOwner", typeof(IMessageRecipient), PropertyFlag.None);

		public static readonly PropertyDefinition MailboxAlias = new SimplePropertyDefinition("MailboxAlias", typeof(string), PropertyFlag.None);

		public static readonly PropertyDefinition SentTime = new SimplePropertyDefinition("SentTime", typeof(ExDateTime), PropertyFlag.None);

		public static readonly PropertyDefinition RecipientsTo = new SimplePropertyDefinition("RecipientsTo", typeof(IList<IMessageRecipient>), PropertyFlag.None);

		public static readonly PropertyDefinition RecipientsCc = new SimplePropertyDefinition("RecipientsCc", typeof(IList<IMessageRecipient>), PropertyFlag.None);

		public static readonly PropertyDefinition IsReply = new SimplePropertyDefinition("IsReply", typeof(bool), PropertyFlag.None);

		public static readonly PropertyDefinition IsBasedOnRecipientInfoData = new SimplePropertyDefinition("IsBasedOnRecipientInfoData", typeof(bool), PropertyFlag.None);

		public static readonly PropertyDefinition RecipientInfoEnumerable = new SimplePropertyDefinition("RecipientInfoEnumerable", typeof(IEnumerable<IRecipientInfo>), PropertyFlag.None);

		public static readonly PropertyDefinition LastProcessedMessageSentTime = new SimplePropertyDefinition("LastProcessedMessageSentTime", typeof(ExDateTime), PropertyFlag.None);
	}
}
