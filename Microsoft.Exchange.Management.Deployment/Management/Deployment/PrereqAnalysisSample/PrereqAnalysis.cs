using System;
using Microsoft.Exchange.Management.Analysis;
using Microsoft.Exchange.Management.Deployment.Analysis;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal class PrereqAnalysis : AnalysisImplementation<IDataProviderFactory, PrereqAnalysisMemberBuilder, PrereqConclusionSetBuilder, PrereqConclusionSet, PrereqConclusion, PrereqSettingConclusion, PrereqRuleConclusion>
	{
		public PrereqAnalysis(IDataProviderFactory dataSourceProvider, PrereqAnalysisMemberBuilder memberBuilder, PrereqConclusionSetBuilder conclusionSetBuilder, SetupMode setupModes, SetupRole setupRoles, GlobalParameters prereqAnalysisParameters, AnalysisThreading threadMode) : base(dataSourceProvider, memberBuilder, conclusionSetBuilder, (Microsoft.Exchange.Management.Deployment.Analysis.AnalysisMember x) => (!x.Features.HasFeature<ModeFeature>() || x.Features.GetFeature<ModeFeature>().Contains(setupModes)) && (!x.Features.HasFeature<RoleFeature>() || x.Features.GetFeature<RoleFeature>().Contains(setupRoles)), (Microsoft.Exchange.Management.Deployment.Analysis.AnalysisMember x) => (!x.Features.HasFeature<ModeFeature>() || x.Features.GetFeature<ModeFeature>().Contains(setupModes)) && (!x.Features.HasFeature<RoleFeature>() || x.Features.GetFeature<RoleFeature>().Contains(setupRoles)), threadMode)
		{
			this.setupModes = setupModes;
			this.setupRoles = setupRoles;
			this.prereqAnalysisParameters = prereqAnalysisParameters;
			this.BuildPrereqSettings();
			this.BuildPrereqRules();
		}

		public SetupMode SetupModes
		{
			get
			{
				return this.setupModes;
			}
		}

		public SetupRole SetupRoles
		{
			get
			{
				return this.setupRoles;
			}
		}

		public GlobalParameters PrereqAnalysisParameters
		{
			get
			{
				return this.prereqAnalysisParameters;
			}
		}

		protected override void OnAnalysisStart()
		{
		}

		protected override void OnAnalysisStop()
		{
		}

		protected override void OnAnalysisMemberStart(Microsoft.Exchange.Management.Deployment.Analysis.AnalysisMember member)
		{
		}

		protected override void OnAnalysisMemberStop(Microsoft.Exchange.Management.Deployment.Analysis.AnalysisMember member)
		{
		}

		protected override void OnAnalysisMemberEvaluate(Microsoft.Exchange.Management.Deployment.Analysis.AnalysisMember member, Microsoft.Exchange.Management.Deployment.Analysis.Result result)
		{
		}

		public Microsoft.Exchange.Management.Deployment.Analysis.Rule InstallWatermark { get; private set; }

		public Microsoft.Exchange.Management.Deployment.Analysis.Rule LonghornIIS6MgmtConsoleNotInstalledWarning { get; private set; }

		private void BuildPrereqRules()
		{
			PrereqAnalysisMemberBuilder build = base.Build;
			Func<Microsoft.Exchange.Management.Deployment.Analysis.AnalysisMember<string>> forEachResult = () => this.ServerRoleUnpacked;
			Optional<SetupRole> roles = SetupRole.Mailbox | SetupRole.Bridgehead | SetupRole.ClientAccess | SetupRole.UnifiedMessaging | SetupRole.Gateway | SetupRole.Cafe | SetupRole.Global | SetupRole.FrontendTransport;
			Optional<SetupMode> modes = SetupMode.Install | SetupMode.DisasterRecovery;
			this.InstallWatermark = build.Rule<string>(forEachResult, (Microsoft.Exchange.Management.Deployment.Analysis.RuleResult x) => Strings.WatermarkPresent(x.AncestorOfType<string>(this.ServerRoleUnpacked).Value), (Microsoft.Exchange.Management.Deployment.Analysis.Result<string> x) => x.ValueOrDefault != null, default(Optional<Evaluate>), roles, modes, default(Optional<Severity>));
			PrereqAnalysisMemberBuilder build2 = base.Build;
			Optional<SetupRole> roles2 = SetupRole.Bridgehead | SetupRole.UnifiedMessaging;
			Optional<SetupMode> modes2 = SetupMode.Install | SetupMode.Upgrade | SetupMode.DisasterRecovery;
			Optional<Severity> severity = Severity.Warning;
			this.LonghornIIS6MgmtConsoleNotInstalledWarning = build2.Rule((Microsoft.Exchange.Management.Deployment.Analysis.RuleResult x) => Strings.ComponentIsRecommended("IIS 6 Management Console"), () => (this.WindowsVersion.Value == "6.0" || this.WindowsVersion.Value == "6.1") && (this.IIS6ManagementConsoleStatus.ValueOrDefault == null || this.IIS6ManagementConsoleStatus.Value == 0), default(Optional<Evaluate>), roles2, modes2, severity);
		}

		public Microsoft.Exchange.Management.Deployment.Analysis.Setting<string> Roles { get; private set; }

		public Microsoft.Exchange.Management.Deployment.Analysis.Setting<string> ServerRoleUnpacked { get; private set; }

		public Microsoft.Exchange.Management.Deployment.Analysis.Setting<string> Watermarks { get; private set; }

		public Microsoft.Exchange.Management.Deployment.Analysis.Setting<string> WindowsVersion { get; private set; }

		public Microsoft.Exchange.Management.Deployment.Analysis.Setting<int?> IIS6ManagementConsoleStatus { get; private set; }

		private void BuildPrereqSettings()
		{
			PrereqAnalysisMemberBuilder build = base.Build;
			Optional<SetupRole> roles = SetupRole.All;
			Optional<SetupMode> modes = SetupMode.All;
			this.Roles = build.Setting<string>(() => (string[])base.DataSources.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15", null), default(Optional<Evaluate>), roles, modes);
			this.ServerRoleUnpacked = base.Build.Setting<string, string>(() => this.Roles, (Microsoft.Exchange.Management.Deployment.Analysis.Result<string> x) => AnalysisHelpers.Replace(x.Value, "(.*Role)", "$1"), default(Optional<Evaluate>), default(Optional<SetupRole>), default(Optional<SetupMode>));
			this.Watermarks = base.Build.Setting<string, string>(() => this.ServerRoleUnpacked, (Microsoft.Exchange.Management.Deployment.Analysis.Result<string> x) => (string)base.DataSources.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + x.Value, "Watermark"), default(Optional<Evaluate>), default(Optional<SetupRole>), default(Optional<SetupMode>));
			this.WindowsVersion = base.Build.Setting<string>(() => (string)base.DataSources.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion", "CurrentVersion"), default(Optional<Evaluate>), default(Optional<SetupRole>), default(Optional<SetupMode>));
			this.IIS6ManagementConsoleStatus = base.Build.Setting<int?>(() => (int?)base.DataSources.RegistryDataProvider.GetRegistryKeyValue(Registry.LocalMachine, "Software\\Microsoft\\InetStp\\Components", "LegacySnapin"), default(Optional<Evaluate>), default(Optional<SetupRole>), default(Optional<SetupMode>));
		}

		private readonly SetupMode setupModes;

		private readonly SetupRole setupRoles;

		private readonly GlobalParameters prereqAnalysisParameters;
	}
}
