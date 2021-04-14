using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LoadItemEventArgs : EventArgs
	{
		internal LoadItemEventArgs(IConversationTreeNode treeNode, IStorePropertyBag propertyBag)
		{
			this.treeNode = treeNode;
			this.propertyBag = propertyBag;
		}

		public IConversationTreeNode TreeNode
		{
			get
			{
				return this.treeNode;
			}
		}

		public IStorePropertyBag StorePropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public PropertyDefinition[] MessagePropertyDefinitions
		{
			get
			{
				return this.propertyDefinitions;
			}
			set
			{
				this.propertyDefinitions = value;
			}
		}

		public HtmlStreamOptionCallback HtmlStreamOptionCallback
		{
			get
			{
				return this.htmlStreamOptionCallback;
			}
			set
			{
				this.htmlStreamOptionCallback = value;
			}
		}

		public PropertyDefinition[] OpportunisticLoadPropertyDefinitions
		{
			get
			{
				return this.opportunitisticPropertyDefinitions;
			}
			set
			{
				this.opportunitisticPropertyDefinitions = value;
			}
		}

		private readonly IConversationTreeNode treeNode;

		private readonly IStorePropertyBag propertyBag;

		private PropertyDefinition[] propertyDefinitions;

		private HtmlStreamOptionCallback htmlStreamOptionCallback;

		private PropertyDefinition[] opportunitisticPropertyDefinitions;
	}
}
