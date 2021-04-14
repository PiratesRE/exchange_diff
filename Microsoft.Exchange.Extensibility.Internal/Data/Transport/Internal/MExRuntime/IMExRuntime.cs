using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal interface IMExRuntime
	{
		int AgentCount { get; }

		void Initialize(string configFile, string agentGroup, ProcessTransportRole processTransportRole, string installPath, FactoryInitializer factoryInitializer = null);

		IMExSession CreateSession(ICloneableInternal state, string name);

		IMExSession CreateSession(ICloneableInternal state, string name, Func<bool> resumeAgentCallback);

		IMExSession CreateSession(ICloneableInternal state, string name, Action startAsyncAgentCallback, Action completeAsyncAgentCallback, Func<bool> resumeAgentCallback);

		void Shutdown();

		string GetAgentName(int agentSequenceNumber);

		XElement[] GetDiagnosticInfo(DiagnosableParameters parameters, string agentType);
	}
}
