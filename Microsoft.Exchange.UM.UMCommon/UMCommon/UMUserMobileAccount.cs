using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMUserMobileAccount
	{
		private UMUserMobileAccount(ADUser user)
		{
			this.user = user;
		}

		public static bool TryCreateUMUserMobileAccount(ExchangePrincipal userExchangePrincipal, out UMUserMobileAccount account)
		{
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(userExchangePrincipal.MailboxInfo.OrganizationId, null, null, false);
			ADRecipient adrecipient = iadrecipientLookup.LookupByExchangePrincipal(userExchangePrincipal);
			ADUser aduser = adrecipient as ADUser;
			if (aduser != null && aduser.UMEnabled)
			{
				account = new UMUserMobileAccount(aduser);
				return true;
			}
			account = null;
			return false;
		}

		public bool TryRegisterNumber(PhoneNumber telephoneNumber, out TelephoneNumberProcessStatus status)
		{
			bool result = AirSyncUtils.AddAirSyncPhoneNumber(this.user, telephoneNumber.ToDial, out status);
			if (status == TelephoneNumberProcessStatus.Success)
			{
				this.user.Session.Save(this.user);
			}
			return result;
		}

		public bool TryDeregisterNumber(PhoneNumber telephoneNumber, out TelephoneNumberProcessStatus status)
		{
			bool result = AirSyncUtils.RemoveAirSyncPhoneNumber(this.user, telephoneNumber.ToDial, out status);
			if (status == TelephoneNumberProcessStatus.Success)
			{
				this.user.Session.Save(this.user);
			}
			return result;
		}

		public bool TryGetTelephonyInfo(PhoneNumber number, out TelephonyInfo ti)
		{
			return AirSyncUtils.GetTelephonyInfo(this.user, number.ToDial, out ti);
		}

		private ADUser user;
	}
}
