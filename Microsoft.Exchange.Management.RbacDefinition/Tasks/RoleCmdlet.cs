using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleCmdlet
	{
		public RoleCmdlet(string snapin, string theCmdlet, RoleParameters[] cmdletParameters, string theCmdletType)
		{
			this.snapin = snapin;
			this.cmdlet = theCmdlet;
			this.parameters = cmdletParameters;
			this.cmdletType = theCmdletType;
		}

		private string GetFormattedCmdletType()
		{
			string a;
			if ((a = this.cmdletType) != null)
			{
				if (a == "c")
				{
					return string.Format("{0},{1},{2}", this.cmdletType, this.cmdlet, this.snapin);
				}
				if (a == "a" || a == "s" || a == "w")
				{
					return string.Format("{0},{1}", this.cmdletType, this.cmdlet);
				}
			}
			throw new ArgumentOutOfRangeException("cmdletType");
		}

		public bool TryGenerateRoleEntry(List<string> enabledFeatures, ref string roleEntry)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			bool flag = false;
			stringBuilder.Append(this.GetFormattedCmdletType());
			foreach (RoleParameters roleParameters in this.parameters)
			{
				flag |= roleParameters.TryAddToRoleEntry(enabledFeatures, stringBuilder);
			}
			if (this.cmdletType == "w")
			{
				flag = true;
			}
			if (flag)
			{
				roleEntry = stringBuilder.ToString();
			}
			return flag;
		}

		public bool TryGenerateRoleEntryFilteringProhibitedActions(List<string> enabledFeatures, List<string> prohibitedActions, ref string roleEntry)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			bool flag = false;
			stringBuilder.Append(this.GetFormattedCmdletType());
			foreach (RoleParameters roleParameters in this.parameters)
			{
				flag |= roleParameters.TryAddToRoleEntryFilteringProhibitedActions(enabledFeatures, prohibitedActions, stringBuilder);
			}
			if (flag)
			{
				roleEntry = stringBuilder.ToString();
			}
			return flag;
		}

		public List<string> GetRequiredFeaturesForAllParameters()
		{
			List<string> list = new List<string>();
			foreach (RoleParameters roleParameters in this.parameters)
			{
				if (roleParameters.RequiredFeatures != null && roleParameters.RequiredFeatures.Length > 0)
				{
					list.AddRange(roleParameters.RequiredFeatures);
				}
			}
			return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
		}

		private RoleParameters[] parameters;

		private readonly string snapin;

		private readonly string cmdlet;

		private readonly string cmdletType;
	}
}
