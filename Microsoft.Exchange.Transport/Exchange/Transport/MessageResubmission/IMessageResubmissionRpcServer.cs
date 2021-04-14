using System;
using Microsoft.Exchange.Rpc.MailSubmission;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal interface IMessageResubmissionRpcServer
	{
		byte[] GetResubmitRequest(byte[] rawRequest);

		byte[] SetResubmitRequest(byte[] requestRaw);

		byte[] RemoveResubmitRequest(byte[] requestRaw);

		AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid mdbGuid, long startTimeInTicks, long endTimeInTicks, string[] unresponsivePrimaryServers, byte[] reservedBytes);

		AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTimeInTicks, long endTimeInTicks, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes);
	}
}
