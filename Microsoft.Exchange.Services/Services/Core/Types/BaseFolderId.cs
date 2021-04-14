using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(DistinguishedFolderId))]
	[XmlType(TypeName = "BaseFolderIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(AddressListId))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(DistinguishedFolderId))]
	[XmlInclude(typeof(FolderId))]
	[KnownType(typeof(FolderId))]
	public abstract class BaseFolderId : ServiceObjectId
	{
		internal override BasicTypes BasicType
		{
			get
			{
				return BasicTypes.Folder;
			}
		}
	}
}
