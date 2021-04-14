using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PredictedActionReasonConverter : EnumConverter
	{
		public static PredictedActionReasonType Parse(string propertyString)
		{
			ushort num = 0;
			if (!ushort.TryParse(propertyString, out num))
			{
				return PredictedActionReasonType.None;
			}
			if (Enum.IsDefined(typeof(PredictedActionReasonType), (int)num))
			{
				return (PredictedActionReasonType)num;
			}
			return PredictedActionReasonType.None;
		}

		public static string ToString(ushort propertyValue)
		{
			if (Enum.IsDefined(typeof(PredictedActionReasonType), propertyValue))
			{
				return Enum.GetName(typeof(PredictedActionReasonType), propertyValue);
			}
			return Enum.GetName(typeof(PredictedActionReasonType), PredictedActionReasonType.None);
		}

		protected override object ConvertToServiceObjectValue(object propertyValue)
		{
			return PredictedActionReasonConverter.ConvertToServiceObjectValue((ushort)propertyValue);
		}

		internal static PredictedActionReasonType ConvertToServiceObjectValue(ushort propertyValue)
		{
			PredictedActionReasonType result = PredictedActionReasonType.None;
			if (Enum.IsDefined(typeof(PredictedActionReasonType), (int)propertyValue))
			{
				result = (PredictedActionReasonType)propertyValue;
			}
			return result;
		}

		public override object ConvertToObject(string propertyString)
		{
			return PredictedActionReasonConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return PredictedActionReasonConverter.ToString((ushort)propertyValue);
		}
	}
}
