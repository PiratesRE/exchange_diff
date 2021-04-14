using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.MailSubmission
{
	internal abstract class MailSubmissionServiceRpcServer : RpcServerBase
	{
		public abstract byte[] SubmitMessage(byte[] inBlob);

		public abstract SubmissionStatus SubmitMessage(string MessageClass, string ServerDN, string ServerSADN, byte[] MailboxGuid, byte[] EntryId, byte[] ParentEntryId, byte[] MdbGuid, int eventCounter, byte[] ipAddress);

		public abstract SubmissionStatus SubmitDumpsterMessages(string StorageGroupDN, long startTime, long endTime);

		public abstract AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid MdbGuid, long startTime, long endTime, string[] unresponsivePrimaryServers, byte[] reservedBytes);

		public abstract MailSubmissionResult SubmitMessage2(string ServerDN, Guid MailboxGuid, Guid MdbGuid, int eventCounter, byte[] EntryId, byte[] ParentEntryId, string serverFQDN, byte[] ipAddress);

		[return: MarshalAs(UnmanagedType.U1)]
		public abstract bool QueryDumpsterStats(string StorageGroupDN, ref long ticksOfOldestDeliveryTime, ref long queueSize, ref int numberOfItems);

		public abstract byte[] ShadowHeartBeat(byte[] inBlob);

		public abstract byte[] GetResubmitRequest(byte[] inBytes);

		public abstract byte[] SetResubmitRequest(byte[] inBytes);

		public abstract byte[] RemoveResubmitRequest(byte[] inBytes);

		public abstract AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTime, long endTime, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes);

		public MailSubmissionServiceRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IBridgeheadSubmission_v1_0_s_ifspec;

		public byte[] m_sdLocalSystemBinaryForm;

		public SubmissionStatus m_rpcStatus;

		public static OutOfMemoryException Oome = new OutOfMemoryException();
	}
}
