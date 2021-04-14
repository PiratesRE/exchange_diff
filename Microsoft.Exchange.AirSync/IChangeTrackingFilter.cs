using System;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal interface IChangeTrackingFilter
	{
		int?[] Filter(XmlNode xmlItemRoot, int?[] oldChangeTrackInfo);

		int?[] UpdateChangeTrackingInformation(XmlNode xmlItemRoot, int?[] oldChangeTrackInfo);
	}
}
