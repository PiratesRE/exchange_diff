using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal abstract class BaseTestStep : ITestStep
	{
		protected abstract void StartTest();

		protected virtual void Finally()
		{
		}

		protected virtual void ExceptionThrown(ScenarioException e)
		{
		}

		protected abstract TestId Id { get; }

		public virtual object Result
		{
			get
			{
				return null;
			}
		}

		public TimeSpan? MaxRunTime { get; set; }

		public IAsyncResult BeginExecute(IHttpSession session, AsyncCallback callback, object state)
		{
			this.session = session;
			this.asyncResult = new LazyAsyncResult(callback, state);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.KickoffTest), null);
			return this.asyncResult;
		}

		public void EndExecute(IAsyncResult result)
		{
			LazyAsyncResult lazyAsyncResult = result as LazyAsyncResult;
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (lazyAsyncResult.Exception != null)
			{
				Exception ex = lazyAsyncResult.Exception as Exception;
				throw ex;
			}
		}

		public Task CreateTask(IHttpSession session)
		{
			return Task.Factory.FromAsync<IHttpSession>(new Func<IHttpSession, AsyncCallback, object, IAsyncResult>(this.BeginExecute), new Action<IAsyncResult>(this.EndExecute), session, null, TaskCreationOptions.None);
		}

		protected void ExecutionCompletedSuccessfully()
		{
			this.session.NotifyTestFinished(this.Id);
			this.Finally();
			ScenarioException exception = null;
			if (this.MaxRunTime != null)
			{
				exception = this.session.VerifyScenarioExceededRunTime(this.MaxRunTime);
			}
			this.asyncResult.Complete(null, exception);
		}

		protected void AsyncCallbackWrapper(AsyncCallback wrappedCallback, IAsyncResult result)
		{
			try
			{
				wrappedCallback.DynamicInvoke(new object[]
				{
					result
				});
			}
			catch (Exception ex)
			{
				this.ReportException(ex);
				this.Finally();
				this.asyncResult.Complete(null, ex);
			}
		}

		private void KickoffTest(object target)
		{
			try
			{
				this.session.NotifyTestStarted(this.Id);
				this.StartTest();
			}
			catch (Exception ex)
			{
				this.ReportException(ex);
				this.Finally();
				this.asyncResult.Complete(null, ex);
			}
		}

		private void ReportException(Exception e)
		{
			ScenarioException scenarioException = e.GetScenarioException();
			if (scenarioException != null)
			{
				this.ExceptionThrown(e.GetScenarioException());
			}
		}

		protected const string UrlEncodedFormContentType = "application/x-www-form-urlencoded";

		protected const string JsonContentType = "application/json; charset=utf-8";

		protected LazyAsyncResult asyncResult;

		protected IHttpSession session;
	}
}
