using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal class PrereqConclusionSet : ConclusionSetImplementation<PrereqConclusion, PrereqSettingConclusion, PrereqRuleConclusion>
	{
		public PrereqConclusionSet()
		{
		}

		public PrereqConclusionSet(PrereqAnalysis analysis, PrereqConclusion root) : base(root)
		{
			this.setupModes = analysis.SetupModes;
			this.setupRoles = analysis.SetupRoles;
		}

		public SetupMode SetupModes
		{
			get
			{
				return this.setupModes;
			}
			set
			{
				base.ThrowIfReadOnly();
				this.setupModes = value;
			}
		}

		public SetupRole SetupRoles
		{
			get
			{
				return this.setupRoles;
			}
			set
			{
				base.ThrowIfReadOnly();
				this.setupRoles = value;
			}
		}

		private SetupMode setupModes;

		private SetupRole setupRoles;
	}
}
