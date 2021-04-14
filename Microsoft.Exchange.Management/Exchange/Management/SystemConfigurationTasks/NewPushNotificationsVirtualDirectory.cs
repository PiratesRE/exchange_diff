using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "PushNotificationsVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewPushNotificationsVirtualDirectory : NewExchangeServiceVirtualDirectory<ADPushNotificationsVirtualDirectory>
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
				return "PushNotifications";
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
				return "MSExchangePushNotificationsAppPool";
			}
		}

		protected override Uri DefaultInternalUrl
		{
			get
			{
				return NewPushNotificationsVirtualDirectory.PushNotificationsInternalUri;
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
			this.AppPoolId = "MSExchangePushNotificationsAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			if (this.IsBackEnd)
			{
				this.WebSiteName = "Exchange Back End";
			}
			else
			{
				this.vdirPath = "FrontEnd\\HttpProxy\\PushNotifications";
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
			virtualDirectory.LiveIdBasicAuthentication = new bool?(false);
			virtualDirectory.LiveIdNegotiateAuthentication = new bool?(false);
			virtualDirectory.WSSecurityAuthentication = new bool?(false);
			virtualDirectory.OAuthAuthentication = new bool?(this.OAuthAuthentication);
			ADPushNotificationsVirtualDirectory adpushNotificationsVirtualDirectory = (ADPushNotificationsVirtualDirectory)virtualDirectory;
			adpushNotificationsVirtualDirectory.LiveIdAuthentication = this.LiveIdAuthentication;
			if (this.IsBackEnd)
			{
				virtualDirectory.WindowsAuthentication = new bool?(true);
			}
		}

		protected override void InternalProcessMetabase()
		{
			base.InternalProcessMetabase();
			if (this.IsBackEnd)
			{
				ExchangeServiceVDirHelper.CheckAndUpdateLocalhostNetPipeBindingsIfNecessary(this.DataObject);
				try
				{
					ExchangeServiceVDirHelper.RunAppcmd(string.Format("set app \"{0}/{1}\" /enabledProtocols:http,net.pipe", this.WebSiteName, this.VirtualDirectoryName));
				}
				catch (AppcmdException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.ServerOperation, this.DataObject.Identity);
				}
			}
		}

		protected override void InternalProcessComplete()
		{
			base.InternalProcessComplete();
			if (this.IsBackEnd)
			{
				ExchangeServiceVDirHelper.ForceAnonymous(this.DataObject.MetabasePath);
			}
		}

		private const string PushNotificationsBackendVDirPath = "ClientAccess\\PushNotifications";

		private const string PushNotificationsCafeVDirPath = "FrontEnd\\HttpProxy\\PushNotifications";

		private const string LiveIdAuthenticationFieldName = "LiveIdFbaAuthentication";

		private const string OAuthAuthenticationFieldName = "OAuthAuthentication";

		private static readonly Uri PushNotificationsInternalUri = new Uri(string.Format("https://{0}/{1}/{2}", ComputerInformation.DnsFullyQualifiedDomainName, "PushNotifications", ""));

		private string vdirPath = "ClientAccess\\PushNotifications";
	}
}
