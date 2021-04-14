using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.AirSync
{
	internal class UserInformationSetting : SettingsBase
	{
		public UserInformationSetting(XmlNode request, XmlNode response, IAirSyncUser user, MailboxSession mailboxSession, int version, ProtocolLogger protocolLogger) : base(request, response, protocolLogger)
		{
			this.user = user;
			this.mailboxSession = mailboxSession;
			this.version = version;
		}

		public override void Execute()
		{
			using (this.user.Context.Tracker.Start(TimeId.UserInformationSettingsExecute))
			{
				XmlNode firstChild = base.Request.FirstChild;
				base.ProtocolLogger.SetValue(ProtocolLoggerData.UserInformationVerb, firstChild.LocalName);
				string localName;
				if ((localName = firstChild.LocalName) != null && localName == "Get")
				{
					this.ProcessGet();
				}
			}
		}

		private void ProcessGet()
		{
			using (this.user.Context.Tracker.Start(TimeId.UserInformationSettingsProcessGet))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Processing UserInformationSetting - Get");
				XmlNode xmlNode = base.Response.OwnerDocument.CreateElement("Get", "Settings:");
				bool flag = this.version > 140;
				XmlNode xmlNode2 = null;
				XmlNode xmlNode3 = null;
				if (flag)
				{
					xmlNode2 = base.Response.OwnerDocument.CreateElement("Accounts", "Settings:");
					xmlNode3 = base.Response.OwnerDocument.CreateElement("Account", "Settings:");
				}
				XmlNode xmlNode4 = base.Response.OwnerDocument.CreateElement("EmailAddresses", "Settings:");
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.user.OrganizationId), 203, "ProcessGet", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\UserInformationSetting.cs");
				ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindByExchangeGuid(this.user.ADUser.ExchangeGuid);
				base.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrRootOrgRecipientSession.LastUsedDc);
				if (adrecipient != null && adrecipient.EmailAddresses != null)
				{
					using (MultiValuedProperty<ProxyAddress>.Enumerator enumerator = adrecipient.EmailAddresses.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ProxyAddress proxyAddress = enumerator.Current;
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Found proxy address {0}!", proxyAddress.AddressString);
							if (SmtpAddress.IsValidSmtpAddress(proxyAddress.AddressString))
							{
								if (this.version > 140 && proxyAddress.IsPrimaryAddress && proxyAddress.Prefix.DisplayName == "SMTP")
								{
									AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Found valid Primary Smtp proxy address {0}!", proxyAddress.AddressString);
									XmlNode xmlNode5 = base.Response.OwnerDocument.CreateElement("PrimarySmtpAddress", "Settings:");
									xmlNode5.InnerText = proxyAddress.AddressString;
									xmlNode4.AppendChild(xmlNode5);
								}
								else
								{
									AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Found valid Smtp proxy address {0}!", proxyAddress.AddressString);
									XmlNode xmlNode6 = base.Response.OwnerDocument.CreateElement("SmtpAddress", "Settings:");
									xmlNode6.InnerText = proxyAddress.AddressString;
									xmlNode4.AppendChild(xmlNode6);
								}
							}
						}
						goto IL_263;
					}
				}
				AirSyncDiagnostics.TraceDebug<bool, bool>(ExTraceGlobals.RequestsTracer, this, "fullRecipient fullRecipient.EmailAddresses is null! fullRecipient == null: {0}, fullRecipient.EmailAddresses == null : {1}", adrecipient == null, adrecipient != null && adrecipient.EmailAddresses == null);
				IL_263:
				if (flag)
				{
					xmlNode3.AppendChild(xmlNode4);
					xmlNode2.AppendChild(xmlNode3);
				}
				if (flag)
				{
					List<AggregationSubscription> allSubscriptions = SubscriptionManager.GetAllSubscriptions(this.mailboxSession, AggregationSubscriptionType.AllEMail);
					foreach (AggregationSubscription aggregationSubscription in allSubscriptions)
					{
						PimAggregationSubscription pimAggregationSubscription = aggregationSubscription as PimAggregationSubscription;
						if (pimAggregationSubscription == null)
						{
							AirSyncDiagnostics.TraceInfo<Guid>(ExTraceGlobals.RequestsTracer, this, "Found AggregationSubscription that was not a PimAggregationSubscription {0}!", pimAggregationSubscription.SubscriptionGuid);
						}
						else
						{
							AirSyncDiagnostics.TraceInfo<Guid>(ExTraceGlobals.RequestsTracer, this, "Found valid subscription {0}!", pimAggregationSubscription.SubscriptionGuid);
							XmlNode xmlNode7 = base.Response.OwnerDocument.CreateElement("Account", "Settings:");
							XmlNode xmlNode8 = base.Response.OwnerDocument.CreateElement("AccountId", "Settings:");
							xmlNode8.InnerText = pimAggregationSubscription.SubscriptionGuid.ToString();
							xmlNode7.AppendChild(xmlNode8);
							XmlNode xmlNode9 = base.Response.OwnerDocument.CreateElement("AccountName", "Settings:");
							xmlNode9.InnerText = pimAggregationSubscription.Name;
							xmlNode7.AppendChild(xmlNode9);
							XmlNode xmlNode10 = base.Response.OwnerDocument.CreateElement("UserDisplayName", "Settings:");
							xmlNode10.InnerText = pimAggregationSubscription.UserDisplayName;
							xmlNode7.AppendChild(xmlNode10);
							if (!pimAggregationSubscription.SendAsCapable || !SubscriptionManager.IsValidForSendAs(pimAggregationSubscription.SendAsState, pimAggregationSubscription.Status))
							{
								XmlNode newChild = base.Response.OwnerDocument.CreateElement("SendDisabled", "Settings:");
								xmlNode7.AppendChild(newChild);
							}
							XmlNode xmlNode11 = base.Response.OwnerDocument.CreateElement("PrimarySmtpAddress", "Settings:");
							xmlNode11.InnerText = pimAggregationSubscription.UserEmailAddress.ToString();
							xmlNode4 = base.Response.OwnerDocument.CreateElement("EmailAddresses", "Settings:");
							xmlNode4.AppendChild(xmlNode11);
							xmlNode7.AppendChild(xmlNode4);
							xmlNode2.AppendChild(xmlNode7);
						}
					}
				}
				XmlNode xmlNode12 = base.Response.OwnerDocument.CreateElement("Status", "Settings:");
				XmlNode xmlNode13 = xmlNode12;
				int num = (int)this.status;
				xmlNode13.InnerText = num.ToString(CultureInfo.InvariantCulture);
				base.Response.AppendChild(xmlNode12);
				if (flag)
				{
					xmlNode.AppendChild(xmlNode2);
				}
				else
				{
					xmlNode.AppendChild(xmlNode4);
				}
				base.Response.AppendChild(xmlNode);
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Done processing UserInformationSetting - Get.");
			}
		}

		private SettingsBase.ErrorCode status = SettingsBase.ErrorCode.Success;

		private IAirSyncUser user;

		private MailboxSession mailboxSession;

		private int version;
	}
}
