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
	public abstract class NewExchangeServiceVirtualDirectory<T> : NewExchangeVirtualDirectory<T> where T : ADExchangeServiceVirtualDirectory, new()
	{
		private string GetAppRootValue()
		{
			return base.RetrieveVDirAppRootValue(base.GetAbsolutePath(IisUtility.AbsolutePathType.WebSiteRoot), this.Name);
		}

		protected abstract string VirtualDirectoryName { get; }

		protected abstract string VirtualDirectoryPath { get; }

		protected abstract string DefaultApplicationPoolId { get; }

		protected virtual Uri DefaultInternalUrl
		{
			get
			{
				return null;
			}
		}

		protected virtual LocalizedString MetabaseSetPropertiesFailureMessage
		{
			get
			{
				return Strings.MetabaseSetPropertiesFailure;
			}
		}

		protected virtual void SetDefaultAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			virtualDirectory.WindowsAuthentication = new bool?(true);
		}

		protected virtual void AddCustomVDirProperties(ArrayList customProperties)
		{
			customProperties.Add(new MetabaseProperty("AppFriendlyName", this.Name));
			customProperties.Add(new MetabaseProperty("AppRoot", this.GetAppRootValue()));
			customProperties.Add(new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled));
			customProperties.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Ntlm));
			customProperties.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script));
		}

		public NewExchangeServiceVirtualDirectory()
		{
		}

		internal override MetabasePropertyTypes.AppPoolIdentityType AppPoolIdentityType
		{
			get
			{
				return MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			}
		}

		protected override ArrayList CustomizedVDirProperties
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				this.AddCustomVDirProperties(arrayList);
				return arrayList;
			}
		}

		protected void InternalValidateBasicLiveIdBasic()
		{
			T dataObject = this.DataObject;
			string metabasePath = dataObject.MetabasePath;
			Task.TaskErrorLoggingReThrowDelegate writeError = new Task.TaskErrorLoggingReThrowDelegate(this.WriteError);
			T dataObject2 = this.DataObject;
			using (IisUtility.CreateIISDirectoryEntry(metabasePath, writeError, dataObject2.Identity))
			{
				T dataObject3 = this.DataObject;
				bool flag = dataObject3.BasicAuthentication ?? false;
				T dataObject4 = this.DataObject;
				bool flag2 = dataObject4.LiveIdBasicAuthentication ?? false;
				if (flag && flag2)
				{
					TaskLogger.Trace("Enabling both Basic and LiveIdBasic Authentication is not allowed.", new object[0]);
					Exception exception = new LocalizedException(Strings.ErrorBasicAndLiveIdBasicNotAllowed);
					ErrorCategory category = ErrorCategory.InvalidOperation;
					T dataObject5 = this.DataObject;
					base.WriteError(exception, category, dataObject5.Identity);
				}
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (!base.Fields.IsModified("Path"))
			{
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, this.VirtualDirectoryPath);
			}
			if (!base.Fields.IsModified("Name"))
			{
				this.Name = this.VirtualDirectoryName;
			}
			if (!base.Fields.IsModified("AppPoolId"))
			{
				base.AppPoolId = this.DefaultApplicationPoolId;
			}
			if (base.Role == VirtualDirectoryRole.ClientAccess && base.InternalUrl == null && this.DefaultInternalUrl != null)
			{
				base.InternalUrl = this.DefaultInternalUrl;
			}
		}

		protected override void InternalProcessComplete()
		{
			ADExchangeServiceVirtualDirectory adexchangeServiceVirtualDirectory = this.DataObject;
			base.InternalProcessComplete();
			ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(adexchangeServiceVirtualDirectory, new Task.TaskErrorLoggingDelegate(base.WriteError), this.MetabaseSetPropertiesFailureMessage);
			T dataObject = this.DataObject;
			if (dataObject.LiveIdBasicAuthentication != null)
			{
				T dataObject2 = this.DataObject;
				ExchangeServiceVDirHelper.SetLiveIdBasicAuthModule(dataObject2.LiveIdBasicAuthentication.Value, false, adexchangeServiceVirtualDirectory);
			}
			T dataObject3 = this.DataObject;
			if (dataObject3.OAuthAuthentication != null)
			{
				T dataObject4 = this.DataObject;
				ExchangeServiceVDirHelper.SetOAuthAuthenticationModule(dataObject4.OAuthAuthentication.Value, false, adexchangeServiceVirtualDirectory);
			}
			ExchangeServiceVDirHelper.CheckAndUpdateWindowsAuthProvidersIfNecessary(adexchangeServiceVirtualDirectory, new bool?(true));
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADExchangeServiceVirtualDirectory adexchangeServiceVirtualDirectory = (ADExchangeServiceVirtualDirectory)base.PrepareDataObject();
			if (base.Fields["BasicAuthentication"] == null && base.Fields["DigestAuthentication"] == null && base.Fields["WindowsAuthentication"] == null)
			{
				this.SetDefaultAuthenticationMethods(adexchangeServiceVirtualDirectory);
			}
			else
			{
				adexchangeServiceVirtualDirectory.BasicAuthentication = new bool?(this.BasicAuthentication);
				adexchangeServiceVirtualDirectory.DigestAuthentication = new bool?(this.DigestAuthentication);
				adexchangeServiceVirtualDirectory.WindowsAuthentication = new bool?(this.WindowsAuthentication);
			}
			return adexchangeServiceVirtualDirectory;
		}

		private bool GetAuthMethodFieldValue(string fieldName)
		{
			return base.Fields[fieldName] != null && (bool)base.Fields[fieldName];
		}

		[Parameter(Mandatory = false)]
		public bool BasicAuthentication
		{
			get
			{
				return this.GetAuthMethodFieldValue("BasicAuthentication");
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
				return this.GetAuthMethodFieldValue("DigestAuthentication");
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
				return this.GetAuthMethodFieldValue("WindowsAuthentication");
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

		internal new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}
	}
}
