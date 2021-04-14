using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncCommandItem : DisposeTrackableBase, ISyncClientOperation
	{
		public bool IsDraft { get; set; }

		public bool SendEnabled { get; set; }

		public string UID { get; set; }

		public Dictionary<string, string> AddedAttachments { get; set; }

		public SyncBase.SyncCommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				this.commandType = value;
			}
		}

		public ChangeType ChangeType
		{
			get
			{
				return this.changeType;
			}
			set
			{
				this.changeType = value;
			}
		}

		public XmlNode XmlNode
		{
			get
			{
				return this.xmlNode;
			}
			set
			{
				this.xmlNode = value;
			}
		}

		public ISyncItemId Id
		{
			get
			{
				return this.serverId;
			}
		}

		public ISyncItemId ServerId
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		public string SyncId
		{
			get
			{
				return this.syncId;
			}
			set
			{
				this.syncId = value;
			}
		}

		public ISyncItem Item
		{
			get
			{
				return this.backendItem;
			}
			set
			{
				this.backendItem = value;
			}
		}

		public string ClientAddId
		{
			get
			{
				return this.clientId;
			}
			set
			{
				this.clientId = value;
			}
		}

		public string Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public int?[] ChangeTrackingInformation
		{
			get
			{
				return this.changeTrackingInformation;
			}
			set
			{
				this.changeTrackingInformation = value;
			}
		}

		public string ClassType
		{
			get
			{
				return this.classType;
			}
			set
			{
				this.classType = value;
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.conversationId;
			}
			set
			{
				this.conversationId = value;
			}
		}

		public byte[] ConversationIndex
		{
			get
			{
				return this.conversationIndex;
			}
			set
			{
				this.conversationIndex = value;
			}
		}

		public bool IsMms { get; set; }

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.backendItem != null)
			{
				this.backendItem.Dispose();
				this.backendItem = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncCommandItem>(this);
		}

		private SyncBase.SyncCommandType commandType;

		private ChangeType changeType;

		private string clientId;

		private XmlNode xmlNode;

		private ISyncItemId serverId;

		private string syncId;

		private ISyncItem backendItem;

		private string status;

		private int?[] changeTrackingInformation;

		private string classType;

		private ConversationId conversationId;

		private byte[] conversationIndex;
	}
}
