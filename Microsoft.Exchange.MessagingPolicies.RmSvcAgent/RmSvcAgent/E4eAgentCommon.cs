using System;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class E4eAgentCommon
	{
		internal static bool IncrementDeferralCountAndCheckCap(MailItem mailItem, string deferralCountPropertyName)
		{
			int num = Utils.IncrementDeferralCount(mailItem, deferralCountPropertyName);
			if (num == -1)
			{
				E4eLog.Instance.LogError(mailItem.Message.MessageId, "Deferral count is broken.", new object[0]);
				return false;
			}
			if (num > 1)
			{
				E4eLog.Instance.LogInfo(mailItem.Message.MessageId, "Deferred {0} time(s).", new object[]
				{
					num - 1
				});
			}
			if (num > 3)
			{
				E4eLog.Instance.LogInfo(mailItem.Message.MessageId, "Max deferrals reached.", new object[0]);
				return false;
			}
			E4eLog.Instance.LogInfo(mailItem.Message.MessageId, "Will be deferred.", new object[0]);
			return true;
		}

		internal static bool RunUnderExceptionHandler(MailItem mailItem, QueuedMessageEventSource source, bool shouldNdr, string deferralCountPropertyName, E4eHelper.E4eDelegate method, E4eHelper.CompleteProcessDelegate completeProcessDelegate)
		{
			Exception ex = null;
			bool flag = false;
			string[] additionalInfo = null;
			E4eHelper.RunUnderExceptionHandler(mailItem.Message.MessageId, method, completeProcessDelegate, out ex, out flag);
			if (ex == null)
			{
				return true;
			}
			if (flag)
			{
				E4eAgentCommon.HandleTransientException(mailItem, ex, additionalInfo, source, shouldNdr, deferralCountPropertyName);
			}
			else
			{
				E4eAgentCommon.HandlePermanentException(mailItem, ex, additionalInfo, shouldNdr);
			}
			return false;
		}

		internal static void HandleTransientException(MailItem mailItem, Exception exception, string[] additionalInfo, QueuedMessageEventSource source, bool shouldNdr, string deferralCountPropertyName)
		{
			if (E4eAgentCommon.IncrementDeferralCountAndCheckCap(mailItem, deferralCountPropertyName))
			{
				E4eLog.Instance.LogError(mailItem.Message.MessageId, "Encountered a transient error. Exception: {0}. Deferring the mail.", new object[]
				{
					exception.ToString()
				});
				source.Defer(RmsClientManager.AppSettings.EncryptionTransientErrorDeferInterval, Utils.GetResponseForExceptionDeferral(exception, null));
				return;
			}
			E4eAgentCommon.HandlePermanentException(mailItem, exception, additionalInfo, shouldNdr);
		}

		internal static void HandlePermanentException(MailItem mailItem, Exception exception, string[] additionalInfo, bool shouldNdr)
		{
			string text = "Decryption will be skipped.";
			if (shouldNdr)
			{
				SmtpResponse responseForNDR;
				if (exception != null && exception.InnerException != null && exception.InnerException.HResult == -2147168383)
				{
					responseForNDR = Utils.GetResponseForNDR(E4eAgentCommon.ndrTextPublishLicenseLimitExceeded);
				}
				else
				{
					responseForNDR = Utils.GetResponseForNDR(Utils.GetSmtpResponseTextsForException(exception, additionalInfo));
				}
				Utils.NDRMessage(mailItem, mailItem.Message.MessageId, responseForNDR);
				text = "Message will be NDRed.";
			}
			E4eLog.Instance.LogError(mailItem.Message.MessageId, "Encountered a permanent error or max defer limit reached. Exception: {0}. {1}", new object[]
			{
				exception.ToString(),
				text
			});
		}

		private static readonly string[] ndrTextPublishLicenseLimitExceeded = new string[]
		{
			"Cannot OME protect the message because there are too many recipients."
		};

		internal static readonly string[] AgentCapReachedDeferralText = new string[]
		{
			"Already processing maximum number of messages."
		};
	}
}
