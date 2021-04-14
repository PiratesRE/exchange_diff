using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetJournalReportNdrTo : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-TransportConfig";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@C:OrganizationConfig";
			}
		}

		[DataMember]
		public string JournalingReportNdrTo
		{
			get
			{
				string text = (string)base["JournalingReportNdrTo"];
				if (string.IsNullOrEmpty(text))
				{
					text = SmtpAddress.NullReversePath.ToString();
				}
				return text;
			}
			set
			{
				string value2 = value;
				if (string.IsNullOrEmpty(value2))
				{
					value2 = SmtpAddress.NullReversePath.ToString();
				}
				base["JournalingReportNdrTo"] = value2;
			}
		}
	}
}
