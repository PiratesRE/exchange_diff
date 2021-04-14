using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class RfriContext : IDisposeTrackable, IDisposable
	{
		internal RfriContext(ClientSecurityContext clientSecurityContext, string userDomain, string clientAddress, string serverAddress, string protocolSequence, string authenticationService, bool encrypted, bool isAnonymous, Guid requestId = default(Guid))
		{
			this.ContextHandle = NspiContext.GetNextContextHandle();
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.clientSecurityContext = clientSecurityContext;
			this.userDomain = userDomain;
			this.encrypted = encrypted;
			this.isAnonymous = isAnonymous;
			this.protocolSequence = protocolSequence;
			this.protocolLogSession = ProtocolLog.CreateSession(this.ContextHandle, clientAddress, serverAddress, protocolSequence);
			this.ProtocolLogSession[ProtocolLog.Field.Authentication] = authenticationService;
			if (requestId != Guid.Empty)
			{
				ActivityContextState activityContextState = new ActivityContextState(new Guid?(requestId), new ConcurrentDictionary<Enum, object>());
				ActivityContext.ClearThreadScope();
				this.scope = ActivityContext.Resume(activityContextState, null);
			}
		}

		internal int ContextHandle { get; private set; }

		internal string LegacyDistinguishedName
		{
			get
			{
				if (this.nspiPrincipal == null)
				{
					return string.Empty;
				}
				return this.nspiPrincipal.LegacyDistinguishedName;
			}
		}

		internal ProtocolLogSession ProtocolLogSession
		{
			get
			{
				return this.protocolLogSession;
			}
		}

		internal IStandardBudget Budget
		{
			get
			{
				if (this.budget == null)
				{
					throw new InvalidOperationException("Budget has not been acquired");
				}
				return this.budget;
			}
		}

		public IActivityScope ActivityScope
		{
			get
			{
				if (this.scope != null && !this.scope.IsDisposed)
				{
					return this.scope;
				}
				return null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.clientSecurityContext != null)
				{
					this.clientSecurityContext.Dispose();
					this.clientSecurityContext = null;
				}
				if (this.budget != null)
				{
					this.budget.Dispose();
					this.budget = null;
				}
				if (this.scope != null)
				{
					this.scope.End();
					this.scope = null;
				}
			}
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriContext>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal bool TryAcquireBudget()
		{
			if (this.budget != null)
			{
				throw new InvalidOperationException("Budget already acquired");
			}
			Exception ex2;
			try
			{
				ADSessionSettings settings;
				if (!string.IsNullOrEmpty(this.userDomain))
				{
					settings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(this.userDomain);
				}
				else
				{
					settings = ADSessionSettings.FromRootOrgScopeSet();
				}
				this.budget = StandardBudget.Acquire(this.clientSecurityContext.UserSid, BudgetType.Rca, settings);
				return true;
			}
			catch (DataValidationException ex)
			{
				ex2 = ex;
			}
			catch (ADTransientException ex3)
			{
				ex2 = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex2 = ex4;
			}
			RfriContext.ReferralTracer.TraceError<string>((long)this.ContextHandle, "TryAcquireBudget exception: {0}", ex2.Message);
			return false;
		}

		internal RfriStatus Initialize()
		{
			if (this.clientSecurityContext == null)
			{
				return RfriStatus.LogonFailed;
			}
			try
			{
				this.nspiPrincipal = NspiPrincipal.FromUserSid(this.clientSecurityContext.UserSid, this.userDomain);
				if (this.nspiPrincipal.OrganizationId != null && this.nspiPrincipal.OrganizationId.OrganizationalUnit != null)
				{
					this.protocolLogSession[ProtocolLog.Field.OrganizationInfo] = this.nspiPrincipal.OrganizationId.OrganizationalUnit.ToCanonicalName();
				}
			}
			catch (NonUniqueRecipientException)
			{
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (MailboxInfoStaleException)
			{
			}
			catch (CannotGetSiteInfoException)
			{
			}
			this.protocolLogSession[ProtocolLog.Field.ClientName] = this.LegacyDistinguishedName;
			if (ExUserTracingAdaptor.Instance.IsTracingEnabledUser(this.LegacyDistinguishedName))
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
			RfriContext.ReferralTracer.TraceDebug<string, SecurityIdentifier>((long)this.ContextHandle, "User {0}, Sid: {1}", this.LegacyDistinguishedName, this.clientSecurityContext.UserSid);
			if (!this.isAnonymous && Configuration.EncryptionRequired && !this.encrypted)
			{
				RfriContext.ReferralTracer.TraceError((long)this.ContextHandle, "Encrypted connection is required.");
				this.ProtocolLogSession[ProtocolLog.Field.Failures] = "EncryptionRequired";
				return RfriStatus.GeneralFailure;
			}
			return RfriStatus.Success;
		}

		internal RfriStatus GetNewDSA(string userDN, out string serverFQDN)
		{
			serverFQDN = null;
			RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "Requested user DN: {0}", RfriContext.GetString(userDN));
			string str;
			RfriStatus rfriStatus;
			if (this.IsUsingHttp())
			{
				str = userDN;
				if (!this.TryGetPersonalizedServer(userDN, out serverFQDN))
				{
					rfriStatus = ServerFqdnCache.LookupFQDNByLegacyDN(null, out serverFQDN);
					RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "ServerFqdnCache.LookupFQDNByLegacyDN(null): {0}", serverFQDN ?? "(null)");
				}
				else
				{
					rfriStatus = RfriStatus.Success;
				}
			}
			else
			{
				string clientAccessServerLegacyDN = this.GetClientAccessServerLegacyDN(userDN);
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "RfriContext.GetClientAccessServerLegacyDN: {0}", clientAccessServerLegacyDN ?? "(null)");
				str = clientAccessServerLegacyDN;
				rfriStatus = ServerFqdnCache.LookupFQDNByLegacyDN(clientAccessServerLegacyDN, out serverFQDN);
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "ServerFqdnCache.LookupFQDNByLegacyDN: {0}", serverFQDN ?? "(null)");
			}
			if (rfriStatus != RfriStatus.Success || string.IsNullOrEmpty(serverFQDN))
			{
				if (rfriStatus == RfriStatus.Success)
				{
					rfriStatus = RfriStatus.NoSuchObject;
				}
				this.ProtocolLogSession[ProtocolLog.Field.Failures] = RfriContext.GetString(str);
			}
			else
			{
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "Referring to {0}", serverFQDN);
			}
			return rfriStatus;
		}

		internal RfriStatus GetFQDNFromLegacyDN(string legacyDN, out string serverFQDN)
		{
			RfriStatus rfriStatus = RfriStatus.Success;
			serverFQDN = null;
			RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "Requested DN: {0}", RfriContext.GetString(legacyDN));
			if (legacyDN.Contains("@"))
			{
				LegacyDN legacyDN2;
				if (LegacyDN.TryParse(legacyDN, out legacyDN2))
				{
					string text;
					legacyDN2.GetParentLegacyDN(out text, out serverFQDN);
					if (string.IsNullOrEmpty(serverFQDN) || !serverFQDN.Contains("@"))
					{
						rfriStatus = RfriStatus.NoSuchObject;
					}
				}
				else
				{
					rfriStatus = RfriStatus.NoSuchObject;
				}
			}
			else
			{
				legacyDN = ExchangeRpcClientAccess.FixFakeRedirectLegacyDNIfNeeded(legacyDN);
				rfriStatus = ServerFqdnCache.LookupFQDNByLegacyDN(legacyDN, out serverFQDN);
			}
			if (rfriStatus != RfriStatus.Success || string.IsNullOrEmpty(serverFQDN))
			{
				if (rfriStatus == RfriStatus.Success)
				{
					rfriStatus = RfriStatus.NoSuchObject;
				}
				this.ProtocolLogSession[ProtocolLog.Field.Failures] = RfriContext.GetString(legacyDN);
			}
			else
			{
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "Referring to {0}", serverFQDN);
			}
			return rfriStatus;
		}

		internal RfriStatus GetMailboxUrl(string hostname, string serverDn, out string serverUrl)
		{
			serverUrl = string.Empty;
			string text;
			RfriStatus rfriStatus = this.GetFQDNFromLegacyDN(serverDn, out text);
			if (rfriStatus == RfriStatus.Success)
			{
				if (text.Contains("@"))
				{
					serverUrl = MapiHttpEndpoints.GetMailboxUrl(hostname, text);
				}
				else
				{
					rfriStatus = RfriStatus.NoSuchObject;
				}
			}
			return rfriStatus;
		}

		internal RfriStatus GetAddressBookUrl(string hostname, string userDn, out string serverUrl)
		{
			serverUrl = string.Empty;
			RfriStatus result = RfriStatus.NoSuchObject;
			string mailboxId;
			if (this.TryGetPersonalizedServer(userDn, out mailboxId))
			{
				serverUrl = MapiHttpEndpoints.GetAddressBookUrl(hostname, mailboxId);
				result = RfriStatus.Success;
			}
			return result;
		}

		internal string GetClientAccessServerLegacyDN(string userLegacyDN)
		{
			Guid guid = Guid.Empty;
			NspiPrincipal nspiPrincipal;
			if (string.IsNullOrEmpty(userLegacyDN) || LegacyDN.StringComparer.Equals(userLegacyDN, this.LegacyDistinguishedName))
			{
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "Self";
				nspiPrincipal = this.nspiPrincipal;
			}
			else
			{
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "Other";
				nspiPrincipal = this.NspiPrincipalFromLegacyDN(userLegacyDN);
				if (nspiPrincipal == null)
				{
					return null;
				}
			}
			if (nspiPrincipal.Database == null)
			{
				return null;
			}
			guid = nspiPrincipal.Database.ObjectGuid;
			if (guid == Guid.Empty)
			{
				return null;
			}
			LegacyDN legacyDN;
			try
			{
				ADObjectId adobjectId;
				ActiveManager.GetCachingActiveManagerInstance().CalculatePreferredHomeServer(guid, out legacyDN, out adobjectId);
			}
			catch (DatabaseNotFoundException)
			{
				legacyDN = null;
			}
			if (legacyDN == null)
			{
				return null;
			}
			return legacyDN.ToString();
		}

		internal NspiPrincipal NspiPrincipalFromLegacyDN(string legacyDN)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.nspiPrincipal.OrganizationId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, this.nspiPrincipal.DirectorySearchRoot, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 584, "NspiPrincipalFromLegacyDN", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\RfriContext.cs");
			tenantOrRootOrgRecipientSession.ServerTimeout = Configuration.ADTimeout;
			try
			{
				ADUser aduser = tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(legacyDN) as ADUser;
				if (aduser != null && !(bool)aduser[ADRecipientSchema.HiddenFromAddressListsValue])
				{
					return NspiPrincipal.FromADUser(aduser);
				}
			}
			catch (NonUniqueRecipientException)
			{
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "NonUniqueRecipientException thrown for {0}", legacyDN);
			}
			catch (ObjectNotFoundException)
			{
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "ObjectNotFoundException thrown: Couldn't find requested user  for {0}", legacyDN);
			}
			catch (MailboxInfoStaleException)
			{
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "MailboxInfoStaleException thrown for {0}", legacyDN);
			}
			catch (CannotGetSiteInfoException)
			{
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "CannotGetSiteInfoException thrown for {0}", legacyDN);
			}
			return null;
		}

		private static string GetString(string str)
		{
			if (str == null)
			{
				return "(null)";
			}
			if (str.Length == 0)
			{
				return "(empty)";
			}
			return str;
		}

		private bool IsUsingHttp()
		{
			return this.protocolSequence.Equals("ncacn_http", StringComparison.OrdinalIgnoreCase) || this.protocolSequence.Equals("MapiHttp");
		}

		private bool TryGetPersonalizedServer(string userLegacyDN, out string personalizedServer)
		{
			personalizedServer = string.Empty;
			bool flag = true;
			NspiPrincipal nspiPrincipal;
			if (string.IsNullOrEmpty(userLegacyDN) || LegacyDN.StringComparer.Equals(userLegacyDN, this.LegacyDistinguishedName))
			{
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "Self";
				nspiPrincipal = this.nspiPrincipal;
			}
			else
			{
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "Other";
				nspiPrincipal = this.NspiPrincipalFromLegacyDN(userLegacyDN);
				flag = false;
				if (nspiPrincipal == null)
				{
					return false;
				}
			}
			if (!flag && nspiPrincipal.ExchangeVersion != null && nspiPrincipal.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
			{
				string clientAccessServerLegacyDN = this.GetClientAccessServerLegacyDN(userLegacyDN);
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "RfriContext.GetClientAccessServerLegacyDN: {0}", RfriContext.GetString(clientAccessServerLegacyDN));
				ServerFqdnCache.LookupFQDNByLegacyDN(clientAccessServerLegacyDN, out personalizedServer);
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "ServerFqdnCache.LookupFQDNByLegacyDN: {0}", RfriContext.GetString(personalizedServer));
				return true;
			}
			SmtpAddress primarySmtpAddress = nspiPrincipal.PrimarySmtpAddress;
			if (nspiPrincipal.ExchangeGuid != Guid.Empty)
			{
				personalizedServer = ExchangeRpcClientAccess.CreatePersonalizedServer(nspiPrincipal.ExchangeGuid, nspiPrincipal.PrimarySmtpAddress.Domain);
				RfriContext.ReferralTracer.TraceDebug<string>((long)this.ContextHandle, "RfriContext.GetPersonalizedServer: {0}", RfriContext.GetString(personalizedServer));
				return true;
			}
			return false;
		}

		private static readonly Trace ReferralTracer = ExTraceGlobals.ReferralTracer;

		private readonly ProtocolLogSession protocolLogSession;

		private readonly string userDomain;

		private readonly bool encrypted;

		private readonly bool isAnonymous;

		private readonly string protocolSequence;

		private ClientSecurityContext clientSecurityContext;

		private NspiPrincipal nspiPrincipal;

		private IStandardBudget budget;

		private DisposeTracker disposeTracker;

		private ActivityScope scope;
	}
}
