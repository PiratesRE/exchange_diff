using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "SetUserThemeResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetUserThemeResponse : BaseJsonResponse
	{
		[DataMember(Name = "OwaSuccess", Order = 1)]
		public bool OwaSuccess { get; set; }

		[DataMember(Name = "O365Success", Order = 2)]
		public bool O365Success { get; set; }
	}
}
