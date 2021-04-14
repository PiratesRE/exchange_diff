using System;
using System.Globalization;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.Providers
{
	internal abstract class Provider
	{
		private protected RequestData RequestData { protected get; private set; }

		private protected ProviderAttribute[] Attributes { protected get; private set; }

		private protected ADRecipient Caller { protected get; private set; }

		private protected ADRecipient RequestedRecipient { protected get; private set; }

		protected Provider(RequestData requestData)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "[Provider.base()] Timestamp=\"{0}\";CompterNameHash=\"{1}\";EmailAddress=\"{2}\";LegacyDN=\"{3}\";UserSID=\"{4}\";", new object[]
			{
				requestData.Timestamp,
				requestData.ComputerNameHash,
				requestData.EMailAddress,
				requestData.LegacyDN,
				requestData.User.Identity.GetSecurityIdentifier()
			});
			this.RequestData = requestData;
			Type type = base.GetType();
			Type typeFromHandle = typeof(ProviderAttribute);
			this.Attributes = (ProviderAttribute[])type.GetCustomAttributes(typeFromHandle, false);
			this.Caller = (HttpContext.Current.Items["CallerRecipient"] as ADRecipient);
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("Caller", (this.Caller == null) ? "null" : this.Caller.PrimarySmtpAddress.ToString());
		}

		public virtual void GenerateResponseXml(XmlWriter xmlFragment)
		{
			if (!this.WriteRedirectXml(xmlFragment))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "[base.GenerateResponseXml()] 'redirectOrError=false; 'Calling provider's WriteConfigXml()'");
				this.WriteConfigXml(xmlFragment);
			}
		}

		public virtual string Get302RedirectUrl()
		{
			return null;
		}

		protected virtual bool WriteRedirectXml(XmlWriter xmlFragment)
		{
			string ns = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006";
			Common.StartEnvelope(xmlFragment);
			xmlFragment.WriteStartElement("Response", ns);
			xmlFragment.WriteStartElement("User", ns);
			xmlFragment.WriteElementString("DisplayName", ns, "John Doe");
			xmlFragment.WriteEndElement();
			xmlFragment.WriteStartElement("Account", ns);
			xmlFragment.WriteElementString("AccountType", ns, "email");
			xmlFragment.WriteElementString("Action", ns, "redirect");
			xmlFragment.WriteElementString("RedirectURL", ns, "http://autodiscover.redirect.com");
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.UrlRedirect);
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			Common.EndEnvelope(xmlFragment);
			ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[base.WriteRedirectXml()] redirectUrl=\"{0}\";Assembly=\"{1}\"", "http://autodiscover.redirect.com", base.GetType().AssemblyQualifiedName);
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvRedirectionResponse, Common.PeriodicKey, new object[]
			{
				"http://autodiscover.redirect.com",
				this.RequestData.EMailAddress,
				this.RequestData.LegacyDN,
				base.GetType().AssemblyQualifiedName
			});
			return true;
		}

		protected abstract void WriteConfigXml(XmlWriter xmlFragment);

		protected void ResolveRequestedADRecipient()
		{
			string resolveMethod = "Unknown";
			try
			{
				if (this.Caller != null)
				{
					if (!string.IsNullOrEmpty(this.RequestData.LegacyDN) && string.Equals(this.Caller.LegacyExchangeDN, this.RequestData.LegacyDN, StringComparison.OrdinalIgnoreCase))
					{
						this.RequestedRecipient = this.Caller;
						resolveMethod = "CallerByLegacyDN";
						return;
					}
					if (!string.IsNullOrEmpty(this.RequestData.LegacyDN) && this.Caller.EmailAddresses != null)
					{
						string x500 = "x500:" + this.RequestData.LegacyDN;
						ProxyAddress a = this.Caller.EmailAddresses.Find((ProxyAddress x) => string.Equals(x.ToString(), x500, StringComparison.OrdinalIgnoreCase));
						if (a != null)
						{
							this.RequestedRecipient = this.Caller;
							resolveMethod = "CallerByX500";
							return;
						}
					}
					if (!string.IsNullOrEmpty(this.RequestData.EMailAddress) && SmtpAddress.IsValidSmtpAddress(this.RequestData.EMailAddress))
					{
						SmtpProxyAddress smtpProxy = new SmtpProxyAddress(this.RequestData.EMailAddress, true);
						ProxyAddress a2 = this.Caller.EmailAddresses.Find((ProxyAddress x) => x.Equals(smtpProxy));
						if (a2 != null)
						{
							this.RequestedRecipient = this.Caller;
							resolveMethod = "CallerByProxy";
							return;
						}
					}
					if (AutodiscoverCommonUserSettings.HasLocalArchive(this.Caller) && AutodiscoverCommonUserSettings.IsEmailAddressTargetingArchive(this.Caller as ADUser, this.RequestData.EMailAddress))
					{
						this.RequestedRecipient = this.Caller;
						resolveMethod = "CallerByArchive";
						return;
					}
				}
				if (this.Caller == null)
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.NoADLookupForUser.Enabled)
					{
						goto IL_285;
					}
				}
				try
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.RequestedUserADLatency, delegate()
					{
						IRecipientSession callerScopedRecipientSession = this.GetCallerScopedRecipientSession();
						if (!string.IsNullOrEmpty(this.RequestData.LegacyDN))
						{
							this.RequestedRecipient = callerScopedRecipientSession.FindByLegacyExchangeDN(this.RequestData.LegacyDN);
							if (this.RequestedRecipient != null)
							{
								resolveMethod = "FoundByLegacyDN";
							}
						}
						if (this.RequestedRecipient == null && this.RequestData.EMailAddress != null && SmtpAddress.IsValidSmtpAddress(this.RequestData.EMailAddress))
						{
							Guid guid;
							if (AutodiscoverCommonUserSettings.TryGetExchangeGuidFromEmailAddress(this.RequestData.EMailAddress, out guid))
							{
								this.RequestedRecipient = callerScopedRecipientSession.FindByExchangeGuidIncludingArchive(guid);
								ADUser aduser = this.RequestedRecipient as ADUser;
								if (aduser != null && aduser.ArchiveGuid.Equals(guid) && RemoteMailbox.IsRemoteMailbox(aduser.RecipientTypeDetails) && aduser.ArchiveDatabase == null)
								{
									this.RequestedRecipient = null;
								}
								if (this.RequestedRecipient != null)
								{
									resolveMethod = "FoundByGUID";
								}
							}
							if (this.RequestedRecipient == null)
							{
								SmtpProxyAddress proxyAddress = new SmtpProxyAddress(this.RequestData.EMailAddress, true);
								this.RequestedRecipient = callerScopedRecipientSession.FindByProxyAddress(proxyAddress);
								if (this.RequestedRecipient != null)
								{
									resolveMethod = "FoundBySMTP";
								}
							}
						}
					});
				}
				catch (LocalizedException ex)
				{
					ExTraceGlobals.FrameworkTracer.TraceError<string, string>(0L, "[UpdateCacheCallback()] 'LocalizedException' Message=\"{0}\";StackTrace=\"{1}\"", ex.Message, ex.StackTrace);
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
					{
						ex.Message,
						ex.StackTrace
					});
					resolveMethod = "Exception";
				}
				IL_285:;
			}
			finally
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ResolveMethod", resolveMethod);
			}
		}

		private IRecipientSession GetCallerScopedRecipientSession()
		{
			ADSessionSettings adsessionSettings = (this.Caller == null) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.Caller.OrganizationId);
			adsessionSettings.IncludeInactiveMailbox = true;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, this.GetQueryBaseDN(), CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 386, "GetCallerScopedRecipientSession", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Common\\Provider.cs");
			tenantOrRootOrgRecipientSession.ServerTimeout = new TimeSpan?(Common.RecipientLookupTimeout);
			tenantOrRootOrgRecipientSession.SessionSettings.AccountingObject = this.RequestData.Budget;
			return tenantOrRootOrgRecipientSession;
		}

		private ADObjectId GetQueryBaseDN()
		{
			ADUser aduser = this.Caller as ADUser;
			if (aduser == null)
			{
				return null;
			}
			return aduser.QueryBaseDN;
		}
	}
}
