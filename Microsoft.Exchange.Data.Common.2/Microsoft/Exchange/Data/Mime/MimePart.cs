using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimePart : MimeNode, IDisposable, IEnumerable<MimePart>, IEnumerable
	{
		public MimePart()
		{
			this.accessToken = new MimePart.MimePartThreadAccessToken(this);
			this.headers = new HeaderList(this);
		}

		public MimePart(string contentType) : this()
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.headers.InternalAppendChild(new ContentTypeHeader(contentType));
			}
		}

		public MimePart(string contentType, string transferEncoding, Stream contentStream, CachingMode cachingMode) : this(contentType)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.SetContentStream(transferEncoding, contentStream, cachingMode);
			}
		}

		public MimePart(string contentType, ContentTransferEncoding transferEncoding, Stream contentStream, CachingMode cachingMode) : this(contentType)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.SetContentStream(transferEncoding, contentStream, cachingMode);
			}
		}

		public HeaderList Headers
		{
			get
			{
				this.ThrowIfDisposed();
				HeaderList result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.headers;
				}
				return result;
			}
		}

		public string ContentType
		{
			get
			{
				this.ThrowIfDisposed();
				string result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					ContentTypeHeader contentTypeHeader = this.headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					if (contentTypeHeader != null)
					{
						result = contentTypeHeader.Value;
					}
					else
					{
						MimePart mimePart = base.Parent as MimePart;
						if (mimePart != null)
						{
							ContentTypeHeader contentTypeHeader2 = mimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
							if (contentTypeHeader2 != null && contentTypeHeader2.Value == "multipart/digest")
							{
								return "message/rfc822";
							}
						}
						result = "text/plain";
					}
				}
				return result;
			}
		}

		public bool IsMultipart
		{
			get
			{
				this.ThrowIfDisposed();
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					ContentTypeHeader contentTypeHeader = this.headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					result = (contentTypeHeader != null && contentTypeHeader.IsMultipart);
				}
				return result;
			}
		}

		public bool IsEmbeddedMessage
		{
			get
			{
				this.ThrowIfDisposed();
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					ContentTypeHeader contentTypeHeader = this.headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					result = (contentTypeHeader != null && contentTypeHeader.IsEmbeddedMessage);
				}
				return result;
			}
		}

		public bool RequiresSMTPUTF8
		{
			get
			{
				this.ThrowIfDisposed();
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					using (MimePart.SubtreeEnumerator enumerator = this.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, true))
					{
						while (enumerator.MoveNext())
						{
							MimePart mimePart = enumerator.Current;
							foreach (Header header in mimePart.Headers)
							{
								if (header.RequiresSMTPUTF8)
								{
									return true;
								}
							}
						}
					}
					result = false;
				}
				return result;
			}
		}

		internal bool IsAnyMessage
		{
			get
			{
				this.ThrowIfDisposed();
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					ContentTypeHeader contentTypeHeader = this.headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					result = (contentTypeHeader != null && contentTypeHeader.IsAnyMessage);
				}
				return result;
			}
		}

		internal int CacheMapStamp
		{
			get
			{
				int result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.cacheMapStamp;
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.cacheMapStamp = value;
				}
			}
		}

		public ContentTransferEncoding ContentTransferEncoding
		{
			get
			{
				this.ThrowIfDisposed();
				ContentTransferEncoding result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					Header header = this.headers.FindFirst(HeaderId.ContentTransferEncoding);
					if (header != null && header.FirstRawToken.Length != 0)
					{
						result = MimePart.GetEncodingType(header.FirstRawToken);
					}
					else
					{
						result = ContentTransferEncoding.SevenBit;
					}
				}
				return result;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		internal ObjectThreadAccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
		}

		internal ObjectThreadAccessToken ParentAccessToken
		{
			get
			{
				MimeNode mimeNode;
				MimeDocument mimeDocument = MimeNode.GetParentDocument(this, out mimeNode);
				if (mimeDocument == null)
				{
					return null;
				}
				return mimeDocument.AccessToken;
			}
		}

		public MimePart.PartSubtree Subtree
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart.PartSubtree result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = new MimePart.PartSubtree(this);
				}
				return result;
			}
		}

		internal DataStorage Storage
		{
			get
			{
				DataStorage result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.dataStorage;
				}
				return result;
			}
		}

		internal long DataStart
		{
			get
			{
				long dataStart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					dataStart = this.storageInfo.DataStart;
				}
				return dataStart;
			}
		}

		internal long DataEnd
		{
			get
			{
				long dataEnd;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					dataEnd = this.storageInfo.DataEnd;
				}
				return dataEnd;
			}
		}

		internal long DataLength
		{
			get
			{
				long result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.dataStorage == null)
					{
						result = 0L;
					}
					else
					{
						if (!MimePart.IsEqualContentTransferEncoding(this.storageInfo.BodyCte, this.ContentTransferEncoding) || this.storageInfo.DataEnd == 9223372036854775807L)
						{
							using (Stream rawContentReadStream = this.GetRawContentReadStream())
							{
								if (rawContentReadStream.CanSeek)
								{
									return rawContentReadStream.Length;
								}
								byte[] array = new byte[4096];
								long num = 0L;
								int num2;
								while ((num2 = rawContentReadStream.Read(array, 0, array.Length)) != 0)
								{
									num += (long)num2;
								}
								return num;
							}
						}
						result = this.storageInfo.DataEnd - this.storageInfo.DataStart - this.storageInfo.BodyOffset;
					}
				}
				return result;
			}
		}

		internal long BodyOffset
		{
			get
			{
				long bodyOffset;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bodyOffset = this.storageInfo.BodyOffset;
				}
				return bodyOffset;
			}
		}

		internal ContentTransferEncoding BodyCte
		{
			get
			{
				ContentTransferEncoding bodyCte;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bodyCte = this.storageInfo.BodyCte;
				}
				return bodyCte;
			}
		}

		internal LineTerminationState BodyLineTermination
		{
			get
			{
				LineTerminationState bodyLineTermination;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bodyLineTermination = this.storageInfo.BodyLineTermination;
				}
				return bodyLineTermination;
			}
		}

		internal byte[] Boundary
		{
			get
			{
				byte[] result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.boundary == null)
					{
						this.boundary = MimePart.GetBoundary(this.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader);
					}
					result = this.boundary;
				}
				return result;
			}
		}

		internal bool IsSignedContent
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.Parent != null && this == base.Parent.FirstChild && this.dataStorage != null && 0L != this.storageInfo.BodyOffset && this.version == 0)
					{
						MimePart mimePart = base.Parent as MimePart;
						ContentTypeHeader contentTypeHeader = mimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
						result = (contentTypeHeader != null && contentTypeHeader.Value == "multipart/signed");
					}
					else
					{
						result = false;
					}
				}
				return result;
			}
		}

		internal bool IsProtectedContent
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.Parent != null && this.dataStorage != null && 0L != this.storageInfo.BodyOffset && this.version == 0)
					{
						MimePart mimePart = base.Parent as MimePart;
						Header header = mimePart.Headers.FindFirst("DKIM-Signature");
						result = (header != null);
					}
					else
					{
						result = false;
					}
				}
				return result;
			}
		}

		internal MimeDocument ParentDocument
		{
			get
			{
				return this.parentDocument;
			}
			set
			{
				base.ThrowIfReadOnly("MimePart.set_ParentDocument");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.parentDocument = value;
				}
			}
		}

		internal bool ContentDirty
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.contentDirty;
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.contentDirty != value)
					{
						base.ThrowIfReadOnly("MimePart.set_ContentDirty");
					}
					this.contentDirty = value;
				}
			}
		}

		internal bool ContentPersisted
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = (this.contentPersisted && this.storageInfo.BodyCte == this.ContentTransferEncoding);
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.contentPersisted = value;
				}
			}
		}

		public Stream GetRawContentReadStream()
		{
			this.ThrowIfDisposed();
			Stream rawContentReadStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				rawContentReadStream = this.GetRawContentReadStream(this.dataStorage, this.storageInfo);
			}
			return rawContentReadStream;
		}

		internal Stream GetDeferredRawContentReadStream()
		{
			Stream rawContentReadStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (!base.IsReadOnly)
				{
					rawContentReadStream = this.GetRawContentReadStream();
				}
				else
				{
					DataStorage storage;
					MimePart.DataStorageInfo dataStorageInfo;
					lock (this.deferredStorageLock)
					{
						storage = this.deferredStorage;
						dataStorageInfo = this.deferredStorageInfo;
					}
					if (dataStorageInfo == null)
					{
						rawContentReadStream = this.GetRawContentReadStream();
					}
					else
					{
						rawContentReadStream = this.GetRawContentReadStream(storage, dataStorageInfo);
					}
				}
			}
			return rawContentReadStream;
		}

		private Stream GetRawContentReadStream(DataStorage storage, MimePart.DataStorageInfo storageInfo)
		{
			this.ThrowIfDisposed();
			Stream result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Stream stream;
				if (!this.TryGetContentReadStream(storage, storageInfo, this.ContentTransferEncoding, out stream))
				{
					throw new MimeException(Strings.CannotDecodeContentStream);
				}
				result = stream;
			}
			return result;
		}

		public Stream GetContentReadStream()
		{
			this.ThrowIfDisposed();
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				Stream stream;
				if (!this.TryGetContentReadStream(this.dataStorage, this.storageInfo, ContentTransferEncoding.Binary, out stream))
				{
					throw new MimeException(Strings.CannotDecodeContentStream);
				}
				result = stream;
			}
			return result;
		}

		public bool TryGetContentReadStream(out Stream result)
		{
			this.ThrowIfDisposed();
			bool result2;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result2 = this.TryGetContentReadStream(this.dataStorage, this.storageInfo, ContentTransferEncoding.Binary, out result);
			}
			return result2;
		}

		private bool TryGetContentReadStream(DataStorage dataStorage, MimePart.DataStorageInfo storageInfo, ContentTransferEncoding desiredCte, out Stream result)
		{
			bool result2;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				result = null;
				ContentTransferEncoding contentTransferEncoding = storageInfo.BodyCte;
				if (contentTransferEncoding == ContentTransferEncoding.Unknown)
				{
					contentTransferEncoding = this.ContentTransferEncoding;
				}
				if (!MimePart.IsEqualContentTransferEncoding(desiredCte, contentTransferEncoding))
				{
					if (desiredCte == ContentTransferEncoding.Unknown)
					{
						return false;
					}
					if (contentTransferEncoding == ContentTransferEncoding.Unknown)
					{
						return false;
					}
					if (this.IsMultipart || this.IsAnyMessage)
					{
						contentTransferEncoding = ContentTransferEncoding.Binary;
						desiredCte = ContentTransferEncoding.Binary;
					}
				}
				bool flag = false;
				try
				{
					if (dataStorage == null)
					{
						if (this.IsMultipart && base.InternalLastChild != null)
						{
							return true;
						}
						if (!this.IsEmbeddedMessage || base.InternalLastChild == null)
						{
							result = DataStorage.NewEmptyReadStream();
							flag = true;
							return true;
						}
						TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
						try
						{
							using (Stream stream = temporaryDataStorage.OpenWriteStream(true))
							{
								base.FirstChild.WriteTo(stream);
							}
							result = temporaryDataStorage.OpenReadStream(0L, temporaryDataStorage.Length);
							goto IL_F2;
						}
						finally
						{
							temporaryDataStorage.Release();
						}
					}
					result = dataStorage.OpenReadStream(storageInfo.DataStart + storageInfo.BodyOffset, storageInfo.DataEnd);
					IL_F2:
					if (!MimePart.IsEqualContentTransferEncoding(desiredCte, contentTransferEncoding))
					{
						if (!MimePart.EncodingIsTransparent(contentTransferEncoding))
						{
							ByteEncoder encoder = MimePart.CreateDecoder(contentTransferEncoding);
							result = new EncoderStream(result, encoder, EncoderStreamAccess.Read, true);
						}
						if (!MimePart.EncodingIsTransparent(desiredCte))
						{
							ByteEncoder encoder2 = MimePart.CreateEncoder(result, desiredCte);
							result = new EncoderStream(result, encoder2, EncoderStreamAccess.Read, true);
						}
					}
					flag = true;
				}
				finally
				{
					if (!flag && result != null)
					{
						result.Dispose();
						result = null;
					}
				}
				result2 = true;
			}
			return result2;
		}

		public Stream GetRawContentWriteStream()
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.GetRawContentWriteStream");
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.IsMultipart)
				{
					throw new NotSupportedException(Strings.ModifyingRawContentOfMultipartNotSupported);
				}
				this.SetStorage(null, 0L, 0L);
				result = new MimePart.PartContentWriteStream(this, ContentTransferEncoding.Unknown);
			}
			return result;
		}

		public Stream GetContentWriteStream(ContentTransferEncoding transferEncoding)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.GetContentWriteStream");
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.IsMultipart)
				{
					throw new NotSupportedException(Strings.ModifyingRawContentOfMultipartNotSupported);
				}
				this.UpdateTransferEncoding(transferEncoding);
				this.SetStorage(null, 0L, 0L);
				result = new MimePart.PartContentWriteStream(this, ContentTransferEncoding.Binary);
			}
			return result;
		}

		public Stream GetContentWriteStream(string transferEncoding)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.GetContentWriteStream");
			Stream contentWriteStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (transferEncoding == null)
				{
					throw new ArgumentNullException("transferEncoding");
				}
				contentWriteStream = this.GetContentWriteStream(MimePart.GetEncodingType(new MimeString(transferEncoding)));
			}
			return contentWriteStream;
		}

		public void SetContentStream(string transferEncoding, Stream contentStream, CachingMode cachingMode)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.SetContentStream");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				ContentTransferEncoding contentTransferEncoding = ContentTransferEncoding.Unknown;
				if (transferEncoding != null)
				{
					contentTransferEncoding = MimePart.GetEncodingType(new MimeString(transferEncoding));
					if (contentTransferEncoding == ContentTransferEncoding.Unknown)
					{
						throw new ArgumentException("Transfer encoding is unknown or not supported", "transferEncoding");
					}
				}
				this.SetContentStream(contentTransferEncoding, contentStream, cachingMode);
			}
		}

		public void SetContentStream(ContentTransferEncoding transferEncoding, Stream contentStream, CachingMode cachingMode)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.SetContentStream");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (contentStream == null)
				{
					throw new ArgumentNullException("contentStream");
				}
				if (!contentStream.CanRead)
				{
					throw new ArgumentException(Strings.StreamMustSupportRead, "contentStream");
				}
				if (this.IsMultipart)
				{
					throw new NotSupportedException(Strings.ModifyingRawContentOfMultipartNotSupported);
				}
				DataStorage dataStorage = null;
				long dataStart = 0L;
				long dataEnd = long.MaxValue;
				if (transferEncoding != ContentTransferEncoding.Unknown)
				{
					this.UpdateTransferEncoding(transferEncoding);
					transferEncoding = ContentTransferEncoding.Binary;
				}
				switch (cachingMode)
				{
				case CachingMode.Copy:
				{
					TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
					dataStorage = temporaryDataStorage;
					using (Stream stream = temporaryDataStorage.OpenWriteStream(true))
					{
						byte[] array = null;
						dataEnd = DataStorage.CopyStreamToStream(contentStream, stream, long.MaxValue, ref array);
						goto IL_12B;
					}
					break;
				}
				case CachingMode.Source:
				case CachingMode.SourceTakeOwnership:
				{
					if (!contentStream.CanSeek)
					{
						throw new NotSupportedException(Strings.CachingModeSourceButStreamCannotSeek);
					}
					StreamOnDataStorage streamOnDataStorage = contentStream as StreamOnDataStorage;
					if (streamOnDataStorage == null)
					{
						dataStorage = new ReadableDataStorageOnStream(contentStream, cachingMode == CachingMode.SourceTakeOwnership);
						goto IL_12B;
					}
					dataStorage = streamOnDataStorage.Storage;
					dataStart = streamOnDataStorage.Start;
					dataEnd = streamOnDataStorage.End;
					dataStorage.AddRef();
					if (cachingMode == CachingMode.SourceTakeOwnership)
					{
						contentStream.Dispose();
						contentStream = null;
						goto IL_12B;
					}
					goto IL_12B;
				}
				}
				throw new ArgumentException("Invalid Caching Mode value", "cachingMode");
				IL_12B:
				this.SetStorage(dataStorage, dataStart, dataEnd, 0L, transferEncoding, LineTerminationState.Unknown);
				dataStorage.Release();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				using (MimePart.SubtreeEnumerator enumerator = this.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, false))
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.dataStorage != null)
						{
							enumerator.Current.dataStorage.Release();
							enumerator.Current.dataStorage = null;
						}
						if (enumerator.Current.headers != null)
						{
							enumerator.Current.headers.InternalDetachParent();
						}
						enumerator.Current.headers = null;
						enumerator.Current.boundary = null;
						enumerator.Current.parentDocument = null;
						if (enumerator.Current.deferredStorage != null)
						{
							enumerator.Current.deferredStorage.Release();
							enumerator.Current.deferredStorage = null;
							enumerator.Current.deferredStorageInfo = null;
						}
						enumerator.Current.isDisposed = true;
						GC.SuppressFinalize(enumerator.Current);
					}
					return;
				}
			}
			this.isDisposed = true;
		}

		public new MimeNode.Enumerator<MimePart> GetEnumerator()
		{
			this.ThrowIfDisposed();
			MimeNode.Enumerator<MimePart> result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = new MimeNode.Enumerator<MimePart>(this);
			}
			return result;
		}

		IEnumerator<MimePart> IEnumerable<MimePart>.GetEnumerator()
		{
			this.ThrowIfDisposed();
			IEnumerator<MimePart> result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = new MimeNode.Enumerator<MimePart>(this);
			}
			return result;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.ThrowIfDisposed();
			IEnumerator result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = new MimeNode.Enumerator<MimePart>(this);
			}
			return result;
		}

		public sealed override MimeNode Clone()
		{
			MimeNode result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimePart mimePart = new MimePart();
				this.CopyTo(mimePart);
				result = mimePart;
			}
			return result;
		}

		public sealed override void CopyTo(object destination)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if (destination != this)
				{
					MimePart mimePart = destination as MimePart;
					if (mimePart == null)
					{
						throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
					}
					using (ThreadAccessGuard.EnterPublic(mimePart.accessToken))
					{
						byte[] array = null;
						TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
						using (Stream stream = temporaryDataStorage.OpenWriteStream(true))
						{
							this.CopyPartTo(false, mimePart, temporaryDataStorage, stream, 0L, ref array);
						}
						temporaryDataStorage.Release();
						mimePart.SetDirty();
					}
				}
			}
		}

		public long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter)
		{
			this.ThrowIfDisposed();
			long result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				if (encodingOptions == null)
				{
					encodingOptions = base.GetDocumentEncodingOptions();
				}
				byte[] array = null;
				MimeStringLength mimeStringLength = new MimeStringLength(0);
				result = this.WriteTo(stream, encodingOptions, filter, ref mimeStringLength, ref array);
			}
			return result;
		}

		internal bool IsProtectedHeader(string headerName)
		{
			this.ThrowIfDisposed();
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (!string.IsNullOrEmpty(headerName))
				{
					Header header = this.Headers.FindFirst("DKIM-Signature");
					if (header != null)
					{
						if (this.protectedHeaders == null)
						{
							this.PopulateProtectedHeaders();
						}
						foreach (string value in this.protectedHeaders)
						{
							if (headerName.Equals(value, StringComparison.OrdinalIgnoreCase))
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private void PopulateProtectedHeaders()
		{
			this.protectedHeaders = new List<string>();
			this.protectedHeaders.Add("DKIM-Signature");
			Header header = this.Headers.FindFirst("DKIM-Signature");
			DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
			while (header != null)
			{
				ValueParser valueParser = new ValueParser(header.Lines, headerDecodingOptions.AllowUTF8);
				MimeStringList mimeStringList = default(MimeStringList);
				bool handleISO = true;
				byte b;
				do
				{
					valueParser.ParseCFWS(false, ref mimeStringList, handleISO);
					b = valueParser.ParseGet();
					if (b != 59)
					{
						if (b == 0)
						{
							break;
						}
						valueParser.ParseUnget();
					}
					valueParser.ParseCFWS(false, ref mimeStringList, handleISO);
					MimeString mimeString = valueParser.ParseToken();
					if (mimeString.Length == 0 || mimeString.Length >= 128)
					{
						valueParser.ParseSkipToNextDelimiterByte(59);
					}
					else
					{
						valueParser.ParseCFWS(false, ref mimeStringList, handleISO);
						b = valueParser.ParseGet();
						if (b != 61)
						{
							if (b != 0)
							{
								valueParser.ParseUnget();
							}
						}
						else
						{
							valueParser.ParseCFWS(false, ref mimeStringList, handleISO);
							MimeStringList lines = default(MimeStringList);
							bool flag = false;
							valueParser.ParseParameterValue(ref lines, ref flag, handleISO);
							string text = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
							if (text.Equals("h", StringComparison.OrdinalIgnoreCase))
							{
								ValueParser valueParser2 = new ValueParser(lines, false);
								byte b2;
								do
								{
									valueParser2.ParseCFWS(false, ref mimeStringList, handleISO);
									b2 = valueParser2.ParseGet();
									if (b2 != 58)
									{
										if (b2 == 0)
										{
											break;
										}
										valueParser2.ParseUnget();
									}
									valueParser2.ParseCFWS(false, ref mimeStringList, handleISO);
									MimeString mimeString2 = valueParser2.ParseToken();
									if (mimeString2.Length == 0 || mimeString2.Length >= 128)
									{
										valueParser2.ParseSkipToNextDelimiterByte(58);
									}
									else
									{
										string item = Header.NormalizeString(mimeString2.Data, mimeString2.Offset, mimeString2.Length, false);
										this.protectedHeaders.Add(item);
									}
								}
								while (b2 != 0);
							}
						}
					}
				}
				while (b != 0);
				header = this.Headers.FindNext(header);
			}
		}

		internal static byte[] GetBoundary(ContentTypeHeader contentType)
		{
			if (contentType == null || !contentType.IsMultipart)
			{
				return null;
			}
			MimeParameter mimeParameter = contentType["boundary"];
			if (mimeParameter == null)
			{
				mimeParameter = new MimeParameter("boundary");
				contentType.InternalAppendChild(mimeParameter);
				mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
			}
			byte[] rawValue = mimeParameter.RawValue;
			int num = (rawValue != null) ? rawValue.Length : 0;
			if (num == 0 || 70 < num)
			{
				mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
			}
			int num2 = MimeString.CRLF2Dashes.Length + rawValue.Length + MimeString.CrLf.Length;
			byte[] array = new byte[num2];
			int num3 = 0;
			Buffer.BlockCopy(MimeString.CRLF2Dashes, 0, array, num3, MimeString.CRLF2Dashes.Length);
			num3 = MimeString.CRLF2Dashes.Length;
			Buffer.BlockCopy(rawValue, 0, array, num3, rawValue.Length);
			num3 += rawValue.Length;
			Buffer.BlockCopy(MimeString.CrLf, 0, array, num3, MimeString.CrLf.Length);
			return array;
		}

		internal static ContentTransferEncoding GetEncodingType(MimeString str)
		{
			if (str.Length != 0)
			{
				for (int i = 0; i < MimePart.EncodingMap.Length; i++)
				{
					if (str.CompareEqI(MimePart.EncodingMap[i].Name))
					{
						return MimePart.EncodingMap[i].Type;
					}
				}
			}
			return ContentTransferEncoding.Unknown;
		}

		internal static byte[] GetEncodingName(ContentTransferEncoding encoding)
		{
			for (int i = 0; i < MimePart.EncodingMap.Length; i++)
			{
				if (MimePart.EncodingMap[i].Type == encoding)
				{
					return MimePart.EncodingMap[i].Name;
				}
			}
			return null;
		}

		internal static bool IsEqualContentTransferEncoding(ContentTransferEncoding cte1, ContentTransferEncoding cte2)
		{
			return cte1 == cte2 || (MimePart.EncodingIsTransparent(cte1) && MimePart.EncodingIsTransparent(cte2));
		}

		internal static bool EncodingIsTransparent(ContentTransferEncoding cte)
		{
			return cte == ContentTransferEncoding.Binary || cte == ContentTransferEncoding.SevenBit || cte == ContentTransferEncoding.EightBit;
		}

		internal static ByteEncoder CreateEncoder(Stream stream, ContentTransferEncoding encoding)
		{
			switch (encoding)
			{
			case ContentTransferEncoding.QuotedPrintable:
				return new QPEncoder();
			case ContentTransferEncoding.Base64:
				return new Base64Encoder();
			case ContentTransferEncoding.UUEncode:
				return new UUEncoder();
			case ContentTransferEncoding.BinHex:
				return new BinHexEncoder(new MacBinaryHeader
				{
					DataForkLength = stream.Length,
					FileName = "binhex.dat"
				});
			default:
				return null;
			}
		}

		internal static ByteEncoder CreateDecoder(ContentTransferEncoding encoding)
		{
			switch (encoding)
			{
			case ContentTransferEncoding.QuotedPrintable:
				return new QPDecoder();
			case ContentTransferEncoding.Base64:
				return new Base64Decoder();
			case ContentTransferEncoding.UUEncode:
				return new UUDecoder();
			case ContentTransferEncoding.BinHex:
				return new BinHexDecoder(true);
			default:
				return null;
			}
		}

		internal static long CopyStorageToStream(DataStorage srcStorage, long start, long end, LineTerminationState srcLineTermination, Stream destStream, ref byte[] scratchBuffer, ref LineTerminationState lineTermination)
		{
			long num = 0L;
			if (lineTermination != LineTerminationState.NotInteresting && srcLineTermination == LineTerminationState.Unknown)
			{
				if (scratchBuffer == null || scratchBuffer.Length < 16384)
				{
					scratchBuffer = new byte[16384];
				}
				using (Stream stream = srcStorage.OpenReadStream(start, end))
				{
					for (;;)
					{
						int num2 = stream.Read(scratchBuffer, 0, scratchBuffer.Length);
						if (num2 == 0)
						{
							break;
						}
						if (lineTermination != LineTerminationState.NotInteresting)
						{
							lineTermination = MimeCommon.AdvanceLineTerminationState(lineTermination, scratchBuffer, 0, num2);
						}
						destStream.Write(scratchBuffer, 0, num2);
						num += (long)num2;
					}
				}
				return num;
			}
			MimePart.CountingWriteStream countingWriteStream = null;
			if ((Stream.Null == destStream || ((countingWriteStream = (destStream as MimePart.CountingWriteStream)) != null && countingWriteStream.IsCountingOnly)) && end != 9223372036854775807L)
			{
				num = end - start;
				if (countingWriteStream != null)
				{
					countingWriteStream.Add(num);
				}
				if (lineTermination != LineTerminationState.NotInteresting)
				{
					lineTermination = srcLineTermination;
				}
				return num;
			}
			num = srcStorage.CopyContentToStream(start, end, destStream, ref scratchBuffer);
			if (lineTermination != LineTerminationState.NotInteresting)
			{
				lineTermination = srcLineTermination;
			}
			return num;
		}

		internal override void SetDirty()
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.SetDirty");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.boundary = null;
				MimePart mimePart = this;
				bool flag = false;
				MimePart mimePart2;
				do
				{
					if (flag)
					{
						mimePart.SetStorageImpl(null, 0L, 0L);
						mimePart.ContentPersisted = false;
						mimePart.ContentDirty = true;
					}
					mimePart.IncrementVersion();
					flag = true;
					mimePart2 = mimePart;
					mimePart = (mimePart.Parent as MimePart);
				}
				while (mimePart != null);
				if (mimePart2.ParentDocument != null)
				{
					mimePart2.ParentDocument.IncrementVersion();
				}
			}
		}

		internal override void ChildRemoved(MimeNode oldChild)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.ChildRemoved");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.dataStorage != null)
				{
					this.SetStorageImpl(null, 0L, 0L);
					this.ContentPersisted = false;
					this.ContentDirty = true;
				}
			}
		}

		internal override void RemoveAllUnparsed()
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.RemoveAllUnparsed");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.dataStorage != null && this.IsEmbeddedMessage)
				{
					this.SetStorageImpl(null, 0L, 0L);
					this.ContentPersisted = false;
					this.ContentDirty = true;
				}
			}
		}

		internal override MimeNode ParseNextChild()
		{
			this.ThrowIfDisposed();
			MimeNode result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (base.InternalLastChild != null || this.dataStorage == null || !this.IsEmbeddedMessage)
				{
					result = null;
				}
				else
				{
					base.ThrowIfReadOnly("MimePart.ParseNextChild");
					MimeDocument mimeDocument = null;
					MimeDocument mimeDocument2;
					MimeNode mimeNode;
					base.GetMimeDocumentOrTreeRoot(out mimeDocument2, out mimeNode);
					try
					{
						if (mimeDocument2 == null)
						{
							mimeDocument = new MimeDocument();
							mimeDocument2 = mimeDocument;
						}
						mimeDocument2.BuildEmbeddedDom((MimePart)mimeNode);
						result = base.InternalLastChild;
					}
					finally
					{
						if (mimeDocument != null)
						{
							mimeDocument.Dispose();
							mimeDocument = null;
						}
					}
				}
			}
			return result;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimePart mimePart = newChild as MimePart;
				if (mimePart == null)
				{
					throw new ArgumentException(Strings.NewChildIsNotAPart);
				}
				MimeNode mimeNode = this;
				while (mimeNode != newChild)
				{
					mimeNode = mimeNode.Parent;
					if (mimeNode == null)
					{
						base.ThrowIfReadOnly("MimePart.ValidateNewChild");
						if (mimePart.parentDocument != null)
						{
							mimePart.parentDocument.RootPart = new MimePart();
							mimePart.parentDocument = null;
						}
						if (this.IsEmbeddedMessage && base.InternalLastChild != null)
						{
							base.InternalRemoveChild(base.InternalLastChild);
							refChild = null;
						}
						if (this.dataStorage != null)
						{
							this.SetStorageImpl(null, 0L, 0L);
							this.ContentPersisted = false;
							this.ContentDirty = true;
						}
						return refChild;
					}
				}
				throw new ArgumentException(Strings.ThisPartBelongsToSubtreeOfNewChild);
			}
			MimeNode result;
			return result;
		}

		internal void CopyPartTo(bool signedOrProtectedContent, MimePart dstPart, TemporaryDataStorage dstStorage, Stream dstStream, long position, ref byte[] scratchBuffer)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				using (ThreadAccessGuard.EnterPublic(dstPart.accessToken))
				{
					long num = 0L;
					MimePart mimePart = null;
					using (MimePart.SubtreeEnumerator enumerator = this.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages | MimePart.SubtreeEnumerationOptions.RevisitParent, false))
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.FirstVisit)
							{
								if (enumerator.Depth != 0)
								{
									dstPart = new MimePart();
								}
								else if (signedOrProtectedContent)
								{
									DataStorage dataStorage = enumerator.Current.dataStorage;
									long dataStart = enumerator.Current.storageInfo.DataStart;
									num = position - dataStart;
									mimePart = dstPart;
									continue;
								}
								if (signedOrProtectedContent && enumerator.Current.dataStorage != null)
								{
									dstPart.SetStorageImpl(dstStorage, num + enumerator.Current.storageInfo.DataStart, num + enumerator.Current.storageInfo.DataEnd, enumerator.Current.storageInfo.BodyOffset, enumerator.Current.storageInfo.BodyCte, enumerator.Current.BodyLineTermination);
								}
								else if (enumerator.Current.IsSignedContent || enumerator.Current.IsProtectedContent)
								{
									long num2 = enumerator.Current.dataStorage.CopyContentToStream(enumerator.Current.storageInfo.DataStart, enumerator.Current.storageInfo.DataEnd, dstStream, ref scratchBuffer);
									dstPart.SetStorageImpl(dstStorage, position, position + num2, enumerator.Current.storageInfo.BodyOffset, enumerator.Current.storageInfo.BodyCte, enumerator.Current.storageInfo.BodyLineTermination);
									if (!enumerator.LastVisit)
									{
										enumerator.Current.CopyPartTo(true, dstPart, dstStorage, null, position, ref scratchBuffer);
										enumerator.SkipChildren();
									}
									position += num2;
								}
								else if (enumerator.Current.dataStorage == null || enumerator.Current.IsMultipart || (enumerator.Current.IsEmbeddedMessage && !enumerator.LastVisit))
								{
									dstPart.SetStorageImpl(null, 0L, 0L);
								}
								else
								{
									long num3 = enumerator.Current.dataStorage.CopyContentToStream(enumerator.Current.storageInfo.DataStart + enumerator.Current.storageInfo.BodyOffset, enumerator.Current.storageInfo.DataEnd, dstStream, ref scratchBuffer);
									dstPart.SetStorageImpl(dstStorage, position, position + num3, 0L, enumerator.Current.storageInfo.BodyCte, enumerator.Current.storageInfo.BodyLineTermination);
									position += num3;
								}
								dstPart.contentDirty = (enumerator.Depth == 0 && !signedOrProtectedContent);
								enumerator.Current.headers.CopyTo(dstPart.headers);
								if (!signedOrProtectedContent && !enumerator.Current.IsSignedContent && !enumerator.Current.IsProtectedContent)
								{
									ContentTypeHeader contentTypeHeader = dstPart.headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
									if (contentTypeHeader != null && contentTypeHeader.IsMultipart)
									{
										MimeParameter mimeParameter = contentTypeHeader["boundary"];
										if (mimeParameter != null)
										{
											mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
										}
									}
								}
								if (mimePart != null)
								{
									if (mimePart.IsEmbeddedMessage && mimePart.dataStorage != null && mimePart.InternalLastChild == null)
									{
										mimePart.InternalInsertAfter(dstPart, null);
									}
									else
									{
										mimePart.InternalAppendChild(dstPart);
									}
								}
								if (!enumerator.LastVisit)
								{
									mimePart = dstPart;
								}
							}
							else if (enumerator.LastVisit && mimePart != null)
							{
								mimePart = (mimePart.Parent as MimePart);
							}
						}
					}
				}
			}
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			this.ThrowIfDisposed();
			long result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				long num = 0L;
				MimePart.CountingWriteStream countingWriteStream = null;
				MimePart.CountingWriteStream countingWriteStream2 = null;
				long num2 = 0L;
				if (filter != null)
				{
					countingWriteStream = (stream as MimePart.CountingWriteStream);
					if (countingWriteStream == null)
					{
						countingWriteStream2 = new MimePart.CountingWriteStream(stream);
						countingWriteStream = countingWriteStream2;
						stream = countingWriteStream;
					}
					num2 = countingWriteStream.Count;
				}
				byte[] array = null;
				bool flag = true;
				LineTerminationState lineTerminationState = this.IsMultipart ? LineTerminationState.NotInteresting : LineTerminationState.CRLF;
				using (MimePart.SubtreeEnumerator enumerator = this.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages | MimePart.SubtreeEnumerationOptions.RevisitParent, false))
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.FirstVisit)
						{
							if (filter != null && filter.FilterPart(enumerator.Current, stream))
							{
								enumerator.SkipChildren();
							}
							else
							{
								if (array != null)
								{
									if (flag)
									{
										stream.Write(array, 0, array.Length);
										num += (long)array.Length;
									}
									else
									{
										stream.Write(array, 2, array.Length - 2);
										num += (long)(array.Length - 2);
										flag = true;
									}
									if (LineTerminationState.NotInteresting != lineTerminationState)
									{
										lineTerminationState = LineTerminationState.CRLF;
									}
								}
								if (enumerator.Current.IsSignedContent)
								{
									num += MimePart.CopyStorageToStream(enumerator.Current.dataStorage, enumerator.Current.storageInfo.DataStart, enumerator.Current.storageInfo.DataEnd, enumerator.Current.storageInfo.BodyLineTermination, stream, ref scratchBuffer, ref lineTerminationState);
									if (filter != null)
									{
										filter.ClosePart(enumerator.Current, stream);
									}
									enumerator.SkipChildren();
								}
								else
								{
									if (filter == null || !filter.FilterHeaderList(enumerator.Current.Headers, stream))
									{
										num += enumerator.Current.Headers.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
										flag = true;
									}
									else
									{
										flag = false;
									}
									if (filter != null && filter.FilterPartBody(enumerator.Current, stream))
									{
										if (filter != null)
										{
											filter.ClosePart(enumerator.Current, stream);
										}
										enumerator.SkipChildren();
										flag = true;
									}
									else if (enumerator.Current.IsMultipart)
									{
										if (!enumerator.LastVisit)
										{
											array = enumerator.Current.Boundary;
										}
										else
										{
											if (flag)
											{
												stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
												num += (long)MimeString.CrLf.Length;
											}
											if (filter != null)
											{
												filter.ClosePart(enumerator.Current, stream);
											}
										}
									}
									else if (enumerator.Current.IsEmbeddedMessage && !enumerator.LastVisit)
									{
										if (flag)
										{
											stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
											num += (long)MimeString.CrLf.Length;
										}
										array = null;
									}
									else
									{
										if (flag)
										{
											stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
											num += (long)MimeString.CrLf.Length;
										}
										if (enumerator.Current.ContentTransferEncoding == enumerator.Current.storageInfo.BodyCte)
										{
											if (enumerator.Current.dataStorage != null)
											{
												num += MimePart.CopyStorageToStream(enumerator.Current.dataStorage, enumerator.Current.storageInfo.DataStart + enumerator.Current.storageInfo.BodyOffset, enumerator.Current.storageInfo.DataEnd, enumerator.Current.storageInfo.BodyLineTermination, stream, ref scratchBuffer, ref lineTerminationState);
											}
										}
										else
										{
											if (scratchBuffer == null || scratchBuffer.Length < 16384)
											{
												scratchBuffer = new byte[16384];
											}
											using (Stream rawContentReadStream = enumerator.Current.GetRawContentReadStream())
											{
												for (;;)
												{
													int num3 = rawContentReadStream.Read(scratchBuffer, 0, scratchBuffer.Length);
													if (num3 == 0)
													{
														break;
													}
													if (lineTerminationState != LineTerminationState.NotInteresting)
													{
														lineTerminationState = MimeCommon.AdvanceLineTerminationState(lineTerminationState, scratchBuffer, 0, num3);
													}
													stream.Write(scratchBuffer, 0, num3);
													num += (long)num3;
												}
											}
										}
										if (filter != null)
										{
											filter.ClosePart(enumerator.Current, stream);
										}
										if (!enumerator.LastVisit)
										{
											enumerator.SkipChildren();
										}
									}
								}
							}
						}
						else if (enumerator.LastVisit)
						{
							if (array != null)
							{
								stream.Write(array, 0, array.Length - 2);
								num += (long)(array.Length - 2);
								stream.Write(MimeString.TwoDashesCRLF, 0, MimeString.TwoDashesCRLF.Length);
								num += (long)MimeString.TwoDashesCRLF.Length;
								if (LineTerminationState.NotInteresting != lineTerminationState)
								{
									lineTerminationState = LineTerminationState.CRLF;
								}
							}
							array = ((enumerator.Current.Parent == null) ? null : ((MimePart)enumerator.Current.Parent).Boundary);
							if (filter != null)
							{
								filter.ClosePart(enumerator.Current, stream);
							}
						}
					}
				}
				if (!this.IsSignedContent)
				{
					switch (lineTerminationState)
					{
					case LineTerminationState.CR:
						stream.Write(MimeString.CrLf, 1, 1);
						num += 1L;
						break;
					case LineTerminationState.Other:
						stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
						num += (long)MimeString.CrLf.Length;
						break;
					}
				}
				if (countingWriteStream != null)
				{
					num = countingWriteStream.Count - num2;
					if (countingWriteStream2 != null)
					{
						countingWriteStream2.Dispose();
					}
				}
				result = num;
			}
			return result;
		}

		internal void SetStorage(DataStorage storage, long dataStart, long dataEnd)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.SetStorageImpl(storage, dataStart, dataEnd);
				this.ContentPersisted = false;
				this.ContentDirty = true;
				this.SetDirty();
			}
		}

		internal void SetStorage(DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.SetStorageImpl(storage, dataStart, dataEnd, bodyOffset, bodyCte, bodyLineTermination);
				this.ContentPersisted = false;
				this.ContentDirty = true;
				this.SetDirty();
			}
		}

		internal void SetStorageImpl(DataStorage storage, long dataStart, long dataEnd)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.SetStorageImpl(storage, dataStart, dataEnd, 0L, ContentTransferEncoding.Binary, LineTerminationState.Unknown);
			}
		}

		internal void SetStorageImpl(DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.SetStorageImpl");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (storage != null)
				{
					storage.AddRef();
					storage.SetReadOnly(false);
				}
				if (this.dataStorage != null)
				{
					this.dataStorage.Release();
				}
				this.dataStorage = storage;
				this.storageInfo.DataStart = dataStart;
				this.storageInfo.DataEnd = dataEnd;
				this.storageInfo.BodyOffset = bodyOffset;
				this.storageInfo.BodyCte = bodyCte;
				this.storageInfo.BodyLineTermination = bodyLineTermination;
			}
		}

		internal void SetDeferredStorageImpl(DataStorage storage, long dataStart, long dataEnd)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.SetDeferredStorageImpl(storage, dataStart, dataEnd, 0L, ContentTransferEncoding.Binary, LineTerminationState.Unknown);
			}
		}

		internal void SetDeferredStorageImpl(DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (base.IsReadOnly)
				{
					if (storage != null)
					{
						storage.AddRef();
					}
					MimePart.DataStorageInfo dataStorageInfo = new MimePart.DataStorageInfo();
					dataStorageInfo.DataStart = dataStart;
					dataStorageInfo.DataEnd = dataEnd;
					dataStorageInfo.BodyOffset = bodyOffset;
					dataStorageInfo.BodyCte = bodyCte;
					dataStorageInfo.BodyLineTermination = bodyLineTermination;
					lock (this.deferredStorageLock)
					{
						if (this.deferredStorage != null)
						{
							this.deferredStorage.Release();
						}
						this.deferredStorage = storage;
						this.deferredStorageInfo = dataStorageInfo;
						goto IL_9C;
					}
				}
				this.SetStorageImpl(storage, dataStart, dataEnd, bodyOffset, bodyCte, bodyLineTermination);
				IL_9C:;
			}
		}

		internal DataStorage DeferredStorage
		{
			get
			{
				DataStorage storage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.IsReadOnly)
					{
						lock (this.deferredStorageLock)
						{
							return (this.deferredStorageInfo != null) ? this.deferredStorage : this.dataStorage;
						}
					}
					storage = this.Storage;
				}
				return storage;
			}
		}

		internal long DeferredDataStart
		{
			get
			{
				long dataStart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.IsReadOnly)
					{
						lock (this.deferredStorageLock)
						{
							return (this.deferredStorageInfo != null) ? this.deferredStorageInfo.DataStart : this.storageInfo.DataStart;
						}
					}
					dataStart = this.DataStart;
				}
				return dataStart;
			}
		}

		internal long DeferredDataEnd
		{
			get
			{
				long dataEnd;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.IsReadOnly)
					{
						lock (this.deferredStorageLock)
						{
							return (this.deferredStorageInfo != null) ? this.deferredStorageInfo.DataEnd : this.storageInfo.DataEnd;
						}
					}
					dataEnd = this.DataEnd;
				}
				return dataEnd;
			}
		}

		internal long DeferredDataLength
		{
			get
			{
				long result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!base.IsReadOnly)
					{
						result = this.DataLength;
					}
					else
					{
						DataStorage dataStorage;
						MimePart.DataStorageInfo dataStorageInfo;
						lock (this.deferredStorageLock)
						{
							dataStorage = this.deferredStorage;
							dataStorageInfo = this.deferredStorageInfo;
						}
						if (dataStorageInfo == null)
						{
							result = this.DataLength;
						}
						else if (dataStorage == null)
						{
							result = 0L;
						}
						else
						{
							if (!MimePart.IsEqualContentTransferEncoding(dataStorageInfo.BodyCte, this.ContentTransferEncoding) || dataStorageInfo.DataEnd == 9223372036854775807L)
							{
								using (Stream rawContentReadStream = this.GetRawContentReadStream(dataStorage, dataStorageInfo))
								{
									if (rawContentReadStream.CanSeek)
									{
										return rawContentReadStream.Length;
									}
									byte[] array = new byte[4096];
									long num = 0L;
									int num2;
									while ((num2 = rawContentReadStream.Read(array, 0, array.Length)) != 0)
									{
										num += (long)num2;
									}
									return num;
								}
							}
							result = dataStorageInfo.DataEnd - dataStorageInfo.DataStart - dataStorageInfo.BodyOffset;
						}
					}
				}
				return result;
			}
		}

		internal long DeferredBodyOffset
		{
			get
			{
				long bodyOffset;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.IsReadOnly)
					{
						lock (this.deferredStorageLock)
						{
							return (this.deferredStorageInfo != null) ? this.deferredStorageInfo.BodyOffset : this.storageInfo.BodyOffset;
						}
					}
					bodyOffset = this.BodyOffset;
				}
				return bodyOffset;
			}
		}

		internal ContentTransferEncoding DeferredBodyCte
		{
			get
			{
				ContentTransferEncoding bodyCte;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.IsReadOnly)
					{
						lock (this.deferredStorageLock)
						{
							return (this.deferredStorageInfo != null) ? this.deferredStorageInfo.BodyCte : this.storageInfo.BodyCte;
						}
					}
					bodyCte = this.BodyCte;
				}
				return bodyCte;
			}
		}

		internal LineTerminationState DeferredBodyLineTermination
		{
			get
			{
				LineTerminationState bodyLineTermination;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (base.IsReadOnly)
					{
						lock (this.deferredStorageLock)
						{
							return (this.deferredStorageInfo != null) ? this.deferredStorageInfo.BodyLineTermination : this.storageInfo.BodyLineTermination;
						}
					}
					bodyLineTermination = this.BodyLineTermination;
				}
				return bodyLineTermination;
			}
		}

		internal void SetReadOnlyInternal(bool makeReadOnly)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (!makeReadOnly && this.deferredStorageInfo != null)
				{
					if (this.dataStorage != null)
					{
						this.dataStorage.Release();
					}
					this.dataStorage = this.deferredStorage;
					this.storageInfo = this.deferredStorageInfo;
					this.deferredStorage = null;
					this.deferredStorageInfo = null;
				}
				if (this.dataStorage != null)
				{
					this.Storage.SetReadOnly(makeReadOnly);
				}
			}
		}

		internal Charset FindMimeTreeCharset()
		{
			Charset result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				Charset charset = null;
				MimePart mimePart = this;
				while (!mimePart.IsEmbeddedMessage && mimePart.FirstChild != null)
				{
					mimePart = (MimePart)mimePart.FirstChild;
				}
				ComplexHeader complexHeader = mimePart.Headers.FindFirst(HeaderId.ContentType) as ComplexHeader;
				if (complexHeader != null)
				{
					MimeParameter mimeParameter = complexHeader["charset"];
					if (mimeParameter != null)
					{
						byte[] rawValue = mimeParameter.RawValue;
						if (rawValue != null)
						{
							string text = ByteString.BytesToString(rawValue, false);
							if (text != null && Charset.TryGetCharset(text, out charset) && charset.AsciiSupport < CodePageAsciiSupport.Fine)
							{
								charset = charset.Culture.MimeCharset;
							}
						}
					}
				}
				result = charset;
			}
			return result;
		}

		private void IncrementVersion()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.version = ((int.MaxValue == this.version) ? 1 : (this.version + 1));
				this.protectedHeaders = null;
			}
		}

		internal void UpdateTransferEncoding(ContentTransferEncoding transferEncoding)
		{
			this.ThrowIfDisposed();
			base.ThrowIfReadOnly("MimePart.UpdateTransferEncoding");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				byte[] encodingName = MimePart.GetEncodingName(transferEncoding);
				if (encodingName == null)
				{
					throw new ArgumentException("Transfer encoding is unknown or not supported", "transferEncoding");
				}
				Header header = this.headers.FindFirst(HeaderId.ContentTransferEncoding);
				if (header == null)
				{
					header = Header.Create(HeaderId.ContentTransferEncoding);
					this.headers.InternalAppendChild(header);
				}
				else if (ContentTransferEncoding.SevenBit == transferEncoding)
				{
					header.RemoveFromParent();
					return;
				}
				header.RawValue = encodingName;
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("MimePart");
			}
		}

		private static readonly MimePart.EncodingEntry[] EncodingMap = new MimePart.EncodingEntry[]
		{
			new MimePart.EncodingEntry(MimeString.Base64, ContentTransferEncoding.Base64),
			new MimePart.EncodingEntry(MimeString.QuotedPrintable, ContentTransferEncoding.QuotedPrintable),
			new MimePart.EncodingEntry(MimeString.Encoding7Bit, ContentTransferEncoding.SevenBit),
			new MimePart.EncodingEntry(MimeString.Encoding8Bit, ContentTransferEncoding.EightBit),
			new MimePart.EncodingEntry(MimeString.Binary, ContentTransferEncoding.Binary),
			new MimePart.EncodingEntry(MimeString.Uuencode, ContentTransferEncoding.UUEncode),
			new MimePart.EncodingEntry(MimeString.MacBinhex, ContentTransferEncoding.BinHex),
			new MimePart.EncodingEntry(MimeString.XUuencode, ContentTransferEncoding.UUEncode),
			new MimePart.EncodingEntry(MimeString.Uue, ContentTransferEncoding.UUEncode)
		};

		private MimePart.MimePartThreadAccessToken accessToken;

		private MimeDocument parentDocument;

		private HeaderList headers;

		private DataStorage dataStorage;

		private DataStorage deferredStorage;

		private MimePart.DataStorageInfo storageInfo = new MimePart.DataStorageInfo();

		private MimePart.DataStorageInfo deferredStorageInfo;

		private object deferredStorageLock = new object();

		private bool isDisposed;

		private bool contentDirty;

		private bool contentPersisted;

		private int version;

		private List<string> protectedHeaders;

		private int cacheMapStamp;

		private byte[] boundary;

		private class MimePartThreadAccessToken : ObjectThreadAccessToken
		{
			internal MimePartThreadAccessToken(MimePart parent)
			{
			}
		}

		private class DataStorageInfo
		{
			public long DataStart;

			public long DataEnd;

			public long BodyOffset;

			public ContentTransferEncoding BodyCte;

			public LineTerminationState BodyLineTermination;
		}

		[Flags]
		public enum SubtreeEnumerationOptions : byte
		{
			None = 0,
			IncludeEmbeddedMessages = 1,
			RevisitParent = 2
		}

		public struct PartSubtree : IEnumerable<MimePart>, IEnumerable
		{
			internal PartSubtree(MimePart part)
			{
				this.part = part;
			}

			public MimePart.SubtreeEnumerator GetEnumerator()
			{
				return new MimePart.SubtreeEnumerator(this.part, MimePart.SubtreeEnumerationOptions.None, true);
			}

			public MimePart.SubtreeEnumerator GetEnumerator(MimePart.SubtreeEnumerationOptions options)
			{
				return new MimePart.SubtreeEnumerator(this.part, options, true);
			}

			internal MimePart.SubtreeEnumerator GetEnumerator(MimePart.SubtreeEnumerationOptions options, bool includeUnparsed)
			{
				return new MimePart.SubtreeEnumerator(this.part, options, includeUnparsed);
			}

			IEnumerator<MimePart> IEnumerable<MimePart>.GetEnumerator()
			{
				return new MimePart.SubtreeEnumerator(this.part, MimePart.SubtreeEnumerationOptions.None, true);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new MimePart.SubtreeEnumerator(this.part, MimePart.SubtreeEnumerationOptions.None, true);
			}

			private MimePart part;
		}

		public struct SubtreeEnumerator : IEnumerator<MimePart>, IDisposable, IEnumerator
		{
			internal SubtreeEnumerator(MimePart part, MimePart.SubtreeEnumerationOptions options, bool includeUnparsed)
			{
				this.options = options;
				this.includeUnparsed = includeUnparsed;
				this.root = part;
				this.current = null;
				this.currentDisposition = (MimePart.SubtreeEnumerator.EnumeratorDisposition)0;
				this.nextChild = part;
				this.depth = -1;
			}

			public MimePart Current
			{
				get
				{
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			public bool FirstVisit
			{
				get
				{
					return 0 != (byte)(this.currentDisposition & MimePart.SubtreeEnumerator.EnumeratorDisposition.Begin);
				}
			}

			public bool LastVisit
			{
				get
				{
					return 0 != (byte)(this.currentDisposition & MimePart.SubtreeEnumerator.EnumeratorDisposition.End);
				}
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			public bool MoveNext()
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.root.AccessToken))
				{
					if (this.nextChild != null)
					{
						this.depth++;
						this.current = this.nextChild;
						if ((byte)(this.options & MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages) == 0)
						{
							if (this.current.IsMultipart)
							{
								if (!this.includeUnparsed)
								{
									this.nextChild = ((this.current.InternalLastChild != null) ? ((MimePart)this.current.FirstChild) : null);
								}
								else
								{
									this.nextChild = (MimePart)this.current.FirstChild;
								}
							}
							else
							{
								this.nextChild = null;
							}
						}
						else if (!this.includeUnparsed)
						{
							this.nextChild = ((this.current.InternalLastChild != null) ? ((MimePart)this.current.FirstChild) : null);
						}
						else
						{
							this.nextChild = (MimePart)this.current.FirstChild;
						}
						this.currentDisposition = (MimePart.SubtreeEnumerator.EnumeratorDisposition)(1 | ((this.nextChild == null) ? 2 : 0));
						result = true;
					}
					else if (this.depth < 0)
					{
						result = false;
					}
					else
					{
						for (;;)
						{
							this.depth--;
							if (this.depth < 0)
							{
								break;
							}
							if (!this.includeUnparsed)
							{
								this.nextChild = (MimePart)this.current.InternalNextSibling;
							}
							else
							{
								this.nextChild = (MimePart)this.current.NextSibling;
							}
							this.current = (MimePart)this.current.Parent;
							this.currentDisposition = ((this.nextChild == null) ? MimePart.SubtreeEnumerator.EnumeratorDisposition.End : ((MimePart.SubtreeEnumerator.EnumeratorDisposition)0));
							if ((byte)(this.options & MimePart.SubtreeEnumerationOptions.RevisitParent) != 0 || this.nextChild != null)
							{
								goto IL_1B8;
							}
						}
						this.current = null;
						this.nextChild = null;
						this.currentDisposition = (MimePart.SubtreeEnumerator.EnumeratorDisposition)0;
						return false;
						IL_1B8:
						result = ((byte)(this.options & MimePart.SubtreeEnumerationOptions.RevisitParent) != 0 || this.MoveNext());
					}
				}
				return result;
			}

			public void SkipChildren()
			{
				if (this.nextChild != null)
				{
					this.nextChild = null;
					this.currentDisposition |= MimePart.SubtreeEnumerator.EnumeratorDisposition.End;
				}
			}

			void IEnumerator.Reset()
			{
				this.current = null;
				this.currentDisposition = (MimePart.SubtreeEnumerator.EnumeratorDisposition)0;
				this.nextChild = this.root;
				this.depth = -1;
			}

			public void Dispose()
			{
				((IEnumerator)this).Reset();
				GC.SuppressFinalize(this);
			}

			private MimePart.SubtreeEnumerationOptions options;

			private bool includeUnparsed;

			private MimePart.SubtreeEnumerator.EnumeratorDisposition currentDisposition;

			private MimePart root;

			private MimePart current;

			private MimePart nextChild;

			private int depth;

			[Flags]
			private enum EnumeratorDisposition : byte
			{
				Begin = 1,
				End = 2
			}
		}

		internal class CountingWriteStream : Stream
		{
			internal CountingWriteStream(Stream stream)
			{
				this.stream = stream;
			}

			public bool IsCountingOnly
			{
				get
				{
					return this.stream == Stream.Null;
				}
			}

			public long Count
			{
				get
				{
					return this.count;
				}
			}

			public void Add(long value)
			{
				this.count += value;
			}

			public override bool CanRead
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
					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
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

			public override int Read(byte[] array, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] array, int offset, int count)
			{
				this.count += (long)count;
				this.stream.Write(array, offset, count);
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				this.stream.Flush();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private long count;

			private Stream stream;
		}

		private class PartContentWriteStream : AppendStreamOnDataStorage
		{
			public PartContentWriteStream(MimePart part, ContentTransferEncoding cte) : base(new TemporaryDataStorage())
			{
				this.part = part;
				this.cte = cte;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && this.part != null)
				{
					using (ThreadAccessGuard.EnterPublic(this.part.AccessToken))
					{
						ReadableDataStorage readableWritableStorage = base.ReadableWritableStorage;
						readableWritableStorage.AddRef();
						base.Dispose(disposing);
						if (!this.part.isDisposed)
						{
							this.part.SetStorage(readableWritableStorage, 0L, readableWritableStorage.Length, 0L, this.cte, LineTerminationState.Unknown);
						}
						readableWritableStorage.Release();
					}
					this.part = null;
					return;
				}
				base.Dispose(disposing);
			}

			private MimePart part;

			private ContentTransferEncoding cte;
		}

		private struct EncodingEntry
		{
			internal EncodingEntry(byte[] name, ContentTransferEncoding type)
			{
				this.Name = name;
				this.Type = type;
			}

			internal byte[] Name;

			internal ContentTransferEncoding Type;
		}
	}
}
