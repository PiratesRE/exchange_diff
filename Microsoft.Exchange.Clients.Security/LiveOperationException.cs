using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Security
{
	public class LiveOperationException : LocalizedException
	{
		public string AdditionalWatsonData { get; set; }

		public LiveOperationException(COMException e, uint errorCode) : base(Strings.LiveOperationExceptionMessage(errorCode, Enum.GetName(typeof(RPSErrorCode), errorCode) ?? string.Empty), e)
		{
		}

		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(this.AdditionalWatsonData))
			{
				return base.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString());
			stringBuilder.AppendLine().AppendLine();
			stringBuilder.AppendLine("AdditionalWatsonData: ");
			stringBuilder.Append("\t").AppendLine(this.AdditionalWatsonData);
			return stringBuilder.ToString();
		}
	}
}
