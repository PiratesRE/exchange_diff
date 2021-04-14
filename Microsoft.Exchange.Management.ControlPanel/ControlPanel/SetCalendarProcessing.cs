using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SetCalendarProcessing : SetObjectProperties
	{
		[DataMember]
		public string AutomaticBooking { get; set; }

		[DataMember]
		public Identity[] ResourceDelegates { get; set; }

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
				return "@W:Self|Organization";
			}
		}

		public void UpdateResourceObjects()
		{
			bool flag;
			if (this.AutomaticBooking != null && bool.TryParse(this.AutomaticBooking, out flag))
			{
				base["AutomateProcessing"] = CalendarProcessingFlags.AutoAccept;
				base["AllBookInPolicy"] = flag;
				base["AllRequestInPolicy"] = !flag;
			}
			if (this.ResourceDelegates != null)
			{
				base["ResourceDelegates"] = this.ResourceDelegates.ToIdParameters();
			}
		}
	}
}
