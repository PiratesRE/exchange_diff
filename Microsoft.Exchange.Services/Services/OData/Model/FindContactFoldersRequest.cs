using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	[AllowedOAuthGrant("Contacts.Read")]
	internal class FindContactFoldersRequest : FindEntitiesRequest<ContactFolder>
	{
		public FindContactFoldersRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string ParentFolderId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			ODataPathSegment parentOfEntitySegment = base.ODataContext.ODataPath.ParentOfEntitySegment;
			if (parentOfEntitySegment.EdmType.Equals(User.EdmEntityType))
			{
				this.ParentFolderId = DistinguishedFolderIdName.contacts.ToString();
				return;
			}
			if (parentOfEntitySegment is KeySegment && parentOfEntitySegment.EdmType.Equals(ContactFolder.EdmEntityType))
			{
				this.ParentFolderId = parentOfEntitySegment.GetIdKey();
				return;
			}
			this.ParentFolderId = DistinguishedFolderIdName.contacts.ToString();
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.ParentFolderId);
		}

		public override ODataCommand GetODataCommand()
		{
			return new FindContactFoldersCommand(this);
		}
	}
}
