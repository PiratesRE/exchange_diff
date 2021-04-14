using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateFavoriteFolderRequest
	{
		[DataMember(Name = "Folder", IsRequired = true)]
		public BaseFolderType Folder { get; set; }

		[DataMember(Name = "Operation", IsRequired = true)]
		public UpdateFavoriteOperationType Operation { get; set; }

		[DataMember(Name = "TargetFolderId", IsRequired = false)]
		public FolderId TargetFolderId { get; set; }

		[DataMember(Name = "MoveType", IsRequired = false)]
		public MoveFavoriteFolderType? MoveType { get; set; }
	}
}
