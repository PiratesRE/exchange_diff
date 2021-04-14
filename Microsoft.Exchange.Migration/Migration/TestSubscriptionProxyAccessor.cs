using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal sealed class TestSubscriptionProxyAccessor : SubscriptionAccessorBase
	{
		private TestSubscriptionProxyAccessor(string endpoint)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(endpoint, "endpoint");
			this.implementation = (ITestSubscriptionAccessor)Activator.GetObject(typeof(ITestSubscriptionAccessor), endpoint);
			if (this.implementation == null)
			{
				throw new InvalidOperationException("couldn't create remote instance at endpoint " + endpoint);
			}
		}

		public static bool TryCreate(out SubscriptionAccessorBase accessor)
		{
			accessor = null;
			string migrationAccessorEndpoint = MigrationTestIntegration.Instance.MigrationAccessorEndpoint;
			if (string.IsNullOrEmpty(migrationAccessorEndpoint))
			{
				return false;
			}
			accessor = new TestSubscriptionProxyAccessor(migrationAccessorEndpoint);
			return true;
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			MigrationEndpointBase endpoint = null;
			if (jobItem.MigrationJob.JobDirection == MigrationBatchDirection.Onboarding)
			{
				endpoint = jobItem.MigrationJob.SourceEndpoint;
			}
			else if (jobItem.MigrationJob.JobDirection == MigrationBatchDirection.Offboarding)
			{
				endpoint = jobItem.MigrationJob.TargetEndpoint;
			}
			TestSubscriptionAspect aspect = new TestSubscriptionAspect(endpoint, jobItem, false);
			SubscriptionSnapshot snapshot = null;
			this.RunProxyOperation(delegate
			{
				snapshot = this.implementation.CreateSubscription(aspect);
			});
			return snapshot;
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			MigrationEndpointBase endpoint = null;
			if (jobItem.MigrationJob.JobDirection == MigrationBatchDirection.Onboarding)
			{
				endpoint = jobItem.MigrationJob.SourceEndpoint;
			}
			else if (jobItem.MigrationJob.JobDirection == MigrationBatchDirection.Offboarding)
			{
				endpoint = jobItem.MigrationJob.TargetEndpoint;
			}
			TestSubscriptionAspect aspect = new TestSubscriptionAspect(endpoint, jobItem, false);
			SubscriptionSnapshot snapshot = null;
			this.RunProxyOperation(delegate
			{
				snapshot = this.implementation.TestCreateSubscription(aspect);
			});
			return snapshot;
		}

		public override SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId)
		{
			MRSSubscriptionId subId = subscriptionId as MRSSubscriptionId;
			SnapshotStatus? status = null;
			this.RunProxyOperation(delegate
			{
				status = new SnapshotStatus?(this.implementation.RetrieveSubscriptionStatus(subId.Id));
			});
			if (status == null)
			{
				throw new MigrationPermanentException(new LocalizedString("could not retrieve subscription status for " + subId.Id));
			}
			return status.Value;
		}

		public override SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId)
		{
			MRSSubscriptionId subId = subscriptionId as MRSSubscriptionId;
			SubscriptionSnapshot snapshot = null;
			this.RunProxyOperation(delegate
			{
				snapshot = this.implementation.RetrieveSubscriptionSnapshot(subId.Id);
			});
			if (snapshot == null)
			{
				throw new MigrationPermanentException(new LocalizedString("could not retrieve subscription for " + subId.Id));
			}
			return snapshot;
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			this.UpdateSubscriptionStatus(subscriptionId, SnapshotStatus.InProgress);
			return true;
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			this.UpdateSubscriptionStatus(subscriptionId, SnapshotStatus.Suspended);
			return true;
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			this.UpdateSubscriptionStatus(subscriptionId, SnapshotStatus.Removed);
			return true;
		}

		private void UpdateSubscriptionStatus(ISubscriptionId subscriptionId, SnapshotStatus status)
		{
			MRSSubscriptionId subId = subscriptionId as MRSSubscriptionId;
			this.RunProxyOperation(delegate
			{
				this.implementation.UpdateSubscriptionStatus(subId.Id, status);
			});
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			MRSSubscriptionId subId = subscriptionId as MRSSubscriptionId;
			TestSubscriptionAspect aspect = new TestSubscriptionAspect(endpoint, jobItem, true);
			this.RunProxyOperation(delegate
			{
				this.implementation.UpdateSubscription(subId.Id, aspect);
			});
			return true;
		}

		private void RunProxyOperation(TestSubscriptionProxyAccessor.ProxyOperation operation)
		{
			try
			{
				this.debugContextIndex = (this.debugContextIndex + 1) % 10;
				this.debugContext[this.debugContextIndex] = ExDateTime.UtcNow.ToString() + " " + this.implementation.GetDebuggingContext();
				operation();
			}
			catch (TransientException)
			{
				throw;
			}
			catch (MigrationPermanentException)
			{
				throw;
			}
			catch (StoragePermanentException)
			{
				throw;
			}
			catch (Exception ex)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 10; i > 0; i--)
				{
					int num = (i + this.debugContextIndex) % 10;
					if (string.IsNullOrEmpty(this.debugContext[num]))
					{
						break;
					}
					stringBuilder.Append(this.debugContext[num]);
				}
				MigrationLogger.Log(MigrationEventType.Error, ex, "TEST RunProxyOperation failed with unknown error: context ", new object[]
				{
					stringBuilder
				});
				throw new MigrationDataCorruptionException("Error running test proxy code:" + this.debugContext[this.debugContextIndex], ex);
			}
		}

		private const int DebugContextSize = 10;

		private readonly ITestSubscriptionAccessor implementation;

		private string[] debugContext = new string[10];

		private int debugContextIndex = -1;

		internal delegate void ProxyOperation();
	}
}
