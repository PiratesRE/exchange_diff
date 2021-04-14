using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConditionCollection : CollectionBase
	{
		public int Add(Condition c)
		{
			return base.List.Add(c);
		}

		public int Add(ConditionCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			int result = base.Count;
			foreach (object obj in collection)
			{
				Condition value = (Condition)obj;
				result = base.List.Add(value);
			}
			return result;
		}

		public bool AddUnique(Condition newCondition, bool removeInconsistencies)
		{
			bool flag = false;
			for (int i = 0; i < base.Count; i++)
			{
				Condition condition = this[i];
				if (newCondition.Match(condition))
				{
					if (newCondition.Role.ExpectedResult != condition.Role.ExpectedResult)
					{
						if (!removeInconsistencies)
						{
							throw new ConditionException(newCondition);
						}
						base.RemoveAt(i);
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.Add(newCondition);
			}
			return !flag;
		}

		public Condition this[int index]
		{
			get
			{
				return (Condition)base.List[index];
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("{");
			for (int i = 0; i < base.List.Count; i++)
			{
				stringBuilder.Append(base.List[i].ToString());
				if (i + 1 < base.List.Count)
				{
					stringBuilder.Append(",");
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
