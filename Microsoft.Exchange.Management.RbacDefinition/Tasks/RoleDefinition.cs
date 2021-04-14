using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal struct RoleDefinition
	{
		public static string GetDCSafeNameForRole(string roleName)
		{
			string text = string.Format("DCSafe {0}", roleName);
			if (text.Length > 64)
			{
				text = text.Substring(0, 64);
			}
			return text.Trim();
		}

		public RoleDefinition(string theRoleName, RoleType theRoleType, RoleCmdlet[] theCmdlets)
		{
			this.roleName = theRoleName;
			this.roleType = theRoleType;
			this.cmdlets = theCmdlets;
			this.parentRoleName = null;
		}

		public RoleDefinition(string theRoleName, string theParentRoleName, RoleType theRoleType, RoleCmdlet[] theCmdlets)
		{
			this = new RoleDefinition(theRoleName, theRoleType, theCmdlets);
			this.parentRoleName = theParentRoleName;
		}

		public string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		public string ParentRoleName
		{
			get
			{
				return this.parentRoleName;
			}
		}

		internal RoleType RoleType
		{
			get
			{
				return this.roleType;
			}
		}

		internal bool IsEndUserRole
		{
			get
			{
				return Array.BinarySearch<RoleType>(ExchangeRole.EndUserRoleTypes, this.RoleType) >= 0;
			}
		}

		internal RoleCmdlet[] Cmdlets
		{
			get
			{
				return this.cmdlets;
			}
		}

		public ExchangeRole GenerateRole(List<string> enabledFeatures, ADObjectId rolesContainerId, string suffix, string resolutionType)
		{
			ExchangeRole exchangeRole = new ExchangeRole();
			string input = null;
			exchangeRole.SetId(rolesContainerId.GetChildId(this.RoleName + suffix));
			exchangeRole.RoleType = this.roleType;
			exchangeRole.MailboxPlanIndex = resolutionType;
			exchangeRole.StampImplicitScopes();
			exchangeRole.StampIsEndUserRole();
			foreach (RoleCmdlet roleCmdlet in this.cmdlets)
			{
				if (roleCmdlet.TryGenerateRoleEntry(enabledFeatures, ref input))
				{
					exchangeRole.RoleEntries.Add(RoleEntry.Parse(input));
				}
			}
			return exchangeRole;
		}

		public List<string> GetRequiredFeaturesForRoleEntries()
		{
			List<string> list = new List<string>();
			foreach (RoleCmdlet roleCmdlet in this.cmdlets)
			{
				list.AddRange(roleCmdlet.GetRequiredFeaturesForAllParameters());
			}
			return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList<string>();
		}

		public bool ContainsProhibitedActions(List<string> prohibitedActions)
		{
			List<string> requiredFeaturesForRoleEntries = this.GetRequiredFeaturesForRoleEntries();
			return requiredFeaturesForRoleEntries.FirstOrDefault((string x) => prohibitedActions.Contains(x, StringComparer.OrdinalIgnoreCase)) != null;
		}

		public List<RoleEntry> GetRoleEntriesFilteringProhibitedActions(List<string> features, List<string> prohibitedActions)
		{
			if (!this.ContainsProhibitedActions(prohibitedActions))
			{
				throw new InvalidOperationException(string.Format(" Role '{0}' doesn't have any prohibited action.", this.RoleName));
			}
			string input = null;
			List<RoleEntry> list = new List<RoleEntry>(this.cmdlets.Length);
			foreach (RoleCmdlet roleCmdlet in this.cmdlets)
			{
				if (roleCmdlet.TryGenerateRoleEntryFilteringProhibitedActions(features, prohibitedActions, ref input))
				{
					list.Add(RoleEntry.Parse(input));
				}
			}
			return list;
		}

		private const string SafeRolePrefixFormat = "DCSafe {0}";

		private RoleType roleType;

		private RoleCmdlet[] cmdlets;

		private string roleName;

		private string parentRoleName;
	}
}
