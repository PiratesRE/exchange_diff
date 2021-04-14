using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexState : StateNode
	{
		public RegexState(int stateid) : base(stateid, false)
		{
			this.list = new List<int>();
		}

		public RegexState(int stateid, List<RegexTreeNode> nodes) : this(stateid)
		{
			this.Add(nodes);
		}

		public bool Marked
		{
			get
			{
				return this.marked;
			}
			set
			{
				this.marked = value;
			}
		}

		public bool IsStartState
		{
			get
			{
				return this.startState;
			}
			set
			{
				this.startState = value;
			}
		}

		public bool Contains(int stateid)
		{
			foreach (int num in this.list)
			{
				if (num == stateid)
				{
					return true;
				}
			}
			return false;
		}

		public void Add(List<RegexTreeNode> nodes)
		{
			if (nodes == null)
			{
				return;
			}
			int count = this.list.Count;
			if (this.list.Count == 0)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					this.list.Add(nodes[i].State);
					if (nodes[i].End)
					{
						base.IsFinal = true;
					}
				}
				return;
			}
			for (int j = 0; j < nodes.Count; j++)
			{
				int state = nodes[j].State;
				int num = 0;
				while (num < count && state != this.list[num])
				{
					num++;
				}
				if (num == count)
				{
					this.list.Add(state);
					if (nodes[j].End)
					{
						base.IsFinal = true;
					}
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			RegexState regexState = (RegexState)obj;
			if (this.list.Count != regexState.list.Count)
			{
				return false;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				int num = 0;
				while (num < regexState.list.Count && this.list[i] != regexState.list[num])
				{
					num++;
				}
				if (num == regexState.list.Count)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool marked;

		private bool startState;

		private List<int> list;
	}
}
