using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchParametersType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public RestrictionType Restriction { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		[XmlArrayItem("FolderId", typeof(FolderId), IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), IsNullable = false)]
		public BaseFolderId[] BaseFolderIds { get; set; }

		[IgnoreDataMember]
		[XmlAttribute]
		public SearchFolderTraversalType Traversal { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Name = "Traversal", Order = 0)]
		public string TraversalString
		{
			get
			{
				return EnumUtilities.ToString<SearchFolderTraversalType>(this.Traversal);
			}
			set
			{
				this.Traversal = EnumUtilities.Parse<SearchFolderTraversalType>(value);
				this.TraversalSpecified = true;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool TraversalSpecified { get; set; }
	}
}
