using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaClientSecurityContextIdentity : OwaIdentity
	{
		private OwaClientSecurityContextIdentity(ClientSecurityContext clientSecurityContext, string logonName, string authenticationType, OrganizationId userOrganizationId)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			if (string.IsNullOrEmpty(logonName))
			{
				throw new ArgumentNullException("logonName", "logonName cannot be null or empty.");
			}
			if (userOrganizationId == null && !OwaIdentity.IsLogonNameFullyQualified(logonName))
			{
				throw new ArgumentException("logonName", string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid logon name.", new object[]
				{
					logonName
				}));
			}
			if (string.IsNullOrEmpty(authenticationType))
			{
				throw new ArgumentNullException("authenticationType", "authenticationType cannot be null or empty.");
			}
			this.logonName = logonName;
			this.authenticationType = authenticationType;
			base.UserOrganizationId = userOrganizationId;
			if (!SyncUtilities.IsDatacenterMode())
			{
				this.clientSecurityContext = clientSecurityContext;
				OWAMiniRecipient owaminiRecipient = base.GetOWAMiniRecipient();
				if (owaminiRecipient != null && owaminiRecipient.MasterAccountSid != null)
				{
					try
					{
						this.clientSecurityContext = OwaClientSecurityContextIdentity.TokenMunger.MungeToken(clientSecurityContext, OrganizationId.ForestWideOrgId);
						return;
					}
					catch (TokenMungingException ex)
					{
						ExTraceGlobals.CoreCallTracer.TraceError(0L, "OwaClientSecurityContextIdentity.TokenMunger.MungeToken for LogonName='{0}', AuthenticationType='{1}', UserOrgId='{2}' failed with exception: {3}", new object[]
						{
							this.logonName,
							this.authenticationType,
							base.UserOrganizationId,
							ex.Message
						});
					}
				}
			}
			this.clientSecurityContext = clientSecurityContext.Clone();
		}

		public override bool IsPartial
		{
			get
			{
				return this.clientSecurityContext == null;
			}
		}

		public override WindowsIdentity WindowsIdentity
		{
			get
			{
				throw new OwaNotSupportedException("WindowsIdentity");
			}
		}

		public override SecurityIdentifier UserSid
		{
			get
			{
				return this.clientSecurityContext.UserSid;
			}
		}

		public override string UniqueId
		{
			get
			{
				return this.UserSid.ToString();
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		public override string GetLogonName()
		{
			return this.logonName;
		}

		public override string SafeGetRenderableName()
		{
			return this.logonName;
		}

		internal static OwaClientSecurityContextIdentity CreateFromClientSecurityContext(ClientSecurityContext clientSecurityContext, string logonName, string authenticationType)
		{
			return new OwaClientSecurityContextIdentity(clientSecurityContext, logonName, authenticationType, null);
		}

		internal static OwaClientSecurityContextIdentity CreateFromClientSecurityContextIdentity(ClientSecurityContextIdentity cscIdentity)
		{
			if (cscIdentity == null)
			{
				throw new ArgumentNullException("cscIdentity");
			}
			return OwaClientSecurityContextIdentity.InternalCreateFromClientSecurityContextIdentity(cscIdentity, cscIdentity.Name, null);
		}

		internal static OwaClientSecurityContextIdentity CreateFromOAuthIdentity(OAuthIdentity oauthIdentity)
		{
			if (oauthIdentity == null)
			{
				throw new ArgumentNullException("oauthIdentity");
			}
			ExAssert.RetailAssert(!oauthIdentity.IsAppOnly, "IsApplyOnly cannot be null in OAuthIdentity.");
			ExAssert.RetailAssert(oauthIdentity.ActAsUser != null, "ActAsUser cannot be null in OAuthIdentity.");
			string partitionId = string.Empty;
			if (!(oauthIdentity.OrganizationId == null) && !(oauthIdentity.OrganizationId.PartitionId == null))
			{
				partitionId = oauthIdentity.OrganizationId.PartitionId.ToString();
			}
			SidBasedIdentity cscIdentity = new SidBasedIdentity(oauthIdentity.ActAsUser.UserPrincipalName, oauthIdentity.ActAsUser.Sid.Value, oauthIdentity.ActAsUser.UserPrincipalName, oauthIdentity.AuthenticationType, partitionId)
			{
				UserOrganizationId = oauthIdentity.OrganizationId
			};
			return OwaClientSecurityContextIdentity.InternalCreateFromClientSecurityContextIdentity(cscIdentity, oauthIdentity.ActAsUser.UserPrincipalName, null);
		}

		internal static OwaClientSecurityContextIdentity CreateFromLiveIDIdentity(LiveIDIdentity liveIDIdentity)
		{
			if (liveIDIdentity == null)
			{
				throw new ArgumentNullException("liveIDIdentity");
			}
			return OwaClientSecurityContextIdentity.InternalCreateFromClientSecurityContextIdentity(liveIDIdentity, liveIDIdentity.MemberName, liveIDIdentity.UserOrganizationId);
		}

		internal static OwaClientSecurityContextIdentity CreateFromAdfsIdentity(AdfsIdentity adfsIdentity)
		{
			if (adfsIdentity == null)
			{
				throw new ArgumentNullException("adfsIdentity");
			}
			return OwaClientSecurityContextIdentity.InternalCreateFromClientSecurityContextIdentity(adfsIdentity, adfsIdentity.MemberName, adfsIdentity.UserOrganizationId);
		}

		internal static OwaClientSecurityContextIdentity CreateFromsidBasedIdentity(SidBasedIdentity sidBasedIdentity)
		{
			if (sidBasedIdentity == null)
			{
				throw new ArgumentNullException("sidBasedIdentity");
			}
			return OwaClientSecurityContextIdentity.InternalCreateFromClientSecurityContextIdentity(sidBasedIdentity, sidBasedIdentity.MemberName, sidBasedIdentity.UserOrganizationId);
		}

		private static OwaClientSecurityContextIdentity InternalCreateFromClientSecurityContextIdentity(ClientSecurityContextIdentity cscIdentity, string logonName, OrganizationId userOrganizationId = null)
		{
			SidBasedIdentity sidBasedIdentity = cscIdentity as SidBasedIdentity;
			if (sidBasedIdentity != null)
			{
				OwaClientSecurityContextIdentity.PrePopulateUserGroupSids(sidBasedIdentity);
			}
			OwaClientSecurityContextIdentity result;
			try
			{
				using (ClientSecurityContext clientSecurityContext = cscIdentity.CreateClientSecurityContext())
				{
					result = new OwaClientSecurityContextIdentity(clientSecurityContext, logonName, cscIdentity.AuthenticationType, userOrganizationId);
				}
			}
			catch (AuthzException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, string, AuthzException>(0L, "OwaClientSecurityContextIdentity.CreateFromClientSecurityContextIdentity for ClientSecurityContextIdentity.Name={0} ClientSecurityContextIdentity.AuthenticationType={1} failed with exception: {2}", cscIdentity.Name, cscIdentity.AuthenticationType, ex);
				if (ex.InnerException is Win32Exception)
				{
					throw new OwaIdentityException("There was a problem creating the Client Security Context.", ex);
				}
				throw;
			}
			return result;
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "OwaClientSecurityContextIdentity.CreateExchangePrincipal Creating scoped AD session for {0}", (base.UserOrganizationId == null) ? this.DomainName : base.UserOrganizationId.ToString());
			bool flag = HttpContext.Current != null && UserAgentUtilities.IsMonitoringRequest(HttpContext.Current.Request.UserAgent);
			ExchangePrincipal result;
			try
			{
				ADSessionSettings sessionSettings = (base.UserOrganizationId == null) ? UserContextUtilities.CreateScopedSessionSettings(this.DomainName, null) : UserContextUtilities.CreateScopedSessionSettings(null, base.UserOrganizationId);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 417, "InternalCreateExchangePrincipal", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\OwaClientSecurityContextIdentity.cs");
				if (flag)
				{
					TimeSpan value = TimeSpan.FromSeconds((double)OwaClientSecurityContextIdentity.ADTimeoutForExchangePrincipalInstantiation.Value);
					ExTraceGlobals.CoreCallTracer.TraceDebug<double>(0L, "OwaClientSecurityContextIdentity.CreateExchangePrincipal Reduced AD timeout to {0} seconds", value.TotalSeconds);
					tenantOrRootOrgRecipientSession.ClientSideSearchTimeout = new TimeSpan?(value);
				}
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaClientSecurityContextIdentity.CreateExchangePrincipal Calling ExchangePrincipal.FromUserSid");
				try
				{
					result = ExchangePrincipal.FromUserSid(tenantOrRootOrgRecipientSession, this.UserSid);
				}
				catch (UserHasNoMailboxException ex)
				{
					ADUser aduser = tenantOrRootOrgRecipientSession.FindBySid(this.UserSid) as ADUser;
					ex.Data.Add("PrimarySmtpAddress", this.logonName);
					if (aduser == null)
					{
						throw;
					}
					if (aduser.RecipientType == RecipientType.MailUser && aduser.SKUAssigned != true)
					{
						throw new OwaUserHasNoMailboxAndNoLicenseAssignedException(ex.Message, ex.InnerException);
					}
					throw;
				}
			}
			catch (Exception ex2)
			{
				OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaFailedToCreateExchangePrincipal, string.Empty, new object[]
				{
					this.UserSid,
					flag,
					ex2
				});
				throw;
			}
			return result;
		}

		internal override MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.CreateMailboxSession(exchangePrincipal, cultureInfo, "Client=OWA;Action=ViaProxy");
		}

		internal override MailboxSession CreateInstantSearchMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			if (exchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				return this.CreateDelegateMailboxSession(exchangePrincipal, cultureInfo, "Client=OWA;Action=InstantSearch");
			}
			return this.CreateMailboxSession(exchangePrincipal, cultureInfo, "Client=OWA;Action=InstantSearch");
		}

		internal MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, string userContextString)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaClientSecurityContextIdentity.CreateMailboxSession");
			MailboxSession result;
			try
			{
				MailboxSession mailboxSession = MailboxSession.Open(exchangePrincipal, this.clientSecurityContext, cultureInfo, userContextString);
				result = mailboxSession;
			}
			catch (AccessDeniedException innerException)
			{
				throw new OwaExplicitLogonException("User has no access rights to the mailbox", "ErrorExplicitLogonAccessDenied", innerException);
			}
			return result;
		}

		internal override MailboxSession CreateDelegateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.CreateDelegateMailboxSession(exchangePrincipal, cultureInfo, "Client=OWA;Action=ViaProxy");
		}

		internal MailboxSession CreateDelegateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, string userContextString)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaClientSecurityContextIdentity.CreateMailboxSession");
			MailboxSession result;
			try
			{
				IADOrgPerson iadorgPerson = base.CreateADRecipientBySid() as IADOrgPerson;
				if (iadorgPerson == null)
				{
					throw new OwaExplicitLogonException("User do not have representation in current forest", null);
				}
				MailboxSession mailboxSession = MailboxSession.OpenWithBestAccess(exchangePrincipal, iadorgPerson, this.clientSecurityContext, cultureInfo, userContextString);
				result = mailboxSession;
			}
			catch (AccessDeniedException innerException)
			{
				throw new OwaExplicitLogonException("User has no access rights to the mailbox", Strings.GetLocalizedString(882888134), innerException);
			}
			return result;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.clientSecurityContext != null)
			{
				this.clientSecurityContext.Dispose();
				this.clientSecurityContext = null;
			}
			base.InternalDispose(isDisposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaClientSecurityContextIdentity>(this);
		}

		private static void PrePopulateUserGroupSids(SidBasedIdentity sidBasedIdentity)
		{
			if (sidBasedIdentity.PrepopulatedGroupSidIds == null || sidBasedIdentity.PrepopulatedGroupSidIds.Count<string>() <= 0)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Attempting to prepopulate group SIDS for user '{0}'.", sidBasedIdentity.Sid.Value);
				if (sidBasedIdentity.UserOrganizationId != null)
				{
					List<string> list = null;
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(sidBasedIdentity.UserOrganizationId), 647, "PrePopulateUserGroupSids", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\OwaClientSecurityContextIdentity.cs");
					if (tenantOrRootOrgRecipientSession != null)
					{
						ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "OwaClientSecurityContextIdentity.GetUserGroupSidIds created IRecipientSession instance for user '{0}'.", sidBasedIdentity.Sid.Value);
						ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindBySid(sidBasedIdentity.Sid);
						if (adrecipient != null)
						{
							ExTraceGlobals.CoreCallTracer.TraceDebug<string, SmtpAddress, string>(0L, "OwaClientSecurityContextIdentity.GetUserGroupSidIds fetched ADRecipient instance with DisplaName '{0}' and PrimarySmtpAddress '{1}' for user '{2}'.", adrecipient.DisplayName, adrecipient.PrimarySmtpAddress, sidBasedIdentity.Sid.Value);
							list = tenantOrRootOrgRecipientSession.GetTokenSids(adrecipient, AssignmentMethod.S4U);
						}
						else
						{
							ExTraceGlobals.CoreCallTracer.TraceError<string>(0L, "OwaClientSecurityContextIdentity.GetUserGroupSidIds was unable to get ADRecipient instance for user '{0}'.", sidBasedIdentity.Sid.Value);
						}
					}
					else
					{
						ExTraceGlobals.CoreCallTracer.TraceError<string>(0L, "OwaClientSecurityContextIdentity.GetUserGroupSidIds was unable to get IRecipientSession instance for user '{0}'.", sidBasedIdentity.Sid.Value);
					}
					if (list == null)
					{
						ExTraceGlobals.CoreCallTracer.TraceError<string>(0L, "OwaClientSecurityContextIdentity.GetUserGroupSidIds was unable to find any group SIDs for user '{0}'.", sidBasedIdentity.Sid.Value);
						return;
					}
					ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>(0L, "Prepopulating User group SIds '{0}', for user '{1}'.", string.Join(", ", list), sidBasedIdentity.Sid.Value);
					sidBasedIdentity.PrepopulateGroupSidIds(list);
				}
			}
		}

		public const string ErrorMessageKeyForUserSmtpAddress = "PrimarySmtpAddress";

		private static readonly IntAppSettingsEntry ADTimeoutForExchangePrincipalInstantiation = new IntAppSettingsEntry("ADTimeoutForExchangePrincipalInstantiation", 20, null);

		private static readonly ITokenMunger TokenMunger = new SlaveAccountTokenMunger();

		private readonly string logonName;

		private readonly string authenticationType;

		private ClientSecurityContext clientSecurityContext;
	}
}
