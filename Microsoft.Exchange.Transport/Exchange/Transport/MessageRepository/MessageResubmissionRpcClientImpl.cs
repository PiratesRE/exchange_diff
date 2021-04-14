using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Rpc.MailSubmission;
using Microsoft.Exchange.Transport.MessageResubmission;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Transport.MessageRepository
{
	internal sealed class MessageResubmissionRpcClientImpl
	{
		public MessageResubmissionRpcClientImpl(string serverName)
		{
			this.rpcClient = new MessageResubmissionRpcClientImpl.RpcClientFacade(serverName);
		}

		public MessageResubmissionRpcClientImpl(IMessageResubmissionRpcServer rpcClient = null)
		{
			this.rpcClient = rpcClient;
		}

		public AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid mdbGuid, long startTimeInTicks, long endTimeInTicks, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			return this.rpcClient.AddMdbResubmitRequest(requestGuid, mdbGuid, startTimeInTicks, endTimeInTicks, unresponsivePrimaryServers, reservedBytes);
		}

		public AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTimeInTicks, long endTimeInTicks, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			return this.rpcClient.AddConditionalResubmitRequest(requestGuid, startTimeInTicks, endTimeInTicks, conditions, unresponsivePrimaryServers, reservedBytes);
		}

		public ResubmitRequest[] GetResubmitRequest(ResubmitRequestId identity)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResubmitRequestIdentity"] = ((identity == null) ? null : identity.ToString());
			return (ResubmitRequest[])Serialization.BytesToObject(this.rpcClient.GetResubmitRequest(Serialization.ObjectToBytes(dictionary)));
		}

		public void SetResubmitRequest(ResubmitRequestId identity, bool enabled)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResubmitRequestIdentity"] = identity.ToString();
			dictionary["DumpsterRequestEnabled"] = enabled;
			byte[] mBinaryData = this.rpcClient.SetResubmitRequest(Serialization.ObjectToBytes(dictionary));
			ResubmitRequestResponse output = (ResubmitRequestResponse)Serialization.BytesToObject(mBinaryData);
			MessageResubmissionRpcClientImpl.ThrowExcpetionIfFailure(output, identity.ResubmitRequestRowId);
		}

		public void RemoveResubmitRequest(ResubmitRequestId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResubmitRequestIdentity"] = identity.ToString();
			byte[] mBinaryData = this.rpcClient.RemoveResubmitRequest(Serialization.ObjectToBytes(dictionary));
			ResubmitRequestResponse output = (ResubmitRequestResponse)Serialization.BytesToObject(mBinaryData);
			MessageResubmissionRpcClientImpl.ThrowExcpetionIfFailure(output, identity.ResubmitRequestRowId);
		}

		private static void ThrowExcpetionIfFailure(ResubmitRequestResponse output, long requestId)
		{
			if (output.ResponseCode == ResubmitRequestResponseCode.Success)
			{
				return;
			}
			string message;
			switch (output.ResponseCode)
			{
			case ResubmitRequestResponseCode.CannotModifyCompletedRequest:
				message = Strings.CannotModifyCompletedRequest(requestId).ToString();
				break;
			case ResubmitRequestResponseCode.ResubmitRequestDoesNotExist:
				message = Strings.ResubmitRequestDoesNotExist(requestId).ToString();
				break;
			case ResubmitRequestResponseCode.CannotRemoveRequestInRunningState:
				message = Strings.CannotRemoveRequestInRunningState(requestId).ToString();
				break;
			default:
				message = output.ErrorMessage;
				break;
			}
			throw new ResubmitRequestException(output.ResponseCode, message);
		}

		private readonly IMessageResubmissionRpcServer rpcClient;

		private class RpcClientFacade : IMessageResubmissionRpcServer
		{
			internal RpcClientFacade(string serverName)
			{
				MailSubmissionServiceRpcClient mailSubmissionServiceRpcClient = new MailSubmissionServiceRpcClient(serverName);
				mailSubmissionServiceRpcClient.SetTimeOut(5000);
				this.rpcClient = mailSubmissionServiceRpcClient;
			}

			public byte[] GetResubmitRequest(byte[] rawRequest)
			{
				return this.rpcClient.GetResubmitRequest(rawRequest);
			}

			public byte[] SetResubmitRequest(byte[] requestRaw)
			{
				return this.rpcClient.SetResubmitRequest(requestRaw);
			}

			public byte[] RemoveResubmitRequest(byte[] requestRaw)
			{
				return this.rpcClient.RemoveResubmitRequest(requestRaw);
			}

			public AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid mdbGuid, long startTimeInTicks, long endTimeInTicks, string[] unresponsivePrimaryServers, byte[] reservedBytes)
			{
				return this.rpcClient.AddMdbResubmitRequest(requestGuid, mdbGuid, startTimeInTicks, endTimeInTicks, unresponsivePrimaryServers, reservedBytes);
			}

			public AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTimeInTicks, long endTimeInTicks, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes)
			{
				return this.rpcClient.AddConditionalResubmitRequest(requestGuid, startTimeInTicks, endTimeInTicks, conditions, unresponsivePrimaryServers, reservedBytes);
			}

			private const int ConnectionTimeoutMsec = 5000;

			private MailSubmissionServiceRpcClient rpcClient;
		}
	}
}
