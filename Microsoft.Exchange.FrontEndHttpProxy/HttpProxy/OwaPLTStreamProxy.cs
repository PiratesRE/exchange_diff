using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaPLTStreamProxy : StreamProxy
	{
		internal OwaPLTStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer, IRequestContext requestContext) : base(streamProxyType, source, target, buffer, requestContext)
		{
		}

		protected override byte[] GetUpdatedBufferToSend(ArraySegment<byte> buffer)
		{
			string @string = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			if (!string.IsNullOrEmpty(@string) && base.RequestContext != null && base.RequestContext.HttpContext.Response != null)
			{
				base.RequestContext.HttpContext.Response.AppendToLog(@string);
			}
			return base.GetUpdatedBufferToSend(buffer);
		}
	}
}
