using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserConfigurationName
	{
		public UserConfigurationName(string name, ConfigurationNameKind kind)
		{
			EnumValidator.ThrowIfInvalid<ConfigurationNameKind>(kind, "kind");
			this.kind = kind;
			this.name = UserConfigurationName.CheckConfigurationName(name, kind);
			switch (kind)
			{
			case ConfigurationNameKind.Name:
			case ConfigurationNameKind.PartialName:
				this.fullName = "IPM.Configuration." + this.name;
				return;
			case ConfigurationNameKind.ItemClass:
				this.fullName = this.name;
				return;
			default:
				throw new InvalidOperationException(string.Format("Unrecognized kind. Kind = {0}.", this.kind));
			}
		}

		internal static bool IsValidName(string name, ConfigurationNameKind kind)
		{
			string text;
			return UserConfigurationName.TryCheckConfigurationName(name, kind, out text);
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		public string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		internal ConfigurationNameKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		internal static IComparer GetCustomComparer(UserConfigurationSearchFlags searchFlags)
		{
			return new UserConfigurationName.CustomConfigNameComparer(searchFlags);
		}

		public override string ToString()
		{
			return string.Format("<{0}, Kind = {1}>", this.name, this.kind);
		}

		private static string CheckConfigurationName(string name, ConfigurationNameKind kind)
		{
			Util.ThrowOnNullArgument(name, "name");
			string result;
			if (!UserConfigurationName.TryCheckConfigurationName(name, kind, out result))
			{
				throw new ArgumentException(ServerStrings.ExConfigNameInvalid(name));
			}
			return result;
		}

		private static bool TryCheckConfigurationName(string name, ConfigurationNameKind kind, out string validatedName)
		{
			validatedName = null;
			int num = name.IndexOf("IPM.Configuration.");
			switch (kind)
			{
			case ConfigurationNameKind.Name:
			case ConfigurationNameKind.PartialName:
				if (num >= 0)
				{
					ExTraceGlobals.StorageTracer.TraceError<string>(0L, "UserConfigurationName::TryCheckConfigurationName. Invalid config name contains well known prefix. Name = {0}", name);
					return false;
				}
				break;
			case ConfigurationNameKind.ItemClass:
				if (num < 0)
				{
					ExTraceGlobals.StorageTracer.TraceError<string>(0L, "UserConfigurationName::TryCheckConfigurationName. Invalid full name. Name = {0}", name);
					return false;
				}
				name = name.Substring("IPM.Configuration.".Length);
				break;
			}
			if (name.Length == 0 && kind != ConfigurationNameKind.PartialName)
			{
				ExTraceGlobals.StorageTracer.TraceError<string, ConfigurationNameKind>(0L, "UserConfigurationName::TryCheckConfigurationName. ConfigName is empty. Name = {0}. Kind = {1}", name, kind);
				return false;
			}
			if (name.Length > UserConfigurationName.MaxUserConfigurationNameLength)
			{
				ExTraceGlobals.StorageTracer.TraceError<string, int, int>(0L, "UserConfigurationName::TryCheckConfigurationName. Name = {0}, Length = {1}, MaxLength = {2}.", name, name.Length, UserConfigurationName.MaxUserConfigurationNameLength);
				return false;
			}
			if (!UserConfigurationName.TryValidateConfigName(name))
			{
				return false;
			}
			validatedName = name;
			return true;
		}

		private static bool TryValidateConfigName(string configurationName)
		{
			char c = '.';
			foreach (char c2 in configurationName)
			{
				if ((c2 < 'a' || c2 > 'z') && (c2 < 'A' || c2 > 'Z') && (c2 < '0' || c2 > '9') && c2 != c)
				{
					ExTraceGlobals.StorageTracer.TraceError<char>(0L, "UserConfigurationName::CheckConfigurationName. The configuration name contains invalid character. Char = {0}.", c2);
					return false;
				}
			}
			return true;
		}

		public const string IPMConfiguration = "IPM.Configuration.";

		private readonly string name;

		private readonly string fullName;

		private readonly ConfigurationNameKind kind;

		private static readonly int IPMConfigurationLength = "IPM.Configuration.".Length;

		private static readonly int MaxUserConfigurationNameLength = 255 - UserConfigurationName.IPMConfigurationLength;

		private class CustomConfigNameComparer : IComparer
		{
			internal CustomConfigNameComparer(UserConfigurationSearchFlags searchFlags)
			{
				this.searchFlags = searchFlags;
			}

			public int Compare(object x, object y)
			{
				string text = x as string;
				UserConfigurationName userConfigurationName = y as UserConfigurationName;
				if (text == null || userConfigurationName == null)
				{
					return -1;
				}
				switch (this.searchFlags)
				{
				case UserConfigurationSearchFlags.FullString:
					if (text == userConfigurationName.FullName)
					{
						return 0;
					}
					break;
				case UserConfigurationSearchFlags.SubString:
				{
					int num = text.IndexOf("IPM.Configuration.");
					int num2 = text.IndexOf(userConfigurationName.Name);
					if (num == 0 && num2 >= "IPM.Configuration.".Length)
					{
						return 0;
					}
					break;
				}
				case UserConfigurationSearchFlags.Prefix:
					if (text.IndexOf(userConfigurationName.FullName) == 0)
					{
						return 0;
					}
					break;
				}
				return -1;
			}

			private readonly UserConfigurationSearchFlags searchFlags;
		}
	}
}
