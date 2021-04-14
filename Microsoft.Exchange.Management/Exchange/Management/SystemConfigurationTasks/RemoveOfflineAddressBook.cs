using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "OfflineAddressBook", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveOfflineAddressBook : RemoveSystemConfigurationObjectTask<OfflineAddressBookIdParameter, OfflineAddressBook>
	{
		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveOfflineAddressBook(this.Identity.ToString());
			}
		}

		private bool HandleRemoveWithAssociatedAddressBookPolicies()
		{
			base.WriteError(new InvalidOperationException(Strings.ErrorRemoveOfflineAddressBookWithAssociatedAddressBookPolicies(base.DataObject.Name)), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			return false;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			OfflineAddressBook dataObject = base.DataObject;
			if (dataObject.CheckForAssociatedAddressBookPolicies() && !this.HandleRemoveWithAssociatedAddressBookPolicies())
			{
				TaskLogger.LogExit();
				return;
			}
			if (base.DataObject.IsDefault && !this.Force && !base.ShouldContinue(Strings.RemoveDefaultOAB(this.Identity.ToString())))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
