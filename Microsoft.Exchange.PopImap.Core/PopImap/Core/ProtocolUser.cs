using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ProtocolUser
	{
		internal ProtocolUser(ProtocolSession session)
		{
			this.session = session;
		}

		public bool IsEnabled { get; private set; }

		public bool UseProtocolDefaults { get; private set; }

		public MimeTextFormat MessagesRetrievalMimeTextFormat { get; private set; }

		public string LegacyDistinguishedName { get; private set; }

		public string AcceptedDomain { get; private set; }

		public string LogonName { get; set; }

		public string UniqueName
		{
			get
			{
				if (this.LegacyDistinguishedName != null)
				{
					return this.LegacyDistinguishedName;
				}
				return this.LogonName;
			}
		}

		public string ConnectionIdentity { get; set; }

		public string PrimarySmtpAddress { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public bool EnableExactRFC822Size { get; private set; }

		public bool SuppressReadReceipt { get; private set; }

		public bool ForceICalForCalendarRetrievalOption { get; private set; }

		public ExDateTime MailboxLogTimeout { get; private set; }

		public bool LrsEnabled { get; private set; }

		public abstract ExEventLog.EventTuple UserExceededNumberOfConnectionsEventTuple { get; }

		public abstract ExEventLog.EventTuple OwaServerNotFoundEventTuple { get; }

		public abstract ExEventLog.EventTuple OwaServerInvalidEventTuple { get; }

		public void Reset()
		{
			this.LegacyDistinguishedName = null;
			this.LogonName = null;
			this.ConnectionIdentity = null;
		}

		public ADSessionSettings GetSessionSettings()
		{
			if (string.IsNullOrEmpty(this.AcceptedDomain))
			{
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			return ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(this.AcceptedDomain);
		}

		public override string ToString()
		{
			return string.Format("{0}, SMTP: \"{1}\", {2}, AcceptedDomain: \"{3}\"", new object[]
			{
				this.UniqueName,
				this.PrimarySmtpAddress,
				this.IsEnabled ? "enabled" : "disabled",
				this.AcceptedDomain
			});
		}

		internal ADUser FindAdUser(string userName, SecurityIdentifier userSid = null, string userPuid = null)
		{
			ADUser aduser = null;
			if (string.IsNullOrEmpty(userName))
			{
				return null;
			}
			string text = null;
			SmtpProxyAddress smtpProxyAddress = null;
			try
			{
				smtpProxyAddress = new SmtpProxyAddress(userName, true);
				this.AcceptedDomain = ((SmtpAddress)smtpProxyAddress).Domain;
			}
			catch (ArgumentOutOfRangeException)
			{
				string[] array = userName.Split("\\/".ToCharArray(), 2);
				if (array.Length == 2)
				{
					text = array[1];
					this.AcceptedDomain = array[0];
				}
				else
				{
					text = userName;
				}
			}
			ADSessionSettings sessionSettings = this.GetSessionSettings();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, 0, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 299, "FindAdUser", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\protocoluser.cs");
			ITenantRecipientSession tenantRecipientSession = tenantOrRootOrgRecipientSession as ITenantRecipientSession;
			if (userSid != null)
			{
				aduser = (tenantOrRootOrgRecipientSession.FindBySid(userSid) as ADUser);
				if (aduser == null)
				{
					ProtocolBaseServices.SessionTracer.TraceError<SecurityIdentifier>(this.session.SessionId, "FindBySid did not return AD obejct for user SID {0}", userSid);
					if (this.session.LightLogSession != null)
					{
						this.session.LightLogSession.ErrorMessage = "NoAdUserBySid";
					}
				}
			}
			else if (!string.IsNullOrEmpty(userPuid) && tenantRecipientSession != null)
			{
				ADRawEntry[] array2 = tenantRecipientSession.FindByNetID(userPuid, new PropertyDefinition[0]);
				if (array2.Length == 0)
				{
					ProtocolBaseServices.SessionTracer.TraceError<string>(this.session.SessionId, "FindByNetID did not return AD obejct for user PUID {0}", userPuid);
					if (this.session.LightLogSession != null)
					{
						this.session.LightLogSession.ErrorMessage = "NoAdUserByPuid";
					}
				}
				else if (array2.Length > 1)
				{
					ProtocolBaseServices.SessionTracer.TraceError<string, int>(this.session.SessionId, "FindByNetID found {1} AD obejcts for user PUID {0}", userPuid, array2.Length);
					if (this.session.LightLogSession != null)
					{
						this.session.LightLogSession.ErrorMessage = "TooManyAdUsersByPuid";
					}
				}
				else
				{
					aduser = (tenantOrRootOrgRecipientSession.Read(array2[0].Id) as ADUser);
				}
			}
			else if (smtpProxyAddress != null)
			{
				aduser = tenantOrRootOrgRecipientSession.FindByProxyAddress<ADUser>(smtpProxyAddress);
				if (aduser == null)
				{
					ProtocolBaseServices.SessionTracer.TraceError<SmtpProxyAddress>(this.session.SessionId, "FindByProxyAddress did not return AD obejct for user ProxyAddress {0}", smtpProxyAddress);
					if (this.session.LightLogSession != null)
					{
						this.session.LightLogSession.ErrorMessage = "NoAdUserByByProxyAddress";
					}
				}
			}
			else
			{
				aduser = (tenantOrRootOrgRecipientSession.FindByAccountName<ADUser>(this.AcceptedDomain, text) as ADUser);
				if (aduser == null)
				{
					ProtocolBaseServices.SessionTracer.TraceError<string, string>(this.session.SessionId, "FindByAccountName did not return AD obejct for user AccountName {0}, domain {1}", text, this.AcceptedDomain);
					if (this.session.LightLogSession != null)
					{
						this.session.LightLogSession.ErrorMessage = "NoAdUserByByAccountName";
					}
				}
			}
			return aduser;
		}

		internal void Configure(ADUser adUser, ADPropertyDefinition enabled, ADPropertyDefinition useProtocolDefaults, ADPropertyDefinition messagesRetrievalMimeFormat, ADPropertyDefinition enableExactRFC822Size, ADPropertyDefinition protocolLoggingEnabled, ADPropertyDefinition suppressReadReceipt, ADPropertyDefinition forceICalForCalendarRetrievalOption)
		{
			this.OrganizationId = adUser.OrganizationId;
			CASMailbox casmailbox = new CASMailbox(adUser);
			this.LegacyDistinguishedName = casmailbox.LegacyExchangeDN;
			this.PrimarySmtpAddress = casmailbox.PrimarySmtpAddress.ToString();
			this.IsEnabled = (bool)casmailbox[enabled];
			this.UseProtocolDefaults = (bool)casmailbox[useProtocolDefaults];
			this.MessagesRetrievalMimeTextFormat = (MimeTextFormat)casmailbox[messagesRetrievalMimeFormat];
			this.EnableExactRFC822Size = (bool)casmailbox[enableExactRFC822Size];
			this.SuppressReadReceipt = (bool)casmailbox[suppressReadReceipt];
			this.ForceICalForCalendarRetrievalOption = (bool)casmailbox[forceICalForCalendarRetrievalOption];
			object obj = casmailbox[protocolLoggingEnabled];
			if (obj == null)
			{
				this.MailboxLogTimeout = ExDateTime.MinValue;
			}
			else
			{
				this.MailboxLogTimeout = CASMailbox.MailboxProtocolLoggingInitialTime.AddMinutes((double)((int)obj)).AddHours(72.0);
			}
			if (ProtocolBaseServices.LrsLogEnabled)
			{
				this.LrsEnabled = true;
			}
		}

		protected internal abstract void Configure(ADUser adUser);

		private ProtocolSession session;
	}
}
