using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.UM.ExSmtpClient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class SmtpSubmissionHelper
	{
		public static void SubmitMessage(MessageItem message, string senderAddress, Guid senderOrgGuid, string recipientAddress, OutboundConversionOptions submissionConversionOptions, string requestId)
		{
			ValidateArgument.NotNull(message, "message");
			ValidateArgument.NotNull(senderAddress, "sender");
			ValidateArgument.NotNull(recipientAddress, "recipient");
			ValidateArgument.NotNull(submissionConversionOptions, "submissionConversionOptions");
			ValidateArgument.NotNull(requestId, "requestId");
			InternalExchangeServer internalExchangeServer = null;
			try
			{
				lock (SmtpSubmissionHelper.bridgeheadPicker)
				{
					internalExchangeServer = SmtpSubmissionHelper.bridgeheadPicker.PickNextServer(new ADObjectId(Guid.Empty));
				}
				if (internalExchangeServer == null)
				{
					CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "No bridgehead server available to process request '{0}'", new object[]
					{
						requestId
					});
					throw new NoHubTransportServerAvailableException();
				}
				SmtpSubmissionHelper.SubmitMessage(message, senderAddress, senderOrgGuid, recipientAddress, submissionConversionOptions, internalExchangeServer);
				PIIMessage data = PIIMessage.Create(PIIType._User, recipientAddress);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, data, "Successfully submitted voice mail message to '_User', request '{0}'", new object[]
				{
					requestId
				});
			}
			catch (Exception e)
			{
				if (!SmtpSubmissionHelper.CheckTransientSmtpFailure(e))
				{
					throw;
				}
				SmtpSubmissionHelper.HandleTransientSmtpFailure(e, internalExchangeServer, recipientAddress);
			}
		}

		private static void SubmitMessage(MessageItem message, string senderAddress, Guid senderOrgGuid, string recipientAddress, OutboundConversionOptions submissionConversionOptions, InternalExchangeServer smtpServer)
		{
			Util.IncrementCounter(AvailabilityCounters.PercentageHubTransportAccessFailures_Base);
			using (SmtpClient smtpClient = new SmtpClient(smtpServer.MachineName, 25, new SmtpClientDebugOutput()))
			{
				smtpClient.AuthCredentials(CredentialCache.DefaultNetworkCredentials);
				smtpClient.From = senderAddress;
				smtpClient.To = new string[]
				{
					recipientAddress
				};
				if (CommonConstants.UseDataCenterCallRouting && senderOrgGuid != Guid.Empty)
				{
					smtpClient.FromParameters.Add(new KeyValuePair<string, string>("XATTRDIRECT", "Originating"));
					smtpClient.FromParameters.Add(new KeyValuePair<string, string>("XATTRORGID", "xorgid:" + senderOrgGuid));
				}
				PIIMessage[] data = new PIIMessage[]
				{
					PIIMessage.Create(PIIType._User, recipientAddress),
					PIIMessage.Create(PIIType._User, senderAddress)
				};
				CallIdTracer.TracePfd(ExTraceGlobals.PFDUMCallAcceptanceTracer, 0, data, "PFD UMS {0} - Submitting voice mail message to  _User1 Sender : _User2 using {1} server ", new object[]
				{
					11578,
					smtpServer.MachineName
				});
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ItemConversion.ConvertItemToSummaryTnef(message, memoryStream, submissionConversionOptions);
					memoryStream.Position = 0L;
					smtpClient.DataStream = memoryStream;
					smtpClient.Submit();
				}
			}
		}

		private static bool CheckTransientSmtpFailure(Exception e)
		{
			return e is AlreadyConnectedToSMTPServerException || e is FailedToConnectToSMTPServerException || e is MustBeTlsForAuthException || e is AuthFailureException || e is AuthApiFailureException || e is InvalidSmtpServerResponseException || e is UnexpectedSmtpServerResponseException || e is NotConnectedToSMTPServerException || e is SocketException || (e is IOException && e.InnerException != null && e.InnerException is SocketException);
		}

		private static void HandleTransientSmtpFailure(Exception e, InternalExchangeServer smtpServer, string recipientAddress)
		{
			Util.IncrementCounter(AvailabilityCounters.PercentageHubTransportAccessFailures);
			PIIMessage data = PIIMessage.Create(PIIType._User, recipientAddress);
			CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, data, "Failed to submit the message to '_User'. Exception: '{0}'", new object[]
			{
				e
			});
			SmtpSubmissionHelper.bridgeheadPicker.ServerUnavailable(smtpServer);
			throw new SmtpSubmissionException(e);
		}

		private const string DirectionalityParam = "XATTRDIRECT";

		private const string DirectionalityOriginating = "Originating";

		private const string OrganizationIdParam = "XATTRORGID";

		private static BridgeheadPicker bridgeheadPicker = new BridgeheadPicker();
	}
}
