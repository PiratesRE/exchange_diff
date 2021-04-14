using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Transport.Email
{
	public class AttachmentCollection : IEnumerable<Attachment>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.message.AttachmentCollection_Count();
			}
		}

		public Attachment this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (index > this.Count - 1)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				Attachment attachment = (Attachment)this.message.AttachmentCollection_Indexer(index);
				if (attachment != null)
				{
					return attachment;
				}
				attachment = new Attachment(this.message);
				AttachmentCookie cookie = this.message.AttachmentCollection_CacheAttachment(index, attachment);
				attachment.Cookie = cookie;
				return attachment;
			}
		}

		internal AttachmentCollection(EmailMessage message)
		{
			this.message = message;
		}

		public Attachment Add()
		{
			return this.Add(null, null);
		}

		public Attachment Add(string fileName)
		{
			return this.Add(fileName, null);
		}

		public Attachment Add(string fileName, string contentType)
		{
			if (string.IsNullOrEmpty(contentType))
			{
				contentType = "application/octet-stream";
			}
			Attachment attachment = new Attachment(this.message);
			AttachmentCookie cookie = this.message.AttachmentCollection_AddAttachment(attachment);
			attachment.Cookie = cookie;
			if (!string.IsNullOrEmpty(fileName))
			{
				this.message.Attachment_SetFileName(cookie, fileName);
			}
			this.message.Attachment_SetContentType(cookie, contentType);
			this.version++;
			return attachment;
		}

		public bool Remove(Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("item");
			}
			bool result = this.message.AttachmentCollection_RemoveAttachment(attachment.Cookie);
			this.version++;
			return result;
		}

		public void Clear()
		{
			this.message.AttachmentCollection_ClearAttachments();
			this.version++;
		}

		public AttachmentCollection.Enumerator GetEnumerator()
		{
			return new AttachmentCollection.Enumerator(this);
		}

		IEnumerator<Attachment> IEnumerable<Attachment>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal void InvalidateEnumerators()
		{
			this.version++;
		}

		private EmailMessage message;

		private int version;

		public struct Enumerator : IEnumerator<Attachment>, IDisposable, IEnumerator
		{
			internal Enumerator(AttachmentCollection collection)
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

			public Attachment Current
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

			private AttachmentCollection collection;

			private int index;
		}
	}
}
