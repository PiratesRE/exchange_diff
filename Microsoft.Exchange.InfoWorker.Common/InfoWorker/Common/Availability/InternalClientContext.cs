using System;
using System.Globalization;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class InternalClientContext : ClientContext
	{
		internal InternalClientContext(ClientSecurityContext clientSecurityContext, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId, ADUser adUser) : base(budget, timeZone, clientCulture, messageId)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			this.clientSecurityContext = clientSecurityContext;
			this.ownsClientSecurityContext = false;
			if (adUser != null)
			{
				this.adUser = adUser;
				this.organizationId = adUser.OrganizationId;
				this.adUserInitialized = true;
			}
			else
			{
				this.adUser = null;
				this.adUserInitialized = false;
			}
			if (this.clientSecurityContext.UserSid != null)
			{
				this.identityForFilteredTracing = this.clientSecurityContext.UserSid.ToString();
			}
			SecurityAccessToken securityAccessToken = new SecurityAccessToken();
			this.clientSecurityContext.SetSecurityAccessToken(securityAccessToken);
			this.serializedSecurityContext = new SerializedSecurityContext(securityAccessToken);
		}

		internal InternalClientContext(ClientSecurityContext clientSecurityContext, OrganizationId organizationId, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId) : base(budget, timeZone, clientCulture, messageId)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			this.clientSecurityContext = clientSecurityContext;
			this.ownsClientSecurityContext = false;
			if (this.clientSecurityContext.UserSid != null)
			{
				this.identityForFilteredTracing = this.clientSecurityContext.UserSid.ToString();
			}
			SecurityAccessToken securityAccessToken = new SecurityAccessToken();
			this.clientSecurityContext.SetSecurityAccessToken(securityAccessToken);
			this.serializedSecurityContext = new SerializedSecurityContext(securityAccessToken);
			this.organizationId = organizationId;
			this.adUser = null;
			this.adUserInitialized = false;
		}

		private InternalClientContext(InternalClientContext clientContext, ClientSecurityContext clientSecurityContext, bool ownsClientSecurityContext, ExchangeVersionType requestSchemaVersion) : base(clientContext.Budget, clientContext.TimeZone, clientContext.ClientCulture, clientContext.MessageId)
		{
			this.clientSecurityContext = clientSecurityContext;
			this.ownsClientSecurityContext = ownsClientSecurityContext;
			this.adUser = clientContext.adUser;
			this.adUserInitialized = clientContext.adUserInitialized;
			this.organizationId = clientContext.OrganizationId;
			this.serializedSecurityContext = clientContext.serializedSecurityContext;
			this.identityForFilteredTracing = clientContext.identityForFilteredTracing;
			this.RequestSchemaVersion = requestSchemaVersion;
		}

		public ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		public ADUser ADUser
		{
			get
			{
				this.TryInitializeADUserIfNeeded();
				return this.adUser;
			}
		}

		public bool ADUserInitialized
		{
			get
			{
				return this.adUserInitialized;
			}
			set
			{
				this.adUserInitialized = value;
			}
		}

		public override OrganizationId OrganizationId
		{
			get
			{
				if (this.organizationId != null)
				{
					return this.organizationId;
				}
				if (this.ADUser != null)
				{
					return this.ADUser.OrganizationId;
				}
				return OrganizationId.ForestWideOrgId;
			}
		}

		public override ADObjectId QueryBaseDN
		{
			get
			{
				if (this.queryBaseDnSpecified)
				{
					return this.queryBaseDn;
				}
				if (this.ADUser != null)
				{
					return this.ADUser.QueryBaseDN;
				}
				return null;
			}
			set
			{
				this.queryBaseDn = value;
				this.queryBaseDnSpecified = true;
			}
		}

		public override ExchangeVersionType RequestSchemaVersion
		{
			get
			{
				return this.requestSchemaVersion;
			}
			set
			{
				this.requestSchemaVersion = value;
			}
		}

		public override string IdentityForFilteredTracing
		{
			get
			{
				return this.identityForFilteredTracing;
			}
		}

		public SerializedSecurityContext SerializedSecurityContext
		{
			get
			{
				return this.serializedSecurityContext;
			}
		}

		public override void ValidateContext()
		{
			if (this.clientSecurityContext.UserSid == null)
			{
				InternalClientContext.Tracer.TraceDebug<InternalClientContext>((long)this.GetHashCode(), "{0}: Internal caller sid is null", this);
				throw new InvalidClientSecurityContextException();
			}
		}

		public InternalClientContext Clone()
		{
			return new InternalClientContext(this, this.ClientSecurityContext.Clone(), true, this.RequestSchemaVersion);
		}

		public override void Dispose()
		{
			if (this.ownsClientSecurityContext && this.clientSecurityContext != null)
			{
				this.clientSecurityContext.Dispose();
				this.clientSecurityContext = null;
			}
		}

		public override string ToString()
		{
			return "InternalClientContext(" + this.clientSecurityContext.ToString() + ")";
		}

		private bool TryInitializeADUserIfNeeded()
		{
			Exception ex = null;
			try
			{
				this.InitializeADUserIfNeeded();
			}
			catch (LocalizedException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (ArgumentException ex4)
			{
				ex = ex4;
			}
			catch (FormatException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				InternalClientContext.Tracer.TraceError((long)this.GetHashCode(), "{0}: {1}: unable to find internal caller by SID {2} in the AD. Exception: {3}", new object[]
				{
					TraceContext.Get(),
					this,
					this.clientSecurityContext.UserSid,
					ex
				});
				return false;
			}
			return true;
		}

		private void InitializeADUserIfNeeded()
		{
			if (this.adUserInitialized)
			{
				return;
			}
			SecurityIdentifier userSid = this.clientSecurityContext.UserSid;
			base.CheckOverBudget();
			ADSessionSettings sessionSettings;
			if (this.organizationId != null)
			{
				sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId);
			}
			else
			{
				sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, sessionSettings, 397, "InitializeADUserIfNeeded", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\InternalClientContext.cs");
			if (base.Budget != null)
			{
				tenantOrRootOrgRecipientSession.SessionSettings.AccountingObject = base.Budget;
			}
			this.adUser = (tenantOrRootOrgRecipientSession.FindBySid(userSid) as ADUser);
			InternalClientContext.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: {1}: found internal caller by SID {2} in the AD. User is {3}", new object[]
			{
				TraceContext.Get(),
				this,
				userSid,
				this.adUser
			});
			this.adUserInitialized = true;
		}

		private readonly bool ownsClientSecurityContext;

		private ClientSecurityContext clientSecurityContext;

		private SerializedSecurityContext serializedSecurityContext;

		private ADUser adUser;

		private OrganizationId organizationId;

		private bool adUserInitialized;

		private string identityForFilteredTracing;

		private bool queryBaseDnSpecified;

		private ADObjectId queryBaseDn;

		private ExchangeVersionType requestSchemaVersion;

		private static readonly Trace Tracer = ExTraceGlobals.RequestRoutingTracer;
	}
}
