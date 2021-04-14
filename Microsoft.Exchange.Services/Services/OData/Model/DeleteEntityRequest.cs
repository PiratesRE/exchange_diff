using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class DeleteEntityRequest<TEntity> : EmptyResultRequest where TEntity : Entity
	{
		public DeleteEntityRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string Id { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			this.Id = base.ODataContext.ODataPath.ResolveEntityKey();
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.Id);
		}
	}
}
