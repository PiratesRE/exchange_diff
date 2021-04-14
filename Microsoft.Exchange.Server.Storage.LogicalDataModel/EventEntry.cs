using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class EventEntry
	{
		internal EventEntry(long eventCounter, DateTime createTime) : this(eventCounter, createTime, 0, (EventType)0, null, new Guid?(Guid.Empty), new Guid?(Guid.Empty), null, null, null, null, null, null, null, null, null, EventFlags.None, null, (ClientType)0, null, null, TenantHint.Empty, null)
		{
		}

		public EventEntry(long eventCounter, DateTime createTime, int transactionId, EventType eventType, int? mailboxNumber, Guid? mailboxGuid, Guid? mapiEntryIdGuid, string objectClass, byte[] fid24, byte[] mid24, byte[] parentFid24, byte[] oldFid24, byte[] oldMid24, byte[] oldParentFid24, int? itemCount, int? unreadCount, EventFlags flags, ExtendedEventFlags? extendedFlags, ClientType clientType, byte[] sid, int? documentId, TenantHint tenantHint, Guid? unifiedMailboxGuid)
		{
			this.eventCounter = eventCounter;
			this.createTime = createTime;
			this.transactionId = transactionId;
			this.eventType = eventType;
			this.mailboxNumber = mailboxNumber;
			this.mailboxGuid = mailboxGuid;
			this.mapiEntryIdGuid = mapiEntryIdGuid;
			this.objectClass = objectClass;
			this.fid24 = fid24;
			this.mid24 = mid24;
			this.parentFid24 = parentFid24;
			this.oldFid24 = oldFid24;
			this.oldMid24 = oldMid24;
			this.oldParentFid24 = oldParentFid24;
			this.itemCount = itemCount;
			this.unreadCount = unreadCount;
			this.flags = flags;
			this.extendedFlags = extendedFlags;
			this.clientType = clientType;
			this.sid = sid;
			this.documentId = documentId;
			this.tenantHint = tenantHint;
			this.unifiedMailboxGuid = unifiedMailboxGuid;
		}

		public long EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return this.createTime;
			}
		}

		public int TransactionId
		{
			get
			{
				return this.transactionId;
			}
		}

		public EventType EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public int? MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Guid? MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public Guid? MapiEntryIdGuid
		{
			get
			{
				return this.mapiEntryIdGuid;
			}
		}

		public string ObjectClass
		{
			get
			{
				return this.objectClass;
			}
		}

		public byte[] Fid24
		{
			get
			{
				return this.fid24;
			}
		}

		public byte[] Mid24
		{
			get
			{
				return this.mid24;
			}
		}

		public byte[] ParentFid24
		{
			get
			{
				return this.parentFid24;
			}
		}

		public byte[] OldFid24
		{
			get
			{
				return this.oldFid24;
			}
		}

		public byte[] OldMid24
		{
			get
			{
				return this.oldMid24;
			}
		}

		public byte[] OldParentFid24
		{
			get
			{
				return this.oldParentFid24;
			}
		}

		public int? ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		public int? UnreadCount
		{
			get
			{
				return this.unreadCount;
			}
		}

		public EventFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public ExtendedEventFlags? ExtendedFlags
		{
			get
			{
				return this.extendedFlags;
			}
		}

		public ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public byte[] Sid
		{
			get
			{
				return this.sid;
			}
		}

		public int? DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		public TenantHint TenantHint
		{
			get
			{
				return this.tenantHint;
			}
		}

		public Guid? UnifiedMailboxGuid
		{
			get
			{
				return this.unifiedMailboxGuid;
			}
		}

		private long eventCounter;

		private DateTime createTime;

		private int transactionId;

		private EventType eventType;

		private int? mailboxNumber;

		private Guid? mailboxGuid;

		private Guid? mapiEntryIdGuid;

		private string objectClass;

		private byte[] fid24;

		private byte[] mid24;

		private byte[] parentFid24;

		private byte[] oldFid24;

		private byte[] oldMid24;

		private byte[] oldParentFid24;

		private int? itemCount;

		private int? unreadCount;

		private EventFlags flags;

		private ExtendedEventFlags? extendedFlags;

		private ClientType clientType;

		private byte[] sid;

		private int? documentId;

		private TenantHint tenantHint;

		private Guid? unifiedMailboxGuid;
	}
}
