using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class SettingBuildContext<T> : BuildContext<T>
	{
		public SettingBuildContext(Func<SettingBuildContext<T>, AnalysisMember<T>> constructor)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
			this.constructor = constructor;
			this.SetFunction = null;
		}

		public Func<Result, IEnumerable<Result<T>>> SetFunction { get; set; }

		public override AnalysisMember<T> Construct()
		{
			return this.constructor(this);
		}

		private Func<SettingBuildContext<T>, AnalysisMember<T>> constructor;
	}
}
