using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "AlternatePublicFolderIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AlternatePublicFolderId : AlternateIdBase
	{
		public AlternatePublicFolderId()
		{
		}

		internal AlternatePublicFolderId(string folderId, IdFormat format) : base(format)
		{
			this.folderId = folderId;
		}

		[XmlAttribute]
		public string FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		internal override CanonicalConvertedId Parse()
		{
			return AlternateIdBase.GetIdConverter(base.Format).Parse(this);
		}

		private string folderId;
	}
}
