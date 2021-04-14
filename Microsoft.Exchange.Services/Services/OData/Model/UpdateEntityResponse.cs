using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class UpdateEntityResponse<TEntity> : ODataResponse<TEntity> where TEntity : Entity
	{
		public UpdateEntityResponse(UpdateEntityRequest<TEntity> request) : base(request)
		{
		}
	}
}
