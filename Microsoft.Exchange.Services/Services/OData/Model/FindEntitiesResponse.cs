using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class FindEntitiesResponse<TEntity> : ODataResponse<IFindEntitiesResult<TEntity>> where TEntity : Entity
	{
		public FindEntitiesResponse(FindEntitiesRequest<TEntity> request) : base(request)
		{
		}
	}
}
