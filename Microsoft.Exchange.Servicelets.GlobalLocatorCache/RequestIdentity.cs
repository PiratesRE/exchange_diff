using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class RequestIdentity
	{
		[DataMember(IsRequired = true)]
		public string CallerId { get; set; }

		[DataMember(IsRequired = false)]
		public Guid TrackingGuid { get; set; }

		[DataMember(IsRequired = false)]
		public Guid RequestTrackingGuid { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			try
			{
				stringBuilder.Append("<RequestIdentity>");
				stringBuilder.AppendFormat("CallerId={0},TrackingGuid={1},RequestTrackingGuid={2}", this.CallerId, this.TrackingGuid, this.RequestTrackingGuid);
				stringBuilder.Append("</RequestIdentity>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
