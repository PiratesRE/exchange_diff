using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class GetEntityResponse<TEntity> : ODataResponse<TEntity> where TEntity : Entity
	{
		public GetEntityResponse(GetEntityRequest<TEntity> request) : base(request)
		{
		}
	}
}
