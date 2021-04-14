using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "FolderIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class FolderId : BaseFolderId
	{
		[XmlAttribute]
		[DataMember(IsRequired = true, Order = 1)]
		public string Id { get; set; }

		[XmlAttribute]
		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
		public string ChangeKey { get; set; }

		internal bool IsPublicFolderId()
		{
			return ServiceIdConverter.IsPublicFolder(this.GetId());
		}

		public FolderId()
		{
		}

		public FolderId(string id, string changeKey)
		{
			this.Id = id;
			this.ChangeKey = changeKey;
		}

		public override string GetId()
		{
			return this.Id;
		}

		public override string GetChangeKey()
		{
			return this.ChangeKey;
		}
	}
}
