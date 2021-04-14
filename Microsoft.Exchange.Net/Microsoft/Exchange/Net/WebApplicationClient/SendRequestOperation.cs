using System;
using System.IO;
using System.Net;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal sealed class SendRequestOperation : AsyncResult
	{
		public SendRequestOperation(HttpWebRequest request, RequestBody requestBody, AsyncCallback callback, object asyncState) : base(callback, asyncState)
		{
			this.Request = request;
			this.requestBody = requestBody;
			if (requestBody != null)
			{
				if (requestBody.ContentType != null)
				{
					this.Request.ContentType = requestBody.ContentType.ToString();
				}
				this.Request.BeginGetRequestStream(new AsyncCallback(this.WriteBody), null);
				return;
			}
			this.Request.BeginGetResponse(new AsyncCallback(this.ReadResponse), null);
		}

		public HttpWebRequest Request { get; private set; }

		public HttpWebResponse Response { get; private set; }

		public long BytesSent
		{
			get
			{
				if (!base.IsCompleted)
				{
					throw new InvalidOperationException();
				}
				return this.Request.Headers.ToByteArray().LongLength + Math.Max(this.Request.ContentLength, 0L);
			}
		}

		public long BytesReceived
		{
			get
			{
				if (!base.IsCompleted)
				{
					throw new InvalidOperationException();
				}
				if (this.Response == null)
				{
					return 0L;
				}
				return this.Response.Headers.ToByteArray().LongLength + Math.Max(this.Response.ContentLength, 0L);
			}
		}

		private void WriteBody(IAsyncResult results)
		{
			try
			{
				using (Stream stream = this.Request.EndGetRequestStream(results))
				{
					this.requestBody.Write(stream);
				}
				this.Request.BeginGetResponse(new AsyncCallback(this.ReadResponse), null);
			}
			catch (Exception exception)
			{
				base.Complete(exception, false);
			}
		}

		private void ReadResponse(IAsyncResult results)
		{
			try
			{
				this.Response = (HttpWebResponse)this.Request.EndGetResponse(results);
				if (!this.Request.HaveResponse || this.Response == null)
				{
					base.Complete(new WebException(NetException.NoResponseFromHttpServer, WebExceptionStatus.ReceiveFailure), false);
				}
				else
				{
					base.Complete(null, false);
				}
			}
			catch (WebException ex)
			{
				this.Response = (ex.Response as HttpWebResponse);
				base.Complete(ex, false);
			}
			catch (Exception exception)
			{
				base.Complete(exception, false);
			}
		}

		private RequestBody requestBody;
	}
}
