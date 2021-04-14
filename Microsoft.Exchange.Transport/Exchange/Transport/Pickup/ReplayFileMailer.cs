using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal class ReplayFileMailer : PickupFileMailer
	{
		public ReplayFileMailer(PickupDirectory parent, string pathName, ExEventLog eventLogger) : base(parent, pathName, eventLogger)
		{
			this.latencyComponent = LatencyComponent.Replay;
		}

		protected override bool ProcessFileStream(FileStream fileStream)
		{
			this.xheadersProcessed = ReplayFileMailer.XHeaderType.None;
			this.messageCreator = null;
			this.messageSource = MessageTrackingSource.GATEWAY;
			this.sourceContext = null;
			try
			{
				base.CreateMailItem();
				this.SetEnvelopeProperties();
				base.LoadStreamToMimeDom(ref fileStream, false);
				if (!this.transportMailItem.ExposeMessageHeaders)
				{
					throw new InvalidOperationException("transportMailItem.ExposeMessageHeaders is false in replay");
				}
				ReplayXHeaderProcessor replayXHeaderProcessor = new ReplayXHeaderProcessor(this.transportMailItem);
				if (!replayXHeaderProcessor.ProcessXHeaders(this.transportMailItem.RootPart.Headers, out this.xheadersProcessed, out this.messageCreator, false))
				{
					this.HandleBadMail("Failed processing X-Headers");
					return false;
				}
				this.transportMailItem.RefreshMimeSize();
				if (!this.GetMessageTrackingSourceAndContext())
				{
					this.HandleBadMail("Invalid X-CreatedBy header");
					return false;
				}
				long messageSize;
				long val;
				if (this.messageSource == MessageTrackingSource.GATEWAY)
				{
					if ((this.xheadersProcessed & ReplayFileMailer.ProhibitedGatewayHeaderType) != ReplayFileMailer.XHeaderType.None)
					{
						this.HandleBadMail("Using a prohibited gateway header");
						return false;
					}
					messageSize = this.transportMailItem.MimeSize;
				}
				else if (this.TryGetOringinalSize(out val))
				{
					messageSize = Math.Min(this.transportMailItem.MimeSize, val);
				}
				else
				{
					messageSize = this.transportMailItem.MimeSize;
				}
				this.PatchHeaders();
				if ((this.transportMailItem.Directionality == MailDirectionality.Undefined || this.transportMailItem.ExternalOrganizationId == Guid.Empty) && !MultiTenantTransport.TryAttributeReplayMessage(this.transportMailItem).Succeeded)
				{
					return false;
				}
				this.transportMailItem.SetMimeDefaultEncoding();
				if (!MultiTenantTransport.TryCreateADRecipientCache(this.transportMailItem).Succeeded)
				{
					return false;
				}
				this.transportMailItem.UpdateDirectionalityAndScopeHeaders();
				this.SetMailPriority();
				this.transportMailItem.UpdateCachedHeaders();
				this.SetOriginatorOrganization();
				RoutingAddress senderAddress = RoutingAddress.Empty;
				if (Components.IsBridgehead)
				{
					EmailRecipient sender = this.transportMailItem.Message.Sender;
					if (sender != null)
					{
						senderAddress = (RoutingAddress)sender.SmtpAddress;
					}
				}
				PickupFileMailer.MessageSizeCheckResults messageSizeCheckResults = base.CheckMessageSize(senderAddress, messageSize);
				if (messageSizeCheckResults == PickupFileMailer.MessageSizeCheckResults.Exceeded)
				{
					this.transportMailItem.Ack(AckStatus.Fail, AckReason.ReplayMessageTooLarge, this.transportMailItem.Recipients, null);
					base.GenerateNdr();
					return true;
				}
				if (messageSizeCheckResults == PickupFileMailer.MessageSizeCheckResults.Retry)
				{
					return false;
				}
				SmtpResponse smtpResponse;
				if (!this.transportMailItem.ValidateDeliveryPriority(out smtpResponse))
				{
					this.transportMailItem.Ack(AckStatus.Fail, smtpResponse, this.transportMailItem.Recipients, null);
					base.GenerateNdr();
					return true;
				}
				base.Submit(this.messageSource, this.sourceContext);
			}
			catch (EsentErrorException)
			{
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_PickupFailedDueToStorageErrors, null, null);
				return false;
			}
			catch (ExchangeDataException ex)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, ExchangeDataException>((long)this.GetHashCode(), "ExchangeDataException processing {0} in Replay, badmail. Exception {1}", this.fileInfo.FullName, ex);
				this.HandleBadMail(string.Format("Threw Exchange Data Exception: {0}", ex.Message));
				return false;
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<string, IOException>((long)this.GetHashCode(), "IOException processing {0}, badmail. Exception {1}", this.fileInfo.FullName, ex2);
				if (ExceptionHelper.IsHandleableTransientCtsException(ex2))
				{
					return false;
				}
				throw;
			}
			finally
			{
				this.transportMailItem = null;
			}
			return true;
		}

		private void SetEnvelopeProperties()
		{
			this.transportMailItem.ReceiveConnectorName = "Pickup";
			this.transportMailItem.SourceIPAddress = IPAddress.Any;
		}

		private void SetMailPriority()
		{
			string name = this.fileInfo.Name;
			string[] array = name.Split(new char[]
			{
				'.'
			});
			Enum.GetNames(typeof(DeliveryPriority));
			if (array.Length > 2)
			{
				string value = array[array.Length - 1 - 1];
				DeliveryPriority priority;
				if (EnumValidator<DeliveryPriority>.TryParse(value, EnumParseOptions.IgnoreCase, out priority))
				{
					((IQueueItem)this.transportMailItem).Priority = priority;
				}
			}
		}

		private bool TryGetOringinalSize(out long size)
		{
			size = 0L;
			HeaderList headers = this.transportMailItem.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-OriginalSize");
			if (header != null)
			{
				try
				{
					return long.TryParse(header.Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out size);
				}
				catch (ExchangeDataException)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		private void PatchHeaders()
		{
			SubmitAuthCategory authCategory;
			if (this.messageSource == MessageTrackingSource.GATEWAY)
			{
				HeaderFirewall.Filter(this.transportMailItem.RootPart.Headers, ~RestrictedHeaderSet.MTA);
				authCategory = SubmitAuthCategory.Anonymous;
			}
			else
			{
				authCategory = SubmitAuthCategory.Internal;
			}
			if (string.Equals(this.transportMailItem.Auth, (string)RoutingAddress.NullReversePath, StringComparison.OrdinalIgnoreCase))
			{
				authCategory = SubmitAuthCategory.Anonymous;
			}
			MultilevelAuth.EnsureSecurityAttributes(this.transportMailItem, authCategory, MultilevelAuthMechanism.Replay, null);
			SubmitHelper.StampOriginalMessageSize(this.transportMailItem);
			base.StripHeaders(ReplayFileMailer.HeaderTypesToStrip);
			DateHeader dateHeader = new DateHeader("Date", this.transportMailItem.DateReceived);
			string value = dateHeader.Value;
			ReceivedHeader receivedHeader = new ReceivedHeader("Pickup", null, PickupFileMailer.ServerName, null, null, "Microsoft SMTP Server", this.version.ToString(), null, value);
			string text;
			Util.PatchHeaders(this.transportMailItem.RootPart.Headers, receivedHeader, this.transportMailItem.From, this.transportMailItem.DateReceived, Components.Configuration.LocalServer.TransportServer.Fqdn, Components.Configuration.LocalServer.TransportServer.IsHubTransportServer, out text);
		}

		private void HandleBadMail(string badmailReason)
		{
			if (string.IsNullOrEmpty(this.sourceContext))
			{
				this.GetMessageTrackingSourceAndContext();
			}
			base.HandleBadMail(this.messageSource, this.sourceContext, badmailReason);
		}

		private bool GetMessageTrackingSourceAndContext()
		{
			if ((this.xheadersProcessed & ReplayFileMailer.XHeaderType.CreatedBy) == ReplayFileMailer.XHeaderType.None)
			{
				this.messageSource = MessageTrackingSource.GATEWAY;
				this.sourceContext = "UnspecifiedSource: " + this.originalFileName;
				return true;
			}
			if (!this.messageCreator.StartsWith("MSExchange", StringComparison.OrdinalIgnoreCase))
			{
				this.messageSource = MessageTrackingSource.GATEWAY;
				this.sourceContext = "NonExchangeSource: " + this.messageCreator + ": " + this.originalFileName;
				return true;
			}
			string s = this.messageCreator.Substring("MSExchange".Length);
			int num;
			if (!int.TryParse(s, out num) || num < 0 || num > ExportStream.ProductVersion)
			{
				this.messageSource = MessageTrackingSource.ADMIN;
				this.sourceContext = "FutureExchangeSource: " + this.originalFileName;
				return false;
			}
			this.messageSource = MessageTrackingSource.ADMIN;
			this.sourceContext = this.originalFileName;
			return true;
		}

		private void SetOriginatorOrganization()
		{
			string text = null;
			Header header = this.transportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-OriginatorOrganization");
			if (header != null)
			{
				text = Util.GetHeaderValue(header);
			}
			if (!string.IsNullOrEmpty(text) && !RoutingAddress.IsValidDomain(text))
			{
				ExTraceGlobals.PickupTracer.TraceError<string, string>((long)this.GetHashCode(), "'{0}' header contains invalid value '{1}'", "X-MS-Exchange-Organization-OriginatorOrganization", text);
				text = null;
			}
			ExTraceGlobals.PickupTracer.TraceError<string>((long)this.GetHashCode(), "Setting Originator Organization to '{0}'", text ?? "none");
			this.transportMailItem.Oorg = text;
		}

		private const string UnspecifiedSource = "UnspecifiedSource: ";

		private const string NonExchangeSource = "NonExchangeSource: ";

		private const string FutureExchangeSource = "FutureExchangeSource: ";

		private static readonly HeaderId[] HeaderTypesToStrip = new HeaderId[]
		{
			HeaderId.Bcc
		};

		private static readonly ReplayFileMailer.XHeaderType ProhibitedGatewayHeaderType = ReplayFileMailer.XHeaderType.ExtendedMessageProps | ReplayFileMailer.XHeaderType.LegacyExch50 | ReplayFileMailer.XHeaderType.ReceiverExtendedProps;

		private ReplayFileMailer.XHeaderType xheadersProcessed;

		private string messageCreator;

		private MessageTrackingSource messageSource;

		private string sourceContext;

		[Flags]
		public enum XHeaderType
		{
			None = 0,
			Sender = 1,
			Receiver = 2,
			Source = 4,
			CreatedBy = 8,
			HeloDomain = 16,
			ExtendedMessageProps = 32,
			LegacyExch50 = 64,
			SourceIPAddress = 128,
			ReceiverExtendedProps = 256,
			End = 512
		}
	}
}
