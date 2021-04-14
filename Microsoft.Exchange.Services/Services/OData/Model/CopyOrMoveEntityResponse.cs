using System;
using System.Collections.Specialized;
using System.Net;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class CopyOrMoveEntityResponse<TEntity> : ODataResponse<TEntity> where TEntity : Entity
	{
		public CopyOrMoveEntityResponse(CopyOrMoveEntityRequest<TEntity> request) : base(request)
		{
		}

		protected override HttpStatusCode HttpResponseCodeOnSuccess
		{
			get
			{
				return HttpStatusCode.Created;
			}
		}

		protected override void ApplyResponseHeaders(NameValueCollection responseHeaders)
		{
			base.ApplyResponseHeaders(responseHeaders);
			responseHeaders["Location"] = base.GetEntityLocation();
		}
	}
}
