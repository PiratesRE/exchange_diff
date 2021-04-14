using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MapiEvent : IMapiEvent
	{
		internal MapiEvent(ref MapiEventNative pEvent) : this(ref pEvent, true)
		{
		}

		internal MapiEvent(ref MapiEventNative pEvent, bool includeSid)
		{
			this.eventCounter = (long)pEvent.llEventCounter;
			this.eventMask = (MapiEventTypeFlags)pEvent.ulMask;
			this.mailboxGuid = pEvent.mailboxGuid;
			this.objectClass = pEvent.rgchObjectClass;
			this.createTime = DateTime.FromFileTimeUtc(pEvent.ftCreate);
			this.itemType = (ObjectType)pEvent.ulItemType;
			this.watermark = null;
			this.mdbWatermark = null;
			this.itemCount = (long)pEvent.lItemCount;
			this.unreadItemCount = (long)pEvent.lUnreadCount;
			this.eventFlags = (MapiEventFlags)pEvent.ulFlags;
			this.extendedEventFlags = (MapiExtendedEventFlags)pEvent.ullExtendedFlags;
			if (pEvent.binEidItem.count > 0)
			{
				this.itemEntryId = new byte[pEvent.binEidItem.count];
				Marshal.Copy(pEvent.binEidItem.intPtr, this.itemEntryId, 0, pEvent.binEidItem.count);
			}
			if (pEvent.binEidParent.count > 0)
			{
				this.parentEntryId = new byte[pEvent.binEidParent.count];
				Marshal.Copy(pEvent.binEidParent.intPtr, this.parentEntryId, 0, pEvent.binEidParent.count);
			}
			if (pEvent.binEidOldItem.count > 0)
			{
				this.oldItemEntryId = new byte[pEvent.binEidOldItem.count];
				Marshal.Copy(pEvent.binEidOldItem.intPtr, this.oldItemEntryId, 0, pEvent.binEidOldItem.count);
			}
			if (pEvent.binEidOldParent.count > 0)
			{
				this.oldParentEntryId = new byte[pEvent.binEidOldParent.count];
				Marshal.Copy(pEvent.binEidOldParent.intPtr, this.oldParentEntryId, 0, pEvent.binEidOldParent.count);
			}
			this.clientType = (MapiEventClientTypes)pEvent.ulClientType;
			if (includeSid && pEvent.binSid.count > 0)
			{
				byte[] array = new byte[pEvent.binSid.count];
				Marshal.Copy(pEvent.binSid.intPtr, array, 0, pEvent.binSid.count);
				this.sid = new SecurityIdentifier(array, 0);
			}
			this.docId = (int)pEvent.ulDocId;
			this.mailboxNumber = (int)pEvent.ulMailboxNumber;
			if (pEvent.binTenantHintBob.count > 0)
			{
				this.tenantHint = new byte[pEvent.binTenantHintBob.count];
				Marshal.Copy(pEvent.binTenantHintBob.intPtr, this.tenantHint, 0, pEvent.binTenantHintBob.count);
			}
			this.unifiedMailboxGuid = pEvent.unifiedMailboxGuid;
		}

		public long EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		public Watermark Watermark
		{
			get
			{
				if (this.watermark == null)
				{
					this.watermark = new Watermark(this.mailboxGuid, this.eventCounter);
				}
				return this.watermark;
			}
		}

		public Watermark DatabaseWatermark
		{
			get
			{
				if (this.mdbWatermark == null)
				{
					this.mdbWatermark = new Watermark(Guid.Empty, this.eventCounter);
				}
				return this.mdbWatermark;
			}
		}

		public MapiEventTypeFlags EventMask
		{
			get
			{
				return this.eventMask;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string ObjectClass
		{
			get
			{
				return this.objectClass;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return this.createTime;
			}
		}

		public ObjectType ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		public byte[] ItemEntryId
		{
			get
			{
				return this.itemEntryId;
			}
		}

		public byte[] ParentEntryId
		{
			get
			{
				return this.parentEntryId;
			}
		}

		public byte[] OldItemEntryId
		{
			get
			{
				return this.oldItemEntryId;
			}
		}

		public byte[] OldParentEntryId
		{
			get
			{
				return this.oldParentEntryId;
			}
		}

		public string ItemEntryIdString
		{
			get
			{
				return this.EntryIdString(this.itemEntryId);
			}
		}

		public string ParentEntryIdString
		{
			get
			{
				return this.EntryIdString(this.parentEntryId);
			}
		}

		public string OldItemEntryIdString
		{
			get
			{
				return this.EntryIdString(this.oldItemEntryId);
			}
		}

		public string OldParentEntryIdString
		{
			get
			{
				return this.EntryIdString(this.oldParentEntryId);
			}
		}

		public long ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		public long UnreadItemCount
		{
			get
			{
				return this.unreadItemCount;
			}
		}

		public MapiEventFlags EventFlags
		{
			get
			{
				return this.eventFlags;
			}
		}

		public MapiExtendedEventFlags ExtendedEventFlags
		{
			get
			{
				return this.extendedEventFlags;
			}
		}

		public MapiEventClientTypes ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return this.sid;
			}
		}

		public int DocumentId
		{
			get
			{
				return this.docId;
			}
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public byte[] TenantHint
		{
			get
			{
				return this.tenantHint;
			}
		}

		public Guid UnifiedMailboxGuid
		{
			get
			{
				return this.unifiedMailboxGuid;
			}
		}

		public override string ToString()
		{
			string text = string.Format("Counter: 0x{0,0:X}; MailboxID: {1}; Mask: {2}; Flags: {10}; ExtendedFlags: {11};\nObject Class: {3}; Created Time: {4};\nItem Type: {5}\nItem EntryId: {6}\nParent entryId: {7}\nOld Item entryId: {8}\nOld parent entryId: {9}\nSID: {12}\nClient Type: {13}\nDocument ID: {14}\n", new object[]
			{
				this.eventCounter,
				this.mailboxGuid,
				this.eventMask.ToString(),
				this.objectClass,
				this.createTime,
				this.ItemType,
				this.ItemEntryIdString,
				this.ParentEntryIdString,
				this.OldItemEntryIdString,
				this.OldParentEntryIdString,
				this.eventFlags,
				this.extendedEventFlags,
				(null != this.sid) ? this.sid.ToString() : "<null>",
				this.clientType,
				this.docId
			});
			if (ObjectType.MAPI_FOLDER == this.itemType)
			{
				text += string.Format("Item Count: {0}\nUnread Item Count: {1}\n", this.itemCount, this.unreadItemCount);
			}
			return text;
		}

		private string EntryIdString(byte[] entryId)
		{
			string result = null;
			if (entryId != null)
			{
				StringBuilder stringBuilder = new StringBuilder(2 * entryId.Length);
				foreach (byte b in entryId)
				{
					stringBuilder.AppendFormat("{0:X} ", b);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private Watermark watermark;

		private Watermark mdbWatermark;

		private readonly long eventCounter;

		private readonly MapiEventTypeFlags eventMask;

		private readonly Guid mailboxGuid;

		private readonly string objectClass;

		private readonly DateTime createTime;

		private readonly ObjectType itemType;

		private readonly byte[] itemEntryId;

		private readonly byte[] parentEntryId;

		private readonly byte[] oldItemEntryId;

		private readonly byte[] oldParentEntryId;

		private readonly long itemCount;

		private readonly long unreadItemCount;

		private readonly MapiEventFlags eventFlags;

		private readonly MapiExtendedEventFlags extendedEventFlags;

		private readonly MapiEventClientTypes clientType;

		[NonSerialized]
		private readonly SecurityIdentifier sid;

		private readonly int docId;

		private readonly int mailboxNumber;

		private readonly byte[] tenantHint;

		private readonly Guid unifiedMailboxGuid;
	}
}
