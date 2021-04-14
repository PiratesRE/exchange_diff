using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class HierarchyNotificationPayload : NotificationPayloadBase
	{
		[DataMember(Name = "FolderType")]
		public string FolderTypeString
		{
			get
			{
				return this.FolderType.ToString();
			}
			set
			{
				this.FolderType = (StoreObjectType)Enum.Parse(typeof(StoreObjectType), value);
			}
		}

		public string FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public string ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
			set
			{
				this.parentFolderId = value;
			}
		}

		public long ItemCount
		{
			get
			{
				return this.itemCount;
			}
			set
			{
				this.itemCount = value;
			}
		}

		public long UnreadCount
		{
			get
			{
				return this.unreadCount;
			}
			set
			{
				this.unreadCount = value;
			}
		}

		[IgnoreDataMember]
		internal StoreObjectType FolderType { get; set; }

		[DataMember]
		private string folderId;

		[DataMember]
		private string displayName;

		[DataMember]
		private string parentFolderId;

		[DataMember]
		private long itemCount;

		[DataMember]
		private long unreadCount;
	}
}
