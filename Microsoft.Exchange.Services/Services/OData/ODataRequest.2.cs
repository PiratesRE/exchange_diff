using System;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ODataRequest<TResult> : ODataRequest
	{
		public ODataRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
