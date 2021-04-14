using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Common.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MessageContextBlobHandler : IMessageHandler
	{
		public MessageContextBlobHandler(SmtpInSessionState sessionStateToUse, IInboundMessageContextBlob messageContextBlobToProcess, long chunkSizeForBdat)
		{
			ArgumentValidator.ThrowIfNull("sessionStateToUse", sessionStateToUse);
			ArgumentValidator.ThrowIfNull("messageContextBlobToProcess", messageContextBlobToProcess);
			this.sessionState = sessionStateToUse;
			this.messageContextBlob = messageContextBlobToProcess;
			this.chunkSize = chunkSizeForBdat;
		}

		public MessageHandlerResult Process(CommandContext commandContext, out SmtpResponse smtpResponse)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			if (!this.ValidateSizeAndPermission(out smtpResponse))
			{
				return MessageHandlerResult.Failure;
			}
			this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Receiving blob {0}", new object[]
			{
				this.messageContextBlob.Name
			});
			int num = (int)Math.Min(this.chunkSize - this.totalBytesRead, (long)commandContext.Length);
			this.totalBytesRead += (long)num;
			this.sessionState.Tracer.TraceDebug<int>((long)this.sessionState.GetHashCode(), "MessageContextBlobHandler.Process consumed {0} bytes", num);
			if (this.sessionState.DiscardingMessage)
			{
				this.sessionState.Tracer.TraceDebug((long)this.sessionState.GetHashCode(), "Discarding message");
			}
			else
			{
				this.blobStream.Write(commandContext.Command, commandContext.Offset, commandContext.Length);
			}
			if (!MessageContextBlobHandler.IsEntireBlobReceived(this.totalBytesRead, this.chunkSize))
			{
				return MessageHandlerResult.MoreDataRequired;
			}
			if (!this.sessionState.DiscardingMessage)
			{
				this.blobStream.Seek(0L, SeekOrigin.Begin);
				smtpResponse = this.DeserializeBlob();
			}
			this.FlushStream();
			MessageContextBlobHandler.PutBackUnreadBytes(commandContext, this.sessionState, num);
			if (smtpResponse.SmtpResponseType != SmtpResponseType.Success && !smtpResponse.IsEmpty)
			{
				return MessageHandlerResult.Failure;
			}
			return MessageHandlerResult.Complete;
		}

		protected virtual void SendWatsonReport(FormatException exception, StringBuilder details)
		{
			ExWatson.SendGenericWatsonReport("E12", ExWatson.ApplicationVersion.ToString(), ExWatson.AppName, "15.00.1497.012", Assembly.GetExecutingAssembly().GetName().Name, exception.GetType().Name, exception.StackTrace, exception.GetHashCode().ToString(CultureInfo.InvariantCulture), exception.TargetSite.Name, details.ToString());
		}

		private void SendWatsonReportWithoutCrashingTheProcess(FormatException exception)
		{
			if (DateTime.UtcNow - MessageContextBlobHandler.WatsonReportSentTimeForMessageContextParseFailure < TimeSpan.FromHours(1.0))
			{
				return;
			}
			if (exception.InnerException is TransientException)
			{
				return;
			}
			MessageContextBlobHandler.WatsonReportSentTimeForMessageContextParseFailure = DateTime.UtcNow;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Exception={0}", exception));
			stringBuilder.AppendLine(string.Format("Time={0}", DateTime.UtcNow));
			stringBuilder.AppendLine(string.Format("ComputerName={0}", Environment.MachineName));
			stringBuilder.AppendLine(string.Format("AdvertisedEhloOptions={0}", this.sessionState.AdvertisedEhloOptions.CreateSmtpResponse(SmtpMessageContextBlob.AdrcSmtpMessageContextBlobInstance, SmtpMessageContextBlob.ExtendedPropertiesSmtpMessageContextBlobInstance, SmtpMessageContextBlob.FastIndexSmtpMessageContextBlobInstance)));
			stringBuilder.AppendLine(string.Format("DiscardingMessage={0}", this.sessionState.DiscardingMessage));
			stringBuilder.AppendLine(string.Format("ChunkSize={0}", this.chunkSize));
			stringBuilder.AppendLine(string.Format("TotalBytesRead={0}", this.totalBytesRead));
			stringBuilder.AppendLine(string.Format("BodyStream Length={0}", (this.blobStream == null) ? "null" : this.blobStream.Length.ToString(CultureInfo.InvariantCulture)));
			stringBuilder.AppendLine(string.Format("BodyStream Position={0}", (this.blobStream == null) ? "null" : this.blobStream.Position.ToString(CultureInfo.InvariantCulture)));
			this.SendWatsonReport(exception, stringBuilder);
		}

		private static bool IsValidChunkSize(long chunkSize, long maxBlobSize)
		{
			return chunkSize <= maxBlobSize;
		}

		private static void PutBackUnreadBytes(CommandContext commandContext, SmtpInSessionState sessionState, int numBytesRead)
		{
			if (commandContext.OriginalLength != numBytesRead)
			{
				sessionState.PutBackReceivedBytes(commandContext.OriginalLength - numBytesRead);
			}
		}

		private static bool IsEntireBlobReceived(long totalBytesRead, long chunkSize)
		{
			return totalBytesRead == chunkSize;
		}

		private bool ValidateSizeAndPermission(out SmtpResponse failureResponse)
		{
			if (this.preProcessChecksComplete)
			{
				failureResponse = SmtpResponse.Empty;
				return true;
			}
			this.preProcessChecksComplete = true;
			if (!MessageContextBlobHandler.IsValidChunkSize(this.chunkSize, (long)this.messageContextBlob.MaxBlobSize.ToBytes()))
			{
				if (this.sessionState.ReceivePerfCounters != null)
				{
					this.sessionState.ReceivePerfCounters.MessagesRefusedForSize.Increment();
				}
				this.sessionState.StartDiscardingMessage();
				failureResponse = SmtpResponse.MessageTooLarge;
				return false;
			}
			if (!this.messageContextBlob.VerifyPermission(this.sessionState.CombinedPermissions))
			{
				this.sessionState.StartDiscardingMessage();
				failureResponse = SmtpResponse.NotAuthorized;
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private void FlushStream()
		{
			if (this.blobStream != null)
			{
				this.blobStream.Close();
				this.blobStream = null;
			}
		}

		private SmtpResponse DeserializeBlob()
		{
			try
			{
				this.messageContextBlob.DeserializeBlob(this.blobStream, this.sessionState, this.chunkSize);
			}
			catch (FormatException ex)
			{
				this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, ex.Message);
				this.sessionState.Tracer.TraceError<string, FormatException>((long)this.GetHashCode(), "Encountered exception while processing the blob {0}. Details: {1}", this.messageContextBlob.Name, ex);
				SystemProbeHelper.SmtpReceiveTracer.TraceFail<string, FormatException>(this.sessionState.TransportMailItem, (long)this.GetHashCode(), "Encountered exception while processing the blob {0}. Details: {1}", this.messageContextBlob.Name, ex);
				this.sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProcessingBlobFailed, this.messageContextBlob.Name, new object[]
				{
					this.messageContextBlob.Name,
					ex
				});
				if (this.sessionState.Configuration.TransportConfiguration.WatsonReportOnFailureEnabled)
				{
					this.SendWatsonReportWithoutCrashingTheProcess(ex);
				}
				if (this.messageContextBlob.IsMandatory)
				{
					return SmtpResponse.XMessageContextError;
				}
			}
			return SmtpResponse.OctetsReceived(this.chunkSize);
		}

		private static DateTime WatsonReportSentTimeForMessageContextParseFailure = DateTime.MinValue;

		private readonly SmtpInSessionState sessionState;

		private readonly long chunkSize;

		private long totalBytesRead;

		private readonly IInboundMessageContextBlob messageContextBlob;

		private bool preProcessChecksComplete;

		private Stream blobStream = new MultiByteArrayMemoryStream();
	}
}
