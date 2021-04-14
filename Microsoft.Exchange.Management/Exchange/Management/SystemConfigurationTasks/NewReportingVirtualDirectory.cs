using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ReportingVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewReportingVirtualDirectory : ManageReportingVirtualDirectory
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewReportingVirtualDirectory;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = "Reporting";
			createVirtualDirectory.Parent = "IIS://localhost/W3SVC/1/Root";
			createVirtualDirectory.LocalPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\Reporting");
			createVirtualDirectory.CustomizedVDirProperties = new List<MetabaseProperty>
			{
				new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Anonymous),
				new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSLNegotiateCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128),
				new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script)
			};
			createVirtualDirectory.ApplicationPool = "MSExchangeReportingAppPool";
			createVirtualDirectory.AppPoolIdentityType = MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			createVirtualDirectory.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
			TaskLogger.LogExit();
		}
	}
}
