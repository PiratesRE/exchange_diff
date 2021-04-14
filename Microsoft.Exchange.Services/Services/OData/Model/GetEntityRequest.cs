using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class GetEntityRequest<TEntity> : ODataRequest<TEntity> where TEntity : Entity
	{
		public GetEntityRequest(ODataContext odataContext) : base(odataContext)
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
