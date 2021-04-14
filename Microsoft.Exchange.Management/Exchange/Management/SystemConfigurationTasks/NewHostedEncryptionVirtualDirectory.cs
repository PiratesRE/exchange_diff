using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "HostedEncryptionVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewHostedEncryptionVirtualDirectory : NewWebAppVirtualDirectory<ADE4eVirtualDirectory>
	{
		public NewHostedEncryptionVirtualDirectory()
		{
			this.Name = "Encryption";
			base.AppPoolId = "MSExchangeEncryptionAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			private set
			{
				base.Name = value;
			}
		}

		public new string ApplicationRoot
		{
			get
			{
				return base.ApplicationRoot;
			}
			private set
			{
				base.ApplicationRoot = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return base.ExternalAuthenticationMethods;
			}
			private set
			{
				base.ExternalAuthenticationMethods = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewHostedEncryptionVirtualDirectory(base.WebSiteName, base.Server.ToString());
			}
		}

		protected override ArrayList CustomizedVDirProperties
		{
			get
			{
				return new ArrayList
				{
					new MetabaseProperty("DefaultDoc", "default.aspx"),
					new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script),
					new MetabaseProperty("CacheControlCustom", "public"),
					new MetabaseProperty("HttpExpires", "D, 0x278d00"),
					new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Basic),
					new MetabaseProperty("AppFriendlyName", this.Name),
					new MetabaseProperty("AppRoot", base.AppRootValue),
					new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled),
					new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128)
				};
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			if (!base.Fields.IsModified("Path"))
			{
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, (base.Role == VirtualDirectoryRole.ClientAccess) ? "FrontEnd\\HttpProxy\\e4e" : "ClientAccess\\e4e");
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!new VirtualDirectoryPathExistsCondition(base.OwningServer.Fqdn, base.Path).Verify())
			{
				base.WriteError(new ArgumentException(Strings.ErrorPathNotExistsOnServer(base.Path, base.OwningServer.Name), "Path"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override bool InternalProcessStartWork()
		{
			this.SetDefaults();
			return true;
		}

		protected override void InternalProcessMetabase()
		{
			ADOwaVirtualDirectory adowaVirtualDirectory = WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADOwaVirtualDirectory>(this.DataObject, base.DataSession);
			if (adowaVirtualDirectory != null && !string.IsNullOrEmpty(adowaVirtualDirectory.DefaultDomain))
			{
				this.DataObject.DefaultDomain = adowaVirtualDirectory.DefaultDomain;
			}
			WebAppVirtualDirectoryHelper.UpdateMetabase(this.DataObject, this.DataObject.MetabasePath, true);
		}

		private void SetDefaults()
		{
			this.DataObject.GzipLevel = GzipLevel.High;
			this.DataObject.FormsAuthentication = (base.Role == VirtualDirectoryRole.ClientAccess);
			this.DataObject.BasicAuthentication = (base.Role == VirtualDirectoryRole.ClientAccess);
			this.DataObject.WindowsAuthentication = (base.Role == VirtualDirectoryRole.Mailbox);
			this.DataObject.DigestAuthentication = false;
			this.DataObject.LiveIdAuthentication = false;
		}

		private const string LocalPath = "ClientAccess\\e4e";

		private const string CafePath = "FrontEnd\\HttpProxy\\e4e";

		private const string DefaultApplicationPool = "MSExchangeEncryptionAppPool";
	}
}
