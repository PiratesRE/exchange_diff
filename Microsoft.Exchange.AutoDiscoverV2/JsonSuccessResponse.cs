using System;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	[DataContract]
	internal class JsonSuccessResponse
	{
		[DataMember]
		public string Protocol { get; set; }

		[DataMember]
		public string Url { get; set; }

		public string SerializeToJson(object value)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(value);
		}
	}
}
