using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	internal class CreateContactRequest : CreateEntityRequest<Contact>
	{
		public string ParentFolderId { get; protected set; }

		public CreateContactRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateContactCommand(this);
		}

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.EntitySegment is NavigationPropertySegment)
			{
				if (base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment && base.ODataContext.ODataPath.ParentOfEntitySegment.EdmType.Equals(Contact.EdmEntityType))
				{
					this.ParentFolderId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
					return;
				}
				if (string.Equals(base.ODataContext.ODataPath.EntitySegment.GetPropertyName(), UserSchema.Contacts.Name))
				{
					this.ParentFolderId = 1.ToString();
				}
			}
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.ParentFolderId);
		}
	}
}
