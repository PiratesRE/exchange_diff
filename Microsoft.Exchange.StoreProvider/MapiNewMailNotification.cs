using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiNewMailNotification : MapiNotification
	{
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		public byte[] ParentId
		{
			get
			{
				return this.parentId;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public int MessageFlags
		{
			get
			{
				return this.messageFlags;
			}
		}

		internal unsafe MapiNewMailNotification(NOTIFICATION* notification) : base(notification)
		{
			if (notification->info.newmail.cbEntryID > 0)
			{
				this.entryId = new byte[notification->info.newmail.cbEntryID];
				Marshal.Copy(notification->info.newmail.lpEntryID, this.entryId, 0, this.entryId.Length);
			}
			if (notification->info.newmail.cbParentID > 0)
			{
				this.parentId = new byte[notification->info.newmail.cbParentID];
				Marshal.Copy(notification->info.newmail.lpParentID, this.parentId, 0, this.parentId.Length);
			}
			if (notification->info.newmail.lpszMessageClass != IntPtr.Zero)
			{
				if ((notification->info.newmail.ulFlags & -2147483648) != 0)
				{
					this.messageClass = Marshal.PtrToStringUni(notification->info.newmail.lpszMessageClass);
				}
				else
				{
					this.messageClass = Marshal.PtrToStringAnsi(notification->info.newmail.lpszMessageClass);
				}
			}
			this.messageFlags = notification->info.newmail.ulMessageFlags;
		}

		private readonly byte[] entryId;

		private readonly byte[] parentId;

		private readonly string messageClass;

		private readonly int messageFlags;
	}
}
