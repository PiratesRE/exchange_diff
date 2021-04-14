using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class ODataResponse<TResult> : ODataResponse
	{
		public ODataResponse(ODataRequest request) : base(request)
		{
		}

		public TResult Result
		{
			get
			{
				return (TResult)((object)base.InternalResult);
			}
			set
			{
				base.InternalResult = value;
			}
		}
	}
}
