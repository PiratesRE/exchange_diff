using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1997125541U, "InvalidSyncPhase");
			Strings.stringIDs.Add(1732782849U, "InvalidSubscriptionGuid");
			Strings.stringIDs.Add(1067634768U, "InvalidSubscriptionType");
			Strings.stringIDs.Add(4237821923U, "SystemMailboxSessionNotAvailable");
			Strings.stringIDs.Add(1923038367U, "CacheTokenNotAvailable");
			Strings.stringIDs.Add(2820269659U, "InvalidUserLegacyDn");
			Strings.stringIDs.Add(2705969914U, "FailedSetMailboxSubscriptionListTimestamp");
			Strings.stringIDs.Add(2794107333U, "FailureToReadCacheData");
			Strings.stringIDs.Add(39652958U, "FailureToRebuildCacheData");
			Strings.stringIDs.Add(1804653958U, "FailedGetMailboxSubscriptionListTimestamp");
			Strings.stringIDs.Add(157386845U, "TransportSyncManagerServiceName");
			Strings.stringIDs.Add(2176604858U, "InvalidSubscriptionMessageId");
			Strings.stringIDs.Add(816076645U, "InvalidDisabledStatus");
			Strings.stringIDs.Add(1083474379U, "InvalidUserMailboxGuid");
			Strings.stringIDs.Add(4289146560U, "InvalidSubscription");
			Strings.stringIDs.Add(934451165U, "FailedToDeleteCacheData");
		}

		public static LocalizedString InvalidSyncPhase
		{
			get
			{
				return new LocalizedString("InvalidSyncPhase", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CachePermanentExceptionInfo(Guid databaseGuid, Guid userMailboxGuid, string exceptionInfo)
		{
			return new LocalizedString("CachePermanentExceptionInfo", "Ex1F3285", false, true, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				userMailboxGuid,
				exceptionInfo
			});
		}

		public static LocalizedString InvalidSubscriptionGuid
		{
			get
			{
				return new LocalizedString("InvalidSubscriptionGuid", "Ex490D64", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSubscriptionType
		{
			get
			{
				return new LocalizedString("InvalidSubscriptionType", "Ex3BCC2D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMailboxSessionNotAvailable
		{
			get
			{
				return new LocalizedString("SystemMailboxSessionNotAvailable", "ExEDCD39", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheTokenNotAvailable
		{
			get
			{
				return new LocalizedString("CacheTokenNotAvailable", "ExA96E36", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxNotFoundExceptionInfo(Guid databaseGuid, Guid userMailboxGuid, string exceptionInfo)
		{
			return new LocalizedString("MailboxNotFoundExceptionInfo", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				userMailboxGuid,
				exceptionInfo
			});
		}

		public static LocalizedString InvalidUserLegacyDn
		{
			get
			{
				return new LocalizedString("InvalidUserLegacyDn", "Ex3CAD93", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedSetMailboxSubscriptionListTimestamp
		{
			get
			{
				return new LocalizedString("FailedSetMailboxSubscriptionListTimestamp", "Ex391E91", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureToReadCacheData
		{
			get
			{
				return new LocalizedString("FailureToReadCacheData", "Ex94881A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureToRebuildCacheData
		{
			get
			{
				return new LocalizedString("FailureToRebuildCacheData", "Ex58EC13", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedGetMailboxSubscriptionListTimestamp
		{
			get
			{
				return new LocalizedString("FailedGetMailboxSubscriptionListTimestamp", "Ex3209F2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportSyncManagerServiceName
		{
			get
			{
				return new LocalizedString("TransportSyncManagerServiceName", "Ex3EAED1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheCorruptExceptionInfo(Guid databaseGuid, Guid userMailboxGuid, string exceptionInfo)
		{
			return new LocalizedString("CacheCorruptExceptionInfo", "ExB4657A", false, true, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				userMailboxGuid,
				exceptionInfo
			});
		}

		public static LocalizedString InvalidSubscriptionMessageId
		{
			get
			{
				return new LocalizedString("InvalidSubscriptionMessageId", "Ex7AB2CE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDisabledStatus
		{
			get
			{
				return new LocalizedString("InvalidDisabledStatus", "Ex150D62", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheNotFoundExceptionInfo(Guid databaseGuid, Guid userMailboxGuid)
		{
			return new LocalizedString("CacheNotFoundExceptionInfo", "Ex4D8143", false, true, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				userMailboxGuid
			});
		}

		public static LocalizedString InvalidUserMailboxGuid
		{
			get
			{
				return new LocalizedString("InvalidUserMailboxGuid", "Ex4612F2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSubscription
		{
			get
			{
				return new LocalizedString("InvalidSubscription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheTransientExceptionInfo(Guid databaseGuid, Guid userMailboxGuid, string exceptionInfo)
		{
			return new LocalizedString("CacheTransientExceptionInfo", "Ex1CE4DE", false, true, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				userMailboxGuid,
				exceptionInfo
			});
		}

		public static LocalizedString FailedToDeleteCacheData
		{
			get
			{
				return new LocalizedString("FailedToDeleteCacheData", "Ex382FFD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(16);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Sync.Manager.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidSyncPhase = 1997125541U,
			InvalidSubscriptionGuid = 1732782849U,
			InvalidSubscriptionType = 1067634768U,
			SystemMailboxSessionNotAvailable = 4237821923U,
			CacheTokenNotAvailable = 1923038367U,
			InvalidUserLegacyDn = 2820269659U,
			FailedSetMailboxSubscriptionListTimestamp = 2705969914U,
			FailureToReadCacheData = 2794107333U,
			FailureToRebuildCacheData = 39652958U,
			FailedGetMailboxSubscriptionListTimestamp = 1804653958U,
			TransportSyncManagerServiceName = 157386845U,
			InvalidSubscriptionMessageId = 2176604858U,
			InvalidDisabledStatus = 816076645U,
			InvalidUserMailboxGuid = 1083474379U,
			InvalidSubscription = 4289146560U,
			FailedToDeleteCacheData = 934451165U
		}

		private enum ParamIDs
		{
			CachePermanentExceptionInfo,
			MailboxNotFoundExceptionInfo,
			CacheCorruptExceptionInfo,
			CacheNotFoundExceptionInfo,
			CacheTransientExceptionInfo
		}
	}
}
