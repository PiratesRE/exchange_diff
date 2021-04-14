using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.PeopleRelevance;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal class PeopleRelevanceDocumentFactory
	{
		protected PeopleRelevanceDocumentFactory()
		{
		}

		public static PeopleRelevanceDocumentFactory Current
		{
			get
			{
				return PeopleRelevanceDocumentFactory.instance.Value;
			}
		}

		internal static Hookable<PeopleRelevanceDocumentFactory> Instance
		{
			get
			{
				return PeopleRelevanceDocumentFactory.instance;
			}
		}

		internal IDocument CreatePeopleRelevanceDocument(PeopleModelItem peopleModelItem, IDocument sentItemsTrainingSubDocument, Guid mailboxGuid, string mailboxAlias)
		{
			Util.ThrowOnNullArgument(peopleModelItem, "modelItem");
			Util.ThrowOnNullArgument(mailboxAlias, "mailboxAlias");
			if (sentItemsTrainingSubDocument == null)
			{
				sentItemsTrainingSubDocument = MdbInferenceFactory.Current.CreateDocument(mailboxGuid, true);
			}
			Document document = MdbInferenceFactory.Current.CreateDocument(mailboxGuid);
			document.SetProperty(PeopleRelevanceSchema.MailboxAlias, mailboxAlias);
			document.SetProperty(PeopleRelevanceSchema.ContactList, peopleModelItem.CreateContactDictionary());
			document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, peopleModelItem.CurrentTimeWindowNumber);
			document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, new ExDateTime(ExTimeZone.UtcTimeZone, peopleModelItem.CurrentTimeWindowStartTime.ToUniversalTime()));
			document.SetProperty(PeopleRelevanceSchema.LastRecipientCacheValidationTime, new ExDateTime(ExTimeZone.UtcTimeZone, peopleModelItem.LastRecipientCacheValidationTime.ToUniversalTime()));
			document.SetProperty(PeopleRelevanceSchema.LastProcessedMessageSentTime, new ExDateTime(ExTimeZone.UtcTimeZone, peopleModelItem.LastProcessedMessageSentTime.ToUniversalTime()));
			document.SetProperty(PeopleRelevanceSchema.SentItemsTrainingSubDocument, sentItemsTrainingSubDocument);
			return document;
		}

		internal static readonly PropertyDefinition[] SentItemsFullDocumentProperties = new PropertyDefinition[]
		{
			PeopleRelevanceSchema.RecipientsTo,
			PeopleRelevanceSchema.RecipientsCc,
			PeopleRelevanceSchema.SentTime,
			PeopleRelevanceSchema.IsReply
		};

		private static readonly Hookable<PeopleRelevanceDocumentFactory> instance = Hookable<PeopleRelevanceDocumentFactory>.Create(true, new PeopleRelevanceDocumentFactory());
	}
}
