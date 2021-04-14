using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MailTips;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class MailTipsNotificationHandler
	{
		public MailTipsNotificationHandler(UserContext userContext)
		{
			this.userContext = userContext;
			this.mailTipsNotifier = new MailTipsPendingRequestNotifier();
			this.mailTipsNotifier.RegisterWithPendingRequestManager(userContext);
		}

		internal IAsyncResult BeginGetMailTipsInBatches(RecipientInfo[] recipientsInfo, RecipientInfo senderInfo, bool doesNeedConfig, AsyncCallback asyncCallback, object asyncCallbackData)
		{
			this.primarySmtpAddress = this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			ClientSecurityContext clientSecurityContext = this.userContext.LogonIdentity.ClientSecurityContext.Clone();
			string weekdayDateTimeFormat = this.userContext.UserOptions.GetWeekdayDateTimeFormat(true);
			MailTipsState mailTipsState = new MailTipsState(recipientsInfo, senderInfo, doesNeedConfig, this.userContext.ExchangePrincipal.LegacyDn, this.primarySmtpAddress, clientSecurityContext, this.userContext.TimeZone, this.userContext.UserCulture, this.userContext.ExchangePrincipal.MailboxInfo.OrganizationId, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, this.userContext.UserOptions.HideMailTipsByDefault, this.userContext.PendingRequestManager, Query<IEnumerable<MailTips>>.GetCurrentHttpRequestServerName(), weekdayDateTimeFormat);
			OwaAsyncResult owaAsyncResult = new OwaAsyncResult(asyncCallback, asyncCallbackData);
			Interlocked.Increment(ref this.concurrentRequestCount);
			if (3 >= this.concurrentRequestCount)
			{
				ExTraceGlobals.CoreCallTracer.TraceError<int, string>((long)this.GetHashCode(), "MailTipsNotificationHandler.BeginGetMailTipsInBatches, serving concurrent request {0} for {1}", this.concurrentRequestCount, this.primarySmtpAddress);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.GetMailTipsWorker), mailTipsState);
				return owaAsyncResult;
			}
			IAsyncResult result;
			try
			{
				ExTraceGlobals.CoreCallTracer.TraceError<int>((long)this.GetHashCode(), "MailTipsNotificationHandler.BeginGetMailTipsInBatches, maximum concurrent request limit {0} has been reached", 3);
				MailTipsNotificationHandler.PopulateException(mailTipsState, new OwaMaxConcurrentRequestsExceededException("Maximum MailTips concurrent requests exceeded"), this.GetHashCode());
				this.mailTipsNotifier.AddToPayload(mailTipsState);
				this.mailTipsNotifier.PickupData();
				result = owaAsyncResult;
			}
			finally
			{
				Interlocked.Decrement(ref this.concurrentRequestCount);
			}
			return result;
		}

		internal void EndGetMailTipsInBatches(IAsyncResult asyncResult)
		{
			OwaAsyncResult owaAsyncResult = (OwaAsyncResult)asyncResult;
			if (owaAsyncResult.Exception != null)
			{
				ExTraceGlobals.CoreCallTracer.TraceError<Exception>((long)this.GetHashCode(), "MailTipsNotificationHandler.EndGetMailTipsInBatches, exception {0}", owaAsyncResult.Exception);
			}
		}

		private static ProxyAddress[] GetNextBatch(RecipientInfo[] recipientsInfo, ref int index)
		{
			int num = recipientsInfo.Length - index;
			if (50 < num)
			{
				num = 50;
			}
			ProxyAddress[] array = new ProxyAddress[num];
			int i = 0;
			while (i < num)
			{
				array[i] = recipientsInfo[index].ToProxyAddress();
				i++;
				index++;
			}
			return array;
		}

		private static void PopulateException(MailTipsState mailTipsState, Exception exception, int hashCode)
		{
			ExTraceGlobals.CoreCallTracer.TraceError<Exception>((long)hashCode, "MailTipsNotificationHandler.PopulateException: {0}", exception);
			List<MailTips> mailTipsResult = mailTipsState.MailTipsResult;
			RecipientInfo[] recipientsInfo = mailTipsState.RecipientsInfo;
			for (int i = mailTipsResult.Count; i < recipientsInfo.Length; i++)
			{
				mailTipsResult.Add(new MailTips(new EmailAddress(recipientsInfo[i].DisplayName, recipientsInfo[i].RoutingAddress, recipientsInfo[i].RoutingType), exception));
			}
			if (mailTipsState.RequestLogger == null)
			{
				mailTipsState.RequestLogger = new RequestLogger();
			}
			mailTipsState.RequestLogger.AppendToLog<Type>("MailTipsException", exception.GetType());
		}

		private void GetMailTipsWorker(object state)
		{
			MailTipsState mailTipsState = null;
			try
			{
				mailTipsState = (MailTipsState)state;
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "MailTipsNotificationHandler.GetMailTipsWorker");
				this.GetMailTipsInBatches(mailTipsState);
				this.mailTipsNotifier.AddToPayload(mailTipsState);
				this.mailTipsNotifier.PickupData();
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<Exception>((long)this.GetHashCode(), "Generic exception caught during GetMailTipsWorker call: {0}", ex);
				if (Globals.SendWatsonReports)
				{
					ExTraceGlobals.CoreTracer.TraceError((long)this.GetHashCode(), "Sending watson report.");
					ExWatson.AddExtraData(this.GetExtraWatsonData(mailTipsState));
					ExWatson.SendReport(ex, ReportOptions.None, null);
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.concurrentRequestCount);
				ExTraceGlobals.CoreCallTracer.TraceError<string, int>((long)this.GetHashCode(), "MailTipsNotificationHandler.GetMailTipsWorker, {0} concurrent requests count decremented to {1}", this.primarySmtpAddress, this.concurrentRequestCount);
			}
		}

		private ProxyAddress GetSendingAsProxyAddress(MailTipsState mailTipsState)
		{
			ProxyAddress proxyAddress;
			if (mailTipsState.SenderInfo == null)
			{
				if (string.IsNullOrEmpty(mailTipsState.LogonUserLegDn))
				{
					proxyAddress = ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, mailTipsState.LogonUserPrimarySmtpAddress ?? string.Empty);
				}
				else
				{
					proxyAddress = ProxyAddress.Parse(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, mailTipsState.LogonUserLegDn);
				}
			}
			else
			{
				proxyAddress = mailTipsState.SenderInfo.ToProxyAddress();
			}
			mailTipsState.SendingAs = proxyAddress;
			return proxyAddress;
		}

		private IEnumerable<MailTips> GetMailTipsInBatches(MailTipsState mailTipsState)
		{
			Exception ex = null;
			try
			{
				using (IStandardBudget standardBudget = StandardBudget.Acquire(mailTipsState.ClientSecurityContext.UserSid, BudgetType.Owa, ADSessionSettings.FromRootOrgScopeSet()))
				{
					string callerInfo = "MailTipsNotificationHandler.GetMailTipsInBatches";
					standardBudget.CheckOverBudget();
					standardBudget.StartConnection(callerInfo);
					standardBudget.StartLocal(callerInfo, default(TimeSpan));
					mailTipsState.Budget = standardBudget;
					ClientContext clientContext = ClientContext.Create(mailTipsState.ClientSecurityContext, mailTipsState.Budget, mailTipsState.LogonUserTimeZone, mailTipsState.LogonUserCulture);
					((InternalClientContext)clientContext).QueryBaseDN = mailTipsState.QueryBaseDn;
					ExTraceGlobals.CoreCallTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "QueryBaseDN set to {0}", mailTipsState.QueryBaseDn);
					ProxyAddress sendingAsProxyAddress = this.GetSendingAsProxyAddress(mailTipsState);
					mailTipsState.CachedOrganizationConfiguration = CachedOrganizationConfiguration.GetInstance(mailTipsState.LogonUserOrgId, CachedOrganizationConfiguration.ConfigurationTypes.All);
					ExTraceGlobals.CoreCallTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Organization ID = {0}", mailTipsState.LogonUserOrgId);
					try
					{
						int num = 0;
						do
						{
							ProxyAddress[] nextBatch = MailTipsNotificationHandler.GetNextBatch(mailTipsState.RecipientsInfo, ref num);
							mailTipsState.GetMailTipsQuery = new GetMailTipsQuery(this.GetHashCode(), clientContext, sendingAsProxyAddress, mailTipsState.CachedOrganizationConfiguration, nextBatch, MailTipTypes.OutOfOfficeMessage | MailTipTypes.MailboxFullStatus | MailTipTypes.CustomMailTip | MailTipTypes.ExternalMemberCount | MailTipTypes.TotalMemberCount | MailTipTypes.DeliveryRestriction | MailTipTypes.ModerationStatus, mailTipsState.LogonUserCulture.LCID, mailTipsState.Budget, null);
							mailTipsState.GetMailTipsQuery.ServerName = mailTipsState.ServerName;
							mailTipsState.RequestLogger = mailTipsState.GetMailTipsQuery.RequestLogger;
							IEnumerable<MailTips> collection = mailTipsState.GetMailTipsQuery.Execute();
							mailTipsState.MailTipsResult.AddRange(collection);
						}
						while (num < mailTipsState.RecipientsInfo.Length);
					}
					catch (UserWithoutFederatedProxyAddressException ex2)
					{
						ex = ex2;
					}
					catch (InvalidFederatedOrganizationIdException ex3)
					{
						ex = ex3;
					}
				}
			}
			catch (OverBudgetException ex4)
			{
				ex = ex4;
			}
			catch (ObjectDisposedException ex5)
			{
				ex = ex5;
			}
			catch (OwaInvalidOperationException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (ex != null)
				{
					MailTipsNotificationHandler.PopulateException(mailTipsState, ex, this.GetHashCode());
				}
			}
			return mailTipsState.MailTipsResult;
		}

		private string GetExtraWatsonData(MailTipsState mailTipsState)
		{
			int num = 0;
			if (this.userContext.Breadcrumbs != null)
			{
				num = this.userContext.Breadcrumbs.Count * 128;
			}
			int num2 = 0;
			if (mailTipsState != null)
			{
				num2 = mailTipsState.GetEstimatedStringLength();
			}
			StringBuilder stringBuilder = new StringBuilder(40 + num + num2);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "OWAVersion: {0}, ", new object[]
			{
				Globals.ApplicationVersion
			});
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "MailTipsState: {0}, ", new object[]
			{
				mailTipsState
			});
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "BreadCrumbs: {0}", new object[]
			{
				this.userContext.DumpBreadcrumbs()
			});
			return stringBuilder.ToString();
		}

		private const int MaxConcurrentRequests = 3;

		private const MailTipTypes MailTipsTypesRequested = MailTipTypes.OutOfOfficeMessage | MailTipTypes.MailboxFullStatus | MailTipTypes.CustomMailTip | MailTipTypes.ExternalMemberCount | MailTipTypes.TotalMemberCount | MailTipTypes.DeliveryRestriction | MailTipTypes.ModerationStatus;

		private UserContext userContext;

		private MailTipsPendingRequestNotifier mailTipsNotifier;

		private int concurrentRequestCount;

		private string primarySmtpAddress;
	}
}
