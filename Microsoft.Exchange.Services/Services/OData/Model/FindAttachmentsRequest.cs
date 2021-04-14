using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class FindAttachmentsRequest : FindEntitiesRequest<Attachment>
	{
		public FindAttachmentsRequest(ODataContext odataContext) : base(odataContext)
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
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.RootItemId);
		}

		public override ODataCommand GetODataCommand()
		{
			return new FindAttachmentsCommand(this);
		}
	}
}
