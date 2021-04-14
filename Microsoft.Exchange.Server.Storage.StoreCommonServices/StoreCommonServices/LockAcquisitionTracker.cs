using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct LockAcquisitionTracker : IOperationExecutionTrackable
	{
		private LockAcquisitionTracker(LockAcquisitionTracker.Key key, LockAcquisitionTracker.Data data)
		{
			this.key = key;
			this.data = data;
		}

		public static LockAcquisitionTracker Create(LockManager.LockType lockType, bool locked, bool contested, byte ownerClientType, byte ownerOperation, TimeSpan timeSpentWaiting)
		{
			LockAcquisitionTracker.Key key = LockAcquisitionTracker.Key.Create(lockType);
			LockAcquisitionTracker.Data data = new LockAcquisitionTracker.Data
			{
				Count = 1,
				NumberSucceeded = (locked ? 1 : 0),
				NumberContested = (contested ? 1 : 0),
				LockOwnerClient = ownerClientType,
				LockOwnerOperation = ownerOperation,
				TimeSpentWaiting = timeSpentWaiting
			};
			return new LockAcquisitionTracker(key, data);
		}

		public LockAcquisitionTracker.Data Tracked
		{
			get
			{
				return this.data;
			}
		}

		public IExecutionPlanner GetExecutionPlanner()
		{
			return null;
		}

		public IOperationExecutionTrackingKey GetTrackingKey()
		{
			return this.key;
		}

		private readonly LockAcquisitionTracker.Key key;

		private readonly LockAcquisitionTracker.Data data;

		internal class Key : IOperationExecutionTrackingKey
		{
			private Key(LockManager.LockType lockType)
			{
				this.key = lockType.GetHashCode();
				this.lockType = lockType;
				this.description = null;
			}

			public static LockAcquisitionTracker.Key Create(LockManager.LockType lockType)
			{
				return new LockAcquisitionTracker.Key(lockType);
			}

			public LockManager.LockType LockType
			{
				get
				{
					return this.lockType;
				}
			}

			public int GetTrackingKeyHashValue()
			{
				return this.key;
			}

			public int GetSimpleHashValue()
			{
				return this.key;
			}

			public bool IsTrackingKeyEqualTo(IOperationExecutionTrackingKey other)
			{
				return other.GetTrackingKeyHashValue().Equals(this.key);
			}

			public string TrackingKeyToString()
			{
				if (this.description == null)
				{
					this.description = this.lockType.ToString();
				}
				return this.description;
			}

			private readonly int key;

			private readonly LockManager.LockType lockType;

			private string description;
		}

		public class Data : IExecutionTrackingData<LockAcquisitionTracker.Data>
		{
			public int Count { get; set; }

			public int NumberSucceeded { get; set; }

			public int NumberContested { get; set; }

			public TimeSpan TimeSpentWaiting { get; set; }

			public byte LockOwnerClient { get; set; }

			public byte LockOwnerOperation { get; set; }

			public TimeSpan TotalTime
			{
				get
				{
					return this.TimeSpentWaiting;
				}
			}

			public void Aggregate(LockAcquisitionTracker.Data dataToAggregate)
			{
				this.Count += dataToAggregate.Count;
				this.NumberSucceeded += dataToAggregate.NumberSucceeded;
				this.NumberContested += dataToAggregate.NumberContested;
				this.TimeSpentWaiting += dataToAggregate.TimeSpentWaiting;
				this.LockOwnerClient = ((dataToAggregate.LockOwnerClient > 0) ? dataToAggregate.LockOwnerClient : this.LockOwnerClient);
				this.LockOwnerOperation = ((dataToAggregate.LockOwnerOperation > 0) ? dataToAggregate.LockOwnerOperation : this.LockOwnerOperation);
			}

			public void Reset()
			{
				this.Count = 0;
				this.NumberSucceeded = 0;
				this.NumberContested = 0;
				this.TimeSpentWaiting = TimeSpan.Zero;
				this.LockOwnerClient = 0;
				this.LockOwnerOperation = 0;
			}

			public void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				long num = (long)this.TimeSpentWaiting.TotalMicroseconds();
				cb.Append("Locked: ");
				cb.Append(this.NumberSucceeded);
				cb.Append(", Contest: ");
				cb.Append(this.NumberContested);
				cb.Append(", Waited: ");
				cb.Append(num.ToString("N0", CultureInfo.InvariantCulture));
				cb.Append(" us, Client: ");
				cb.Append((int)this.LockOwnerClient);
				cb.Append(", Oper: ");
				cb.Append((int)this.LockOwnerOperation);
			}

			public void AppendDetailsToTraceContentBuilder(TraceContentBuilder cb, int indentLevel)
			{
			}
		}

		public enum LockCategory
		{
			Mailbox,
			Component,
			Other
		}
	}
}
