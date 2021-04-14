using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class RequestJobObjectId : ObjectId
	{
		internal RequestJobObjectId(Guid requestGuid, Guid mdbGuid, IRequestIndexEntry indexEntry = null)
		{
			this.requestGuid = requestGuid;
			this.mdbGuid = mdbGuid;
			this.messageId = null;
			this.adUser = null;
			this.sourceUser = null;
			this.targetUser = null;
			this.indexEntry = indexEntry;
		}

		internal RequestJobObjectId(Guid requestGuid, Guid mdbGuid, byte[] messageId)
		{
			this.requestGuid = requestGuid;
			this.mdbGuid = mdbGuid;
			this.messageId = messageId;
			this.adUser = null;
			this.sourceUser = null;
			this.targetUser = null;
			this.indexEntry = null;
		}

		internal RequestJobObjectId(ADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser", "An ADUser must be provided to this constructor");
			}
			this.adUser = adUser;
			this.sourceUser = null;
			this.targetUser = null;
			this.indexEntry = null;
			Guid a;
			Guid a2;
			RequestIndexEntryProvider.GetMoveGuids(adUser, out a, out a2);
			if (a != Guid.Empty && a2 != Guid.Empty)
			{
				this.requestGuid = a;
				this.mdbGuid = a2;
			}
			else
			{
				this.requestGuid = adUser.ExchangeGuid;
				this.mdbGuid = Guid.Empty;
			}
			this.messageId = null;
		}

		public Guid RequestGuid
		{
			get
			{
				return this.requestGuid;
			}
			internal set
			{
				this.requestGuid = value;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
			internal set
			{
				this.mdbGuid = value;
			}
		}

		internal byte[] MessageId
		{
			get
			{
				return this.messageId;
			}
			set
			{
				this.messageId = value;
			}
		}

		internal ADUser User
		{
			get
			{
				return this.adUser;
			}
			set
			{
				this.adUser = value;
			}
		}

		internal ADUser SourceUser
		{
			get
			{
				return this.sourceUser;
			}
			set
			{
				this.sourceUser = value;
			}
		}

		internal ADUser TargetUser
		{
			get
			{
				return this.targetUser;
			}
			set
			{
				this.targetUser = value;
			}
		}

		internal IRequestIndexEntry IndexEntry
		{
			get
			{
				return this.indexEntry;
			}
		}

		public override byte[] GetBytes()
		{
			byte[] array = new byte[32];
			Array.Copy(this.requestGuid.ToByteArray(), array, 16);
			Array.Copy(this.mdbGuid.ToByteArray(), 0, array, 16, 16);
			return array;
		}

		public override string ToString()
		{
			if (this.indexEntry != null)
			{
				return this.indexEntry.GetRequestIndexEntryId(null).ToString();
			}
			return string.Format("{0}\\{1}", this.mdbGuid, this.requestGuid);
		}

		public bool Equals(RequestJobObjectId id)
		{
			return id != null && (this.requestGuid.Equals(id.RequestGuid) && this.mdbGuid.Equals(id.MdbGuid)) && (this.messageId == null || id.MessageId == null || CommonUtils.IsSameEntryId(this.messageId, id.MessageId));
		}

		private Guid requestGuid;

		private Guid mdbGuid;

		private byte[] messageId;

		[NonSerialized]
		private ADUser adUser;

		[NonSerialized]
		private ADUser sourceUser;

		[NonSerialized]
		private ADUser targetUser;

		[NonSerialized]
		private IRequestIndexEntry indexEntry;
	}
}
