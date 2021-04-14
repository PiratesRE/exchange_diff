using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class CreateFolderRequest : CreateEntityRequest<Folder>
	{
		public CreateFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string ParentFolderId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.EntitySegment is NavigationPropertySegment)
			{
				if (base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment)
				{
					this.ParentFolderId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
					return;
				}
				if (base.ODataContext.ODataPath.ParentOfEntitySegment is NavigationPropertySegment)
				{
					this.ParentFolderId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetPropertyName();
				}
			}
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.ParentFolderId);
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateFolderCommand(this);
		}
	}
}
