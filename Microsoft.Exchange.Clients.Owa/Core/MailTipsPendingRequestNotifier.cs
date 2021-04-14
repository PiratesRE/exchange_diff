using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MailTips;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class MailTipsPendingRequestNotifier : IPendingRequestNotifier
	{
		public bool ShouldThrottle
		{
			get
			{
				return false;
			}
		}

		public event DataAvailableEventHandler DataAvailable;

		public string ReadDataAndResetState()
		{
			string result;
			lock (this.payload)
			{
				if (0 < this.payload.Count)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<int>((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.ReadDataAndResetState. Processing {0} requests.", this.payload.Count);
					int capacity = 512 * this.payload.Count;
					StringBuilder stringBuilder = new StringBuilder(capacity);
					for (int i = 1; i <= this.payload.Count; i++)
					{
						MailTipsState mailTipsState = this.payload.Dequeue();
						ExTraceGlobals.CoreCallTracer.TraceDebug<int, string, ProxyAddress>((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.ReadDataAndResetState. Request: {0}, Requester: {1}, Sending as: {2}.", i, mailTipsState.LogonUserPrimarySmtpAddress, mailTipsState.SendingAs);
						stringBuilder.Append("processMailTipsResponse({");
						bool flag2 = true;
						foreach (MailTips mailTips in mailTipsState.MailTipsResult)
						{
							if (!flag2)
							{
								stringBuilder.Append(",");
							}
							flag2 = false;
							MailTipsPendingRequestNotifier.SerializeMailTips(mailTips, mailTipsState, stringBuilder);
						}
						if (mailTipsState.Budget != null)
						{
							mailTipsState.Budget.Dispose();
						}
						stringBuilder.Append("}");
						stringBuilder.Append(",{");
						stringBuilder.Append("'fHideByDefault' : ");
						stringBuilder.Append(mailTipsState.ShouldHideByDefault ? 1 : 0);
						if (mailTipsState.DoesNeedConfig)
						{
							Organization configuration = mailTipsState.CachedOrganizationConfiguration.OrganizationConfiguration.Configuration;
							stringBuilder.Append(", 'fEnabled' : ");
							stringBuilder.Append(configuration.MailTipsAllTipsEnabled ? 1 : 0);
							stringBuilder.Append(", 'fMailboxEnabled' : ");
							stringBuilder.Append(configuration.MailTipsMailboxSourcedTipsEnabled ? 1 : 0);
							stringBuilder.Append(", 'fGroupMetricsEnabled' : ");
							stringBuilder.Append(configuration.MailTipsGroupMetricsEnabled ? 1 : 0);
							stringBuilder.Append(", 'fExternalEnabled' : ");
							stringBuilder.Append(configuration.MailTipsExternalRecipientsTipsEnabled ? 1 : 0);
							stringBuilder.Append(", 'iLargeAudienceThreshold' : ");
							stringBuilder.Append(configuration.MailTipsLargeAudienceThreshold);
						}
						stringBuilder.Append("}");
						stringBuilder.Append(");");
						if (mailTipsState.GetMailTipsQuery != null)
						{
							if (mailTipsState.RequestLogger == null)
							{
								mailTipsState.RequestLogger = new RequestLogger();
							}
						}
						else
						{
							ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.ReadDataAndResetState: GetMailTipsQuery was null.");
						}
						if (mailTipsState.PendingRequestManager.ChunkedHttpResponse != null)
						{
							mailTipsState.PendingRequestManager.ChunkedHttpResponse.Log(mailTipsState.RequestLogger);
						}
					}
					this.isPickupInProgress = false;
					result = stringBuilder.ToString();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public void AddToPayload(MailTipsState mailTipsState)
		{
			lock (this.payload)
			{
				if (256 == this.payload.Count)
				{
					this.payload.Dequeue();
				}
				this.payload.Enqueue(mailTipsState);
			}
		}

		public void PickupData()
		{
			bool flag = false;
			lock (this.payload)
			{
				if (this.isPickupInProgress)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.PickupData. No need to call DataAvailable method, data pickup is already in progress.");
				}
				else if (this.payload.Count == 0)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.PickupData. No data available to be picked up.");
				}
				else
				{
					this.isPickupInProgress = true;
					flag = true;
				}
			}
			if (flag)
			{
				this.DataAvailable(this, new EventArgs());
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.PickupData. Pickup is in progress.");
			}
		}

		public void ConnectionAliveTimer()
		{
		}

		internal static void SerializeMailTips(MailTips mailTips, MailTipsState mailTipsState, StringBuilder stringBuilder)
		{
			stringBuilder.Append("'");
			stringBuilder.Append(Utilities.JavascriptEncode(mailTips.EmailAddress.Address));
			stringBuilder.Append("' : ");
			stringBuilder.Append("{");
			stringBuilder.Append("'iSize' : ");
			stringBuilder.Append(mailTips.TotalMemberCount);
			stringBuilder.Append(", ");
			stringBuilder.Append("'iExternalSize' : ");
			stringBuilder.Append(mailTips.ExternalMemberCount);
			if (!string.IsNullOrEmpty(mailTips.CustomMailTip))
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'sCustomTip' : '");
				stringBuilder.Append(Utilities.JavascriptEncode(Utilities.RemoveHtmlComments(mailTips.CustomMailTip)));
				stringBuilder.Append("'");
			}
			if (!string.IsNullOrEmpty(mailTips.OutOfOfficeMessage))
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'sAutoReplyMessage' : '");
				stringBuilder.Append(Utilities.JavascriptEncode(Utilities.RemoveHtmlComments(mailTips.OutOfOfficeMessage)));
				stringBuilder.Append("'");
			}
			if (mailTips.OutOfOfficeDuration != null && DateTime.MinValue != mailTips.OutOfOfficeDuration.EndTime)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'sAutoReplyEndDate' : '");
				ExDateTime exDateTime = new ExDateTime(mailTipsState.LogonUserTimeZone, mailTips.OutOfOfficeDuration.EndTime);
				stringBuilder.Append(Utilities.JavascriptEncode(exDateTime.ToString(mailTipsState.WeekdayDateTimeFormat, mailTipsState.LogonUserCulture)));
				stringBuilder.Append("'");
			}
			if (mailTips.IsModerated)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'fModerated' : 1");
			}
			if (mailTips.MailboxFull)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'fFull' : 1");
			}
			if (null != mailTipsState.SendingAs && !string.IsNullOrEmpty(mailTipsState.SendingAs.AddressString))
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'oRestricted' : {'");
				stringBuilder.Append(Utilities.JavascriptEncode(mailTipsState.SendingAs.AddressString));
				stringBuilder.Append("' : ");
				stringBuilder.Append(mailTips.DeliveryRestricted ? "1}" : "0}");
			}
			if (mailTips.Exception != null)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append("'fErrored' : 1");
			}
			stringBuilder.Append("}");
		}

		internal void RegisterWithPendingRequestManager(UserContext userContext)
		{
			if (userContext != null && userContext.PendingRequestManager != null)
			{
				userContext.PendingRequestManager.AddPendingRequestNotifier(this);
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "MailTipsPendingRequestNotifier.RegisterWithPendingRequestManager. Cannot register because the pending request manager is null.");
		}

		internal MailTipsPendingRequestNotifier()
		{
			this.payload = new Queue<MailTipsState>(16);
		}

		private const int DefaultQueueCapacity = 16;

		private const int MaxQueueCapacity = 256;

		private const int AverageMailTipsLength = 512;

		private Queue<MailTipsState> payload;

		private bool isPickupInProgress;
	}
}
