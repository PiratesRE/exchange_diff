using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "AvailabilityAddressSpace", SupportsShouldProcess = true)]
	public sealed class AddAvailabilityAddressSpace : NewMultitenancyFixedNameSystemConfigurationObjectTask<AvailabilityAddressSpace>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddAvailabilityAddressSpace(this.ForestName.ToString(), this.AccessMethod.ToString());
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string ForestName
		{
			get
			{
				return this.DataObject.ForestName;
			}
			set
			{
				this.DataObject.ForestName = value;
			}
		}

		[Parameter(Mandatory = true)]
		public AvailabilityAccessMethod AccessMethod
		{
			get
			{
				return this.DataObject.AccessMethod;
			}
			set
			{
				this.DataObject.AccessMethod = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseServiceAccount
		{
			get
			{
				return this.DataObject.UseServiceAccount;
			}
			set
			{
				this.DataObject.UseServiceAccount = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential Credentials
		{
			get
			{
				return (PSCredential)base.Fields["Credentials"];
			}
			set
			{
				base.Fields["Credentials"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri ProxyUrl
		{
			get
			{
				return this.DataObject.ProxyUrl;
			}
			set
			{
				this.DataObject.ProxyUrl = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri TargetAutodiscoverEpr
		{
			get
			{
				return this.DataObject.TargetAutodiscoverEpr;
			}
			set
			{
				this.DataObject.TargetAutodiscoverEpr = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			IConfigurationSession session = (IConfigurationSession)base.DataSession;
			if (Datacenter.IsMultiTenancyEnabled())
			{
				if (this.AccessMethod != AvailabilityAccessMethod.OrgWideFB)
				{
					base.ThrowTerminatingError(new InvalidAvailabilityAccessMethodException(), ErrorCategory.InvalidOperation, this.ForestName);
				}
				if (this.TargetAutodiscoverEpr == null)
				{
					base.ThrowTerminatingError(new AvailabilityAddressSpaceInvalidTargetAutodiscoverEprException(), ErrorCategory.InvalidOperation, this.ForestName);
				}
			}
			if (this.AccessMethod == AvailabilityAccessMethod.OrgWideFBBasic && this.TargetAutodiscoverEpr == null)
			{
				base.ThrowTerminatingError(new AvailabilityAddressSpaceInvalidTargetAutodiscoverEprException(), ErrorCategory.InvalidOperation, this.ForestName);
			}
			this.DataObject.SetId(session, this.ForestName);
			PSCredential credentials = this.Credentials;
			if (credentials != null)
			{
				this.DataObject.UserName = credentials.UserName;
				this.DataObject.SetPassword(credentials.Password);
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			TaskLogger.LogExit();
		}

		private const string propCredentials = "Credentials";
	}
}
