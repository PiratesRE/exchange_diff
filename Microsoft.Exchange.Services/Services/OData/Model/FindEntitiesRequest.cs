using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class FindEntitiesRequest<TEntity> : ODataRequest<IFindEntitiesResult<TEntity>> where TEntity : Entity
	{
		public FindEntitiesRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
