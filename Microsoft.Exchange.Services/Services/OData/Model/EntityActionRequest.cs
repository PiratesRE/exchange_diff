using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class EntityActionRequest<TEntity> : CustomActionRequest where TEntity : Entity
	{
		public EntityActionRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string Id { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment)
			{
				this.Id = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
				return;
			}
			if (base.ODataContext.ODataPath.ParentOfEntitySegment is NavigationPropertySegment)
			{
				this.Id = base.ODataContext.ODataPath.ParentOfEntitySegment.GetPropertyName();
			}
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.Id);
		}
	}
}
