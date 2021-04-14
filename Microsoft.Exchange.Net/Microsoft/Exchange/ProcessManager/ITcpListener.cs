using System;
using System.Net;

namespace Microsoft.Exchange.ProcessManager
{
	internal interface ITcpListener
	{
		int MaxConnectionRate { set; }

		bool ProcessStopping { get; set; }

		IPEndPoint[] CurrentBindings { get; }

		bool IsListening();

		void SetBindings(IPEndPoint[] newBindings, bool invokeDelegateOnFailure);

		void StartListening(bool invokeDelegateOnFailure);

		void StopListening();

		void Shutdown();
	}
}
