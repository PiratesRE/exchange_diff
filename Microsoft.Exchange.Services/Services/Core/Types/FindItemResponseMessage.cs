using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("FindItemResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindItemResponseMessage : ResponseMessage
	{
		public FindItemResponseMessage()
		{
		}

		internal FindItemResponseMessage(ServiceResultCode code, ServiceError error, FindItemParentWrapper parentWrapper, HighlightTermType[] highlightTerms, bool isSearchInProgress, FolderId searchFolderId) : base(code, error)
		{
			this.parentFolder = parentWrapper;
			this.HighlightTerms = highlightTerms;
			this.IsSearchInProgress = isSearchInProgress;
			this.SearchFolderId = searchFolderId;
		}

		[DataMember(Name = "RootFolder")]
		[XmlElement("RootFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindItemParentWrapper ParentFolder
		{
			get
			{
				return this.parentFolder;
			}
			set
			{
				this.parentFolder = value;
			}
		}

		[DataMember]
		[XmlArrayItem(ElementName = "Term", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray(ElementName = "HighlightTerms")]
		public HighlightTermType[] HighlightTerms { get; set; }

		[DataMember(Name = "IsSearchInProgress", EmitDefaultValue = false)]
		[XmlIgnore]
		public bool IsSearchInProgress { get; set; }

		[DataMember(Name = "SearchFolderId", EmitDefaultValue = false)]
		[XmlIgnore]
		public FolderId SearchFolderId { get; set; }

		private FindItemParentWrapper parentFolder = new FindItemParentWrapper();
	}
}
