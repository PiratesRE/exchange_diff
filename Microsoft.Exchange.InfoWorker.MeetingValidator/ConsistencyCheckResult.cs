using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class ConsistencyCheckResult : IXmlSerializable, IEnumerable<Inconsistency>, IEnumerable
	{
		private ConsistencyCheckResult() : this(ConsistencyCheckType.None, null)
		{
		}

		protected ConsistencyCheckResult(ConsistencyCheckType checkType, string checkDescription)
		{
			this.Initialize(checkType, checkDescription, CheckStatusType.Passed, new List<Inconsistency>(0));
		}

		internal static ConsistencyCheckResult CreateInstance(ConsistencyCheckType checkType, string checkDescription)
		{
			return new ConsistencyCheckResult(checkType, checkDescription);
		}

		private void Initialize(ConsistencyCheckType checkType, string checkDescription, CheckStatusType status, List<Inconsistency> inconsistencies)
		{
			this.CheckDescription = checkDescription;
			this.CheckType = checkType;
			this.status = status;
			this.RepairInfo = RepairSteps.CreateInstance();
			this.Inconsistencies = inconsistencies;
			this.Severity = SeverityType.Information;
			this.timestamp = DateTime.UtcNow;
			this.comparedRecurrenceBlobs = false;
			this.recurrenceBlobComparison = false;
			this.meetingOverlap = int.MinValue;
			this.responseStatus = int.MinValue;
			this.replyTime = DateTime.MinValue;
			this.fbStatus = int.MinValue;
			this.ShouldBeReported = false;
		}

		internal string CheckDescription { get; private set; }

		internal ConsistencyCheckType CheckType { get; private set; }

		internal SeverityType Severity { get; set; }

		internal bool ShouldBeReported { get; set; }

		internal CheckStatusType Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		internal RepairSteps RepairInfo { get; private set; }

		public int InconsistencyCount
		{
			get
			{
				return this.Inconsistencies.Count;
			}
		}

		internal string ErrorString
		{
			get
			{
				return this.errorString;
			}
			set
			{
				this.errorString = value;
			}
		}

		internal DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		internal RecurrenceInfo OrganizerRecurrence
		{
			get
			{
				return this.organizerRecurrence;
			}
			set
			{
				this.organizerRecurrence = value;
			}
		}

		internal RecurrenceInfo AttendeeRecurrence
		{
			get
			{
				return this.attendeeRecurrence;
			}
			set
			{
				this.attendeeRecurrence = value;
			}
		}

		internal bool ComparedRecurrenceBlobs
		{
			get
			{
				return this.comparedRecurrenceBlobs;
			}
			set
			{
				this.comparedRecurrenceBlobs = value;
			}
		}

		internal bool RecurrenceBlobComparison
		{
			get
			{
				return this.recurrenceBlobComparison;
			}
			set
			{
				this.recurrenceBlobComparison = value;
			}
		}

		public int MeetingOverlap
		{
			get
			{
				return this.meetingOverlap;
			}
			set
			{
				this.meetingOverlap = value;
			}
		}

		public int ResponseStatus
		{
			get
			{
				return this.responseStatus;
			}
			set
			{
				this.responseStatus = value;
			}
		}

		public DateTime ReplyTime
		{
			get
			{
				return this.replyTime;
			}
			set
			{
				this.replyTime = value;
			}
		}

		public int FreeBusyStatus
		{
			get
			{
				return this.fbStatus;
			}
			set
			{
				this.fbStatus = value;
			}
		}

		public List<Inconsistency> Inconsistencies { get; private set; }

		public void ForEachInconsistency(Action<Inconsistency> action)
		{
			this.Inconsistencies.ForEach(action);
		}

		internal void AddInconsistency(CalendarValidationContext context, Inconsistency inconsistency)
		{
			this.Inconsistencies.Add(inconsistency);
			RumInfo rumInfo = RumFactory.Instance.CreateRumInfo(context, inconsistency);
			if (!rumInfo.IsNullOp)
			{
				inconsistency.ShouldFix = true;
				this.RepairInfo.AddStep(rumInfo);
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException("XML deserialization is not supported.");
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Description", this.CheckDescription);
			writer.WriteElementString("Status", this.status.ToString());
			writer.WriteElementString("Timestamp", this.timestamp.ToString());
			if (this.Status != CheckStatusType.Passed)
			{
				writer.WriteElementString("Severity", this.Severity.ToString());
			}
			if (this.ComparedRecurrenceBlobs)
			{
				writer.WriteElementString("XSORecurrenceBlobComparison", this.RecurrenceBlobComparison.ToString());
			}
			if (this.meetingOverlap != -2147483648)
			{
				writer.WriteElementString("MeetingOverlap", this.meetingOverlap.ToString());
			}
			if (this.responseStatus != -2147483648)
			{
				writer.WriteElementString("ResponseStatus", this.responseStatus.ToString());
			}
			if (this.replyTime != DateTime.MinValue)
			{
				writer.WriteElementString("ReplyTime", this.replyTime.ToString());
			}
			if (this.fbStatus != -2147483648)
			{
				writer.WriteElementString("FBStatus", this.fbStatus.ToString());
			}
			XmlSerializer xmlSerializer;
			if (this.OrganizerRecurrence != null || this.AttendeeRecurrence != null)
			{
				xmlSerializer = new XmlSerializer(typeof(RecurrenceInfo));
				if (this.OrganizerRecurrence != null)
				{
					xmlSerializer.Serialize(writer, this.organizerRecurrence);
				}
				if (this.AttendeeRecurrence != null)
				{
					xmlSerializer.Serialize(writer, this.attendeeRecurrence);
				}
			}
			writer.WriteStartElement("Inconsistencies");
			xmlSerializer = new XmlSerializer(typeof(Inconsistency));
			foreach (Inconsistency o in this.Inconsistencies)
			{
				xmlSerializer.Serialize(writer, o);
			}
			writer.WriteEndElement();
		}

		public IEnumerator<Inconsistency> GetEnumerator()
		{
			return this.Inconsistencies.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private CheckStatusType status;

		private string errorString;

		private DateTime timestamp;

		private RecurrenceInfo organizerRecurrence;

		private RecurrenceInfo attendeeRecurrence;

		private bool comparedRecurrenceBlobs;

		private bool recurrenceBlobComparison;

		private int meetingOverlap;

		private int responseStatus;

		private DateTime replyTime;

		private int fbStatus;
	}
}
