using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class ConclusionImplementation<TConclusion, TSettingConclusion, TRuleConclusion> : Conclusion where TConclusion : ConclusionImplementation<TConclusion, TSettingConclusion, TRuleConclusion> where TSettingConclusion : TConclusion where TRuleConclusion : TConclusion, IRuleConclusion
	{
		protected ConclusionImplementation()
		{
			this.children = new List<TConclusion>();
		}

		protected ConclusionImplementation(Result result) : base(result)
		{
			this.children = new List<TConclusion>();
		}

		public TConclusion Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				base.ThrowIfReadOnly();
				this.parent = value;
			}
		}

		public IList<TConclusion> Children
		{
			get
			{
				return this.children;
			}
			set
			{
				base.ThrowIfReadOnly();
				this.children = new ReadOnlyCollection<TConclusion>(this.children);
			}
		}

		public IEnumerable<TConclusion> ChildrenWithName(string name)
		{
			return from x in this.children
			where string.Equals(name, x.Name, StringComparison.Ordinal)
			select x;
		}

		public IEnumerable<TConclusion> ChildrenWithoutExceptions()
		{
			return from x in this.children
			where !x.HasException
			select x;
		}

		public IEnumerable<TConclusion> ChildrenWithoutExceptions(string name)
		{
			return from x in this.ChildrenWithName(name)
			where !x.HasException
			select x;
		}

		public IEnumerable<TConclusion> AncestorsAndSelf()
		{
			for (TConclusion current = (TConclusion)((object)this); current != null; current = current.Parent)
			{
				yield return current;
			}
			yield break;
		}

		public IEnumerable<TConclusion> AncestorsAndSelf(string name)
		{
			return from x in this.AncestorsAndSelf()
			where string.Equals(name, x.Name, StringComparison.Ordinal)
			select x;
		}

		public IEnumerable<TConclusion> Ancestors()
		{
			return this.AncestorsAndSelf().Skip(1);
		}

		public IEnumerable<TConclusion> Ancestors(string name)
		{
			return this.AncestorsAndSelf(name).Skip(1);
		}

		public IEnumerable<TConclusion> DescendantsAndSelf()
		{
			Stack<TConclusion> stack = new Stack<TConclusion>();
			stack.Push((TConclusion)((object)this));
			while (stack.Count > 0)
			{
				TConclusion current = stack.Pop();
				yield return current;
				for (int i = current.Children.Count - 1; i >= 0; i--)
				{
					stack.Push(current.Children[i]);
				}
			}
			yield break;
		}

		public IEnumerable<TConclusion> DescendantsAndSelf(string name)
		{
			return from x in this.DescendantsAndSelf()
			where string.Equals(name, x.Name, StringComparison.Ordinal)
			select x;
		}

		public IEnumerable<TConclusion> DescendantsAndSelfWithoutExceptions()
		{
			return from x in this.DescendantsAndSelf()
			where !x.HasException
			select x;
		}

		public IEnumerable<TConclusion> DescendantsAndSelfWithoutExceptions(string name)
		{
			return from x in this.DescendantsAndSelf(name)
			where !x.HasException
			select x;
		}

		public IEnumerable<TConclusion> Descendants()
		{
			return this.DescendantsAndSelf().Skip(1);
		}

		public IEnumerable<TConclusion> Descendants(string name)
		{
			return this.DescendantsAndSelf(name).Skip(1);
		}

		public IEnumerable<TConclusion> DescendantsWithoutExceptions()
		{
			return from x in this.Descendants()
			where !x.HasException
			select x;
		}

		public IEnumerable<TConclusion> DescendantsWithoutExceptions(string name)
		{
			return from x in this.Descendants(name)
			where !x.HasException
			select x;
		}

		public IEnumerable<TConclusion> Exceptions()
		{
			return from x in this.DescendantsAndSelf()
			where x.HasException
			select x;
		}

		public IEnumerable<TConclusion> Exceptions(string name)
		{
			return from x in this.DescendantsAndSelf(name)
			where x.HasException
			select x;
		}

		public IEnumerable<TSettingConclusion> Settings()
		{
			return this.DescendantsAndSelfWithoutExceptions().OfType<TSettingConclusion>();
		}

		public IEnumerable<TSettingConclusion> Settings(string name)
		{
			return this.DescendantsAndSelfWithoutExceptions(name).OfType<TSettingConclusion>();
		}

		public IEnumerable<TRuleConclusion> Rules()
		{
			return this.DescendantsAndSelfWithoutExceptions().OfType<TRuleConclusion>();
		}

		public IEnumerable<TRuleConclusion> Rules(string name)
		{
			return this.DescendantsAndSelfWithoutExceptions(name).OfType<TRuleConclusion>();
		}

		public IEnumerable<TRuleConclusion> Errors()
		{
			return from x in this.Rules()
			where x.IsConditionMet && x.Severity == Severity.Error
			select x;
		}

		public IEnumerable<TRuleConclusion> Errors(string name)
		{
			return from x in this.Rules(name)
			where x.IsConditionMet && x.Severity == Severity.Error
			select x;
		}

		public IEnumerable<TRuleConclusion> Warnings()
		{
			return from x in this.Rules()
			where x.IsConditionMet && x.Severity == Severity.Warning
			select x;
		}

		public IEnumerable<TRuleConclusion> Warnings(string name)
		{
			return from x in this.Rules(name)
			where x.IsConditionMet && x.Severity == Severity.Warning
			select x;
		}

		public IEnumerable<TRuleConclusion> Info()
		{
			return from x in this.Rules()
			where x.IsConditionMet && x.Severity == Severity.Info
			select x;
		}

		public IEnumerable<TRuleConclusion> Info(string name)
		{
			return from x in this.Rules(name)
			where x.IsConditionMet && x.Severity == Severity.Info
			select x;
		}

		public sealed override void MakeReadOnly()
		{
			if (base.IsReadOnly)
			{
				return;
			}
			foreach (TConclusion tconclusion in this.Children)
			{
				tconclusion.MakeReadOnly();
			}
			base.MakeReadOnly();
			this.OnMakeReadonly();
		}

		protected virtual void OnMakeReadonly()
		{
		}

		private TConclusion parent;

		private IList<TConclusion> children;
	}
}
