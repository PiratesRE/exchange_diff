using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PropertyErrorType
	{
		[DataMember(EmitDefaultValue = false)]
		public PropertyPath PropertyPath { get; set; }

		[IgnoreDataMember]
		public PropertyErrorCodeType ErrorCode { get; set; }

		[DataMember(Name = "ErrorCode", IsRequired = true)]
		public string ErrorCodeString
		{
			get
			{
				return EnumUtilities.ToString<PropertyErrorCodeType>(this.ErrorCode);
			}
			set
			{
				this.ErrorCode = EnumUtilities.Parse<PropertyErrorCodeType>(value);
			}
		}
	}
}
