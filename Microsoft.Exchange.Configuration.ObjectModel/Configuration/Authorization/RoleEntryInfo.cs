using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RoleEntryInfo
	{
		internal RoleEntryInfo(RoleEntry roleEntry, ExchangeRoleAssignment roleAssignment)
		{
			this.RoleEntry = roleEntry;
			this.RoleAssignment = roleAssignment;
			this.ScopeSet = null;
		}

		internal RoleEntryInfo(RoleEntry roleEntry)
		{
			this.RoleEntry = roleEntry;
			this.RoleAssignment = null;
			this.ScopeSet = null;
		}

		internal static IComparer<RoleEntryInfo> NameComparer
		{
			get
			{
				return RoleEntryInfo.nameRoleEntryInfoComparer;
			}
		}

		internal static IComparer<RoleEntryInfo> NameAndInstanceHashCodeComparer
		{
			get
			{
				return RoleEntryInfo.nameAndInstanceHashCodeRoleEntryInfoComparer;
			}
		}

		internal static RoleEntryInfo GetRoleInfoForCmdlet(string cmdletFullName)
		{
			RoleEntry roleEntry;
			if (cmdletFullName.Equals("Impersonate-ExchangeUser", StringComparison.OrdinalIgnoreCase))
			{
				roleEntry = new ApplicationPermissionRoleEntry("a," + cmdletFullName);
			}
			else
			{
				string str = RoleEntryInfo.ConvertToCommaSeparatedCmdletName(cmdletFullName);
				roleEntry = new CmdletRoleEntry("c," + str);
			}
			return new RoleEntryInfo(roleEntry);
		}

		internal static RoleEntryInfo GetRoleInfoForScript(string scriptName)
		{
			return new RoleEntryInfo(new ScriptRoleEntry("s," + scriptName));
		}

		internal static RoleEntryInfo GetRoleInfoForWebMethod(string webMethodName)
		{
			return new RoleEntryInfo(new WebServiceRoleEntry("w," + webMethodName));
		}

		internal static bool IsExchangeCmdlet(CmdletRoleEntry cmdletRoleEntry)
		{
			foreach (string value in ExchangeRunspaceConfiguration.ExchangeSnapins)
			{
				if (cmdletRoleEntry.PSSnapinName.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static string ConvertToCommaSeparatedCmdletName(string cmdletFullName)
		{
			int num = cmdletFullName.IndexOf('\\');
			string result;
			if (-1 == num)
			{
				result = cmdletFullName;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(cmdletFullName.Length);
				stringBuilder.Append(cmdletFullName, 1 + num, cmdletFullName.Length - num - 1);
				stringBuilder.Append(',');
				stringBuilder.Append(cmdletFullName, 0, num);
				result = stringBuilder.ToString();
			}
			return result;
		}

		internal readonly RoleEntry RoleEntry;

		internal readonly ExchangeRoleAssignment RoleAssignment;

		internal RoleAssignmentScopeSet ScopeSet;

		private static RoleEntryInfo.NameRoleEntryInfoComparer nameRoleEntryInfoComparer = new RoleEntryInfo.NameRoleEntryInfoComparer();

		private static RoleEntryInfo.NameAndInstanceHashCodeRoleEntryInfoComparer nameAndInstanceHashCodeRoleEntryInfoComparer = new RoleEntryInfo.NameAndInstanceHashCodeRoleEntryInfoComparer();

		private class NameRoleEntryInfoComparer : IComparer<RoleEntryInfo>
		{
			public int Compare(RoleEntryInfo a, RoleEntryInfo b)
			{
				return RoleEntry.CompareRoleEntriesByName(a.RoleEntry, b.RoleEntry);
			}
		}

		private class NameAndInstanceHashCodeRoleEntryInfoComparer : IComparer<RoleEntryInfo>
		{
			public int Compare(RoleEntryInfo a, RoleEntryInfo b)
			{
				return RoleEntry.CompareRoleEntriesByNameAndInstanceHashCode(a.RoleEntry, b.RoleEntry);
			}
		}
	}
}
