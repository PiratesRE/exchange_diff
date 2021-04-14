using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("FindFolderResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class FindFolderResponseMessage : ResponseMessage
	{
		public FindFolderResponseMessage()
		{
		}

		internal FindFolderResponseMessage(ServiceResultCode code, ServiceError error, FindFolderParentWrapper parentWrapper) : base(code, error)
		{
			this.RootFolder = parentWrapper;
		}

		[DataMember(Name = "RootFolder", IsRequired = false)]
		[XmlElement("RootFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindFolderParentWrapper RootFolder { get; set; }
	}
}
