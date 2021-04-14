using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TokenManager
	{
		internal TokenManager(int capacity, int maxCapacity)
		{
			this.capacity = capacity;
			this.syncLogSession = this.GetSyncLogSession();
			this.userMailboxAffinity = new Dictionary<Guid, Token>(this.capacity);
			this.userMailboxWorkers = new Dictionary<Guid, Queue<PoolItem<ManualResetEvent>>>(this.capacity);
			this.eventsPool = this.CreateManualResetEventPool(this.capacity, maxCapacity);
		}

		internal void Shutdown()
		{
			this.eventsPool.Shutdown();
		}

		internal void ReleaseToken(Guid mailboxGuid, Token? token)
		{
			lock (this.tokenLock)
			{
				this.userMailboxAffinity.Remove(mailboxGuid);
				if (this.userMailboxWorkers.ContainsKey(mailboxGuid))
				{
					Queue<PoolItem<ManualResetEvent>> queue = this.userMailboxWorkers[mailboxGuid];
					PoolItem<ManualResetEvent> poolItem = queue.Dequeue();
					int num = 0;
					while (num < 2 && !poolItem.Item.Set())
					{
						this.syncLogSession.LogError((TSLID)218UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Failed to notify other threads waiting on release of this token.", new object[0]);
						Thread.Sleep(TimeSpan.FromSeconds(1.0));
						num++;
					}
					if (queue.Count == 0)
					{
						this.userMailboxWorkers.Remove(mailboxGuid);
					}
				}
			}
			this.syncLogSession.LogDebugging((TSLID)164UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Token {0} released.", new object[]
			{
				token
			});
		}

		internal Token? GetToken(Guid mailboxGuid)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			Token? result;
			for (;;)
			{
				PoolItem<ManualResetEvent> item;
				lock (this.tokenLock)
				{
					Token? token;
					if (this.TryGetToken(mailboxGuid, out token))
					{
						ExDateTime utcNow2 = ExDateTime.UtcNow;
						this.SetTimeTakenToGetToken(ExDateTime.TimeDiff(utcNow2, utcNow));
						result = token;
						break;
					}
					bool flag2 = false;
					item = this.eventsPool.GetItem(out flag2);
					if (item == null)
					{
						this.syncLogSession.LogError((TSLID)165UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Failed to get a manual reset event, so we're failing to get the token.", new object[0]);
						result = null;
						break;
					}
					if (!this.userMailboxWorkers.ContainsKey(mailboxGuid))
					{
						this.userMailboxWorkers[mailboxGuid] = new Queue<PoolItem<ManualResetEvent>>(this.capacity);
					}
					this.userMailboxWorkers[mailboxGuid].Enqueue(item);
				}
				this.syncLogSession.LogVerbose((TSLID)166UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Waiting on token.", new object[0]);
				try
				{
					if (item.Item.WaitOne(ContentAggregationConfig.TokenWaitTimeOutInterval, false))
					{
						continue;
					}
					this.syncLogSession.LogError((TSLID)167UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Token wait has timed out. Will fail token operation.", new object[0]);
					StackTrace stackTrace = new StackTrace();
					ContentAggregationConfig.EventLogger.LogEvent(TransportSyncManagerEventLogConstants.Tuple_SyncManagerTokenWaitTimedout, null, new object[]
					{
						mailboxGuid,
						ContentAggregationConfig.TokenWaitTimeOutInterval.TotalMinutes,
						stackTrace.ToString()
					});
					result = null;
				}
				finally
				{
					bool flag3 = item.Item.Reset();
					if (!flag3)
					{
						this.syncLogSession.LogError((TSLID)219UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Failed to reset manual reset event to false state, hence disposing it and NOT reusing it.", new object[0]);
					}
					bool reuse = flag3;
					this.eventsPool.ReturnItem(item, reuse);
				}
				break;
			}
			return result;
		}

		internal bool TryGetToken(Guid mailboxGuid, out Token? token)
		{
			token = null;
			lock (this.tokenLock)
			{
				if (!this.userMailboxAffinity.ContainsKey(mailboxGuid))
				{
					token = new Token?(this.userMailboxAffinity[mailboxGuid] = new Token(Guid.NewGuid()));
				}
			}
			if (token != null)
			{
				this.syncLogSession.LogDebugging((TSLID)168UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Token {0} issued.", new object[]
				{
					token
				});
				return true;
			}
			this.syncLogSession.LogDebugging((TSLID)169UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Token was NOT issued.", new object[0]);
			return false;
		}

		protected virtual GlobalSyncLogSession GetSyncLogSession()
		{
			return ContentAggregationConfig.SyncLogSession;
		}

		protected virtual void SetTimeTakenToGetToken(TimeSpan timeTakenToGetToken)
		{
			ManagerPerfCounterHandler.Instance.SetWaitToGetSubscriptionsCacheToken((long)timeTakenToGetToken.TotalMilliseconds);
		}

		protected virtual ManualResetEventPool CreateManualResetEventPool(int capacity, int maxCapacity)
		{
			return new ManualResetEventPool(this.capacity, maxCapacity);
		}

		[Conditional("DEBUG")]
		private void ValidateToken(Guid mailboxGuid, Token? token)
		{
			if (token == null)
			{
				throw new InvalidOperationException("Token is null.");
			}
			lock (this.tokenLock)
			{
				if (!this.userMailboxAffinity.ContainsKey(mailboxGuid))
				{
					throw new InvalidOperationException("No token ever issued for this userMailbox: " + mailboxGuid);
				}
				if (this.userMailboxAffinity[mailboxGuid] != token)
				{
					throw new InvalidOperationException("Invalid Token for this userMailbox: " + mailboxGuid);
				}
			}
			this.syncLogSession.LogDebugging((TSLID)170UL, TokenManager.diag, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "Token {0} validated.", new object[]
			{
				token
			});
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace diag = ExTraceGlobals.TokenManagerTracer;

		private readonly object tokenLock = new object();

		private readonly int capacity;

		private readonly Dictionary<Guid, Token> userMailboxAffinity;

		private readonly Dictionary<Guid, Queue<PoolItem<ManualResetEvent>>> userMailboxWorkers;

		private readonly GlobalSyncLogSession syncLogSession;

		private ManualResetEventPool eventsPool;
	}
}
