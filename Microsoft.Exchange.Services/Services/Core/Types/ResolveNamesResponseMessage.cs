using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ResolveNamesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ResolveNamesResponseMessage : ResponseMessage
	{
		public ResolveNamesResponseMessage()
		{
		}

		internal ResolveNamesResponseMessage(ServiceResultCode code, ServiceError error, ResolutionSet resolutionSet) : base(code, error)
		{
			this.resolutionSet = resolutionSet;
		}

		[XmlElement("ResolutionSet")]
		[DataMember(Name = "ResolutionSet", EmitDefaultValue = false, Order = 1)]
		public ResolutionSet ResolutionSet
		{
			get
			{
				return this.resolutionSet;
			}
			set
			{
				this.resolutionSet = value;
			}
		}

		private ResolutionSet resolutionSet;
	}
}
