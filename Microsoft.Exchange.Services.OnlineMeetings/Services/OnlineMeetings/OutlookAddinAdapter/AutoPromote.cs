using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("AutoPromote")]
	[DataContract(Name = "AutoPromote")]
	[KnownType(typeof(AutoPromote))]
	public class AutoPromote
	{
		[XmlAttribute("OrganizerOnly")]
		[DataMember(Name = "OrganizerOnly", EmitDefaultValue = true)]
		public bool OrganizerOnly { get; set; }

		[XmlAttribute("Value")]
		[DataMember(Name = "Value", EmitDefaultValue = true)]
		public AutoPromoteEnum Value { get; set; }

		internal static AutoPromote ConvertFrom(AutomaticLeaderAssignment leaderAssignment)
		{
			AutoPromote autoPromote = new AutoPromote();
			switch (leaderAssignment)
			{
			case AutomaticLeaderAssignment.Disabled:
				autoPromote.OrganizerOnly = true;
				autoPromote.Value = AutoPromoteEnum.None;
				break;
			case AutomaticLeaderAssignment.SameEnterprise:
				autoPromote.OrganizerOnly = false;
				autoPromote.Value = AutoPromoteEnum.Company;
				break;
			case AutomaticLeaderAssignment.Everyone:
				autoPromote.OrganizerOnly = false;
				autoPromote.Value = AutoPromoteEnum.Everyone;
				break;
			default:
				throw new InvalidEnumArgumentException("Invalid value for AutomaticLeaderAssignment");
			}
			return autoPromote;
		}
	}
}
