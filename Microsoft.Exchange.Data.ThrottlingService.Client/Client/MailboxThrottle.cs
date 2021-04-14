using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ThrottlingService;

namespace Microsoft.Exchange.Data.ThrottlingService.Client
{
	internal sealed class MailboxThrottle : IDisposeTrackable, IDisposable
	{
		public MailboxThrottle()
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.rpcClients = new Dictionary<string, ThrottlingRpcClientImpl>(16, StringComparer.OrdinalIgnoreCase);
		}

		public static MailboxThrottle Instance
		{
			get
			{
				if (MailboxThrottle.instance == null)
				{
					lock (MailboxThrottle.instanceCreationSyncRoot)
					{
						if (MailboxThrottle.instance == null)
						{
							MailboxThrottle.instance = new MailboxThrottle();
							MailboxThrottle.instance.SuppressDisposeTracker();
						}
					}
				}
				return MailboxThrottle.instance;
			}
		}

		internal int TestabilityVersionForNewRpc
		{
			get
			{
				return MailboxThrottle.NewRpcVersion;
			}
		}

		internal bool TestabilityLastCallUsedNewRpc
		{
			get
			{
				return this.testabilityLastCallUsedNewRpc;
			}
		}

		public static Unlimited<uint> GetUserSubmissionQuota(ADObjectId throttlingPolicyId, OrganizationId organizationId)
		{
			IThrottlingPolicy throttlingPolicy = ThrottlingPolicyCache.Singleton.Get(organizationId, throttlingPolicyId);
			return throttlingPolicy.RecipientRateLimit;
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.rpcClients == null)
			{
				return;
			}
			lock (this.rpcClients)
			{
				foreach (KeyValuePair<string, ThrottlingRpcClientImpl> keyValuePair in this.rpcClients)
				{
					MailboxThrottle.tracer.TraceDebug<string>(0L, "Disposing RPC client for mailbox server {0}", keyValuePair.Key);
					keyValuePair.Value.Dispose();
				}
				this.rpcClients = null;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxThrottle>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public bool ObtainUserSubmissionTokens(string mailboxServer, int mailboxServerVersion, Guid mailboxGuid, int recipientCount, ADObjectId throttlingPolicyId, OrganizationId organizationId, string clientInfo)
		{
			Unlimited<uint> userSubmissionQuota = MailboxThrottle.GetUserSubmissionQuota(throttlingPolicyId, organizationId);
			MailboxThrottle.tracer.TraceDebug<Guid, Unlimited<uint>, int>(0L, "Submission quota for mailbox <{0}> is {1}, requesting {2} tokens.", mailboxGuid, userSubmissionQuota, recipientCount);
			return this.ObtainTokens(mailboxServer, mailboxServerVersion, mailboxGuid, RequestedAction.UserMailSubmission, recipientCount, userSubmissionQuota, clientInfo);
		}

		public bool ObtainUserSubmissionTokens(string mailboxServer, int mailboxServerVersion, Guid mailboxGuid, int recipientCount, Unlimited<uint> totalTokenCount, string clientInfo)
		{
			MailboxThrottle.tracer.TraceDebug<Guid, Unlimited<uint>, int>(0L, "Rule submission quota for mailbox <{0}> is {1}, requesting {2} tokens.", mailboxGuid, totalTokenCount, recipientCount);
			return this.ObtainTokens(mailboxServer, mailboxServerVersion, mailboxGuid, RequestedAction.UserMailSubmission, recipientCount, totalTokenCount, clientInfo);
		}

		public bool ObtainRuleSubmissionTokens(string mailboxServer, int mailboxServerVersion, Guid mailboxGuid, int recipientCount, Unlimited<uint> totalTokenCount, string clientInfo)
		{
			MailboxThrottle.tracer.TraceDebug<Guid, Unlimited<uint>, int>(0L, "Rule submission quota for mailbox <{0}> is {1}, requesting {2} tokens.", mailboxGuid, totalTokenCount, recipientCount);
			return this.ObtainTokens(mailboxServer, mailboxServerVersion, mailboxGuid, RequestedAction.MailboxRuleMailSubmission, recipientCount, totalTokenCount, clientInfo);
		}

		private static bool TryAnswerWithoutRpc(Guid mailboxGuid, int requestedTokenCount, Unlimited<uint> totalTokenCount, out int quota, out bool allow)
		{
			if (totalTokenCount.IsUnlimited)
			{
				MailboxThrottle.tracer.TraceDebug<Guid, int>(0L, "Automatically allow submission for mailbox GUID <{0}> and requestedTokenCount {1} because the quota is unlimited", mailboxGuid, requestedTokenCount);
				allow = true;
				quota = int.MaxValue;
				return true;
			}
			quota = (int)((totalTokenCount.Value <= 2147483647U) ? totalTokenCount.Value : 2147483647U);
			if (requestedTokenCount > quota)
			{
				MailboxThrottle.tracer.TraceDebug<Guid, int, int>(0L, "Automatically deny submission for mailbox GUID <{0}> because requestedTokenCount {1} is over the quota of {2}", mailboxGuid, requestedTokenCount, quota);
				allow = false;
				return true;
			}
			allow = true;
			return false;
		}

		private static bool ThrottlingRpcResultToBoolean(ThrottlingRpcResult result)
		{
			switch (result)
			{
			case ThrottlingRpcResult.Allowed:
				return true;
			case ThrottlingRpcResult.Bypassed:
			case ThrottlingRpcResult.Failed:
				return true;
			case ThrottlingRpcResult.Denied:
				return false;
			default:
				throw new InvalidOperationException("Unexpected result from RPC client: " + result);
			}
		}

		private bool ObtainTokens(string mailboxServer, int mailboxServerVersion, Guid mailboxGuid, RequestedAction requestedAction, int requestedTokenCount, Unlimited<uint> totalTokenCount, string clientInfo)
		{
			this.ThrowIfInvalid(mailboxServer, requestedTokenCount, totalTokenCount);
			int quota;
			bool flag;
			if (MailboxThrottle.TryAnswerWithoutRpc(mailboxGuid, requestedTokenCount, totalTokenCount, out quota, out flag))
			{
				return flag;
			}
			if (mailboxServerVersion >= MailboxThrottle.NewRpcVersion)
			{
				flag = this.ObtainTokensViaNewApi(mailboxServer, mailboxGuid, requestedAction, requestedTokenCount, quota, clientInfo);
				this.testabilityLastCallUsedNewRpc = true;
			}
			else if (requestedAction == RequestedAction.UserMailSubmission)
			{
				flag = this.ObtainTokensViaOldApi(mailboxServer, mailboxGuid, requestedTokenCount, quota);
				this.testabilityLastCallUsedNewRpc = false;
			}
			else
			{
				flag = true;
				this.testabilityLastCallUsedNewRpc = false;
			}
			if (!flag)
			{
				ThrottlingRpcClientImpl.EventLogger.LogEvent(ThrottlingClientEventLogConstants.Tuple_MailboxThrottled, mailboxGuid.ToString(), new object[]
				{
					mailboxGuid,
					mailboxServer,
					requestedAction,
					requestedTokenCount,
					totalTokenCount,
					clientInfo,
					Process.GetCurrentProcess().ProcessName
				});
			}
			return flag;
		}

		private bool ObtainTokensViaOldApi(string mailboxServer, Guid mailboxGuid, int requestedTokenCount, int quota)
		{
			ThrottlingRpcClientImpl rpcClient = this.GetRpcClient(mailboxServer);
			ThrottlingRpcResult result = rpcClient.ObtainSubmissionTokens(mailboxGuid, requestedTokenCount, quota, 0);
			return MailboxThrottle.ThrottlingRpcResultToBoolean(result);
		}

		private bool ObtainTokensViaNewApi(string mailboxServer, Guid mailboxGuid, RequestedAction requestedAction, int requestedTokenCount, int quota, string clientInfo)
		{
			ThrottlingRpcClientImpl rpcClient = this.GetRpcClient(mailboxServer);
			ThrottlingRpcResult result = rpcClient.ObtainTokens(mailboxGuid, requestedAction, requestedTokenCount, quota, clientInfo);
			return MailboxThrottle.ThrottlingRpcResultToBoolean(result);
		}

		private void ThrowIfInvalid(string mailboxServer, int requestedTokenCount, Unlimited<uint> totalTokenCount)
		{
			if (string.IsNullOrEmpty(mailboxServer))
			{
				throw new ArgumentNullException("mailboxServer");
			}
			if (requestedTokenCount < 1)
			{
				throw new ArgumentOutOfRangeException("requestedTokenCount", requestedTokenCount, "requestedTokenCount should be greater than zero");
			}
			if (!totalTokenCount.IsUnlimited && totalTokenCount.Value < 1U)
			{
				throw new ArgumentOutOfRangeException("totalTokenCount", totalTokenCount, "totalTokenCount should be greater than zero and less than Int32.MaxValue");
			}
			if (this.rpcClients == null)
			{
				throw new ObjectDisposedException("MailboxThrottle instance is disposed");
			}
		}

		private ThrottlingRpcClientImpl GetRpcClient(string mailboxServer)
		{
			ThrottlingRpcClientImpl throttlingRpcClientImpl = null;
			lock (this.rpcClients)
			{
				if (!this.rpcClients.TryGetValue(mailboxServer, out throttlingRpcClientImpl))
				{
					throttlingRpcClientImpl = new ThrottlingRpcClientImpl(mailboxServer);
					this.rpcClients.Add(mailboxServer, throttlingRpcClientImpl);
					MailboxThrottle.tracer.TraceDebug<string>(0L, "Added a new RPC client for mailbox server {0}", mailboxServer);
				}
			}
			return throttlingRpcClientImpl;
		}

		private const int RpcClientDictionaryInitialCapacity = 16;

		private static readonly int NewRpcVersion = new ServerVersion(14, 0, 582, 0).ToInt();

		private static Microsoft.Exchange.Diagnostics.Trace tracer = ExTraceGlobals.ThrottlingClientTracer;

		private static MailboxThrottle instance;

		private static object instanceCreationSyncRoot = new object();

		private Dictionary<string, ThrottlingRpcClientImpl> rpcClients;

		private DisposeTracker disposeTracker;

		private bool testabilityLastCallUsedNewRpc;
	}
}
