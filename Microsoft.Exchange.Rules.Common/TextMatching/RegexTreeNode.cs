using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexTreeNode
	{
		public RegexTreeNode(char ch, int stateid) : this(RegexTreeNode.NodeType.Leaf, stateid)
		{
			this.charClass = new RegexCharacterClass(ch);
			if (ch == RegexParser.EOS)
			{
				this.end = true;
			}
		}

		public RegexTreeNode(RegexCharacterClass.ValueType charClass, int stateid) : this(RegexTreeNode.NodeType.Leaf, stateid)
		{
			this.charClass = new RegexCharacterClass(charClass);
		}

		public RegexTreeNode(RegexTreeNode.NodeType nodeType, int stateid)
		{
			this.nodeType = nodeType;
			this.stateid = stateid;
		}

		public int State
		{
			get
			{
				return this.stateid;
			}
		}

		public RegexTreeNode Left
		{
			get
			{
				return this.left;
			}
			set
			{
				this.left = value;
			}
		}

		public RegexTreeNode Right
		{
			get
			{
				return this.right;
			}
			set
			{
				this.right = value;
			}
		}

		public RegexTreeNode.NodeType Type
		{
			get
			{
				return this.nodeType;
			}
		}

		public RegexCharacterClass CharClass
		{
			get
			{
				return this.charClass;
			}
		}

		public List<RegexTreeNode> FirstPos
		{
			get
			{
				return this.firstPos;
			}
		}

		public List<RegexTreeNode> FollowPos
		{
			get
			{
				return this.followPos;
			}
		}

		public bool End
		{
			get
			{
				return this.end;
			}
		}

		public void ComputeNFL()
		{
			this.ComputeNFL(this);
			this.ComputeFollowPos(this);
		}

		private static List<RegexTreeNode> CreateList(RegexTreeNode node)
		{
			return new List<RegexTreeNode>(1)
			{
				node
			};
		}

		private static List<RegexTreeNode> CopyList(List<RegexTreeNode> list)
		{
			List<RegexTreeNode> result = null;
			if (list != null)
			{
				result = new List<RegexTreeNode>(list);
			}
			return result;
		}

		private static List<RegexTreeNode> CombineList(List<RegexTreeNode> list1, List<RegexTreeNode> list2)
		{
			if (list1 != null)
			{
				if (list2 == null)
				{
					return new List<RegexTreeNode>(list1);
				}
				List<RegexTreeNode> list3 = new List<RegexTreeNode>(list1.Count + list2.Count);
				list3.AddRange(list1);
				list3.AddRange(list2);
				return list3;
			}
			else
			{
				if (list2 != null)
				{
					return new List<RegexTreeNode>(list2);
				}
				return null;
			}
		}

		private void ComputeNFL(RegexTreeNode node)
		{
			if (node != null)
			{
				this.ComputeNFL(node.Left);
				this.ComputeNFL(node.Right);
				switch (node.nodeType)
				{
				case RegexTreeNode.NodeType.Leaf:
					node.nullable = false;
					node.firstPos = RegexTreeNode.CreateList(node);
					node.lastPos = RegexTreeNode.CreateList(node);
					return;
				case RegexTreeNode.NodeType.Bar:
					if (node.Left.nullable || node.Right.nullable)
					{
						node.nullable = true;
					}
					node.firstPos = RegexTreeNode.CombineList(node.Left.firstPos, node.Right.firstPos);
					node.lastPos = RegexTreeNode.CombineList(node.Left.lastPos, node.Right.lastPos);
					return;
				case RegexTreeNode.NodeType.Cat:
					if (node.Left.nullable && node.Right.nullable)
					{
						node.nullable = true;
					}
					if (node.Left.nullable)
					{
						node.firstPos = RegexTreeNode.CombineList(node.Left.firstPos, node.Right.firstPos);
					}
					else
					{
						node.firstPos = RegexTreeNode.CopyList(node.Left.firstPos);
					}
					if (node.Right.nullable)
					{
						node.lastPos = RegexTreeNode.CombineList(node.Left.lastPos, node.Right.lastPos);
						return;
					}
					node.lastPos = RegexTreeNode.CopyList(node.Right.lastPos);
					return;
				case RegexTreeNode.NodeType.Star:
					node.nullable = true;
					if (node.Left != null)
					{
						node.firstPos = RegexTreeNode.CopyList(node.Left.firstPos);
						node.lastPos = RegexTreeNode.CopyList(node.Left.firstPos);
					}
					break;
				default:
					return;
				}
			}
		}

		private void ComputeFollowPos(RegexTreeNode node)
		{
			if (node != null)
			{
				this.ComputeFollowPos(node.Left);
				if (node.nodeType == RegexTreeNode.NodeType.Cat)
				{
					if (node.Left.lastPos == null)
					{
						goto IL_D2;
					}
					using (List<RegexTreeNode>.Enumerator enumerator = node.Left.lastPos.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							RegexTreeNode regexTreeNode = enumerator.Current;
							regexTreeNode.followPos = RegexTreeNode.CombineList(regexTreeNode.followPos, node.Right.firstPos);
						}
						goto IL_D2;
					}
				}
				if (node.nodeType == RegexTreeNode.NodeType.Star && node.lastPos != null)
				{
					foreach (RegexTreeNode regexTreeNode2 in node.lastPos)
					{
						regexTreeNode2.followPos = RegexTreeNode.CombineList(regexTreeNode2.followPos, node.firstPos);
					}
				}
				IL_D2:
				this.ComputeFollowPos(node.Right);
			}
		}

		private int stateid;

		private bool end;

		private List<RegexTreeNode> firstPos;

		private List<RegexTreeNode> lastPos;

		private List<RegexTreeNode> followPos;

		private RegexTreeNode.NodeType nodeType;

		private RegexCharacterClass charClass;

		private RegexTreeNode left;

		private RegexTreeNode right;

		private bool nullable;

		internal enum NodeType
		{
			Leaf,
			Bar,
			Cat,
			Star,
			LeftParen,
			RightParen
		}
	}
}
