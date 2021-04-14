using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.Shared.Providers;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.Internal;

namespace Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem
{
	internal abstract class SubmissionItemBase : IDisposable
	{
		static SubmissionItemBase()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
			SubmissionItemBase.serverVersion = versionInfo.FileVersion;
		}

		public SubmissionItemBase(string mailProtocol, IStoreDriverTracer storeDriverTracer)
		{
			this.conversionOptions = ConfigurationProvider.GetGlobalConversionOptions();
			this.mailProtocol = mailProtocol;
			this.storeDriverTracer = storeDriverTracer;
			try
			{
				this.localIp = Dns.GetHostEntry(Dns.GetHostName());
			}
			catch (SocketException ex)
			{
				this.storeDriverTracer.StoreDriverCommonTracer.TraceFail<string>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Unable to determine local ip {0}", ex.ToString());
				throw;
			}
		}

		public SubmissionItemBase(string mailProtocol) : this(mailProtocol, new StoreDriverTracer())
		{
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

		public IStoreDriverTracer StoreDriverTracer
		{
			get
			{
				return this.storeDriverTracer;
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

		public IPHostEntry LocalIP
		{
			get
			{
				return this.localIp;
			}
		}

		public string ReceivedHeaderTcpInfo
		{
			get
			{
				return SubmissionItemBase.FormatIPAddress(this.localIp.AddressList[0]);
			}
		}

		public static T? GetValueTypePropValue<T>(Recipient recipient, PropertyDefinition propDefinition) where T : struct
		{
			object obj = recipient.TryGetProperty(propDefinition);
			SubmissionItemBase.LogPropError(obj);
			if (obj == null || !(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		public static T GetRefTypePropValue<T>(Recipient recipient, PropertyDefinition propDefinition) where T : class
		{
			object obj = recipient.TryGetProperty(propDefinition);
			SubmissionItemBase.LogPropError(obj);
			return obj as T;
		}

		public static string FormatIPAddress(IPAddress address)
		{
			return "[" + address.ToString() + "]";
		}

		public virtual uint LoadFromStore()
		{
			return 0U;
		}

		public virtual Exception DoneWithMessage()
		{
			return null;
		}

		public TimeSpan CopyContentTo(TransportMailItem mailItem)
		{
			this.ConversionOptions.RecipientCache = mailItem.ADRecipientCache;
			this.ConversionOptions.UserADSession = mailItem.ADRecipientCache.ADSession;
			mailItem.CacheTransportSettings();
			this.ConversionOptions.ClearCategories = mailItem.TransportSettings.ClearCategories;
			this.ConversionOptions.UseRFC2231Encoding = mailItem.TransportSettings.Rfc2231EncodingEnabled;
			this.ConversionOptions.AllowDlpHeadersToPenetrateFirewall = true;
			this.storeDriverTracer.StoreDriverCommonTracer.TracePass<long>(this.storeDriverTracer.MessageProbeActivityId, 0L, "Generate content for mailitem {0}", mailItem.RecordId);
			ExDateTime utcNow = ExDateTime.UtcNow;
			using (Stream stream = mailItem.OpenMimeWriteStream(MimeLimits.Default))
			{
				this.conversionResult = ItemConversion.ConvertItemToSummaryTnef(this.messageItem, stream, this.ConversionOptions);
				stream.Flush();
			}
			return ExDateTime.UtcNow - utcNow;
		}

		public void DecorateMessage(TransportMailItem message)
		{
			message.HeloDomain = ConfigurationProvider.GetDefaultDomainName();
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
			ReceivedHeader newChild2 = new ReceivedHeader(this.SourceServerFqdn, SubmissionItemBase.FormatIPAddress(this.SourceServerNetworkAddress), this.LocalIP.HostName, this.ReceivedHeaderTcpInfo, null, this.mailProtocol, SubmissionItemBase.serverVersion, null, value);
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
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Processed-By-MBTSubmission", string.Empty));
			if (ConfigurationProvider.GetForwardingProhibitedFeatureStatus() && this.IsAltRecipientProhibited)
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
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Transport-FromEntityHeader", RoutingEndpoint.Hosted.ToString()));
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-FromEntityHeader", RoutingEndpoint.Hosted.ToString()));
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
			SubmissionItemBase.LogPropError(obj);
			if (obj == null || !(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		public T GetRefTypePropValue<T>(PropertyDefinition propDefinition) where T : class
		{
			object obj = this.messageItem.TryGetProperty(propDefinition);
			SubmissionItemBase.LogPropError(obj);
			return obj as T;
		}

		public T GetPropValue<T>(PropertyDefinition propDefinition, T defaultValue) where T : struct
		{
			object obj = this.messageItem.TryGetProperty(propDefinition);
			SubmissionItemBase.LogPropError(obj);
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
			if (this.Session != null)
			{
				this.Session.ExTimeZone = ExTimeZone.CurrentTimeZone;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		protected void DisposeMessageItem()
		{
			if (this.Item != null)
			{
				this.Item.Dispose();
				this.Item = null;
			}
		}

		protected void DisposeStoreSession()
		{
			if (this.Session != null)
			{
				this.Session.Dispose();
				this.Session = null;
			}
		}

		private static void LogPropError(object value)
		{
			PropertyError propertyError = value as PropertyError;
			if (propertyError != null && propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
			{
				TraceHelper.StoreDriverTracer.TracePass<PropertyError>(TraceHelper.MessageProbeActivityId, 0L, "Error when trying to access prop : {0}", propertyError);
			}
		}

		private const int ResendMessageFlag = 128;

		private static string serverVersion;

		private readonly IPHostEntry localIp;

		private readonly string mailProtocol;

		private StoreSession storeSession;

		private MessageItem messageItem;

		private OutboundConversionOptions conversionOptions;

		private ConversionResult conversionResult;

		private IStoreDriverTracer storeDriverTracer;
	}
}
