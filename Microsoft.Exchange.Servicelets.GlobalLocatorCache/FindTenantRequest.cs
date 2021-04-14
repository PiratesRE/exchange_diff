using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindTenantRequest
	{
		[DataMember(IsRequired = true)]
		public TenantQuery Tenant { get; set; }

		[DataMember(IsRequired = false)]
		public int ReadFlag { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			try
			{
				stringBuilder.Append("<FindTenantRequest>");
				stringBuilder.Append(this.Tenant.ToString());
				stringBuilder.AppendFormat("ReadFlag={0}", this.ReadFlag);
				stringBuilder.Append("</FindTenantRequest>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
