using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class EncodersStrings
	{
		static EncodersStrings()
		{
			EncodersStrings.stringIDs.Add(3966390553U, "MacBinFileNameTooLong");
			EncodersStrings.stringIDs.Add(25286151U, "BinHexEncoderInternalError");
			EncodersStrings.stringIDs.Add(217720121U, "BinHexHeaderBadFileNameLength");
			EncodersStrings.stringIDs.Add(1288990070U, "EncStrCannotCloneWriteableStream");
			EncodersStrings.stringIDs.Add(812742886U, "BinHexDecoderLineTooLong");
			EncodersStrings.stringIDs.Add(810486593U, "BinHexDecoderInternalError");
			EncodersStrings.stringIDs.Add(2498997775U, "BinHexDecoderDataCorrupt");
			EncodersStrings.stringIDs.Add(2916696602U, "BinHexDecoderFoundInvalidCharacter");
			EncodersStrings.stringIDs.Add(804376101U, "BinHexHeaderInvalidNameLength");
			EncodersStrings.stringIDs.Add(2503051247U, "MacBinBadVersion");
			EncodersStrings.stringIDs.Add(2749850901U, "BinHexHeaderIncomplete");
			EncodersStrings.stringIDs.Add(450079973U, "BinHexDecoderFileNameTooLong");
			EncodersStrings.stringIDs.Add(2959384831U, "UUDecoderInvalidData");
			EncodersStrings.stringIDs.Add(2345827606U, "EncStrCannotRead");
			EncodersStrings.stringIDs.Add(4253113056U, "BinHexHeaderUnsupportedVersion");
			EncodersStrings.stringIDs.Add(2009660788U, "UUDecoderInvalidDataBadLine");
			EncodersStrings.stringIDs.Add(1330362610U, "BinHexHeaderInvalidCrc");
			EncodersStrings.stringIDs.Add(2479018069U, "BinHexDecoderFirstNonWhitespaceMustBeColon");
			EncodersStrings.stringIDs.Add(3717726462U, "MacBinHeaderMustBe128Long");
			EncodersStrings.stringIDs.Add(67282974U, "EncStrCannotSeek");
			EncodersStrings.stringIDs.Add(3114903713U, "BinHexEncoderDoesNotSupportResourceFork");
			EncodersStrings.stringIDs.Add(3183910392U, "BinHexEncoderDataCorruptCannotFinishEncoding");
			EncodersStrings.stringIDs.Add(2984114443U, "MacBinInvalidData");
			EncodersStrings.stringIDs.Add(2549049936U, "BinHexHeaderTooSmall");
			EncodersStrings.stringIDs.Add(1639523968U, "QPEncoderNoSpaceForLineBreak");
			EncodersStrings.stringIDs.Add(2739587701U, "BinHexDecoderBadResourceForkCrc");
			EncodersStrings.stringIDs.Add(62154753U, "BinHexDecoderLineCorrupt");
			EncodersStrings.stringIDs.Add(2823726815U, "BinHexDecoderBadCrc");
			EncodersStrings.stringIDs.Add(1415490657U, "EncStrCannotWrite");
		}

		public static string MacBinFileNameTooLong
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("MacBinFileNameTooLong");
			}
		}

		public static string BinHexEncoderInternalError
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexEncoderInternalError");
			}
		}

		public static string EncStrLengthExceeded(int sum, int length)
		{
			return string.Format(EncodersStrings.ResourceManager.GetString("EncStrLengthExceeded"), sum, length);
		}

		public static string BinHexHeaderBadFileNameLength
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexHeaderBadFileNameLength");
			}
		}

		public static string EncStrCannotCloneWriteableStream
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("EncStrCannotCloneWriteableStream");
			}
		}

		public static string BinHexDecoderLineTooLong
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderLineTooLong");
			}
		}

		public static string BinHexDecoderInternalError
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderInternalError");
			}
		}

		public static string BinHexDecoderDataCorrupt
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderDataCorrupt");
			}
		}

		public static string BinHexDecoderFoundInvalidCharacter
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderFoundInvalidCharacter");
			}
		}

		public static string BinHexHeaderInvalidNameLength
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexHeaderInvalidNameLength");
			}
		}

		public static string ThisEncoderDoesNotSupportCloning(string type)
		{
			return string.Format(EncodersStrings.ResourceManager.GetString("ThisEncoderDoesNotSupportCloning"), type);
		}

		public static string MacBinBadVersion
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("MacBinBadVersion");
			}
		}

		public static string BinHexHeaderIncomplete
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexHeaderIncomplete");
			}
		}

		public static string BinHexDecoderFileNameTooLong
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderFileNameTooLong");
			}
		}

		public static string UUDecoderInvalidData
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("UUDecoderInvalidData");
			}
		}

		public static string EncStrCannotRead
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("EncStrCannotRead");
			}
		}

		public static string BinHexHeaderUnsupportedVersion
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexHeaderUnsupportedVersion");
			}
		}

		public static string UUDecoderInvalidDataBadLine
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("UUDecoderInvalidDataBadLine");
			}
		}

		public static string BinHexHeaderInvalidCrc
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexHeaderInvalidCrc");
			}
		}

		public static string BinHexDecoderFirstNonWhitespaceMustBeColon
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderFirstNonWhitespaceMustBeColon");
			}
		}

		public static string UUEncoderFileNameTooLong(int maxChars)
		{
			return string.Format(EncodersStrings.ResourceManager.GetString("UUEncoderFileNameTooLong"), maxChars);
		}

		public static string MacBinHeaderMustBe128Long
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("MacBinHeaderMustBe128Long");
			}
		}

		public static string EncStrCannotSeek
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("EncStrCannotSeek");
			}
		}

		public static string EncStrCannotCloneChildStream(string className)
		{
			return string.Format(EncodersStrings.ResourceManager.GetString("EncStrCannotCloneChildStream"), className);
		}

		public static string BinHexEncoderDoesNotSupportResourceFork
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexEncoderDoesNotSupportResourceFork");
			}
		}

		public static string BinHexEncoderDataCorruptCannotFinishEncoding
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexEncoderDataCorruptCannotFinishEncoding");
			}
		}

		public static string MacBinInvalidData
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("MacBinInvalidData");
			}
		}

		public static string BinHexHeaderTooSmall
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexHeaderTooSmall");
			}
		}

		public static string MacBinIconOffsetTooLarge(int max)
		{
			return string.Format(EncodersStrings.ResourceManager.GetString("MacBinIconOffsetTooLarge"), max);
		}

		public static string QPEncoderNoSpaceForLineBreak
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("QPEncoderNoSpaceForLineBreak");
			}
		}

		public static string BinHexDecoderBadResourceForkCrc
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderBadResourceForkCrc");
			}
		}

		public static string BinHexDecoderLineCorrupt
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderLineCorrupt");
			}
		}

		public static string BinHexDecoderBadCrc
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("BinHexDecoderBadCrc");
			}
		}

		public static string EncStrCannotWrite
		{
			get
			{
				return EncodersStrings.ResourceManager.GetString("EncStrCannotWrite");
			}
		}

		public static string GetLocalizedString(EncodersStrings.IDs key)
		{
			return EncodersStrings.ResourceManager.GetString(EncodersStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(29);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.EncodersStrings", typeof(EncodersStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MacBinFileNameTooLong = 3966390553U,
			BinHexEncoderInternalError = 25286151U,
			BinHexHeaderBadFileNameLength = 217720121U,
			EncStrCannotCloneWriteableStream = 1288990070U,
			BinHexDecoderLineTooLong = 812742886U,
			BinHexDecoderInternalError = 810486593U,
			BinHexDecoderDataCorrupt = 2498997775U,
			BinHexDecoderFoundInvalidCharacter = 2916696602U,
			BinHexHeaderInvalidNameLength = 804376101U,
			MacBinBadVersion = 2503051247U,
			BinHexHeaderIncomplete = 2749850901U,
			BinHexDecoderFileNameTooLong = 450079973U,
			UUDecoderInvalidData = 2959384831U,
			EncStrCannotRead = 2345827606U,
			BinHexHeaderUnsupportedVersion = 4253113056U,
			UUDecoderInvalidDataBadLine = 2009660788U,
			BinHexHeaderInvalidCrc = 1330362610U,
			BinHexDecoderFirstNonWhitespaceMustBeColon = 2479018069U,
			MacBinHeaderMustBe128Long = 3717726462U,
			EncStrCannotSeek = 67282974U,
			BinHexEncoderDoesNotSupportResourceFork = 3114903713U,
			BinHexEncoderDataCorruptCannotFinishEncoding = 3183910392U,
			MacBinInvalidData = 2984114443U,
			BinHexHeaderTooSmall = 2549049936U,
			QPEncoderNoSpaceForLineBreak = 1639523968U,
			BinHexDecoderBadResourceForkCrc = 2739587701U,
			BinHexDecoderLineCorrupt = 62154753U,
			BinHexDecoderBadCrc = 2823726815U,
			EncStrCannotWrite = 1415490657U
		}

		private enum ParamIDs
		{
			EncStrLengthExceeded,
			ThisEncoderDoesNotSupportCloning,
			UUEncoderFileNameTooLong,
			EncStrCannotCloneChildStream,
			MacBinIconOffsetTooLarge
		}
	}
}
