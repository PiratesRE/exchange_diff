using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Tools
{
	[Cmdlet("Get", "ToolInformation")]
	public sealed class GetToolInformation : Task
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public ToolId Identity
		{
			get
			{
				return (ToolId)base.Fields["ToolId"];
			}
			set
			{
				base.Fields["ToolId"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public Version Version
		{
			get
			{
				return (Version)base.Fields["Version"];
			}
			set
			{
				base.Fields["Version"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.LoadSupportedToolsData();
			this.tenantVersionRequired = this.toolsData.RequiresTenantVersion();
			if (this.tenantVersionRequired)
			{
				this.rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				base.CurrentOrganizationId = OrganizationTaskHelper.ResolveOrganization(this, this.Organization, this.rootOrgContainerId, Strings.ErrorOrganizationParameterRequired);
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			if (this.tenantVersionRequired)
			{
				this.session = GetToolInformation.CreateSession();
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			Version tenantVersion = null;
			if (this.tenantVersionRequired)
			{
				ExchangeConfigurationUnit dataObject = this.session.Read<ExchangeConfigurationUnit>(base.CurrentOrganizationId.ConfigurationUnit);
				Version installedVersion = ConfigurationContext.Setup.InstalledVersion;
				TenantOrganizationPresentationObject tenantOrganizationPresentationObject = new TenantOrganizationPresentationObject(dataObject);
				tenantVersion = new Version(installedVersion.Major, installedVersion.Minor, tenantOrganizationPresentationObject.BuildMajor, tenantOrganizationPresentationObject.BuildMinor);
			}
			SupportedVersion supportedVersion = this.toolsData.GetSupportedVersion(this.Identity, tenantVersion);
			base.WriteObject(this.ConstructOutputObject(supportedVersion));
		}

		private void LoadSupportedToolsData()
		{
			try
			{
				this.toolsData = SupportedToolsData.GetSupportedToolData(GetToolInformation.GetFilePath("SupportedTools.xml"), GetToolInformation.GetFilePath("SupportedTools.xsd"));
			}
			catch (Exception e)
			{
				this.HandleInvalidSupportedToolsData(e);
			}
		}

		private static string GetFilePath(string fileName)
		{
			return Path.Combine(ConfigurationContext.Setup.BinPath, fileName);
		}

		private void HandleInvalidSupportedToolsData(Exception e)
		{
			if (e is FileNotFoundException)
			{
				FileNotFoundException ex = e as FileNotFoundException;
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_SupportedToolsInformationFileMissing, new string[]
				{
					ex.FileName
				});
			}
			else if (e is InvalidDataException)
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_SupportedToolsInformationDataFileInconsistent, new string[]
				{
					GetToolInformation.GetFilePath("SupportedTools.xml"),
					e.Message
				});
			}
			else
			{
				Exception ex2 = e.InnerException ?? e;
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_SupportedToolsInformationDataFileCorupted, new string[]
				{
					GetToolInformation.GetFilePath("SupportedTools.xml"),
					ex2.Message
				});
			}
			base.WriteError(new SupportedToolsDataException(Strings.SupportedToolsUnableToGetToolData), ErrorCategory.InvalidData, null);
		}

		private ToolInformation ConstructOutputObject(SupportedVersion supportedVersion)
		{
			ToolInformation result;
			try
			{
				Version version = SupportedToolsData.GetVersion(supportedVersion.minSupportedVersion, SupportedToolsData.MinimumVersion);
				Version version2 = SupportedToolsData.GetVersion(supportedVersion.latestVersion, SupportedToolsData.MaximumVersion);
				ToolVersionStatus versionStatus = GetToolInformation.GetVersionStatus(version, version2, this.Version);
				Uri updateInformationUrl = (versionStatus != ToolVersionStatus.LatestVersion) ? new Uri(supportedVersion.updateUrl) : null;
				result = new ToolInformation(this.Identity, versionStatus, version, version2, updateInformationUrl);
			}
			catch (Exception e)
			{
				this.HandleInvalidSupportedToolsData(e);
				result = null;
			}
			return result;
		}

		private static ToolVersionStatus GetVersionStatus(Version minSupportedVersion, Version latestVersion, Version toolVersion)
		{
			if (toolVersion < minSupportedVersion)
			{
				return ToolVersionStatus.VersionNoLongerSupported;
			}
			if (toolVersion < latestVersion)
			{
				return ToolVersionStatus.NewerVersionAvailable;
			}
			return ToolVersionStatus.LatestVersion;
		}

		private static IConfigurationSession CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 261, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\tools\\GetToolInformation.cs");
		}

		private const string ToolInfoIdParameterName = "ToolId";

		private const string VersionParameterName = "Version";

		private const string OrganizationParameterName = "Organization";

		private const string SupportedToolsDataFileName = "SupportedTools.xml";

		private const string SupportedToolsSchemaFileName = "SupportedTools.xsd";

		private ADObjectId rootOrgContainerId;

		private IConfigurationSession session;

		private SupportedToolsData toolsData;

		private bool tenantVersionRequired;
	}
}
