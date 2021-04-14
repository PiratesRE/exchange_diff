using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Read")]
	[AllowedOAuthGrant("Mail.Write")]
	internal class FindFoldersRequest : FindEntitiesRequest<Folder>
	{
		public FindFoldersRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string ParentFolderId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			ODataPathSegment parentOfEntitySegment = base.ODataContext.ODataPath.ParentOfEntitySegment;
			if (parentOfEntitySegment is KeySegment && parentOfEntitySegment.EdmType.Equals(Folder.EdmEntityType))
			{
				this.ParentFolderId = parentOfEntitySegment.GetIdKey();
				return;
			}
			if (parentOfEntitySegment is NavigationPropertySegment && parentOfEntitySegment.EdmType.Equals(Folder.EdmEntityType))
			{
				this.ParentFolderId = parentOfEntitySegment.GetPropertyName();
				return;
			}
			this.ParentFolderId = DistinguishedFolderIdName.msgfolderroot.ToString();
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.ParentFolderId);
		}

		public override ODataCommand GetODataCommand()
		{
			return new FindFoldersCommand(this);
		}
	}
}
