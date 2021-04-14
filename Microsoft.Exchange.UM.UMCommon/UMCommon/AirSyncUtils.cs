using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class AirSyncUtils
	{
		public static bool AddAirSyncPhoneNumber(ADUser user, string number, out TelephoneNumberProcessStatus status)
		{
			IADRecipientLookup tenantRecipientLookup = ADRecipientLookupFactory.CreateFromADRecipient(user, true);
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(user);
			UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(user.UMRecipientDialPlanId);
			if (!dialPlanFromId.SupportsAirSync())
			{
				status = TelephoneNumberProcessStatus.DialPlanNotSupported;
				return false;
			}
			if (!Utils.IsUriValid(number, UMUriType.E164))
			{
				status = TelephoneNumberProcessStatus.PhoneNumberInvalidFormat;
				return false;
			}
			if (!number.StartsWith(string.Format(CultureInfo.InvariantCulture, "+{0}", new object[]
			{
				dialPlanFromId.CountryOrRegionCode
			}), StringComparison.Ordinal))
			{
				status = TelephoneNumberProcessStatus.PhoneNumberInvalidCountryCode;
				return false;
			}
			if (number.Length != dialPlanFromId.NumberOfDigitsInExtension + dialPlanFromId.CountryOrRegionCode.Length + 1)
			{
				status = TelephoneNumberProcessStatus.PhoneNumberInvalidLength;
				return false;
			}
			if (user.PhoneNumberExistsInUMAddresses(number))
			{
				status = TelephoneNumberProcessStatus.PhoneNumberAlreadyRegistered;
				return true;
			}
			if (user.IsAirSyncNumberQuotaReached())
			{
				status = TelephoneNumberProcessStatus.PhoneNumberReachQuota;
				return false;
			}
			bool flag = false;
			LocalizedException ex;
			Utils.IsPhoneNumberRegistered(tenantRecipientLookup, iadsystemConfigurationLookup, user, dialPlanFromId, number, out ex, out status);
			if (TelephoneNumberProcessStatus.PhoneNumberUsedByOthers == status)
			{
				return false;
			}
			if (TelephoneNumberProcessStatus.PhoneNumberAlreadyRegistered == status)
			{
				flag = true;
			}
			string phoneNumber = number.Substring(number.Length - dialPlanFromId.NumberOfDigitsInExtension);
			bool flag2 = false;
			Utils.IsPhoneNumberRegistered(tenantRecipientLookup, iadsystemConfigurationLookup, user, dialPlanFromId, phoneNumber, out ex, out status);
			if (TelephoneNumberProcessStatus.PhoneNumberUsedByOthers == status)
			{
				return false;
			}
			if (TelephoneNumberProcessStatus.PhoneNumberAlreadyRegistered == status)
			{
				flag2 = true;
			}
			try
			{
				user.AddASUMProxyAddress(number, dialPlanFromId);
				if (!flag)
				{
					user.AddEUMProxyAddress(number, dialPlanFromId);
				}
				if (!flag2)
				{
					user.AddEUMProxyAddress(phoneNumber, dialPlanFromId);
				}
			}
			catch (Exception)
			{
				status = TelephoneNumberProcessStatus.Failure;
				return false;
			}
			status = TelephoneNumberProcessStatus.Success;
			return true;
		}

		public static bool RemoveAirSyncPhoneNumber(ADUser user, string number, out TelephoneNumberProcessStatus status)
		{
			if (!Utils.IsUriValid(number, UMUriType.E164))
			{
				status = TelephoneNumberProcessStatus.PhoneNumberInvalidFormat;
				return false;
			}
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(user);
			UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(user.UMRecipientDialPlanId);
			if (!dialPlanFromId.SupportsAirSync())
			{
				status = TelephoneNumberProcessStatus.DialPlanNotSupported;
				return false;
			}
			if (!user.PhoneNumberExistsInUMAddresses(number))
			{
				status = TelephoneNumberProcessStatus.Success;
				return true;
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(number);
			UMMailbox.RemoveProxy(user, user.UMAddresses, ProxyAddressPrefix.ASUM, arrayList, dialPlanFromId);
			arrayList.Add(number.Substring(number.Length - dialPlanFromId.NumberOfDigitsInExtension));
			UMMailbox.RemoveProxy(user, user.EmailAddresses, ProxyAddressPrefix.UM, arrayList, dialPlanFromId);
			status = TelephoneNumberProcessStatus.Success;
			return true;
		}

		public static bool GetTelephonyInfo(ADUser user, string phoneNumber, out TelephonyInfo ti)
		{
			ti = TelephonyInfo.Empty;
			phoneNumber = Utils.TrimSpaces(phoneNumber);
			if (phoneNumber == null)
			{
				return false;
			}
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(user);
			UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(user.UMRecipientDialPlanId);
			return AirSyncUtils.GetTelephonyInfo(dialPlanFromId, phoneNumber, out ti);
		}

		public static bool GetTelephonyInfo(UMDialPlan dialPlan, string phoneNumber, out TelephonyInfo ti)
		{
			ti = TelephonyInfo.Empty;
			string text = null;
			int num = -1;
			foreach (string text2 in dialPlan.PilotIdentifierList)
			{
				int num2 = AirSyncUtils.PrefixMatchCount(phoneNumber, text2);
				if (num2 > num)
				{
					num = num2;
					text = text2;
				}
			}
			if (text != null)
			{
				PhoneNumber phoneNumber2;
				PhoneNumber.TryParse(text, out phoneNumber2);
				ti = new TelephonyInfo(phoneNumber2, phoneNumber2);
				return true;
			}
			return false;
		}

		private static int PrefixMatchCount(string str1, string str2)
		{
			int i;
			for (i = 0; i < Math.Min(str1.Length, str2.Length); i++)
			{
				if (str1[i] != str2[i])
				{
					return i;
				}
			}
			return i;
		}
	}
}
