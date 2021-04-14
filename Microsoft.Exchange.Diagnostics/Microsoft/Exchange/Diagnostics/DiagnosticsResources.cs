using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class DiagnosticsResources
	{
		static DiagnosticsResources()
		{
			DiagnosticsResources.stringIDs.Add(3725486369U, "NullSourceName");
			DiagnosticsResources.stringIDs.Add(1209655894U, "BreadCrumbSize");
			DiagnosticsResources.stringIDs.Add(306844145U, "InvalidPrivilegeName");
			DiagnosticsResources.stringIDs.Add(3193246957U, "RequestDetailsLoggerWasDisposed");
			DiagnosticsResources.stringIDs.Add(780120066U, "UnauthorizedAccess");
			DiagnosticsResources.stringIDs.Add(1925645844U, "AppendColumnNullKey");
			DiagnosticsResources.stringIDs.Add(966025339U, "SourceAlreadyExists");
			DiagnosticsResources.stringIDs.Add(2224301001U, "WrongThread");
			DiagnosticsResources.stringIDs.Add(2131565734U, "DatacenterInvalidRegistryException");
			DiagnosticsResources.stringIDs.Add(4000883259U, "TypeNotSupported");
			DiagnosticsResources.stringIDs.Add(595440494U, "InvalidCharacterInLoggedText");
			DiagnosticsResources.stringIDs.Add(2482240435U, "ToomanyParams");
			DiagnosticsResources.stringIDs.Add(3949951791U, "ExceptionActivityContextEnumMetadataOnly");
			DiagnosticsResources.stringIDs.Add(2281395385U, "RevertPrivilege");
			DiagnosticsResources.stringIDs.Add(3384754759U, "InvalidSourceName");
			DiagnosticsResources.stringIDs.Add(18235917U, "ExcInvalidOpPropertyBeforeEnd");
			DiagnosticsResources.stringIDs.Add(3490062012U, "ExceptionActivityContextKeyCollision");
		}

		public static LocalizedString ArgumentValueCannotBeParsed(string key, string value, string typeName)
		{
			return new LocalizedString("ArgumentValueCannotBeParsed", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				key,
				value,
				typeName
			});
		}

		public static LocalizedString ExceptionStartInvokedTwice(string debugInfo)
		{
			return new LocalizedString("ExceptionStartInvokedTwice", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				debugInfo
			});
		}

		public static LocalizedString NullSourceName
		{
			get
			{
				return new LocalizedString("NullSourceName", "Ex8F1FD3", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionWantedVersionButFileNotFound(string filename)
		{
			return new LocalizedString("ExceptionWantedVersionButFileNotFound", "ExAA26C5", false, true, DiagnosticsResources.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString BreadCrumbSize
		{
			get
			{
				return new LocalizedString("BreadCrumbSize", "Ex3A077B", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPrivilegeName
		{
			get
			{
				return new LocalizedString("InvalidPrivilegeName", "Ex08AC8B", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestDetailsLoggerWasDisposed
		{
			get
			{
				return new LocalizedString("RequestDetailsLoggerWasDisposed", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnauthorizedAccess
		{
			get
			{
				return new LocalizedString("UnauthorizedAccess", "Ex3E7948", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppendColumnNullKey
		{
			get
			{
				return new LocalizedString("AppendColumnNullKey", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSetupVersionInformationCorrupt(string keyPath)
		{
			return new LocalizedString("ExceptionSetupVersionInformationCorrupt", "ExC7F7D6", false, true, DiagnosticsResources.ResourceManager, new object[]
			{
				keyPath
			});
		}

		public static LocalizedString SourceAlreadyExists
		{
			get
			{
				return new LocalizedString("SourceAlreadyExists", "ExA0479A", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongThread
		{
			get
			{
				return new LocalizedString("WrongThread", "ExD42091", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatacenterInvalidRegistryException
		{
			get
			{
				return new LocalizedString("DatacenterInvalidRegistryException", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TypeNotSupported
		{
			get
			{
				return new LocalizedString("TypeNotSupported", "ExC7DC8A", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCharacterInLoggedText
		{
			get
			{
				return new LocalizedString("InvalidCharacterInLoggedText", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToomanyParams
		{
			get
			{
				return new LocalizedString("ToomanyParams", "ExB54861", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionActivityContextEnumMetadataOnly
		{
			get
			{
				return new LocalizedString("ExceptionActivityContextEnumMetadataOnly", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionMshSetupInformationCorrupt(string keyPath)
		{
			return new LocalizedString("ExceptionMshSetupInformationCorrupt", "ExABC22E", false, true, DiagnosticsResources.ResourceManager, new object[]
			{
				keyPath
			});
		}

		public static LocalizedString ExceptionMustStartBeforeSuspend(string debugInfo)
		{
			return new LocalizedString("ExceptionMustStartBeforeSuspend", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				debugInfo
			});
		}

		public static LocalizedString RevertPrivilege
		{
			get
			{
				return new LocalizedString("RevertPrivilege", "Ex23E94D", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionMustStartBeforeEnd(string debugInfo)
		{
			return new LocalizedString("ExceptionMustStartBeforeEnd", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				debugInfo
			});
		}

		public static LocalizedString ArgumentNotSupported(string argumentName, string supportedArguments)
		{
			return new LocalizedString("ArgumentNotSupported", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				argumentName,
				supportedArguments
			});
		}

		public static LocalizedString InvalidSourceName
		{
			get
			{
				return new LocalizedString("InvalidSourceName", "Ex1016A8", false, true, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionFileVersionNotFound(string filename)
		{
			return new LocalizedString("ExceptionFileVersionNotFound", "ExD1B2B2", false, true, DiagnosticsResources.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString ExceptionScopeAlreadyExists(string debugInfo)
		{
			return new LocalizedString("ExceptionScopeAlreadyExists", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				debugInfo
			});
		}

		public static LocalizedString ExceptionOutOfScope(string debugInfo)
		{
			return new LocalizedString("ExceptionOutOfScope", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				debugInfo
			});
		}

		public static LocalizedString ExcInvalidOpPropertyBeforeEnd
		{
			get
			{
				return new LocalizedString("ExcInvalidOpPropertyBeforeEnd", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArgumentDuplicated(string msg)
		{
			return new LocalizedString("ArgumentDuplicated", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ExceptionActivityContextMustBeCleared(string debugInfo)
		{
			return new LocalizedString("ExceptionActivityContextMustBeCleared", "", false, false, DiagnosticsResources.ResourceManager, new object[]
			{
				debugInfo
			});
		}

		public static LocalizedString ExceptionActivityContextKeyCollision
		{
			get
			{
				return new LocalizedString("ExceptionActivityContextKeyCollision", "", false, false, DiagnosticsResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(DiagnosticsResources.IDs key)
		{
			return new LocalizedString(DiagnosticsResources.stringIDs[(uint)key], DiagnosticsResources.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(17);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Diagnostics.DiagnosticsResources", typeof(DiagnosticsResources).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NullSourceName = 3725486369U,
			BreadCrumbSize = 1209655894U,
			InvalidPrivilegeName = 306844145U,
			RequestDetailsLoggerWasDisposed = 3193246957U,
			UnauthorizedAccess = 780120066U,
			AppendColumnNullKey = 1925645844U,
			SourceAlreadyExists = 966025339U,
			WrongThread = 2224301001U,
			DatacenterInvalidRegistryException = 2131565734U,
			TypeNotSupported = 4000883259U,
			InvalidCharacterInLoggedText = 595440494U,
			ToomanyParams = 2482240435U,
			ExceptionActivityContextEnumMetadataOnly = 3949951791U,
			RevertPrivilege = 2281395385U,
			InvalidSourceName = 3384754759U,
			ExcInvalidOpPropertyBeforeEnd = 18235917U,
			ExceptionActivityContextKeyCollision = 3490062012U
		}

		private enum ParamIDs
		{
			ArgumentValueCannotBeParsed,
			ExceptionStartInvokedTwice,
			ExceptionWantedVersionButFileNotFound,
			ExceptionSetupVersionInformationCorrupt,
			ExceptionMshSetupInformationCorrupt,
			ExceptionMustStartBeforeSuspend,
			ExceptionMustStartBeforeEnd,
			ArgumentNotSupported,
			ExceptionFileVersionNotFound,
			ExceptionScopeAlreadyExists,
			ExceptionOutOfScope,
			ArgumentDuplicated,
			ExceptionActivityContextMustBeCleared
		}
	}
}
