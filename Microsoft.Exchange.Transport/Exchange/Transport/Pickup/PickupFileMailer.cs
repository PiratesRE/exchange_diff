using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal class PickupFileMailer
	{
		public PickupFileMailer(PickupDirectory parent, string pathName, ExEventLog eventLogger)
		{
			this.parent = parent;
			this.pathName = pathName;
			this.eventLogger = eventLogger;
			this.latencyComponent = LatencyComponent.Pickup;
			this.version = Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion;
		}

		internal bool ProcessFile(string fileName)
		{
			FileStream fileStream = null;
			this.originalFileName = fileName;
			if (this.fileInfo != null || this.transportMailItem != null)
			{
				throw new InvalidOperationException("Pickup ProcessFile does not support multithread");
			}
			try
			{
				try
				{
					this.fileInfo = new FileInfo(fileName);
					if (PoisonMessage.Verify(this.fileInfo.Name))
					{
						string name = this.fileInfo.Name;
						bool flag = this.TryRenameFileWithNewExtension(this.fileInfo, "psn");
						if (flag)
						{
							PoisonMessage.MarkFileHandled(name);
							this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DeletedPoisonPickupFile, null, new object[]
							{
								name
							});
						}
						return flag;
					}
					PoisonMessage.Context = new PickupContext(this.fileInfo.Name);
					if (this.fileInfo.IsReadOnly)
					{
						this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ReadOnlyFileFound, this.pathName, new object[]
						{
							this.pathName
						});
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "ReadOnlyFileFound", null, "Read-only files have been found in the Pickup directory.", ResultSeverityLevel.Warning, false);
						return true;
					}
					ExTraceGlobals.PickupTracer.TraceDebug<FileInfo>((long)this.GetHashCode(), "Opening file {0}", this.fileInfo);
					fileStream = this.fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Delete);
					this.fileCreationTime = this.fileInfo.CreationTimeUtc;
					this.TryRenameFileWithNewExtension(this.fileInfo, "tmp");
				}
				catch (FileNotFoundException)
				{
					return true;
				}
				catch (IOException arg)
				{
					ExTraceGlobals.PickupTracer.TraceDebug<string, IOException>((long)this.GetHashCode(), "Cannot open {0}, exception {1}, leaving file.", fileName, arg);
					return true;
				}
				catch (UnauthorizedAccessException arg2)
				{
					ExTraceGlobals.PickupTracer.TraceDebug<string, UnauthorizedAccessException>((long)this.GetHashCode(), "Unauthorized cannot open {0}, exception {1}, leaving file.", fileName, arg2);
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NoPermissionToRenamePickupFile, this.pathName, new object[]
					{
						this.pathName
					});
					return true;
				}
				catch (SecurityException arg3)
				{
					ExTraceGlobals.PickupTracer.TraceDebug<string, SecurityException>((long)this.GetHashCode(), "Unauthorized cannot open {0}, exception {1}, leaving file.", fileName, arg3);
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NoPermissionToRenamePickupFile, this.pathName, new object[]
					{
						this.pathName
					});
					return true;
				}
				ExTraceGlobals.PickupTracer.TraceDebug<string>((long)this.GetHashCode(), "Begin to process {0}", fileName);
				bool flag2 = this.ProcessFileStream(fileStream);
				if (flag2)
				{
					PoisonMessage.MarkFileHandled(((PickupContext)PoisonMessage.Context).FileName);
					return this.DeleteFile();
				}
			}
			finally
			{
				if (this.transportMailItem != null)
				{
					this.transportMailItem.MimeDocument.Dispose();
					this.transportMailItem = null;
				}
				this.fileInfo = null;
				this.invalidAddresses = null;
				this.invalidAddressesToTrack = null;
			}
			return true;
		}

		protected virtual bool ProcessFileStream(FileStream fileStream)
		{
			try
			{
				bool flag = false;
				this.CreateMailItem();
				this.SetEnvelopeProperties();
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3957730621U);
				if (!Components.IsBridgehead)
				{
					PickupFileMailer.MessageSizeCheckResults messageSizeCheckResults = this.CheckMessageSize(RoutingAddress.Empty, this.fileInfo.Length);
					if (messageSizeCheckResults == PickupFileMailer.MessageSizeCheckResults.Retry)
					{
						if (fileStream != null)
						{
							fileStream.Close();
							fileStream = null;
						}
						return false;
					}
					flag = (messageSizeCheckResults == PickupFileMailer.MessageSizeCheckResults.Exceeded);
				}
				this.LoadStreamToMimeDom(ref fileStream, flag);
				this.transportMailItem.MimeSize = this.fileInfo.Length;
				if (!this.transportMailItem.ExposeMessageHeaders)
				{
					throw new InvalidOperationException("transportMailItem.ExposeMessageHeaders is false in pickup");
				}
				bool flag2 = !this.EnforceMaxHeaders();
				HeaderFirewall.Filter(this.transportMailItem.RootPart.Headers, RestrictedHeaderSet.All);
				this.transportMailItem.UpdateCachedHeaders();
				bool flag3 = false;
				RoutingAddress senderAddress;
				if (!this.PromoteSender(out senderAddress))
				{
					this.HandleBadMail("Sender Promotion failed");
					return false;
				}
				if (!this.PromoteAndPatchP2Recipients())
				{
					if (this.invalidAddresses == null)
					{
						this.HandleBadMail("No recipients in header");
						return false;
					}
					flag3 = true;
				}
				this.PatchHeaders();
				ADOperationResult adoperationResult = MultiTenantTransport.TryAttributePickupMessage(this.transportMailItem);
				if (!adoperationResult.Succeeded)
				{
					return false;
				}
				adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(this.transportMailItem);
				if (!adoperationResult.Succeeded)
				{
					return false;
				}
				this.transportMailItem.Oorg = null;
				if (flag3)
				{
					this.TrackMessageReceived();
					this.GenerateNDRForInvalidAddresses();
					return true;
				}
				if (!flag2 && Components.IsBridgehead)
				{
					PickupFileMailer.MessageSizeCheckResults messageSizeCheckResults2 = this.CheckMessageSize(senderAddress, this.fileInfo.Length);
					if (messageSizeCheckResults2 == PickupFileMailer.MessageSizeCheckResults.Retry)
					{
						return false;
					}
					flag = (messageSizeCheckResults2 == PickupFileMailer.MessageSizeCheckResults.Exceeded);
				}
				if (flag2 || flag)
				{
					if (flag)
					{
						this.transportMailItem.Ack(AckStatus.Fail, AckReason.PickupMessageTooLarge, this.transportMailItem.Recipients, null);
					}
					else
					{
						this.transportMailItem.Ack(AckStatus.Fail, AckReason.PickupHeaderTooLarge, this.transportMailItem.Recipients, null);
					}
					this.GenerateNdr();
					return true;
				}
				SmtpResponse smtpResponse;
				if (!this.transportMailItem.ValidateDeliveryPriority(out smtpResponse))
				{
					this.transportMailItem.Ack(AckStatus.Fail, smtpResponse, this.transportMailItem.Recipients, null);
					this.GenerateNdr();
					return true;
				}
				if (!this.EnforceMaxRecipients())
				{
					this.GenerateNdr();
					return true;
				}
				this.Submit(MessageTrackingSource.PICKUP, this.originalFileName);
			}
			catch (ExchangeDataException ex)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, ExchangeDataException>((long)this.GetHashCode(), "ExchangeDataException processing {0} in Pickup, badmail. Exception {1}", this.fileInfo.FullName, ex);
				this.HandleBadMail(string.Format("Threw Exchange Data Exception: {0}", ex.Message));
				return false;
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, IOException>((long)this.GetHashCode(), "ExchangeDataException processing {0}, badmail. Exception {1}", this.fileInfo.FullName, ex2);
				if (ExceptionHelper.IsHandleableTransientCtsException(ex2))
				{
					return false;
				}
				throw;
			}
			catch (EsentErrorException)
			{
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PickupFailedDueToStorageErrors, null, null);
				return false;
			}
			return true;
		}

		protected void LoadStreamToMimeDom(ref FileStream sourceStream, bool headersOnly)
		{
			FileStream fileStream = sourceStream;
			sourceStream = null;
			fileStream.Position = 0L;
			if (headersOnly)
			{
				this.endOfHeaders = false;
				this.transportMailItem.MimeDocument.EndOfHeaders = new MimeDocument.EndOfHeadersCallback(this.EndOfHeadersCallback);
			}
			this.transportMailItem.MimeDocument.Load(fileStream, CachingMode.SourceTakeOwnership);
		}

		protected void StripHeaders(HeaderId[] headerIds)
		{
			SubmitHelper.StripHeaders(this.transportMailItem, headerIds);
		}

		protected void Submit(MessageTrackingSource source, string sourceContext)
		{
			this.transportMailItem.Message.Normalize(NormalizeOptions.NormalizeMessageId | NormalizeOptions.MergeAddressHeaders | NormalizeOptions.RemoveDuplicateHeaders, false);
			this.TrackMessageReceived(source, sourceContext);
			this.GenerateNDRForInvalidAddresses();
			LatencyTracker.TrackPreProcessLatency(this.latencyComponent, this.transportMailItem.LatencyTracker, this.transportMailItem.DateReceived);
			this.parent.SubmitHandler.OnSubmit(this.transportMailItem, (this.transportMailItem.Directionality == MailDirectionality.Undefined) ? MailDirectionality.Incoming : this.transportMailItem.Directionality, PickupType.Pickup);
			this.transportMailItem = null;
			this.parent.PickupPerformanceCounter.Submitted.Increment();
		}

		protected void CreateMailItem()
		{
			this.transportMailItem = TransportMailItem.NewMailItem(this.latencyComponent);
			this.transportMailItem.PerfCounterAttribution = "pickup";
			this.transportMailItem.DateReceived = this.fileCreationTime;
		}

		protected PickupFileMailer.MessageSizeCheckResults CheckMessageSize(RoutingAddress senderAddress, long messageSize)
		{
			Unlimited<ByteQuantifiedSize> arg = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			if (Util.IsValidAddress(senderAddress) && senderAddress != RoutingAddress.NullReversePath)
			{
				ProxyAddress senderProxyAddress;
				if (Components.IsBridgehead)
				{
					senderProxyAddress = Sender.GetInnermostAddress(senderAddress);
				}
				else
				{
					senderProxyAddress = new SmtpProxyAddress(senderAddress.ToString(), false);
				}
				if (senderProxyAddress != null)
				{
					Result<TransportMiniRecipient> entry = new Result<TransportMiniRecipient>(null, null);
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						entry = this.transportMailItem.ADRecipientCache.FindAndCacheRecipient(senderProxyAddress);
					}, 1);
					if (!adoperationResult.Succeeded)
					{
						ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "FindAndCacheRecipient failed with .");
						return PickupFileMailer.MessageSizeCheckResults.Retry;
					}
					if (entry.Data != null)
					{
						arg = entry.Data.MaxSendSize;
						ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Sender not found in AD for Max message check.");
					}
				}
			}
			if (arg.IsUnlimited)
			{
				arg = Components.Configuration.TransportSettings.TransportSettings.MaxSendSize;
			}
			ExTraceGlobals.PickupTracer.TraceDebug<long, RoutingAddress, Unlimited<ByteQuantifiedSize>>((long)this.GetHashCode(), "Message size is '{0}', effective limit for <{1}> is '{2}'.", messageSize, senderAddress, arg);
			if (arg.IsUnlimited || messageSize < (long)arg.Value.ToBytes())
			{
				return PickupFileMailer.MessageSizeCheckResults.WithinLimit;
			}
			return PickupFileMailer.MessageSizeCheckResults.Exceeded;
		}

		protected bool EnforceMaxHeaders()
		{
			bool flag = true;
			ulong num = Components.Configuration.LocalServer.TransportServer.PickupDirectoryMaxHeaderSize.ToBytes();
			ulong num2 = 0UL;
			HeaderList headers = this.transportMailItem.RootPart.Headers;
			Header header2;
			for (Header header = headers.FirstChild as Header; header != null; header = header2)
			{
				header2 = (header.NextSibling as Header);
				if (flag)
				{
					num2 += (ulong)header.WriteTo(Stream.Null);
				}
				if (num2 > num)
				{
					headers.RemoveChild(header);
					flag = false;
				}
			}
			return flag;
		}

		protected bool EnforceMaxRecipients()
		{
			int pickupDirectoryMaxRecipientsPerMessage = Components.Configuration.LocalServer.TransportServer.PickupDirectoryMaxRecipientsPerMessage;
			if (this.transportMailItem.Recipients.Count > pickupDirectoryMaxRecipientsPerMessage)
			{
				this.transportMailItem.Ack(AckStatus.Fail, AckReason.PickupTooManyRecipients, this.transportMailItem.Recipients, null);
				return false;
			}
			return true;
		}

		protected void HandleBadMail(MessageTrackingSource source, string sourceContext, string badmailReason)
		{
			try
			{
				this.TryRenameFileWithNewExtension(this.fileInfo, "bad");
			}
			finally
			{
				if (this.transportMailItem != null)
				{
					this.transportMailItem.MimeDocument.Dispose();
					this.transportMailItem = null;
				}
			}
			PickupFileMailer.TrackMessageBadmailed(source, sourceContext, badmailReason);
			this.parent.PickupPerformanceCounter.Badmailed.Increment();
			this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PickupIsBadmailingFile, this.pathName, new object[]
			{
				this.pathName
			});
			string notificationReason = string.Format("At least one file in {0} can't be processed.", this.pathName);
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "PickupIsBadmailingFile", null, notificationReason, ResultSeverityLevel.Warning, false);
		}

		protected bool TryRenameFileWithNewExtension(FileInfo fileInfo, string extension)
		{
			bool result = false;
			string renamedFileName = PickupDirectory.GetRenamedFileName(fileInfo.FullName, extension);
			try
			{
				this.fileInfo.MoveTo(renamedFileName);
				result = true;
			}
			catch (IOException arg)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, string, IOException>((long)this.GetHashCode(), "Cannot rename {0} to {1}, exception {2}, leaving file.", fileInfo.FullName, renamedFileName, arg);
			}
			catch (UnauthorizedAccessException arg2)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, string, UnauthorizedAccessException>((long)this.GetHashCode(), "Cannot rename {0} to {1}, exception {2}, leaving file.", fileInfo.FullName, renamedFileName, arg2);
			}
			catch (SecurityException arg3)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, string, SecurityException>((long)this.GetHashCode(), "No permission to rename {0} to {1}, exception {2}, leaving file.", fileInfo.FullName, renamedFileName, arg3);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NoPermissionToRenamePickupFile, this.pathName, new object[]
				{
					this.pathName
				});
			}
			return result;
		}

		protected void GenerateNdr()
		{
			this.TrackMessageReceived();
			this.transportMailItem.DsnFormat = DsnFormat.Headers;
			Components.DsnGenerator.GenerateDSNs(this.transportMailItem);
			MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.PICKUP, this.transportMailItem, this.transportMailItem.Recipients, null);
			this.parent.PickupPerformanceCounter.NDRed.Increment();
			this.TrackInvalidAddressesFailed();
		}

		private static void TrackMessageBadmailed(MessageTrackingSource source, string sourceContext, string badmailReason)
		{
			MsgTrackReceiveInfo msgTrackInfo = new MsgTrackReceiveInfo(sourceContext);
			MessageTrackingLog.TrackBadmail(source, msgTrackInfo, null, badmailReason);
		}

		private void PatchHeaders()
		{
			this.StripHeaders(PickupFileMailer.HeaderTypesToStrip);
			DateHeader dateHeader = new DateHeader("Date", this.transportMailItem.DateReceived);
			string value = dateHeader.Value;
			string text = PickupFileMailer.ServerName;
			if (!AsciiString.IsStringArgumentAscii(text))
			{
				text = Components.Configuration.LocalServer.TransportServer.Fqdn;
			}
			ReceivedHeader receivedHeader = new ReceivedHeader("Pickup", null, text, null, null, "Microsoft SMTP Server", this.version.ToString(), null, value);
			string text2;
			Util.PatchHeaders(this.transportMailItem.RootPart.Headers, receivedHeader, this.transportMailItem.From, this.transportMailItem.DateReceived, Components.Configuration.LocalServer.TransportServer.Fqdn, Components.Configuration.LocalServer.TransportServer.IsHubTransportServer, out text2);
			MultilevelAuth.EnsureSecurityAttributes(this.transportMailItem, SubmitAuthCategory.Anonymous, MultilevelAuthMechanism.Pickup, null);
			SubmitHelper.StampOriginalMessageSize(this.transportMailItem);
		}

		private void HandleBadMail(string badmailReason)
		{
			this.HandleBadMail(MessageTrackingSource.PICKUP, this.originalFileName, badmailReason);
		}

		private bool DeleteFile()
		{
			if (this.fileInfo == null)
			{
				throw new InvalidOperationException("fileInfo is null");
			}
			try
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string>((long)this.GetHashCode(), "deleting file {0}", this.fileInfo.Name);
				this.fileInfo.Delete();
			}
			catch (IOException arg)
			{
				ExTraceGlobals.PickupTracer.TraceError<string, IOException>((long)this.GetHashCode(), "Cannot Delete {0} due to IOException: {1}", this.fileInfo.Name, arg);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotDeleteFile, this.pathName, new object[]
				{
					this.pathName
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "CannotDeleteFile", null, "Can't delete files from Pickup directory.", ResultSeverityLevel.Warning, false);
				return false;
			}
			catch (UnauthorizedAccessException arg2)
			{
				ExTraceGlobals.PickupTracer.TraceError<string, UnauthorizedAccessException>((long)this.GetHashCode(), "Cannot Delete {0} due to UnauthorizedAccessException: {1}", this.fileInfo.Name, arg2);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotDeleteFile, this.pathName, new object[]
				{
					this.pathName
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "CannotDeleteFile", null, "Can't delete files from Pickup directory.", ResultSeverityLevel.Warning, false);
				return false;
			}
			catch (SecurityException arg3)
			{
				ExTraceGlobals.PickupTracer.TraceError<string, SecurityException>((long)this.GetHashCode(), "Cannot Delete {0} due to SecurityException: {1}", this.fileInfo.Name, arg3);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotDeleteFile, this.pathName, new object[]
				{
					this.pathName
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "CannotDeleteFile", null, "Can't delete files from Pickup directory.", ResultSeverityLevel.Warning, false);
				return false;
			}
			return true;
		}

		private void GenerateNDRForInvalidAddresses()
		{
			if (this.invalidAddresses == null)
			{
				return;
			}
			Components.DsnGenerator.GenerateNDRForInvalidAddresses(true, this.transportMailItem, this.invalidAddresses);
			this.TrackInvalidAddressesFailed();
			this.parent.PickupPerformanceCounter.NDRed.Increment();
		}

		private void EndOfHeadersCallback(MimePart part, out bool stopLoading)
		{
			stopLoading = true;
			this.endOfHeaders = true;
		}

		private void SetEnvelopeProperties()
		{
			this.transportMailItem.ReceiveConnectorName = "Pickup";
			this.transportMailItem.Auth = "<>";
		}

		private bool PromoteSender(out RoutingAddress senderAddress)
		{
			SubmitHelper.SenderPromotionError sender = SubmitHelper.GetSender(this.transportMailItem.Message, out senderAddress);
			ExTraceGlobals.PickupTracer.TraceDebug<SubmitHelper.SenderPromotionError>((long)this.GetHashCode(), "Sender promote result: {0}", sender);
			if (sender == SubmitHelper.SenderPromotionError.None)
			{
				this.transportMailItem.From = senderAddress;
				return true;
			}
			senderAddress = RoutingAddress.Empty;
			return false;
		}

		private bool PromoteAndPatchP2Recipients()
		{
			EmailMessage message = this.transportMailItem.Message;
			int num = 0;
			num += this.AddRecipients(message.To);
			num += this.AddRecipients(message.Cc);
			num += this.AddRecipients(message.Bcc);
			ExTraceGlobals.PickupTracer.TraceDebug<int>((long)this.GetHashCode(), "Number of recipieints promoted: {0}", num);
			return num != 0;
		}

		private int AddRecipients(EmailRecipientCollection recipients)
		{
			int num = 0;
			for (int i = 0; i < recipients.Count; i++)
			{
				if (Util.IsValidAddress((RoutingAddress)recipients[i].SmtpAddress))
				{
					this.transportMailItem.Recipients.Add(recipients[i].SmtpAddress);
					num++;
				}
				else
				{
					if (this.invalidAddresses == null)
					{
						this.invalidAddresses = new List<DsnRecipientInfo>();
						this.invalidAddressesToTrack = new List<string>();
					}
					this.invalidAddressesToTrack.Add(recipients[i].SmtpAddress);
					this.invalidAddresses.Add(DsnGenerator.CreateDsnRecipientInfo(recipients[i].DisplayName, recipients[i].SmtpAddress, null, AckReason.PickupInvalidAddress));
				}
			}
			return num;
		}

		private void TrackMessageReceived(MessageTrackingSource source, string sourceContext)
		{
			MsgTrackReceiveInfo msgTrackInfo = new MsgTrackReceiveInfo(sourceContext, this.transportMailItem.MessageTrackingSecurityInfo, this.invalidAddressesToTrack);
			MessageTrackingLog.TrackReceive(source, this.transportMailItem, msgTrackInfo);
		}

		private void TrackMessageReceived()
		{
			this.TrackMessageReceived(MessageTrackingSource.PICKUP, this.originalFileName);
		}

		private void TrackInvalidAddressesFailed()
		{
			if (this.invalidAddresses == null)
			{
				return;
			}
			for (int i = 0; i < this.invalidAddresses.Count; i++)
			{
				SmtpResponse empty;
				if (SmtpResponse.TryParse(this.invalidAddresses[i].StatusText, out empty))
				{
					empty = SmtpResponse.Empty;
				}
				LatencyFormatter latencyFormatter = new LatencyFormatter(this.transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
				MessageTrackingLog.TrackRecipientFail(MessageTrackingSource.PICKUP, this.transportMailItem, (RoutingAddress)this.invalidAddresses[i].Address, empty, this.originalFileName, latencyFormatter);
			}
		}

		protected static readonly string ServerName = SmtpReceiveServer.ServerName;

		protected FileInfo fileInfo;

		protected ExEventLog eventLogger;

		protected PickupDirectory parent;

		protected string pathName;

		protected Version version;

		protected TransportMailItem transportMailItem;

		protected bool endOfHeaders;

		protected string originalFileName;

		protected DateTime fileCreationTime;

		protected LatencyComponent latencyComponent;

		private static readonly HeaderId[] HeaderTypesToStrip = new HeaderId[]
		{
			HeaderId.Bcc
		};

		private List<DsnRecipientInfo> invalidAddresses;

		private List<string> invalidAddressesToTrack;

		protected enum MessageSizeCheckResults
		{
			WithinLimit,
			Exceeded,
			Retry
		}
	}
}
