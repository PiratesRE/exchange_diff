using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Live.DomainServices
{
	internal static class Strings
	{
		public static LocalizedString ErrorWLCDPartnerAccessException(string url, string message)
		{
			return new LocalizedString("ErrorWLCDPartnerAccessException", "ExE444F3", false, true, Strings.ResourceManager, new object[]
			{
				url,
				message
			});
		}

		public static LocalizedString ErrorCertificateHasExpired(string certSubject)
		{
			return new LocalizedString("ErrorCertificateHasExpired", "Ex57D6EE", false, true, Strings.ResourceManager, new object[]
			{
				certSubject
			});
		}

		public static LocalizedString ErrorInvalidPartnerCert(string message, string key)
		{
			return new LocalizedString("ErrorInvalidPartnerCert", "Ex5BF0B3", false, true, Strings.ResourceManager, new object[]
			{
				message,
				key
			});
		}

		public static LocalizedString ErrorReadingRegistryValue(string key, string value)
		{
			return new LocalizedString("ErrorReadingRegistryValue", "ExC62F87", false, true, Strings.ResourceManager, new object[]
			{
				key,
				value
			});
		}

		public static LocalizedString ErrorCertificateNotFound(string certSubject)
		{
			return new LocalizedString("ErrorCertificateNotFound", "Ex03079B", false, true, Strings.ResourceManager, new object[]
			{
				certSubject
			});
		}

		public static LocalizedString ErrorMemberNotAuthorized(string message)
		{
			return new LocalizedString("ErrorMemberNotAuthorized", "ExD08D8B", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorInvalidPartnerSpecified(string message, string key)
		{
			return new LocalizedString("ErrorInvalidPartnerSpecified", "ExB9BD8A", false, true, Strings.ResourceManager, new object[]
			{
				message,
				key
			});
		}

		public static LocalizedString ErrorInvalidManagementCertificate(string message)
		{
			return new LocalizedString("ErrorInvalidManagementCertificate", "ExA3B8DA", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorReadingRegistryKey(string key)
		{
			return new LocalizedString("ErrorReadingRegistryKey", "Ex14EFF8", false, true, Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString ErrorCertificateHasNoPrivateKey(string certSubject)
		{
			return new LocalizedString("ErrorCertificateHasNoPrivateKey", "ExE96793", false, true, Strings.ResourceManager, new object[]
			{
				certSubject
			});
		}

		public static LocalizedString ErrorOpeningCertificateStore(string store)
		{
			return new LocalizedString("ErrorOpeningCertificateStore", "Ex907F81", false, true, Strings.ResourceManager, new object[]
			{
				store
			});
		}

		public static LocalizedString ErrorPartnerNotAuthorized(string message)
		{
			return new LocalizedString("ErrorPartnerNotAuthorized", "Ex514400", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Live.DomainServices.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			ErrorWLCDPartnerAccessException,
			ErrorCertificateHasExpired,
			ErrorInvalidPartnerCert,
			ErrorReadingRegistryValue,
			ErrorCertificateNotFound,
			ErrorMemberNotAuthorized,
			ErrorInvalidPartnerSpecified,
			ErrorInvalidManagementCertificate,
			ErrorReadingRegistryKey,
			ErrorCertificateHasNoPrivateKey,
			ErrorOpeningCertificateStore,
			ErrorPartnerNotAuthorized
		}
	}
}
