using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "AlternatePublicFolderItemIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AlternatePublicFolderItemId : AlternatePublicFolderId
	{
		public AlternatePublicFolderItemId()
		{
		}

		internal AlternatePublicFolderItemId(string itemId, string folderId, IdFormat format) : base(folderId, format)
		{
			this.itemId = itemId;
		}

		[XmlAttribute]
		public string ItemId
		{
			get
			{
				return this.itemId;
			}
			set
			{
				this.itemId = value;
			}
		}

		internal override CanonicalConvertedId Parse()
		{
			return AlternateIdBase.GetIdConverter(base.Format).Parse(this);
		}

		private string itemId;
	}
}
