using System;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ExchangeServiceCommand<TRequest, TResponse> : ODataCommand<TRequest, TResponse> where TRequest : ODataRequest where TResponse : ODataResponse
	{
		public ExchangeServiceCommand(TRequest request) : base(request)
		{
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.exchangeService != null)
			{
				this.exchangeService.Dispose();
				this.exchangeService = null;
			}
			base.InternalDispose(disposing);
		}

		protected IExchangeService ExchangeService
		{
			get
			{
				if (this.exchangeService == null)
				{
					ExchangeServiceFactory @default = ExchangeServiceFactory.Default;
					TRequest request = base.Request;
					this.exchangeService = @default.CreateForEws(request.ODataContext.CallContext);
				}
				return this.exchangeService;
			}
		}

		private IExchangeService exchangeService;
	}
}
