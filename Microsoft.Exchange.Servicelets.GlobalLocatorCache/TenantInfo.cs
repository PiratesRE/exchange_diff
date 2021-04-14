using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class TenantInfo
	{
		[DataMember(IsRequired = true)]
		public Guid TenantId { get; set; }

		[DataMember(IsRequired = true)]
		public List<KeyValuePair<string, string>> Properties { get; set; }

		[DataMember]
		public List<string> NoneExistNamespaces { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			try
			{
				stringBuilder.AppendFormat("<TenantInfo>TenantId={0},Props=", this.TenantId.ToString());
				if (this.Properties != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in this.Properties)
					{
						stringBuilder.AppendFormat("{0}:{1};", keyValuePair.Key, keyValuePair.Value);
					}
				}
				stringBuilder.Append("</TenantInfo>");
			}
			catch (Exception ex)
			{
				stringBuilder.Append(" ??TraceErr" + ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
