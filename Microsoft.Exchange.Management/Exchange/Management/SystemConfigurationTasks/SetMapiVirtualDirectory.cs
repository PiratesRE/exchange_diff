using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "MapiVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMapiVirtualDirectory : SetExchangeVirtualDirectory<ADMapiVirtualDirectory>
	{
		private new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
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

		private new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
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

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)base.Fields["IISAuthenticationMethods"];
			}
			set
			{
				base.Fields["IISAuthenticationMethods"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ApplyDefaults
		{
			get
			{
				return this.applyDefaults;
			}
			set
			{
				this.applyDefaults = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMapiVirtualDirectory;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADMapiVirtualDirectory admapiVirtualDirectory = (ADMapiVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			bool flag = admapiVirtualDirectory.ExternalUrl != null && !string.IsNullOrEmpty(admapiVirtualDirectory.ExternalUrl.ToString());
			bool flag2 = admapiVirtualDirectory.InternalUrl != null && !string.IsNullOrEmpty(admapiVirtualDirectory.InternalUrl.ToString());
			if (!flag && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMapiVirtualDirectoryMustSpecifyEitherInternalOrExternalUrl), ErrorCategory.InvalidArgument, admapiVirtualDirectory.Identity);
				return null;
			}
			if (flag)
			{
				admapiVirtualDirectory.ExternalUrl = new Uri(string.Format("https://{0}/mapi", admapiVirtualDirectory.ExternalUrl.Host));
			}
			if (flag2)
			{
				admapiVirtualDirectory.InternalUrl = new Uri(string.Format("https://{0}/mapi", admapiVirtualDirectory.InternalUrl.Host));
			}
			if (this.IISAuthenticationMethods == null)
			{
				this.IISAuthenticationMethods = admapiVirtualDirectory.IISAuthenticationMethods;
			}
			else
			{
				admapiVirtualDirectory.IISAuthenticationMethods = this.IISAuthenticationMethods;
			}
			if (admapiVirtualDirectory.IISAuthenticationMethods == null)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMapiVirtualDirectoryMustSpecifyIISAuthenticationMethods), ErrorCategory.InvalidArgument, admapiVirtualDirectory.Identity);
				return null;
			}
			if (this.applyDefaults && !SetMapiVirtualDirectory.IsLocalServer(admapiVirtualDirectory))
			{
				base.WriteError(new ArgumentException(Strings.ErrorMapiVirtualDirectoryMustBeLocalServerToReset), ErrorCategory.InvalidArgument, admapiVirtualDirectory.Identity);
				return null;
			}
			this.metabasePath = admapiVirtualDirectory.MetabasePath;
			return admapiVirtualDirectory;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			IISConfigurationUtilities.TryRunIISConfigurationOperation(new IISConfigurationUtilities.IISConfigurationOperation(this.RunIisConfigurationOperation), 5, 2, this);
			TaskLogger.LogExit();
		}

		private static string ServerShortName(string serverFqdn)
		{
			int num = serverFqdn.IndexOf('.');
			if (num != -1)
			{
				return serverFqdn.Substring(0, num);
			}
			return serverFqdn;
		}

		private static bool IsLocalServer(ADMapiVirtualDirectory dataObject)
		{
			string hostName = IisUtility.GetHostName(dataObject.MetabasePath);
			if (string.IsNullOrEmpty(hostName))
			{
				return false;
			}
			string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(false);
			if (hostName.IndexOf('.') != -1)
			{
				return hostName.Equals(localComputerFqdn, StringComparison.InvariantCultureIgnoreCase);
			}
			string value = SetMapiVirtualDirectory.ServerShortName(localComputerFqdn);
			string text = SetMapiVirtualDirectory.ServerShortName(hostName);
			return text.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}

		private void RunIisConfigurationOperation()
		{
			if (this.applyDefaults)
			{
				IISConfigurationUtilities.CreateAndConfigureLocalMapiHttpFrontEnd(this.IISAuthenticationMethods);
				return;
			}
			IISConfigurationUtilities.UpdateRemoteMapiHttpFrontEnd(IisUtility.GetHostName(this.metabasePath), this.IISAuthenticationMethods);
		}

		private const string IISAuthenticationMethodsKey = "IISAuthenticationMethods";

		private string metabasePath;

		private bool applyDefaults;
	}
}
