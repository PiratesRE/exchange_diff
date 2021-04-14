using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class EmsmdbAsyncOperation : AsyncOperation
	{
		protected EmsmdbAsyncOperation(HttpContextBase context, AsyncOperationCookieFlags cookieFlags) : base(context, EmsmdbAsyncOperation.cookieVdirPath, cookieFlags)
		{
		}

		protected IntPtr EmsmdbContextHandle
		{
			get
			{
				IntPtr? intPtr = (IntPtr?)base.ContextHandle;
				if (intPtr == null)
				{
					return IntPtr.Zero;
				}
				return intPtr.Value;
			}
			set
			{
				if (value == IntPtr.Zero)
				{
					base.ContextHandle = null;
					return;
				}
				if (this.EmsmdbContextHandle != IntPtr.Zero && value != this.EmsmdbContextHandle)
				{
					throw new InvalidOperationException("Context handle can only be set to zero once set.");
				}
				base.ContextHandle = new IntPtr?(value);
			}
		}

		protected uint StatusCode { get; set; }

		protected int ErrorCode { get; set; }

		public override string GetTraceBeginParameters(MapiHttpRequest mapiHttpRequest)
		{
			base.CheckDisposed();
			return AsyncOperation.CombineTraceParameters(base.GetTraceBeginParameters(mapiHttpRequest), string.Format("contextHandle={0}", this.EmsmdbContextHandle));
		}

		public override string GetTraceEndParameters(MapiHttpRequest mapiHttpRequest, MapiHttpResponse mapiHttpResponse)
		{
			base.CheckDisposed();
			if (mapiHttpResponse.StatusCode != 0U)
			{
				return AsyncOperation.CombineTraceParameters(base.GetTraceEndParameters(mapiHttpRequest, mapiHttpResponse), string.Format("Failed; statusCode={0}, contextHandle={1}", mapiHttpResponse.StatusCode, this.EmsmdbContextHandle));
			}
			if (mapiHttpResponse is MapiHttpOperationResponse && ((MapiHttpOperationResponse)mapiHttpResponse).ReturnCode != 0U)
			{
				return AsyncOperation.CombineTraceParameters(base.GetTraceEndParameters(mapiHttpRequest, mapiHttpResponse), string.Format("Failed; errorCode={0}, contextHandle={1}", ((MapiHttpOperationResponse)mapiHttpResponse).ReturnCode, this.EmsmdbContextHandle));
			}
			return AsyncOperation.CombineTraceParameters(base.GetTraceEndParameters(mapiHttpRequest, mapiHttpResponse), string.Format("Success; contextHandle={0}", this.EmsmdbContextHandle));
		}

		private static readonly string cookieVdirPath = AsyncOperation.CreateCookieVdirPath(MapiHttpEndpoints.VdirPathEmsmdb);
	}
}
