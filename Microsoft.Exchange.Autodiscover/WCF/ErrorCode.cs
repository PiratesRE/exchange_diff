using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "ErrorCode", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public enum ErrorCode
	{
		[EnumMember]
		NoError,
		[EnumMember]
		RedirectAddress,
		[EnumMember]
		RedirectUrl,
		[EnumMember]
		InvalidUser,
		[EnumMember]
		InvalidRequest,
		[EnumMember]
		InvalidSetting,
		[EnumMember]
		SettingIsNotAvailable,
		[EnumMember]
		ServerBusy,
		[EnumMember]
		InvalidDomain,
		[EnumMember]
		NotFederated,
		[EnumMember]
		InternalServerError
	}
}
