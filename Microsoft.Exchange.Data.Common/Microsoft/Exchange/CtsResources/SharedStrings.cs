using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class SharedStrings
	{
		static SharedStrings()
		{
			SharedStrings.stringIDs.Add(3996289637U, "InvalidFactory");
			SharedStrings.stringIDs.Add(1551326176U, "CannotSetNegativelength");
			SharedStrings.stringIDs.Add(1590522975U, "CountOutOfRange");
			SharedStrings.stringIDs.Add(2864662625U, "CannotSeekBeforeBeginning");
			SharedStrings.stringIDs.Add(2489963781U, "StringArgumentMustBeUTF8");
			SharedStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
			SharedStrings.stringIDs.Add(2746482960U, "CountTooLarge");
			SharedStrings.stringIDs.Add(431486251U, "StringArgumentMustBeAscii");
		}

		public static string InvalidFactory
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("InvalidFactory");
			}
		}

		public static string CannotSetNegativelength
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("CannotSetNegativelength");
			}
		}

		public static string CreateFileFailed(string filePath)
		{
			return string.Format(SharedStrings.ResourceManager.GetString("CreateFileFailed"), filePath);
		}

		public static string CountOutOfRange
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("CountOutOfRange");
			}
		}

		public static string CannotSeekBeforeBeginning
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("CannotSeekBeforeBeginning");
			}
		}

		public static string StringArgumentMustBeUTF8
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("StringArgumentMustBeUTF8");
			}
		}

		public static string OffsetOutOfRange
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("OffsetOutOfRange");
			}
		}

		public static string CountTooLarge
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("CountTooLarge");
			}
		}

		public static string StringArgumentMustBeAscii
		{
			get
			{
				return SharedStrings.ResourceManager.GetString("StringArgumentMustBeAscii");
			}
		}

		public static string GetLocalizedString(SharedStrings.IDs key)
		{
			return SharedStrings.ResourceManager.GetString(SharedStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(8);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.SharedStrings", typeof(SharedStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidFactory = 3996289637U,
			CannotSetNegativelength = 1551326176U,
			CountOutOfRange = 1590522975U,
			CannotSeekBeforeBeginning = 2864662625U,
			StringArgumentMustBeUTF8 = 2489963781U,
			OffsetOutOfRange = 3590683541U,
			CountTooLarge = 2746482960U,
			StringArgumentMustBeAscii = 431486251U
		}

		private enum ParamIDs
		{
			CreateFileFailed
		}
	}
}
