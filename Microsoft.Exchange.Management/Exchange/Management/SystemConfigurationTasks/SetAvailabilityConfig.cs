using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AvailabilityConfig", SupportsShouldProcess = true)]
	public sealed class SetAvailabilityConfig : SetMultitenancySingletonSystemConfigurationObjectTask<AvailabilityConfig>
	{
		[Parameter(Mandatory = false)]
		public SecurityPrincipalIdParameter OrgWideAccount
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["OrgWideAccount"];
			}
			set
			{
				base.Fields["OrgWideAccount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecurityPrincipalIdParameter PerUserAccount
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["PerUserAccount"];
			}
			set
			{
				base.Fields["PerUserAccount"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAvailabilityConfig;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
				return configurationSession.GetOrgContainerId();
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.DataObject.OrgWideAccount = this.ValidateUser((SecurityPrincipalIdParameter)base.Fields["OrgWideAccount"]);
			this.DataObject.PerUserAccount = this.ValidateUser((SecurityPrincipalIdParameter)base.Fields["PerUserAccount"]);
		}

		private ADObjectId ValidateUser(SecurityPrincipalIdParameter principalId)
		{
			if (principalId == null)
			{
				return null;
			}
			IEnumerable<ADRecipient> objects = principalId.GetObjects<ADRecipient>(null, base.TenantGlobalCatalogSession);
			ADObjectId result;
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotFound(principalId.ToString())), ErrorCategory.ObjectNotFound, null);
				}
				ADObjectId adobjectId = (ADObjectId)enumerator.Current.Identity;
				if (enumerator.MoveNext())
				{
					base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorUserNotUnique(principalId.ToString())), ErrorCategory.InvalidData, null);
				}
				this.WriteWarning(Strings.AccountPrivilegeWarning);
				result = adobjectId;
			}
			return result;
		}

		private const string propOrgWideAccount = "OrgWideAccount";

		private const string propPerUserAccount = "PerUserAccount";
	}
}
