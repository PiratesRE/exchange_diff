using System;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal interface ISearchProvider
	{
		int NumberResponses { get; }

		bool RightsManagementSupport { get; }

		void BuildResponse(XmlElement responseNode);

		void Execute();

		void ParseOptions(XmlElement optionsNode);

		void ParseQueryNode(XmlElement queryNode);
	}
}
