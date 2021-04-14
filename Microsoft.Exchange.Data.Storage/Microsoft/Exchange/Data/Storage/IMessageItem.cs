using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessageItem : IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		bool IsDraft { get; set; }

		bool IsRead { get; set; }

		string InReplyTo { get; set; }

		IList<Participant> ReplyTo { get; }

		bool IsGroupEscalationMessage { get; set; }

		RecipientCollection Recipients { get; }

		Participant Sender { get; set; }

		Participant From { get; set; }

		void SendWithoutSavingMessage();

		void SuppressAllAutoResponses();

		void MarkRecipientAsSubmitted(IEnumerable<Participant> submittedParticipants);

		void StampMessageBodyTag();

		void CommitReplyTo();
	}
}
