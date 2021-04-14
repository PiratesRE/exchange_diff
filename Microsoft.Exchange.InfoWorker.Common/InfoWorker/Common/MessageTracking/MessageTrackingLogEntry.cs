using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class MessageTrackingLogEntry
	{
		private MessageTrackingLogEntry(MessageTrackingLogRow rowData)
		{
			this.rowData = rowData;
			string text = Names<MessageTrackingEvent>.Map[(int)this.rowData.EventId];
			if (this.rowData.EventId == MessageTrackingEvent.TRANSFER || (this.rowData.EventId == MessageTrackingEvent.RESUBMIT && this.rowData.Source == MessageTrackingSource.REDUNDANCY))
			{
				long num = 0L;
				if (this.rowData.References == null || this.rowData.References.Length != 1 || !long.TryParse(this.rowData.References[0], out num))
				{
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "RelatedInternalId could not be parsed for TRANSFER event from server {0}", new object[]
					{
						this.Server
					});
				}
				this.relatedMailItemId = num;
				return;
			}
			if (this.rowData.EventId == MessageTrackingEvent.RECEIVE)
			{
				if (this.rowData.Source == MessageTrackingSource.SMTP && string.IsNullOrEmpty(this.rowData.ClientHostName) && string.IsNullOrEmpty(this.rowData.ClientIP))
				{
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "No ClientHostName or ClientIP for SMTP RECEIVE from server {0}", new object[]
					{
						this.Server
					});
					return;
				}
			}
			else
			{
				if (this.rowData.EventId == MessageTrackingEvent.SEND && this.rowData.Source == MessageTrackingSource.SMTP)
				{
					if (string.IsNullOrEmpty(this.rowData.ServerHostName) && string.IsNullOrEmpty(this.rowData.ServerIP))
					{
						TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "No ServerHostName or ServerIP for SMTP SEND from server {0}", new object[]
						{
							this.Server
						});
					}
					this.ProcessOutboundProxyTargetServer();
					return;
				}
				if (this.rowData.EventId == MessageTrackingEvent.DELIVER && this.rowData.Source == MessageTrackingSource.STOREDRIVER && this.rowData.RecipientStatuses != null)
				{
					if (this.rowData.RecipientAddresses == null || this.rowData.RecipientAddresses.Length != this.rowData.RecipientStatuses.Length)
					{
						TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "Mismatched RecipientAddress and RecipientStatus counts for STOREDRIVER DELIVER from server {0}", new object[]
						{
							this.Server
						});
					}
					for (int i = 0; i < this.rowData.RecipientAddresses.Length; i++)
					{
						if (!string.IsNullOrEmpty(this.rowData.RecipientStatuses[i]))
						{
							if (this.recipientFolders == null)
							{
								this.recipientFolders = new Dictionary<string, string>(this.rowData.RecipientAddresses.Length);
							}
							this.recipientFolders.Add(this.rowData.RecipientAddresses[i], this.rowData.RecipientStatuses[i]);
						}
					}
					return;
				}
				if (this.rowData.EventId == MessageTrackingEvent.INITMESSAGECREATED)
				{
					if (this.rowData.References == null || this.rowData.References.Length < 1 || string.IsNullOrEmpty(this.rowData.References[0]) || !SmtpAddress.IsValidSmtpAddress(this.rowData.Context))
					{
						TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "No RelatedMessageId for INITMESSAGECREATED from server {0}", new object[]
						{
							this.Server
						});
					}
					this.arbitrationMailboxAddress = SmtpAddress.Parse(this.rowData.Context);
					return;
				}
				if (this.rowData.EventId == MessageTrackingEvent.MODERATORAPPROVE || this.rowData.EventId == MessageTrackingEvent.MODERATORREJECT || this.rowData.EventId == MessageTrackingEvent.MODERATIONEXPIRE)
				{
					if (this.rowData.References == null || this.rowData.References.Length < 1 || string.IsNullOrEmpty(this.rowData.References[0]))
					{
						TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "No RelatedMessageId for {0}", new object[]
						{
							text,
							this.Server
						});
						return;
					}
				}
				else if (this.rowData.EventId == MessageTrackingEvent.HAREDIRECT && this.rowData.Source == MessageTrackingSource.SMTP)
				{
					this.FixupHARedirectEvent();
				}
			}
		}

		private void FixupHARedirectEvent()
		{
			string text = string.Empty;
			Match match = MessageTrackingLogEntry.serverNameFromHAContext.Match(this.rowData.Context);
			if (match.Success)
			{
				text = match.Groups[1].Value;
			}
			else
			{
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "Failed to get destination server value from HAREDIRECT event. MessageId: {0}. Server: {1}. Value {2}", new object[]
				{
					this.rowData.MessageId,
					this.Server,
					this.rowData.Context
				});
			}
			this.clientHostName = this.rowData.ServerHostName;
			this.serverHostName = text;
		}

		private void ProcessOutboundProxyTargetServer()
		{
			string customData = this.rowData.GetCustomData<string>(MessageTrackingLogEntry.OutboundProxyTargetHostNamePropertyName, string.Empty);
			if (!string.IsNullOrEmpty(customData))
			{
				this.serverHostName = customData;
			}
		}

		public static bool TryCreateFromCursor(LogSearchCursor cursor, string server, TrackingErrorCollection errors, out MessageTrackingLogEntry entry)
		{
			entry = null;
			MessageTrackingLogRow messageTrackingLogRow;
			if (MessageTrackingLogRow.TryRead(server, cursor, MessageTrackingLogEntry.allRows, errors, out messageTrackingLogRow))
			{
				entry = new MessageTrackingLogEntry(messageTrackingLogRow);
				return true;
			}
			return false;
		}

		private static HashSet<MessageTrackingEvent> CreateTerminalEventSet()
		{
			return new HashSet<MessageTrackingEvent>
			{
				MessageTrackingEvent.SUBMIT,
				MessageTrackingEvent.TRANSFER,
				MessageTrackingEvent.SEND,
				MessageTrackingEvent.DELIVER,
				MessageTrackingEvent.DUPLICATEDELIVER,
				MessageTrackingEvent.FAIL,
				MessageTrackingEvent.INITMESSAGECREATED,
				MessageTrackingEvent.MODERATORAPPROVE,
				MessageTrackingEvent.MODERATORREJECT,
				MessageTrackingEvent.MODERATIONEXPIRE,
				MessageTrackingEvent.PROCESS,
				MessageTrackingEvent.RESUBMIT,
				MessageTrackingEvent.HAREDIRECT
			};
		}

		public MessageTrackingLogEntry Clone()
		{
			return (MessageTrackingLogEntry)base.MemberwiseClone();
		}

		private static Dictionary<string, MimeRecipientType> CreateMimeRecipientTypeDictionary()
		{
			return new Dictionary<string, MimeRecipientType>(4, StringComparer.OrdinalIgnoreCase)
			{
				{
					"Unknown",
					MimeRecipientType.Unknown
				},
				{
					"To",
					MimeRecipientType.To
				},
				{
					"Cc",
					MimeRecipientType.Cc
				},
				{
					"Bcc",
					MimeRecipientType.Bcc
				}
			};
		}

		public ServerInfo GetNextHopServer()
		{
			if (this.nextHopServer == null)
			{
				if (string.IsNullOrEmpty(this.NextHopFqdnOrName))
				{
					return ServerInfo.NotFound;
				}
				this.nextHopServer = new ServerInfo?(ServerCache.Instance.FindMailboxOrHubServer(this.NextHopFqdnOrName, 32UL));
			}
			return this.nextHopServer.Value;
		}

		public bool IsNextHopCrossSite(DirectoryContext directoryContext)
		{
			if (this.nextHopIsCrossSite == null)
			{
				ADObjectId localServerSiteId = ServerCache.Instance.GetLocalServerSiteId(directoryContext);
				ServerInfo serverInfo = this.GetNextHopServer();
				if (serverInfo.Status != ServerStatus.NotFound && serverInfo.ServerSiteId != null && !serverInfo.ServerSiteId.Equals(localServerSiteId))
				{
					this.nextHopIsCrossSite = new bool?(true);
				}
				else
				{
					this.nextHopIsCrossSite = new bool?(false);
				}
			}
			return this.nextHopIsCrossSite.Value;
		}

		public bool SharesRowDataWithEntry(MessageTrackingLogEntry otherEntry)
		{
			return otherEntry != null && this.rowData == otherEntry.rowData;
		}

		public DateTime Time
		{
			get
			{
				return this.rowData.DateTime;
			}
		}

		public string MessageId
		{
			get
			{
				return this.rowData.MessageId;
			}
		}

		public string Server
		{
			get
			{
				return this.rowData.ServerFqdn;
			}
		}

		public long InternalMessageId
		{
			get
			{
				return this.rowData.InternalMessageId;
			}
		}

		public string[] RecipientAddresses
		{
			get
			{
				if (this.submitRecipientAddresses != null)
				{
					return this.submitRecipientAddresses;
				}
				return this.rowData.RecipientAddresses;
			}
			set
			{
				if (this.EventId != MessageTrackingEvent.SUBMIT && this.EventId != MessageTrackingEvent.MODERATORAPPROVE && this.EventId != MessageTrackingEvent.MODERATORREJECT && (this.EventId != MessageTrackingEvent.EXPAND || !"Federated Delivery Encryption Agent".Equals(this.SourceContext, StringComparison.OrdinalIgnoreCase)))
				{
					throw new InvalidOperationException("Recipient addresses can only be set for SUBMIT, moderator decision and federated delivery events, for all others log-data must be the source");
				}
				this.submitRecipientAddresses = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return this.recipientAddress;
			}
			set
			{
				this.recipientAddress = value;
			}
		}

		public string RootAddress
		{
			get
			{
				return this.rootAddress;
			}
			set
			{
				this.rootAddress = value;
			}
		}

		public MessageTrackingEvent EventId
		{
			get
			{
				return this.rowData.EventId;
			}
		}

		public MessageTrackingSource Source
		{
			get
			{
				return this.rowData.Source;
			}
		}

		public string SourceContext
		{
			get
			{
				return this.rowData.Context;
			}
		}

		public EventTree ProcessedBy
		{
			get
			{
				return this.processedBy;
			}
			set
			{
				this.processedBy = value;
			}
		}

		public virtual long ServerLogKeyMailItemId
		{
			get
			{
				if (this.EventId == MessageTrackingEvent.TRANSFER || (this.EventId == MessageTrackingEvent.RESUBMIT && this.Source == MessageTrackingSource.REDUNDANCY))
				{
					return this.relatedMailItemId;
				}
				return this.InternalMessageId;
			}
		}

		public string NextHopFqdnOrName
		{
			get
			{
				if (string.IsNullOrEmpty(this.serverHostName))
				{
					return this.rowData.ServerHostName;
				}
				return this.serverHostName;
			}
		}

		public string ServerIP
		{
			get
			{
				return this.rowData.ServerIP;
			}
		}

		public string ClientHostName
		{
			get
			{
				if (string.IsNullOrEmpty(this.clientHostName))
				{
					return this.rowData.ClientHostName;
				}
				return this.clientHostName;
			}
		}

		public string ClientIP
		{
			get
			{
				return this.rowData.ClientIP;
			}
		}

		public string Subject
		{
			get
			{
				return this.rowData.MessageSubject;
			}
		}

		public string SenderAddress
		{
			get
			{
				return this.rowData.SenderAddress;
			}
		}

		public long RelatedMailItemId
		{
			get
			{
				return this.relatedMailItemId;
			}
			set
			{
				this.relatedMailItemId = value;
			}
		}

		public string RelatedRecipientAddress
		{
			get
			{
				return this.rowData.RelatedRecipientAddress;
			}
		}

		public string Folder
		{
			get
			{
				if (this.recipientAddress == null || this.recipientFolders == null)
				{
					return null;
				}
				string result = null;
				this.recipientFolders.TryGetValue(this.recipientAddress, out result);
				return result;
			}
		}

		public string[] RecipientStatuses
		{
			get
			{
				return this.rowData.RecipientStatuses;
			}
		}

		public string RecipientStatus
		{
			get
			{
				return this.recipientStatus;
			}
			set
			{
				this.recipientStatus = value;
			}
		}

		public string InitMessageId
		{
			get
			{
				return this.rowData.References[0];
			}
		}

		public SmtpAddress ArbitrationMailboxAddress
		{
			get
			{
				return this.arbitrationMailboxAddress;
			}
		}

		public string OrigMessageId
		{
			get
			{
				return this.rowData.References[0];
			}
		}

		public bool HiddenRecipient
		{
			get
			{
				return this.hiddenRecipient;
			}
			set
			{
				this.hiddenRecipient = value;
			}
		}

		public bool? BccRecipient
		{
			get
			{
				return this.bccRecipient;
			}
			set
			{
				this.bccRecipient = value;
			}
		}

		public string FederatedDeliveryAddress
		{
			get
			{
				return this.federatedDeliveryAddress;
			}
			set
			{
				this.federatedDeliveryAddress = value;
			}
		}

		public bool IsTerminalEvent
		{
			get
			{
				return MessageTrackingLogEntry.terminalEventTypes.Contains(this.EventId);
			}
		}

		public KeyValuePair<string, object>[] CustomData
		{
			get
			{
				return this.rowData.CustomData;
			}
		}

		public string TenantId
		{
			get
			{
				return this.rowData.TenantId;
			}
		}

		public bool IsEntryCompatible
		{
			get
			{
				return this.rowData.IsLogCompatible;
			}
		}

		internal MessageTrackingLogRow LogRow
		{
			get
			{
				return this.rowData;
			}
		}

		private static HashSet<MessageTrackingEvent> terminalEventTypes = MessageTrackingLogEntry.CreateTerminalEventSet();

		private static Dictionary<string, MimeRecipientType> mimeRecipientTypes = MessageTrackingLogEntry.CreateMimeRecipientTypeDictionary();

		private static BitArray allRows = new BitArray(MessageTrackingLogRow.FieldCount, true);

		private static Regex serverNameFromHAContext = new Regex("(^.+)(?==250)");

		private static string OutboundProxyTargetHostNamePropertyName = "OutboundProxyTargetHostName";

		private MessageTrackingLogRow rowData;

		private long relatedMailItemId;

		private string recipientAddress;

		private string rootAddress;

		private string[] submitRecipientAddresses;

		private string federatedDeliveryAddress;

		private string recipientStatus;

		private Dictionary<string, string> recipientFolders;

		private ServerInfo? nextHopServer;

		private SmtpAddress arbitrationMailboxAddress;

		private bool? nextHopIsCrossSite;

		private EventTree processedBy;

		private bool hiddenRecipient;

		private bool? bccRecipient = null;

		private string serverHostName;

		private string clientHostName;

		public static readonly Dictionary<string, MimeRecipientType> RecipientAddressTypeGetter = new Dictionary<string, MimeRecipientType>
		{
			{
				"To",
				MimeRecipientType.To
			},
			{
				"Cc",
				MimeRecipientType.Cc
			},
			{
				"Bcc",
				MimeRecipientType.Bcc
			},
			{
				"Unknown",
				MimeRecipientType.Unknown
			}
		};
	}
}
