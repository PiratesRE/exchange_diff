using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface IExchangeProxy
	{
		ExchangeService ExchangeService { get; }

		TIn Create<TIn, TOut>() where TIn : SimpleServiceRequestBase where TOut : ServiceResponse;

		IEnumerable<TOut> Execute<TIn, TOut>(TIn request) where TIn : SimpleServiceRequestBase where TOut : ServiceResponse;

		IAsyncResult BeginExecute<TIn, TOut>(AsyncCallback callback, object state, TIn request) where TIn : SimpleServiceRequestBase where TOut : ServiceResponse;

		void Abort(IAsyncResult result);

		IEnumerable<TOut> EndExecute<TIn, TOut>(IAsyncResult result) where TIn : SimpleServiceRequestBase where TOut : ServiceResponse;
	}
}
