using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	internal interface ISmtpClientDebugOutput
	{
		void Output(Trace tracer, object context, string message, params object[] args);
	}
}
