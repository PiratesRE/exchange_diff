using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class RuleCollection : IList<Rule>, ICollection<Rule>, IEnumerable<Rule>, IEnumerable
	{
		public RuleCollection(string name)
		{
			this.name = name;
			this.historyPropertyName = "Microsoft.Exchange." + name + ".RuleCollectionHistory";
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Count
		{
			get
			{
				return this.rules.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public int CountAllNotDisabled
		{
			get
			{
				int num = 0;
				foreach (Rule rule in this.rules)
				{
					if (rule.Enabled != RuleState.Disabled)
					{
						num++;
					}
				}
				return num;
			}
		}

		public string HistoryPropertyName
		{
			get
			{
				return this.historyPropertyName;
			}
		}

		public Rule this[int index]
		{
			get
			{
				return this.rules[index];
			}
			set
			{
				this.CheckNameNotExistsExcept(value.Name, index);
				this.rules[index] = value;
			}
		}

		public Rule this[string ruleName]
		{
			get
			{
				foreach (Rule rule in this.rules)
				{
					if (rule.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase))
					{
						return rule;
					}
				}
				return null;
			}
		}

		public int IndexOf(Rule rule)
		{
			return this.rules.IndexOf(rule);
		}

		public void RemoveAt(int index)
		{
			this.rules.RemoveAt(index);
		}

		public bool Remove(Rule rule)
		{
			return this.rules.Remove(rule);
		}

		public void Insert(int index, Rule rule)
		{
			this.CheckNameNotExists(rule.Name);
			this.rules.Insert(index, rule);
		}

		public void Add(Rule rule)
		{
			this.CheckNameNotExists(rule.Name);
			this.rules.Add(rule);
		}

		internal void AddWithoutNameCheck(Rule rule)
		{
			this.rules.Add(rule);
		}

		public void Clear()
		{
			this.rules.Clear();
		}

		public bool Contains(Rule rule)
		{
			return this.rules.Contains(rule);
		}

		public void CopyTo(Rule[] array, int arrayIndex)
		{
			this.rules.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Rule> GetEnumerator()
		{
			return this.rules.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.rules.GetEnumerator();
		}

		public bool Remove(string name)
		{
			foreach (Rule rule in this.rules)
			{
				if (rule.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return this.rules.Remove(rule);
				}
			}
			return false;
		}

		public int GetEstimatedSize()
		{
			int num = 0;
			foreach (Rule rule in this.rules)
			{
				num += rule.GetEstimatedSize();
			}
			return num;
		}

		private void CheckNameNotExists(string name)
		{
			foreach (Rule rule in this.rules)
			{
				if (rule.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					throw new RulesValidationException(RulesStrings.RuleNameExists(name));
				}
			}
		}

		private void CheckNameNotExistsExcept(string name, int index)
		{
			for (int i = 0; i < this.rules.Count; i++)
			{
				if (this.rules[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase) && i != index)
				{
					throw new RulesValidationException(RulesStrings.RuleNameExists(name));
				}
			}
		}

		private string name;

		private ShortList<Rule> rules = new ShortList<Rule>();

		private string historyPropertyName;
	}
}
