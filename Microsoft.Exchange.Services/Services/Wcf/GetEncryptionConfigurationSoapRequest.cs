using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class GetEncryptionConfigurationSoapRequest : BaseSoapRequest
	{
		[MessageBodyMember(Name = "GetEncryptionConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public GetEncryptionConfigurationRequest Body;
	}
}
