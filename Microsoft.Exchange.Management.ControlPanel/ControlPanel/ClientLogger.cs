using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientLogger : ExtensibleLogger
	{
		private ClientLogger() : base(new ClientLogConfiguration())
		{
		}

		internal static readonly ClientLogger Instance = new ClientLogger();
	}
}
