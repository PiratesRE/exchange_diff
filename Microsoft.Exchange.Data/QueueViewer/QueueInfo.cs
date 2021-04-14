using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class QueueInfo : PagedDataObject, IConfigurable
	{
		internal QueueInfo(QueueIdentity identity)
		{
			this.identity = identity;
		}

		public QueueInfo(ExtensibleQueueInfo queueInfo)
		{
			this.identity = queueInfo.QueueIdentity;
			this.deliveryType = queueInfo.DeliveryType;
			this.nextHopConnector = queueInfo.NextHopConnector;
			this.status = queueInfo.Status;
			this.messageCount = queueInfo.MessageCount;
			this.lastError = queueInfo.LastError;
			this.lastRetryTime = queueInfo.LastRetryTime;
			this.nextRetryTime = queueInfo.NextRetryTime;
		}

		public bool IsDeliveryQueue()
		{
			return this.identity.Type == QueueType.Delivery;
		}

		public bool IsSubmissionQueue()
		{
			return this.identity.Type == QueueType.Submission;
		}

		public bool IsPoisonQueue()
		{
			return this.identity.Type == QueueType.Poison;
		}

		public bool IsShadowQueue()
		{
			return this.identity.Type == QueueType.Shadow;
		}

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public DeliveryType DeliveryType
		{
			get
			{
				return this.deliveryType;
			}
			internal set
			{
				this.deliveryType = value;
			}
		}

		public string NextHopDomain
		{
			get
			{
				return this.identity.NextHopDomain;
			}
			internal set
			{
				this.identity.NextHopDomain = value;
			}
		}

		public Guid NextHopConnector
		{
			get
			{
				return this.nextHopConnector;
			}
			internal set
			{
				this.nextHopConnector = value;
			}
		}

		public QueueStatus Status
		{
			get
			{
				return this.status;
			}
			internal set
			{
				this.status = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return this.messageCount;
			}
			internal set
			{
				this.messageCount = value;
			}
		}

		public string LastError
		{
			get
			{
				return this.lastError;
			}
			internal set
			{
				this.lastError = value;
			}
		}

		public DateTime? LastRetryTime
		{
			get
			{
				return this.lastRetryTime;
			}
			internal set
			{
				this.lastRetryTime = value;
			}
		}

		public DateTime? NextRetryTime
		{
			get
			{
				return this.nextRetryTime;
			}
			internal set
			{
				this.nextRetryTime = value;
			}
		}

		public void ConvertDatesToLocalTime()
		{
			throw new NotImplementedException();
		}

		public void ConvertDatesToUniversalTime()
		{
			throw new NotImplementedException();
		}

		public ValidationError[] Validate()
		{
			throw new NotImplementedException();
		}

		public bool IsValid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		private QueueIdentity identity;

		private DeliveryType deliveryType;

		private Guid nextHopConnector;

		private QueueStatus status;

		private int messageCount;

		private string lastError;

		private DateTime? lastRetryTime;

		private DateTime? nextRetryTime;
	}
}
