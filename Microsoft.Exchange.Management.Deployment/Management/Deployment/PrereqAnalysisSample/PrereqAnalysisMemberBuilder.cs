using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal sealed class PrereqAnalysisMemberBuilder : AnalysisMemberBuilder
	{
		public Setting<TResult> Setting<TResult>(Func<TResult> setValue, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>))
		{
			return base.Setting<TResult>(setValue, evaluate.DefaultTo(Evaluate.OnDemand), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All))
			});
		}

		public Setting<TResult> Setting<TResult>(Func<IEnumerable<TResult>> setValues, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>))
		{
			return base.Setting<TResult>(setValues, evaluate.DefaultTo(Evaluate.OnDemand), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All))
			});
		}

		public Setting<TResult> Setting<TResult>(Func<IEnumerable<Result<TResult>>> setResults, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>))
		{
			return base.Setting<TResult>(setResults, evaluate.DefaultTo(Evaluate.OnDemand), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All))
			});
		}

		public Setting<TResult> Setting<TResult, TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, TResult> setValue, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>))
		{
			return base.Setting<TResult, TParent>(forEachResult, setValue, evaluate.DefaultTo(Evaluate.OnDemand), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All))
			});
		}

		public Setting<TResult> Setting<TResult, TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, IEnumerable<TResult>> setValues, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>))
		{
			return base.Setting<TResult, TParent>(forEachResult, setValues, evaluate.DefaultTo(Evaluate.OnDemand), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All))
			});
		}

		public Setting<TResult> Setting<TResult, TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<Result<TParent>, IEnumerable<Result<TResult>>> setResults, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>))
		{
			return base.Setting<TResult, TParent>(forEachResult, setResults, evaluate.DefaultTo(Evaluate.OnDemand), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All))
			});
		}

		public Rule Rule(Func<RuleResult, string> message, Func<bool> condition, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>), Optional<Severity> severity = default(Optional<Severity>))
		{
			return base.Rule(condition, evaluate.DefaultTo(Evaluate.OnDemand), severity.DefaultTo(Severity.Error), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All)),
				new MessageFeature((Result x) => message((RuleResult)x))
			});
		}

		public Rule Rule<TParent>(Func<AnalysisMember<TParent>> forEachResult, Func<RuleResult, string> message, Func<Result<TParent>, bool> condition, Optional<Evaluate> evaluate = default(Optional<Evaluate>), Optional<SetupRole> roles = default(Optional<SetupRole>), Optional<SetupMode> modes = default(Optional<SetupMode>), Optional<Severity> severity = default(Optional<Severity>))
		{
			return base.Rule<TParent>(forEachResult, condition, evaluate.DefaultTo(Evaluate.OnDemand), severity.DefaultTo(Severity.Error), new Feature[]
			{
				new RoleFeature(roles.DefaultTo(SetupRole.All)),
				new ModeFeature(modes.DefaultTo(SetupMode.All)),
				new MessageFeature((Result x) => message((RuleResult)x))
			});
		}
	}
}
