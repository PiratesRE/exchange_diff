using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class GetKillBit : BaseAsyncOmexCommand
	{
		public GetKillBit(OmexWebServiceUrlsCache urlsCache) : base(urlsCache, "GetKillBit")
		{
		}

		public void Execute(GetKillBit.SuccessCallback successCallback, BaseAsyncCommand.FailureCallback failureCallback)
		{
			if (successCallback == null)
			{
				throw new ArgumentNullException("successCallback");
			}
			if (failureCallback == null)
			{
				throw new ArgumentNullException("failureCallback");
			}
			this.successCallback = successCallback;
			this.failureCallback = failureCallback;
			if (this.urlsCache.IsInitialized)
			{
				this.InternalExecute();
				return;
			}
			this.urlsCache.Initialize(new OmexWebServiceUrlsCache.InitializeCompletionCallback(this.UrlsCacheInitializationCompletionCallback));
		}

		private void UrlsCacheInitializationCompletionCallback(bool isInitialized)
		{
			if (isInitialized)
			{
				this.InternalExecute();
				return;
			}
			this.InternalFailureCallback(null, "UrlsCache initialization failed. Killbit method won't be called");
		}

		private void InternalExecute()
		{
			base.ResetRequestID();
			Uri uri = new Uri(this.urlsCache.KillbitUrl + string.Format("?rt=XML&corr={0}", this.requestId));
			base.InternalExecute(uri);
		}

		protected override void ParseResponse(byte[] responseBuffer, int responseBufferSize)
		{
			XDocument xdocument = null;
			int num = 0;
			IOException ex;
			using (MemoryStream memoryStream = new MemoryStream(responseBuffer, 0, responseBufferSize))
			{
				try
				{
					ex = null;
					xdocument = XDocument.Load(memoryStream);
					xdocument.Save(KillBitHelper.KillBitFilePath);
				}
				catch (DirectoryNotFoundException exception)
				{
					this.InternalFailureCallback(exception, null);
					return;
				}
				catch (IOException ex2)
				{
					BaseAsyncCommand.Tracer.TraceWarning(0L, ex2.Message);
					ex = ex2;
					num++;
					Thread.Sleep(50);
				}
				catch (XmlException exception2)
				{
					this.InternalFailureCallback(exception2, null);
					return;
				}
				goto IL_B4;
			}
			try
			{
				IL_71:
				ex = null;
				xdocument.Save(KillBitHelper.KillBitFilePath);
			}
			catch (IOException ex3)
			{
				BaseAsyncCommand.Tracer.TraceWarning(0L, ex3.Message);
				ex = ex3;
				if (num >= 3)
				{
					this.InternalFailureCallback(ex3, null);
					return;
				}
				num++;
				Thread.Sleep(50);
			}
			IL_B4:
			if (ex != null && xdocument != null)
			{
				goto IL_71;
			}
			base.LogResponseParsed();
			this.successCallback();
		}

		private const string KillbitQueryStringFormat = "?rt=XML&corr={0}";

		private const int MaxRetryCount = 3;

		private const int RetryTimeInterval = 50;

		private GetKillBit.SuccessCallback successCallback;

		internal delegate void SuccessCallback();
	}
}
