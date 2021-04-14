using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[XmlType(TypeName = "AttachmentRefinerDataEntryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class AttachmentRefinerDataEntryType : RefinerDataEntryType
	{
		public AttachmentRefinerDataEntryType()
		{
		}

		public AttachmentRefinerDataEntryType(bool withAttachments, long hitCount, string refinementQuery) : base(hitCount, refinementQuery)
		{
			this.WithAttachments = withAttachments;
		}

		[DataMember(IsRequired = true)]
		public bool WithAttachments { get; set; }
	}
}
