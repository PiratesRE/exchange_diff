using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.Versioning
{
	[FriendAccessAllowed]
	internal static class BinaryCompatibility
	{
		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Phone_V7_1
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Phone_V7_1;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Phone_V8_0
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Phone_V8_0;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Desktop_V4_5
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Desktop_V4_5;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Desktop_V4_5_1
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Desktop_V4_5_1;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Desktop_V4_5_2
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Desktop_V4_5_2;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Desktop_V4_5_3
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Desktop_V4_5_3;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Desktop_V4_5_4
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Desktop_V4_5_4;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Desktop_V5_0
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Desktop_V5_0;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Silverlight_V4
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Silverlight_V4;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Silverlight_V5
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Silverlight_V5;
			}
		}

		[FriendAccessAllowed]
		internal static bool TargetsAtLeast_Silverlight_V6
		{
			[FriendAccessAllowed]
			get
			{
				return BinaryCompatibility.s_map.TargetsAtLeast_Silverlight_V6;
			}
		}

		[FriendAccessAllowed]
		internal static TargetFrameworkId AppWasBuiltForFramework
		{
			[FriendAccessAllowed]
			get
			{
				if (BinaryCompatibility.s_AppWasBuiltForFramework == TargetFrameworkId.NotYetChecked)
				{
					BinaryCompatibility.ReadTargetFrameworkId();
				}
				return BinaryCompatibility.s_AppWasBuiltForFramework;
			}
		}

		[FriendAccessAllowed]
		internal static int AppWasBuiltForVersion
		{
			[FriendAccessAllowed]
			get
			{
				if (BinaryCompatibility.s_AppWasBuiltForFramework == TargetFrameworkId.NotYetChecked)
				{
					BinaryCompatibility.ReadTargetFrameworkId();
				}
				return BinaryCompatibility.s_AppWasBuiltForVersion;
			}
		}

		private static bool ParseTargetFrameworkMonikerIntoEnum(string targetFrameworkMoniker, out TargetFrameworkId targetFramework, out int targetFrameworkVersion)
		{
			targetFramework = TargetFrameworkId.NotYetChecked;
			targetFrameworkVersion = 0;
			string a = null;
			string text = null;
			BinaryCompatibility.ParseFrameworkName(targetFrameworkMoniker, out a, out targetFrameworkVersion, out text);
			if (!(a == ".NETFramework"))
			{
				if (!(a == ".NETPortable"))
				{
					if (!(a == ".NETCore"))
					{
						if (!(a == "WindowsPhone"))
						{
							if (!(a == "WindowsPhoneApp"))
							{
								if (!(a == "Silverlight"))
								{
									targetFramework = TargetFrameworkId.Unrecognized;
								}
								else
								{
									targetFramework = TargetFrameworkId.Silverlight;
									if (!string.IsNullOrEmpty(text))
									{
										if (text == "WindowsPhone")
										{
											targetFramework = TargetFrameworkId.Phone;
											targetFrameworkVersion = 70000;
										}
										else if (text == "WindowsPhone71")
										{
											targetFramework = TargetFrameworkId.Phone;
											targetFrameworkVersion = 70100;
										}
										else if (text == "WindowsPhone8")
										{
											targetFramework = TargetFrameworkId.Phone;
											targetFrameworkVersion = 80000;
										}
										else if (text.StartsWith("WindowsPhone", StringComparison.Ordinal))
										{
											targetFramework = TargetFrameworkId.Unrecognized;
											targetFrameworkVersion = 70100;
										}
										else
										{
											targetFramework = TargetFrameworkId.Unrecognized;
										}
									}
								}
							}
							else
							{
								targetFramework = TargetFrameworkId.Phone;
							}
						}
						else if (targetFrameworkVersion >= 80100)
						{
							targetFramework = TargetFrameworkId.Phone;
						}
						else
						{
							targetFramework = TargetFrameworkId.Unspecified;
						}
					}
					else
					{
						targetFramework = TargetFrameworkId.NetCore;
					}
				}
				else
				{
					targetFramework = TargetFrameworkId.Portable;
				}
			}
			else
			{
				targetFramework = TargetFrameworkId.NetFramework;
			}
			return true;
		}

		private static void ParseFrameworkName(string frameworkName, out string identifier, out int version, out string profile)
		{
			if (frameworkName == null)
			{
				throw new ArgumentNullException("frameworkName");
			}
			if (frameworkName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_StringZeroLength"), "frameworkName");
			}
			string[] array = frameworkName.Split(new char[]
			{
				','
			});
			version = 0;
			if (array.Length < 2 || array.Length > 3)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_FrameworkNameTooShort"), "frameworkName");
			}
			identifier = array[0].Trim();
			if (identifier.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_FrameworkNameInvalid"), "frameworkName");
			}
			bool flag = false;
			profile = null;
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				});
				if (array2.Length != 2)
				{
					throw new ArgumentException(Environment.GetResourceString("SR.Argument_FrameworkNameInvalid"), "frameworkName");
				}
				string text = array2[0].Trim();
				string text2 = array2[1].Trim();
				if (text.Equals("Version", StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					if (text2.Length > 0 && (text2[0] == 'v' || text2[0] == 'V'))
					{
						text2 = text2.Substring(1);
					}
					Version version2 = new Version(text2);
					version = version2.Major * 10000;
					if (version2.Minor > 0)
					{
						version += version2.Minor * 100;
					}
					if (version2.Build > 0)
					{
						version += version2.Build;
					}
				}
				else
				{
					if (!text.Equals("Profile", StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_FrameworkNameInvalid"), "frameworkName");
					}
					if (!string.IsNullOrEmpty(text2))
					{
						profile = text2;
					}
				}
			}
			if (!flag)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_FrameworkNameMissingVersion"), "frameworkName");
			}
		}

		[SecuritySafeCritical]
		private static void ReadTargetFrameworkId()
		{
			string text = AppDomain.CurrentDomain.GetTargetFrameworkName();
			string valueInternal = CompatibilitySwitch.GetValueInternal("TargetFrameworkMoniker");
			if (!string.IsNullOrEmpty(valueInternal))
			{
				text = valueInternal;
			}
			int num = 0;
			TargetFrameworkId targetFrameworkId;
			if (text == null)
			{
				targetFrameworkId = TargetFrameworkId.Unspecified;
			}
			else if (!BinaryCompatibility.ParseTargetFrameworkMonikerIntoEnum(text, out targetFrameworkId, out num))
			{
				targetFrameworkId = TargetFrameworkId.Unrecognized;
			}
			BinaryCompatibility.s_AppWasBuiltForFramework = targetFrameworkId;
			BinaryCompatibility.s_AppWasBuiltForVersion = num;
		}

		private static TargetFrameworkId s_AppWasBuiltForFramework;

		private static int s_AppWasBuiltForVersion;

		private static readonly BinaryCompatibility.BinaryCompatibilityMap s_map = new BinaryCompatibility.BinaryCompatibilityMap();

		private const char c_componentSeparator = ',';

		private const char c_keyValueSeparator = '=';

		private const char c_versionValuePrefix = 'v';

		private const string c_versionKey = "Version";

		private const string c_profileKey = "Profile";

		private sealed class BinaryCompatibilityMap
		{
			internal BinaryCompatibilityMap()
			{
				this.AddQuirksForFramework(BinaryCompatibility.AppWasBuiltForFramework, BinaryCompatibility.AppWasBuiltForVersion);
			}

			private void AddQuirksForFramework(TargetFrameworkId builtAgainstFramework, int buildAgainstVersion)
			{
				switch (builtAgainstFramework)
				{
				case TargetFrameworkId.NotYetChecked:
				case TargetFrameworkId.Unrecognized:
				case TargetFrameworkId.Unspecified:
				case TargetFrameworkId.Portable:
					break;
				case TargetFrameworkId.NetFramework:
				case TargetFrameworkId.NetCore:
					if (buildAgainstVersion >= 50000)
					{
						this.TargetsAtLeast_Desktop_V5_0 = true;
					}
					if (buildAgainstVersion >= 40504)
					{
						this.TargetsAtLeast_Desktop_V4_5_4 = true;
					}
					if (buildAgainstVersion >= 40503)
					{
						this.TargetsAtLeast_Desktop_V4_5_3 = true;
					}
					if (buildAgainstVersion >= 40502)
					{
						this.TargetsAtLeast_Desktop_V4_5_2 = true;
					}
					if (buildAgainstVersion >= 40501)
					{
						this.TargetsAtLeast_Desktop_V4_5_1 = true;
					}
					if (buildAgainstVersion >= 40500)
					{
						this.TargetsAtLeast_Desktop_V4_5 = true;
						this.AddQuirksForFramework(TargetFrameworkId.Phone, 70100);
						this.AddQuirksForFramework(TargetFrameworkId.Silverlight, 50000);
						return;
					}
					break;
				case TargetFrameworkId.Silverlight:
					if (buildAgainstVersion >= 40000)
					{
						this.TargetsAtLeast_Silverlight_V4 = true;
					}
					if (buildAgainstVersion >= 50000)
					{
						this.TargetsAtLeast_Silverlight_V5 = true;
					}
					if (buildAgainstVersion >= 60000)
					{
						this.TargetsAtLeast_Silverlight_V6 = true;
					}
					break;
				case TargetFrameworkId.Phone:
					if (buildAgainstVersion >= 80000)
					{
						this.TargetsAtLeast_Phone_V8_0 = true;
					}
					if (buildAgainstVersion >= 80100)
					{
						this.TargetsAtLeast_Desktop_V4_5 = true;
						this.TargetsAtLeast_Desktop_V4_5_1 = true;
					}
					if (buildAgainstVersion >= 710)
					{
						this.TargetsAtLeast_Phone_V7_1 = true;
						return;
					}
					break;
				default:
					return;
				}
			}

			internal bool TargetsAtLeast_Phone_V7_1;

			internal bool TargetsAtLeast_Phone_V8_0;

			internal bool TargetsAtLeast_Phone_V8_1;

			internal bool TargetsAtLeast_Desktop_V4_5;

			internal bool TargetsAtLeast_Desktop_V4_5_1;

			internal bool TargetsAtLeast_Desktop_V4_5_2;

			internal bool TargetsAtLeast_Desktop_V4_5_3;

			internal bool TargetsAtLeast_Desktop_V4_5_4;

			internal bool TargetsAtLeast_Desktop_V5_0;

			internal bool TargetsAtLeast_Silverlight_V4;

			internal bool TargetsAtLeast_Silverlight_V5;

			internal bool TargetsAtLeast_Silverlight_V6;
		}
	}
}
