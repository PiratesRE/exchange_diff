using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AsyncWebRequest : AsyncRequest
	{
		public AsyncWebRequest(Application application, ClientContext clientContext, RequestLogger requestLogger, string traceLabel) : base(application, clientContext, requestLogger)
		{
			this.traceLabel = traceLabel;
		}

		public sealed override void BeginInvoke(TaskCompleteCallback callback)
		{
			if (this.ShouldCallBeginInvokeByNewThread)
			{
				ThreadPool.QueueUserWorkItem(delegate(object dummy)
				{
					ThreadContext.SetWithExceptionHandling(this.traceLabel + "ByNewThread", this.Application.Worker, this.ClientContext, this.RequestLogger, delegate
					{
						this.BeginInvokeInternal(callback);
					});
				});
				return;
			}
			this.BeginInvokeInternal(callback);
		}

		private void BeginInvokeInternal(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						this.asyncResult = this.BeginInvoke();
					}
					catch (SoapException exception)
					{
						this.HandleException(exception);
					}
					catch (UriFormatException exception2)
					{
						this.HandleException(exception2);
					}
					catch (WebException exception3)
					{
						this.HandleException(exception3);
					}
					catch (IOException ex2)
					{
						SocketException ex3 = ex2.InnerException as SocketException;
						if (ex3 == null)
						{
							ExWatson.SendReport(ex2, ReportOptions.None, null);
							this.HandleException(ex2);
						}
						else
						{
							this.HandleException(ex3);
						}
					}
					catch (LocalizedException exception4)
					{
						this.HandleException(exception4);
					}
				});
			}
			catch (GrayException ex)
			{
				this.HandleException(ex.InnerException);
			}
			if (this.asyncResult == null && this.CompleteAtomically())
			{
				this.Complete();
			}
		}

		protected abstract IAsyncResult BeginInvoke();

		protected abstract void EndInvoke(IAsyncResult asyncResult);

		protected abstract void HandleException(Exception exception);

		protected abstract bool IsImpersonating { get; }

		protected virtual bool ShouldCallBeginInvokeByNewThread
		{
			get
			{
				return false;
			}
		}

		protected void Complete(IAsyncResult asyncResult)
		{
			if (this.asyncResult == null)
			{
				this.asyncResult = asyncResult;
			}
			if (this.CompleteAtomically())
			{
				if (asyncResult.CompletedSynchronously)
				{
					this.CompleteInternal();
					return;
				}
				ThreadContext.SetWithExceptionHandling(this.traceLabel, base.Application.IOCompletion, base.ClientContext, base.RequestLogger, new ThreadContext.ExecuteDelegate(this.CompleteInternal));
			}
		}

		private new void Complete()
		{
			base.Complete();
		}

		private void CompleteInternal()
		{
			try
			{
				this.EndInvokeWithErrorHandling();
			}
			finally
			{
				if (this.asyncResult.CompletedSynchronously && this.IsImpersonating)
				{
					this.CompleteAsync();
				}
				else
				{
					this.Complete();
				}
			}
		}

		private void CompleteAsync()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompleteAsync));
		}

		private void CompleteAsync(object notUsed)
		{
			ThreadContext.SetWithExceptionHandling(this.traceLabel, base.Application.Worker, base.ClientContext, base.RequestLogger, new ThreadContext.ExecuteDelegate(this.Complete));
		}

		private bool CompleteAtomically()
		{
			return Interlocked.CompareExchange(ref this.completed, 1, 0) == 0;
		}

		private void EndInvokeWithErrorHandling()
		{
			try
			{
				this.EndInvoke(this.asyncResult);
			}
			catch (ArgumentException exception)
			{
				this.HandleException(exception);
			}
			catch (WebException exception2)
			{
				this.HandleException(exception2);
			}
			catch (SoapException exception3)
			{
				this.HandleException(exception3);
			}
			catch (InvalidOperationException exception4)
			{
				this.HandleException(exception4);
			}
			catch (IOException ex)
			{
				SocketException ex2 = ex.InnerException as SocketException;
				if (ex2 == null)
				{
					ExWatson.SendReport(ex, ReportOptions.None, null);
					this.HandleException(ex);
				}
				else
				{
					this.HandleException(ex2);
				}
			}
			catch (LocalizedException exception5)
			{
				this.HandleException(exception5);
			}
		}

		private int completed;

		private string traceLabel;

		private IAsyncResult asyncResult;

		public delegate void CompleteDelegate();
	}
}
