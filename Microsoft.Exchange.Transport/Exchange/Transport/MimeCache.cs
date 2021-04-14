using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport
{
	internal class MimeCache : IDataWithinRowComponent, IDataObjectComponent
	{
		public MimeCache(IMailItemStorage parent)
		{
			this.parent = parent;
			this.FallbackToRawOverride = false;
		}

		public MimeDocument MimeDocument
		{
			get
			{
				if (this.mimeDocument == null)
				{
					lock (this.parent)
					{
						if (this.mimeDocument == null)
						{
							if (this.deferredLoadAllowed)
							{
								this.DeferredLoad();
							}
							else
							{
								this.NewMimeDocument(MimeLimits.Unlimited);
							}
						}
					}
				}
				return this.mimeDocument;
			}
		}

		public MimePart RootPart
		{
			get
			{
				return this.MimeDocument.RootPart;
			}
		}

		public EmailMessage Message
		{
			get
			{
				if (this.emailMessage == null)
				{
					lock (this.parent)
					{
						if (this.emailMessage == null)
						{
							if (this.MimeDocument.RootPart == null)
							{
								this.InitializeRootPart();
							}
							if (this.emailMessage == null)
							{
								if (this.IsReadOnly)
								{
									throw new InvalidOperationException("EmailMessage should be pre-created in the r/o mode");
								}
								this.emailMessage = EmailMessage.Create(this.MimeDocument);
							}
						}
					}
				}
				return this.emailMessage;
			}
		}

		public long MimeStreamSize
		{
			get
			{
				if (this.mimeDocument != null)
				{
					long num = this.parent.MimeSize;
					if (this.persistedMimeVersion != this.mimeDocument.Version)
					{
						num = this.mimeDocument.WriteTo(Stream.Null);
						if (!this.IsReadOnly)
						{
							this.parent.MimeSize = num;
						}
					}
					return num;
				}
				if (!this.saved)
				{
					return 0L;
				}
				return this.parent.MimeSize;
			}
			private set
			{
				this.ThrowIfReadOnly();
				this.parent.MimeSize = value;
			}
		}

		public bool MimeWriteStreamOpen
		{
			get
			{
				return this.mimeWriteStream != null && this.mimeWriteStream.CanWrite;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.parent.IsReadOnly;
			}
		}

		public bool IsDirty
		{
			get
			{
				return ((IDataObjectComponent)this).PendingDatabaseUpdates;
			}
		}

		public bool FallbackToRawOverride
		{
			get
			{
				return this.fallbackToRawOverride;
			}
			set
			{
				this.fallbackToRawOverride = value;
			}
		}

		public Encoding DefaultEncoding
		{
			get
			{
				return this.defaultEncoding;
			}
			set
			{
				this.defaultEncoding = value;
			}
		}

		bool IDataObjectComponent.PendingDatabaseUpdates
		{
			get
			{
				return this.mimeDocument != null && this.mimeDocument.RootPart != null && this.persistedMimeVersion != this.mimeDocument.Version;
			}
		}

		int IDataObjectComponent.PendingDatabaseUpdateCount
		{
			get
			{
				if (!((IDataObjectComponent)this).PendingDatabaseUpdates)
				{
					return 0;
				}
				long num = this.parent.MimeSize / 102400L;
				if (num < 2147483647L)
				{
					return (int)num + 1;
				}
				return int.MaxValue;
			}
		}

		public static void SetConfig(bool priorityHeaderPromotionEnabled)
		{
			MimeCache.priorityHeaderPromotionEnabled = priorityHeaderPromotionEnabled;
		}

		public Stream OpenMimeWriteStream(MimeLimits mimeLimits, bool expectBinaryContent)
		{
			this.ThrowIfReadOnly();
			this.audit.Drop(Breadcrumb.OpenMimeWriteStream);
			this.NewMimeDocument(mimeLimits);
			this.parent.MimeNotSequential = false;
			this.canRestore = this.saved;
			this.saved = false;
			this.mimeWriteStream = MimeInternalHelpers.GetLoadStream(this.mimeDocument, expectBinaryContent);
			return this.mimeWriteStream;
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			this.audit.Drop(Breadcrumb.OpenMimeReadStream);
			Stream stream2;
			if (downConvert)
			{
				Stream stream = Streams.CreateTemporaryStorageStream();
				using (EightToSevenBitConverter.OutputFilter outputFilter = new EightToSevenBitConverter.OutputFilter())
				{
					if (this.RootPart != null)
					{
						this.RootPart.WriteTo(stream, null, outputFilter);
					}
				}
				stream2 = new ReadOnlyStream(stream);
			}
			else
			{
				if (!this.IsReadOnly && this.mimeReadStream != null && this.mimeDocument != null && this.readStreamVersion != this.mimeDocument.Version)
				{
					this.mimeReadStream = null;
				}
				if (this.mimeReadStream == null)
				{
					lock (this.parent)
					{
						if (this.mimeReadStream == null)
						{
							if (this.persistedMimeVersion != -2147483648 && this.mimeDocument == null && !this.parent.MimeNotSequential)
							{
								this.mimeReadStream = this.parent.OpenMimeDBReader();
								this.readStreamVersion = this.persistedMimeVersion;
							}
							else
							{
								Stream stream3 = Streams.CreateTemporaryStorageStream();
								if (this.RootPart != null)
								{
									this.RootPart.WriteTo(stream3);
								}
								this.mimeReadStream = new ReadOnlyStream(stream3);
								this.readStreamVersion = this.MimeDocument.Version;
							}
						}
					}
				}
				stream2 = new SynchronizedStream(this.mimeReadStream);
			}
			stream2.Position = 0L;
			return stream2;
		}

		public void RestoreLastSavedMime()
		{
			this.ThrowIfReadOnly();
			this.audit.Drop(Breadcrumb.RestoreLastSavedMime);
			this.mimeWriteStream = null;
			this.CleanupEmailMessage();
			this.CleanupMimeDocument();
			this.deferredLoadAllowed = this.canRestore;
		}

		void IDataObjectComponent.CloneFrom(IDataObjectComponent otherComponent)
		{
			this.deferredLoadAllowed = true;
			this.saved = true;
		}

		void IDataObjectComponent.MinimizeMemory()
		{
			this.audit.Drop(Breadcrumb.MinimizeMemory);
			if (Monitor.TryEnter(this.parent))
			{
				try
				{
					if (!((IDataObjectComponent)this).PendingDatabaseUpdates)
					{
						this.deferredLoadAllowed = true;
						this.CleanupEmailMessage();
						this.CleanupMimeDocument();
					}
					this.CleanupReadStream();
				}
				finally
				{
					Monitor.Exit(this.parent);
				}
			}
		}

		void IDataWithinRowComponent.LoadFromParentRow(DataTableCursor cursor)
		{
			this.ThrowIfReadOnly();
			this.audit.Drop(Breadcrumb.LoadFromParentRow);
			this.deferredLoadAllowed = true;
			this.saved = true;
		}

		void IDataWithinRowComponent.SaveToParentRow(DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			this.audit.Drop(Breadcrumb.SaveToParentRow);
			if (!((IDataObjectComponent)this).PendingDatabaseUpdates)
			{
				return;
			}
			if (!this.IsReadOnly)
			{
				this.PromoteHeaders();
			}
			long mimeStreamSize = this.Save(cursor, checkpointCallback);
			if (!this.IsReadOnly)
			{
				this.MimeStreamSize = mimeStreamSize;
			}
		}

		void IDataWithinRowComponent.Cleanup()
		{
			this.audit.Drop(Breadcrumb.Cleanup);
			lock (this.parent)
			{
				this.CleanupEmailMessage();
				this.CleanupMimeDocument();
				this.CleanupReadStream();
			}
		}

		public void SetMimeDocument(MimeDocument mimeDocument)
		{
			this.ThrowIfReadOnly();
			this.audit.Drop(Breadcrumb.SetMimeDocument);
			this.InternalSetMimeDocument(mimeDocument);
		}

		public void PromoteHeaders()
		{
			this.ThrowIfReadOnly();
			this.audit.Drop(Breadcrumb.PromoteHeaders);
			string subject = string.Empty;
			string internetMessageId = string.Empty;
			Guid? guid = null;
			RoutingAddress empty = RoutingAddress.Empty;
			string mimeFrom = string.Empty;
			string probeName = string.Empty;
			bool persistProbeTrace = false;
			if (this.RootPart != null)
			{
				HeaderList headers = this.RootPart.Headers;
				Header header = headers.FindFirst("Subject");
				if (header != null)
				{
					try
					{
						subject = header.Value;
					}
					catch (ExchangeDataException)
					{
						byte[] headerRawValue = MimeInternalHelpers.GetHeaderRawValue(header);
						subject = Encoding.ASCII.GetString(headerRawValue);
					}
				}
				header = headers.FindFirst("Message-ID");
				if (header != null)
				{
					internetMessageId = header.Value;
				}
				guid = new Guid?(MimeCache.GetNetworkMessageId(headers));
				header = headers.FindFirst(HeaderId.Sender);
				if (header != null)
				{
					MimeRecipient mimeRecipient = header.FirstChild as MimeRecipient;
					if (mimeRecipient != null)
					{
						empty = new RoutingAddress(mimeRecipient.Email);
						if (!empty.IsValid)
						{
							empty = RoutingAddress.Empty;
						}
					}
				}
				header = headers.FindFirst(HeaderId.From);
				if (header != null)
				{
					mimeFrom = MimeCache.FromToString(header);
					if (!empty.IsValid)
					{
						MimeRecipient mimeRecipient2 = header.FirstChild as MimeRecipient;
						if (mimeRecipient2 != null)
						{
							empty = new RoutingAddress(mimeRecipient2.Email);
							if (!empty.IsValid)
							{
								empty = RoutingAddress.Empty;
							}
						}
						else
						{
							MimeGroup mimeGroup = header.FirstChild as MimeGroup;
							if (mimeGroup != null)
							{
								mimeRecipient2 = (mimeGroup.FirstChild as MimeRecipient);
								if (mimeRecipient2 != null)
								{
									empty = new RoutingAddress(mimeRecipient2.Email);
									if (!empty.IsValid)
									{
										empty = RoutingAddress.Empty;
									}
								}
							}
						}
					}
				}
				if (this.parent.IsJournalReport && string.Compare(this.parent.FromAddress, "<>", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.parent.RetryDeliveryIfRejected = true;
				}
				bool flag;
				if (this.parent.TransportPropertiesHeader.TryGetBoolValue("MustDeliver", out flag) && flag)
				{
					this.parent.RetryDeliveryIfRejected = true;
				}
				bool flag2 = false;
				if (MimeCache.priorityHeaderPromotionEnabled)
				{
					Header priorityHeader = headers.FindFirst("X-MS-Exchange-Organization-Prioritization");
					DeliveryPriority value;
					string prioritizationReason;
					flag2 = Util.DecodePriorityHeader(priorityHeader, out value, out prioritizationReason);
					if (flag2)
					{
						this.parent.PrioritizationReason = prioritizationReason;
						this.parent.Priority = new DeliveryPriority?(value);
					}
				}
				string value2;
				if (!flag2 && this.parent.TransportPropertiesHeader.TryGetStringValue("DeliveryPriority", out value2))
				{
					DeliveryPriority value3;
					if (!EnumValidator<DeliveryPriority>.TryParse(value2, EnumParseOptions.IgnoreCase, out value3))
					{
						value3 = DeliveryPriority.Normal;
					}
					this.parent.Priority = new DeliveryPriority?(value3);
				}
				ContentTransferEncoding contentTransferEncoding = ContentTransferEncoding.Unknown;
				header = headers.FindFirst(HeaderId.ContentTransferEncoding);
				if (header != null)
				{
					contentTransferEncoding = this.RootPart.ContentTransferEncoding;
				}
				if (contentTransferEncoding == ContentTransferEncoding.Binary)
				{
					this.parent.BodyType = BodyType.BinaryMIME;
				}
				else if (contentTransferEncoding == ContentTransferEncoding.EightBit)
				{
					this.parent.BodyType = BodyType.EightBitMIME;
				}
				else if (contentTransferEncoding == ContentTransferEncoding.Unknown)
				{
					this.parent.BodyType = BodyType.Default;
				}
				else
				{
					this.parent.BodyType = BodyType.SevenBit;
				}
				int scl;
				if (MimeCache.TryGetSCLValue(headers, out scl))
				{
					this.parent.Scl = scl;
				}
				header = headers.FindFirst("X-MS-Exchange-Organization-Spam-Filter-Enumerated-Risk");
				if (header != null && !string.IsNullOrEmpty(header.Value))
				{
					RiskLevel riskLevel;
					if (!EnumValidator<RiskLevel>.TryParse(header.Value, EnumParseOptions.IgnoreCase, out riskLevel))
					{
						riskLevel = RiskLevel.Normal;
					}
					this.parent.RiskLevel = riskLevel;
				}
				header = headers.FindFirst("X-MS-Exchange-ActiveMonitoringProbeName");
				if (header != null && !string.IsNullOrEmpty(header.Value))
				{
					probeName = header.Value;
				}
				header = headers.FindFirst("X-Exchange-Persist-Probe-Trace");
				if (header != null)
				{
					bool.TryParse(header.Value, out persistProbeTrace);
				}
			}
			this.parent.Subject = subject;
			this.parent.InternetMessageId = internetMessageId;
			this.parent.MimeFrom = mimeFrom;
			this.parent.MimeSenderAddress = empty.ToString();
			this.parent.ProbeName = probeName;
			this.parent.PersistProbeTrace = persistProbeTrace;
			if (guid != null)
			{
				this.parent.NetworkMessageId = guid.Value;
			}
		}

		public void ResetMimeParserEohCallback()
		{
			MimeDocument mimeDocument = this.mimeDocument;
			if (mimeDocument != null)
			{
				mimeDocument.EndOfHeaders = null;
			}
		}

		public void CleanupMimeDocument()
		{
			if (this.mimeDocument != null)
			{
				this.mimeDocument.Dispose();
				this.mimeDocument = null;
			}
		}

		public void CleanupEmailMessage()
		{
			if (this.emailMessage != null)
			{
				this.emailMessage.Dispose();
				this.emailMessage = null;
			}
		}

		private static Guid GetNetworkMessageId(HeaderList headers)
		{
			Guid guid = Guid.Empty;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Network-Message-Id");
			if (header != null && Guid.TryParse(header.Value, out guid) && guid != Guid.Empty)
			{
				return guid;
			}
			guid = CombGuidGenerator.NewGuid();
			if (header != null)
			{
				header.Value = guid.ToString();
			}
			else
			{
				header = Header.Create("X-MS-Exchange-Organization-Network-Message-Id");
				header.Value = guid.ToString();
				headers.AppendChild(header);
			}
			return guid;
		}

		public void SetReadOnly(bool readOnly)
		{
			this.audit.Drop(Breadcrumb.SetReadOnly);
			if (readOnly)
			{
				if (this.mimeWriteStream != null)
				{
					if (this.mimeWriteStream.CanWrite)
					{
						throw new InvalidOperationException(Strings.MimeWriteStreamOpen);
					}
					this.mimeWriteStream = null;
				}
				if (this.MimeDocument.RootPart == null)
				{
					this.InitializeRootPart();
				}
				if (this.mimeReadStream != null && this.readStreamVersion != this.mimeDocument.Version)
				{
					this.CleanupReadStream();
				}
				this.parent.RefreshMimeSize();
			}
			if (this.mimeDocument != null)
			{
				if (readOnly && this.emailMessage == null)
				{
					this.emailMessage = EmailMessage.Create(this.mimeDocument);
				}
				if (this.emailMessage != null)
				{
					this.emailMessage.SetReadOnly(readOnly);
					return;
				}
				EmailMessage.SetDocumentReadOnly(this.mimeDocument, readOnly);
			}
		}

		private static bool TryGetSCLValue(HeaderList headers, out int scl)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-SCL");
			if (header != null)
			{
				string value;
				try
				{
					value = header.Value;
				}
				catch (ExchangeDataException)
				{
					scl = 0;
					return false;
				}
				if (int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out scl) && scl <= 9 && scl >= -1)
				{
					return true;
				}
			}
			scl = 0;
			return false;
		}

		private static string FromToString(Header header)
		{
			AddressHeader addressHeader = header as AddressHeader;
			if (addressHeader == null)
			{
				return null;
			}
			string result;
			using (MemoryStream memoryStream = new MemoryStream(512))
			{
				foreach (AddressItem addressItem in addressHeader)
				{
					if (memoryStream.Position > 512L)
					{
						break;
					}
					if (memoryStream.Position != 0L)
					{
						memoryStream.WriteByte(44);
						memoryStream.WriteByte(32);
					}
					addressItem.WriteTo(memoryStream);
				}
				result = ByteString.BytesToString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, true);
			}
			return result;
		}

		private static void ParseHeader(MimeDocument dom, HeaderId headerId)
		{
			MimePart rootPart = dom.RootPart;
			if (rootPart == null)
			{
				return;
			}
			HeaderList headers = rootPart.Headers;
			if (headers == null)
			{
				return;
			}
			AddressHeader addressHeader = headers.FindFirst(headerId) as AddressHeader;
			if (addressHeader == null)
			{
				return;
			}
			for (MimeNode mimeNode = addressHeader.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
			{
				MimeGroup mimeGroup = mimeNode as MimeGroup;
				if (mimeGroup != null)
				{
					for (MimeNode mimeNode2 = mimeGroup.FirstChild; mimeNode2 != null; mimeNode2 = mimeNode2.NextSibling)
					{
					}
				}
			}
		}

		private void InternalSetMimeDocument(MimeDocument newDocument)
		{
			if (this.mimeDocument != null)
			{
				this.ThrowIfReadOnly();
			}
			if (newDocument == this.mimeDocument)
			{
				return;
			}
			this.CleanupEmailMessage();
			this.CleanupMimeDocument();
			this.CleanupReadStream();
			this.persistedMimeVersion = int.MinValue;
			MimeCache.ParseHeader(newDocument, HeaderId.Sender);
			MimeCache.ParseHeader(newDocument, HeaderId.From);
			EmailMessage emailMessage = null;
			if (this.IsReadOnly)
			{
				emailMessage = EmailMessage.Create(newDocument);
				emailMessage.SetReadOnly(true);
			}
			else
			{
				EmailMessage.SetDocumentReadOnly(newDocument, false);
			}
			this.mimeDocument = newDocument;
			this.emailMessage = emailMessage;
		}

		private DecodingOptions GetMimeDecodingOptions()
		{
			if (this.DefaultEncoding != null)
			{
				return new DecodingOptions(DecodingFlags.AllEncodings | (this.fallbackToRawOverride ? DecodingFlags.FallbackToRaw : DecodingFlags.None), this.DefaultEncoding);
			}
			this.DefaultEncoding = DecodingOptions.Default.CharsetEncoding;
			DecodingOptions @default = DecodingOptions.Default;
			if (this.FallbackToRawOverride)
			{
				MimeInternalHelpers.SetDecodingOptionsDecodingFlags(ref @default, @default.DecodingFlags | DecodingFlags.FallbackToRaw);
			}
			return @default;
		}

		private void NewMimeDocument(MimeLimits mimeLimits)
		{
			this.ThrowIfReadOnly();
			DecodingOptions mimeDecodingOptions = this.GetMimeDecodingOptions();
			MimeDocument mimeDocument = new MimeDocument(mimeDecodingOptions, mimeLimits);
			this.SetMimeDocument(mimeDocument);
			this.persistedMimeVersion = int.MinValue;
			this.MimeStreamSize = 0L;
			this.PromoteHeaders();
		}

		private long Save(DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			if (!cursor.IsWithinTransaction)
			{
				throw new InvalidOperationException("No open Transaction");
			}
			Stream stream;
			CreateFixedStream createFixedStream;
			this.parent.OpenMimeDBWriter(cursor, this.saved, checkpointCallback, out stream, out createFixedStream);
			long result;
			if (!this.saved)
			{
				using (stream)
				{
					MimeCacheMap.Create(this.mimeDocument, stream, createFixedStream, new ReOpenFixedStream(DataColumn.ReopenAsLazyReader), MimeCacheMap.SmallMessageSizeThreshold, out result);
					goto IL_D5;
				}
			}
			Serialized serialized;
			using (stream)
			{
				serialized = MimeCacheMap.Update(this.mimeDocument, stream, createFixedStream, new ReOpenFixedStream(DataColumn.ReopenAsLazyReader), MimeCacheMap.SmallMessageSizeThreshold, out result);
			}
			if (serialized == Serialized.NonSequential && !this.parent.MimeNotSequential)
			{
				this.parent.MimeNotSequential = true;
			}
			else if (serialized == Serialized.Sequential && this.parent.MimeNotSequential)
			{
				this.parent.MimeNotSequential = false;
			}
			IL_D5:
			this.saved = true;
			this.persistedMimeVersion = this.mimeDocument.Version;
			return result;
		}

		private void DeferredLoad()
		{
			DecodingOptions mimeDecodingOptions = this.GetMimeDecodingOptions();
			this.InternalSetMimeDocument(this.parent.LoadMimeFromDB(mimeDecodingOptions));
			this.persistedMimeVersion = this.mimeDocument.Version;
		}

		private void CleanupReadStream()
		{
			if (this.mimeReadStream != null)
			{
				this.mimeReadStream.Close();
				this.mimeReadStream = null;
				this.readStreamVersion = int.MinValue;
			}
		}

		private void InitializeRootPart()
		{
			this.MimeDocument.RootPart = new MimePart();
			this.MimeDocument.RootPart.Headers.AppendChild(new AsciiTextHeader("MIME-Version", "1.0"));
			this.MimeDocument.RootPart.Headers.AppendChild(new ContentTypeHeader("text/plain"));
		}

		private void ThrowIfReadOnly()
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException("This MimeCache operation cannot be performed in read-only mode.");
			}
		}

		private const int MimeNotPersisted = -2147483648;

		private static bool priorityHeaderPromotionEnabled;

		private IMailItemStorage parent;

		private volatile MimeDocument mimeDocument;

		private volatile EmailMessage emailMessage;

		private volatile Stream mimeReadStream;

		private Stream mimeWriteStream;

		private int readStreamVersion = int.MinValue;

		private bool deferredLoadAllowed;

		private bool saved;

		private bool canRestore;

		private bool fallbackToRawOverride;

		private int persistedMimeVersion = int.MinValue;

		private Encoding defaultEncoding;

		private Breadcrumbs audit = new Breadcrumbs(8);
	}
}
