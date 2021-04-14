using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class QueryBaseDNRestrictionModifyObjectProvisioningHandler : ProvisioningHandlerBase
	{
		public sealed override bool UpdateAffectedIConfigurable(IConfigurable writeableIConfigurable)
		{
			ADObject adobject;
			if (writeableIConfigurable is ADPresentationObject)
			{
				adobject = ((ADPresentationObject)writeableIConfigurable).DataObject;
			}
			else
			{
				adobject = (ADObject)writeableIConfigurable;
			}
			ADUser aduser = adobject as ADUser;
			if (aduser == null)
			{
				return false;
			}
			if (!aduser.QueryBaseDNRestrictionEnabled && ADObjectId.Equals(aduser.QueryBaseDN, aduser.Id))
			{
				aduser.QueryBaseDN = null;
				return true;
			}
			if (aduser.QueryBaseDNRestrictionEnabled && !ADObjectId.Equals(aduser.QueryBaseDN, aduser.Id))
			{
				aduser.QueryBaseDN = aduser.Id;
				return true;
			}
			return false;
		}
	}
}
