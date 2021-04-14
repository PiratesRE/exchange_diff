using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "SearchFolder")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchFolderType : FolderType
	{
		[DataMember(EmitDefaultValue = false)]
		public SearchParametersType SearchParameters
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SearchParametersType>(SearchFolderSchema.SearchParameters);
			}
			set
			{
				base.PropertyBag[SearchFolderSchema.SearchParameters] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.SearchFolder;
			}
		}
	}
}
