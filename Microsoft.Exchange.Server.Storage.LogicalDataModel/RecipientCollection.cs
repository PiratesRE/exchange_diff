using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class RecipientCollection
	{
		public RecipientCollection(Message parent)
		{
			this.parent = parent;
			this.recipients = new List<Recipient>();
			this.changed = true;
		}

		public RecipientCollection(Message parent, byte[][] blob) : this(parent)
		{
			this.FromBinary(blob);
			this.changed = false;
		}

		internal ObjectPropertySchema RecipientSchema
		{
			get
			{
				if (this.recipientPropertySchema == null)
				{
					this.recipientPropertySchema = PropertySchema.GetObjectSchema(this.parent.Mailbox.Database, ObjectType.Recipient);
				}
				return this.recipientPropertySchema;
			}
		}

		public Message ParentMessage
		{
			get
			{
				return this.parent;
			}
		}

		public int Count
		{
			get
			{
				return this.recipients.Count;
			}
		}

		public bool Changed
		{
			get
			{
				return this.changed;
			}
			set
			{
				this.changed = value;
			}
		}

		public Recipient this[int index]
		{
			get
			{
				return this.recipients[index];
			}
			set
			{
				this.recipients[index] = value;
				this.Changed = true;
			}
		}

		public Recipient Add(string email, string name, RecipientType type, int rowId)
		{
			Recipient recipient = new Recipient(this);
			recipient.Email = email;
			recipient.Name = name;
			recipient.RecipientType = type;
			recipient.RowId = rowId;
			this.Add(recipient);
			return recipient;
		}

		public byte[][] ToBinary(Context context)
		{
			if (this.recipients.Count == 0)
			{
				return null;
			}
			byte[][] array = new byte[this.recipients.Count][];
			for (int i = 0; i < this.recipients.Count; i++)
			{
				this.recipients[i].ToBinary(context, out array[i]);
			}
			return array;
		}

		public IEnumerator GetEnumerator()
		{
			return this.recipients.GetEnumerator();
		}

		public void Delete()
		{
			this.recipients = new List<Recipient>();
			this.Changed = true;
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException((LID)60024U, "CopyTo on RecipientCollection is unsupported");
		}

		public void Clear()
		{
			this.Delete();
		}

		public void Remove(Recipient recipient)
		{
			this.recipients.Remove(recipient);
			this.Changed = true;
		}

		public int IndexOf(Recipient recipient)
		{
			return this.recipients.IndexOf(recipient);
		}

		private int Add(Recipient recipient)
		{
			this.recipients.Add(recipient);
			this.Changed = true;
			return this.recipients.Count;
		}

		private void FromBinary(byte[][] blob)
		{
			if (blob == null)
			{
				return;
			}
			for (int i = 0; i < blob.Length; i++)
			{
				Recipient recipient = new Recipient(this, blob[i]);
				recipient.RowId = this.recipients.Count;
				this.recipients.Add(recipient);
			}
		}

		private Message parent;

		private List<Recipient> recipients;

		private bool changed;

		private ObjectPropertySchema recipientPropertySchema;
	}
}
