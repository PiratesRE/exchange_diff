using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderNode
	{
		public FolderNode(string serverId, string displayName, string parentId, string contentClass)
		{
			this.serverId = serverId;
			this.displayName = displayName;
			this.parentId = parentId;
			this.contentClass = contentClass.Substring(contentClass.LastIndexOf(':') + 1);
		}

		public string ServerId
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

		public string ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		public string ContentClass
		{
			get
			{
				return this.contentClass;
			}
			set
			{
				this.contentClass = value;
			}
		}

		private string serverId;

		private string displayName;

		private string parentId;

		private string contentClass;
	}
}
