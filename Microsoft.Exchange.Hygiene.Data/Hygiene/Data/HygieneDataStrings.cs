using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class HygieneDataStrings
	{
		static HygieneDataStrings()
		{
			HygieneDataStrings.stringIDs.Add(2445616851U, "ErrorEmptyList");
			HygieneDataStrings.stringIDs.Add(590305096U, "ErrorPermanentDALException");
			HygieneDataStrings.stringIDs.Add(1719343280U, "ErrorEmptyGuid");
			HygieneDataStrings.stringIDs.Add(3093538361U, "ErrorDataStoreUnavailable");
			HygieneDataStrings.stringIDs.Add(797725276U, "ErrorTransientDALExceptionMaxRetries");
			HygieneDataStrings.stringIDs.Add(2833073812U, "ErrorTransientDALExceptionAmbientTransaction");
			HygieneDataStrings.stringIDs.Add(3127795643U, "ErrorTransactionNotSupported");
		}

		public static LocalizedString ErrorPropertyValueTypeMismatch(string propertyName, string propertyType, string valueType)
		{
			return new LocalizedString("ErrorPropertyValueTypeMismatch", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				propertyName,
				propertyType,
				valueType
			});
		}

		public static LocalizedString ErrorEmptyList
		{
			get
			{
				return new LocalizedString("ErrorEmptyList", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPermanentDALException
		{
			get
			{
				return new LocalizedString("ErrorPermanentDALException", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidArgumentTenantIdMismatch(Guid domainTargetEnvironmentTenantId, Guid targetServiceTenantId)
		{
			return new LocalizedString("ErrorInvalidArgumentTenantIdMismatch", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				domainTargetEnvironmentTenantId,
				targetServiceTenantId
			});
		}

		public static LocalizedString ErrorQueryFilterType(string filter)
		{
			return new LocalizedString("ErrorQueryFilterType", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString ErrorEmptyGuid
		{
			get
			{
				return new LocalizedString("ErrorEmptyGuid", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidArgumentDomainKeyMismatch(string domainTargetEnvironmentDomainKey, string targetServiceDomainKey)
		{
			return new LocalizedString("ErrorInvalidArgumentDomainKeyMismatch", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				domainTargetEnvironmentDomainKey,
				targetServiceDomainKey
			});
		}

		public static LocalizedString ErrorInvalidArgumentDomainNameMismatch(string domainTargetEnvironmentDomainName, string targetServiceDomainName)
		{
			return new LocalizedString("ErrorInvalidArgumentDomainNameMismatch", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				domainTargetEnvironmentDomainName,
				targetServiceDomainName
			});
		}

		public static LocalizedString ErrorDataStoreUnavailable
		{
			get
			{
				return new LocalizedString("ErrorDataStoreUnavailable", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTransientDALExceptionMaxRetries
		{
			get
			{
				return new LocalizedString("ErrorTransientDALExceptionMaxRetries", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidDataStoreType(string storeType)
		{
			return new LocalizedString("ErrorInvalidDataStoreType", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				storeType
			});
		}

		public static LocalizedString ErrorMultipleMatchForUserProxy(string proxyAddress, string matchingIds)
		{
			return new LocalizedString("ErrorMultipleMatchForUserProxy", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				proxyAddress,
				matchingIds
			});
		}

		public static LocalizedString ErrorCannotFindClientCertificate(string certificateName)
		{
			return new LocalizedString("ErrorCannotFindClientCertificate", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				certificateName
			});
		}

		public static LocalizedString ErrorUnsupportedInterface(string typeName, string interfaceName)
		{
			return new LocalizedString("ErrorUnsupportedInterface", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				typeName,
				interfaceName
			});
		}

		public static LocalizedString ErrorTransientDALExceptionAmbientTransaction
		{
			get
			{
				return new LocalizedString("ErrorTransientDALExceptionAmbientTransaction", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDataProviderIsTooBusy(string store, string partition, string copyId)
		{
			return new LocalizedString("ErrorDataProviderIsTooBusy", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				store,
				partition,
				copyId
			});
		}

		public static LocalizedString ErrorInvalidInstanceType(string typeName)
		{
			return new LocalizedString("ErrorInvalidInstanceType", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString ErrorInvalidDataOperationException(string dataOperationType)
		{
			return new LocalizedString("ErrorInvalidDataOperationException", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				dataOperationType
			});
		}

		public static LocalizedString ErrorInvalidObjectTypeForSession(string session, string type)
		{
			return new LocalizedString("ErrorInvalidObjectTypeForSession", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				session,
				type
			});
		}

		public static LocalizedString ErrorTransactionNotSupported
		{
			get
			{
				return new LocalizedString("ErrorTransactionNotSupported", "", false, false, HygieneDataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidColumnType(string propName, string typeName)
		{
			return new LocalizedString("ErrorInvalidColumnType", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				propName,
				typeName
			});
		}

		public static LocalizedString ErrorObjectIdTypeNotSupported(string objectIdType)
		{
			return new LocalizedString("ErrorObjectIdTypeNotSupported", "", false, false, HygieneDataStrings.ResourceManager, new object[]
			{
				objectIdType
			});
		}

		public static LocalizedString GetLocalizedString(HygieneDataStrings.IDs key)
		{
			return new LocalizedString(HygieneDataStrings.stringIDs[(uint)key], HygieneDataStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(7);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Hygiene.Data.DataStrings", typeof(HygieneDataStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorEmptyList = 2445616851U,
			ErrorPermanentDALException = 590305096U,
			ErrorEmptyGuid = 1719343280U,
			ErrorDataStoreUnavailable = 3093538361U,
			ErrorTransientDALExceptionMaxRetries = 797725276U,
			ErrorTransientDALExceptionAmbientTransaction = 2833073812U,
			ErrorTransactionNotSupported = 3127795643U
		}

		private enum ParamIDs
		{
			ErrorPropertyValueTypeMismatch,
			ErrorInvalidArgumentTenantIdMismatch,
			ErrorQueryFilterType,
			ErrorInvalidArgumentDomainKeyMismatch,
			ErrorInvalidArgumentDomainNameMismatch,
			ErrorInvalidDataStoreType,
			ErrorMultipleMatchForUserProxy,
			ErrorCannotFindClientCertificate,
			ErrorUnsupportedInterface,
			ErrorDataProviderIsTooBusy,
			ErrorInvalidInstanceType,
			ErrorInvalidDataOperationException,
			ErrorInvalidObjectTypeForSession,
			ErrorInvalidColumnType,
			ErrorObjectIdTypeNotSupported
		}
	}
}
