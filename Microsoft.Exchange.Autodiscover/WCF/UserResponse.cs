using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "UserResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class UserResponse : AutodiscoverResponse
	{
		public UserResponse()
		{
			this.UserSettings = new UserSettingCollection();
			this.UserSettingErrors = new UserSettingErrorCollection();
		}

		[DataMember(Name = "RedirectTarget", IsRequired = false)]
		public string RedirectTarget { get; set; }

		[DataMember(Name = "UserSettings", IsRequired = false)]
		public UserSettingCollection UserSettings { get; set; }

		[DataMember(Name = "UserSettingErrors", IsRequired = false)]
		public UserSettingErrorCollection UserSettingErrors { get; set; }
	}
}
