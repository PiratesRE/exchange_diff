using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IFlowManager
	{
		void EnsureIndexingFlow();

		void EnsureQueryFlows(string indexSystemName);

		void EnsureTransportFlow();

		XElement GetFlowDiagnostics();

		IEnumerable<string> GetFlows();

		void RemoveFlowsForIndexSystem(string indexSystemName);

		void AddCtsFlow(string flowName, string flowXML);

		bool RemoveCTSFlow(string flowName);

		ICollection<FlowDescriptor> GetExpectedFlowsForIndexSystem(string indexSystemName);

		string GetFlow(string flowName);
	}
}
