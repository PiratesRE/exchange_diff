using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TraceToHeadersLogger : ITraceLogger
	{
		public TraceToHeadersLogger(NameValueCollection headers)
		{
			ArgumentValidator.ThrowIfNull("headers", headers);
			this.headers = headers;
		}

		public void LogTraces(ITracer tracer)
		{
			if (tracer == null || NullTracer.Instance.Equals(tracer))
			{
				return;
			}
			this.StampTracesOnHeader(tracer, this.headers);
		}

		private void StampTracesOnHeader(ITracer tracer, NameValueCollection headers)
		{
			headers["X-Exchange-Server-Traces"] = this.CollectTraces(tracer);
		}

		private string CollectTraces(ITracer tracer)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				tracer.Dump(stringWriter, true, true);
			}
			return this.SanitizeHttpHeaderValue(stringBuilder.ToString());
		}

		private string SanitizeHttpHeaderValue(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			foreach (char c in s)
			{
				if (c == '\r')
				{
					stringBuilder.Append("\\r");
				}
				else if (c == '\n')
				{
					stringBuilder.Append("\\n");
				}
				else if (c >= ' ' && c < '\u007f')
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('.');
				}
			}
			return stringBuilder.ToString();
		}

		private const char Space = ' ';

		private const char Delete = '\u007f';

		private const string ServerTracesHeaderName = "X-Exchange-Server-Traces";

		private readonly NameValueCollection headers;
	}
}
