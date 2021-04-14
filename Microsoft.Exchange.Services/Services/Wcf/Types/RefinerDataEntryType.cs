using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[XmlInclude(typeof(PeopleRefinerDataEntryType))]
	[KnownType(typeof(PeopleRefinerDataEntryType))]
	[KnownType(typeof(AttachmentRefinerDataEntryType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RefinerDataEntryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(FolderRefinerDataEntryType))]
	[XmlInclude(typeof(AttachmentRefinerDataEntryType))]
	[KnownType(typeof(FolderRefinerDataEntryType))]
	[Serializable]
	public class RefinerDataEntryType
	{
		public RefinerDataEntryType()
		{
		}

		public RefinerDataEntryType(long hitCount, string refinementQuery)
		{
			this.HitCount = hitCount;
			this.RefinementQuery = refinementQuery;
		}

		[DataMember(IsRequired = true)]
		public string RefinementQuery { get; set; }

		[DataMember(IsRequired = true)]
		public long HitCount { get; set; }
	}
}
