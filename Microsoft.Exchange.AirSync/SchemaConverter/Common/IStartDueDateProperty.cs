using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IStartDueDateProperty : IProperty
	{
		ExDateTime? DueDate { get; }

		ExDateTime? StartDate { get; }

		ExDateTime? UtcDueDate { get; }

		ExDateTime? UtcStartDate { get; }
	}
}
