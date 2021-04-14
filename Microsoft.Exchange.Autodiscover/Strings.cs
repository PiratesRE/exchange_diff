using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Autodiscover
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1005127777U, "NoError");
			Strings.stringIDs.Add(467439996U, "OutlookAddressNotFound");
			Strings.stringIDs.Add(2670476165U, "ExternalUrlNotFound");
			Strings.stringIDs.Add(3070945727U, "RedirectUrlForUser");
			Strings.stringIDs.Add(2828903911U, "InvalidBinarySecretHeader");
			Strings.stringIDs.Add(4170881310U, "MobileSyncBadAddress");
			Strings.stringIDs.Add(2743507733U, "ActiveDirectoryFailure");
			Strings.stringIDs.Add(1980408852U, "NoSettingsRequested");
			Strings.stringIDs.Add(813325346U, "SettingIsNotAvailable");
			Strings.stringIDs.Add(3025903418U, "InternalServerError");
			Strings.stringIDs.Add(934963490U, "RedirectAddress");
			Strings.stringIDs.Add(3758180504U, "OutlookBadAddress");
			Strings.stringIDs.Add(3863897719U, "ProviderIsNotAvailable");
			Strings.stringIDs.Add(3959337510U, "InvalidRequest");
			Strings.stringIDs.Add(2553117850U, "MobileSyncAddressNotFound");
			Strings.stringIDs.Add(2888966445U, "NotFederated");
			Strings.stringIDs.Add(1987653008U, "OutlookInvalidEMailAddress");
			Strings.stringIDs.Add(3403459873U, "InvalidDomain");
			Strings.stringIDs.Add(3720978267U, "MobileMailBoxNotFound");
			Strings.stringIDs.Add(554711641U, "MaxDomainsPerGetDomainSettingsRequestExceeded");
			Strings.stringIDs.Add(163658367U, "OutlookServerTooOld");
			Strings.stringIDs.Add(3929050450U, "InvalidUser");
			Strings.stringIDs.Add(3986215214U, "InvalidUserSetting");
			Strings.stringIDs.Add(1668664063U, "NoUsersRequested");
			Strings.stringIDs.Add(2706529289U, "InvalidPartnerTokenRequest");
			Strings.stringIDs.Add(2878595352U, "ServerBusy");
			Strings.stringIDs.Add(12114365U, "MaxUsersPerGetUserSettingsRequestExceeded");
			Strings.stringIDs.Add(2779025613U, "RedirectUrl");
			Strings.stringIDs.Add(1022239443U, "InvalidRequestedVersion");
			Strings.stringIDs.Add(2676368715U, "ADUnavailable");
			Strings.stringIDs.Add(2120454641U, "SoapEndpointNotSupported");
			Strings.stringIDs.Add(964474549U, "NoSettingsToReturn");
			Strings.stringIDs.Add(1797171863U, "MissingOrInvalidRequestedServerVersion");
		}

		public static LocalizedString NoError
		{
			get
			{
				return new LocalizedString("NoError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookAddressNotFound
		{
			get
			{
				return new LocalizedString("OutlookAddressNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalUrlNotFound
		{
			get
			{
				return new LocalizedString("ExternalUrlNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectUrlForUser
		{
			get
			{
				return new LocalizedString("RedirectUrlForUser", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidBinarySecretHeader
		{
			get
			{
				return new LocalizedString("InvalidBinarySecretHeader", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileSyncBadAddress
		{
			get
			{
				return new LocalizedString("MobileSyncBadAddress", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDirectoryFailure
		{
			get
			{
				return new LocalizedString("ActiveDirectoryFailure", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSettingsRequested
		{
			get
			{
				return new LocalizedString("NoSettingsRequested", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingIsNotAvailable
		{
			get
			{
				return new LocalizedString("SettingIsNotAvailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalServerError
		{
			get
			{
				return new LocalizedString("InternalServerError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectAddress
		{
			get
			{
				return new LocalizedString("RedirectAddress", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookBadAddress
		{
			get
			{
				return new LocalizedString("OutlookBadAddress", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProviderIsNotAvailable
		{
			get
			{
				return new LocalizedString("ProviderIsNotAvailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRequest
		{
			get
			{
				return new LocalizedString("InvalidRequest", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileSyncAddressNotFound
		{
			get
			{
				return new LocalizedString("MobileSyncAddressNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotFederated
		{
			get
			{
				return new LocalizedString("NotFederated", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookInvalidEMailAddress
		{
			get
			{
				return new LocalizedString("OutlookInvalidEMailAddress", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDomain
		{
			get
			{
				return new LocalizedString("InvalidDomain", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileMailBoxNotFound
		{
			get
			{
				return new LocalizedString("MobileMailBoxNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxDomainsPerGetDomainSettingsRequestExceeded
		{
			get
			{
				return new LocalizedString("MaxDomainsPerGetDomainSettingsRequestExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookServerTooOld
		{
			get
			{
				return new LocalizedString("OutlookServerTooOld", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUser
		{
			get
			{
				return new LocalizedString("InvalidUser", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUserSetting
		{
			get
			{
				return new LocalizedString("InvalidUserSetting", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoUsersRequested
		{
			get
			{
				return new LocalizedString("NoUsersRequested", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPartnerTokenRequest
		{
			get
			{
				return new LocalizedString("InvalidPartnerTokenRequest", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerBusy
		{
			get
			{
				return new LocalizedString("ServerBusy", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxUsersPerGetUserSettingsRequestExceeded
		{
			get
			{
				return new LocalizedString("MaxUsersPerGetUserSettingsRequestExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectUrl
		{
			get
			{
				return new LocalizedString("RedirectUrl", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRequestedVersion
		{
			get
			{
				return new LocalizedString("InvalidRequestedVersion", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADUnavailable
		{
			get
			{
				return new LocalizedString("ADUnavailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SoapEndpointNotSupported
		{
			get
			{
				return new LocalizedString("SoapEndpointNotSupported", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSettingsToReturn
		{
			get
			{
				return new LocalizedString("NoSettingsToReturn", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingOrInvalidRequestedServerVersion
		{
			get
			{
				return new LocalizedString("MissingOrInvalidRequestedServerVersion", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(33);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Autodiscover.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NoError = 1005127777U,
			OutlookAddressNotFound = 467439996U,
			ExternalUrlNotFound = 2670476165U,
			RedirectUrlForUser = 3070945727U,
			InvalidBinarySecretHeader = 2828903911U,
			MobileSyncBadAddress = 4170881310U,
			ActiveDirectoryFailure = 2743507733U,
			NoSettingsRequested = 1980408852U,
			SettingIsNotAvailable = 813325346U,
			InternalServerError = 3025903418U,
			RedirectAddress = 934963490U,
			OutlookBadAddress = 3758180504U,
			ProviderIsNotAvailable = 3863897719U,
			InvalidRequest = 3959337510U,
			MobileSyncAddressNotFound = 2553117850U,
			NotFederated = 2888966445U,
			OutlookInvalidEMailAddress = 1987653008U,
			InvalidDomain = 3403459873U,
			MobileMailBoxNotFound = 3720978267U,
			MaxDomainsPerGetDomainSettingsRequestExceeded = 554711641U,
			OutlookServerTooOld = 163658367U,
			InvalidUser = 3929050450U,
			InvalidUserSetting = 3986215214U,
			NoUsersRequested = 1668664063U,
			InvalidPartnerTokenRequest = 2706529289U,
			ServerBusy = 2878595352U,
			MaxUsersPerGetUserSettingsRequestExceeded = 12114365U,
			RedirectUrl = 2779025613U,
			InvalidRequestedVersion = 1022239443U,
			ADUnavailable = 2676368715U,
			SoapEndpointNotSupported = 2120454641U,
			NoSettingsToReturn = 964474549U,
			MissingOrInvalidRequestedServerVersion = 1797171863U
		}
	}
}
