using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class ResponseBase
	{
		[DataMember]
		public string TransactionID { get; set; }

		[DataMember]
		public string Diagnostics { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			try
			{
				stringBuilder.Append("<ResponseBase>");
				stringBuilder.AppendFormat("TransactionID={0},Diagnostics={1}", this.TransactionID, this.Diagnostics);
				stringBuilder.Append("</ResponseBase>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
