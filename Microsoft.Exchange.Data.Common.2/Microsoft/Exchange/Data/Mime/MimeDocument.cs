using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeDocument : IDisposable
	{
		public MimeDocument() : this(DecodingOptions.Default, MimeLimits.Default)
		{
		}

		public MimeDocument(DecodingOptions headerDecodingOptions, MimeLimits mimeLimits)
		{
			if (mimeLimits == null)
			{
				throw new ArgumentNullException("mimeLimits");
			}
			this.decodingOptions = headerDecodingOptions;
			this.limits = mimeLimits;
			this.accessToken = new MimeDocument.MimeDocumentThreadAccessToken(this);
		}

		public MimeDocument.EndOfHeadersCallback EndOfHeaders
		{
			get
			{
				this.ThrowIfDisposed();
				MimeDocument.EndOfHeadersCallback result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.eohCallback;
				}
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.eohCallback = value;
				}
			}
		}

		public MimePart RootPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.root;
				}
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (value == null)
					{
						throw new ArgumentNullException("value");
					}
					if (value.Parent != null)
					{
						throw new ArgumentException(Strings.RootPartCantHaveAParent);
					}
					this.ThrowIfReadOnly("MimeDocument.set_RootPart");
					if (this.reader != null)
					{
						throw new InvalidOperationException("Cannot set a new document root part while document loading is not complete");
					}
					this.lastPart = null;
					this.contentStart = 0L;
					this.complianceStatus = MimeComplianceStatus.Compliant;
					this.stopLoading = false;
					if (this.root != null)
					{
						this.root.ParentDocument = null;
					}
					this.root = value;
					this.root.ParentDocument = this;
					this.parsedSize = 0L;
					this.IncrementVersion();
				}
			}
		}

		public DecodingOptions HeaderDecodingOptions
		{
			get
			{
				this.ThrowIfDisposed();
				DecodingOptions result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.decodingOptions;
				}
				return result;
			}
			internal set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("MimeDocument.set_HeaderDecodingOptions");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.decodingOptions = value;
				}
			}
		}

		public MimeLimits MimeLimits
		{
			get
			{
				this.ThrowIfDisposed();
				MimeLimits result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.limits;
				}
				return result;
			}
		}

		public MimeComplianceMode ComplianceMode
		{
			get
			{
				this.ThrowIfDisposed();
				MimeComplianceMode result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.complianceMode;
				}
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("MimeDocument.set_ComplianceMode");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.complianceMode = value;
				}
			}
		}

		public MimeComplianceStatus ComplianceStatus
		{
			get
			{
				this.ThrowIfDisposed();
				MimeComplianceStatus result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.complianceStatus;
				}
				return result;
			}
		}

		public bool RequiresSMTPUTF8
		{
			get
			{
				this.ThrowIfDisposed();
				bool requiresSMTPUTF;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					requiresSMTPUTF = this.root.RequiresSMTPUTF8;
				}
				return requiresSMTPUTF;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		internal static bool FixMimeForTestUseOnly
		{
			get
			{
				return MimeDocument.fixMime;
			}
			set
			{
				MimeDocument.fixMime = value;
			}
		}

		internal ObjectThreadAccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		internal DecodingOptions EffectiveHeaderDecodingOptions
		{
			get
			{
				this.ThrowIfDisposed();
				DecodingOptions result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					DecodingOptions decodingOptions = this.decodingOptions;
					Charset charset = this.GetMimeTreeCharset();
					if (charset != null)
					{
						decodingOptions.Charset = charset;
					}
					result = decodingOptions;
				}
				return result;
			}
		}

		internal EncodingOptions EncodingOptions
		{
			get
			{
				this.ThrowIfDisposed();
				EncodingOptions result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.encodingOptions == null)
					{
						this.encodingOptions = new EncodingOptions(this.GetMimeTreeCharset());
					}
					result = this.encodingOptions;
				}
				return result;
			}
		}

		internal bool CreateValidateStorage
		{
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.createValidateStorage = value;
				}
			}
		}

		internal long Position
		{
			get
			{
				long streamOffset;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					streamOffset = this.reader.StreamOffset;
				}
				return streamOffset;
			}
		}

		internal long ParsedSize
		{
			get
			{
				long result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.parsedSize;
				}
				return result;
			}
		}

		private bool CreateDomObjects
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPrivate(this.accessToken))
				{
					result = (this.loadEmbeddedMessages || this.lastPart == null || !this.lastPart.IsEmbeddedMessage);
				}
				return result;
			}
		}

		public MimePart Load(Stream stream, CachingMode cachingMode)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("MimeDocument.Load");
			MimePart rootPart;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				if (this.root != null)
				{
					throw new InvalidOperationException(Strings.CannotLoadIntoNonEmptyDocument);
				}
				if (this.reader != null)
				{
					throw new InvalidOperationException("Cannot load document again while previous load is not complete");
				}
				switch (cachingMode)
				{
				case CachingMode.Copy:
				{
					this.InitializePushMode(true);
					bool discard = true;
					try
					{
						byte[] array = new byte[4096];
						while (!this.stopLoading)
						{
							int num = stream.Read(array, 0, array.Length);
							if (num == 0)
							{
								break;
							}
							this.Write(array, 0, num);
						}
						discard = false;
						goto IL_17B;
					}
					finally
					{
						this.Flush(discard);
					}
					break;
				}
				case CachingMode.Source:
				case CachingMode.SourceTakeOwnership:
					if (this.createValidateStorage)
					{
						if (!stream.CanSeek)
						{
							throw new NotSupportedException(Strings.CachingModeSourceButStreamCannotSeek);
						}
						stream.Position = 0L;
						this.backingStorage = new ReadableDataStorageOnStream(stream, cachingMode == CachingMode.SourceTakeOwnership);
					}
					this.reader = new MimeReader(stream, true, this.decodingOptions, this.limits, true, true, this.expectBinaryContent);
					this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
					try
					{
						this.BuildDom(null, 0, 0, true);
					}
					finally
					{
						this.reader.DisconnectInputStream();
					}
					this.parsedSize = this.reader.StreamOffset;
					this.reader.Dispose();
					this.reader = null;
					if (this.backingStorage != null)
					{
						this.backingStorage.Release();
						this.backingStorage = null;
						goto IL_17B;
					}
					goto IL_17B;
				}
				throw new ArgumentException("Invalid Caching Mode value", "cachingMode");
				IL_17B:
				rootPart = this.RootPart;
			}
			return rootPart;
		}

		public Stream GetLoadStream()
		{
			return this.GetLoadStream(true);
		}

		public MimeDocument Clone()
		{
			this.ThrowIfDisposed();
			MimeDocument result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.reader != null)
				{
					throw new NotSupportedException(Strings.DocumentCloneNotSupportedInThisState);
				}
				MimeDocument mimeDocument = (MimeDocument)base.MemberwiseClone();
				if (this.root != null)
				{
					mimeDocument.root = (MimePart)this.root.Clone();
					mimeDocument.root.ParentDocument = mimeDocument;
				}
				mimeDocument.contentPositionStack = null;
				mimeDocument.lastPart = null;
				result = mimeDocument;
			}
			return result;
		}

		public long WriteTo(Stream stream)
		{
			this.ThrowIfDisposed();
			long result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (Stream.Null != stream || this.cachedSizeVersion != this.version)
				{
					this.cachedSizeVersion = this.version;
					this.cachedSize = ((this.root == null) ? 0L : this.root.WriteTo(stream, this.EncodingOptions, null));
				}
				result = this.cachedSize;
			}
			return result;
		}

		public long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter)
		{
			this.ThrowIfDisposed();
			long result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.root != null)
				{
					if (encodingOptions == null)
					{
						encodingOptions = this.EncodingOptions;
					}
					result = this.root.WriteTo(stream, encodingOptions, filter);
				}
				else
				{
					result = 0L;
				}
			}
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void DangerousSetFixBadMimeBoundary(bool value)
		{
			if (this.reader != null)
			{
				throw new InvalidOperationException("Cannot change FixBadMimeBoundary flag while previous load is not complete");
			}
			this.dangerousFixBadMimeBoundary = value;
		}

		internal Stream GetLoadStream(bool expectBinaryContent)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("MimeDocument.GetLoadStream");
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.root != null)
				{
					throw new InvalidOperationException(Strings.CannotLoadIntoNonEmptyDocument);
				}
				if (this.reader != null)
				{
					throw new InvalidOperationException(Strings.CannotGetLoadStreamMoreThanOnce);
				}
				this.expectBinaryContent = expectBinaryContent;
				this.InitializePushMode(this.expectBinaryContent);
				result = new MimeDocument.PushStream(this);
			}
			return result;
		}

		internal void SetReadOnly(bool makeReadOnly)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (makeReadOnly != this.isReadOnly)
				{
					if (makeReadOnly)
					{
						this.CompleteParse();
					}
					this.SetReadOnlyInternal(makeReadOnly);
				}
			}
		}

		internal void CompleteParse()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.BuildDomAndCompleteParse(this.RootPart);
				EncodingOptions encodingOptions = this.EncodingOptions;
				this.GetMimeTreeCharset();
			}
		}

		internal void SetReadOnlyInternal(bool makeReadOnly)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.isReadOnly = makeReadOnly;
				using (MimePart.SubtreeEnumerator enumerator = this.root.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, false))
				{
					while (enumerator.MoveNext())
					{
						MimePart mimePart = enumerator.Current;
						mimePart.SetReadOnlyInternal(makeReadOnly);
					}
				}
			}
		}

		internal void IncrementVersion()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.version = ((int.MaxValue == this.version) ? 1 : (this.version + 1));
			}
		}

		internal void BuildEmbeddedDom(MimePart part)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("MimeDocument.BuildEmbeddedDom");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.reader == null && !this.stopLoading)
				{
					MimePart mimePart = this.root;
					bool flag = this.loadEmbeddedMessages;
					MimeDocument.EndOfHeadersCallback endOfHeadersCallback = this.eohCallback;
					this.eohCallback = null;
					this.loadEmbeddedMessages = true;
					this.reader = new MimeReader(null, true, this.decodingOptions, this.limits, true, true, this.expectBinaryContent);
					this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
					try
					{
						using (MimePart.SubtreeEnumerator enumerator = part.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, true))
						{
							while (enumerator.MoveNext())
							{
								MimePart mimePart2 = enumerator.Current;
								if (mimePart2.InternalLastChild == null && mimePart2.Storage != null && mimePart2.IsEmbeddedMessage)
								{
									this.ParseOnePart(mimePart2);
									this.root.ParentDocument = null;
									mimePart2.InternalInsertAfter(this.root, null);
								}
							}
						}
					}
					finally
					{
						this.reader.DisconnectInputStream();
						this.reader.Dispose();
						this.reader = null;
						if (this.backingStorage != null)
						{
							this.backingStorage.Release();
							this.backingStorage = null;
						}
						this.backingStorageOffset = 0L;
						this.root = mimePart;
						this.loadEmbeddedMessages = flag;
						this.eohCallback = endOfHeadersCallback;
					}
				}
			}
		}

		internal void BuildDomAndCompleteParse(MimePart rootPart)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.reader != null)
				{
					throw new InvalidOperationException("do not call BuildDomAndCompleteParse() before Load is complete");
				}
				if (this.stopLoading)
				{
					throw new InvalidOperationException("do not call BuildDomAndCompleteParse() after canceling Load");
				}
				if (rootPart.InternalLastChild == null && rootPart.Storage == null)
				{
					this.ParseAllHeaders(rootPart);
				}
				else
				{
					MimePart mimePart = this.root;
					bool flag = this.loadEmbeddedMessages;
					bool flag2 = this.parseCompletely;
					MimeDocument.EndOfHeadersCallback endOfHeadersCallback = this.eohCallback;
					this.eohCallback = null;
					this.loadEmbeddedMessages = true;
					this.parseCompletely = true;
					this.reader = new MimeReader(null, true, this.decodingOptions, this.limits, true, true, this.expectBinaryContent);
					this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
					try
					{
						Stack<MimePart> stack = new Stack<MimePart>(5);
						stack.Push(rootPart);
						while (stack.Count > 0)
						{
							MimePart mimePart2 = stack.Pop();
							do
							{
								MimeNode internalLastChild = mimePart2.InternalLastChild;
								this.ParseAllHeaders(mimePart2);
								MimePart mimePart3 = mimePart2.FirstChild as MimePart;
								if (mimePart3 != null)
								{
									stack.Push(mimePart3);
								}
								mimePart2 = (mimePart2.NextSibling as MimePart);
							}
							while (mimePart2 != null);
						}
					}
					finally
					{
						this.reader.DisconnectInputStream();
						this.reader.Dispose();
						this.reader = null;
						if (this.backingStorage != null)
						{
							this.backingStorage.Release();
							this.backingStorage = null;
						}
						this.backingStorageOffset = 0L;
						this.root = mimePart;
						this.loadEmbeddedMessages = flag;
						this.parseCompletely = flag2;
						this.eohCallback = endOfHeadersCallback;
					}
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				if (this.backingStorageWriteStream != null)
				{
					this.backingStorageWriteStream.Dispose();
					this.backingStorageWriteStream = null;
				}
				if (this.backingStorage != null)
				{
					this.backingStorage.Release();
					this.backingStorage = null;
				}
				if (this.reader != null)
				{
					this.reader.Dispose();
					this.reader = null;
				}
				if (this.root != null)
				{
					this.root.Dispose();
					this.root = null;
				}
			}
			this.isDisposed = true;
		}

		private static bool IsContentBinary(Stream stream, int bytesToExamine, int thresholdPercentage)
		{
			int i = 0;
			int num = 0;
			byte[] array = new byte[bytesToExamine];
			while (i < array.Length)
			{
				int num2 = stream.Read(array, i, array.Length - i);
				if (num2 == 0)
				{
					break;
				}
				i += num2;
			}
			if (i < 1)
			{
				return false;
			}
			for (int j = 0; j < i; j++)
			{
				if ((array[j] & 128) != 0)
				{
					num++;
				}
			}
			int num3 = num * 100 / i;
			return num3 >= thresholdPercentage;
		}

		private Charset GetMimeTreeCharset()
		{
			this.ThrowIfDisposed();
			Charset result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.searchMimeTreeCharset)
				{
					this.searchMimeTreeCharset = false;
					if (this.root != null)
					{
						this.mimeTreeCharset = this.root.FindMimeTreeCharset();
					}
				}
				result = this.mimeTreeCharset;
			}
			return result;
		}

		private void ParseOnePart(MimePart nextPart)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.root = null;
				try
				{
					this.backingStorage = nextPart.Storage;
					this.backingStorage.AddRef();
					this.backingStorageOffset = nextPart.DataStart + nextPart.BodyOffset;
					Stream rawContentReadStream;
					Stream stream = rawContentReadStream = nextPart.GetRawContentReadStream();
					try
					{
						this.reader.Reset(stream);
						this.BuildDom(null, 0, 0, true, nextPart.IsEmbeddedMessage);
					}
					finally
					{
						if (rawContentReadStream != null)
						{
							((IDisposable)rawContentReadStream).Dispose();
						}
					}
				}
				finally
				{
					this.backingStorage.Release();
					this.backingStorage = null;
				}
			}
		}

		private void Flush(bool discard)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("MimeDocument.Flush");
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.reader != null)
				{
					if (!discard)
					{
						this.backingStorageWriteStream.Flush();
						if (!this.stopLoading)
						{
							this.BuildDom(null, 0, 0, true);
						}
					}
					this.parsedSize = this.reader.StreamOffset;
					this.reader.Dispose();
					this.reader = null;
					this.backingStorageWriteStream.Dispose();
					this.backingStorageWriteStream = null;
					this.backingStorage.Release();
					this.backingStorage = null;
				}
			}
		}

		private void Write(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("MimeDocument.Write");
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.reader == null)
				{
					throw new InvalidOperationException(Strings.CannotWriteAfterFlush);
				}
				if (count != 0)
				{
					this.backingStorageWriteStream.Write(buffer, offset, count);
					if (!this.stopLoading)
					{
						this.BuildDom(buffer, offset, count, false);
					}
				}
			}
		}

		private void InitializePushMode(bool expectBinaryContent)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
				this.backingStorage = temporaryDataStorage;
				this.backingStorageWriteStream = temporaryDataStorage.OpenWriteStream(true);
				this.reader = new MimeReader(null, true, this.decodingOptions, this.limits, true, true, expectBinaryContent);
				this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
			}
		}

		private void BuildDom(byte[] buffer, int offset, int length, bool eof)
		{
			this.BuildDom(buffer, offset, length, eof, false);
		}

		private void BuildDom(byte[] buffer, int offset, int length, bool eof, bool parseHeaders)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				while (!this.reader.EndOfFile && (!this.reader.DataExhausted || length != 0 || eof) && !this.stopLoading)
				{
					if (this.reader.DataExhausted)
					{
						int num = this.reader.AddMoreData(buffer, offset, length, eof);
						offset += num;
						length -= num;
					}
					bool flag = this.reader.TryReachNextState();
					if (flag)
					{
						MimeComplianceStatus mimeComplianceStatus = this.reader.ComplianceStatus;
						MimeReaderState readerState = this.reader.ReaderState;
						if (readerState <= MimeReaderState.PartBody)
						{
							if (readerState <= MimeReaderState.HeaderComplete)
							{
								switch (readerState)
								{
								case MimeReaderState.PartStart:
									this.StartPart(false);
									goto IL_191;
								case MimeReaderState.Start | MimeReaderState.PartStart:
									goto IL_186;
								case MimeReaderState.HeaderStart:
									if (!this.reader.TryCompleteCurrentHeader(this.embeddedMessagePartDepth == 0 || this.CreateDomObjects))
									{
										goto IL_191;
									}
									break;
								default:
									if (readerState != MimeReaderState.HeaderComplete)
									{
										goto IL_186;
									}
									break;
								}
								if (this.embeddedMessagePartDepth == 0 || this.CreateDomObjects)
								{
									Header currentHeaderObject = this.reader.CurrentHeaderObject;
									if (currentHeaderObject != null)
									{
										this.lastPart.Headers.InternalAppendChild(currentHeaderObject);
									}
								}
							}
							else if (readerState != MimeReaderState.EndOfHeaders)
							{
								if (readerState != MimeReaderState.PartBody)
								{
									goto IL_186;
								}
							}
							else
							{
								this.EndPartHeaders();
							}
						}
						else if (readerState <= MimeReaderState.InlineStart)
						{
							if (readerState != MimeReaderState.PartEnd)
							{
								if (readerState != MimeReaderState.InlineStart)
								{
									goto IL_186;
								}
								this.StartPart(true);
							}
							else
							{
								mimeComplianceStatus = this.CompletePart(false, parseHeaders);
							}
						}
						else if (readerState != MimeReaderState.InlineBody)
						{
							if (readerState != MimeReaderState.InlineEnd)
							{
								if (readerState != MimeReaderState.End)
								{
									goto IL_186;
								}
							}
							else
							{
								mimeComplianceStatus = this.CompletePart(true, parseHeaders);
							}
						}
						IL_191:
						if (this.reader.ReaderState == MimeReaderState.PartBody)
						{
							continue;
						}
						this.complianceStatus |= mimeComplianceStatus;
						if (this.ComplianceMode == MimeComplianceMode.Strict && this.ComplianceStatus != MimeComplianceStatus.Compliant)
						{
							throw new MimeException(Strings.StrictComplianceViolation);
						}
						continue;
						IL_186:
						throw new InvalidOperationException("unexpected reader state");
					}
				}
			}
		}

		private void StartPart(bool inline)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.CreateDomObjects)
				{
					MimePart mimePart = new MimePart();
					if (this.root == null)
					{
						this.root = mimePart;
						mimePart.ParentDocument = this;
					}
					else
					{
						if (inline)
						{
							Header header;
							Header header2;
							if (!this.root.IsMultipart)
							{
								MimePart mimePart2 = (this.lastPart == null) ? this.root : ((MimePart)this.lastPart.FirstChild);
								MimePart mimePart3 = new MimePart();
								header = mimePart2.Headers.FindFirst(HeaderId.ContentType);
								header2 = mimePart2.Headers.FindFirst(HeaderId.ContentTransferEncoding);
								if (header != null)
								{
									mimePart2.Headers.InternalRemoveChild(header);
									mimePart3.Headers.InternalAppendChild(header);
								}
								if (header2 != null)
								{
									mimePart2.Headers.InternalRemoveChild(header2);
									mimePart3.Headers.InternalAppendChild(header2);
								}
								Header header3 = new AsciiTextHeader("X-ConvertedToMime", HeaderId.Unknown);
								header3.RawValue = MimeString.ConvertedToMimeUU;
								mimePart2.Headers.InternalAppendChild(header3);
								DataStorage storage = mimePart2.Storage;
								mimePart3.SetStorageImpl(storage, mimePart2.DataStart + mimePart2.BodyOffset, mimePart2.DataEnd, 0L, mimePart2.BodyCte, mimePart2.BodyLineTermination);
								mimePart2.SetStorageImpl(null, 0L, 0L);
								header = new ContentTypeHeader("multipart/mixed");
								mimePart2.Headers.InternalInsertAfter(header, null);
								mimePart2.InternalAppendChild(mimePart3);
								this.lastPart = mimePart2;
								if (this.eohCallback != null)
								{
									bool flag;
									this.eohCallback(mimePart3, out flag);
									if (flag)
									{
										this.stopLoading = true;
									}
								}
							}
							string value = this.reader.InlineFileName;
							if (string.IsNullOrEmpty(value))
							{
								value = "unnamed.dat";
							}
							header2 = Header.Create(HeaderId.ContentTransferEncoding);
							header2.RawValue = MimeString.Uuencode;
							mimePart.Headers.InternalAppendChild(header2);
							Header header4 = new ContentDispositionHeader("attachment");
							MimeParameter newChild = new MimeParameter("filename", value);
							header4.InternalAppendChild(newChild);
							mimePart.Headers.InternalAppendChild(header4);
							header = new ContentTypeHeader("application/octet-stream");
							newChild = new MimeParameter("name", value);
							header.InternalAppendChild(newChild);
							mimePart.Headers.InternalAppendChild(header);
							if (this.eohCallback != null)
							{
								bool flag2;
								this.eohCallback(mimePart, out flag2);
								if (flag2)
								{
									this.stopLoading = true;
								}
							}
						}
						this.lastPart.InternalInsertAfter(mimePart, this.lastPart.InternalLastChild);
					}
					if (this.contentPositionStack == null)
					{
						this.contentPositionStack = new MimeDocument.ContentPositionEntry[4];
					}
					else if (this.contentPositionStack.Length == this.contentPositionStackTop)
					{
						MimeDocument.ContentPositionEntry[] destinationArray = new MimeDocument.ContentPositionEntry[this.contentPositionStack.Length * 2];
						Array.Copy(this.contentPositionStack, 0, destinationArray, 0, this.contentPositionStackTop);
						this.contentPositionStack = destinationArray;
					}
					this.contentPositionStack[this.contentPositionStackTop++] = new MimeDocument.ContentPositionEntry(this.contentStart, this.headersEnd, this.contentTransferEncoding);
					this.lastPart = mimePart;
					this.contentStart = (this.headersEnd = this.reader.StreamOffset);
					this.contentTransferEncoding = (inline ? this.reader.ContentTransferEncoding : ContentTransferEncoding.Unknown);
				}
				else if (this.embeddedMessagePartDepth == 0)
				{
					this.embeddedMessagePartDepth = this.reader.Depth;
				}
				this.complianceStatus |= this.reader.ComplianceStatus;
				this.reader.ResetComplianceStatus();
			}
		}

		private void EndPartHeaders()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.complianceStatus |= this.reader.ComplianceStatus;
				this.reader.ResetComplianceStatus();
				if (this.embeddedMessagePartDepth == 0 || this.CreateDomObjects)
				{
					this.headersEnd = this.reader.StreamOffset;
					this.contentTransferEncoding = this.reader.ContentTransferEncoding;
					if (MimeDocument.fixMime)
					{
						if ((this.lastPart == this.root || ((MimePart)this.lastPart.Parent).IsEmbeddedMessage) && this.lastPart.Headers.FindFirst(HeaderId.MimeVersion) == null)
						{
							Header header = Header.Create(HeaderId.MimeVersion);
							header.RawValue = MimeString.Version1;
							this.lastPart.Headers.InternalAppendChild(header);
						}
						if (this.lastPart.Headers.FindFirst(HeaderId.ContentType) == null)
						{
							bool flag = false;
							MimePart mimePart = this.lastPart.Parent as MimePart;
							if (mimePart != null && mimePart.Headers.FindFirst(HeaderId.ContentType).Value == "multipart/digest")
							{
								flag = true;
							}
							ContentTypeHeader newChild = new ContentTypeHeader(flag ? "message/rfc822" : "text/plain");
							this.lastPart.Headers.InternalAppendChild(newChild);
						}
					}
					if (this.parseCompletely)
					{
						this.ParseAllHeaders(this.lastPart);
					}
					if (this.eohCallback != null)
					{
						bool flag2;
						this.eohCallback(this.lastPart, out flag2);
						if (flag2)
						{
							this.stopLoading = true;
						}
					}
				}
			}
		}

		private void ParseAllHeaders(MimePart part)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				foreach (Header header in part.Headers)
				{
					header.ForceParse();
				}
			}
		}

		private MimeComplianceStatus CompletePart(bool inline, bool parseHeaders)
		{
			MimeComplianceStatus result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimeComplianceStatus mimeComplianceStatus = this.reader.ComplianceStatus;
				if (this.embeddedMessagePartDepth == 0 || this.CreateDomObjects)
				{
					if (this.createValidateStorage)
					{
						if (parseHeaders)
						{
							this.ParseAllHeaders(this.lastPart);
						}
						this.lastPart.SetStorageImpl(this.backingStorage, this.contentStart + this.backingStorageOffset, this.reader.StreamOffset + this.backingStorageOffset, this.headersEnd - this.contentStart, this.contentTransferEncoding, this.reader.LineTerminationState);
						MimeComplianceStatus mimeComplianceStatus2 = MimeComplianceStatus.InvalidWrapping | MimeComplianceStatus.BareLinefeedInBody | MimeComplianceStatus.UnexpectedBinaryContent;
						if (MimeDocument.fixMime && (mimeComplianceStatus2 & mimeComplianceStatus) != MimeComplianceStatus.Compliant && this.FixPartContent())
						{
							mimeComplianceStatus &= ~mimeComplianceStatus2;
						}
					}
					ContentTypeHeader contentTypeHeader = this.lastPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					if (contentTypeHeader != null && contentTypeHeader.IsMultipart && !this.lastPart.HasChildren)
					{
						contentTypeHeader.RawValue = MimeString.TextPlain;
					}
					this.lastPart = (this.lastPart.Parent as MimePart);
					this.contentPositionStackTop--;
					this.contentStart = this.contentPositionStack[this.contentPositionStackTop].ContentStart;
					this.headersEnd = this.contentPositionStack[this.contentPositionStackTop].HeadersEnd;
					this.contentTransferEncoding = this.contentPositionStack[this.contentPositionStackTop].ContentTransferEncoding;
				}
				if (0 < this.embeddedMessagePartDepth && this.embeddedMessagePartDepth == this.reader.Depth)
				{
					this.embeddedMessagePartDepth = 0;
				}
				this.reader.ResetComplianceStatus();
				result = mimeComplianceStatus;
			}
			return result;
		}

		private bool FixPartContent()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				ContentTypeHeader contentTypeHeader = this.lastPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
				if (contentTypeHeader.IsMultipart || contentTypeHeader.MediaType == "message")
				{
					result = false;
				}
				else if (this.lastPart.BodyCte == ContentTransferEncoding.Unknown)
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.lastPart;
					for (;;)
					{
						MimePart mimePart2 = mimePart.Parent as MimePart;
						if (mimePart2 != null)
						{
							if (mimePart == mimePart2.FirstChild)
							{
								ContentTypeHeader contentTypeHeader2 = mimePart2.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
								if (contentTypeHeader2 != null && contentTypeHeader2.Value == "multipart/signed")
								{
									break;
								}
							}
							Header header = mimePart2.Headers.FindFirst("DKIM-Signature");
							if (header != null)
							{
								goto Block_9;
							}
						}
						mimePart = mimePart2;
						if (mimePart == null)
						{
							goto Block_10;
						}
					}
					return false;
					Block_9:
					return false;
					Block_10:
					Header header2 = this.lastPart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
					if (header2 == null)
					{
						header2 = Header.Create(HeaderId.ContentTransferEncoding);
						this.lastPart.Headers.InternalAppendChild(header2);
						header2.RawValue = MimeString.QuotedPrintable;
						result = true;
					}
					else
					{
						ContentTransferEncoding encodingType = MimePart.GetEncodingType(header2.FirstRawToken);
						switch (encodingType)
						{
						case ContentTransferEncoding.Unknown:
						case ContentTransferEncoding.SevenBit:
						case ContentTransferEncoding.EightBit:
							header2.RawValue = MimeString.QuotedPrintable;
							return true;
						case ContentTransferEncoding.QuotedPrintable:
							this.ForceReencoding(encodingType);
							return true;
						case ContentTransferEncoding.Base64:
						{
							bool flag = false;
							if ((this.reader.ComplianceStatus & MimeComplianceStatus.UnexpectedBinaryContent) != MimeComplianceStatus.Compliant)
							{
								long num = this.lastPart.DataStart + this.lastPart.BodyOffset;
								long num2 = Math.Min(this.lastPart.DataEnd - num, 1000L);
								long end = num + num2;
								if (num2 > 10L)
								{
									using (Stream stream = this.lastPart.Storage.OpenReadStream(num, end))
									{
										flag = MimeDocument.IsContentBinary(stream, (int)num2, 10);
									}
								}
							}
							if (flag)
							{
								this.RepairBrokenExchangeMime(encodingType);
							}
							else
							{
								this.ForceReencoding(encodingType);
							}
							return true;
						}
						}
						result = false;
					}
				}
			}
			return result;
		}

		private void RepairBrokenExchangeMime(ContentTransferEncoding encoding)
		{
			MimeDocument.EncodingDataStorage encodingDataStorage = new MimeDocument.EncodingDataStorage(this.lastPart.Storage, this.lastPart.DataStart + this.lastPart.BodyOffset, this.lastPart.DataEnd, encoding);
			this.lastPart.SetStorageImpl(encodingDataStorage, 0L, long.MaxValue, 0L, encoding, LineTerminationState.CRLF);
			encodingDataStorage.Release();
		}

		private void ForceReencoding(ContentTransferEncoding encoding)
		{
			MimeDocument.DecodingDataStorage decodingDataStorage = new MimeDocument.DecodingDataStorage(this.lastPart.Storage, this.lastPart.DataStart + this.lastPart.BodyOffset, this.lastPart.DataEnd, encoding);
			this.lastPart.SetStorageImpl(decodingDataStorage, 0L, long.MaxValue, 0L, ContentTransferEncoding.Binary, LineTerminationState.CRLF);
			decodingDataStorage.Release();
		}

		private void ThrowIfDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("MimeDocument");
			}
		}

		private void ThrowIfReadOnly(string method)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyMimeException(method);
			}
		}

		private static bool fixMime = true;

		private bool dangerousFixBadMimeBoundary = true;

		private MimeDocument.MimeDocumentThreadAccessToken accessToken;

		private DataStorage backingStorage;

		private long backingStorageOffset;

		private Stream backingStorageWriteStream;

		private MimePart root;

		private MimeComplianceMode complianceMode;

		private MimeComplianceStatus complianceStatus;

		private DecodingOptions decodingOptions = DecodingOptions.Default;

		private EncodingOptions encodingOptions;

		private Charset mimeTreeCharset;

		private bool searchMimeTreeCharset = true;

		private MimeReader reader;

		private MimeLimits limits;

		private long contentStart;

		private long headersEnd;

		private ContentTransferEncoding contentTransferEncoding;

		private MimeDocument.ContentPositionEntry[] contentPositionStack;

		private int contentPositionStackTop;

		private MimePart lastPart;

		private long parsedSize;

		private MimeDocument.EndOfHeadersCallback eohCallback;

		private int version;

		private bool createValidateStorage = true;

		private bool stopLoading;

		private bool loadEmbeddedMessages;

		private bool isDisposed;

		private bool isReadOnly;

		private bool parseCompletely;

		private int embeddedMessagePartDepth;

		private long cachedSize;

		private int cachedSizeVersion = -1;

		private bool expectBinaryContent;

		public delegate void EndOfHeadersCallback(MimePart part, out bool stopLoading);

		private struct ContentPositionEntry
		{
			public ContentPositionEntry(long contentStart, long headersEnd, ContentTransferEncoding contentTransferEncoding)
			{
				this.ContentStart = contentStart;
				this.HeadersEnd = headersEnd;
				this.ContentTransferEncoding = contentTransferEncoding;
			}

			public long ContentStart;

			public long HeadersEnd;

			public ContentTransferEncoding ContentTransferEncoding;
		}

		private class MimeDocumentThreadAccessToken : ObjectThreadAccessToken
		{
			internal MimeDocumentThreadAccessToken(MimeDocument parent)
			{
			}
		}

		private class PushStream : Stream
		{
			public PushStream(MimeDocument document)
			{
				this.document = document;
			}

			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return this.document != null;
				}
			}

			public override long Length
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public override long Position
			{
				get
				{
					throw new NotSupportedException();
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public override void Flush()
			{
				if (this.document == null)
				{
					throw new ObjectDisposedException("stream");
				}
				using (ThreadAccessGuard.EnterPublic(this.document.AccessToken))
				{
					if (!this.badState)
					{
						if (this.document.stopLoading)
						{
							throw new InvalidOperationException(Strings.LoadingStopped);
						}
						this.badState = true;
						this.document.Flush(false);
						this.badState = false;
					}
				}
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long length)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (this.document == null)
				{
					throw new ObjectDisposedException("stream");
				}
				using (ThreadAccessGuard.EnterPublic(this.document.AccessToken))
				{
					if (!this.badState)
					{
						if (this.document.stopLoading)
						{
							throw new InvalidOperationException(Strings.LoadingStopped);
						}
						this.badState = true;
						this.document.Write(buffer, offset, count);
						this.badState = false;
					}
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && this.document != null)
				{
					using (ThreadAccessGuard.EnterPublic(this.document.AccessToken))
					{
						this.document.Flush(this.badState);
						this.document = null;
					}
				}
				base.Dispose(disposing);
			}

			private MimeDocument document;

			private bool badState;
		}

		private class CodingDataStorage : DataStorage
		{
			public CodingDataStorage(DataStorage storage, long start, long end, ContentTransferEncoding cte, bool encode)
			{
				storage.AddRef();
				this.storage = storage;
				this.start = start;
				this.end = end;
				this.cte = cte;
				this.encode = encode;
			}

			public override Stream OpenReadStream(long start, long end)
			{
				base.ThrowIfDisposed();
				start = this.start + start;
				end = ((end != long.MaxValue) ? (this.start + end) : this.end);
				ByteEncoder byteEncoder = this.encode ? MimeDocument.EncodingDataStorage.CreateEncoder(this.cte) : MimePart.CreateDecoder(this.cte);
				if (byteEncoder == null)
				{
					return this.storage.OpenReadStream(start, end);
				}
				return new EncoderStream(this.storage.OpenReadStream(start, end), byteEncoder, EncoderStreamAccess.Read, true);
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !base.IsDisposed && this.storage != null)
				{
					this.storage.Release();
					this.storage = null;
				}
				base.Dispose(disposing);
			}

			private DataStorage storage;

			private long start;

			private long end;

			private ContentTransferEncoding cte;

			private bool encode;
		}

		private class EncodingDataStorage : MimeDocument.CodingDataStorage
		{
			public EncodingDataStorage(DataStorage storage, long start, long end, ContentTransferEncoding cte) : base(storage, start, end, cte, true)
			{
			}

			internal static ByteEncoder CreateEncoder(ContentTransferEncoding encoding)
			{
				switch (encoding)
				{
				case ContentTransferEncoding.QuotedPrintable:
					return new QPEncoder();
				case ContentTransferEncoding.Base64:
					return new Base64Encoder();
				case ContentTransferEncoding.UUEncode:
					return new UUEncoder();
				default:
					return null;
				}
			}
		}

		private class DecodingDataStorage : MimeDocument.CodingDataStorage
		{
			public DecodingDataStorage(DataStorage storage, long start, long end, ContentTransferEncoding cte) : base(storage, start, end, cte, false)
			{
			}
		}
	}
}
