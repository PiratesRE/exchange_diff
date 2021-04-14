using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class CreateAttachmentRequest : CreateEntityRequest<Attachment>
	{
		public CreateAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string RootItemId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.EntitySegment is NavigationPropertySegment && base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment)
			{
				this.RootItemId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
			}
			base.Template.ParentItemNavigationName = this.ParentItemNavigationName;
		}

		protected abstract string ParentItemNavigationName { get; }

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.RootItemId);
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateAttachmentCommand(this);
		}
	}
}
