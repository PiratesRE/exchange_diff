using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class WebServiceRunspaceConfiguration : ExchangeRunspaceConfiguration
	{
		public WebServiceRunspaceConfiguration(IIdentity identity) : base(identity, ExchangeRunspaceConfigurationSettings.GetDefaultInstance())
		{
		}

		public virtual bool IsWebMethodAvailable(string webMethodName)
		{
			if (string.IsNullOrEmpty(webMethodName))
			{
				throw new ArgumentNullException("webMethodName");
			}
			RoleEntryInfo roleInfoForWebMethod = RoleEntryInfo.GetRoleInfoForWebMethod(webMethodName);
			int num = this.allRoleEntries.BinarySearch(roleInfoForWebMethod, RoleEntryInfo.NameComparer);
			if (num < 0)
			{
				ExTraceGlobals.AccessCheckTracer.TraceWarning<string, string>((long)this.GetHashCode(), "IsWebMethodAvailable() returns false for user {0} because web method {1} is not in role.", this.identityName, webMethodName);
				return false;
			}
			return true;
		}

		public virtual bool IsWebMethodInRole(string webMethodName, RoleType roleType)
		{
			if (string.IsNullOrEmpty(webMethodName))
			{
				throw new ArgumentNullException("webMethodName");
			}
			List<ADObjectId> rolesFromRoleType = this.GetRolesFromRoleType(roleType);
			if (rolesFromRoleType == null)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, RoleType>((long)this.GetHashCode(), "IsWebMethodInRole() returns false because identity {0} doesn't have role {1}", this.identityName, roleType);
				return false;
			}
			List<RoleEntryInfo> list = this.allRoleEntries.FindAll((RoleEntryInfo x) => x.RoleEntry.Name.Equals(webMethodName, StringComparison.OrdinalIgnoreCase));
			if (list.Count == 0)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, RoleType, string>((long)this.GetHashCode(), "IsWebMethodInRole() returns false because identity {0}'s role {1} doesn't include web method {2}.", this.identityName, roleType, webMethodName);
				return false;
			}
			using (List<ADObjectId>.Enumerator enumerator = rolesFromRoleType.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ADObjectId roleId = enumerator.Current;
					IEnumerable<ExchangeRoleAssignment> enumerable = from x in this.allRoleAssignments
					where x.Role.Equals(roleId)
					select x;
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in enumerable)
					{
						if (list.Exists((RoleEntryInfo x) => x.RoleAssignment.Role.Equals(roleId)))
						{
							return true;
						}
					}
				}
			}
			ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, RoleType, string>((long)this.GetHashCode(), "IsWebMethodInRole() returns false because identity {0}'s role {1}'s role assigments are not associated with the roles that include web method {2}.", this.identityName, roleType, webMethodName);
			return false;
		}

		public virtual List<ADObjectId> GetRolesFromRoleType(RoleType roleType)
		{
			List<ADObjectId> result = null;
			this.allRoleTypes.TryGetValue(roleType, out result);
			return result;
		}

		public virtual bool IsTargetObjectInRoleScope(RoleType roleType, ADRecipient targetRecipient)
		{
			if (targetRecipient == null)
			{
				throw new ArgumentNullException("targetRecipient");
			}
			List<ADObjectId> rolesFromRoleType = this.GetRolesFromRoleType(roleType);
			if (rolesFromRoleType == null)
			{
				ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, RoleType>((long)this.GetHashCode(), "IsTargetObjectInRoleScope() returns false because identity {0} doesn't have role {1}", this.identityName, roleType);
				return false;
			}
			using (List<ADObjectId>.Enumerator enumerator = rolesFromRoleType.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ADObjectId roleId = enumerator.Current;
					IEnumerable<ExchangeRoleAssignment> enumerable = from x in this.allRoleAssignments
					where x.Role.Equals(roleId)
					select x;
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in enumerable)
					{
						RoleAssignmentScopeSet effectiveScopeSet = exchangeRoleAssignment.GetEffectiveScopeSet(this.allScopes, base.UserAccessToken);
						OrganizationId organizationId = targetRecipient.OrganizationId;
						effectiveScopeSet.RecipientReadScope.PopulateRootAndFilter(organizationId, targetRecipient);
						effectiveScopeSet.RecipientWriteScope.PopulateRootAndFilter(organizationId, targetRecipient);
						ADScopeException ex;
						if (ADSession.TryVerifyIsWithinScopes(targetRecipient, effectiveScopeSet.RecipientReadScope, new ADScopeCollection[]
						{
							new ADScopeCollection(new ADScope[]
							{
								effectiveScopeSet.RecipientWriteScope
							})
						}, this.exclusiveRecipientScopesCollection, false, out ex))
						{
							return true;
						}
					}
				}
			}
			ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, RoleType, ObjectId>((long)this.GetHashCode(), "IsTargetObjectInRoleScope() returns false because identity {0}'s roles of type {1} don't have the scope that covers target object {2}.", this.identityName, roleType, targetRecipient.Identity);
			return false;
		}

		public RoleType[] ResolveRoleTypeByMethod(string webMethodName)
		{
			if (string.IsNullOrEmpty(webMethodName))
			{
				throw new ArgumentNullException("webMethodName");
			}
			return (from r in this.allRoleTypes.Keys
			where this.IsWebMethodInRole(webMethodName, r)
			select r).ToArray<RoleType>();
		}
	}
}
