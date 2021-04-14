using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(MeetingRequestMessageType))]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(MeetingResponseMessageType))]
	[DebuggerStepThrough]
	[Serializable]
	public class MeetingMessageType : MessageType
	{
		public ItemIdType AssociatedCalendarItemId
		{
			get
			{
				return this.associatedCalendarItemIdField;
			}
			set
			{
				this.associatedCalendarItemIdField = value;
			}
		}

		public bool IsDelegated
		{
			get
			{
				return this.isDelegatedField;
			}
			set
			{
				this.isDelegatedField = value;
			}
		}

		[XmlIgnore]
		public bool IsDelegatedSpecified
		{
			get
			{
				return this.isDelegatedFieldSpecified;
			}
			set
			{
				this.isDelegatedFieldSpecified = value;
			}
		}

		public bool IsOutOfDate
		{
			get
			{
				return this.isOutOfDateField;
			}
			set
			{
				this.isOutOfDateField = value;
			}
		}

		[XmlIgnore]
		public bool IsOutOfDateSpecified
		{
			get
			{
				return this.isOutOfDateFieldSpecified;
			}
			set
			{
				this.isOutOfDateFieldSpecified = value;
			}
		}

		public bool HasBeenProcessed
		{
			get
			{
				return this.hasBeenProcessedField;
			}
			set
			{
				this.hasBeenProcessedField = value;
			}
		}

		[XmlIgnore]
		public bool HasBeenProcessedSpecified
		{
			get
			{
				return this.hasBeenProcessedFieldSpecified;
			}
			set
			{
				this.hasBeenProcessedFieldSpecified = value;
			}
		}

		public ResponseTypeType ResponseType
		{
			get
			{
				return this.responseTypeField;
			}
			set
			{
				this.responseTypeField = value;
			}
		}

		[XmlIgnore]
		public bool ResponseTypeSpecified
		{
			get
			{
				return this.responseTypeFieldSpecified;
			}
			set
			{
				this.responseTypeFieldSpecified = value;
			}
		}

		public string UID
		{
			get
			{
				return this.uIDField;
			}
			set
			{
				this.uIDField = value;
			}
		}

		public DateTime RecurrenceId
		{
			get
			{
				return this.recurrenceIdField;
			}
			set
			{
				this.recurrenceIdField = value;
			}
		}

		[XmlIgnore]
		public bool RecurrenceIdSpecified
		{
			get
			{
				return this.recurrenceIdFieldSpecified;
			}
			set
			{
				this.recurrenceIdFieldSpecified = value;
			}
		}

		public DateTime DateTimeStamp
		{
			get
			{
				return this.dateTimeStampField;
			}
			set
			{
				this.dateTimeStampField = value;
			}
		}

		[XmlIgnore]
		public bool DateTimeStampSpecified
		{
			get
			{
				return this.dateTimeStampFieldSpecified;
			}
			set
			{
				this.dateTimeStampFieldSpecified = value;
			}
		}

		public bool IsOrganizer
		{
			get
			{
				return this.isOrganizerField;
			}
			set
			{
				this.isOrganizerField = value;
			}
		}

		[XmlIgnore]
		public bool IsOrganizerSpecified
		{
			get
			{
				return this.isOrganizerFieldSpecified;
			}
			set
			{
				this.isOrganizerFieldSpecified = value;
			}
		}

		private ItemIdType associatedCalendarItemIdField;

		private bool isDelegatedField;

		private bool isDelegatedFieldSpecified;

		private bool isOutOfDateField;

		private bool isOutOfDateFieldSpecified;

		private bool hasBeenProcessedField;

		private bool hasBeenProcessedFieldSpecified;

		private ResponseTypeType responseTypeField;

		private bool responseTypeFieldSpecified;

		private string uIDField;

		private DateTime recurrenceIdField;

		private bool recurrenceIdFieldSpecified;

		private DateTime dateTimeStampField;

		private bool dateTimeStampFieldSpecified;

		private bool isOrganizerField;

		private bool isOrganizerFieldSpecified;
	}
}
