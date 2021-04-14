using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface ITaskState : IProperty
	{
		bool Complete { get; }

		ExDateTime? DateCompleted { get; }
	}
}
