using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class MoveFolderSoapRequest : BaseSoapRequest
	{
		[MessageBodyMember(Name = "MoveFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public MoveFolderRequest Body;
	}
}
