using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class FreeBusyPermission
	{
		public static FreeBusyPermissionLevel DetermineAllowedAccess(ClientContext clientContext, MailboxSession session, CalendarFolder calendarFolder, FreeBusyQuery freeBusyQuery, bool defaultFreeBusyOnly)
		{
			RawSecurityDescriptor rawSecurityDescriptor = calendarFolder.TryGetProperty(CalendarFolderSchema.FreeBusySecurityDescriptor) as RawSecurityDescriptor;
			if (rawSecurityDescriptor == null)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, CalendarFolder>(0L, "{0}: Unable to retrieve FreeBusySecurityDescriptor from folder {1}. Using None as permission level.", TraceContext.Get(), calendarFolder);
				return FreeBusyPermissionLevel.None;
			}
			if (FreeBusyPermission.SecurityTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string sddlForm = rawSecurityDescriptor.GetSddlForm(AccessControlSections.All);
				FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress, string>(0L, "{0}: The SDDL form of calendar folder security descriptor of mailbox {1} is: {2}.", TraceContext.Get(), freeBusyQuery.Email, sddlForm);
			}
			if (defaultFreeBusyOnly)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug(0L, "{0}: Using DefaultClientSecurityContext because of defaultFreeBusyOnly is set.", new object[]
				{
					TraceContext.Get()
				});
				return FreeBusyPermission.AccessCheck(rawSecurityDescriptor, ClientSecurityContext.FreeBusyPermissionDefaultClientSecurityContext);
			}
			InternalClientContext internalClientContext = clientContext as InternalClientContext;
			if (internalClientContext != null)
			{
				return FreeBusyPermission.FromInternalClient(internalClientContext, rawSecurityDescriptor, freeBusyQuery);
			}
			ExternalClientContext externalClientContext = clientContext as ExternalClientContext;
			return FreeBusyPermission.FromExternalClient(externalClientContext, session, rawSecurityDescriptor, freeBusyQuery);
		}

		private static FreeBusyPermissionLevel FromInternalClient(InternalClientContext internalClientContext, RawSecurityDescriptor securityDescriptor, FreeBusyQuery freeBusyQuery)
		{
			if (internalClientContext.ClientSecurityContext == null)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress>(0L, "{0}: Caller {1} has no ClientSecurityContext, using default context as 'everyone'.", TraceContext.Get(), freeBusyQuery.Email);
				return FreeBusyPermission.AccessCheck(securityDescriptor, ClientSecurityContext.FreeBusyPermissionDefaultClientSecurityContext);
			}
			if (!Configuration.UseDisabledAccount || VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				return FreeBusyPermission.GetPermissionLevel(internalClientContext.ClientSecurityContext, freeBusyQuery, securityDescriptor);
			}
			FreeBusyPermission.SecurityTracer.TraceDebug<object, InternalClientContext>(0L, "{0}: Creating a munged security context for caller {1}.", TraceContext.Get(), internalClientContext);
			ClientSecurityContext clientSecurityContext = null;
			try
			{
				clientSecurityContext = new SlaveAccountTokenMunger().MungeToken(internalClientContext.ClientSecurityContext, OrganizationId.ForestWideOrgId);
				return FreeBusyPermission.GetPermissionLevel(clientSecurityContext, freeBusyQuery, securityDescriptor);
			}
			catch (TokenMungingException arg)
			{
				FreeBusyPermission.SecurityTracer.TraceError<object, InternalClientContext, TokenMungingException>(0L, "{0}: Unable to get the munged token for Caller {1}, error {2}, using the client context supplied.", TraceContext.Get(), internalClientContext, arg);
			}
			finally
			{
				if (clientSecurityContext != null)
				{
					clientSecurityContext.Dispose();
				}
			}
			return FreeBusyPermission.GetPermissionLevel(internalClientContext.ClientSecurityContext, freeBusyQuery, securityDescriptor);
		}

		private static FreeBusyPermissionLevel GetPermissionLevel(ClientSecurityContext clientSecurityContext, FreeBusyQuery freeBusyQuery, RawSecurityDescriptor securityDescriptor)
		{
			if (FreeBusyPermission.CallerHasFullPermission(clientSecurityContext, freeBusyQuery))
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, ClientSecurityContext, EmailAddress>(0L, "{0}: Caller {1} has owner access on mailbox {2}.", TraceContext.Get(), clientSecurityContext, freeBusyQuery.Email);
				return FreeBusyPermissionLevel.Owner;
			}
			return FreeBusyPermission.AccessCheck(securityDescriptor, clientSecurityContext);
		}

		private static bool CallerHasFullPermission(ClientSecurityContext clientSecurityContext, FreeBusyQuery freeBusyQuery)
		{
			SecurityIdentifier sid = freeBusyQuery.RecipientData.Sid;
			SecurityIdentifier masterAccountSid = freeBusyQuery.RecipientData.MasterAccountSid;
			bool flag = (sid != null && sid.Equals(clientSecurityContext.UserSid)) || (masterAccountSid != null && masterAccountSid.Equals(clientSecurityContext.UserSid));
			if (flag)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug(0L, "{0}: Caller {1} is owner of mailbox {2}, mailbox user SID {3}, master account SID {4}.", new object[]
				{
					TraceContext.Get(),
					clientSecurityContext,
					freeBusyQuery.Email,
					sid,
					masterAccountSid
				});
				return true;
			}
			RawSecurityDescriptor exchangeSecurityDescriptor = freeBusyQuery.RecipientData.ExchangeSecurityDescriptor;
			if (exchangeSecurityDescriptor != null)
			{
				if (FreeBusyPermission.SecurityTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					string sddlForm = exchangeSecurityDescriptor.GetSddlForm(AccessControlSections.All);
					FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress, string>(0L, "{0}: The SDDL form of mailbox security descriptor of mailbox {1} is: {2}.", TraceContext.Get(), freeBusyQuery.Email, sddlForm);
				}
				if (clientSecurityContext.GetGrantedAccess(exchangeSecurityDescriptor, AccessMask.CreateChild) == 1 || clientSecurityContext.GetGrantedAccess(exchangeSecurityDescriptor, AccessMask.List) == 4)
				{
					FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress>(0L, "{0}: Caller does have 'owner' rights in mailbox {1}.", TraceContext.Get(), freeBusyQuery.Email);
					return true;
				}
			}
			else
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress>(0L, "{0}: User does not have an ExchangeSecurityDescriptor.", TraceContext.Get(), freeBusyQuery.Email);
			}
			FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress>(0L, "{0}: Caller does NOT have 'owner' rights in mailbox {1}.", TraceContext.Get(), freeBusyQuery.Email);
			return false;
		}

		private static FreeBusyPermissionLevel FromExternalClient(ExternalClientContext externalClientContext, MailboxSession mailboxSession, RawSecurityDescriptor securityDescriptor, FreeBusyQuery freeBusyQuery)
		{
			FreeBusyPermissionLevel val = FreeBusyPermission.FromExternalClientWithPersonalRelationship(externalClientContext, mailboxSession, securityDescriptor, freeBusyQuery);
			FreeBusyPermissionLevel val2 = FreeBusyPermission.FromExternalClientWithOrganizationalRelationship(externalClientContext, mailboxSession, securityDescriptor, freeBusyQuery);
			FreeBusyPermissionLevel freeBusyPermissionLevel = (FreeBusyPermissionLevel)Math.Max((int)val2, (int)val);
			FreeBusyPermission.SecurityTracer.TraceDebug<object, SmtpAddress, FreeBusyPermissionLevel>(0L, "{0}: permission level for {1} is {2}", TraceContext.Get(), externalClientContext.EmailAddress, freeBusyPermissionLevel);
			return freeBusyPermissionLevel;
		}

		private static FreeBusyPermissionLevel FromExternalClientWithPersonalRelationship(ExternalClientContext externalClientContext, MailboxSession mailboxSession, RawSecurityDescriptor securityDescriptor, FreeBusyQuery freeBusyQuery)
		{
			string externalIdentity = FreeBusyPermission.GetExternalIdentity(externalClientContext, mailboxSession);
			if (externalIdentity == null)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, SmtpAddress, IExchangePrincipal>(0L, "{0}: No external identity for {1} in mailbox {2}.", TraceContext.Get(), externalClientContext.EmailAddress, mailboxSession.MailboxOwner);
				return FreeBusyPermissionLevel.None;
			}
			ISecurityAccessToken securityAccessToken = new SecurityAccessToken
			{
				UserSid = externalIdentity,
				GroupSids = ClientSecurityContext.DisabledEveryoneOnlySidStringAndAttributesArray
			};
			FreeBusyPermissionLevel result;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(securityAccessToken, AuthzFlags.AuthzSkipTokenGroups))
			{
				result = FreeBusyPermission.AccessCheck(securityDescriptor, clientSecurityContext);
			}
			return result;
		}

		private static string GetExternalIdentity(ExternalClientContext externalClientContext, MailboxSession session)
		{
			FreeBusyPermission.SecurityTracer.TraceDebug<object, ExternalClientContext, IExchangePrincipal>(0L, "{0}: searching for external identity for caller {1} in mailbox {2}", TraceContext.Get(), externalClientContext, session.MailboxOwner);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				PersonalClientContext personalClientContext = externalClientContext as PersonalClientContext;
				if (personalClientContext != null)
				{
					using (ExternalUserCollection externalUsers = session.GetExternalUsers())
					{
						ExternalUser externalUser = externalUsers.FindExternalUser(personalClientContext.ExternalId.ToString());
						if (externalUser != null)
						{
							string text = externalUser.Sid.ToString();
							FreeBusyPermission.SecurityTracer.TraceDebug<object, string>(0L, "{0}: found personal client context from external identity: {1}", TraceContext.Get(), text);
							return text;
						}
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				PerformanceCounters.AverageExternalAuthenticationIdentityMappingTime.IncrementBy(stopwatch.ElapsedTicks);
				PerformanceCounters.AverageExternalAuthenticationIdentityMappingTimeBase.Increment();
			}
			return null;
		}

		private static FreeBusyPermissionLevel FromExternalClientWithOrganizationalRelationship(ExternalClientContext externalClientContext, MailboxSession mailboxSession, RawSecurityDescriptor securityDescriptor, FreeBusyQuery freeBusyQuery)
		{
			OrganizationRelationship organizationRelationship = FreeBusyPermission.GetOrganizationRelationship(mailboxSession.MailboxOwner.MailboxInfo.OrganizationId, externalClientContext.EmailAddress.Domain);
			if (organizationRelationship == null)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, SmtpAddress, string>(0L, "{0}: No organization relationship for {1} with organization id {2}", TraceContext.Get(), externalClientContext.EmailAddress, (mailboxSession.MailboxOwner.MailboxInfo.OrganizationId == null) ? "<null>" : mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.ToString());
				return FreeBusyPermissionLevel.None;
			}
			FreeBusyPermissionLevel freeBusyPermissionLevel = FreeBusyPermissionLevel.Detail;
			if (organizationRelationship != null)
			{
				freeBusyPermissionLevel = FreeBusyPermission.GetMaximumFreeBusyPermissionLevel(organizationRelationship);
				if (freeBusyPermissionLevel == FreeBusyPermissionLevel.None)
				{
					FreeBusyPermission.SecurityTracer.TraceDebug<object, ADObjectId>(0L, "{0}: OrganizationRelationship {1} restricts permission level to None.", TraceContext.Get(), organizationRelationship.Id);
					return FreeBusyPermissionLevel.None;
				}
			}
			FreeBusyPermissionLevel freeBusyPermissionLevel2 = FreeBusyPermission.AccessCheck(securityDescriptor, ClientSecurityContext.FreeBusyPermissionDefaultClientSecurityContext);
			if (freeBusyPermissionLevel2 == FreeBusyPermissionLevel.None)
			{
				return FreeBusyPermissionLevel.None;
			}
			if (freeBusyPermissionLevel2 > freeBusyPermissionLevel)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug(0L, "{0}: OrganizationRelationship {1} restricts permission level to {2}. Lowering permission from {3}.", new object[]
				{
					TraceContext.Get(),
					organizationRelationship.Id,
					freeBusyPermissionLevel,
					freeBusyPermissionLevel2
				});
				freeBusyPermissionLevel2 = freeBusyPermissionLevel;
			}
			if (!FreeBusyPermission.IsAllowedByFreeBusyAccessScope(freeBusyQuery, organizationRelationship))
			{
				freeBusyPermissionLevel2 = FreeBusyPermissionLevel.None;
			}
			return freeBusyPermissionLevel2;
		}

		private static FreeBusyPermissionLevel GetMaximumFreeBusyPermissionLevel(OrganizationRelationship organizationRelationship)
		{
			switch (organizationRelationship.FreeBusyAccessLevel)
			{
			case FreeBusyAccessLevel.None:
				return FreeBusyPermissionLevel.None;
			case FreeBusyAccessLevel.AvailabilityOnly:
				return FreeBusyPermissionLevel.Simple;
			case FreeBusyAccessLevel.LimitedDetails:
				return FreeBusyPermissionLevel.Detail;
			default:
				return FreeBusyPermissionLevel.None;
			}
		}

		private static bool IsAllowedByFreeBusyAccessScope(FreeBusyQuery freeBusyQuery, OrganizationRelationship organizationRelationship)
		{
			if (organizationRelationship.FreeBusyAccessScope == null)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, ADObjectId>(0L, "{0}: OrganizationRelationship {1} doesn't restrict any mailbox to share externally.", TraceContext.Get(), organizationRelationship.Id);
				return true;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, organizationRelationship.Session.SessionSettings, 620, "IsAllowedByFreeBusyAccessScope", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\Availability\\FreeBusyPermission.cs");
			ADGroup adgroup = tenantOrRootOrgRecipientSession.Read(organizationRelationship.FreeBusyAccessScope) as ADGroup;
			if (adgroup == null)
			{
				FreeBusyPermission.SecurityTracer.TraceError<object, ADObjectId>(0L, "{0}: OrganizationRelationship.FreeBusyAccessScope is defined as {1}, but cannot be found in AD.", TraceContext.Get(), organizationRelationship.FreeBusyAccessScope);
				return false;
			}
			if (!adgroup.ContainsMember(freeBusyQuery.RecipientData.Id, false))
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, EmailAddress, ADObjectId>(0L, "{0}: mailbox {1} is not member of OrganizationRelationship.FreeBusyAccessScope {2}.", TraceContext.Get(), freeBusyQuery.Email, organizationRelationship.FreeBusyAccessScope);
				return false;
			}
			return true;
		}

		internal static OrganizationRelationship GetOrganizationRelationship(OrganizationId organizationId, string requesterDomain)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get((organizationId == null) ? OrganizationId.ForestWideOrgId : organizationId);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(requesterDomain);
			if (organizationRelationship == null)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, string>(0L, "{0}: No organization relationship found for domain {1}", TraceContext.Get(), requesterDomain);
				return null;
			}
			if (!organizationRelationship.Enabled)
			{
				FreeBusyPermission.SecurityTracer.TraceDebug<object, string>(0L, "{0}: Organization relationship for domain {1} is disabled.", TraceContext.Get(), requesterDomain);
				return null;
			}
			return organizationRelationship;
		}

		public static FreeBusyPermissionLevel AccessCheck(RawSecurityDescriptor securityDescriptor, ClientSecurityContext clientContext)
		{
			int grantedAccess = clientContext.GetGrantedAccess(securityDescriptor, AccessMask.MaximumAllowed);
			FreeBusyPermissionLevel freeBusyPermissionLevel = FreeBusyPermissionLevel.None;
			if ((grantedAccess & 2) != 0)
			{
				freeBusyPermissionLevel = FreeBusyPermissionLevel.Detail;
			}
			else if ((grantedAccess & 1) != 0)
			{
				freeBusyPermissionLevel = FreeBusyPermissionLevel.Simple;
			}
			FreeBusyPermission.SecurityTracer.TraceDebug(0L, "{0}: Access check for {1} resulted in granted access {2}, permission level {3}", new object[]
			{
				TraceContext.Get(),
				clientContext,
				grantedAccess,
				freeBusyPermissionLevel
			});
			return freeBusyPermissionLevel;
		}

		private const int FsdpermUserMailboxOwner = 1;

		private const int FsdpermUserPrimaryUser = 4;

		private static readonly Microsoft.Exchange.Diagnostics.Trace SecurityTracer = ExTraceGlobals.SecurityTracer;
	}
}
