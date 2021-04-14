using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "O365SuiteServiceVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewO365SuiteServiceVirtualDirectory : NewExchangeServiceVirtualDirectory<ADO365SuiteServiceVirtualDirectory>
	{
		[Parameter(Mandatory = false)]
		public bool LiveIdAuthentication
		{
			get
			{
				return base.Fields["LiveIdFbaAuthentication"] != null && (bool)base.Fields["LiveIdFbaAuthentication"];
			}
			set
			{
				base.Fields["LiveIdFbaAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OAuthAuthentication
		{
			get
			{
				return base.Fields["OAuthAuthentication"] != null && (bool)base.Fields["OAuthAuthentication"];
			}
			set
			{
				base.Fields["OAuthAuthentication"] = value;
			}
		}

		protected override string VirtualDirectoryName
		{
			get
			{
				return "O365SuiteService";
			}
		}

		protected override string VirtualDirectoryPath
		{
			get
			{
				return this.vdirPath;
			}
		}

		protected override string DefaultApplicationPoolId
		{
			get
			{
				return "MSExchangeO365SuiteServiceAppPool";
			}
		}

		protected override Uri DefaultInternalUrl
		{
			get
			{
				return NewO365SuiteServiceVirtualDirectory.O365SuiteServiceInternalUri;
			}
		}

		internal new string WebSiteName
		{
			get
			{
				return base.WebSiteName;
			}
			private set
			{
				base.WebSiteName = value;
			}
		}

		internal new string AppPoolId
		{
			get
			{
				return base.AppPoolId;
			}
			private set
			{
				base.AppPoolId = value;
			}
		}

		internal new string ApplicationRoot
		{
			get
			{
				return base.ApplicationRoot;
			}
			set
			{
				base.ApplicationRoot = value;
			}
		}

		internal new string Path
		{
			get
			{
				return base.Path;
			}
			set
			{
				base.Path = value;
			}
		}

		internal new ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
		{
			get
			{
				return base.ExtendedProtectionTokenChecking;
			}
			set
			{
				base.ExtendedProtectionTokenChecking = value;
			}
		}

		internal new MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
		{
			get
			{
				return base.ExtendedProtectionFlags;
			}
			set
			{
				base.ExtendedProtectionFlags = value;
			}
		}

		internal new MultiValuedProperty<string> ExtendedProtectionSPNList
		{
			get
			{
				return base.ExtendedProtectionSPNList;
			}
			set
			{
				base.ExtendedProtectionSPNList = value;
			}
		}

		internal new bool BasicAuthentication
		{
			get
			{
				return base.BasicAuthentication;
			}
			set
			{
				base.BasicAuthentication = value;
			}
		}

		internal new bool DigestAuthentication
		{
			get
			{
				return base.DigestAuthentication;
			}
			set
			{
				base.DigestAuthentication = value;
			}
		}

		internal new bool WindowsAuthentication
		{
			get
			{
				return base.WindowsAuthentication;
			}
			set
			{
				base.WindowsAuthentication = value;
			}
		}

		private bool IsBackEnd
		{
			get
			{
				return base.Role == VirtualDirectoryRole.Mailbox;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.Name = "O365SuiteService";
			this.AppPoolId = "MSExchangeO365SuiteServiceAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			if (this.IsBackEnd)
			{
				this.WebSiteName = "Exchange Back End";
			}
			else
			{
				this.vdirPath = "FrontEnd\\HttpProxy\\O365SuiteService";
			}
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void AddCustomVDirProperties(ArrayList customProperties)
		{
			base.AddCustomVDirProperties(customProperties);
			customProperties.Add(new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128));
		}

		protected override void SetDefaultAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			virtualDirectory.WindowsAuthentication = new bool?(false);
			virtualDirectory.BasicAuthentication = new bool?(false);
			virtualDirectory.DigestAuthentication = new bool?(false);
			virtualDirectory.LiveIdNegotiateAuthentication = new bool?(false);
			virtualDirectory.WSSecurityAuthentication = new bool?(false);
			virtualDirectory.OAuthAuthentication = new bool?(this.OAuthAuthentication);
			ADO365SuiteServiceVirtualDirectory ado365SuiteServiceVirtualDirectory = (ADO365SuiteServiceVirtualDirectory)virtualDirectory;
			ado365SuiteServiceVirtualDirectory.LiveIdAuthentication = this.LiveIdAuthentication;
			if (this.IsBackEnd)
			{
				virtualDirectory.WindowsAuthentication = new bool?(true);
			}
		}

		private const string BackendVDirPath = "ClientAccess\\O365SuiteService";

		private const string CafeVDirPath = "FrontEnd\\HttpProxy\\O365SuiteService";

		private const string ApplicationPoolId = "MSExchangeO365SuiteServiceAppPool";

		private const string LiveIdAuthenticationFieldName = "LiveIdFbaAuthentication";

		private const string OAuthAuthenticationFieldName = "OAuthAuthentication";

		private static readonly Uri O365SuiteServiceInternalUri = new Uri(string.Format("https://{0}/{1}/{2}", ComputerInformation.DnsFullyQualifiedDomainName, "O365SuiteService", ""));

		private string vdirPath = "ClientAccess\\O365SuiteService";
	}
}
