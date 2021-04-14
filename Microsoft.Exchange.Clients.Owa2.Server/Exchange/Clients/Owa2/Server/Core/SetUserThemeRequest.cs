using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "SetUserThemeRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetUserThemeRequest : BaseJsonResponse
	{
		[DataMember(Name = "ThemeId", IsRequired = true)]
		public string ThemeId { get; set; }

		[DataMember(Name = "SkipO365Call", IsRequired = false)]
		public bool SkipO365Call { get; set; }
	}
}
