using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal abstract class WebServiceCallState<TRequest, TResponse> : WebServiceCall where TRequest : class where TResponse : class
	{
		internal WebServiceCallState(WebServiceUserInformation userInformation, IExchangeAsyncDispatch exchangeAsyncDispatch, AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.userInformation = userInformation;
			this.exchangeAsyncDispatch = exchangeAsyncDispatch;
		}

		protected IExchangeAsyncDispatch ExchangeAsyncDispatch
		{
			get
			{
				return this.exchangeAsyncDispatch;
			}
		}

		protected WebServiceUserInformation UserInformation
		{
			get
			{
				return this.userInformation;
			}
		}

		protected abstract string Name { get; }

		protected abstract Trace Tracer { get; }

		protected abstract void InternalBegin(TRequest request);

		protected abstract void InternalBeginCleanup(bool isSuccessful);

		protected abstract TResponse InternalEnd(ICancelableAsyncResult asyncResult);

		protected abstract void InternalEndCleanup();

		protected abstract TResponse InternalFailure(uint serviceCode);

		public IAsyncResult Begin(TRequest request)
		{
			try
			{
				Exception ex = null;
				uint num = 0U;
				bool isSuccessful = false;
				try
				{
					this.InternalBegin(request);
					isSuccessful = true;
				}
				catch (RpcException ex2)
				{
					ex = ex2;
					num = (uint)ex2.ErrorCode;
				}
				catch (ThreadAbortException ex3)
				{
					ex = ex3;
					num = 1726U;
				}
				catch (OutOfMemoryException ex4)
				{
					ex = ex4;
					num = 14U;
				}
				catch (Exception ex5)
				{
					ex = ex5;
					this.exception = ex5;
				}
				finally
				{
					this.InternalBeginCleanup(isSuccessful);
				}
				if (num != 0U || ex != null)
				{
					WebServiceEndPoint.LogFailure(string.Format("Begin{0}: Service failure: serviceCode={1}", this.Name, num), ex, this.userInformation.EmailAddress, this.userInformation.Domain, this.userInformation.Organization, this.Tracer);
					base.InvokeCallback();
				}
				if (num != 0U)
				{
					this.response = this.InternalFailure(num);
				}
			}
			catch (Exception ex6)
			{
				WebServiceEndPoint.LogFailure(string.Format("Begin{0}: Unhandled exception thrown.", this.Name), ex6, this.userInformation.EmailAddress, this.userInformation.Domain, this.userInformation.Organization, this.Tracer);
				throw;
			}
			return this;
		}

		public TResponse End()
		{
			TResponse result;
			try
			{
				base.WaitForCompletion();
				Exception ex = null;
				uint num = 0U;
				try
				{
					if (this.exception != null)
					{
						throw this.exception;
					}
					if (this.response != null)
					{
						return this.response;
					}
					try
					{
						this.response = this.InternalEnd(this.completeAsyncResult);
					}
					catch (RpcException ex2)
					{
						ex = ex2;
						num = (uint)ex2.ErrorCode;
					}
					catch (ThreadAbortException ex3)
					{
						ex = ex3;
						num = 1726U;
					}
					catch (OutOfMemoryException ex4)
					{
						ex = ex4;
						num = 14U;
					}
					catch (Exception ex5)
					{
						ex = ex5;
						this.exception = ex5;
					}
				}
				finally
				{
					this.InternalEndCleanup();
				}
				if (num != 0U || ex != null)
				{
					WebServiceEndPoint.LogFailure(string.Format("End{0}: Service failure: serviceCode={1}", this.Name, num), ex, this.userInformation.EmailAddress, this.userInformation.Domain, this.userInformation.Organization, this.Tracer);
				}
				if (num != 0U)
				{
					this.response = this.InternalFailure(num);
				}
				if (this.exception != null)
				{
					throw this.exception;
				}
				result = this.response;
			}
			catch (Exception ex6)
			{
				WebServiceEndPoint.LogFailure(string.Format("End{0}: Unhandled exception thrown.", this.Name), ex6, this.userInformation.EmailAddress, this.userInformation.Domain, this.userInformation.Organization, this.Tracer);
				throw;
			}
			return result;
		}

		public void Complete(ICancelableAsyncResult asyncResult)
		{
			this.completeAsyncResult = asyncResult;
			base.InvokeCallback();
		}

		private readonly WebServiceUserInformation userInformation;

		private readonly IExchangeAsyncDispatch exchangeAsyncDispatch;

		private ICancelableAsyncResult completeAsyncResult;

		private Exception exception;

		private TResponse response = default(TResponse);
	}
}
