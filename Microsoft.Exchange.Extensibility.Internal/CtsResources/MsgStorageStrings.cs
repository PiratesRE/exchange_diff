using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class MsgStorageStrings
	{
		static MsgStorageStrings()
		{
			MsgStorageStrings.stringIDs.Add(3471497494U, "CallReadNextProperty");
			MsgStorageStrings.stringIDs.Add(2188699373U, "CorruptData");
			MsgStorageStrings.stringIDs.Add(3520643888U, "RecipientPropertyTooLong");
			MsgStorageStrings.stringIDs.Add(3735744638U, "PropertyValueTruncated");
			MsgStorageStrings.stringIDs.Add(539623025U, "RecipientPropertiesNotStreamable");
			MsgStorageStrings.stringIDs.Add(3819751606U, "NotANamedProperty");
			MsgStorageStrings.stringIDs.Add(819337844U, "NotAMessageAttachment");
			MsgStorageStrings.stringIDs.Add(3801565302U, "ComExceptionThrown");
			MsgStorageStrings.stringIDs.Add(2099708518U, "PropertyLongValue");
			MsgStorageStrings.stringIDs.Add(3821818057U, "NotAnOleAttachment");
			MsgStorageStrings.stringIDs.Add(566373508U, "NonStreamableProperty");
			MsgStorageStrings.stringIDs.Add(188730555U, "LargeNamedPropertyList");
			MsgStorageStrings.stringIDs.Add(3430902968U, "AllPropertiesRead");
			MsgStorageStrings.stringIDs.Add(3394043311U, "NotFound");
		}

		public static string FailedCreateStorage(string filename)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("FailedCreateStorage"), filename);
		}

		public static string UnsupportedPropertyType(string type)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("UnsupportedPropertyType"), type);
		}

		public static string FailedWrite(string streamName)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("FailedWrite"), streamName);
		}

		public static string CallReadNextProperty
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("CallReadNextProperty");
			}
		}

		public static string CorruptData
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("CorruptData");
			}
		}

		public static string StreamNotSeakable(string className)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("StreamNotSeakable"), className);
		}

		public static string RecipientPropertyTooLong
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("RecipientPropertyTooLong");
			}
		}

		public static string PropertyValueTruncated
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("PropertyValueTruncated");
			}
		}

		public static string RecipientPropertiesNotStreamable
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("RecipientPropertiesNotStreamable");
			}
		}

		public static string NotANamedProperty
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("NotANamedProperty");
			}
		}

		public static string StreamTooBig(string streamName, long streamLength)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("StreamTooBig"), streamName, streamLength);
		}

		public static string PropertyNotFound(int tag)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("PropertyNotFound"), tag);
		}

		public static string NotAMessageAttachment
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("NotAMessageAttachment");
			}
		}

		public static string ComExceptionThrown
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("ComExceptionThrown");
			}
		}

		public static string PropertyLongValue
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("PropertyLongValue");
			}
		}

		public static string FailedRead(string streamName)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("FailedRead"), streamName);
		}

		public static string FailedOpenStorage(string filename)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("FailedOpenStorage"), filename);
		}

		public static string NotAnOleAttachment
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("NotAnOleAttachment");
			}
		}

		public static string InvalidValueType(Type expected, Type actual)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("InvalidValueType"), expected, actual);
		}

		public static string InvalidPropertyTag(int tag)
		{
			return string.Format(MsgStorageStrings.ResourceManager.GetString("InvalidPropertyTag"), tag);
		}

		public static string NonStreamableProperty
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("NonStreamableProperty");
			}
		}

		public static string LargeNamedPropertyList
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("LargeNamedPropertyList");
			}
		}

		public static string AllPropertiesRead
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("AllPropertiesRead");
			}
		}

		public static string NotFound
		{
			get
			{
				return MsgStorageStrings.ResourceManager.GetString("NotFound");
			}
		}

		public static string GetLocalizedString(MsgStorageStrings.IDs key)
		{
			return MsgStorageStrings.ResourceManager.GetString(MsgStorageStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(14);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.MsgStorageStrings", typeof(MsgStorageStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			CallReadNextProperty = 3471497494U,
			CorruptData = 2188699373U,
			RecipientPropertyTooLong = 3520643888U,
			PropertyValueTruncated = 3735744638U,
			RecipientPropertiesNotStreamable = 539623025U,
			NotANamedProperty = 3819751606U,
			NotAMessageAttachment = 819337844U,
			ComExceptionThrown = 3801565302U,
			PropertyLongValue = 2099708518U,
			NotAnOleAttachment = 3821818057U,
			NonStreamableProperty = 566373508U,
			LargeNamedPropertyList = 188730555U,
			AllPropertiesRead = 3430902968U,
			NotFound = 3394043311U
		}

		private enum ParamIDs
		{
			FailedCreateStorage,
			UnsupportedPropertyType,
			FailedWrite,
			StreamNotSeakable,
			StreamTooBig,
			PropertyNotFound,
			FailedRead,
			FailedOpenStorage,
			InvalidValueType,
			InvalidPropertyTag
		}
	}
}
