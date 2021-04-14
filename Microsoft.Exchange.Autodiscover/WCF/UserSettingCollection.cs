using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[CollectionDataContract(Name = "UserSettings", ItemName = "UserSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class UserSettingCollection : Collection<UserSetting>
	{
	}
}
