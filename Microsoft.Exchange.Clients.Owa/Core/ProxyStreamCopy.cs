using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class ProxyStreamCopy
	{
		internal ProxyStreamCopy(object source, Stream destination, StreamCopyMode requestedCopyMode)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			Type type = source.GetType();
			if (type.IsSubclassOf(typeof(Stream)))
			{
				this.sourceType = ProxyStreamCopy.SourceType.Stream;
			}
			else
			{
				if (!(type == typeof(string)))
				{
					throw new ArgumentException("Source is of an invalid type");
				}
				this.sourceType = ProxyStreamCopy.SourceType.String;
			}
			if ((this.copyMode & (StreamCopyMode)4) != (StreamCopyMode)0 && this.sourceType == ProxyStreamCopy.SourceType.String)
			{
				throw new ArgumentException("Async read is not supported for string source");
			}
			this.source = source;
			this.destination = destination;
			this.copyMode = requestedCopyMode;
			this.completedSynchronously = true;
		}

		internal IAsyncResult BeginCopy(AsyncCallback callback, object extraData)
		{
			this.result = new OwaAsyncResult(callback, extraData);
			if (this.ProcessNextSyncBatch(ProxyStreamCopy.CopyStage.BeginRead, null))
			{
				this.completedSynchronously = true;
				this.result.CompleteRequest(this.completedSynchronously);
			}
			return this.result;
		}

		internal int EndCopy(IAsyncResult result)
		{
			OwaAsyncResult owaAsyncResult = (OwaAsyncResult)result;
			if (owaAsyncResult.Exception != null)
			{
				throw owaAsyncResult.Exception;
			}
			return this.totalBytesCopied;
		}

		private bool ProcessNextSyncBatch(ProxyStreamCopy.CopyStage stage, IAsyncResult result)
		{
			ProxyStreamCopy.CopyStage copyStage = stage;
			IAsyncResult asyncResult = result;
			Stream stream = this.source as Stream;
			Stream stream2 = this.destination;
			for (;;)
			{
				if (copyStage == ProxyStreamCopy.CopyStage.EndWrite)
				{
					stream2.EndWrite(asyncResult);
				}
				if (copyStage == ProxyStreamCopy.CopyStage.BeginRead || copyStage == ProxyStreamCopy.CopyStage.EndWrite)
				{
					if (this.sourceType == ProxyStreamCopy.SourceType.String)
					{
						this.bytesRead = this.ReadNextStringChunk();
					}
					else if ((this.copyMode & (StreamCopyMode)1) != (StreamCopyMode)0)
					{
						this.bytesRead = stream.Read(this.Buffer, 0, this.Buffer.Length);
						copyStage = ProxyStreamCopy.CopyStage.BeginWrite;
						asyncResult = null;
					}
					else
					{
						IAsyncResult asyncResult2 = stream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(ProxyStreamCopy.ReadCallback), this);
						if (!asyncResult2.CompletedSynchronously)
						{
							break;
						}
						copyStage = ProxyStreamCopy.CopyStage.EndRead;
						asyncResult = asyncResult2;
					}
				}
				if (copyStage == ProxyStreamCopy.CopyStage.EndRead)
				{
					this.bytesRead = stream.EndRead(asyncResult);
				}
				if (this.bytesRead <= 0)
				{
					return true;
				}
				this.totalBytesCopied += this.bytesRead;
				bool flag;
				if ((this.copyMode & (StreamCopyMode)2) != (StreamCopyMode)0)
				{
					stream2.Write(this.Buffer, 0, this.bytesRead);
					copyStage = ProxyStreamCopy.CopyStage.BeginRead;
					asyncResult = null;
					flag = true;
				}
				else
				{
					IAsyncResult asyncResult3 = stream2.BeginWrite(this.Buffer, 0, this.bytesRead, new AsyncCallback(ProxyStreamCopy.WriteCallback), this);
					if (!asyncResult3.CompletedSynchronously)
					{
						goto IL_158;
					}
					copyStage = ProxyStreamCopy.CopyStage.EndWrite;
					asyncResult = asyncResult3;
					flag = true;
				}
				if (!flag)
				{
					return true;
				}
			}
			this.completedSynchronously = false;
			return false;
			IL_158:
			this.completedSynchronously = false;
			return false;
		}

		private void InternalStageCallback(ProxyStreamCopy.CopyStage nextStage, IAsyncResult result)
		{
			bool flag = false;
			if (result.CompletedSynchronously)
			{
				return;
			}
			try
			{
				if (this.ProcessNextSyncBatch(nextStage, result))
				{
					flag = true;
				}
			}
			catch (Exception exception)
			{
				this.result.CompleteRequest(false, exception);
				return;
			}
			if (flag)
			{
				this.result.CompleteRequest(false);
			}
		}

		public static void ReadCallback(IAsyncResult result)
		{
			ProxyStreamCopy proxyStreamCopy = (ProxyStreamCopy)result.AsyncState;
			proxyStreamCopy.InternalStageCallback(ProxyStreamCopy.CopyStage.EndRead, result);
		}

		public static void WriteCallback(IAsyncResult result)
		{
			ProxyStreamCopy proxyStreamCopy = (ProxyStreamCopy)result.AsyncState;
			proxyStreamCopy.InternalStageCallback(ProxyStreamCopy.CopyStage.EndWrite, result);
		}

		private int ReadNextStringChunk()
		{
			string text = (string)this.source;
			if (this.totalCharsRead >= text.Length)
			{
				return 0;
			}
			int num = 4096;
			if (this.totalCharsRead + num > text.Length)
			{
				num = text.Length - this.totalCharsRead;
			}
			char[] chars = text.ToCharArray(this.totalCharsRead, num);
			int byteCount = this.Encoding.GetByteCount(chars, 0, num);
			if (byteCount > this.Buffer.Length)
			{
				this.buffer = new byte[byteCount];
			}
			int bytes = this.Encoding.GetBytes(chars, 0, num, this.Buffer, 0);
			this.totalCharsRead += num;
			return bytes;
		}

		public Encoding Encoding
		{
			get
			{
				if (this.encoding == null)
				{
					this.encoding = Encoding.UTF8;
				}
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		private byte[] Buffer
		{
			get
			{
				if (this.buffer == null)
				{
					this.buffer = new byte[4096];
				}
				return this.buffer;
			}
		}

		private const int BufferSize = 4096;

		private object source;

		private Stream destination;

		private ProxyStreamCopy.SourceType sourceType;

		private OwaAsyncResult result;

		private int bytesRead;

		private int totalBytesCopied;

		private bool completedSynchronously;

		private StreamCopyMode copyMode;

		private byte[] buffer;

		private Encoding encoding;

		private int totalCharsRead;

		private enum SourceType
		{
			Stream,
			String
		}

		[Flags]
		private enum CopyStage
		{
			Read = 1,
			Write = 2,
			Begin = 256,
			End = 512,
			BeginRead = 257,
			EndRead = 513,
			BeginWrite = 258,
			EndWrite = 514
		}
	}
}
