using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class DeleteAttachmentRequest : DeleteEntityRequest<Attachment>
	{
		public DeleteAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string RootItemId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.GrandParentOfEntitySegment is KeySegment)
			{
				this.RootItemId = base.ODataContext.ODataPath.GrandParentOfEntitySegment.GetIdKey();
			}
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.RootItemId);
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteAttachmentCommand(this);
		}
	}
}
