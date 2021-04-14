using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class BusyTypeConverter : EnumConverter
	{
		public static BusyType Parse(string busyTypeValue)
		{
			if (busyTypeValue != null)
			{
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000c17-1 == null)
				{
					<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000c17-1 = new Dictionary<string, int>(6)
					{
						{
							"Busy",
							0
						},
						{
							"Free",
							1
						},
						{
							"OOF",
							2
						},
						{
							"Tentative",
							3
						},
						{
							"WorkingElsewhere",
							4
						},
						{
							"NoData",
							5
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000c17-1.TryGetValue(busyTypeValue, out num))
				{
					BusyType result;
					switch (num)
					{
					case 0:
						result = BusyType.Busy;
						break;
					case 1:
						result = BusyType.Free;
						break;
					case 2:
						result = BusyType.OOF;
						break;
					case 3:
						result = BusyType.Tentative;
						break;
					case 4:
						if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
						{
							result = BusyType.WorkingElseWhere;
						}
						else
						{
							result = BusyType.Free;
						}
						break;
					case 5:
						result = BusyType.Unknown;
						break;
					default:
						goto IL_C4;
					}
					return result;
				}
			}
			IL_C4:
			throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid busyType string: {0}", new object[]
			{
				busyTypeValue
			}));
		}

		public static string ToString(BusyType propertyValue)
		{
			string result = null;
			switch (propertyValue)
			{
			case BusyType.Unknown:
				result = "NoData";
				break;
			case BusyType.Free:
				result = "Free";
				break;
			case BusyType.Tentative:
				result = "Tentative";
				break;
			case BusyType.Busy:
				result = "Busy";
				break;
			case BusyType.OOF:
				result = "OOF";
				break;
			case BusyType.WorkingElseWhere:
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
				{
					result = "WorkingElsewhere";
				}
				else
				{
					result = "Free";
				}
				break;
			}
			return result;
		}

		public override object ConvertToObject(string propertyString)
		{
			return BusyTypeConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return BusyTypeConverter.ToString((BusyType)propertyValue);
		}

		private const string BusyStringValue = "Busy";

		private const string FreeStringValue = "Free";

		private const string OOFStringValue = "OOF";

		private const string TentativeStringValue = "Tentative";

		private const string WorkingElsewhereStringValue = "WorkingElsewhere";

		private const string UnknownStringValue = "NoData";
	}
}
