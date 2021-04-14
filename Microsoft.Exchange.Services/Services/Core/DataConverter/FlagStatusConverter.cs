using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class FlagStatusConverter : EnumConverter
	{
		public static string ToString(FlagStatus propertyValue)
		{
			string result = "NotFlagged";
			switch (propertyValue)
			{
			case FlagStatus.NotFlagged:
				result = "NotFlagged";
				break;
			case FlagStatus.Complete:
				result = "Complete";
				break;
			case FlagStatus.Flagged:
				result = "Flagged";
				break;
			}
			return result;
		}

		public override string ConvertToString(object propertyValue)
		{
			return FlagStatusConverter.ToString((FlagStatus)propertyValue);
		}

		public override object ConvertToObject(string propertyString)
		{
			throw new NotImplementedException();
		}

		private const string NotFlagged = "NotFlagged";

		private const string Flagged = "Flagged";

		private const string Complete = "Complete";
	}
}
