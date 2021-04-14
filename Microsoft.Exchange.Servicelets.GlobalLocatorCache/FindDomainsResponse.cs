using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindDomainsResponse : ResponseBase
	{
		[DataMember]
		public List<FindDomainResponse> DomainsResponse { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(500);
			try
			{
				stringBuilder.Append("<FindDomainsResponse>");
				if (this.DomainsResponse != null)
				{
					foreach (FindDomainResponse findDomainResponse in this.DomainsResponse)
					{
						stringBuilder.Append(findDomainResponse.ToString());
					}
				}
				stringBuilder.Append(base.ToString());
				stringBuilder.Append("</FindDomainsResponse>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
