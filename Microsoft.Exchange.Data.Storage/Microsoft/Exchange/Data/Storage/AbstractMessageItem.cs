using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractMessageItem : AbstractItem, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public virtual bool IsDraft
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsRead
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string InReplyTo
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IList<Participant> ReplyTo
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual RecipientCollection Recipients
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Participant Sender
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Participant From
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Subject
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string InternetMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsGroupEscalationMessage
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual void SendWithoutSavingMessage()
		{
			throw new NotImplementedException();
		}

		public void SuppressAllAutoResponses()
		{
			throw new NotImplementedException();
		}

		public void MarkRecipientAsSubmitted(IEnumerable<Participant> submittedParticipants)
		{
			throw new NotImplementedException();
		}

		public void StampMessageBodyTag()
		{
			throw new NotImplementedException();
		}

		public void CommitReplyTo()
		{
			throw new NotImplementedException();
		}

		public virtual Reminders<ModernReminder> ModernReminders
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual RemindersState<ModernReminderState> ModernRemindersState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual GlobalObjectId GetGlobalObjectId()
		{
			throw new NotImplementedException();
		}
	}
}
