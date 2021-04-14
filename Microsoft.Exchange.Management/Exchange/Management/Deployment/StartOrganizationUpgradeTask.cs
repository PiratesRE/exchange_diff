using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Start", "OrganizationUpgrade", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class StartOrganizationUpgradeTask : StartOrganizationUpgradeBase
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter AuthoritativeOnly
		{
			get
			{
				return this.authoritativeOnly;
			}
			set
			{
				this.authoritativeOnly = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ConfigOnly
		{
			get
			{
				return this.configOnly;
			}
			set
			{
				this.configOnly = value;
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartOrganizationUpgradeDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartOrganizationUpgrade(base.Identity.ToString());
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new StartOrganizationUpgradeTaskModuleFactory();
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			this.monadConnection.RunspaceProxy.SetVariable(StartOrganizationUpgradeTask.ConfigOnlyVarName, this.ConfigOnly);
			if (this.authoritativeOnly || base.IsMSITTenant(base.CurrentOrganizationId))
			{
				this.monadConnection.RunspaceProxy.SetVariable(StartOrganizationUpgradeTask.AuthoritativeOnlyVarName, true);
			}
		}

		internal static readonly string AuthoritativeOnlyVarName = "AuthoritativeOnly";

		internal static readonly string ConfigOnlyVarName = "ConfigOnly";

		private SwitchParameter authoritativeOnly;

		private SwitchParameter configOnly;
	}
}
