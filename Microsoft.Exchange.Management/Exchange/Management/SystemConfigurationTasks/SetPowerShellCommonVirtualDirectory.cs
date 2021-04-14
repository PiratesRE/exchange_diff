using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetPowerShellCommonVirtualDirectory<T> : SetExchangeServiceVirtualDirectory<T> where T : ADPowerShellCommonVirtualDirectory, new()
	{
		[Parameter(Mandatory = false)]
		public bool CertificateAuthentication
		{
			get
			{
				return base.Fields["CertificateAuthentication"] != null && (bool)base.Fields["CertificateAuthentication"];
			}
			set
			{
				base.Fields["CertificateAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableCertificateHeaderAuthModule
		{
			get
			{
				return base.Fields["EnableCertificateHeaderAuthModule"] != null && (bool)base.Fields["EnableCertificateHeaderAuthModule"];
			}
			set
			{
				base.Fields["EnableCertificateHeaderAuthModule"] = value;
			}
		}

		protected new ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
		{
			get
			{
				return ExtendedProtectionTokenCheckingMode.None;
			}
			set
			{
			}
		}

		protected new MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		protected new MultiValuedProperty<string> ExtendedProtectionSPNList
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		protected abstract PowerShellVirtualDirectoryType AllowedVirtualDirectoryType { get; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			T dataObject = this.DataObject;
			if (dataObject.VirtualDirectoryType != this.AllowedVirtualDirectoryType)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(T).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			base.InternalEnableLiveIdNegotiateAuxiliaryModule();
			if (base.Fields["CertificateAuthentication"] != null)
			{
				T dataObject = this.DataObject;
				dataObject.CertificateAuthentication = new bool?((bool)base.Fields["CertificateAuthentication"]);
				ADExchangeServiceVirtualDirectory virtualDirectory = this.DataObject;
				Task.TaskErrorLoggingDelegate errorHandler = new Task.TaskErrorLoggingDelegate(base.WriteError);
				T dataObject2 = this.DataObject;
				ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(virtualDirectory, errorHandler, Strings.ErrorUpdatingVDir(dataObject2.MetabasePath, string.Empty));
				T dataObject3 = this.DataObject;
				ExchangeServiceVDirHelper.ConfigureAnonymousAuthentication(dataObject3.MetabasePath, false);
			}
			if (base.Fields["EnableCertificateHeaderAuthModule"] != null)
			{
				base.SetCertificateHeaderAuthenticationModule(this.EnableCertificateHeaderAuthModule, false);
			}
		}
	}
}
