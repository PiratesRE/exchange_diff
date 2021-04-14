using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Analysis.Builders;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis
{
	internal sealed class Setting<T> : AnalysisMember<T>
	{
		private Setting(Func<AnalysisMember> parent, ConcurrencyType runAs, Analysis analysis, IEnumerable<Feature> features, Func<Result, IEnumerable<Result<T>>> setFunction) : base(parent, runAs, analysis, features, setFunction)
		{
		}

		public static SettingParentBuilder<T> Build()
		{
			SettingBuildContext<T> context = new SettingBuildContext<T>((SettingBuildContext<T> x) => new Setting<T>(x.Parent, x.RunAs, x.Analysis, x.Features, x.SetFunction));
			return new SettingParentBuilder<T>(context);
		}
	}
}
