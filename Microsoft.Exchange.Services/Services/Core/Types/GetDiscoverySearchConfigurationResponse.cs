using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetDiscoverySearchConfigurationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetDiscoverySearchConfigurationResponse : ResponseMessage
	{
		public GetDiscoverySearchConfigurationResponse()
		{
		}

		internal GetDiscoverySearchConfigurationResponse(ServiceResultCode code, ServiceError error, DiscoverySearchConfiguration[] configuration) : base(code, error)
		{
			if (configuration != null && configuration.Length > 0)
			{
				this.configurations.AddRange(configuration);
			}
		}

		[DataMember(Name = "DiscoverySearchConfiguration", IsRequired = false)]
		[XmlArray]
		[XmlArrayItem("DiscoverySearchConfiguration", Type = typeof(DiscoverySearchConfiguration), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public DiscoverySearchConfiguration[] DiscoverySearchConfigurations
		{
			get
			{
				return this.configurations.ToArray();
			}
			set
			{
				this.configurations.Clear();
				if (value != null && value.Length > 0)
				{
					this.configurations.AddRange(value);
				}
			}
		}

		private List<DiscoverySearchConfiguration> configurations = new List<DiscoverySearchConfiguration>();
	}
}
