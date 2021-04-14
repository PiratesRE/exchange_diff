using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PeopleIdentity : INamedIdentity
	{
		public PeopleIdentity(string displayName, string legacyDN, string smtpAddress, int addressOrigin, string routingType)
		{
			this.DisplayName = displayName;
			this.Address = legacyDN;
			this.SmtpAddress = smtpAddress;
			this.RoutingType = routingType;
			this.AddressOrigin = addressOrigin;
		}

		[DataMember]
		public string DisplayName { get; private set; }

		[DataMember]
		public string Address { get; private set; }

		[DataMember]
		public string SmtpAddress { get; private set; }

		[DataMember]
		public string RoutingType { get; private set; }

		[DataMember]
		public int AddressOrigin { get; private set; }

		string INamedIdentity.Identity
		{
			get
			{
				if (this.RoutingType == "SMTP")
				{
					return string.Concat(new string[]
					{
						"\"",
						this.DisplayName,
						"\"<",
						this.SmtpAddress,
						">"
					});
				}
				return this.Address;
			}
		}
	}
}
