using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public abstract class PlayOnPhoneUserRequest : PlayOnPhoneRequest
	{
		internal ADRecipient CallerInfo
		{
			get
			{
				if (this.callerInfo == null)
				{
					this.GetCallerInfo();
				}
				return this.callerInfo;
			}
		}

		private void GetCallerInfo()
		{
			if (string.IsNullOrEmpty(base.ProxyAddress))
			{
				throw new InvalidADRecipientException();
			}
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(base.TenantGuid);
			ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(base.UserObjectGuid));
			if (adrecipient == null)
			{
				throw new InvalidADRecipientException();
			}
			this.callerInfo = adrecipient;
		}

		protected ADRecipient callerInfo;
	}
}
