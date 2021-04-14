using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindTenantResponse : ResponseBase
	{
		[DataMember]
		public TenantInfo TenantInfo { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			try
			{
				stringBuilder.Append("<FindTenantResponse>");
				if (this.TenantInfo != null)
				{
					stringBuilder.Append(this.TenantInfo.ToString());
				}
				stringBuilder.Append(base.ToString());
				stringBuilder.Append("</FindTenantResponse>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
