using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class UpdateEntityRequest<TEntity> : ODataRequest<TEntity> where TEntity : Entity
	{
		public UpdateEntityRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string Id { get; protected set; }

		public TEntity Change { get; protected set; }

		public string ChangeKey
		{
			get
			{
				return base.IfMatchETag;
			}
		}

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			this.Id = base.ODataContext.ODataPath.ResolveEntityKey();
			this.Change = base.ReadPostBodyAsEntity<TEntity>();
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateIdEmpty(this.Id);
		}
	}
}
