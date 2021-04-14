using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ODataCommand<TRequest, TResponse> : ODataCommand where TRequest : ODataRequest where TResponse : ODataResponse
	{
		public ODataCommand(TRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.Request = request;
		}

		public TRequest Request { get; private set; }

		public override object Execute()
		{
			object result;
			try
			{
				result = this.InternalExecute();
			}
			catch (Exception)
			{
				throw;
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ODataCommand<TRequest, TResponse>>(this);
		}

		protected abstract TResponse InternalExecute();
	}
}
