using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetCalendarWorkflowConfiguration : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-CalendarProcessing";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string AutomateProcessing
		{
			get
			{
				return (string)base["AutomateProcessing"];
			}
			set
			{
				base["AutomateProcessing"] = value;
			}
		}

		[DataMember]
		public bool RemoveOldMeetingMessages
		{
			get
			{
				return (bool)(base["RemoveOldMeetingMessages"] ?? false);
			}
			set
			{
				base["RemoveOldMeetingMessages"] = value;
			}
		}

		[DataMember]
		public bool AddNewRequestsTentatively
		{
			get
			{
				return (bool)(base["AddNewRequestsTentatively"] ?? false);
			}
			set
			{
				base["AddNewRequestsTentatively"] = value;
			}
		}

		[DataMember]
		public bool ProcessExternalMeetingMessages
		{
			get
			{
				return (bool)(base["ProcessExternalMeetingMessages"] ?? false);
			}
			set
			{
				base["ProcessExternalMeetingMessages"] = value;
			}
		}

		[DataMember]
		public bool RemoveForwardedMeetingNotifications
		{
			get
			{
				return (bool)(base["RemoveForwardedMeetingNotifications"] ?? false);
			}
			set
			{
				base["RemoveForwardedMeetingNotifications"] = value;
			}
		}
	}
}
