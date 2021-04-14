using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ActiveSyncExecutionContext
	{
		public ActiveSyncExecutionContext(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mailboxFqdn)
		{
			this.instance = instance;
			this.mailboxFqdn = mailboxFqdn;
		}

		public TimeSpan PingLatency
		{
			get
			{
				return this.pingLatency;
			}
			set
			{
				this.pingLatency = value;
			}
		}

		public bool VerifyItemCameDown
		{
			get
			{
				return this.verifyItemCameDown;
			}
			set
			{
				this.verifyItemCameDown = value;
			}
		}

		public ActiveSyncExecutionState NextStateToExecute
		{
			get
			{
				return this.nextStateToRun;
			}
			set
			{
				this.nextStateToRun = value;
			}
		}

		public TimeSpan SyncAfterPingLatency
		{
			get
			{
				return this.syncAfterPingLatency;
			}
			set
			{
				this.syncAfterPingLatency = value;
			}
		}

		public TestCasConnectivity.TestCasConnectivityRunInstance Instance
		{
			get
			{
				return this.instance;
			}
		}

		public string MailboxFqdn
		{
			get
			{
				return this.mailboxFqdn;
			}
		}

		public ActiveSyncState CurrentState
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public string CollectionId
		{
			get
			{
				return this.collectionId;
			}
			set
			{
				this.collectionId = value;
			}
		}

		public string SyncState
		{
			get
			{
				return this.syncState;
			}
			set
			{
				this.syncState = value;
			}
		}

		public int Estimate
		{
			get
			{
				return this.estimate;
			}
			set
			{
				this.estimate = value;
			}
		}

		private TestCasConnectivity.TestCasConnectivityRunInstance instance;

		private ActiveSyncState state;

		private string collectionId;

		private readonly string mailboxFqdn;

		private string syncState = "0";

		private int estimate;

		private bool verifyItemCameDown;

		private ActiveSyncExecutionState nextStateToRun;

		private TimeSpan pingLatency = default(TimeSpan);

		private TimeSpan syncAfterPingLatency = default(TimeSpan);
	}
}
