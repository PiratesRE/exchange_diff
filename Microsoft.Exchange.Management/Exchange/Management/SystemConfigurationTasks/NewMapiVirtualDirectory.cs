using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "MapiVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewMapiVirtualDirectory : NewExchangeVirtualDirectory<ADMapiVirtualDirectory>
	{
		public NewMapiVirtualDirectory()
		{
			this.Name = "mapi";
			this.WebSiteName = null;
		}

		private new string WebSiteName
		{
			get
			{
				return base.WebSiteName;
			}
			set
			{
				base.WebSiteName = value;
			}
		}

		private new string Path
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

		private new string AppPoolId
		{
			get
			{
				return base.AppPoolId;
			}
			set
			{
				base.AppPoolId = value;
			}
		}

		private new string ApplicationRoot
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

		private new string Name
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

		[Parameter]
		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return this.iisAuthenticationMethods;
			}
			set
			{
				this.iisAuthenticationMethods = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMapiVirtualDirectory;
			}
		}

		protected override bool FailOnVirtualDirectoryAlreadyExists()
		{
			return false;
		}

		protected override bool InternalShouldCreateMetabaseObject()
		{
			return false;
		}

		private void ValidateParameterValues()
		{
			bool flag = base.ExternalUrl != null && !string.IsNullOrEmpty(base.ExternalUrl.ToString());
			bool flag2 = base.InternalUrl != null && !string.IsNullOrEmpty(base.InternalUrl.ToString());
			if (!flag && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMapiVirtualDirectoryMustSpecifyEitherInternalOrExternalUrl), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
			}
			if (flag)
			{
				base.ExternalUrl = new Uri(string.Format("https://{0}/mapi", base.ExternalUrl.Host));
			}
			if (flag2)
			{
				base.InternalUrl = new Uri(string.Format("https://{0}/mapi", base.InternalUrl.Host));
			}
			if (this.IISAuthenticationMethods == null)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMapiVirtualDirectoryMustSpecifyIISAuthenticationMethods), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
			}
			this.DataObject.IISAuthenticationMethods = this.iisAuthenticationMethods;
		}

		private bool IsInstalled()
		{
			ADMapiVirtualDirectory[] array = (base.DataSession as IConfigurationSession).Find<ADMapiVirtualDirectory>((ADObjectId)base.OwningServer.Identity, QueryScope.SubTree, null, null, 1);
			return array.Length > 0;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.ValidateParameterValues();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			try
			{
				if (this.IsInstalled())
				{
					base.WriteError(new ArgumentException(Strings.ErrorMapiHttpAlreadyEnabled(base.OwningServer.Fqdn), string.Empty), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					return;
				}
			}
			catch (ADTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.DataObject.Identity);
			}
			catch (DataValidationException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidData, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			IISConfigurationUtilities.TryRunIISConfigurationOperation(new IISConfigurationUtilities.IISConfigurationOperation(this.RunIisConfigurationOperation), 5, 2, this);
			TaskLogger.LogExit();
		}

		private void RunIisConfigurationOperation()
		{
			IISConfigurationUtilities.CreateAndConfigureLocalMapiHttpFrontEnd(this.IISAuthenticationMethods);
		}

		private const string VirtualDirectoryName = "mapi";

		private MultiValuedProperty<AuthenticationMethod> iisAuthenticationMethods;
	}
}
