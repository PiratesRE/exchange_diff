using System;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.ThrottlingService.Client;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LimitChecker
	{
		public LimitChecker(IRuleEvaluationContext context)
		{
			this.context = context;
		}

		public IPAddress ServerIPAddress { protected get; set; }

		public bool CheckAndIncrementContentRestrictionCount(int count, string valueToCompare)
		{
			return string.IsNullOrEmpty(valueToCompare) || 500000 >= valueToCompare.Length || this.CheckAndIncrementContentRestrictionCount(count);
		}

		public bool CheckAndIncrementContentRestrictionCount(int count, byte[] valueToCompare)
		{
			return RuleUtil.IsNullOrEmpty(valueToCompare) || 1000000 >= valueToCompare.Length || this.CheckAndIncrementContentRestrictionCount(count);
		}

		public bool CheckAndIncrementForwardeeCount(int count)
		{
			if (count == 0)
			{
				this.context.TraceDebug("No check passed since count is 0");
				return true;
			}
			this.LoadThrottlingPolicy();
			return !this.DoesExceedPerMessageForwardeeLimit(count) && !this.DoesExceedPerDayForwardeeLimit(count);
		}

		public bool DoesExceedAutoReplyLimit()
		{
			bool result = false;
			MailboxSession mailboxSession = this.context.StoreSession as MailboxSession;
			if (mailboxSession == null)
			{
				this.context.TraceDebug<StoreSession>("No throttling applied to non mailbox session {0}", this.context.StoreSession);
			}
			else
			{
				this.LoadThrottlingPolicy();
				if (!MailboxThrottle.Instance.ObtainUserSubmissionTokens(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn, mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion, this.recipientMailboxGuid, 1, this.throttlingPolicy.RecipientRateLimit, mailboxSession.ClientInfoString))
				{
					this.context.TraceError<string, Unlimited<uint>>("Rule {0} exceeded the auto reply limit of {1}.", this.context.CurrentRule.Name, this.throttlingPolicy.RecipientRateLimit);
					this.MessageTrackThrottle<Unlimited<uint>, Unlimited<uint>>("AutoReplyRate", this.throttlingPolicy.RecipientRateLimit, this.throttlingPolicy.RecipientRateLimit);
					result = true;
				}
			}
			return result;
		}

		public bool DoesExceedXLoopMaxCount(int count)
		{
			bool flag = this.LoadHostedMailboxLimits();
			int num = flag ? 1 : 3;
			if (count >= num)
			{
				this.context.TraceDebug<int, bool>("Possible loop detected: too many X-Loop headers present. Maximum allowed is {0} since loadHostedMailboxLimits is {1}", num, flag);
				this.MessageTrackThrottle<int, int>("XLoopHeaderCount", count, num);
				return true;
			}
			return false;
		}

		private bool CheckAndIncrementContentRestrictionCount(int count)
		{
			this.contentRestrictionCount += count;
			if (!this.LoadHostedMailboxLimits())
			{
				this.context.TraceDebug<int>("Skipping content restriction count check since we are not in Microsoft datacenter, current count is {0}", this.contentRestrictionCount);
				return true;
			}
			if (150 < this.contentRestrictionCount)
			{
				this.context.TraceDebug<int, int>("Content restriction count exceeded limit, evaluation will return false. Count {0}, Limit {1}", this.contentRestrictionCount, 150);
				this.MessageTrackThrottle<int, int>("ContentRestrictionCount", this.contentRestrictionCount, 150);
				return false;
			}
			this.context.TraceDebug<int, int>("Content restriction count is under limit, evaluation will continue. Count {0}, Limit {1}", this.contentRestrictionCount, 150);
			return true;
		}

		private bool LoadHostedMailboxLimits()
		{
			if (this.loadHostedMailboxLimits != null)
			{
				return this.loadHostedMailboxLimits.Value;
			}
			bool result;
			try
			{
				this.loadHostedMailboxLimits = new bool?(false);
				if (VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.LoadHostedMailboxLimits.Enabled)
				{
					this.loadHostedMailboxLimits = new bool?(true);
				}
				result = this.loadHostedMailboxLimits.Value;
			}
			catch (CannotDetermineExchangeModeException argument)
			{
				this.context.TraceError<CannotDetermineExchangeModeException>("Could not determine if LoadHostedMailboxLimits feature is enabled, assuming the more restrictive HostedMailbox mode. Exception {0}", argument);
				result = true;
			}
			return result;
		}

		private void LoadThrottlingPolicy()
		{
			if (this.throttlingPolicy == null)
			{
				Result<ADRawEntry> result = this.context.RecipientCache.FindAndCacheRecipient(this.context.Recipient);
				if (result.Data == null)
				{
					this.context.TraceError("Failed to load throttling policy from recipient cache and AD, using global policy instead.");
					this.throttlingPolicy = ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
					return;
				}
				this.throttlingPolicy = ThrottlingPolicyCache.Singleton.Get(this.context.RecipientCache.OrganizationId, (ADObjectId)result.Data[ADRecipientSchema.ThrottlingPolicy]);
				this.recipientMailboxGuid = (Guid)result.Data[IADMailStorageSchema.ExchangeGuid];
				this.context.TraceDebug<Guid>("Obtained recipient mailbox GUID {0} from recipient cache as part of throttling policy loading.", this.recipientMailboxGuid);
			}
		}

		private bool DoesExceedPerMessageForwardeeLimit(int forwardees)
		{
			this.forwardeeCount += forwardees;
			bool result = false;
			if (this.throttlingPolicy.ForwardeeLimit.IsUnlimited || 0U == this.throttlingPolicy.ForwardeeLimit)
			{
				this.context.TraceDebug<string>("No throttling limit was set on throttling policy {0}", this.throttlingPolicy.GetIdentityString());
			}
			else if ((long)this.forwardeeCount > (long)((ulong)this.throttlingPolicy.ForwardeeLimit.Value))
			{
				this.context.TraceDebug<int, Unlimited<uint>>("Per message forwardee limit was exceeded. Forwardees {0}, Limit {1}", this.forwardeeCount, this.throttlingPolicy.ForwardeeLimit);
				this.MessageTrackThrottle<int, Unlimited<uint>>("PerMessageForwardee", this.forwardeeCount, this.throttlingPolicy.ForwardeeLimit);
				result = true;
			}
			return result;
		}

		private bool DoesExceedPerDayForwardeeLimit(int forwardees)
		{
			bool result = false;
			Unlimited<uint> unlimited = this.throttlingPolicy.RecipientRateLimit * 4;
			MailboxSession mailboxSession = this.context.StoreSession as MailboxSession;
			if (mailboxSession == null)
			{
				this.context.TraceDebug<StoreSession>("No throttling applied to non mailbox session {0}", this.context.StoreSession);
			}
			else if (!MailboxThrottle.Instance.ObtainRuleSubmissionTokens(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn, mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion, this.recipientMailboxGuid, forwardees, unlimited, mailboxSession.ClientInfoString))
			{
				this.context.TraceDebug<string, Unlimited<uint>>("Rule {0} exceeded the forward-by-rule limit of {1}.", this.context.CurrentRule.Name, unlimited);
				this.MessageTrackThrottle<Unlimited<uint>, Unlimited<uint>>("PerDayForwardee", unlimited, unlimited);
				result = true;
			}
			return result;
		}

		protected virtual void MessageTrackThrottle<C, L>(string limitType, C count, L limit)
		{
		}

		private const int MaxContentRestrictionCount = 150;

		private const int MaxCharContentRestriction = 500000;

		private const int MaxBytesContentRestriction = 1000000;

		protected IRuleEvaluationContext context;

		private int contentRestrictionCount;

		private int forwardeeCount;

		private bool? loadHostedMailboxLimits;

		private IThrottlingPolicy throttlingPolicy;

		private Guid recipientMailboxGuid;

		private struct LimitType
		{
			internal const string AutoReplyRate = "AutoReplyRate";

			internal const string XLoopHeaderCount = "XLoopHeaderCount";

			internal const string ContentRestrictionCount = "ContentRestrictionCount";

			internal const string PerMessageForwardee = "PerMessageForwardee";

			internal const string PerDayForwardee = "PerDayForwardee";
		}
	}
}
