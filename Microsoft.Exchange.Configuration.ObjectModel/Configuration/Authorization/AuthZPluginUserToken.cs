using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class AuthZPluginUserToken
	{
		internal AuthZPluginUserToken(DelegatedPrincipal delegatedPrincipal, ADRawEntry userEntry, Microsoft.Exchange.Configuration.Core.AuthenticationType authenticatedType, string defaultUserName)
		{
			this.DelegatedPrincipal = delegatedPrincipal;
			this.UserEntry = userEntry;
			this.AuthenticationType = authenticatedType;
			this.DefaultUserName = defaultUserName;
		}

		internal DelegatedPrincipal DelegatedPrincipal { get; private set; }

		internal bool IsDelegatedUser
		{
			get
			{
				return this.DelegatedPrincipal != null;
			}
		}

		internal virtual ADRawEntry UserEntry { get; private set; }

		internal string DefaultUserName { get; private set; }

		internal Microsoft.Exchange.Configuration.Core.AuthenticationType AuthenticationType { get; private set; }

		internal virtual string UserName
		{
			get
			{
				if (this.userName == null)
				{
					this.userName = this.DefaultUserName;
					if (this.DelegatedPrincipal != null)
					{
						this.userName = this.DelegatedPrincipal.GetUserName();
						ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Generate username {0} for AuthZPluginUserToken using DelegatedPrincipal.", this.userName);
					}
					else
					{
						object obj = this.UserEntry[ADObjectSchema.Id];
						if (obj != null)
						{
							this.userName = obj.ToString();
							ADObjectId adobjectId = obj as ADObjectId;
							if (adobjectId != null)
							{
								this.userNameForLogging = adobjectId.Name;
							}
							ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Generate username {0} for AuthZPluginUserToken using UserEntry.", this.userName);
						}
					}
				}
				return this.userName;
			}
		}

		internal virtual string WindowsLiveId
		{
			get
			{
				if (!this.windowsLiveIdCalculated)
				{
					if (this.DelegatedPrincipal != null)
					{
						string userId = this.DelegatedPrincipal.UserId;
						if (SmtpAddress.IsValidSmtpAddress(userId))
						{
							this.windowsLiveId = userId;
						}
					}
					else if (this.UserEntry != null)
					{
						object obj = this.UserEntry[ADRecipientSchema.WindowsLiveID];
						this.windowsLiveId = ((obj == null) ? null : obj.ToString());
					}
					this.windowsLiveIdCalculated = true;
				}
				return this.windowsLiveId;
			}
		}

		internal virtual string OrgIdInString
		{
			get
			{
				if (!this.orgIdCalculated)
				{
					this.orgIdCalculated = true;
					this.CalculateOrgId();
				}
				if (!(this.orgId == null) && this.orgId.ConfigurationUnit != null)
				{
					return this.orgId.ConfigurationUnit.ToString();
				}
				return null;
			}
		}

		internal virtual OrganizationId OrgId
		{
			get
			{
				if (!this.orgIdCalculated)
				{
					this.orgIdCalculated = true;
					this.CalculateOrgId();
				}
				return this.orgId;
			}
		}

		internal string UserNameForLogging
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(this.WindowsLiveId))
				{
					return this.WindowsLiveId;
				}
				if (!string.IsNullOrWhiteSpace(this.userNameForLogging))
				{
					return this.userNameForLogging;
				}
				if (string.IsNullOrWhiteSpace(this.UserName))
				{
					return this.DefaultUserName;
				}
				if (!string.IsNullOrWhiteSpace(this.userNameForLogging))
				{
					return this.userNameForLogging;
				}
				return this.UserName;
			}
		}

		internal virtual IList<string> DomainsToBlockTogether
		{
			get
			{
				if (this.domainsToBlockTogether == null)
				{
					OrganizationId organizationId = this.OrgId;
					IEnumerable<SmtpDomainWithSubdomains> enumerable = null;
					if (organizationId != null && organizationId.ConfigurationUnit != null)
					{
						enumerable = AuthZPluginHelper.GetAcceptedDomains(this.OrgId, this.OrgId);
					}
					this.domainsToBlockTogether = new List<string>();
					if (enumerable != null)
					{
						foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in enumerable)
						{
							if (!smtpDomainWithSubdomains.IsStar && !smtpDomainWithSubdomains.IncludeSubDomains)
							{
								this.domainsToBlockTogether.Add(smtpDomainWithSubdomains.Domain);
							}
						}
					}
				}
				return this.domainsToBlockTogether;
			}
		}

		internal IThrottlingPolicy GetThrottlingPolicy()
		{
			using (IPowerShellBudget powerShellBudget = this.CreateBudget(BudgetType.PowerShell))
			{
				if (powerShellBudget != null)
				{
					return powerShellBudget.ThrottlingPolicy;
				}
			}
			if (this.UserEntry is MiniRecipient)
			{
				return (this.UserEntry as MiniRecipient).ReadThrottlingPolicy();
			}
			return (this.UserEntry as ADUser).ReadThrottlingPolicy();
		}

		internal virtual IPowerShellBudget CreateBudget(BudgetType budgetType)
		{
			IPowerShellBudget result = null;
			if (this.DelegatedPrincipal != null)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<BudgetType, string>(0L, "Create Budge {0} for AuthZPluginUserToken {1} using DelegatedPrincipal.", budgetType, this.UserName);
				result = PowerShellBudget.Acquire(new DelegatedPrincipalBudgetKey(this.DelegatedPrincipal, budgetType));
			}
			else
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<BudgetType, string>(0L, "Create Budge {0} for AuthZPluginUserToken {1} using UserEntry.", budgetType, this.UserName);
				if (budgetType == BudgetType.WSManTenant)
				{
					return PowerShellBudget.Acquire(new TenantBudgetKey(this.OrgId, budgetType));
				}
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)this.UserEntry[IADSecurityPrincipalSchema.Sid];
				if (securityIdentifier != null)
				{
					ADObjectId rootOrgId;
					if (this.OrgId == null || this.OrgId.Equals(OrganizationId.ForestWideOrgId))
					{
						rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
					}
					else
					{
						rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerId(this.OrgId.PartitionId.ForestFQDN, null, null);
					}
					result = PowerShellBudget.Acquire(securityIdentifier, budgetType, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgId, this.OrgId, this.OrgId, true));
				}
				else
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Sid is null, return null budget for AuthZPluginUserToken {0}.", this.UserName);
				}
			}
			return result;
		}

		protected virtual void CalculateOrgId()
		{
			if (this.DelegatedPrincipal != null)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Create OrgId for AuthZPluginUserToken {0} using DelegatedPrincipal.", this.UserName);
				ExchangeAuthorizationPlugin.TryFindOrganizationIdForDelegatedPrincipal(this.DelegatedPrincipal, out this.orgId);
				return;
			}
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Create OrgId for AuthZPluginUserToken {0} from UserEntry.", this.UserName);
			this.orgId = (OrganizationId)this.UserEntry[ADObjectSchema.OrganizationId];
		}

		protected string userName;

		protected bool orgIdCalculated;

		protected OrganizationId orgId;

		protected string windowsLiveId;

		protected bool windowsLiveIdCalculated;

		protected IList<string> domainsToBlockTogether;

		private string userNameForLogging;
	}
}
