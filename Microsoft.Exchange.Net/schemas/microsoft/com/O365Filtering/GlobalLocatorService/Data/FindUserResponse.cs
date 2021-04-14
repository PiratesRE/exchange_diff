using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "FindUserResponse", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindUserResponse : ResponseBase
	{
		[DataMember]
		public TenantInfo TenantInfo
		{
			get
			{
				return this.TenantInfoField;
			}
			set
			{
				this.TenantInfoField = value;
			}
		}

		[DataMember]
		public UserInfo UserInfo
		{
			get
			{
				return this.UserInfoField;
			}
			set
			{
				this.UserInfoField = value;
			}
		}

		private TenantInfo TenantInfoField;

		private UserInfo UserInfoField;
	}
}
