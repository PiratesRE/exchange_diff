using System;
using System.IO;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.SoapWebClient
{
	internal sealed class SoapHttpClientTraceExtension : SoapExtension
	{
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}

		public override object GetInitializer(Type WebServiceType)
		{
			return null;
		}

		public override void Initialize(object initializer)
		{
		}

		public override Stream ChainStream(Stream stream)
		{
			if (SoapHttpClientTraceExtension.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.oldStream = stream;
				this.newStream = new MemoryStream();
				this.reader = new StreamReader(this.newStream);
				return this.newStream;
			}
			return stream;
		}

		public override void ProcessMessage(SoapMessage message)
		{
			if (this.oldStream != null && this.newStream != null)
			{
				switch (message.Stage)
				{
				case SoapMessageStage.AfterSerialize:
					this.WriteRequest();
					return;
				case (SoapMessageStage)3:
					break;
				case SoapMessageStage.BeforeDeserialize:
					this.ReadResponse();
					break;
				default:
					return;
				}
			}
		}

		private void WriteRequest()
		{
			this.newStream.Position = 0L;
			string arg = this.reader.ReadToEnd();
			this.newStream.Position = 0L;
			if (this.oldStream.CanWrite)
			{
				try
				{
					SoapHttpClientTraceExtension.Copy(this.newStream, this.oldStream);
				}
				catch (NotSupportedException)
				{
				}
			}
			SoapHttpClientTraceExtension.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Request: {0}", arg);
		}

		private void ReadResponse()
		{
			string arg = "<stream not readable>";
			if (this.oldStream.CanRead)
			{
				try
				{
					SoapHttpClientTraceExtension.Copy(this.oldStream, this.newStream);
					this.newStream.Position = 0L;
					arg = this.reader.ReadToEnd();
					this.newStream.Position = 0L;
				}
				catch (NotSupportedException)
				{
				}
			}
			SoapHttpClientTraceExtension.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Response: {0}", arg);
		}

		private static void Copy(Stream from, Stream to)
		{
			byte[] array = new byte[1024];
			for (;;)
			{
				int num = from.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				to.Write(array, 0, num);
			}
		}

		private Stream oldStream;

		private Stream newStream;

		private StreamReader reader;

		private static Trace Tracer = ExTraceGlobals.EwsClientTracer;
	}
}
