using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Rpc.MailSubmission;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal class MessageResubmissionRpcServerImpl : MailSubmissionServiceRpcServer
	{
		public static IMessageResubmissionRpcServer MessageResubmissionImpl
		{
			get
			{
				IMessageResubmissionRpcServer result;
				if ((result = MessageResubmissionRpcServerImpl.messageResubmissionImpl) == null)
				{
					result = (MessageResubmissionRpcServerImpl.messageResubmissionImpl = Components.MessageResubmissionComponent);
				}
				return result;
			}
			set
			{
				MessageResubmissionRpcServerImpl.messageResubmissionImpl = value;
			}
		}

		public ObjectSecurity LocalSystemSecurityDescriptor
		{
			set
			{
				this.m_sdLocalSystemBinaryForm = value.GetSecurityDescriptorBinaryForm();
			}
		}

		public override byte[] SubmitMessage(byte[] inBlob)
		{
			throw new NotImplementedException();
		}

		public override SubmissionStatus SubmitMessage(string messageClass, string serverDN, string serverSADN, byte[] mailboxGuid, byte[] entryId, byte[] parentEntryId, byte[] mdbGuid, int eventCounter, byte[] ipAddress)
		{
			throw new NotImplementedException();
		}

		public override SubmissionStatus SubmitDumpsterMessages(string storageGroupDN, long startTime, long endTime)
		{
			throw new NotImplementedException();
		}

		public override MailSubmissionResult SubmitMessage2(string serverDN, Guid mailboxGuid, Guid mdbGuid, int eventCounter, byte[] entryId, byte[] parentEntryId, string serverFQDN, byte[] ipAddress)
		{
			throw new NotImplementedException();
		}

		public override bool QueryDumpsterStats(string storageGroupDN, ref long ticksOfOldestDeliveryTime, ref long queueSize, ref int numberOfItems)
		{
			throw new NotImplementedException();
		}

		public override byte[] ShadowHeartBeat(byte[] inBlob)
		{
			throw new NotImplementedException();
		}

		public override AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid mdbGuid, long startTime, long endTime, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			return MessageResubmissionRpcServerImpl.MessageResubmissionImpl.AddMdbResubmitRequest(requestGuid, mdbGuid, startTime, endTime, unresponsivePrimaryServers, reservedBytes);
		}

		public override AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTime, long endTime, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			return MessageResubmissionRpcServerImpl.MessageResubmissionImpl.AddConditionalResubmitRequest(requestGuid, startTime, endTime, conditions, unresponsivePrimaryServers, reservedBytes);
		}

		public override byte[] GetResubmitRequest(byte[] requestRaw)
		{
			return MessageResubmissionRpcServerImpl.MessageResubmissionImpl.GetResubmitRequest(requestRaw);
		}

		public override byte[] SetResubmitRequest(byte[] requestRaw)
		{
			return MessageResubmissionRpcServerImpl.MessageResubmissionImpl.SetResubmitRequest(requestRaw);
		}

		public override byte[] RemoveResubmitRequest(byte[] requestRaw)
		{
			return MessageResubmissionRpcServerImpl.MessageResubmissionImpl.RemoveResubmitRequest(requestRaw);
		}

		private static IMessageResubmissionRpcServer messageResubmissionImpl;
	}
}
