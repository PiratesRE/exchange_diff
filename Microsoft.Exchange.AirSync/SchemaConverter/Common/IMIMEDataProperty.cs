using System;
using System.IO;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IMIMEDataProperty : IMIMERelatedProperty, IProperty
	{
		Stream MIMEData { get; set; }

		long MIMESize { get; set; }
	}
}
