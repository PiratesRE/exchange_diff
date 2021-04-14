using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CalendarWorkflowConfiguration : BaseRow
	{
		public CalendarWorkflowConfiguration(CalendarConfiguration calendarConfiguration) : base(calendarConfiguration)
		{
			this.CalendarConfiguration = calendarConfiguration;
		}

		public CalendarConfiguration CalendarConfiguration { get; private set; }

		[DataMember]
		public string AutomateProcessing
		{
			get
			{
				return this.CalendarConfiguration.AutomateProcessing.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool RemoveOldMeetingMessages
		{
			get
			{
				return this.CalendarConfiguration.RemoveOldMeetingMessages;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AddNewRequestsTentatively
		{
			get
			{
				return this.CalendarConfiguration.AddNewRequestsTentatively;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ProcessExternalMeetingMessages
		{
			get
			{
				return this.CalendarConfiguration.ProcessExternalMeetingMessages;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool RemoveForwardedMeetingNotifications
		{
			get
			{
				return this.CalendarConfiguration.RemoveForwardedMeetingNotifications;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
