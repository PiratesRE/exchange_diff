using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Transport.Email
{
	public class EmailRecipientCollection : IEnumerable<EmailRecipient>, IEnumerable
	{
		internal EmailRecipientCollection(MessageImplementation message, RecipientType recipientType) : this(message, recipientType, false)
		{
		}

		internal EmailRecipientCollection(MessageImplementation message, RecipientType recipientType, bool isReadOnly)
		{
			this.message = message;
			this.recipientType = recipientType;
			this.isReadOnly = isReadOnly;
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		internal object Cache
		{
			get
			{
				return this.cache;
			}
			set
			{
				this.cache = value;
			}
		}

		public EmailRecipient this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		public void Add(EmailRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (this.isReadOnly)
			{
				throw new NotSupportedException(EmailMessageStrings.CollectionIsReadOnly);
			}
			if (recipient.MimeRecipient.Parent != null || recipient.TnefRecipient.TnefMessage != null)
			{
				throw new ArgumentException(EmailMessageStrings.RecipientAlreadyHasParent, "recipient");
			}
			this.message.AddRecipient(this.recipientType, ref this.cache, recipient);
			this.list.Add(recipient);
		}

		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException(EmailMessageStrings.CollectionIsReadOnly);
			}
			this.message.ClearRecipients(this.recipientType, ref this.cache);
			this.list.Clear();
		}

		IEnumerator<EmailRecipient> IEnumerable<EmailRecipient>.GetEnumerator()
		{
			return new EmailRecipientCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new EmailRecipientCollection.Enumerator(this);
		}

		public EmailRecipientCollection.Enumerator GetEnumerator()
		{
			return new EmailRecipientCollection.Enumerator(this);
		}

		public bool Remove(EmailRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (this.isReadOnly)
			{
				throw new NotSupportedException(EmailMessageStrings.CollectionIsReadOnly);
			}
			if (!this.list.Contains(recipient))
			{
				return false;
			}
			this.message.RemoveRecipient(this.recipientType, ref this.cache, recipient);
			return this.list.Remove(recipient);
		}

		internal void InternalAdd(EmailRecipient recipient)
		{
			this.list.Add(recipient);
		}

		private MessageImplementation message;

		private List<EmailRecipient> list = new List<EmailRecipient>();

		private RecipientType recipientType;

		private bool isReadOnly;

		private object cache;

		public struct Enumerator : IEnumerator<EmailRecipient>, IDisposable, IEnumerator
		{
			internal Enumerator(EmailRecipientCollection collection)
			{
				this.collection = collection;
				this.index = -1;
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.collection == null)
					{
						throw new InvalidOperationException(EmailMessageStrings.ErrorInit);
					}
					if (this.index == -1)
					{
						throw new InvalidOperationException(EmailMessageStrings.ErrorBeforeFirst);
					}
					if (this.index == this.collection.Count)
					{
						throw new InvalidOperationException(EmailMessageStrings.ErrorAfterLast);
					}
					return this.collection[this.index];
				}
			}

			public EmailRecipient Current
			{
				get
				{
					if (this.collection == null)
					{
						throw new InvalidOperationException(EmailMessageStrings.ErrorInit);
					}
					if (this.index == -1)
					{
						throw new InvalidOperationException(EmailMessageStrings.ErrorBeforeFirst);
					}
					if (this.index == this.collection.Count)
					{
						throw new InvalidOperationException(EmailMessageStrings.ErrorAfterLast);
					}
					return this.collection[this.index];
				}
			}

			public bool MoveNext()
			{
				if (this.collection == null)
				{
					throw new InvalidOperationException(EmailMessageStrings.ErrorInit);
				}
				return this.index != this.collection.Count && ++this.index < this.collection.Count;
			}

			public void Reset()
			{
				this.index = -1;
			}

			public void Dispose()
			{
				this.Reset();
			}

			private EmailRecipientCollection collection;

			private int index;
		}
	}
}
