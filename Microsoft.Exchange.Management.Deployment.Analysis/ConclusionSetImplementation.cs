using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class ConclusionSetImplementation<TConclusion, TSettingConclusion, TRuleConclusion> where TConclusion : ConclusionImplementation<TConclusion, TSettingConclusion, TRuleConclusion> where TSettingConclusion : TConclusion where TRuleConclusion : TConclusion, IRuleConclusion
	{
		protected ConclusionSetImplementation()
		{
		}

		protected ConclusionSetImplementation(TConclusion root)
		{
			this.root = root;
		}

		public TConclusion Root
		{
			get
			{
				return this.root;
			}
			set
			{
				this.ThrowIfReadOnly();
				this.root = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public IEnumerable<TConclusion> Conclusions()
		{
			return this.root.DescendantsAndSelfWithoutExceptions();
		}

		public IEnumerable<TConclusion> Conclusions(string name)
		{
			return this.root.DescendantsAndSelfWithoutExceptions(name);
		}

		public IEnumerable<TConclusion> Exceptions()
		{
			return this.root.Exceptions();
		}

		public IEnumerable<TConclusion> Exceptions(string name)
		{
			return this.root.Exceptions(name);
		}

		public IEnumerable<TSettingConclusion> Settings()
		{
			return this.root.Settings();
		}

		public IEnumerable<TSettingConclusion> Settings(string name)
		{
			return this.root.Settings(name);
		}

		public IEnumerable<TRuleConclusion> Rules()
		{
			return this.root.Rules();
		}

		public IEnumerable<TRuleConclusion> Rules(string name)
		{
			return this.root.Rules(name);
		}

		public IEnumerable<TRuleConclusion> Errors()
		{
			return this.root.Errors();
		}

		public IEnumerable<TRuleConclusion> Errors(string name)
		{
			return this.root.Errors(name);
		}

		public IEnumerable<TRuleConclusion> Warnings()
		{
			return this.root.Warnings();
		}

		public IEnumerable<TRuleConclusion> Warnings(string name)
		{
			return this.root.Warnings(name);
		}

		public IEnumerable<TRuleConclusion> Info()
		{
			return this.root.Info();
		}

		public IEnumerable<TRuleConclusion> Info(string name)
		{
			return this.root.Info(name);
		}

		public void MakeReadOnly()
		{
			if (this.IsReadOnly)
			{
				return;
			}
			this.root.MakeReadOnly();
			this.isReadOnly = true;
		}

		protected void ThrowIfReadOnly()
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException(Strings.CannotModifyReadOnlyProperty);
			}
		}

		private TConclusion root;

		private bool isReadOnly;
	}
}
