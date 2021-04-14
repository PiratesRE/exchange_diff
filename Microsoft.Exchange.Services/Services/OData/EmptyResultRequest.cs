using System;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class EmptyResultRequest : ODataRequest<EmptyResult>
	{
		public EmptyResultRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
