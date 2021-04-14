using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "GetRegionalConfigurationResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetRegionalConfigurationResponse : BaseJsonResponse
	{
		[DataMember(Name = "SupportedCultures", IsRequired = false)]
		public CultureInfoData[] SupportedCultures { get; set; }
	}
}
