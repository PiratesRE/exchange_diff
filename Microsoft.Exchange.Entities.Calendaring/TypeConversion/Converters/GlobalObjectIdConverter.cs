using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal struct GlobalObjectIdConverter : IConverter<GlobalObjectId, string>, IConverter<string, GlobalObjectId>
	{
		public string Convert(GlobalObjectId value)
		{
			if (value == null)
			{
				throw new ExArgumentNullException("value");
			}
			return value.ToString();
		}

		public GlobalObjectId Convert(string value)
		{
			if (value == null)
			{
				throw new ExArgumentNullException(value);
			}
			if (value.Length == 0)
			{
				throw new CorruptDataException(CalendaringStrings.ErrorInvalidIdentifier);
			}
			return new GlobalObjectId(value);
		}
	}
}
