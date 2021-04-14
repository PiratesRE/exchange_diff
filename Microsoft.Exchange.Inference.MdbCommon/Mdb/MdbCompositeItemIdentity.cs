using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Inference.Mdb
{
	[DataContract]
	[Serializable]
	internal class MdbCompositeItemIdentity : IIdentity, IEquatable<IIdentity>
	{
		internal MdbCompositeItemIdentity(Guid mdbGuid, Guid mailboxGuid, StoreObjectId itemId, int documentId) : this(mdbGuid, mailboxGuid, 1, itemId, documentId)
		{
		}

		internal MdbCompositeItemIdentity(Guid mdbGuid, MapiEvent mapiEvent)
		{
			Util.ThrowOnNullArgument(mapiEvent, "mapiEvent");
			this.Initialize(mdbGuid, mapiEvent.MailboxGuid, mapiEvent.MailboxNumber, StoreObjectId.FromProviderSpecificId(mapiEvent.ItemEntryId, ObjectClass.GetObjectType(mapiEvent.ObjectClass)), mapiEvent.DocumentId);
		}

		internal MdbCompositeItemIdentity(Guid mdbGuid, Guid mailboxGuid, int mailboxNumber, StoreObjectId itemId, int documentId)
		{
			Util.ThrowOnNullArgument(itemId, "itemId");
			this.Initialize(mdbGuid, mailboxGuid, mailboxNumber, itemId, documentId);
		}

		[DataMember]
		internal Guid MdbGuid { get; private set; }

		[DataMember]
		internal Guid MailboxGuid { get; private set; }

		[DataMember]
		internal int MailboxNumber { get; private set; }

		[DataMember]
		internal StoreObjectId ItemId { get; private set; }

		[DataMember]
		internal int DocumentId { get; private set; }

		public static MdbCompositeItemIdentity Parse(string mdbComositeItemIdentity)
		{
			string[] array = mdbComositeItemIdentity.Split(new char[]
			{
				','
			});
			if (array.Length != 5)
			{
				throw new ArgumentException(string.Format("The string representation of the Identity is invalid. {0}", mdbComositeItemIdentity));
			}
			string text = array[3];
			return new MdbCompositeItemIdentity(new Guid(array[0]), new Guid(array[1]), int.Parse(array[2]), text.Equals(MdbCompositeItemIdentity.DummyStoreObjectId) ? StoreObjectId.DummyId : StoreObjectId.Deserialize(text), int.Parse(array[4]));
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as IIdentity);
		}

		public virtual bool Equals(IIdentity other)
		{
			return this.Equals(other as MdbCompositeItemIdentity);
		}

		public override int GetHashCode()
		{
			return this.ItemId.GetHashCode();
		}

		public override string ToString()
		{
			if (this.stringizedId == null)
			{
				this.stringizedId = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}", new object[]
				{
					',',
					this.MdbGuid,
					this.MailboxGuid,
					this.MailboxNumber,
					this.ItemId,
					this.DocumentId
				});
			}
			return this.stringizedId;
		}

		private bool Equals(MdbCompositeItemIdentity other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (this.MdbGuid.Equals(other.MdbGuid) && this.MailboxGuid.Equals(other.MailboxGuid) && this.MailboxNumber.Equals(other.MailboxNumber) && this.ItemId.Equals(other.ItemId) && this.DocumentId.Equals(other.DocumentId)));
		}

		private void Initialize(Guid mdbGuid, Guid mailboxGuid, int mailboxNumber, StoreObjectId itemId, int documentId)
		{
			this.MdbGuid = mdbGuid;
			this.MailboxGuid = mailboxGuid;
			this.MailboxNumber = mailboxNumber;
			this.ItemId = itemId;
			this.DocumentId = documentId;
		}

		private const char SeparatorChar = ',';

		private static readonly string DummyStoreObjectId = StoreObjectId.DummyId.ToString();

		[DataMember]
		private string stringizedId;

		private enum Segments
		{
			MdbGuid,
			MailboxGuid,
			MailboxNumber,
			ItemId,
			DocumentId,
			Size
		}
	}
}
