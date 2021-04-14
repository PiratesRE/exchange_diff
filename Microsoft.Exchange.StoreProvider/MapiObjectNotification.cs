using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiObjectNotification : MapiNotification
	{
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				return (ObjectType)this.objectType;
			}
		}

		public byte[] ParentId
		{
			get
			{
				return this.parentId;
			}
		}

		public byte[] OldId
		{
			get
			{
				return this.oldId;
			}
		}

		public byte[] OldParentId
		{
			get
			{
				return this.oldParentId;
			}
		}

		public PropTag[] Tags
		{
			get
			{
				return this.tags;
			}
		}

		internal unsafe MapiObjectNotification(NOTIFICATION* notification) : base(notification)
		{
			if (notification->info.obj.cbEntryID > 0)
			{
				this.entryId = new byte[notification->info.obj.cbEntryID];
				Marshal.Copy(notification->info.obj.lpEntryID, this.entryId, 0, this.entryId.Length);
			}
			this.objectType = notification->info.obj.ulObjType;
			if (notification->info.obj.cbParentID > 0)
			{
				this.parentId = new byte[notification->info.obj.cbParentID];
				Marshal.Copy(notification->info.obj.lpParentID, this.parentId, 0, this.parentId.Length);
			}
			if (notification->info.obj.cbOldID > 0)
			{
				this.oldId = new byte[notification->info.obj.cbOldID];
				Marshal.Copy(notification->info.obj.lpOldID, this.oldId, 0, this.oldId.Length);
			}
			if (notification->info.obj.cbOldParentID > 0)
			{
				this.oldParentId = new byte[notification->info.obj.cbOldParentID];
				Marshal.Copy(notification->info.obj.lpOldParentID, this.oldParentId, 0, this.oldParentId.Length);
			}
			if (notification->info.obj.lpPropTagArray != null)
			{
				this.tags = Array<PropTag>.New(*notification->info.obj.lpPropTagArray);
				for (int i = 0; i < this.tags.Length; i++)
				{
					this.tags[i] = (PropTag)notification->info.obj.lpPropTagArray[i + 1];
				}
			}
		}

		private readonly byte[] entryId;

		private readonly int objectType;

		private readonly byte[] parentId;

		private readonly byte[] oldId;

		private readonly byte[] oldParentId;

		private readonly PropTag[] tags;
	}
}
