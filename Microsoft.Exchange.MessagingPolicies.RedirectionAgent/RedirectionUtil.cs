using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.Redirection
{
	internal static class RedirectionUtil
	{
		public static void MarkProcessedByRedirectionAgent(MailItem mailItem)
		{
			mailItem.Properties["ProcessedByRedirectionAgent"] = true;
		}

		public static void ClearProcessedByRedirectionAgent(MailItem mailItem)
		{
			mailItem.Properties.Remove("ProcessedByRedirectionAgent");
		}

		public static bool WasProcessedByRedirectionAgent(MailItem mailItem)
		{
			object obj;
			bool flag = mailItem.Properties.TryGetValue("ProcessedByRedirectionAgent", out obj);
			bool? flag2 = obj as bool?;
			return flag && flag2 != null && flag2.Value;
		}

		public static void SetRedirectionAddress(MailItem mailItem, string redirectionAddress)
		{
			mailItem.Properties["RedirectionAddress"] = redirectionAddress;
		}

		public static void RemoveRedirectionAddress(MailItem mailItem)
		{
			mailItem.Properties.Remove("RedirectionAddress");
		}

		public static string GetRedirectionAddress(MailItem mailItem)
		{
			object obj;
			if (!mailItem.Properties.TryGetValue("RedirectionAddress", out obj))
			{
				return null;
			}
			return obj as string;
		}

		public static ProxyAddress GetForwardingSmtpAddress(EnvelopeRecipient recipient)
		{
			object obj;
			if (!recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.ForwardingSmtpAddress", out obj))
			{
				return null;
			}
			string text = obj as string;
			if (text == null)
			{
				return null;
			}
			return ProxyAddress.Parse(text);
		}

		public static bool GetDeliverAndForward(EnvelopeRecipient recipient)
		{
			object obj;
			if (recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.DeliverToMailboxAndForward", out obj))
			{
				bool? flag = obj as bool?;
				return flag == null || flag.Value;
			}
			return true;
		}

		public static Guid? GetMailboxGuid(EnvelopeRecipient recipient)
		{
			object obj;
			if (recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid", out obj))
			{
				return obj as Guid?;
			}
			return null;
		}

		public static Guid? GetMailboxDatabaseGuid(EnvelopeRecipient recipient)
		{
			ADObjectId adobjectId = null;
			object obj;
			if (recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.Database", out obj))
			{
				adobjectId = (obj as ADObjectId);
			}
			if (adobjectId == null)
			{
				return null;
			}
			return new Guid?(adobjectId.ObjectGuid);
		}

		public static IThrottlingPolicy GetThrottlingPolicy(EnvelopeRecipient recipient, OrganizationId orgId)
		{
			ADObjectId adobjectId = null;
			object obj;
			if (recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.ThrottlingPolicy", out obj))
			{
				adobjectId = (obj as ADObjectId);
			}
			IThrottlingPolicy result;
			if (adobjectId != null)
			{
				result = ThrottlingPolicyCache.Singleton.Get(orgId, adobjectId);
			}
			else
			{
				result = ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
			}
			return result;
		}

		public static string GetPrimarySmtpAddress(ADRawEntry entry)
		{
			SmtpAddress value = (SmtpAddress)entry[ADRecipientSchema.PrimarySmtpAddress];
			if (value == SmtpAddress.Empty)
			{
				return null;
			}
			if (!value.IsValidAddress)
			{
				return null;
			}
			return value.ToString();
		}

		public static bool TryResolveForwardingSmtpAddress(IADRecipientCache cache, ProxyAddress address, out ADRawEntry entry)
		{
			Result<ADRawEntry> result = cache.FindAndCacheRecipient(address);
			entry = result.Data;
			return result.Data != null;
		}

		public static void LogErrorWithMessageId(ExEventLog.EventTuple tuple, long internalMessageId, params object[] args)
		{
			RedirectionUtil.LogEvent(tuple, new object[]
			{
				internalMessageId,
				args
			});
		}

		public static void LogEvent(ExEventLog.EventTuple tuple, params object[] args)
		{
			RedirectionUtil.logger.LogEvent(tuple, null, args);
		}

		private static ExEventLog logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");
	}
}
