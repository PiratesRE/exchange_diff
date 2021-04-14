using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal struct FreeBusyConverter : IConverter<BusyType, FreeBusyStatus>, IConverter<FreeBusyStatus, BusyType>
	{
		public FreeBusyStatus Convert(BusyType value)
		{
			return FreeBusyConverter.mappingConverter.Convert(value);
		}

		public BusyType Convert(FreeBusyStatus value)
		{
			return FreeBusyConverter.mappingConverter.Reverse(value);
		}

		private static SimpleMappingConverter<BusyType, FreeBusyStatus> mappingConverter = SimpleMappingConverter<BusyType, FreeBusyStatus>.CreateStrictConverter(new Tuple<BusyType, FreeBusyStatus>[]
		{
			new Tuple<BusyType, FreeBusyStatus>(BusyType.Unknown, FreeBusyStatus.Unknown),
			new Tuple<BusyType, FreeBusyStatus>(BusyType.Free, FreeBusyStatus.Free),
			new Tuple<BusyType, FreeBusyStatus>(BusyType.Tentative, FreeBusyStatus.Tentative),
			new Tuple<BusyType, FreeBusyStatus>(BusyType.Busy, FreeBusyStatus.Busy),
			new Tuple<BusyType, FreeBusyStatus>(BusyType.OOF, FreeBusyStatus.Oof),
			new Tuple<BusyType, FreeBusyStatus>(BusyType.WorkingElseWhere, FreeBusyStatus.WorkingElsewhere)
		});
	}
}
