using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class IntermediateNodePool
	{
		public void Reclaim(IntermediateNode node)
		{
			if (node.ChildrenMap != null)
			{
				foreach (IntermediateNode node2 in node.ChildrenMap.Values)
				{
					this.Reclaim(node2);
				}
				node.ChildrenMap.Clear();
			}
			this.pool.Add(node);
		}

		public IntermediateNode Create(char ch)
		{
			if (this.pool.Count > 0)
			{
				IntermediateNode intermediateNode = this.pool[this.pool.Count - 1];
				this.pool.RemoveAt(this.pool.Count - 1);
				intermediateNode.Reinitialize(ch);
				return intermediateNode;
			}
			return new IntermediateNode(ch);
		}

		public IntermediateNode Create(string value = "")
		{
			if (this.pool.Count > 0)
			{
				IntermediateNode intermediateNode = this.pool[this.pool.Count - 1];
				this.pool.RemoveAt(this.pool.Count - 1);
				intermediateNode.Reinitialize(value);
				return intermediateNode;
			}
			return new IntermediateNode(value);
		}

		private List<IntermediateNode> pool = new List<IntermediateNode>(16);
	}
}
