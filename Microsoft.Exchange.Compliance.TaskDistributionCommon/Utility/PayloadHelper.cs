using System;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility
{
	internal static class PayloadHelper
	{
		internal static PayloadReference GetPayloadReference(Guid jobRunId, int taskId = -1)
		{
			PayloadIdentifier payloadIdentifier = new PayloadIdentifier
			{
				JobRunId = jobRunId,
				TaskId = taskId
			};
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return new PayloadReference
			{
				PayloadId = javaScriptSerializer.Serialize(payloadIdentifier)
			};
		}

		internal static bool TryReadFromPayloadReference(PayloadReference payloadReference, out Guid jobRunId, out int taskId, out PayloadLevel payloadLevel)
		{
			jobRunId = Guid.Empty;
			taskId = -1;
			payloadLevel = PayloadLevel.Job;
			bool result;
			try
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				PayloadIdentifier payloadIdentifier = javaScriptSerializer.Deserialize<PayloadIdentifier>(payloadReference.PayloadId);
				jobRunId = payloadIdentifier.JobRunId;
				if (payloadIdentifier.TaskId > -1)
				{
					taskId = payloadIdentifier.TaskId;
					payloadLevel = PayloadLevel.Task;
				}
				else
				{
					payloadLevel = PayloadLevel.Job;
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}
	}
}
