using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInMailItemStorage : ISmtpInMailItemStorage
	{
		public IAsyncResult BeginCommitMailItem(TransportMailItem mailItem, AsyncCallback callback, object state)
		{
			IAsyncResult result;
			try
			{
				result = mailItem.BeginCommitForReceive(callback, state);
			}
			catch (ExchangeDataException exception)
			{
				result = this.HandleException(exception, callback, state);
			}
			catch (IOException exception2)
			{
				result = this.HandleException(exception2, callback, state);
			}
			return result;
		}

		public bool EndCommitMailItem(TransportMailItem mailItem, IAsyncResult asyncResult, out Exception exception)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult != null)
			{
				SmtpInMailItemStorage smtpInMailItemStorage = lazyAsyncResult.AsyncObject as SmtpInMailItemStorage;
				if (smtpInMailItemStorage != null)
				{
					exception = (lazyAsyncResult.Result as Exception);
					return false;
				}
			}
			return mailItem.EndCommitForReceive(asyncResult, out exception);
		}

		public Task CommitMailItemAsync(TransportMailItem mailItem)
		{
			TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
			this.BeginCommitMailItem(mailItem, delegate(IAsyncResult ar)
			{
				TaskCompletionSource<object> taskCompletionSource2 = (TaskCompletionSource<object>)ar.AsyncState;
				Exception exception;
				if (this.EndCommitMailItem(mailItem, ar, out exception))
				{
					taskCompletionSource2.SetResult(null);
					return;
				}
				taskCompletionSource2.SetException(exception);
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		private IAsyncResult HandleException(Exception exception, AsyncCallback originalCallback, object originalState)
		{
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(this, originalState, originalCallback);
			lazyAsyncResult.InvokeCallback(exception);
			return lazyAsyncResult;
		}
	}
}
