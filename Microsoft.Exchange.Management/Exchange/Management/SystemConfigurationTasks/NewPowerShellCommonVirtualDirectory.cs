using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewPowerShellCommonVirtualDirectory<T> : NewExchangeServiceVirtualDirectory<T> where T : ADPowerShellCommonVirtualDirectory, new()
	{
		public NewPowerShellCommonVirtualDirectory()
		{
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
		}

		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public new string Name
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
		public new bool LimitMaximumMemory
		{
			get
			{
				return base.LimitMaximumMemory;
			}
			set
			{
				base.LimitMaximumMemory = value;
			}
		}

		protected override string VirtualDirectoryName
		{
			get
			{
				return this.Name;
			}
		}

		protected override void InternalProcessComplete()
		{
			base.InternalProcessComplete();
			if (this.CertificateAuthentication)
			{
				T dataObject = this.DataObject;
				dataObject.CertificateAuthentication = new bool?(true);
				ADExchangeServiceVirtualDirectory virtualDirectory = this.DataObject;
				Task.TaskErrorLoggingDelegate errorHandler = new Task.TaskErrorLoggingDelegate(base.WriteError);
				T dataObject2 = this.DataObject;
				ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(virtualDirectory, errorHandler, Strings.ErrorUpdatingVDir(dataObject2.MetabasePath, string.Empty));
			}
		}
	}
}
