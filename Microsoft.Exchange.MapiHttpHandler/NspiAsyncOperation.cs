using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class NspiAsyncOperation : AsyncOperation
	{
		protected NspiAsyncOperation(HttpContextBase context, AsyncOperationCookieFlags cookieFlags) : base(context, NspiAsyncOperation.cookieVdirPath, cookieFlags)
		{
		}

		protected IntPtr NspiContextHandle
		{
			get
			{
				IntPtr? intPtr = (IntPtr?)base.ContextHandle;
				if (intPtr == null)
				{
					return IntPtr.Zero;
				}
				return intPtr.GetValueOrDefault();
			}
			set
			{
				if (value == IntPtr.Zero)
				{
					base.ContextHandle = null;
					return;
				}
				if (this.NspiContextHandle != IntPtr.Zero && value != this.NspiContextHandle)
				{
					throw new InvalidOperationException("Context handle can only be set to zero once set.");
				}
				base.ContextHandle = new IntPtr?(value);
			}
		}

		public override string GetTraceBeginParameters(MapiHttpRequest mapiHttpRequest)
		{
			base.CheckDisposed();
			return AsyncOperation.CombineTraceParameters(base.GetTraceBeginParameters(mapiHttpRequest), string.Format("contextHandle={0}", this.NspiContextHandle));
		}

		public override string GetTraceEndParameters(MapiHttpRequest mapiHttpRequest, MapiHttpResponse mapiHttpResponse)
		{
			base.CheckDisposed();
			if (mapiHttpResponse.StatusCode != 0U)
			{
				return AsyncOperation.CombineTraceParameters(base.GetTraceEndParameters(mapiHttpRequest, mapiHttpResponse), string.Format("Failed; statusCode={0}, contextHandle={1}", mapiHttpResponse.StatusCode, this.NspiContextHandle));
			}
			if (mapiHttpResponse is MapiHttpOperationResponse && ((MapiHttpOperationResponse)mapiHttpResponse).ReturnCode != 0U)
			{
				return AsyncOperation.CombineTraceParameters(base.GetTraceEndParameters(mapiHttpRequest, mapiHttpResponse), string.Format("Failed; statusCode={0}. errorCode={1}, contextHandle={2}", mapiHttpResponse.StatusCode, ((MapiHttpOperationResponse)mapiHttpResponse).ReturnCode, this.NspiContextHandle));
			}
			return AsyncOperation.CombineTraceParameters(base.GetTraceEndParameters(mapiHttpRequest, mapiHttpResponse), string.Format("Success; contextHandle={0}", this.NspiContextHandle));
		}

		private static readonly string cookieVdirPath = AsyncOperation.CreateCookieVdirPath(MapiHttpEndpoints.VdirPathNspi);
	}
}
