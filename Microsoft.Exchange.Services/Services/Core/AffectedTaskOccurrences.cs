using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal static class AffectedTaskOccurrences
	{
		public static AffectedTaskOccurrencesType ConvertToEnum(string affectedTaskOccurencesValue)
		{
			AffectedTaskOccurrencesType result = AffectedTaskOccurrencesType.AllOccurrences;
			if (affectedTaskOccurencesValue != null)
			{
				if (!(affectedTaskOccurencesValue == "AllOccurrences"))
				{
					if (affectedTaskOccurencesValue == "SpecifiedOccurrenceOnly")
					{
						result = AffectedTaskOccurrencesType.SpecifiedOccurrenceOnly;
					}
				}
				else
				{
					result = AffectedTaskOccurrencesType.AllOccurrences;
				}
			}
			return result;
		}

		public const string AllOccurrences = "AllOccurrences";

		public const string SpecifiedOccurrenceOnly = "SpecifiedOccurrenceOnly";
	}
}
