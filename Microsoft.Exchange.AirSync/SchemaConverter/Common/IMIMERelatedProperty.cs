using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IMIMERelatedProperty : IProperty
	{
		bool IsOnSMIMEMessage { get; }
	}
}
