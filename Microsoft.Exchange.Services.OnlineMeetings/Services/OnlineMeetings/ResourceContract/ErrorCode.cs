using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "code")]
	internal enum ErrorCode
	{
		[EnumMember]
		BadRequest = 400,
		[EnumMember]
		Forbidden = 403,
		[EnumMember]
		ResourceNotFound,
		[EnumMember]
		MethodNotAllowed,
		[EnumMember]
		Conflict = 409,
		[EnumMember]
		InvalidOperation = 422,
		[EnumMember]
		TooManyRequests = 429,
		[EnumMember]
		ServiceFailure = 500
	}
}
