using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("FindPeopleResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FindPeopleResponseMessage : ResponseMessage
	{
		public FindPeopleResponseMessage()
		{
		}

		internal FindPeopleResponseMessage(ServiceResultCode code, ServiceError error, FindPeopleResult findPeopleResult) : base(code, error)
		{
			if (code == ServiceResultCode.Success)
			{
				this.ResultSet = findPeopleResult.PersonaList;
				this.TotalNumberOfPeopleInView = findPeopleResult.TotalNumberOfPeopleInView;
				this.FirstLoadedRowIndex = findPeopleResult.FirstLoadedRowIndex;
				this.FirstMatchingRowIndex = findPeopleResult.FirstMatchingRowIndex;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.FindPeopleResponseMessage;
		}

		[DataMember]
		[XmlArray(ElementName = "People")]
		[XmlArrayItem(ElementName = "Persona", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public Persona[] ResultSet { get; set; }

		[DataMember]
		public int TotalNumberOfPeopleInView { get; set; }

		[DataMember]
		public int FirstMatchingRowIndex { get; set; }

		[DataMember]
		public int FirstLoadedRowIndex { get; set; }
	}
}
