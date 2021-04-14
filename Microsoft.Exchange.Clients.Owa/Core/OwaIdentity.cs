using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public abstract class OwaIdentity : DisposeTrackableBase
	{
		internal string LastRecipientSessionDCServerName { get; set; }

		internal static OwaIdentity CreateOwaIdentityFromSmtpAddress(OwaIdentity logonIdentity, string smtpAddress, out ExchangePrincipal logonExchangePrincipal, out bool isExplicitLogon, out bool isAlternateMailbox)
		{
			OwaIdentity owaIdentity = null;
			isAlternateMailbox = false;
			isExplicitLogon = false;
			logonExchangePrincipal = null;
			try
			{
				logonExchangePrincipal = logonIdentity.CreateExchangePrincipal();
				Guid? alternateMailbox = OwaAlternateMailboxIdentity.GetAlternateMailbox(logonExchangePrincipal, smtpAddress);
				if (alternateMailbox != null)
				{
					owaIdentity = OwaAlternateMailboxIdentity.Create(logonIdentity, logonExchangePrincipal, alternateMailbox.Value);
					isAlternateMailbox = true;
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "The request is under alternate mailbox: {0}", smtpAddress);
				}
				else
				{
					isExplicitLogon = true;
				}
			}
			catch (AdUserNotFoundException)
			{
				isExplicitLogon = true;
			}
			catch (UserHasNoMailboxException)
			{
				isExplicitLogon = true;
			}
			if (isExplicitLogon)
			{
				if (owaIdentity != null)
				{
					owaIdentity.Dispose();
					owaIdentity = null;
				}
				owaIdentity = OwaMiniRecipientIdentity.CreateFromProxyAddress(smtpAddress);
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "The request is under explicit logon: {0}", smtpAddress);
			}
			return owaIdentity;
		}

		public virtual OrganizationProperties UserOrganizationProperties
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

		public abstract WindowsIdentity WindowsIdentity { get; }

		public abstract SecurityIdentifier UserSid { get; }

		public abstract string AuthenticationType { get; }

		public abstract string GetLogonName();

		public abstract string SafeGetRenderableName();

		public abstract string UniqueId { get; }

		internal abstract ClientSecurityContext ClientSecurityContext { get; }

		public abstract bool IsPartial { get; }

		internal abstract ExchangePrincipal InternalCreateExchangePrincipal();

		internal abstract MailboxSession CreateMailboxSession(IExchangePrincipal exchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest);

		internal abstract MailboxSession CreateWebPartMailboxSession(IExchangePrincipal mailBoxExchangePrincipal, CultureInfo cultureInfo, HttpRequest clientRequest);

		internal abstract UncSession CreateUncSession(DocumentLibraryObjectId objectId);

		internal abstract SharepointSession CreateSharepointSession(DocumentLibraryObjectId objectId);

		public virtual void Refresh(OwaIdentity identity)
		{
			if (identity.GetType() != base.GetType())
			{
				throw new OwaInvalidOperationException(string.Format("Type of passed in identity does not match current identity.  Expected {0} but got {1}.", base.GetType(), identity.GetType()));
			}
		}

		public virtual SmtpAddress PrimarySmtpAddress
		{
			get
			{
				if (this.owaMiniRecipient == null)
				{
					return default(SmtpAddress);
				}
				return this.owaMiniRecipient.PrimarySmtpAddress;
			}
		}

		public virtual string DomainName
		{
			get
			{
				if (string.IsNullOrEmpty(this.domainName))
				{
					string text = this.SafeGetRenderableName();
					if (!string.IsNullOrEmpty(text))
					{
						int num = text.IndexOf('\\');
						if (num > 0)
						{
							return text.Substring(0, num);
						}
						if (text.IndexOf('@') >= 0)
						{
							SmtpAddress smtpAddress = new SmtpAddress(text);
							if (smtpAddress.IsValidAddress)
							{
								return smtpAddress.Domain;
							}
						}
					}
				}
				return this.domainName;
			}
			set
			{
				this.domainName = value;
			}
		}

		public virtual bool IsEqualsTo(OwaIdentity otherIdentity)
		{
			return otherIdentity != null && otherIdentity.UserSid.Equals(this.UserSid);
		}

		protected override void InternalDispose(bool isDisposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaIdentity>(this);
		}

		internal ExchangePrincipal CreateExchangePrincipal()
		{
			ExchangePrincipal result = null;
			try
			{
				OwaDiagnostics.TracePfd(18057, "Creating new ExchangePrincipal", new object[0]);
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
				result = this.InternalCreateExchangePrincipal();
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
								result = ExchangePrincipal.FromMiniRecipient(owaminiRecipient, RemotingOptions.AllowCrossSite);
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
					throw ex;
				}
			}
			return result;
		}

		internal OWAMiniRecipient FixCorruptOWAMiniRecipientCultureEntry()
		{
			if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0} has corrupt culture, setting client culture to empty", this.SafeGetRenderableName());
			}
			IRecipientSession recipientSession = Utilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, this.DomainName);
			ADUser aduser = recipientSession.FindBySid(this.UserSid) as ADUser;
			this.LastRecipientSessionDCServerName = recipientSession.LastUsedDc;
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
			return null;
		}

		protected internal void ThrowNotSupported(string methodName)
		{
			string message = string.Format("This type of identity ({0}) doesn't support {1}", base.GetType().ToString(), methodName);
			throw new OwaNotSupportedException(message);
		}

		public ADRecipient CreateADRecipientBySid()
		{
			IRecipientSession recipientSession = Utilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, this.DomainName);
			ADRecipient adrecipient = recipientSession.FindBySid(this.UserSid);
			this.LastRecipientSessionDCServerName = recipientSession.LastUsedDc;
			if (adrecipient == null)
			{
				throw new OwaADObjectNotFoundException();
			}
			return adrecipient;
		}

		public OWAMiniRecipient GetOWAMiniRecipient()
		{
			if (this.owaMiniRecipient == null)
			{
				this.owaMiniRecipient = this.CreateOWAMiniRecipientBySid();
			}
			return this.owaMiniRecipient;
		}

		public OWAMiniRecipient CreateOWAMiniRecipientBySid()
		{
			IRecipientSession recipientSession = Utilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, this.DomainName);
			OWAMiniRecipient owaminiRecipient = recipientSession.FindMiniRecipientBySid<OWAMiniRecipient>(this.UserSid, OWAMiniRecipientSchema.AdditionalProperties);
			this.LastRecipientSessionDCServerName = recipientSession.LastUsedDc;
			if (owaminiRecipient == null)
			{
				throw new OwaADObjectNotFoundException();
			}
			return owaminiRecipient;
		}

		public bool IsCrossForest(SecurityIdentifier masterAccountSid)
		{
			return this.UserSid != null && this.UserSid.Equals(masterAccountSid);
		}

		protected ADUser CreateADUserBySid()
		{
			ADUser aduser = this.CreateADRecipientBySid() as ADUser;
			if (aduser == null)
			{
				throw new OwaExplicitLogonException(string.Format("The SID {0} is an object in AD database but it is not an user", this.UserSid), LocalizedStrings.GetNonEncoded(-1332692688), null);
			}
			return aduser;
		}

		internal IADOrgPerson CreateADOrgPersonForWebPartUserBySid()
		{
			IADOrgPerson iadorgPerson = this.CreateADRecipientBySid() as IADOrgPerson;
			if (iadorgPerson == null)
			{
				throw new OwaExplicitLogonException(string.Format("The SID {0} is an object in AD database but it is not an ADOrgPerson, which is required for web part delegate access", this.UserSid), LocalizedStrings.GetNonEncoded(-1332692688), null);
			}
			return iadorgPerson;
		}

		protected OWAMiniRecipient owaMiniRecipient;

		protected OrganizationProperties userOrganizationProperties;

		protected string domainName;
	}
}
