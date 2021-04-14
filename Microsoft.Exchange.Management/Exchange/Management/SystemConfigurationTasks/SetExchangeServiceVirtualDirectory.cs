using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.RedirectionModule;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetExchangeServiceVirtualDirectory<T> : SetExchangeVirtualDirectory<T> where T : ADExchangeServiceVirtualDirectory, new()
	{
		internal static bool CheckAuthModule(ADExchangeServiceVirtualDirectory advdir, bool isChildVDirApplication, string moduleName)
		{
			DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(advdir.MetabasePath);
			bool result;
			using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(advdir.MetabasePath)))
			{
				Configuration webConfiguration;
				if (isChildVDirApplication)
				{
					webConfiguration = serverManager.Sites[IisUtility.GetWebSiteName(directoryEntry.Parent.Parent.Path)].Applications[string.Format("/{0}/{1}", directoryEntry.Parent.Name, directoryEntry.Name)].GetWebConfiguration();
				}
				else
				{
					webConfiguration = serverManager.Sites[IisUtility.GetWebSiteName(directoryEntry.Parent.Path)].Applications["/" + directoryEntry.Name].GetWebConfiguration();
				}
				ConfigurationElementCollection collection = webConfiguration.GetSection("system.webServer/modules").GetCollection();
				foreach (ConfigurationElement configurationElement in collection)
				{
					if (string.Equals(configurationElement.Attributes["name"].Value.ToString(), moduleName, StringComparison.Ordinal))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		protected bool CheckLiveIdBasicAuthModule(bool isChildVDirApplication)
		{
			return SetExchangeServiceVirtualDirectory<T>.CheckAuthModule(this.DataObject, isChildVDirApplication, "LiveIdBasicAuthenticationModule");
		}

		protected void SetLiveIdNegotiateAuxiliaryModule(bool EnableModule, bool isChildVDirApplication)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "LiveIdNegotiateAuxiliaryModule", typeof(LiveIdNegotiateAuxiliaryModule).FullName, this.DataObject);
		}

		protected void SetDelegatedAuthenticationModule(bool EnableModule, bool isChildVDirApplication)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "DelegatedAuthModule", "Microsoft.Exchange.Configuration.DelegatedAuthentication.DelegatedAuthenticationModule", this.DataObject);
		}

		protected void SetPowerShellRequestFilterModule(bool EnableModule, bool isChildVDirApplication)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "PowerShellRequestFilterModule", "Microsoft.Exchange.Configuration.DelegatedAuthentication.PowerShellRequestFilterModule", this.DataObject);
		}

		protected void SetCertificateHeaderAuthenticationModule(bool EnableModule, bool isChildVDirApplication)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "CertificateHeaderAuthModule", "Microsoft.Exchange.Configuration.CertificateAuthentication.CertificateHeaderAuthModule", this.DataObject);
		}

		protected void SetSessionKeyRedirectionModule(bool EnableModule, bool isChildVDirApplication)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "SessionKeyRedirectionModule", typeof(SessionKeyRedirectionModule).FullName, this.DataObject);
		}

		protected virtual LocalizedString MetabaseSetPropertiesFailureMessage
		{
			get
			{
				return Strings.MetabaseSetPropertiesFailure;
			}
		}

		protected void InternalValidateBasicLiveIdBasic()
		{
			T dataObject = this.DataObject;
			string metabasePath = dataObject.MetabasePath;
			Task.TaskErrorLoggingReThrowDelegate writeError = new Task.TaskErrorLoggingReThrowDelegate(this.WriteError);
			T dataObject2 = this.DataObject;
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath, writeError, dataObject2.Identity))
			{
				T dataObject3 = this.DataObject;
				bool? basicAuthentication = dataObject3.BasicAuthentication;
				T dataObject4 = this.DataObject;
				bool? liveIdBasicAuthentication = dataObject4.LiveIdBasicAuthentication;
				bool flag = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic);
				bool flag2 = this.CheckLiveIdBasicAuthModule(false);
				bool flag3 = basicAuthentication ?? flag;
				bool flag4 = liveIdBasicAuthentication ?? flag2;
				if (flag3 && flag4)
				{
					string format = "Enabling both Basic and LiveIdBasic Authentication is not allowed. Virtual directory '{0}' has Basic={1}, LiveIdBasic={2}";
					object[] array = new object[3];
					object[] array2 = array;
					int num = 0;
					T dataObject5 = this.DataObject;
					array2[num] = dataObject5.MetabasePath;
					array[1] = flag.ToString();
					array[2] = flag2.ToString();
					TaskLogger.Trace(format, array);
					T dataObject6 = this.DataObject;
					Exception exception = new LocalizedException(Strings.ErrorBasicAndLiveIdBasicNotAllowedVDir(dataObject6.MetabasePath, flag.ToString(), flag2.ToString()));
					ErrorCategory category = ErrorCategory.InvalidOperation;
					T dataObject7 = this.DataObject;
					base.WriteError(exception, category, dataObject7.Identity);
				}
			}
		}

		protected void InternalEnableLiveIdNegotiateAuxiliaryModule()
		{
			T dataObject = this.DataObject;
			if (dataObject.LiveIdNegotiateAuthentication != null)
			{
				bool liveIdNegotiateAuthentication = this.LiveIdNegotiateAuthentication;
				this.SetLiveIdNegotiateAuxiliaryModule(liveIdNegotiateAuthentication, false);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADExchangeServiceVirtualDirectory adexchangeServiceVirtualDirectory = (ADExchangeServiceVirtualDirectory)base.PrepareDataObject();
			adexchangeServiceVirtualDirectory.BasicAuthentication = (bool?)base.Fields["BasicAuthentication"];
			adexchangeServiceVirtualDirectory.DigestAuthentication = (bool?)base.Fields["DigestAuthentication"];
			adexchangeServiceVirtualDirectory.WindowsAuthentication = (bool?)base.Fields["WindowsAuthentication"];
			adexchangeServiceVirtualDirectory.LiveIdBasicAuthentication = (bool?)base.Fields["LiveIdBasicAuthentication"];
			adexchangeServiceVirtualDirectory.LiveIdNegotiateAuthentication = (bool?)base.Fields["LiveIdNegotiateAuthentication"];
			adexchangeServiceVirtualDirectory.OAuthAuthentication = (bool?)base.Fields["OAuthAuthentication"];
			return adexchangeServiceVirtualDirectory;
		}

		protected override void InternalProcessRecord()
		{
			T dataObject = this.DataObject;
			byte major = dataObject.ExchangeVersion.ExchangeBuild.Major;
			T dataObject2 = this.DataObject;
			if (major != dataObject2.MaximumSupportedExchangeObjectVersion.ExchangeBuild.Major)
			{
				T dataObject3 = this.DataObject;
				base.WriteError(new CannotModifyCrossVersionObjectException(dataObject3.Id.DistinguishedName), ErrorCategory.InvalidOperation, null);
				return;
			}
			base.InternalProcessRecord();
			T dataObject4 = this.DataObject;
			base.WriteVerbose(Strings.VerboseApplyingAuthenticationSettingForVDir(dataObject4.MetabasePath));
			ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), this.MetabaseSetPropertiesFailureMessage);
			T dataObject5 = this.DataObject;
			if (dataObject5.LiveIdBasicAuthentication != null)
			{
				ExchangeServiceVDirHelper.SetLiveIdBasicAuthModule(this.LiveIdBasicAuthentication, false, this.DataObject);
			}
			T dataObject6 = this.DataObject;
			if (dataObject6.OAuthAuthentication != null)
			{
				T dataObject7 = this.DataObject;
				ExchangeServiceVDirHelper.SetOAuthAuthenticationModule(dataObject7.OAuthAuthentication.Value, false, this.DataObject);
			}
			ExchangeServiceVDirHelper.CheckAndUpdateWindowsAuthProvidersIfNecessary(this.DataObject, (bool?)base.Fields["WindowsAuthentication"]);
		}

		[Parameter(Mandatory = false)]
		public bool LiveIdBasicAuthentication
		{
			get
			{
				return base.Fields["LiveIdBasicAuthentication"] != null && (bool)base.Fields["LiveIdBasicAuthentication"];
			}
			set
			{
				base.Fields["LiveIdBasicAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LiveIdNegotiateAuthentication
		{
			get
			{
				return base.Fields["LiveIdNegotiateAuthentication"] != null && (bool)base.Fields["LiveIdNegotiateAuthentication"];
			}
			set
			{
				base.Fields["LiveIdNegotiateAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BasicAuthentication
		{
			get
			{
				return base.Fields["BasicAuthentication"] != null && (bool)base.Fields["BasicAuthentication"];
			}
			set
			{
				base.Fields["BasicAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DigestAuthentication
		{
			get
			{
				return base.Fields["DigestAuthentication"] != null && (bool)base.Fields["DigestAuthentication"];
			}
			set
			{
				base.Fields["DigestAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WindowsAuthentication
		{
			get
			{
				return base.Fields["WindowsAuthentication"] != null && (bool)base.Fields["WindowsAuthentication"];
			}
			set
			{
				base.Fields["WindowsAuthentication"] = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
			set
			{
				base.InternalAuthenticationMethods = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return base.ExternalAuthenticationMethods;
			}
			set
			{
				base.ExternalAuthenticationMethods = value;
			}
		}
	}
}
