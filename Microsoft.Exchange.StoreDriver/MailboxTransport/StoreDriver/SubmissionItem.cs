using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Internal;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal abstract class SubmissionItem : IDisposable
	{
		static SubmissionItem()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
			SubmissionItem.serverVersion = versionInfo.FileVersion;
		}

		public SubmissionItem(string mailProtocol) : this(mailProtocol, null, null)
		{
		}

		public SubmissionItem(string mailProtocol, MailItemSubmitter context, SubmissionInfo submissionInfo)
		{
			this.conversionOptions = SubmissionItem.GetGlobalConversionOptions();
			this.mailProtocol = mailProtocol;
			this.Context = context;
			this.submissionInfo = submissionInfo;
		}

		public OutboundConversionOptions ConversionOptions
		{
			get
			{
				return this.conversionOptions;
			}
		}

		public ConversionResult ConversionResult
		{
			get
			{
				return this.conversionResult;
			}
		}

		public StoreSession Session
		{
			get
			{
				return this.storeSession;
			}
			protected set
			{
				this.storeSession = value;
			}
		}

		public bool ResubmittedMessage
		{
			get
			{
				int valueOrDefault = this.GetValueTypePropValue<int>(MessageItemSchema.Flags).GetValueOrDefault();
				return (valueOrDefault & 128) != 0;
			}
		}

		public MessageItem Item
		{
			get
			{
				return this.messageItem;
			}
			protected set
			{
				this.messageItem = value;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageItem.ClassName;
			}
		}

		public abstract string SourceServerFqdn { get; }

		public abstract IPAddress SourceServerNetworkAddress { get; }

		public abstract DateTime OriginalCreateTime { get; }

		public Participant Sender
		{
			get
			{
				return this.messageItem.Sender;
			}
		}

		public string QuarantineOriginalSender
		{
			get
			{
				return this.GetRefTypePropValue<string>(ItemSchema.QuarantineOriginalSender);
			}
		}

		public RecipientCollection Recipients
		{
			get
			{
				return this.messageItem.Recipients;
			}
		}

		public virtual bool HasMessageItem
		{
			get
			{
				return this.storeSession != null && this.messageItem != null;
			}
		}

		public bool IsSubmitMessage
		{
			get
			{
				int? valueTypePropValue = this.GetValueTypePropValue<int>(MessageItemSchema.Flags);
				return valueTypePropValue != null && (valueTypePropValue.Value & 4) == 4;
			}
		}

		public bool IsElcJournalReport
		{
			get
			{
				return this.GetRefTypePropValue<string>(MessageItemSchema.ElcAutoCopyLabel) != null;
			}
		}

		public bool IsMapiAdminSubmission
		{
			get
			{
				bool? valueTypePropValue = this.GetValueTypePropValue<bool>(ItemSchema.SubmittedByAdmin);
				return valueTypePropValue != null && valueTypePropValue.Value;
			}
		}

		public bool IsDlExpansionProhibited
		{
			get
			{
				bool? valueTypePropValue = this.GetValueTypePropValue<bool>(MessageItemSchema.DlExpansionProhibited);
				return valueTypePropValue != null && valueTypePropValue.Value;
			}
		}

		public bool IsAltRecipientProhibited
		{
			get
			{
				bool? valueTypePropValue = this.GetValueTypePropValue<bool>(MessageItemSchema.RecipientReassignmentProhibited);
				return valueTypePropValue != null && valueTypePropValue.Value;
			}
		}

		public virtual bool Done
		{
			get
			{
				return false;
			}
		}

		public virtual TenantPartitionHint TenantPartitionHint
		{
			get
			{
				return this.tenantPartitionHint;
			}
			set
			{
				this.tenantPartitionHint = value;
			}
		}

		private protected MailItemSubmitter Context { protected get; private set; }

		public static T? GetValueTypePropValue<T>(Recipient recipient, PropertyDefinition propDefinition) where T : struct
		{
			object obj = recipient.TryGetProperty(propDefinition);
			SubmissionItem.LogPropError(obj);
			if (obj == null || !(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		public static T GetRefTypePropValue<T>(Recipient recipient, PropertyDefinition propDefinition) where T : class
		{
			object obj = recipient.TryGetProperty(propDefinition);
			SubmissionItem.LogPropError(obj);
			return obj as T;
		}

		public virtual uint OpenStore()
		{
			return 0U;
		}

		public virtual uint LoadFromStore()
		{
			return 0U;
		}

		public virtual Exception DoneWithMessage()
		{
			return null;
		}

		public void CopyContentTo(TransportMailItem mailItem)
		{
			this.conversionOptions.RecipientCache = mailItem.ADRecipientCache;
			this.conversionOptions.UserADSession = mailItem.ADRecipientCache.ADSession;
			mailItem.CacheTransportSettings();
			this.conversionOptions.ClearCategories = mailItem.TransportSettings.ClearCategories;
			this.conversionOptions.UseRFC2231Encoding = mailItem.TransportSettings.Rfc2231EncodingEnabled;
			this.conversionOptions.AllowDlpHeadersToPenetrateFirewall = true;
			SubmissionItem.diag.TraceDebug<long>(0L, "Generate content for mailitem {0}", mailItem.RecordId);
			using (Stream stream = mailItem.OpenMimeWriteStream(MimeLimits.Default))
			{
				this.conversionResult = ItemConversion.ConvertItemToSummaryTnef(this.messageItem, stream, this.conversionOptions);
				stream.Flush();
			}
		}

		public void DecorateMessage(TransportMailItem message)
		{
			message.HeloDomain = Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName;
			message.ReceiveConnectorName = "FromLocal";
			message.RefreshMimeSize();
			long mimeSize = message.MimeSize;
			HeaderList headers = message.RootPart.Headers;
			if (!(headers.FindFirst(HeaderId.Date) is DateHeader))
			{
				DateHeader newChild = new DateHeader("Date", DateTime.UtcNow.ToLocalTime());
				headers.AppendChild(newChild);
			}
			headers.RemoveAll(HeaderId.Received);
			DateHeader dateHeader = new DateHeader("Date", DateTime.UtcNow.ToLocalTime());
			string value = dateHeader.Value;
			ReceivedHeader newChild2 = new ReceivedHeader(this.SourceServerFqdn, StoreDriver.FormatIPAddress(this.SourceServerNetworkAddress), StoreDriver.LocalIP.HostName, StoreDriver.ReceivedHeaderTcpInfo, null, this.mailProtocol, SubmissionItem.serverVersion, null, value);
			headers.PrependChild(newChild2);
			message.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.ElcJournalReport", this.IsElcJournalReport);
			if (this.IsMapiAdminSubmission)
			{
				headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Mapi-Admin-Submission", string.Empty));
			}
			if (this.IsDlExpansionProhibited)
			{
				headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-DL-Expansion-Prohibited", string.Empty));
			}
			if (Components.TransportAppConfig.Resolver.EnableForwardingProhibitedFeature && this.IsAltRecipientProhibited)
			{
				headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Alt-Recipient-Prohibited", string.Empty));
			}
			headers.RemoveAll("X-MS-Exchange-Organization-OriginalSize");
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalSize", mimeSize.ToString(NumberFormatInfo.InvariantInfo)));
			headers.RemoveAll("X-MS-Exchange-Organization-OriginalArrivalTime");
			Header newChild3 = new AsciiTextHeader("X-MS-Exchange-Organization-OriginalArrivalTime", Util.FormatOrganizationalMessageArrivalTime(this.OriginalCreateTime));
			headers.AppendChild(newChild3);
			headers.RemoveAll("X-MS-Exchange-Organization-MessageSource");
			Header newChild4 = new AsciiTextHeader("X-MS-Exchange-Organization-MessageSource", "StoreDriver");
			headers.AppendChild(newChild4);
			message.Directionality = MailDirectionality.Originating;
			message.UpdateDirectionalityAndScopeHeaders();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public T? GetValueTypePropValue<T>(PropertyDefinition propDefinition) where T : struct
		{
			object obj = this.messageItem.TryGetProperty(propDefinition);
			SubmissionItem.LogPropError(obj);
			if (obj == null || !(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		public T GetRefTypePropValue<T>(PropertyDefinition propDefinition) where T : class
		{
			object obj = this.messageItem.TryGetProperty(propDefinition);
			SubmissionItem.LogPropError(obj);
			return obj as T;
		}

		public T GetPropValue<T>(PropertyDefinition propDefinition, T defaultValue) where T : struct
		{
			object obj = this.messageItem.TryGetProperty(propDefinition);
			SubmissionItem.LogPropError(obj);
			if (obj != null && obj is T)
			{
				return (T)((object)obj);
			}
			return defaultValue;
		}

		public void ApplySecurityAttributesTo(TransportMailItem mailitem)
		{
			TransportConfigContainer transportConfigObject = Configuration.TransportConfigObject;
			if (this.GetPropValue<bool>(MessageItemSchema.ClientSubmittedSecurely, false))
			{
				MultilevelAuth.EnsureSecurityAttributes(mailitem, SubmitAuthCategory.Internal, MultilevelAuthMechanism.SecureMapiSubmit, null);
				return;
			}
			if (transportConfigObject.VerifySecureSubmitEnabled)
			{
				MultilevelAuth.EnsureSecurityAttributes(mailitem, SubmitAuthCategory.Anonymous, MultilevelAuthMechanism.MapiSubmit, null);
				return;
			}
			MultilevelAuth.EnsureSecurityAttributes(mailitem, SubmitAuthCategory.Internal, MultilevelAuthMechanism.MapiSubmit, null);
		}

		protected void SetSessionTimeZone()
		{
			this.Session.ExTimeZone = ExTimeZone.CurrentTimeZone;
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		protected void DisposeMapiObjects()
		{
			if (this.Item != null)
			{
				this.Item.Dispose();
				this.Item = null;
			}
			if (this.Session != null)
			{
				if (this.Context != null)
				{
					LatencyTracker.BeginTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession, this.Context.LatencyTracker);
				}
				try
				{
					this.Session.Dispose();
					this.Session = null;
				}
				finally
				{
					if (this.Context != null)
					{
						TimeSpan additionalLatency = LatencyTracker.EndTrackLatency(LatencyComponent.StoreDriverSubmissionRpc, this.Context.LatencyTracker);
						this.Context.AddRpcLatency(additionalLatency, "Session dispose");
					}
				}
			}
			if (this.Context != null)
			{
				MailItemSubmitter.TraceLatency(this.submissionInfo, "RPC", this.Context.RpcLatency);
			}
		}

		private static void LogPropError(object value)
		{
			PropertyError propertyError = value as PropertyError;
			if (propertyError != null && propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
			{
				SubmissionItem.diag.TraceDebug<PropertyError>(0L, "Error when trying to access prop : {0}", propertyError);
			}
		}

		private static OutboundConversionOptions GetGlobalConversionOptions()
		{
			return new OutboundConversionOptions(new EmptyRecipientCache(), Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName)
			{
				DsnMdnOptions = DsnMdnOptions.PropagateUserSettings,
				DsnHumanReadableWriter = Components.DsnGenerator.DsnHumanReadableWriter,
				Limits = 
				{
					MimeLimits = MimeLimits.Unlimited
				},
				QuoteDisplayNameBeforeRfc2047Encoding = Components.TransportAppConfig.ContentConversion.QuoteDisplayNameBeforeRfc2047Encoding,
				LogDirectoryPath = Components.Configuration.LocalServer.ContentConversionTracingPath
			};
		}

		private const int ResendMessageFlag = 128;

		private static readonly Microsoft.Exchange.Diagnostics.Trace diag = ExTraceGlobals.MapiSubmitTracer;

		private static string serverVersion;

		private TenantPartitionHint tenantPartitionHint;

		private StoreSession storeSession;

		private MessageItem messageItem;

		private OutboundConversionOptions conversionOptions;

		private ConversionResult conversionResult;

		private string mailProtocol;

		private SubmissionInfo submissionInfo;
	}
}
