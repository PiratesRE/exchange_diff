using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IProperty
	{
		int SchemaLinkId { get; set; }

		PropertyState State { get; set; }

		void CopyFrom(IProperty srcProperty);

		string ToString();
	}
}
