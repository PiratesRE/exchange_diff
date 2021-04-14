using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MeetingRegistrationResponseObjectType : WellKnownResponseObjectType
	{
		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string ProposedStart { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ProposedEnd { get; set; }

		public bool HasTimeProposal
		{
			get
			{
				return this.ProposedStart != null && this.ProposedEnd != null;
			}
		}

		public bool HasStartAndEndProposedTimeOrNone()
		{
			return !(this.ProposedStart == null ^ this.ProposedEnd == null);
		}
	}
}
