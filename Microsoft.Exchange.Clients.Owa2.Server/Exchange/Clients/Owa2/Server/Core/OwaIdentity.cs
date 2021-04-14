using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public abstract class OwaIdentity : DisposeTrackableBase
	{
		internal static OwaIdentity ResolveLogonIdentity(HttpContext httpContext, AuthZClientInfo effectiveCaller)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			OwaIdentity owaIdentity;
			if (effectiveCaller != null && effectiveCaller.ClientSecurityContext != null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "[OwaIdentity::ResolveLogonIdentity] - Taking identity from overrideClientSecurityContext. User: {0}.", effectiveCaller.PrimarySmtpAddress);
				owaIdentity = OwaCompositeIdentity.CreateFromAuthZClientInfo(effectiveCaller);
			}
			else
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Looking for identity on httpContext.");
				IIdentity userIdentity = CompositeIdentityBuilder.GetUserIdentity(httpContext);
				if (userIdentity == null)
				{
					ExTraceGlobals.CoreCallTracer.TraceError(0L, "[OwaIdentity::ResolveLogonIdentity] - httpContext was passed without an identity");
					throw new OwaIdentityException("The httpContext must have an identity associated with it.");
				}
				owaIdentity = OwaIdentity.GetOwaIdentity(userIdentity);
			}
			if (owaIdentity != null)
			{
				string logonName = owaIdentity.GetLogonName();
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] Successfully resolved logon identity. Type={0}, AuthType={1}, Name={2}, IsPartial={3}", new object[]
				{
					owaIdentity.GetType(),
					owaIdentity.AuthenticationType ?? string.Empty,
					logonName ?? string.Empty,
					owaIdentity.IsPartial
				});
				return owaIdentity;
			}
			ExTraceGlobals.CoreCallTracer.TraceError(0L, "[OwaIdentity::ResolveLogonIdentity] - was unable to create the security context.");
			throw new OwaIdentityException("Cannot create security context for the specified identity.");
		}

		protected static OwaIdentity GetOwaIdentity(IIdentity identity)
		{
			CompositeIdentity compositeIdentity = identity as CompositeIdentity;
			if (compositeIdentity != null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve CompositeIdentity.");
				return OwaCompositeIdentity.CreateFromCompositeIdentity(compositeIdentity);
			}
			WindowsIdentity windowsIdentity = identity as WindowsIdentity;
			if (windowsIdentity != null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve WindowsIdentity.");
				if (windowsIdentity.IsAnonymous)
				{
					ExTraceGlobals.CoreCallTracer.TraceError(0L, "[OwaIdentity::ResolveLogonIdentity] - Windows identity cannot be anonymous.");
					throw new OwaIdentityException("Cannot create security context for anonymous windows identity.");
				}
				return OwaWindowsIdentity.CreateFromWindowsIdentity(windowsIdentity);
			}
			else
			{
				LiveIDIdentity liveIDIdentity = identity as LiveIDIdentity;
				if (liveIDIdentity != null)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve LiveIDIdentity.");
					return OwaClientSecurityContextIdentity.CreateFromLiveIDIdentity(liveIDIdentity);
				}
				WindowsTokenIdentity windowsTokenIdentity = identity as WindowsTokenIdentity;
				if (windowsTokenIdentity != null)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve WindowsTokenIdentity.");
					return OwaClientSecurityContextIdentity.CreateFromClientSecurityContextIdentity(windowsTokenIdentity);
				}
				OAuthIdentity oauthIdentity = identity as OAuthIdentity;
				if (oauthIdentity != null)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve OAuthIdentity.");
					return OwaClientSecurityContextIdentity.CreateFromOAuthIdentity(oauthIdentity);
				}
				AdfsIdentity adfsIdentity = identity as AdfsIdentity;
				if (adfsIdentity != null)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve AdfsIdentity.");
					return OwaClientSecurityContextIdentity.CreateFromAdfsIdentity(identity as AdfsIdentity);
				}
				SidBasedIdentity sidBasedIdentity = identity as SidBasedIdentity;
				if (sidBasedIdentity != null)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[OwaIdentity::ResolveLogonIdentity] - Trying to resolve SidBasedIdentity.");
					return OwaClientSecurityContextIdentity.CreateFromsidBasedIdentity(sidBasedIdentity);
				}
				ExTraceGlobals.CoreCallTracer.TraceError<Type>(0L, "[OwaIdentity::ResolveLogonIdentity] - Cannot resolve unsupported identity type: {0}.", identity.GetType());
				throw new NotSupportedException(string.Format("Unexpected identity type. {0}", identity.GetType()));
			}
		}

		public abstract WindowsIdentity WindowsIdentity { get; }

		public OWAMiniRecipient OwaMiniRecipient { get; set; }

		public virtual string DomainName
		{
			get
			{
				if (string.IsNullOrEmpty(this.domainName))
				{
					string logonName = this.GetLogonName();
					string text;
					if (!OwaIdentity.TryParseDomainFromLogonName(logonName, out text))
					{
						ExTraceGlobals.CoreCallTracer.TraceError<string>(0L, "Unable to parse domain name from logon name '{0}'.", logonName);
						throw new OwaIdentityException(string.Format(CultureInfo.InvariantCulture, "Could not get a valid domain name from the identity '{0}'.", new object[]
						{
							logonName ?? "<NULL>"
						}));
					}
					this.domainName = text;
				}
				return this.domainName;
			}
		}

		protected static bool IsLogonNameFullyQualified(string logonName)
		{
			string text;
			return OwaIdentity.TryParseDomainFromLogonName(logonName, out text);
		}

		internal static bool TryParseDomainFromLogonName(string logonName, out string domainName)
		{
			domainName = null;
			if (!string.IsNullOrEmpty(logonName))
			{
				int num = logonName.IndexOf('\\');
				if (num > 0)
				{
					domainName = logonName.Substring(0, num);
				}
				else
				{
					SmtpAddress smtpAddress = new SmtpAddress(logonName);
					if (smtpAddress.IsValidAddress)
					{
						domainName = smtpAddress.Domain;
					}
				}
			}
			return !string.IsNullOrEmpty(domainName);
		}

		public OrganizationId UserOrganizationId { get; set; }

		public abstract SecurityIdentifier UserSid { get; }

		public abstract string AuthenticationType { get; }

		public abstract string UniqueId { get; }

		public abstract bool IsPartial { get; }

		public virtual SmtpAddress PrimarySmtpAddress
		{
			get
			{
				if (this.OwaMiniRecipient == null)
				{
					return default(SmtpAddress);
				}
				return this.OwaMiniRecipient.PrimarySmtpAddress;
			}
		}

		public OrganizationProperties UserOrganizationProperties
		{
			get
			{
				if (this.userOrganizationProperties == null)
				{
					OWAMiniRecipient owaminiRecipient = this.GetOWAMiniRecipient();
					if (!OrganizationPropertyCache.TryGetOrganizationProperties(owaminiRecipient.OrganizationId, out this.userOrganizationProperties))
					{
						throw new OwaADObjectNotFoundException("The organization does not exist in AD. OrgId:" + owaminiRecipient.OrganizationId);
					}
				}
				return this.userOrganizationProperties;
			}
		}

		internal abstract ClientSecurityContext ClientSecurityContext { get; }

		public abstract string GetLogonName();

		public abstract string SafeGetRenderableName();

		public virtual void Refresh(OwaIdentity identity)
		{
			if (identity.GetType() != base.GetType())
			{
				throw new OwaInvalidOperationException(string.Format("Type of passed in identity does not match current identity.  Expected {0} but got {1}.", base.GetType(), identity.GetType()));
			}
		}

		public virtual bool IsEqualsTo(OwaIdentity otherIdentity)
		{
			return otherIdentity != null && otherIdentity.UserSid.Equals(this.UserSid);
		}

		public ADRecipient CreateADRecipientBySid()
		{
			IRecipientSession recipientSession = (this.UserOrganizationId == null) ? UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, this.DomainName, null) : UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, null, this.UserOrganizationId);
			ADRecipient adrecipient = recipientSession.FindBySid(this.UserSid);
			if (adrecipient == null)
			{
				throw new OwaADUserNotFoundException(this.SafeGetRenderableName());
			}
			return adrecipient;
		}

		public OWAMiniRecipient GetOWAMiniRecipient()
		{
			if (this.OwaMiniRecipient == null)
			{
				this.OwaMiniRecipient = this.CreateOWAMiniRecipientBySid();
			}
			return this.OwaMiniRecipient;
		}

		public OWAMiniRecipient CreateOWAMiniRecipientBySid()
		{
			IRecipientSession recipientSession = (this.UserOrganizationId == null) ? UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, this.DomainName, null) : UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, null, this.UserOrganizationId);
			bool flag = false;
			bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaServer.OwaClientAccessRulesEnabled.Enabled;
			if (enabled)
			{
				ClientAccessRuleCollection collection = ClientAccessRulesCache.Instance.GetCollection(this.UserOrganizationId ?? OrganizationId.ForestWideOrgId);
				flag = (collection.Count > 0);
			}
			OWAMiniRecipient owaminiRecipient = recipientSession.FindMiniRecipientBySid<OWAMiniRecipient>(this.UserSid, flag ? OWAMiniRecipientSchema.AdditionalPropertiesWithClientAccessRules : OWAMiniRecipientSchema.AdditionalProperties);
			if (owaminiRecipient == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "OwaIdentity.CreateOWAMiniRecipientBySid: got null OWAMiniRecipient for Sid: {0}", this.UserSid);
				throw new OwaADUserNotFoundException(this.SafeGetRenderableName());
			}
			return owaminiRecipient;
		}

		internal static OwaIdentity CreateOwaIdentityFromExplicitLogonAddress(string smtpAddress)
		{
			OwaIdentity result = OwaMiniRecipientIdentity.CreateFromProxyAddress(smtpAddress);
			ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "The request is under explicit logon: {0}", smtpAddress);
			return result;
		}

		internal ExchangePrincipal CreateExchangePrincipal()
		{
			ExchangePrincipal exchangePrincipal = null;
			try
			{
				if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					string text = null;
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						text = current.Name;
					}
					if (string.IsNullOrEmpty(text))
					{
						text = "<n/a>";
					}
					string arg = this.SafeGetRenderableName();
					ExTraceGlobals.CoreTracer.TraceDebug<string, string>(0L, "Using accout {0} to bind to ExchangePrincipal object for user {1}", text, arg);
				}
				exchangePrincipal = this.InternalCreateExchangePrincipal();
			}
			catch (AdUserNotFoundException innerException)
			{
				throw new OwaADUserNotFoundException(this.SafeGetRenderableName(), null, innerException);
			}
			catch (ObjectNotFoundException ex)
			{
				bool flag = false;
				DataValidationException ex2 = ex.InnerException as DataValidationException;
				if (ex2 != null)
				{
					PropertyValidationError propertyValidationError = ex2.Error as PropertyValidationError;
					if (propertyValidationError != null && propertyValidationError.PropertyDefinition == MiniRecipientSchema.Languages)
					{
						OWAMiniRecipient owaminiRecipient = this.FixCorruptOWAMiniRecipientCultureEntry();
						if (owaminiRecipient != null)
						{
							try
							{
								exchangePrincipal = ExchangePrincipal.FromMiniRecipient(owaminiRecipient);
								ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "OwaIdentity.CreateExchangePrincipal: Got ExchangePrincipal from MiniRecipient for Sid: {0}", this.UserSid);
								flag = true;
							}
							catch (ObjectNotFoundException)
							{
							}
						}
					}
				}
				if (!flag)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier, ObjectNotFoundException>(0L, "OwaIdentity.CreateExchangePrincipal: Fail to create ExchangePrincipal for Sid: {0}. Cannot recover from exception: {1}", this.UserSid, ex);
					throw ex;
				}
			}
			if (exchangePrincipal == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "OwaIdentity.CreateExchangePrincipal: Got a null ExchangePrincipal for Sid: {0}", this.UserSid);
			}
			return exchangePrincipal;
		}

		internal abstract ExchangePrincipal InternalCreateExchangePrincipal();

		internal abstract MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo);

		internal abstract MailboxSession CreateInstantSearchMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo);

		internal abstract MailboxSession CreateDelegateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo);

		internal OWAMiniRecipient FixCorruptOWAMiniRecipientCultureEntry()
		{
			if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0} has corrupt culture, setting client culture to empty", this.SafeGetRenderableName());
			}
			IRecipientSession recipientSession = (this.UserOrganizationId == null) ? UserContextUtilities.CreateScopedRecipientSession(false, ConsistencyMode.PartiallyConsistent, this.DomainName, null) : UserContextUtilities.CreateScopedRecipientSession(false, ConsistencyMode.PartiallyConsistent, null, this.UserOrganizationId);
			ADUser aduser = recipientSession.FindBySid(this.UserSid) as ADUser;
			if (aduser != null)
			{
				aduser.Languages = new MultiValuedProperty<CultureInfo>();
				if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Saving culture for User {0}, setting client culture to empty", this.SafeGetRenderableName());
				}
				recipientSession.Save(aduser);
				return recipientSession.FindMiniRecipientBySid<OWAMiniRecipient>(this.UserSid, OWAMiniRecipientSchema.AdditionalProperties);
			}
			ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "OwaIdentity.FixCorruptOWAMiniRecipientCultureEntry: got null adUser for Sid: {0}", this.UserSid);
			return null;
		}

		internal bool IsCrossForest(SecurityIdentifier masterAccountSid)
		{
			return this.UserSid != null && this.UserSid.Equals(masterAccountSid);
		}

		protected override void InternalDispose(bool isDisposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaIdentity>(this);
		}

		private string domainName;

		private OrganizationProperties userOrganizationProperties;
	}
}
