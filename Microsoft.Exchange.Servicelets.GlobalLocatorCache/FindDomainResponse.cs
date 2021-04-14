using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindDomainResponse : ResponseBase
	{
		[DataMember]
		public DomainInfo DomainInfo { get; set; }

		[DataMember]
		public TenantInfo TenantInfo { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			try
			{
				stringBuilder.Append("<FindDomainResponse>");
				if (this.DomainInfo != null)
				{
					stringBuilder.Append(this.DomainInfo.ToString());
				}
				if (this.TenantInfo != null)
				{
					stringBuilder.Append(this.TenantInfo.ToString());
				}
				stringBuilder.AppendFormat("TransactionID={0},Diagnostics={1}", base.TransactionID, base.Diagnostics);
				stringBuilder.Append("</FindDomainResponse>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
