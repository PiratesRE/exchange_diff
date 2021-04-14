using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IPropertyContainer : IProperty
	{
		IList<IProperty> Children { get; }

		void SetCopyDestination(IPropertyContainer dstPropertyContainer);
	}
}
