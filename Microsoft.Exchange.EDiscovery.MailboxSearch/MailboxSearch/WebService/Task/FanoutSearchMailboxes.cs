using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class FanoutSearchMailboxes : SearchTask<FanoutParameters>
	{
		public override void Process(IList<FanoutParameters> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "FanoutSearchMailboxes.Process Item:", item);
			FanoutParameters fanoutParameters = item.First<FanoutParameters>();
			if (fanoutParameters.GroupId.GroupType == GroupType.Local)
			{
				this.ProcessLocal(item);
				return;
			}
			this.ProcessRemote(item);
		}

		private void ProcessLocal(IList<FanoutParameters> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "FanoutSearchMailboxes.ProcessLocal Item:", item);
			SearchMailboxesInputs searchMailboxesInputs = (SearchMailboxesInputs)base.Executor.Context.Input;
			searchMailboxesInputs = searchMailboxesInputs.Clone();
			searchMailboxesInputs.Sources = (from t in item
			select t.Source).ToList<SearchSource>();
			searchMailboxesInputs.IsLocalCall = true;
			Recorder.Record record = base.Policy.Recorder.Start("FanoutSyncTime", TraceType.InfoTrace, true);
			try
			{
				SearchMailboxesResults item2 = Controller.SeachMailboxes(base.Policy, searchMailboxesInputs);
				base.Executor.EnqueueNext(item2);
			}
			catch (SearchException ex)
			{
				Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
				{
					"FanoutSearchMailboxes.ProcessLocal Falied:",
					ex,
					"Count:",
					searchMailboxesInputs.Sources
				});
				foreach (SearchSource errorSource in searchMailboxesInputs.Sources)
				{
					base.Executor.Fail(new SearchException(ex)
					{
						ErrorSource = errorSource
					});
				}
			}
			finally
			{
				base.Policy.Recorder.End(record);
			}
		}

		private void ProcessRemote(IList<FanoutParameters> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "FanoutSearchMailboxes.ProcessRemote Item:", item);
			FanoutParameters first = item.First<FanoutParameters>();
			FanoutParameters.FanoutState state = new FanoutParameters.FanoutState();
			state.Proxy = SearchFactory.Current.GetProxy(base.Policy, first);
			state.Parameters = item;
			SearchMailboxesRequest request = state.Proxy.Create<SearchMailboxesRequest, SearchMailboxesResponse>();
			((SearchMailboxesInputs)base.Executor.Context.Input).UpdateRequest(request, from t in item
			select t.Source);
			this.ProtectedExecute(state, delegate
			{
				Recorder.Trace(4L, TraceType.InfoTrace, new object[]
				{
					"FanoutSearchMailboxes.ProtectedExecute.Method Uri:",
					first.GroupId.Uri,
					"Request:",
					request
				});
				state.Proxy.ExchangeService.OnResponseHeadersCaptured += delegate(WebHeaderCollection headers)
				{
					this.Policy.Recorder.Merge(first.GroupId.Uri.Host, headers);
				};
				this.Policy.Recorder.WriteTimestampHeader(state.Proxy.ExchangeService.HttpHeaders);
				return state.Proxy.Execute<SearchMailboxesRequest, SearchMailboxesResponse>(request);
			});
		}

		private void ProtectedExecute(FanoutParameters.FanoutState state, Func<IEnumerable<SearchMailboxesResponse>> method)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "FanoutSearchMailboxes.ProtectedExecute State:", state);
			Exception ex = null;
			try
			{
				foreach (SearchMailboxesResponse response in method())
				{
					this.Enqueue(state, response, ex);
				}
			}
			catch (ServiceResponseException ex2)
			{
				ex = ex2;
			}
			catch (ServiceRemoteException ex3)
			{
				ex = ex3;
			}
			catch (ServiceLocalException ex4)
			{
				ex = ex4;
			}
			catch (WebServiceProxyInvalidResponseException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				Recorder.Trace(4L, TraceType.ErrorTrace, "FanoutSearchMailboxes.ProtectedExecute Failed:", ex);
				this.Enqueue(state, null, ex);
			}
		}

		private void Enqueue(FanoutParameters.FanoutState state, SearchMailboxesResponse response, Exception exception)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"FanoutSearchMailboxes.Enqueue State:",
				state,
				"Response:",
				response,
				"Exception:",
				exception
			});
			SearchMailboxesResults searchMailboxesResults = new SearchMailboxesResults(null);
			searchMailboxesResults.UpdateResults(state.Parameters, (SearchMailboxesInputs)base.Executor.Context.Input, response, exception);
			base.Executor.EnqueueNext(searchMailboxesResults);
		}
	}
}
