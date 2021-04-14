using System;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal interface IItemOperationsProvider : IReusable
	{
		bool RightsManagementSupport { get; }

		void BuildErrorResponse(string statusCode, XmlNode responseNode, ProtocolLogger protocolLogger);

		void BuildResponse(XmlNode responseNode);

		void Execute();

		void ParseRequest(XmlNode fetchNode);
	}
}
