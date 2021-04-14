using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3266846781U, "AddToFavorites");
			Strings.stringIDs.Add(1297161044U, "ComponentNotInitialized");
			Strings.stringIDs.Add(2723658401U, "LiveIdLabel");
			Strings.stringIDs.Add(2216610333U, "LogonErrorLogoutUrlText");
			Strings.stringIDs.Add(3228633421U, "OutlookWebAccess");
			Strings.stringIDs.Add(641346049U, "ErrorUnexpectedFailure");
			Strings.stringIDs.Add(3147877247U, "GoThereNowButtonText");
			Strings.stringIDs.Add(2956867297U, "ComponentAlreadyInitialized");
			Strings.stringIDs.Add(4265046865U, "EducationMessage");
			Strings.stringIDs.Add(675314258U, "SignUpSuggestion");
			Strings.stringIDs.Add(3937131501U, "WhyMessage");
			Strings.stringIDs.Add(2886870364U, "GetLiveIdMessage");
			Strings.stringIDs.Add(933672694U, "ErrorTitle");
			Strings.stringIDs.Add(4082986936U, "NextButtonText");
			Strings.stringIDs.Add(579076706U, "InvalidLiveIdWarning");
			Strings.stringIDs.Add(3976540663U, "ConnectedToExchange");
			Strings.stringIDs.Add(2308016970U, "LogonCopyright");
		}

		public static LocalizedString AddToFavorites
		{
			get
			{
				return new LocalizedString("AddToFavorites", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveExternalFCodeException(int fCode, string msppErrorString)
		{
			return new LocalizedString("LiveExternalFCodeException", "", false, false, Strings.ResourceManager, new object[]
			{
				fCode,
				msppErrorString
			});
		}

		public static LocalizedString ComponentNotInitialized
		{
			get
			{
				return new LocalizedString("ComponentNotInitialized", "ExE5CD58", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveConfigurationHRESULTExceptionMessage(uint error, string textError)
		{
			return new LocalizedString("LiveConfigurationHRESULTExceptionMessage", "Ex844F7A", false, true, Strings.ResourceManager, new object[]
			{
				error,
				textError
			});
		}

		public static LocalizedString LiveIdLabel
		{
			get
			{
				return new LocalizedString("LiveIdLabel", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogonErrorLogoutUrlText
		{
			get
			{
				return new LocalizedString("LogonErrorLogoutUrlText", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveClientHRESULTExceptionMessage(uint error, string textError)
		{
			return new LocalizedString("LiveClientHRESULTExceptionMessage", "Ex21A033", false, true, Strings.ResourceManager, new object[]
			{
				error,
				textError
			});
		}

		public static LocalizedString OutlookWebAccess
		{
			get
			{
				return new LocalizedString("OutlookWebAccess", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnexpectedFailure
		{
			get
			{
				return new LocalizedString("ErrorUnexpectedFailure", "ExC6DD63", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveClientFCodeException(int fCode, string msppErrorString)
		{
			return new LocalizedString("LiveClientFCodeException", "", false, false, Strings.ResourceManager, new object[]
			{
				fCode,
				msppErrorString
			});
		}

		public static LocalizedString LiveExternalUnknownFCodeException(string fCodeString, string msppErrorString)
		{
			return new LocalizedString("LiveExternalUnknownFCodeException", "", false, false, Strings.ResourceManager, new object[]
			{
				fCodeString,
				msppErrorString
			});
		}

		public static LocalizedString LiveOperationExceptionMessage(uint error, string textError)
		{
			return new LocalizedString("LiveOperationExceptionMessage", "Ex7CFB70", false, true, Strings.ResourceManager, new object[]
			{
				error,
				textError
			});
		}

		public static LocalizedString LiveTransientHRESULTExceptionMessage(uint error, string textError)
		{
			return new LocalizedString("LiveTransientHRESULTExceptionMessage", "Ex6AC7BF", false, true, Strings.ResourceManager, new object[]
			{
				error,
				textError
			});
		}

		public static LocalizedString GoThereNowButtonText
		{
			get
			{
				return new LocalizedString("GoThereNowButtonText", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentAlreadyInitialized
		{
			get
			{
				return new LocalizedString("ComponentAlreadyInitialized", "Ex5F7B71", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EducationMessage
		{
			get
			{
				return new LocalizedString("EducationMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveTransientFCodeException(int fCode, string msppErrorString)
		{
			return new LocalizedString("LiveTransientFCodeException", "", false, false, Strings.ResourceManager, new object[]
			{
				fCode,
				msppErrorString
			});
		}

		public static LocalizedString SignUpSuggestion
		{
			get
			{
				return new LocalizedString("SignUpSuggestion", "Ex51B22A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhyMessage
		{
			get
			{
				return new LocalizedString("WhyMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLiveIdMessage
		{
			get
			{
				return new LocalizedString("GetLiveIdMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveConfigurationFCodeException(int fCode, string msppErrorString)
		{
			return new LocalizedString("LiveConfigurationFCodeException", "", false, false, Strings.ResourceManager, new object[]
			{
				fCode,
				msppErrorString
			});
		}

		public static LocalizedString ErrorTitle
		{
			get
			{
				return new LocalizedString("ErrorTitle", "ExA60E2A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NextButtonText
		{
			get
			{
				return new LocalizedString("NextButtonText", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidLiveIdWarning
		{
			get
			{
				return new LocalizedString("InvalidLiveIdWarning", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveExternalHRESULTExceptionMessage(uint error, string textError)
		{
			return new LocalizedString("LiveExternalHRESULTExceptionMessage", "Ex6DE241", false, true, Strings.ResourceManager, new object[]
			{
				error,
				textError
			});
		}

		public static LocalizedString ConnectedToExchange
		{
			get
			{
				return new LocalizedString("ConnectedToExchange", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogonCopyright
		{
			get
			{
				return new LocalizedString("LogonCopyright", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(17);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Clients.Security.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			AddToFavorites = 3266846781U,
			ComponentNotInitialized = 1297161044U,
			LiveIdLabel = 2723658401U,
			LogonErrorLogoutUrlText = 2216610333U,
			OutlookWebAccess = 3228633421U,
			ErrorUnexpectedFailure = 641346049U,
			GoThereNowButtonText = 3147877247U,
			ComponentAlreadyInitialized = 2956867297U,
			EducationMessage = 4265046865U,
			SignUpSuggestion = 675314258U,
			WhyMessage = 3937131501U,
			GetLiveIdMessage = 2886870364U,
			ErrorTitle = 933672694U,
			NextButtonText = 4082986936U,
			InvalidLiveIdWarning = 579076706U,
			ConnectedToExchange = 3976540663U,
			LogonCopyright = 2308016970U
		}

		private enum ParamIDs
		{
			LiveExternalFCodeException,
			LiveConfigurationHRESULTExceptionMessage,
			LiveClientHRESULTExceptionMessage,
			LiveClientFCodeException,
			LiveExternalUnknownFCodeException,
			LiveOperationExceptionMessage,
			LiveTransientHRESULTExceptionMessage,
			LiveTransientFCodeException,
			LiveConfigurationFCodeException,
			LiveExternalHRESULTExceptionMessage
		}
	}
}
