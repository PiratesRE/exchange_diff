using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class DeleteEntityResponse<TEntity> : EmptyResultResponse where TEntity : Entity
	{
		public DeleteEntityResponse(DeleteEntityRequest<TEntity> request) : base(request)
		{
		}

		protected override HttpStatusCode HttpResponseCodeOnSuccess
		{
			get
			{
				return HttpStatusCode.NoContent;
			}
		}
	}
}
