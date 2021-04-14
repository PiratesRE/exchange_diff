using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ResponseTypeConverter : EnumConverter
	{
		public static ResponseType Parse(string stringResponseType)
		{
			if (stringResponseType != null)
			{
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000c92-1 == null)
				{
					<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000c92-1 = new Dictionary<string, int>(6)
					{
						{
							"Accept",
							0
						},
						{
							"Decline",
							1
						},
						{
							"Unknown",
							2
						},
						{
							"NoResponseReceived",
							3
						},
						{
							"Organizer",
							4
						},
						{
							"Tentative",
							5
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000c92-1.TryGetValue(stringResponseType, out num))
				{
					ResponseType result;
					switch (num)
					{
					case 0:
						result = ResponseType.Accept;
						break;
					case 1:
						result = ResponseType.Decline;
						break;
					case 2:
						result = ResponseType.None;
						break;
					case 3:
						result = ResponseType.NotResponded;
						break;
					case 4:
						result = ResponseType.Organizer;
						break;
					case 5:
						result = ResponseType.Tentative;
						break;
					default:
						goto IL_B1;
					}
					return result;
				}
			}
			IL_B1:
			throw new FormatException(string.Format(CultureInfo.InvariantCulture, "ResponseType type not supported '{0}'", new object[]
			{
				stringResponseType
			}));
		}

		public static string ToString(ResponseType responseType)
		{
			string result = "Unknown";
			switch (responseType)
			{
			case ResponseType.None:
				result = "Unknown";
				break;
			case ResponseType.Organizer:
				result = "Organizer";
				break;
			case ResponseType.Tentative:
				result = "Tentative";
				break;
			case ResponseType.Accept:
				result = "Accept";
				break;
			case ResponseType.Decline:
				result = "Decline";
				break;
			case ResponseType.NotResponded:
				result = "NoResponseReceived";
				break;
			}
			return result;
		}

		public override object ConvertToObject(string propertyString)
		{
			return ResponseTypeConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return ResponseTypeConverter.ToString((ResponseType)propertyValue);
		}

		private const string Accept = "Accept";

		private const string Decline = "Decline";

		private const string Unknown = "Unknown";

		private const string NoResponseReceived = "NoResponseReceived";

		private const string Organizer = "Organizer";

		private const string Tentative = "Tentative";
	}
}
