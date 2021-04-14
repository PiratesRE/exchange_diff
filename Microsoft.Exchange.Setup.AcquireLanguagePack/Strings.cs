using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2736429093U, "VerifyingUpdates");
			Strings.stringIDs.Add(168100618U, "BaseXMLFileNotLoaded");
			Strings.stringIDs.Add(1799593368U, "VerifyingMsps");
			Strings.stringIDs.Add(1395665443U, "InvalidLanguageBundle");
			Strings.stringIDs.Add(2925052803U, "ErrorInLog");
			Strings.stringIDs.Add(686568928U, "NotEnoughDiskSpace");
			Strings.stringIDs.Add(2061069263U, "SignatureFailed");
			Strings.stringIDs.Add(871634395U, "UnableToFindBuildVersion");
			Strings.stringIDs.Add(3339504604U, "VerifyingLangPackBundle");
			Strings.stringIDs.Add(509984218U, "CheckForAvailableSpace");
			Strings.stringIDs.Add(3065873541U, "IncompatibleBundle");
			Strings.stringIDs.Add(3541939918U, "ErrorCreatingRegKey");
			Strings.stringIDs.Add(4285450436U, "UnableToDownload");
		}

		public static LocalizedString fWLinkNotFound(string exVersion, string pathToLPVersioning)
		{
			return new LocalizedString("fWLinkNotFound", "ExA75EC1", false, true, Strings.ResourceManager, new object[]
			{
				exVersion,
				pathToLPVersioning
			});
		}

		public static LocalizedString URLCantBeReached(string URL)
		{
			return new LocalizedString("URLCantBeReached", "ExE32C68", false, true, Strings.ResourceManager, new object[]
			{
				URL
			});
		}

		public static LocalizedString VerifyingUpdates
		{
			get
			{
				return new LocalizedString("VerifyingUpdates", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotExist(string name)
		{
			return new LocalizedString("NotExist", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString WrongFileType(string name)
		{
			return new LocalizedString("WrongFileType", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidFieldDataSize(uint field)
		{
			return new LocalizedString("InvalidFieldDataSize", "", false, false, Strings.ResourceManager, new object[]
			{
				field
			});
		}

		public static LocalizedString BaseXMLFileNotLoaded
		{
			get
			{
				return new LocalizedString("BaseXMLFileNotLoaded", "ExBA9350", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LPVersioningExtractionFailed(string pathToBundle)
		{
			return new LocalizedString("LPVersioningExtractionFailed", "Ex586BC5", false, true, Strings.ResourceManager, new object[]
			{
				pathToBundle
			});
		}

		public static LocalizedString VerifyingMsps
		{
			get
			{
				return new LocalizedString("VerifyingMsps", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidLanguageBundle
		{
			get
			{
				return new LocalizedString("InvalidLanguageBundle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInLog
		{
			get
			{
				return new LocalizedString("ErrorInLog", "Ex37776C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SignatureVerificationFailed(string pathToFile)
		{
			return new LocalizedString("SignatureVerificationFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				pathToFile
			});
		}

		public static LocalizedString IsNullOrEmpty(string name)
		{
			return new LocalizedString("IsNullOrEmpty", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MspValidationFailedOn(string name)
		{
			return new LocalizedString("MspValidationFailedOn", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NotEnoughDiskSpace
		{
			get
			{
				return new LocalizedString("NotEnoughDiskSpace", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SignatureFailed1(string path)
		{
			return new LocalizedString("SignatureFailed1", "ExEAD8BF", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString InvalidSaveToPath(string saveToPath)
		{
			return new LocalizedString("InvalidSaveToPath", "Ex48B774", false, true, Strings.ResourceManager, new object[]
			{
				saveToPath
			});
		}

		public static LocalizedString SignatureFailed
		{
			get
			{
				return new LocalizedString("SignatureFailed", "ExE5C374", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindBuildVersion
		{
			get
			{
				return new LocalizedString("UnableToFindBuildVersion", "ExB342F6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindBuildVersion1(string xmlPath)
		{
			return new LocalizedString("UnableToFindBuildVersion1", "Ex0A6FF5", false, true, Strings.ResourceManager, new object[]
			{
				xmlPath
			});
		}

		public static LocalizedString VerifyingLangPackBundle
		{
			get
			{
				return new LocalizedString("VerifyingLangPackBundle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientDiskSpace(double requiredSize)
		{
			return new LocalizedString("InsufficientDiskSpace", "ExA1C4F0", false, true, Strings.ResourceManager, new object[]
			{
				requiredSize
			});
		}

		public static LocalizedString CheckForAvailableSpace
		{
			get
			{
				return new LocalizedString("CheckForAvailableSpace", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFile(string fileName)
		{
			return new LocalizedString("InvalidFile", "", false, false, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString IncompatibleBundle
		{
			get
			{
				return new LocalizedString("IncompatibleBundle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCreatingRegKey
		{
			get
			{
				return new LocalizedString("ErrorCreatingRegKey", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToDownload
		{
			get
			{
				return new LocalizedString("UnableToDownload", "Ex86FA31", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(13);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Setup.AcquireLanguagePack.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			VerifyingUpdates = 2736429093U,
			BaseXMLFileNotLoaded = 168100618U,
			VerifyingMsps = 1799593368U,
			InvalidLanguageBundle = 1395665443U,
			ErrorInLog = 2925052803U,
			NotEnoughDiskSpace = 686568928U,
			SignatureFailed = 2061069263U,
			UnableToFindBuildVersion = 871634395U,
			VerifyingLangPackBundle = 3339504604U,
			CheckForAvailableSpace = 509984218U,
			IncompatibleBundle = 3065873541U,
			ErrorCreatingRegKey = 3541939918U,
			UnableToDownload = 4285450436U
		}

		private enum ParamIDs
		{
			fWLinkNotFound,
			URLCantBeReached,
			NotExist,
			WrongFileType,
			InvalidFieldDataSize,
			LPVersioningExtractionFailed,
			SignatureVerificationFailed,
			IsNullOrEmpty,
			MspValidationFailedOn,
			SignatureFailed1,
			InvalidSaveToPath,
			UnableToFindBuildVersion1,
			InsufficientDiskSpace,
			InvalidFile
		}
	}
}
