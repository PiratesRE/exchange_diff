using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "JobPickupRec")]
	public class JobPickupRec : XMLSerializableBase
	{
		public JobPickupRec()
		{
		}

		internal JobPickupRec(MoveJob job, JobPickupResult pickupResult, DateTime nextRecommendedPickup, LocalizedString locMessage, ResourceReservationException ex = null)
		{
			this.RequestGuid = job.RequestGuid;
			this.RequestType = job.RequestType;
			this.RequestStatus = job.Status;
			this.WorkloadType = job.WorkloadType;
			this.Priority = job.Priority;
			this.LastUpdateTimeStamp = job.LastUpdateTimeStamp;
			this.Timestamp = DateTime.UtcNow;
			this.PickupResult = pickupResult;
			this.NextRecommendedPickup = nextRecommendedPickup;
			this.locMessage = locMessage;
			if (this.PickupResult == JobPickupResult.ReservationFailure && ex != null)
			{
				this.ReservationFailureRecord = new JobPickupRec.ReservationFailureRec(ex);
				return;
			}
			this.ReservationFailureRecord = null;
		}

		[XmlAttribute(AttributeName = "RequestGuid")]
		public Guid RequestGuid { get; set; }

		[XmlAttribute(AttributeName = "Type")]
		public MRSRequestType RequestType { get; set; }

		[XmlAttribute(AttributeName = "Status")]
		public RequestStatus RequestStatus { get; set; }

		[XmlAttribute(AttributeName = "Workload")]
		public RequestWorkloadType WorkloadType { get; set; }

		[XmlAttribute(AttributeName = "Pri")]
		public int Priority { get; set; }

		[XmlAttribute(AttributeName = "LastUpdate")]
		public DateTime LastUpdateTimeStamp { get; set; }

		[XmlAttribute(AttributeName = "PickupAttemptTime")]
		public DateTime Timestamp { get; set; }

		[XmlAttribute(AttributeName = "Result")]
		public JobPickupResult PickupResult { get; set; }

		[XmlText]
		public string Message
		{
			get
			{
				return this.locMessage.ToString();
			}
			set
			{
				this.locMessage = new LocalizedString(value);
			}
		}

		[XmlIgnore]
		public LocalizedString LocMessage
		{
			get
			{
				return this.locMessage;
			}
			set
			{
				this.locMessage = value;
			}
		}

		[XmlAttribute(AttributeName = "NextRecommendPickup")]
		public DateTime NextRecommendedPickup { get; set; }

		[XmlElement(ElementName = "ReserveFailDetail")]
		public JobPickupRec.ReservationFailureRec ReservationFailureRecord { get; set; }

		public bool HasPickupFailed
		{
			get
			{
				return this.PickupResult >= JobPickupResult.ReservationFailure;
			}
		}

		public LocalizedString GetPickupFailureMessage()
		{
			if (!this.HasPickupFailed)
			{
				return LocalizedString.Empty;
			}
			return this.locMessage;
		}

		public bool IsQueuedOrActiveJob
		{
			get
			{
				return this.PickupResult != JobPickupResult.CompletedJobCleanedUp && this.PickupResult != JobPickupResult.CompletedJobSkipped && this.PickupResult != JobPickupResult.InvalidJob && (this.RequestStatus == RequestStatus.Queued || this.RequestStatus == RequestStatus.InProgress);
			}
		}

		public bool IsActiveJob
		{
			get
			{
				return this.RequestStatus == RequestStatus.InProgress && (this.PickupResult == JobPickupResult.JobPickedUp || this.PickupResult == JobPickupResult.JobAlreadyActive);
			}
		}

		public bool IsQueuedJob
		{
			get
			{
				return this.IsQueuedOrActiveJob && !this.IsActiveJob;
			}
		}

		private LocalizedString locMessage;

		public sealed class ReservationFailureRec : XMLSerializableBase
		{
			public ReservationFailureRec()
			{
			}

			internal ReservationFailureRec(ResourceReservationException ex)
			{
				this.Reason = ex.GetType().Name;
				StaticCapacityExceededReservationException ex2 = ex as StaticCapacityExceededReservationException;
				if (ex2 != null)
				{
					this.Name = ex2.ResourceName;
					this.Type = ex2.ResourceType;
				}
				WlmCapacityExceededReservationException ex3 = ex as WlmCapacityExceededReservationException;
				if (ex3 != null)
				{
					this.Name = ex3.ResourceName;
					this.Type = ex3.ResourceType;
					this.WLMResourceKey = ex3.WlmResourceKey;
					this.WLMResourceMonitorType = ex3.WlmResourceMetricType;
				}
				WlmResourceUnhealthyException ex4 = ex as WlmResourceUnhealthyException;
				if (ex4 != null)
				{
					this.Name = ex4.ResourceName;
					this.Type = ex4.ResourceType;
					this.WLMResourceKey = ex4.WlmResourceKey;
					this.WLMResourceMonitorType = ex4.WlmResourceMetricType;
				}
			}

			[XmlText]
			public string Reason { get; set; }

			[XmlAttribute(AttributeName = "ResName")]
			public string Name { get; set; }

			[XmlAttribute(AttributeName = "ResType")]
			public string Type { get; set; }

			[XmlAttribute(AttributeName = "WLMResKey")]
			public string WLMResourceKey { get; set; }

			[XmlAttribute(AttributeName = "WLMResMonitorType")]
			public int WLMResourceMonitorType { get; set; }
		}
	}
}
