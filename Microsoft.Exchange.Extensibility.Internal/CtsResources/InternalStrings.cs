using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class InternalStrings
	{
		static InternalStrings()
		{
			InternalStrings.stringIDs.Add(431048246U, "SettingPositionDoesntMakeSense");
			InternalStrings.stringIDs.Add(4254659307U, "NonZeroPositionDoesntMakeSense");
			InternalStrings.stringIDs.Add(4279611434U, "CacheStreamCannotWrite");
			InternalStrings.stringIDs.Add(3780970154U, "FixedStreamCannotReadOrSeek");
			InternalStrings.stringIDs.Add(398319694U, "CacheStreamCannotReadOrSeek");
			InternalStrings.stringIDs.Add(1426955056U, "TooManyEntriesInApplefile");
			InternalStrings.stringIDs.Add(4142370976U, "WrongOffsetsInApplefile");
			InternalStrings.stringIDs.Add(882916622U, "FixedStreamCannotWrite");
			InternalStrings.stringIDs.Add(797792475U, "UnexpectedEndOfStream");
			InternalStrings.stringIDs.Add(4435227U, "CannotAccessClosedStream");
			InternalStrings.stringIDs.Add(426059850U, "MapEntryNoComma");
			InternalStrings.stringIDs.Add(1826387763U, "FixedStreamIsNull");
			InternalStrings.stringIDs.Add(158433569U, "LengthNotSupportedDuringReads");
			InternalStrings.stringIDs.Add(4281132505U, "WrongAppleMagicNumber");
			InternalStrings.stringIDs.Add(2260635443U, "NoStorageProperty");
			InternalStrings.stringIDs.Add(1686645306U, "WrongAppleVersionNumber");
			InternalStrings.stringIDs.Add(3682676966U, "MacBinWrongFilename");
			InternalStrings.stringIDs.Add(173345003U, "MergedLengthNotSupportedOnNonseekableCacheStream");
			InternalStrings.stringIDs.Add(2969036038U, "SeekingSupportedToBeginningOnly");
			InternalStrings.stringIDs.Add(2544842942U, "WrongMacBinHeader");
			InternalStrings.stringIDs.Add(182340944U, "ArgumentInvalidOffLen");
			InternalStrings.stringIDs.Add(2565209827U, "BadMapEntry");
			InternalStrings.stringIDs.Add(242522856U, "NoMapLoaded");
			InternalStrings.stringIDs.Add(3647337301U, "NoBackingStore");
			InternalStrings.stringIDs.Add(963287759U, "UnsupportedMapDataVersion");
		}

		public static string SettingPositionDoesntMakeSense
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("SettingPositionDoesntMakeSense");
			}
		}

		public static string NonZeroPositionDoesntMakeSense
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("NonZeroPositionDoesntMakeSense");
			}
		}

		public static string CacheStreamCannotWrite
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("CacheStreamCannotWrite");
			}
		}

		public static string FixedStreamCannotReadOrSeek
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("FixedStreamCannotReadOrSeek");
			}
		}

		public static string CacheStreamCannotReadOrSeek
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("CacheStreamCannotReadOrSeek");
			}
		}

		public static string TooManyEntriesInApplefile
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("TooManyEntriesInApplefile");
			}
		}

		public static string WrongOffsetsInApplefile
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("WrongOffsetsInApplefile");
			}
		}

		public static string FixedStreamCannotWrite
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("FixedStreamCannotWrite");
			}
		}

		public static string UnexpectedEndOfStream
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("UnexpectedEndOfStream");
			}
		}

		public static string CannotAccessClosedStream
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("CannotAccessClosedStream");
			}
		}

		public static string InvalidMimePart(string part)
		{
			return string.Format(InternalStrings.ResourceManager.GetString("InvalidMimePart"), part);
		}

		public static string MapEntryNoComma
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("MapEntryNoComma");
			}
		}

		public static string FixedStreamIsNull
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("FixedStreamIsNull");
			}
		}

		public static string LengthNotSupportedDuringReads
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("LengthNotSupportedDuringReads");
			}
		}

		public static string InvalidMimeMapEntry(string entry, string innerString)
		{
			return string.Format(InternalStrings.ResourceManager.GetString("InvalidMimeMapEntry"), entry, innerString);
		}

		public static string WrongAppleMagicNumber
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("WrongAppleMagicNumber");
			}
		}

		public static string NoStorageProperty
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("NoStorageProperty");
			}
		}

		public static string WrongAppleVersionNumber
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("WrongAppleVersionNumber");
			}
		}

		public static string MacBinWrongFilename
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("MacBinWrongFilename");
			}
		}

		public static string MergedLengthNotSupportedOnNonseekableCacheStream
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("MergedLengthNotSupportedOnNonseekableCacheStream");
			}
		}

		public static string CorruptMapData(int checkNumber)
		{
			return string.Format(InternalStrings.ResourceManager.GetString("CorruptMapData"), checkNumber);
		}

		public static string SeekingSupportedToBeginningOnly
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("SeekingSupportedToBeginningOnly");
			}
		}

		public static string WrongMacBinHeader
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("WrongMacBinHeader");
			}
		}

		public static string ArgumentInvalidOffLen
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("ArgumentInvalidOffLen");
			}
		}

		public static string BadMapEntry
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("BadMapEntry");
			}
		}

		public static string NoMapLoaded
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("NoMapLoaded");
			}
		}

		public static string NoBackingStore
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("NoBackingStore");
			}
		}

		public static string UnsupportedMapDataVersion
		{
			get
			{
				return InternalStrings.ResourceManager.GetString("UnsupportedMapDataVersion");
			}
		}

		public static string GetLocalizedString(InternalStrings.IDs key)
		{
			return InternalStrings.ResourceManager.GetString(InternalStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(25);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.InternalStrings", typeof(InternalStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			SettingPositionDoesntMakeSense = 431048246U,
			NonZeroPositionDoesntMakeSense = 4254659307U,
			CacheStreamCannotWrite = 4279611434U,
			FixedStreamCannotReadOrSeek = 3780970154U,
			CacheStreamCannotReadOrSeek = 398319694U,
			TooManyEntriesInApplefile = 1426955056U,
			WrongOffsetsInApplefile = 4142370976U,
			FixedStreamCannotWrite = 882916622U,
			UnexpectedEndOfStream = 797792475U,
			CannotAccessClosedStream = 4435227U,
			MapEntryNoComma = 426059850U,
			FixedStreamIsNull = 1826387763U,
			LengthNotSupportedDuringReads = 158433569U,
			WrongAppleMagicNumber = 4281132505U,
			NoStorageProperty = 2260635443U,
			WrongAppleVersionNumber = 1686645306U,
			MacBinWrongFilename = 3682676966U,
			MergedLengthNotSupportedOnNonseekableCacheStream = 173345003U,
			SeekingSupportedToBeginningOnly = 2969036038U,
			WrongMacBinHeader = 2544842942U,
			ArgumentInvalidOffLen = 182340944U,
			BadMapEntry = 2565209827U,
			NoMapLoaded = 242522856U,
			NoBackingStore = 3647337301U,
			UnsupportedMapDataVersion = 963287759U
		}

		private enum ParamIDs
		{
			InvalidMimePart,
			InvalidMimeMapEntry,
			CorruptMapData
		}
	}
}
