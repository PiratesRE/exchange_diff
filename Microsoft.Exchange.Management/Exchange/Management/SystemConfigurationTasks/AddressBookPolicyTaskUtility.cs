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
	internal sealed class AddressBookPolicyTaskUtility
	{
		internal static MultiValuedProperty<ADObjectId> ValidateAddressBook(IConfigDataProvider session, AddressListIdParameter[] addressBooks, AddressBookPolicyTaskUtility.GetUniqueObject getAddressBook, AddressBookMailboxPolicy target, Task.TaskErrorLoggingDelegate writeError)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>(false, AddressBookMailboxPolicySchema.AddressLists, new object[0]);
			if (addressBooks != null)
			{
				foreach (AddressListIdParameter addressListIdParameter in addressBooks)
				{
					if (addressListIdParameter != null)
					{
						IConfigurable configurable = getAddressBook(addressListIdParameter, session, null, new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotFound(addressListIdParameter.ToString())), new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotUnique(addressListIdParameter.ToString())));
						if (configurable != null)
						{
							if (multiValuedProperty.Contains((ADObjectId)configurable.Identity))
							{
								writeError(new InvalidOperationException(Strings.ErrorOabALAlreadyAssigned((target.Identity != null) ? target.Identity.ToString() : target.Name, configurable.Identity.ToString())), ErrorCategory.InvalidOperation, target.Identity);
							}
							else
							{
								multiValuedProperty.Add((ADObjectId)configurable.Identity);
							}
						}
					}
				}
			}
			return multiValuedProperty;
		}

		internal delegate IConfigurable GetUniqueObject(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError);
	}
}
