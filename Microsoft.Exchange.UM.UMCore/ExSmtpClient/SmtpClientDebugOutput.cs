using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ExSmtpClient
{
	internal class SmtpClientDebugOutput : ISmtpClientDebugOutput
	{
		public void Output(Trace tracer, object context, string message, params object[] args)
		{
			CallIdTracer.TraceDebug(tracer, context, message, args);
		}
	}
}
