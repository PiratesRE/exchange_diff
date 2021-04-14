using System;
using System.Text;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class MwiFailureEventLogStrategy
	{
		internal abstract void LogFailure(MwiMessage message, Exception ex);

		protected virtual string ConstructErrorMessage(MwiMessage message, Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(ex.Message);
			foreach (MwiDeliveryException ex2 in message.DeliveryErrors)
			{
				stringBuilder.AppendLine(ex2.Message);
			}
			return stringBuilder.ToString();
		}
	}
}
