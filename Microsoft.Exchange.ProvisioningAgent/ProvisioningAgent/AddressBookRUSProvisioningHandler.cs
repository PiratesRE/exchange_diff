using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.DefaultProvisioningAgent.Rus;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AddressBookRUSProvisioningHandler : RUSProvisioningHandler
	{
		protected override void Validate(IConfigurable readOnlyADObject, List<ProvisioningValidationError> errors)
		{
			base.Validate(readOnlyADObject, errors);
			ExTraceGlobals.RusTracer.TraceDebug<string, string>((long)this.GetHashCode(), "RUSProvisioningHandler.Validate: readOnlyADObject={0}, TaskName={1}", readOnlyADObject.Identity.ToString(), base.TaskName);
			ADObject adobject;
			if (readOnlyADObject is ADPresentationObject)
			{
				adobject = ((ADPresentationObject)readOnlyADObject).DataObject;
			}
			else
			{
				adobject = (ADObject)readOnlyADObject;
			}
			AddressBookBase addressBookBase = adobject as AddressBookBase;
			if (addressBookBase != null)
			{
				errors.AddRange(new AddressBookHandler(null, null, null, null, base.PartitionId, base.UserScope, base.ProvisioningCache, base.LogMessage).Validate(addressBookBase));
			}
		}

		internal static readonly string[] SupportedTasks = new string[]
		{
			"New-AddressList",
			"Set-AddressList",
			"Update-AddressList",
			"New-GlobalAddressList",
			"Set-GlobalAddressList",
			"Update-GlobalAddressList"
		};
	}
}
