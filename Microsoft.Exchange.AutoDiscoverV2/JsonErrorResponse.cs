using System;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	[DataContract]
	public class JsonErrorResponse
	{
		[DataMember]
		public string ErrorCode { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		public string SerializeToJson(object value)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(value);
		}
	}
}
