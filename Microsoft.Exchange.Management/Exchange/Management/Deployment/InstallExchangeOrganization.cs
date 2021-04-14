using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "ExchangeOrganization", SupportsShouldProcess = true)]
	public sealed class InstallExchangeOrganization : ComponentInfoBasedTask
	{
		public InstallExchangeOrganization()
		{
			base.ImplementsResume = false;
			base.Fields["InstallationMode"] = InstallationModes.Install;
			base.Fields["OrgConfigVersion"] = Organization.OrgConfigurationVersion;
			base.Fields["PrepareSchema"] = false;
			base.Fields["PrepareOrganization"] = false;
			base.Fields["CustomerFeedbackEnabled"] = null;
			base.Fields["Industry"] = IndustryType.NotSpecified;
			base.Fields["PrepareDomain"] = false;
			base.Fields["PrepareSCT"] = false;
			base.Fields["PrepareAllDomains"] = false;
			base.Fields["BinPath"] = ConfigurationContext.Setup.BinPath;
			base.Fields["ActiveDirectorySplitPermissions"] = null;
			ADSession.InitializeForestModeFlagForLocalForest();
		}

		protected override LocalizedString Description
		{
			get
			{
				if (this.PrepareOrganization)
				{
					return Strings.InstallExchangeOrganizationDescription;
				}
				if (this.PrepareDomain || this.PrepareAllDomains)
				{
					return Strings.PrepareDomainDescription;
				}
				return Strings.ConfigureSchema;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareOrganization
		{
			get
			{
				return (bool)base.Fields["PrepareOrganization"];
			}
			set
			{
				base.Fields["PrepareOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)base.Fields["CustomerFeedbackEnabled"];
			}
			set
			{
				base.Fields["CustomerFeedbackEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IndustryType Industry
		{
			get
			{
				return (IndustryType)base.Fields["Industry"];
			}
			set
			{
				base.Fields["Industry"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ActiveDirectorySplitPermissions
		{
			get
			{
				return (bool?)base.Fields["ActiveDirectorySplitPermissions"];
			}
			set
			{
				base.Fields["ActiveDirectorySplitPermissions"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareSchema
		{
			get
			{
				return (bool)base.Fields["PrepareSchema"];
			}
			set
			{
				base.Fields["PrepareSchema"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareDomain
		{
			get
			{
				return (bool)base.Fields["PrepareDomain"];
			}
			set
			{
				base.Fields["PrepareDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareSCT
		{
			get
			{
				return (bool)base.Fields["PrepareSCT"];
			}
			set
			{
				base.Fields["PrepareSCT"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareAllDomains
		{
			get
			{
				return (bool)base.Fields["PrepareAllDomains"];
			}
			set
			{
				base.Fields["PrepareAllDomains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Domain
		{
			get
			{
				return (string)base.Fields["Domain"];
			}
			set
			{
				base.Fields["Domain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OrganizationName
		{
			get
			{
				return (string)base.Fields["OrganizationName"];
			}
			set
			{
				base.Fields["OrganizationName"] = value;
			}
		}

		protected override void CheckInstallationMode()
		{
			bool flag = false;
			try
			{
				IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 272, "CheckInstallationMode", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\InstallExchangeOrganization.cs");
				configurationSession.GetOrgContainer();
				flag = true;
			}
			catch (DataSourceOperationException)
			{
			}
			catch (DataSourceTransientException)
			{
			}
			catch (DataValidationException)
			{
			}
			if (flag)
			{
				base.Fields["InstallationMode"] = InstallationModes.BuildToBuildUpgrade;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.PrepareSCT && base.Fields.Contains("DatacenterFfoEnvironment") && Convert.ToBoolean(base.Fields["DatacenterFfoEnvironment"]))
			{
				base.WriteVerbose(Strings.FFoDisablePrepareSct);
				this.PrepareSCT = false;
			}
			base.ComponentInfoFileNames = new List<string>();
			if (this.PrepareSchema)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\UpdateResourcePropertySchemaComponent.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\ADSchemaComponent.xml");
			}
			if (this.PrepareOrganization)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\CommonGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\TransportGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\BridgeheadGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\ClientAccessGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\MailboxGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\UnifiedMessagingGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\CafeGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\DatacenterGlobalConfig.xml");
			}
			if (this.PrepareDomain || this.PrepareAllDomains)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\DomainGlobalConfig.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\DatacenterDomainGlobalConfig.xml");
			}
			if (this.PrepareOrganization)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\PostPrepForestGlobalConfig.xml");
			}
			if (this.PrepareSCT)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\PrepareSharedConfig.xml");
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 364, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\InstallExchangeOrganization.cs");
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

		protected override void PopulateContextVariables()
		{
			ADDomain addomain = ADForest.GetLocalForest().FindLocalDomain();
			if (addomain == null || addomain.Fqdn == null)
			{
				throw new InvalidFqdnException();
			}
			base.Fields["FullyQualifiedDomainName"] = addomain.Fqdn;
			if (this.PrepareSchema)
			{
				ADSchemaVersion schemaVersion = DirectoryUtilities.GetSchemaVersion(base.DomainController);
				switch (schemaVersion)
				{
				case ADSchemaVersion.Windows:
					base.Fields["SchemaPrefix"] = "PostWindows2003_";
					break;
				case ADSchemaVersion.Exchange2000:
					base.Fields["SchemaPrefix"] = "PostExchange2000_";
					break;
				case ADSchemaVersion.Exchange2003:
				case ADSchemaVersion.Exchange2007Rtm:
					base.Fields["SchemaPrefix"] = "PostExchange2003_";
					break;
				}
				base.Fields["UpdateResourcePropertySchema"] = false;
				base.Fields["ResourcePropertySchemaSaveFile"] = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, "ResourcePropertySchema.xml");
				if (this.ShouldUpdateResourceSchemaAttributeId(schemaVersion))
				{
					base.Fields["UpdateResourcePropertySchema"] = true;
					base.WriteVerbose(Strings.WillSaveResourcePropertySchemaValue((string)base.Fields["ResourcePropertySchemaSaveFile"]));
				}
			}
			base.PopulateContextVariables();
		}

		private bool ShouldUpdateResourceSchemaAttributeId(ADSchemaVersion schemaVersion)
		{
			bool result = false;
			if (schemaVersion == ADSchemaVersion.Exchange2007Rtm)
			{
				ADSchemaAttributeObject[] array = this.configurationSession.Find<ADSchemaAttributeObject>(this.configurationSession.SchemaNamingContext, QueryScope.OneLevel, new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ms-exch-resource-property-schema"),
					new ComparisonFilter(ComparisonOperator.Equal, ADSchemaAttributeSchema.AttributeID, "1.2.840.113556.1.4.7000.102.50329")
				}), null, 1);
				if (array.Length > 0)
				{
					result = true;
				}
			}
			return result;
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is DataSourceOperationException || e is DataSourceTransientException || e is DataValidationException || base.IsKnownException(e);
		}

		private const string OldResourceSchemaAttributeID = "1.2.840.113556.1.4.7000.102.50329";

		private IConfigurationSession configurationSession;
	}
}
