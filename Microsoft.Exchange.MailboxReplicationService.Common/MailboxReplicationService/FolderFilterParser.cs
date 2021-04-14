using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class FolderFilterParser
	{
		public static string GetAlias(WellKnownFolderType wkfType)
		{
			return '#' + wkfType.ToString() + '#';
		}

		public static void Parse(string folderPath, out WellKnownFolderType root, out List<string> folderNames, out FolderMappingFlags inheritanceFlags)
		{
			folderNames = new List<string>();
			root = WellKnownFolderType.IpmSubtree;
			inheritanceFlags = FolderMappingFlags.None;
			if (folderPath.EndsWith("/"))
			{
				folderPath = folderPath.Substring(0, folderPath.Length - "/".Length);
			}
			else if (folderPath.EndsWith("/*"))
			{
				folderPath = folderPath.Substring(0, folderPath.Length - "/*".Length);
				inheritanceFlags = FolderMappingFlags.Inherit;
			}
			bool flag = true;
			int i = 0;
			while (i < folderPath.Length)
			{
				string nextSubfolderName = FolderFilterParser.GetNextSubfolderName(folderPath, ref i);
				if (flag)
				{
					WellKnownFolderType wellKnownFolderType = WellKnownFolderType.None;
					if (folderPath[0] != '\\')
					{
						wellKnownFolderType = FolderFilterParser.CheckAlias(nextSubfolderName);
					}
					if (wellKnownFolderType != WellKnownFolderType.None)
					{
						root = wellKnownFolderType;
					}
					else
					{
						folderNames.Add(nextSubfolderName);
					}
					flag = false;
				}
				else
				{
					folderNames.Add(nextSubfolderName);
				}
			}
		}

		public static bool IsDumpster(WellKnownFolderType folderType)
		{
			return FolderFilterParser.dumpsterIds.Contains(folderType);
		}

		private static string GetNextSubfolderName(string folderPath, ref int curIndex)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			int num = curIndex;
			while (curIndex < folderPath.Length)
			{
				if (folderPath[curIndex] == '\\')
				{
					stringBuilder.Append(folderPath.Substring(num, curIndex - num));
					curIndex++;
					if (curIndex == folderPath.Length)
					{
						throw new FolderPathIsInvalidPermanentException(folderPath);
					}
					if (char.IsLetterOrDigit(folderPath[curIndex]))
					{
						throw new InvalidEscapedCharPermanentException(folderPath, curIndex);
					}
					num = curIndex;
				}
				else if (folderPath[curIndex] == '/')
				{
					break;
				}
				curIndex++;
			}
			stringBuilder.Append(folderPath.Substring(num, curIndex - num));
			if (curIndex < folderPath.Length)
			{
				curIndex++;
			}
			if (stringBuilder.Length == 0)
			{
				throw new FolderPathIsInvalidPermanentException(folderPath);
			}
			return stringBuilder.ToString();
		}

		private static WellKnownFolderType CheckAlias(string subfolderName)
		{
			if (subfolderName.Length < 2 || subfolderName[0] != '#' || subfolderName[subfolderName.Length - 1] != '#')
			{
				return WellKnownFolderType.None;
			}
			string text = subfolderName.Substring(1, subfolderName.Length - 2);
			WellKnownFolderType result;
			if (!Enum.TryParse<WellKnownFolderType>(text, true, out result))
			{
				throw new FolderAliasIsInvalidPermanentException(text);
			}
			return result;
		}

		public const char FolderAliasMarker = '#';

		public const char FolderSeparator = '/';

		public const char FolderEscapeChar = '\\';

		public const string ThisFolderOnlyInheritanceIndicator = "/";

		public const string FolderAndSubfoldersInheritanceIndicator = "/*";

		private static readonly HashSet<WellKnownFolderType> dumpsterIds = new HashSet<WellKnownFolderType>(new WellKnownFolderType[]
		{
			WellKnownFolderType.Dumpster,
			WellKnownFolderType.DumpsterDeletions,
			WellKnownFolderType.DumpsterVersions,
			WellKnownFolderType.DumpsterPurges,
			WellKnownFolderType.DumpsterAdminAuditLogs,
			WellKnownFolderType.DumpsterAudits,
			WellKnownFolderType.CalendarLogging
		});
	}
}
