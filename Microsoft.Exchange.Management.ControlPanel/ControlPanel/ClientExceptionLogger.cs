using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientExceptionLogger : ExtensibleLogger
	{
		private ClientExceptionLogger() : base(new ClientExceptionLogConfiguration())
		{
		}

		internal static readonly ClientExceptionLogger Instance = new ClientExceptionLogger();
	}
}
