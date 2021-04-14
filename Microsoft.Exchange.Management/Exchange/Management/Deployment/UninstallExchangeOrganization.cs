using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Uninstall", "ExchangeOrganization", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class UninstallExchangeOrganization : ComponentInfoBasedTask
	{
		public UninstallExchangeOrganization()
		{
			base.Fields["InstallationMode"] = InstallationModes.Uninstall;
			base.Fields["RemoveOrganization"] = false;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.UninstallExchangeOrganizationDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemoveOrganization
		{
			get
			{
				return (bool)base.Fields["RemoveOrganization"];
			}
			set
			{
				base.Fields["RemoveOrganization"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.ComponentInfoFileNames = new List<string>();
			if (this.RemoveOrganization)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\UpdateResourcePropertySchemaComponent.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\ADSchemaComponent.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\CommonGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\TransportGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\BridgeheadGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\ClientAccessGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\MailboxGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\UnifiedMessagingGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\PostPrepForestGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("Setup\\data\\DomainGlobalConfig.xml");
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.ComponentInfoFileNames.Count == 0)
			{
				base.WriteProgress(new LocalizedString(this.Description), Strings.ProgressStatusCompleted, 100);
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}
	}
}
