using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct FormatNode
	{
		internal FormatNode(FormatStore.NodeStore nodes, int nodeHandle)
		{
			this.nodes = nodes;
			this.nodeHandle = nodeHandle;
		}

		internal FormatNode(FormatStore store, int nodeHandle)
		{
			this.nodes = store.Nodes;
			this.nodeHandle = nodeHandle;
		}

		public int Handle
		{
			get
			{
				return this.nodeHandle;
			}
		}

		public bool IsNull
		{
			get
			{
				return this.nodeHandle == 0;
			}
		}

		public bool IsInOrder
		{
			get
			{
				return 0 == (byte)(this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.OutOfOrder);
			}
		}

		public bool OnRightEdge
		{
			get
			{
				return 0 != (byte)(this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.OnRightEdge);
			}
		}

		public bool OnLeftEdge
		{
			get
			{
				return 0 != (byte)(this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.OnLeftEdge);
			}
		}

		public bool IsVisited
		{
			get
			{
				return 0 != (byte)(this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.Visited);
			}
		}

		public bool IsEmptyBlockNode
		{
			get
			{
				return (byte)(this.NodeType & FormatContainerType.BlockFlag) != 0 && this.BeginTextPosition + 1U == this.EndTextPosition;
			}
		}

		public bool CanFlush
		{
			get
			{
				return 0 != (byte)(this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NodeFlags & FormatStore.NodeFlags.CanFlush);
			}
		}

		public FormatContainerType NodeType
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type = value;
			}
		}

		public bool IsText
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type == FormatContainerType.Text;
			}
		}

		public FormatNode Parent
		{
			get
			{
				int parent = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Parent;
				if (parent != 0)
				{
					return new FormatNode(this.nodes, parent);
				}
				return FormatNode.Null;
			}
		}

		public bool IsOnlySibling
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NextSibling == this.nodeHandle;
			}
		}

		public FormatNode FirstChild
		{
			get
			{
				int lastChild = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].LastChild;
				if (lastChild != 0)
				{
					return new FormatNode(this.nodes, this.nodes.Plane(lastChild)[this.nodes.Index(lastChild)].NextSibling);
				}
				return FormatNode.Null;
			}
		}

		public FormatNode LastChild
		{
			get
			{
				int lastChild = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].LastChild;
				if (lastChild != 0)
				{
					return new FormatNode(this.nodes, lastChild);
				}
				return FormatNode.Null;
			}
		}

		public FormatNode NextSibling
		{
			get
			{
				int parent = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Parent;
				if (parent != 0 && this.nodeHandle != this.nodes.Plane(parent)[this.nodes.Index(parent)].LastChild)
				{
					return new FormatNode(this.nodes, this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].NextSibling);
				}
				return FormatNode.Null;
			}
		}

		public FormatNode PreviousSibling
		{
			get
			{
				FormatNode formatNode = this.Parent.FirstChild;
				if (this == formatNode)
				{
					return FormatNode.Null;
				}
				while (formatNode.NextSibling != this)
				{
					formatNode = formatNode.NextSibling;
				}
				return formatNode;
			}
		}

		public uint BeginTextPosition
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].BeginTextPosition;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].BeginTextPosition = value;
			}
		}

		public uint EndTextPosition
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].EndTextPosition;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].EndTextPosition = value;
			}
		}

		public int InheritanceMaskIndex
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].InheritanceMaskIndex;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].InheritanceMaskIndex = value;
			}
		}

		public FlagProperties FlagProperties
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].FlagProperties;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].FlagProperties = value;
			}
		}

		public PropertyBitMask PropertyMask
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].PropertyMask;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].PropertyMask = value;
			}
		}

		public Property[] Properties
		{
			get
			{
				return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Properties;
			}
			set
			{
				this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Properties = value;
			}
		}

		public NodePropertiesEnumerator PropertiesEnumerator
		{
			get
			{
				return new NodePropertiesEnumerator(this);
			}
		}

		public bool IsBlockNode
		{
			get
			{
				return 0 != (byte)(this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type & FormatContainerType.BlockFlag);
			}
		}

		public FormatNode.NodeSubtree Subtree
		{
			get
			{
				return new FormatNode.NodeSubtree(this);
			}
		}

		public FormatNode.NodeChildren Children
		{
			get
			{
				return new FormatNode.NodeChildren(this);
			}
		}

		public static bool operator ==(FormatNode x, FormatNode y)
		{
			return x.nodes == y.nodes && x.nodeHandle == y.nodeHandle;
		}

		public static bool operator !=(FormatNode x, FormatNode y)
		{
			return x.nodes != y.nodes || x.nodeHandle != y.nodeHandle;
		}

		public void SetOutOfOrder()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].NodeFlags = (array[num].NodeFlags | FormatStore.NodeFlags.OutOfOrder);
		}

		public void SetOnLeftEdge()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].NodeFlags = (array[num].NodeFlags | FormatStore.NodeFlags.OnLeftEdge);
		}

		public void ResetOnLeftEdge()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].NodeFlags = (array[num].NodeFlags & ~FormatStore.NodeFlags.OnLeftEdge);
		}

		public void SetOnRightEdge()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].NodeFlags = (array[num].NodeFlags | FormatStore.NodeFlags.OnRightEdge);
		}

		public void SetVisited()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].NodeFlags = (array[num].NodeFlags | FormatStore.NodeFlags.Visited);
		}

		public void ResetVisited()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].NodeFlags = (array[num].NodeFlags & ~FormatStore.NodeFlags.Visited);
		}

		public PropertyValue GetProperty(PropertyId id)
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			if (FlagProperties.IsFlagProperty(id))
			{
				return array[num].FlagProperties.GetPropertyValue(id);
			}
			if (array[num].PropertyMask.IsSet(id))
			{
				for (int i = 0; i < array[num].Properties.Length; i++)
				{
					Property property = array[num].Properties[i];
					if (property.Id == id)
					{
						return property.Value;
					}
					if (property.Id > id)
					{
						break;
					}
				}
			}
			return PropertyValue.Null;
		}

		public void SetProperty(PropertyId id, PropertyValue value)
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			if (FlagProperties.IsFlagProperty(id))
			{
				array[num].FlagProperties.SetPropertyValue(id, value);
				return;
			}
			int i = 0;
			if (array[num].Properties != null)
			{
				while (i < array[num].Properties.Length)
				{
					Property property = array[num].Properties[i];
					if (property.Id == id)
					{
						array[num].Properties[i].Set(id, value);
						return;
					}
					if (property.Id > id)
					{
						break;
					}
					i++;
				}
			}
			if (array[num].Properties == null)
			{
				array[num].Properties = new Property[1];
				array[num].Properties[0].Set(id, value);
				array[num].PropertyMask.Set(id);
				return;
			}
			Property[] array2 = new Property[array[num].Properties.Length + 1];
			if (i != 0)
			{
				Array.Copy(array[num].Properties, 0, array2, 0, i);
			}
			if (i != array[num].Properties.Length)
			{
				Array.Copy(array[num].Properties, i, array2, i + 1, array[num].Properties.Length - i);
			}
			array2[i].Set(id, value);
			array[num].Properties = array2;
			array[num].PropertyMask.Set(id);
		}

		public void AppendChild(FormatNode newChildNode)
		{
			FormatNode.InternalAppendChild(this.nodes, this.nodeHandle, newChildNode.Handle);
		}

		public void PrependChild(FormatNode newChildNode)
		{
			FormatNode.InternalPrependChild(this.nodes, this.nodeHandle, newChildNode.Handle);
		}

		public void InsertSiblingAfter(FormatNode newSiblingNode)
		{
			int num = newSiblingNode.nodeHandle;
			FormatStore.NodeEntry[] array = this.nodes.Plane(num);
			int num2 = this.nodes.Index(num);
			FormatStore.NodeEntry[] array2 = this.nodes.Plane(this.nodeHandle);
			int num3 = this.nodes.Index(this.nodeHandle);
			int parent = array2[num3].Parent;
			FormatStore.NodeEntry[] array3 = this.nodes.Plane(parent);
			int num4 = this.nodes.Index(parent);
			array[num2].Parent = parent;
			array[num2].NextSibling = array2[num3].NextSibling;
			array2[num3].NextSibling = num;
			if (this.nodeHandle == array3[num4].LastChild)
			{
				array3[num4].LastChild = num;
			}
		}

		public void InsertSiblingBefore(FormatNode newSiblingNode)
		{
			int num = newSiblingNode.nodeHandle;
			FormatStore.NodeEntry[] array = this.nodes.Plane(num);
			int num2 = this.nodes.Index(num);
			FormatStore.NodeEntry[] array2 = this.nodes.Plane(this.nodeHandle);
			int num3 = this.nodes.Index(this.nodeHandle);
			int parent = array2[num3].Parent;
			FormatStore.NodeEntry[] array3 = this.nodes.Plane(parent);
			int num4 = this.nodes.Index(parent);
			int handle = array3[num4].LastChild;
			FormatStore.NodeEntry[] array4 = this.nodes.Plane(handle);
			int num5 = this.nodes.Index(handle);
			while (array4[num5].NextSibling != this.nodeHandle)
			{
				handle = array4[num5].NextSibling;
				array4 = this.nodes.Plane(handle);
				num5 = this.nodes.Index(handle);
			}
			array[num2].Parent = parent;
			array[num2].NextSibling = this.nodeHandle;
			array4[num5].NextSibling = num;
		}

		public void RemoveFromParent()
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			int parent = array[num].Parent;
			FormatStore.NodeEntry[] array2 = this.nodes.Plane(parent);
			int num2 = this.nodes.Index(parent);
			int nextSibling = array[num].NextSibling;
			if (this.nodeHandle == nextSibling)
			{
				array2[num2].LastChild = 0;
			}
			else
			{
				int num3 = array2[num2].LastChild;
				FormatStore.NodeEntry[] array3 = this.nodes.Plane(num3);
				int num4 = this.nodes.Index(num3);
				while (array3[num4].NextSibling != this.nodeHandle)
				{
					num3 = array3[num4].NextSibling;
					array3 = this.nodes.Plane(num3);
					num4 = this.nodes.Index(num3);
				}
				array3[num4].NextSibling = array[num].NextSibling;
				if (array2[num2].LastChild == this.nodeHandle)
				{
					array2[num2].LastChild = num3;
				}
			}
			array[num].Parent = 0;
		}

		public void MoveAllChildrenToNewParent(FormatNode newParent)
		{
			while (!this.FirstChild.IsNull)
			{
				FormatNode firstChild = this.FirstChild;
				firstChild.RemoveFromParent();
				newParent.AppendChild(firstChild);
			}
		}

		public void ChangeNodeType(FormatContainerType newType)
		{
			this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].Type = newType;
		}

		public void PrepareToClose(uint endTextPosition)
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].EndTextPosition = endTextPosition;
			FormatStore.NodeEntry[] array2 = array;
			int num2 = num;
			array2[num2].NodeFlags = (array2[num2].NodeFlags & ~FormatStore.NodeFlags.OnRightEdge);
			FormatStore.NodeEntry[] array3 = array;
			int num3 = num;
			array3[num3].NodeFlags = (array3[num3].NodeFlags | FormatStore.NodeFlags.CanFlush);
		}

		public void SetProps(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] properties, int inheritanceMaskIndex)
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			array[num].FlagProperties = flagProperties;
			array[num].PropertyMask = propertyMask;
			array[num].Properties = properties;
			array[num].InheritanceMaskIndex = inheritanceMaskIndex;
		}

		public FormatNode SplitTextNode(uint splitPosition)
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			int handle = this.nodes.Allocate(FormatContainerType.Text, array[num].BeginTextPosition);
			FormatStore.NodeEntry[] array2 = this.nodes.Plane(handle);
			int num2 = this.nodes.Index(handle);
			array2[num2].NodeFlags = array[num].NodeFlags;
			array2[num2].TextMapping = array[num].TextMapping;
			array2[num2].EndTextPosition = splitPosition;
			array2[num2].FlagProperties = array[num].FlagProperties;
			array2[num2].PropertyMask = array[num].PropertyMask;
			array2[num2].Properties = array[num].Properties;
			array[num].BeginTextPosition = splitPosition;
			FormatNode formatNode = new FormatNode(this.nodes, handle);
			this.InsertSiblingBefore(formatNode);
			return formatNode;
		}

		public FormatNode SplitNodeBeforeChild(FormatNode child)
		{
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			int handle = this.nodes.Allocate(this.NodeType, array[num].BeginTextPosition);
			FormatStore.NodeEntry[] array2 = this.nodes.Plane(handle);
			int num2 = this.nodes.Index(handle);
			array2[num2].NodeFlags = array[num].NodeFlags;
			array2[num2].TextMapping = array[num].TextMapping;
			array2[num2].EndTextPosition = child.BeginTextPosition;
			array2[num2].FlagProperties = array[num].FlagProperties;
			array2[num2].PropertyMask = array[num].PropertyMask;
			array2[num2].Properties = array[num].Properties;
			array[num].BeginTextPosition = child.BeginTextPosition;
			FormatNode formatNode = new FormatNode(this.nodes, handle);
			do
			{
				FormatNode firstChild = this.FirstChild;
				firstChild.RemoveFromParent();
				formatNode.AppendChild(firstChild);
			}
			while (this.FirstChild != child);
			this.InsertSiblingBefore(formatNode);
			return formatNode;
		}

		public FormatNode DuplicateInsertAsChild()
		{
			int handle = this.nodes.Allocate(this.NodeType, this.BeginTextPosition);
			FormatStore.NodeEntry[] array = this.nodes.Plane(this.nodeHandle);
			int num = this.nodes.Index(this.nodeHandle);
			FormatStore.NodeEntry[] array2 = this.nodes.Plane(handle);
			int num2 = this.nodes.Index(handle);
			array2[num2].NodeFlags = array[num].NodeFlags;
			array2[num2].TextMapping = array[num].TextMapping;
			array2[num2].EndTextPosition = array[num].EndTextPosition;
			array2[num2].FlagProperties = array[num].FlagProperties;
			array2[num2].PropertyMask = array[num].PropertyMask;
			array2[num2].Properties = array[num].Properties;
			FormatNode formatNode = new FormatNode(this.nodes, handle);
			this.MoveAllChildrenToNewParent(formatNode);
			this.AppendChild(formatNode);
			return formatNode;
		}

		public override bool Equals(object obj)
		{
			return obj is FormatNode && this.nodes == ((FormatNode)obj).nodes && this.nodeHandle == ((FormatNode)obj).nodeHandle;
		}

		public override int GetHashCode()
		{
			return this.nodeHandle;
		}

		internal static void InternalAppendChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
		{
			FormatNode.InternalPrependChild(nodes, thisNode, newChildNode);
			nodes.Plane(thisNode)[nodes.Index(thisNode)].LastChild = newChildNode;
		}

		internal static void InternalPrependChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
		{
			FormatStore.NodeEntry[] array = nodes.Plane(thisNode);
			int num = nodes.Index(thisNode);
			FormatStore.NodeEntry[] array2 = nodes.Plane(newChildNode);
			int num2 = nodes.Index(newChildNode);
			if (array[num].LastChild != 0)
			{
				int lastChild = array[num].LastChild;
				FormatStore.NodeEntry[] array3 = nodes.Plane(lastChild);
				int num3 = nodes.Index(lastChild);
				array2[num2].NextSibling = array3[num3].NextSibling;
				array3[num3].NextSibling = newChildNode;
				array2[num2].Parent = thisNode;
				return;
			}
			array2[num2].NextSibling = newChildNode;
			array2[num2].Parent = thisNode;
			array[num].LastChild = newChildNode;
		}

		public static readonly FormatNode Null = default(FormatNode);

		private FormatStore.NodeStore nodes;

		private int nodeHandle;

		internal struct NodeSubtree : IEnumerable<FormatNode>, IEnumerable
		{
			internal NodeSubtree(FormatNode node)
			{
				this.node = node;
			}

			public FormatNode.SubtreeEnumerator GetEnumerator()
			{
				return new FormatNode.SubtreeEnumerator(this.node, false);
			}

			public FormatNode.SubtreeEnumerator GetEnumerator(bool revisitParent)
			{
				return new FormatNode.SubtreeEnumerator(this.node, revisitParent);
			}

			IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
			{
				return new FormatNode.SubtreeEnumerator(this.node, false);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new FormatNode.SubtreeEnumerator(this.node, false);
			}

			private FormatNode node;
		}

		internal struct NodeChildren : IEnumerable<FormatNode>, IEnumerable
		{
			internal NodeChildren(FormatNode node)
			{
				this.node = node;
			}

			public FormatNode.ChildrenEnumerator GetEnumerator()
			{
				return new FormatNode.ChildrenEnumerator(this.node);
			}

			IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
			{
				return new FormatNode.ChildrenEnumerator(this.node);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new FormatNode.ChildrenEnumerator(this.node);
			}

			private FormatNode node;
		}

		internal struct ChildrenEnumerator : IEnumerator<FormatNode>, IDisposable, IEnumerator
		{
			internal ChildrenEnumerator(FormatNode node)
			{
				this.node = node;
				this.current = FormatNode.Null;
				this.next = this.node.FirstChild;
			}

			public FormatNode Current
			{
				get
				{
					if (this.current.IsNull)
					{
						throw new InvalidOperationException(this.next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");
					}
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.current.IsNull)
					{
						throw new InvalidOperationException(this.next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");
					}
					return this.current;
				}
			}

			public bool MoveNext()
			{
				this.current = this.next;
				if (this.current.IsNull)
				{
					return false;
				}
				this.next = this.current.NextSibling;
				return true;
			}

			public void Reset()
			{
				this.current = FormatNode.Null;
				this.next = this.node.FirstChild;
			}

			public void Dispose()
			{
				this.Reset();
			}

			private FormatNode node;

			private FormatNode current;

			private FormatNode next;
		}

		internal struct SubtreeEnumerator : IEnumerator<FormatNode>, IDisposable, IEnumerator
		{
			internal SubtreeEnumerator(FormatNode node, bool revisitParent)
			{
				this.revisitParent = revisitParent;
				this.root = node;
				this.current = FormatNode.Null;
				this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition)0;
				this.nextChild = node;
				this.depth = -1;
			}

			public FormatNode Current
			{
				get
				{
					return this.current;
				}
			}

			public bool FirstVisit
			{
				get
				{
					return 0 != (byte)(this.currentDisposition & FormatNode.SubtreeEnumerator.EnumeratorDisposition.Begin);
				}
			}

			public bool LastVisit
			{
				get
				{
					return 0 != (byte)(this.currentDisposition & FormatNode.SubtreeEnumerator.EnumeratorDisposition.End);
				}
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			public bool MoveNext()
			{
				if (this.nextChild != FormatNode.Null)
				{
					this.depth++;
					this.current = this.nextChild;
					this.nextChild = this.current.FirstChild;
					this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition)(1 | ((this.nextChild == FormatNode.Null) ? 2 : 0));
					return true;
				}
				if (this.depth < 0)
				{
					return false;
				}
				for (;;)
				{
					this.depth--;
					if (this.depth < 0)
					{
						break;
					}
					this.nextChild = this.current.NextSibling;
					this.current = this.current.Parent;
					this.currentDisposition = ((this.nextChild == FormatNode.Null) ? FormatNode.SubtreeEnumerator.EnumeratorDisposition.End : ((FormatNode.SubtreeEnumerator.EnumeratorDisposition)0));
					if (this.revisitParent || !(this.nextChild == FormatNode.Null))
					{
						goto IL_FA;
					}
				}
				this.current = FormatNode.Null;
				this.nextChild = FormatNode.Null;
				this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition)0;
				return false;
				IL_FA:
				return this.revisitParent || this.MoveNext();
			}

			public FormatNode PreviewNextNode()
			{
				if (this.nextChild != FormatNode.Null)
				{
					return this.nextChild;
				}
				if (this.depth < 0)
				{
					return FormatNode.Null;
				}
				int num = this.depth;
				FormatNode parent = this.current;
				for (;;)
				{
					num--;
					if (num < 0)
					{
						break;
					}
					FormatNode nextSibling = parent.NextSibling;
					parent = parent.Parent;
					if (this.revisitParent || !(nextSibling == FormatNode.Null))
					{
						goto IL_69;
					}
				}
				return FormatNode.Null;
				IL_69:
				if (!this.revisitParent)
				{
					FormatNode nextSibling;
					return nextSibling;
				}
				return parent;
			}

			public void SkipChildren()
			{
				if (this.nextChild != FormatNode.Null)
				{
					this.nextChild = FormatNode.Null;
					this.currentDisposition |= FormatNode.SubtreeEnumerator.EnumeratorDisposition.End;
				}
			}

			void IEnumerator.Reset()
			{
				this.current = FormatNode.Null;
				this.currentDisposition = (FormatNode.SubtreeEnumerator.EnumeratorDisposition)0;
				this.nextChild = this.root;
				this.depth = -1;
			}

			void IDisposable.Dispose()
			{
				((IEnumerator)this).Reset();
				GC.SuppressFinalize(this);
			}

			private bool revisitParent;

			private FormatNode.SubtreeEnumerator.EnumeratorDisposition currentDisposition;

			private FormatNode root;

			private FormatNode current;

			private FormatNode nextChild;

			private int depth;

			[Flags]
			private enum EnumeratorDisposition : byte
			{
				Begin = 1,
				End = 2
			}
		}
	}
}
