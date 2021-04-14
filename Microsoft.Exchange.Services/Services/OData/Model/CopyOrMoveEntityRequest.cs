using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class CopyOrMoveEntityRequest<TEntity> : EntityActionRequest<TEntity> where TEntity : Entity
	{
		public CopyOrMoveEntityRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string DestinationId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			object obj;
			if (base.Parameters.TryGetValue("DestinationId", out obj))
			{
				this.DestinationId = (string)obj;
			}
		}

		public override void Validate()
		{
			base.Validate();
			ValidationHelper.ValidateParameterEmpty("DestinationId", this.DestinationId);
		}
	}
}
