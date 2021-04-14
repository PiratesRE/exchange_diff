using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindDomainsRequest
	{
		[DataMember(IsRequired = true)]
		public List<string> DomainsName { get; set; }

		[DataMember(IsRequired = true)]
		public List<string> DomainPropertyNames { get; set; }

		[DataMember(IsRequired = false)]
		public List<string> TenantPropertyNames { get; set; }

		[DataMember(IsRequired = false)]
		public int ReadFlag { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			try
			{
				stringBuilder.Append("<FindDomainsRequest>Domains=");
				foreach (string arg in this.DomainsName)
				{
					stringBuilder.AppendFormat("{0};", arg);
				}
				stringBuilder.Append(",Props=");
				foreach (string arg2 in this.DomainPropertyNames)
				{
					stringBuilder.AppendFormat("{0};", arg2);
				}
				stringBuilder.Append(",TenantProps=");
				foreach (string arg3 in this.TenantPropertyNames)
				{
					stringBuilder.AppendFormat("{0};", arg3);
				}
				stringBuilder.AppendFormat(",ReadFlag={0}", this.ReadFlag.ToString());
				stringBuilder.Append("</FindDomainsRequest>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
