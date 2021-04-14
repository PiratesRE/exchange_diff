using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConditionList : IList<Condition>, ICollection<Condition>, IEnumerable<Condition>, IEnumerable
	{
		public ConditionList(Rule rule)
		{
			this.internalList = new List<Condition>();
			this.rule = rule;
		}

		int IList<Condition>.IndexOf(Condition condition)
		{
			return this.internalList.IndexOf(condition);
		}

		void IList<Condition>.Insert(int index, Condition condition)
		{
			this.internalList.Insert(index, condition);
			this.rule.SetDirty();
		}

		void IList<Condition>.RemoveAt(int index)
		{
			this.internalList.RemoveAt(index);
			this.rule.SetDirty();
		}

		Condition IList<Condition>.this[int index]
		{
			get
			{
				return this.internalList[index];
			}
			set
			{
				this.internalList[index] = value;
				this.rule.SetDirty();
			}
		}

		void ICollection<Condition>.Add(Condition condition)
		{
			this.CheckForDuplicate(condition.ConditionType);
			this.internalList.Add(condition);
			this.rule.SetDirty();
		}

		void ICollection<Condition>.Clear()
		{
			this.internalList.Clear();
			this.rule.SetDirty();
		}

		bool ICollection<Condition>.Contains(Condition condition)
		{
			return this.internalList.Contains(condition);
		}

		void ICollection<Condition>.CopyTo(Condition[] conditions, int index)
		{
			this.internalList.CopyTo(conditions, index);
		}

		bool ICollection<Condition>.Remove(Condition condition)
		{
			this.rule.SetDirty();
			return this.internalList.Remove(condition);
		}

		int ICollection<Condition>.Count
		{
			get
			{
				return this.internalList.Count;
			}
		}

		bool ICollection<Condition>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		IEnumerator<Condition> IEnumerable<Condition>.GetEnumerator()
		{
			return this.internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.internalList.GetEnumerator();
		}

		public Condition[] ToArray()
		{
			return this.internalList.ToArray();
		}

		private void CheckForDuplicate(ConditionType type)
		{
			foreach (Condition condition in ((IEnumerable<Condition>)this))
			{
				if (condition.ConditionType == type)
				{
					this.rule.ThrowValidateException(delegate
					{
						throw new ArgumentException(ServerStrings.DuplicateCondition);
					}, ServerStrings.DuplicateCondition);
				}
			}
		}

		private List<Condition> internalList;

		private Rule rule;
	}
}
