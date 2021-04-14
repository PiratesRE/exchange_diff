using System;
using System.IO;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal interface IMultipartResponse
	{
		void BuildResponse(XmlNode responseNode, int partnumber);

		Stream GetResponseStream();
	}
}
