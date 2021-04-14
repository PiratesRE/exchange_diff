using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetUserRetentionPolicyTagsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserRetentionPolicyTagsResponse : ResponseMessage
	{
		public GetUserRetentionPolicyTagsResponse()
		{
		}

		internal GetUserRetentionPolicyTagsResponse(ServiceResultCode code, ServiceError error, RetentionPolicyTag[] retentionPolicyTags) : base(code, error)
		{
			if (retentionPolicyTags != null && retentionPolicyTags.Length > 0)
			{
				this.retentionPolicyTags.AddRange(retentionPolicyTags);
			}
		}

		[XmlArrayItem("RetentionPolicyTag", Type = typeof(RetentionPolicyTag), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray]
		[DataMember(Name = "RetentionPolicyTags", IsRequired = false)]
		public RetentionPolicyTag[] RetentionPolicyTags
		{
			get
			{
				return this.retentionPolicyTags.ToArray();
			}
			set
			{
				this.retentionPolicyTags.Clear();
				if (value != null && value.Length > 0)
				{
					this.retentionPolicyTags.AddRange(value);
				}
			}
		}

		private List<RetentionPolicyTag> retentionPolicyTags = new List<RetentionPolicyTag>();
	}
}
