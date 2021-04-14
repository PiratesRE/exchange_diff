using System;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class SettingInBuilder<T, TParent>
	{
		public SettingInBuilder(SettingBuildContext<T> context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public SettingConcurrencyBuilder<T, TParent> In(Analysis analysis)
		{
			if (analysis == null)
			{
				throw new ArgumentNullException("analysis");
			}
			this.context.Analysis = analysis;
			return new SettingConcurrencyBuilder<T, TParent>(this.context);
		}

		private SettingBuildContext<T> context;
	}
}
