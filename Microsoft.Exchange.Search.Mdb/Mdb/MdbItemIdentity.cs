using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class MdbItemIdentity : IIdentity, IEquatable<IIdentity>, IEquatable<MdbItemIdentity>
	{
		internal MdbItemIdentity(byte[] persistableTenantId, Guid mdbGuid, Guid mailboxGuid, int mailboxNumber, StoreObjectId itemId, int documentId, bool isPublicFolder) : this(1, persistableTenantId, mdbGuid, mailboxGuid, mailboxNumber, itemId, documentId, isPublicFolder)
		{
		}

		private MdbItemIdentity(int version, byte[] persistableTenantId, Guid mdbGuid, Guid mailboxGuid, int mailboxNumber, StoreObjectId itemId, int documentId, bool isPublicFolder)
		{
			Util.ThrowOnNullArgument(itemId, "itemId");
			this.Version = version;
			this.PersistableTenantId = persistableTenantId;
			this.fastMdbGuid = mdbGuid;
			this.MailboxGuid = mailboxGuid;
			this.MailboxNumber = mailboxNumber;
			this.ItemId = itemId;
			this.DocumentId = documentId;
			this.IsPublicFolder = isPublicFolder;
			this.RemapIdsForCatalogRestoreScenario();
		}

		internal int Version { get; private set; }

		internal byte[] PersistableTenantId { get; private set; }

		internal Guid MailboxGuid { get; private set; }

		internal int MailboxNumber { get; private set; }

		internal StoreObjectId ItemId { get; private set; }

		internal int DocumentId { get; private set; }

		internal bool IsPublicFolder { get; private set; }

		public static MdbItemIdentity Parse(string mdbItemIdentity)
		{
			Util.ThrowOnNullOrEmptyArgument(mdbItemIdentity, "mdbItemIdentity");
			string[] array = mdbItemIdentity.Split(new char[]
			{
				','
			});
			int num = 0;
			if (array[0].Length < 11)
			{
				num = int.Parse(array[0]);
			}
			if (num == 0)
			{
				string text = array[3];
				return new MdbItemIdentity(1, null, new Guid(array[0]), new Guid(array[1]), int.Parse(array[2]), text.Equals(MdbItemIdentity.DummyStoreObjectId) ? StoreObjectId.DummyId : StoreObjectId.Deserialize(text), int.Parse(array[4]), false);
			}
			if (num == 1)
			{
				string text2 = array[5];
				return new MdbItemIdentity(num, Convert.FromBase64String(array[1]), new Guid(array[2]), new Guid(array[3]), int.Parse(array[4]), text2.Equals(MdbItemIdentity.DummyStoreObjectId) ? StoreObjectId.DummyId : StoreObjectId.Deserialize(text2), int.Parse(array[6]), bool.Parse(array[7]));
			}
			throw new ArgumentException(string.Format("MdbItemIdentity is not a supported version.  Version: {0}", num));
		}

		public Guid GetMdbGuid(MdbItemIdentity.Location location)
		{
			if (location == MdbItemIdentity.Location.ExchangeMdb)
			{
				return this.exchangeMdbGuid;
			}
			return this.fastMdbGuid;
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as IIdentity);
		}

		public virtual bool Equals(IIdentity other)
		{
			return this.Equals(other as MdbItemIdentity);
		}

		public override int GetHashCode()
		{
			return this.MailboxGuid.GetHashCode() ^ this.DocumentId;
		}

		public override string ToString()
		{
			if (this.stringizedId == null)
			{
				this.stringizedId = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}", new object[]
				{
					',',
					1,
					(this.PersistableTenantId == null) ? string.Empty : Convert.ToBase64String(this.PersistableTenantId),
					this.fastMdbGuid,
					this.MailboxGuid,
					this.MailboxNumber,
					this.ItemId,
					this.DocumentId,
					this.IsPublicFolder
				});
			}
			return this.stringizedId;
		}

		public bool Equals(MdbItemIdentity other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (this.fastMdbGuid.Equals(other.fastMdbGuid) && this.MailboxGuid.Equals(other.MailboxGuid) && this.DocumentId.Equals(other.DocumentId)));
		}

		private void RemapIdsForCatalogRestoreScenario()
		{
			this.exchangeMdbGuid = this.fastMdbGuid;
			if (CatalogRestoreHelper.IsEnabled)
			{
				this.exchangeMdbGuid = CatalogRestoreHelper.GetExchangeMapping(this.fastMdbGuid);
				this.fastMdbGuid = CatalogRestoreHelper.GetFastMapping(this.exchangeMdbGuid);
			}
		}

		private const int CurrentVersion = 1;

		private const char SeparatorChar = ',';

		private static readonly string DummyStoreObjectId = StoreObjectId.DummyId.ToString();

		private string stringizedId;

		private Guid exchangeMdbGuid;

		private Guid fastMdbGuid;

		public enum Location
		{
			FastCatalog,
			ExchangeMdb
		}

		private enum Segments
		{
			MdbGuid,
			MailboxGuid,
			MailboxNumber,
			ItemId,
			DocumentId,
			Size
		}

		private enum Version1Segments
		{
			Version,
			PersistableTenantId,
			MdbGuid,
			MailboxGuid,
			MailboxNumber,
			ItemId,
			DocumentId,
			IsPublicFolder,
			Size
		}
	}
}
