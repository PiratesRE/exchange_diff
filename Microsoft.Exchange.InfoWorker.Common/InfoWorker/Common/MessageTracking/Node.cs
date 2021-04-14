using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class Node
	{
		public Node(string key, string parentKey, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Attempt to create node with null key.", "key");
			}
			if (parentKey == null)
			{
				throw new ArgumentNullException("Attempt to create node with null parent key.", "parentKey");
			}
			this.key = key;
			this.parentKey = parentKey;
			this.value = value;
		}

		public void AddChild(Node newNode)
		{
			if (this.Children == null)
			{
				this.children = new List<Node>();
			}
			this.children.Add(newNode);
			newNode.Parent = this;
		}

		public Node Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public ReadOnlyCollection<Node> Children
		{
			get
			{
				if (this.children == null)
				{
					this.children = new List<Node>();
				}
				return new ReadOnlyCollection<Node>(this.children);
			}
		}

		public string ParentKey
		{
			get
			{
				return this.parentKey;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public bool HasChildren
		{
			get
			{
				return this.Children != null && this.Children.Count > 0;
			}
		}

		private string key;

		private string parentKey;

		private object value;

		private Node parent;

		private List<Node> children;
	}
}
