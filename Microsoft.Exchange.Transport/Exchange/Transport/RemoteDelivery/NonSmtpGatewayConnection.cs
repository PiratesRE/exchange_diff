using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class NonSmtpGatewayConnection
	{
		public NonSmtpGatewayConnection(NextHopConnection connection)
		{
			this.eventLogger = NonSmtpGatewayConnectionHandler.EventLogger;
			this.nextHopConnection = connection;
		}

		public bool Retired
		{
			get
			{
				return this.retired;
			}
		}

		public void Retire()
		{
			this.retired = true;
			ExTraceGlobals.QueuingTracer.TraceDebug<string>((long)this.GetHashCode(), "Retire Gateway Connection {0}", this.nextHopConnection.Key.NextHopDomain);
		}

		public void StartConnection()
		{
			ExTraceGlobals.QueuingTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "StartConnection for nonSMTP gateway {0}, Total Q Count {1}, Active Q Length {2}", this.nextHopConnection.Key.NextHopDomain, this.nextHopConnection.TotalQueueLength, this.nextHopConnection.ActiveQueueLength);
			if (!Components.RoutingComponent.MailRouter.TryGetLocalSendConnector<ForeignConnector>(this.nextHopConnection.Key.NextHopConnector, out this.gatewayConnector))
			{
				ExTraceGlobals.QueuingTracer.TraceError((long)this.GetHashCode(), "Connector is deleted or disabled");
				this.AckConnection(AckStatus.Resubmit, AckReason.GWConnectorDeleted);
				return;
			}
			if (!this.DropDirectoryExists())
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<string>((long)this.GetHashCode(), "NonSmtp Connector {0} Drop Directory does not exist", this.nextHopConnection.Key.NextHopDomain);
				this.AckConnection(AckStatus.Retry, AckReason.GWNoDropDirectory);
				return;
			}
			if (this.QuotaExceeded())
			{
				this.AckConnection(AckStatus.Retry, AckReason.GWQuotaExceeded);
				return;
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<string>((long)this.GetHashCode(), "Start local delivery for connection to {0}", this.nextHopConnection.Key.NextHopDomain);
			if (this.gatewayConnector.RelayDsnRequired)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<string>((long)this.GetHashCode(), "NonSmtp Connector {0} Requires RelayDSNs", this.nextHopConnection.Key.NextHopDomain);
				this.nextHopConnection.GenerateSuccessDSNs = DsnFlags.Relay;
			}
			this.DeliverMessages();
		}

		private void DeliverMessages()
		{
			while (!this.retired)
			{
				RoutedMailItem nextRoutedMailItem = this.nextHopConnection.GetNextRoutedMailItem();
				if (nextRoutedMailItem == null)
				{
					break;
				}
				PoisonMessage.Context = new MessageContext(nextRoutedMailItem.RecordId, nextRoutedMailItem.InternetMessageId, MessageProcessingSource.NonSmtpGateway);
				if (!this.DeliverMessage(nextRoutedMailItem))
				{
					return;
				}
				if (this.QuotaExceeded())
				{
					this.AckConnection(AckStatus.Retry, AckReason.GWQuotaExceeded);
					return;
				}
			}
			this.AckConnection(AckStatus.Success, SmtpResponse.NoopOk);
		}

		private bool DeliverMessage(RoutedMailItem mailItem)
		{
			SmtpResponse smtpResponse;
			string exceptionMessage;
			bool flag = this.WriteMessage(mailItem, this.nextHopConnection.ReadyRecipients, out smtpResponse, out exceptionMessage);
			if (flag)
			{
				for (MailRecipient nextRecipient = this.nextHopConnection.GetNextRecipient(); nextRecipient != null; nextRecipient = this.nextHopConnection.GetNextRecipient())
				{
					this.nextHopConnection.AckRecipient(AckStatus.Success, SmtpResponse.NoopOk);
				}
				this.nextHopConnection.AckMailItem(AckStatus.Success, SmtpResponse.NoopOk, null, MessageTrackingSource.GATEWAY, LatencyComponent.NonSmtpGateway, true);
			}
			else
			{
				this.nextHopConnection.AckMailItem(AckStatus.Pending, smtpResponse, null, MessageTrackingSource.GATEWAY, LatencyComponent.NonSmtpGateway, true);
				this.AckConnection(AckStatus.Retry, smtpResponse, exceptionMessage);
			}
			return flag;
		}

		private bool WriteMessage(RoutedMailItem mailItem, IEnumerable<MailRecipient> recipients, out SmtpResponse smtpResponse, out string exceptionMessage)
		{
			bool result = false;
			exceptionMessage = null;
			smtpResponse = SmtpResponse.NoopOk;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}-{1}-{2}.{3}.eml", new object[]
				{
					Environment.MachineName,
					mailItem.RecordId,
					DateTime.UtcNow.ToString("yyyyMMddHHmmssZ", DateTimeFormatInfo.InvariantInfo),
					((IQueueItem)mailItem).Priority
				});
				string text = Path.Combine(this.dropDirectory, stringBuilder.ToString());
				ExTraceGlobals.QueuingTracer.TraceDebug<long>((long)this.GetHashCode(), "TryCreateExportStream for mailitem.Id {0}", mailItem.RecordId);
				Stream stream;
				if (!ExportStream.TryCreate(mailItem, recipients, true, out stream) || stream == null)
				{
					throw new InvalidOperationException("Failed to create an export stream because there were no ready recipients");
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<string>((long)this.GetHashCode(), "Write MailItem as filename {0}", text);
				using (stream)
				{
					using (FileStream fileStream = new FileStream(text, FileMode.CreateNew, FileAccess.Write, FileShare.None))
					{
						this.nextHopConnection.ConnectionAttemptSucceeded();
						stream.Position = 0L;
						for (;;)
						{
							int num = stream.Read(this.buffer, 0, 65536);
							if (num == 0)
							{
								break;
							}
							fileStream.Write(this.buffer, 0, num);
						}
					}
				}
				ExTraceGlobals.QueuingTracer.TraceDebug<string>((long)this.GetHashCode(), "file {0} written successfully", text);
				result = true;
			}
			catch (PathTooLongException ex)
			{
				smtpResponse = AckReason.GWPathTooLongException;
				ExTraceGlobals.QueuingTracer.TraceError<string>((long)this.GetHashCode(), "PathTooLongException writing nonSMTP gateway message. Exception text: {0}", ex.Message);
			}
			catch (IOException ex2)
			{
				exceptionMessage = ex2.Message;
				smtpResponse = AckReason.GWIOException;
				ExTraceGlobals.QueuingTracer.TraceError<IOException>((long)this.GetHashCode(), "IOException writing nonSMTP gateway message. Exception text: Message{0}", ex2);
			}
			catch (UnauthorizedAccessException arg)
			{
				smtpResponse = AckReason.GWUnauthorizedAccess;
				ExTraceGlobals.QueuingTracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "UnauthorizedAccessException writing nonSMTP gateway message. Exception text: {0}", arg);
			}
			return result;
		}

		private bool DropDirectoryExists()
		{
			string text = Components.Configuration.LocalServer.TransportServer.RootDropDirectoryPath;
			if (string.IsNullOrEmpty(text))
			{
				text = ConfigurationContext.Setup.InstallPath;
			}
			if (string.IsNullOrEmpty(this.gatewayConnector.DropDirectory))
			{
				return false;
			}
			this.dropDirectory = Path.Combine(text, this.gatewayConnector.DropDirectory);
			this.directoryInfo = new DirectoryInfo(this.dropDirectory);
			if (!this.directoryInfo.Exists)
			{
				ExTraceGlobals.QueuingTracer.TraceError<string>((long)this.GetHashCode(), "Drop Directory does not exist or has bad ACLs {0}", this.dropDirectory);
				return false;
			}
			return true;
		}

		private bool QuotaExceeded()
		{
			if (this.gatewayConnector.DropDirectoryQuota.IsUnlimited)
			{
				return false;
			}
			long num = (long)((double)this.gatewayConnector.DropDirectoryQuota.Value);
			long num2 = 0L;
			FileInfo[] files = this.directoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				num2 += files[i].Length;
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<long, long, string>((long)this.GetHashCode(), "DropDir size {0} QuotaSize {1} NonSmtp Connector {2}", num2, num, this.nextHopConnection.Key.NextHopDomain);
			if (num2 >= num)
			{
				ExTraceGlobals.QueuingTracer.TraceWarning<string, long>((long)this.GetHashCode(), "NonSmtp Connector {0} QuotaSize {1} is exceeded", this.nextHopConnection.Key.NextHopDomain, num);
				return true;
			}
			return false;
		}

		private void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.AckConnection(ackStatus, smtpResponse, null);
		}

		private void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse, string exceptionMessage)
		{
			if (this.nextHopConnection == null)
			{
				throw new InvalidOperationException("Connection has already been acked!");
			}
			ExTraceGlobals.QueuingTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Stop nonSMTP gateway Connection for {0} with ackStatus {1}", this.nextHopConnection.Key.NextHopDomain, (int)ackStatus);
			if (ackStatus != AckStatus.Success)
			{
				if (smtpResponse.StatusText[0].Equals(AckReason.GWPathTooLongException.StatusText[0]))
				{
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NonSmtpGWPathTooLongException, this.nextHopConnection.Key.NextHopDomain, new object[]
					{
						this.nextHopConnection.Key.NextHopDomain,
						this.dropDirectory
					});
				}
				else if (smtpResponse.StatusText[0].Equals(AckReason.GWNoDropDirectory.StatusText[0]))
				{
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NonSmtpGWNoDropDirectory, this.nextHopConnection.Key.NextHopDomain, new object[]
					{
						this.nextHopConnection.Key.NextHopDomain,
						this.dropDirectory
					});
				}
				else if (smtpResponse.StatusText[0].Equals(AckReason.GWUnauthorizedAccess.StatusText[0]))
				{
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NonSmtpGWUnauthorizedAccess, this.nextHopConnection.Key.NextHopDomain, new object[]
					{
						this.nextHopConnection.Key.NextHopDomain,
						this.dropDirectory
					});
				}
				else if (smtpResponse.StatusText[0].Equals(AckReason.GWIOException.StatusText[0]))
				{
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NonSmtpGWBadDropDirectory, this.nextHopConnection.Key.NextHopDomain, new object[]
					{
						this.nextHopConnection.Key.NextHopDomain,
						this.dropDirectory,
						exceptionMessage
					});
				}
				else if (smtpResponse.StatusText[0].Equals(AckReason.GWQuotaExceeded.StatusText[0]))
				{
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NonSmtpGWQuotaExceeded, this.nextHopConnection.Key.NextHopDomain, new object[]
					{
						this.nextHopConnection.Key.NextHopDomain,
						this.gatewayConnector.DropDirectoryQuota.Value
					});
				}
			}
			this.nextHopConnection.AckConnection(ackStatus, smtpResponse, null);
			this.nextHopConnection = null;
		}

		private const int BlockSize = 65536;

		private ExEventLog eventLogger;

		private NextHopConnection nextHopConnection;

		private ForeignConnector gatewayConnector;

		private string dropDirectory;

		private DirectoryInfo directoryInfo;

		private volatile bool retired;

		private byte[] buffer = new byte[65536];
	}
}
