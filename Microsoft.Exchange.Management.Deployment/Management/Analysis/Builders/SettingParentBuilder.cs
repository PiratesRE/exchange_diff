using System;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class SettingParentBuilder<T>
	{
		public SettingParentBuilder(SettingBuildContext<T> context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public SettingInBuilder<T, TParent> WithParent<TParent>(Func<AnalysisMember<TParent>> parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			this.context.Parent = parent;
			return new SettingInBuilder<T, TParent>(this.context);
		}

		public SettingInBuilder<T, object> AsRootSetting()
		{
			this.context.Parent = null;
			return new SettingInBuilder<T, object>(this.context);
		}

		private SettingBuildContext<T> context;
	}
}
