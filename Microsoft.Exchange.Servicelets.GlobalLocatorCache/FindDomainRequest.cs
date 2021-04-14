using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindDomainRequest
	{
		[DataMember(IsRequired = true)]
		public DomainQuery Domain { get; set; }

		[DataMember]
		public TenantQuery Tenant { get; set; }

		[DataMember(IsRequired = false)]
		public int ReadFlag { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			try
			{
				stringBuilder.Append("<FindDomainRequest>");
				stringBuilder.Append(this.Domain.ToString());
				if (this.Tenant != null)
				{
					stringBuilder.Append(this.Tenant.ToString());
				}
				stringBuilder.AppendFormat("ReadFlag={0}", this.ReadFlag);
				stringBuilder.Append("</FindDomainRequest>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
