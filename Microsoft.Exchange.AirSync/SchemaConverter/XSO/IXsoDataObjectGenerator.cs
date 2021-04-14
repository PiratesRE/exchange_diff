using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal interface IXsoDataObjectGenerator : IDataObjectGenerator
	{
		XsoDataObject GetInnerXsoDataObject();
	}
}
