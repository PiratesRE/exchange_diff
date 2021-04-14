using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "ClientAccessArray", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveClientAccessArray : RemoveSystemConfigurationObjectTask<ClientAccessArrayIdParameter, ClientAccessArray>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveClientAccessArray(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.DataObject.ExchangeVersion.IsOlderThan(ClientAccessArray.MinimumSupportedExchangeObjectVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorCannotChangeBecauseTooOld(base.DataObject.ExchangeVersion.ToString(), ClientAccessArray.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
			ClientAccessArray dataObject = base.DataObject;
			this.CheckArrayRemovalMembership(dataObject);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<ClientAccessArray>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorClientAccessArrayNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorClientAccessArrayNotUnique(this.Identity.ToString())));
		}

		private void CheckArrayRemovalMembership(ClientAccessArray array)
		{
			if (array.Servers.Count > 0)
			{
				base.WriteError(new TaskException(Strings.ErrorArrayRemovalMembership(base.DataObject.Identity.ToString(), array.Servers.Count)), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
