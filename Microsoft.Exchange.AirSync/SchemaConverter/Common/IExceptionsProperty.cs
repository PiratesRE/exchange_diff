using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IExceptionsProperty : IMultivaluedProperty<ExceptionInstance>, IProperty, IEnumerable<ExceptionInstance>, IEnumerable, IDataObjectGeneratorContainer
	{
	}
}
