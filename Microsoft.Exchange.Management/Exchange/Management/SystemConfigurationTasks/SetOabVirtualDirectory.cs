using System;
using System.Collections;
using System.DirectoryServices;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OabVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOabVirtualDirectory : SetExchangeVirtualDirectory<ADOabVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOabVirtualDirectory(this.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(IISNotInstalledException).IsInstanceOfType(exception);
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

		[Parameter]
		public bool RequireSSL
		{
			get
			{
				return (bool)base.Fields["RequireSSL"];
			}
			set
			{
				base.Fields["RequireSSL"] = value;
			}
		}

		[Parameter]
		public bool BasicAuthentication
		{
			get
			{
				return (bool)base.Fields["BasicAuthentication"];
			}
			set
			{
				base.Fields["BasicAuthentication"] = value;
			}
		}

		[Parameter]
		public bool WindowsAuthentication
		{
			get
			{
				return (bool)base.Fields["WindowsAuthentication"];
			}
			set
			{
				base.Fields["WindowsAuthentication"] = value;
			}
		}

		[Parameter]
		public bool OAuthAuthentication
		{
			get
			{
				return (bool)base.Fields["OAuthAuthentication"];
			}
			set
			{
				base.Fields["OAuthAuthentication"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.BasicAuthentication && !this.DataObject.RequireSSL)
			{
				this.WriteWarning(Strings.OABVdirBasicAuthWithoutSSL(this.DataObject.Identity.ToString()));
			}
			if (!base.HasErrors && this.DataObject.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new ArgumentException(Strings.ErrorOabVirtualDirectoryNameIsImmutable, "Name"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADOabVirtualDirectory adoabVirtualDirectory = (ADOabVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains("RequireSSL"))
			{
				adoabVirtualDirectory.RequireSSL = (bool)base.Fields["RequireSSL"];
			}
			if (base.Fields.Contains("BasicAuthentication"))
			{
				adoabVirtualDirectory.BasicAuthentication = (bool)base.Fields["BasicAuthentication"];
			}
			if (base.Fields.Contains("WindowsAuthentication"))
			{
				adoabVirtualDirectory.WindowsAuthentication = (bool)base.Fields["WindowsAuthentication"];
			}
			if (base.Fields.Contains("OAuthAuthentication"))
			{
				adoabVirtualDirectory.OAuthAuthentication = (bool)base.Fields["OAuthAuthentication"];
			}
			return adoabVirtualDirectory;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			ADOabVirtualDirectory adoabVirtualDirectory = (ADOabVirtualDirectory)dataObject;
			adoabVirtualDirectory.OAuthAuthentication = adoabVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.OAuth);
			adoabVirtualDirectory.RequireSSL = IisUtility.SSLEnabled(adoabVirtualDirectory.MetabasePath);
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adoabVirtualDirectory.MetabasePath))
			{
				adoabVirtualDirectory.BasicAuthentication = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic);
				adoabVirtualDirectory.WindowsAuthentication = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Ntlm);
			}
			dataObject.ResetChangeTracking();
			base.StampChangesOn(dataObject);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject.IsModified(ADOabVirtualDirectorySchema.OAuthAuthentication))
			{
				ExchangeServiceVDirHelper.SetOAuthAuthenticationModule(this.OAuthAuthentication, false, this.DataObject);
			}
			bool flag = this.DataObject.IsModified(ADOabVirtualDirectorySchema.RequireSSL) | this.DataObject.IsModified(ADOabVirtualDirectorySchema.BasicAuthentication) | this.DataObject.IsModified(ADOabVirtualDirectorySchema.WindowsAuthentication);
			base.InternalProcessRecord();
			if (flag)
			{
				try
				{
					SetOabVirtualDirectory.UpdateMetabase(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				finally
				{
					if (base.HasErrors)
					{
						base.WriteError(new LocalizedException(Strings.ErrorADOperationSucceededButMetabaseOperationFailed), ErrorCategory.WriteError, this.DataObject.Identity);
					}
				}
			}
			TaskLogger.LogExit();
		}

		internal static void UpdateMetabase(ADOabVirtualDirectory virtualDirectory, Task.TaskErrorLoggingDelegate handler)
		{
			try
			{
				DirectoryEntry directoryEntry2;
				DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(virtualDirectory.MetabasePath);
				try
				{
					ArrayList arrayList = new ArrayList();
					int num = (int)(IisUtility.GetIisPropertyValue("AccessSSLFlags", IisUtility.GetProperties(directoryEntry)) ?? 0);
					if (virtualDirectory.RequireSSL)
					{
						num |= 8;
					}
					else
					{
						num &= -9;
						num &= -257;
						num &= -65;
					}
					arrayList.Add(new MetabaseProperty("AccessSSLFlags", num, true));
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic, virtualDirectory.BasicAuthentication);
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WindowsIntegrated, virtualDirectory.WindowsAuthentication);
					IisUtility.SetProperties(directoryEntry, arrayList);
					directoryEntry.CommitChanges();
					IisUtility.CommitMetabaseChanges((virtualDirectory.Server == null) ? null : virtualDirectory.Server.ToString());
				}
				finally
				{
					if (directoryEntry2 != null)
					{
						((IDisposable)directoryEntry2).Dispose();
					}
				}
			}
			catch (COMException exception)
			{
				handler(exception, ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
		}
	}
}
