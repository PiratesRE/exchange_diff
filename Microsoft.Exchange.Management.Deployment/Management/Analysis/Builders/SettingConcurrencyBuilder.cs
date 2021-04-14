using System;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class SettingConcurrencyBuilder<T, TParent>
	{
		public SettingConcurrencyBuilder(SettingBuildContext<T> context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public SettingBuilder<T, TParent> AsSync()
		{
			this.context.RunAs = ConcurrencyType.Synchronous;
			return new SettingBuilder<T, TParent>(this.context);
		}

		public SettingBuilder<T, TParent> AsAsync()
		{
			this.context.RunAs = ConcurrencyType.ASynchronous;
			return new SettingBuilder<T, TParent>(this.context);
		}

		private SettingBuildContext<T> context;
	}
}
