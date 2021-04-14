using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Enable", "OabWebDistribution", SupportsShouldProcess = false, DefaultParameterSetName = "Identity")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class EnableOabWebDistribution : SetOfflineAddressBookInternal
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return LocalizedString.Empty;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.skipExecution = false;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.skipExecution)
			{
				return;
			}
			this.DataObject = (OfflineAddressBook)this.PrepareDataObject();
			if (this.DataObject.WebDistributionEnabled)
			{
				base.WriteVerbose(Strings.VerboseDefaultOABWebDistributionEnabled(this.DataObject.DistinguishedName));
				this.skipExecution = true;
				return;
			}
			ITopologyConfigurationSession topologyConfigurationSession = base.DataSession as ITopologyConfigurationSession;
			this.localOABVDirs = topologyConfigurationSession.FindOABVirtualDirectoriesForLocalServer();
			if (this.localOABVDirs == null || this.localOABVDirs.Length == 0)
			{
				base.WriteVerbose(Strings.VerboseNoOabVDirOnLocalServer);
				this.skipExecution = true;
				return;
			}
			if (this.DataObject.IsReadOnly)
			{
				base.WriteVerbose(Strings.VerboseDefaultOABIsNewerThanVersionE12(this.DataObject.DistinguishedName, this.DataObject.ExchangeVersion.ExchangeBuild.ToString()));
				this.skipExecution = true;
				return;
			}
			if (this.DataObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				base.WriteVerbose(Strings.VerboseDefaultOABIsOlderThanVersionE12(this.DataObject.DistinguishedName));
				this.skipExecution = true;
				return;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.skipExecution)
			{
				return;
			}
			MultiValuedProperty<OfflineAddressBookVersion> versions = this.DataObject.Versions;
			if (!versions.Contains(OfflineAddressBookVersion.Version4))
			{
				versions.Add(OfflineAddressBookVersion.Version4);
				this.DataObject.Versions = versions;
			}
			foreach (ADOabVirtualDirectory adoabVirtualDirectory in this.localOABVDirs)
			{
				this.DataObject.VirtualDirectories.Add(adoabVirtualDirectory.Id);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private bool skipExecution;

		private ADOabVirtualDirectory[] localOABVDirs;
	}
}
