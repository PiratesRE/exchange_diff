using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class DomainQuery
	{
		[DataMember(IsRequired = true)]
		public string DomainName { get; set; }

		[DataMember(IsRequired = false)]
		public List<string> PropertyNames { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			try
			{
				stringBuilder.AppendFormat("<DomainQuery>DomainName={0},Props=", this.DomainName);
				if (this.PropertyNames != null)
				{
					foreach (string arg in this.PropertyNames)
					{
						stringBuilder.AppendFormat("{0};", arg);
					}
				}
				stringBuilder.Append("</DomainQuery>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
