using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class OrgSuiteStorageHelper
	{
		private static bool IsDatacenter
		{
			get
			{
				return OrgSuiteStorageHelper.IsDatacenterEnvironment.Member;
			}
		}

		public static ExchangePrincipal GetOrgMailbox(string domain, RequestDetailsLogger logger)
		{
			ADSessionSettings adsessionSettings = null;
			if (OrgSuiteStorageHelper.IsDatacenter)
			{
				if (domain == null)
				{
					logger.Set(SuiteStorage.GetOrgMailboxIsDcDomain, "IsNull");
					return null;
				}
				logger.Set(SuiteStorage.GetOrgMailboxIsDcDomain, domain);
				try
				{
					adsessionSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
				}
				catch (CannotResolveTenantNameException)
				{
					ExTraceGlobals.ExceptionTracer.TraceError(0L, "[OrgSuiteStorageHelper::GetOrgMailbox] CannotResolveTenantNameException.");
					logger.Set(SuiteStorage.CannotResolveTenantNameException, domain);
					return null;
				}
			}
			if (adsessionSettings == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[OrgSuiteStorageHelper::GetOrgMailbox] Getting mailbox session settings for FirstOrgFromRootOrgScopeSet.");
				logger.Set(SuiteStorage.GetOrgMailboxSessionSettings, "FirstOrgFromRootOrgScopeSet");
				adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 99, "GetOrgMailbox", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\jsonservice\\servicecommands\\OrgSuiteStorageHelper.cs");
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(tenantOrRootOrgRecipientSession, OrganizationCapability.SuiteServiceStorage);
			if (organizationMailboxesByCapability == null || 1 != organizationMailboxesByCapability.Count)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[OrgSuiteStorageHelper::GetOrgMailbox] Failed to get an org mailbox, count is 0.");
				logger.Set(SuiteStorage.GetOrgMailboxCount, "0");
				return null;
			}
			logger.Set(SuiteStorage.GetOrgMailbox, organizationMailboxesByCapability[0]);
			try
			{
				return ExchangePrincipal.FromADUser(tenantOrRootOrgRecipientSession.SessionSettings, organizationMailboxesByCapability[0], RemotingOptions.AllowCrossSite);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[OrgSuiteStorageHelper::GetOrgMailbox] ObjectNotFoundException.");
				logger.Set(SuiteStorage.ObjectNotFoundException, ex.ToString());
			}
			return null;
		}

		private static readonly LazyMember<bool> IsDatacenterEnvironment = new LazyMember<bool>(new Func<bool>(Datacenter.IsMultiTenancyEnabled));
	}
}
