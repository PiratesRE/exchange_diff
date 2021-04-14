using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal sealed class MimeCacheMap
	{
		internal static bool UseInvalidDataException
		{
			set
			{
				MimeCacheMap.useInvalidDataException = value;
			}
		}

		public static long SmallMessageSizeThreshold
		{
			get
			{
				if (MimeCacheMap.smallMessageSizeThreshold == -1L)
				{
					lock (MimeCacheMap.configurationLockObject)
					{
						if (MimeCacheMap.smallMessageSizeThreshold == -1L)
						{
							IList<CtsConfigurationSetting> configuration = ApplicationServices.Provider.GetConfiguration(null);
							int num = 65536;
							foreach (CtsConfigurationSetting ctsConfigurationSetting in configuration)
							{
								if (ctsConfigurationSetting.Name.Equals("MimeCacheMapThreshold", StringComparison.OrdinalIgnoreCase))
								{
									num = ApplicationServices.ParseIntegerSetting(ctsConfigurationSetting, num, 0, true);
									break;
								}
							}
							MimeCacheMap.smallMessageSizeThreshold = (long)num;
						}
					}
				}
				return MimeCacheMap.smallMessageSizeThreshold;
			}
		}

		public static void RefreshConfiguration()
		{
			MimeCacheMap.smallMessageSizeThreshold = -1L;
		}

		public static Serialized Create(MimeDocument document, Stream cacheStream, CreateFixedStream createFixedStream, ReOpenFixedStream reopenFixedStream, long smallMessageSizeThreshold, out long messageSize)
		{
			return MimeCacheMap.CreateOrUpdate(document, cacheStream, createFixedStream, reopenFixedStream, smallMessageSizeThreshold, true, out messageSize);
		}

		public static Serialized Update(MimeDocument document, Stream cacheStream, CreateFixedStream createFixedStream, ReOpenFixedStream reopenFixedStream, long smallMessageSizeThreshold, out long messageSize)
		{
			return MimeCacheMap.CreateOrUpdate(document, cacheStream, createFixedStream, reopenFixedStream, smallMessageSizeThreshold, false, out messageSize);
		}

		public static MimeDocument Load(Stream cacheStream, CreateFixedStream createFixedStream, DecodingOptions decodingOptions)
		{
			if (cacheStream == null)
			{
				throw new ArgumentNullException("cacheStream");
			}
			if (createFixedStream == null)
			{
				throw new ArgumentNullException("createFixedStream");
			}
			if (!cacheStream.CanRead || !cacheStream.CanSeek)
			{
				throw new NotSupportedException(InternalStrings.CacheStreamCannotReadOrSeek);
			}
			Stream stream = null;
			MimeDocument result;
			try
			{
				stream = createFixedStream();
				if (stream == null)
				{
					throw new InvalidOperationException(InternalStrings.FixedStreamIsNull);
				}
				if (!stream.CanRead || !stream.CanSeek)
				{
					throw new NotSupportedException(InternalStrings.FixedStreamCannotReadOrSeek);
				}
				MimeDocument mimeDocument = new MimeDocument(decodingOptions, MimeLimits.Unlimited);
				if (0L == cacheStream.Length)
				{
					stream.Position = 0L;
					mimeDocument.Load(stream, CachingMode.Copy);
				}
				else
				{
					using (MimeCacheMap.MapLoadStream mapLoadStream = new MimeCacheMap.MapLoadStream(cacheStream))
					{
						mimeDocument.CreateValidateStorage = false;
						mimeDocument.Load(mapLoadStream, CachingMode.Source);
						mimeDocument.CreateValidateStorage = true;
						int num = Interlocked.Increment(ref MimeCacheMap.nextStamp);
						if (num == 0)
						{
							num = Interlocked.Increment(ref MimeCacheMap.nextStamp);
						}
						int num2 = 0;
						ReadableDataStorageOnStream readableDataStorageOnStream = null;
						try
						{
							readableDataStorageOnStream = new ReadableDataStorageOnStream(stream, true);
							foreach (MimePart mimePart in mimeDocument.RootPart.Subtree)
							{
								mapLoadStream.Resolve(mimePart, num2, readableDataStorageOnStream);
								num2++;
								mimePart.CacheMapStamp = num;
								mimePart.ContentPersisted = true;
							}
							if (mapLoadStream.PartCount != num2)
							{
								MimeCacheMap.SignalDataCorruption(1);
							}
							stream = null;
						}
						finally
						{
							if (readableDataStorageOnStream != null)
							{
								readableDataStorageOnStream.Release();
								readableDataStorageOnStream = null;
							}
						}
					}
				}
				result = mimeDocument;
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
			}
			return result;
		}

		public static Stream Merge(Stream cacheStream, Stream fixedStream)
		{
			if (cacheStream == null)
			{
				throw new ArgumentNullException("cacheStream");
			}
			if (fixedStream == null)
			{
				throw new ArgumentNullException("fixedStream");
			}
			if (!cacheStream.CanRead || !cacheStream.CanSeek)
			{
				throw new NotSupportedException(InternalStrings.CacheStreamCannotReadOrSeek);
			}
			if (!fixedStream.CanRead || !fixedStream.CanSeek)
			{
				throw new NotSupportedException(InternalStrings.FixedStreamCannotReadOrSeek);
			}
			if (0L != cacheStream.Length)
			{
				return new MimeCacheMap.MapMergeStream(cacheStream, fixedStream);
			}
			return fixedStream;
		}

		private static Serialized CreateOrUpdate(MimeDocument document, Stream cacheStream, CreateFixedStream createFixedStream, ReOpenFixedStream reopenFixedStream, long smallMessageSizeThreshold, bool create, out long messageSize)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			if (document.RootPart == null)
			{
				throw new ArgumentNullException("document.RootPart");
			}
			if (cacheStream == null)
			{
				throw new ArgumentNullException("cacheStream");
			}
			if (createFixedStream == null)
			{
				throw new ArgumentNullException("createFixedStream");
			}
			if (reopenFixedStream == null)
			{
				throw new ArgumentNullException("reopenFixedStream");
			}
			if (!cacheStream.CanWrite)
			{
				throw new NotSupportedException(InternalStrings.CacheStreamCannotWrite);
			}
			Stream stream = null;
			Serialized result;
			try
			{
				stream = createFixedStream();
				if (stream == null)
				{
					throw new InvalidOperationException(InternalStrings.FixedStreamIsNull);
				}
				if (!stream.CanWrite)
				{
					throw new NotSupportedException(InternalStrings.FixedStreamCannotWrite);
				}
				if (MimeCacheMap.IsContentRelativelySmall(create, document, cacheStream, smallMessageSizeThreshold))
				{
					stream.SetLength(0L);
					document.RootPart.WriteTo(stream);
					stream.Flush();
					messageSize = stream.Length;
					cacheStream.SetLength(0L);
					result = Serialized.Sequential;
				}
				else
				{
					using (MimeCacheMap.MapOutputTeeStream mapOutputTeeStream = new MimeCacheMap.MapOutputTeeStream(cacheStream, stream, create))
					{
						int num;
						if (create)
						{
							num = Interlocked.Increment(ref MimeCacheMap.nextStamp);
							if (num == 0)
							{
								num = Interlocked.Increment(ref MimeCacheMap.nextStamp);
							}
						}
						else if (document.RootPart.CacheMapStamp != 0)
						{
							num = document.RootPart.CacheMapStamp;
						}
						else
						{
							num = Interlocked.Increment(ref MimeCacheMap.nextStamp);
							if (num == 0)
							{
								num = Interlocked.Increment(ref MimeCacheMap.nextStamp);
							}
						}
						mapOutputTeeStream.CacheMapStamp = num;
						mapOutputTeeStream.ProcessTree(document.RootPart, null, false);
						mapOutputTeeStream.Flush();
						messageSize = mapOutputTeeStream.MessageSize;
						reopenFixedStream(stream);
						stream = null;
					}
					result = Serialized.NonSequential;
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
			}
			return result;
		}

		private static bool IsContentRelativelySmall(bool create, MimeDocument document, Stream cacheStream, long smallMessageSizeThreshold)
		{
			if (create)
			{
				return smallMessageSizeThreshold > document.ParsedSize;
			}
			bool result = true;
			if (0L < cacheStream.Length)
			{
				cacheStream.SetLength(0L);
				result = false;
			}
			return result;
		}

		private static void SignalDataCorruption(int checkNumber)
		{
			string message = InternalStrings.CorruptMapData(checkNumber);
			if (MimeCacheMap.useInvalidDataException)
			{
				throw new InvalidDataException(message);
			}
			throw new MimeException(message);
		}

		private const ushort CacheMapVersion = 256;

		private const int BufferSize = 4096;

		private const int EntryLengthFieldSize = 2;

		private const int CacheLengthFieldSize = 4;

		private const int MaxMapEntrySize = 15;

		private static int nextStamp = 1;

		private static long smallMessageSizeThreshold = -1L;

		private static object configurationLockObject = new object();

		private static bool useInvalidDataException;

		internal enum EntryTag
		{
			HeadersStart,
			LeafBodyStartEnd,
			ChildrenStart,
			ChildrenEnd,
			Continuation,
			Version
		}

		internal abstract class MapBaseStream : Stream
		{
			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanWrite
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
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}
		}

		internal sealed class MapOutputTeeStream : MimeCacheMap.MapBaseStream
		{
			public MapOutputTeeStream(Stream cacheStream, Stream fixedStream, bool create)
			{
				this.create = create;
				this.writeToFixed = create;
				this.fixedStorage = new ReadableWritableDataStorageOnStream(fixedStream, true);
				this.fixedStream = this.fixedStorage.OpenWriteStream(true);
				this.cacheStream = cacheStream;
				this.WriteEntryTag(MimeCacheMap.EntryTag.Version, null);
			}

			protected override void Dispose(bool disposing)
			{
				if (this.fixedStream != null)
				{
					this.fixedStream.Dispose();
					this.fixedStream = null;
				}
				if (this.fixedStorage != null)
				{
					this.fixedStorage.Release();
					this.fixedStorage = null;
				}
				base.Dispose(disposing);
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

			public int CacheMapStamp
			{
				set
				{
					this.cacheMapStamp = value;
				}
			}

			public long MessageSize
			{
				get
				{
					return this.messageSize;
				}
			}

			private bool IsInsideOfClearSigned
			{
				get
				{
					return long.MaxValue != this.clearSignedBaseDelta;
				}
			}

			public override void Write(byte[] array, int offset, int count)
			{
				if (count > 0)
				{
					this.WriteToCacheStream(array, offset, count);
					if (this.writeToFixed)
					{
						this.fixedStream.Write(array, offset, count);
					}
				}
			}

			public override void Flush()
			{
				if (this.bufferPosition > 0)
				{
					this.FlushBuffer(0);
				}
				this.cacheStream.Flush();
				this.fixedStream.Flush();
			}

			public void ProcessTree(MimePart part, byte[] parentBoundary, bool clearSigned)
			{
				using (MimePart.SubtreeEnumerator enumerator = part.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.RevisitParent, false))
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.FirstVisit)
						{
							if (enumerator.Current.IsSignedContent && !clearSigned)
							{
								this.ProcessClearSigned(enumerator.Current, parentBoundary);
								enumerator.SkipChildren();
							}
							else
							{
								if (parentBoundary != null)
								{
									this.Write(parentBoundary, 0, parentBoundary.Length);
								}
								this.ProcessPart(enumerator.Current, clearSigned);
								if (!clearSigned)
								{
									this.writeToFixed = this.create;
								}
								if (enumerator.Current.IsMultipart && !enumerator.LastVisit)
								{
									parentBoundary = enumerator.Current.Boundary;
								}
							}
						}
						else if (enumerator.LastVisit)
						{
							if (parentBoundary != null)
							{
								this.Write(parentBoundary, 0, parentBoundary.Length - 2);
								this.Write(MimeString.TwoDashesCRLF, 0, MimeString.TwoDashesCRLF.Length);
							}
							this.WriteEntryTag(MimeCacheMap.EntryTag.ChildrenEnd, enumerator.Current);
							part.ContentPersisted = true;
							parentBoundary = ((enumerator.Depth == 0) ? null : ((MimePart)enumerator.Current.Parent).Boundary);
							if (!clearSigned)
							{
								this.writeToFixed = this.create;
							}
						}
					}
				}
			}

			private void ProcessClearSigned(MimePart node, byte[] parentBoundary)
			{
				if (this.create)
				{
					this.fixedStream.Write(parentBoundary, 0, parentBoundary.Length);
				}
				else
				{
					this.writeToFixed = this.IsStampInvalid(node);
				}
				long num = node.DeferredDataEnd - node.DeferredDataStart;
				if (this.writeToFixed)
				{
					this.clearSignedBaseDelta = this.fixedStream.End - node.DeferredDataStart;
					if (num != 0L)
					{
						num = node.DeferredStorage.CopyContentToStream(node.DeferredDataStart, node.DeferredDataEnd, this.fixedStream, ref this.scratchBuffer);
					}
					this.writeToFixed = false;
				}
				else
				{
					this.clearSignedBaseDelta = 0L;
				}
				this.messageSize += num + (long)parentBoundary.Length;
				this.ProcessTree(node, parentBoundary, true);
				this.clearSignedBaseDelta = long.MaxValue;
				this.writeToFixed = this.create;
			}

			private void ProcessPart(MimePart part, bool clearSigned)
			{
				if (clearSigned && (this.create || this.IsStampInvalid(part)))
				{
					part.SetDeferredStorageImpl(this.fixedStorage, part.DeferredDataStart + this.clearSignedBaseDelta, part.DeferredDataEnd + this.clearSignedBaseDelta, part.DeferredBodyOffset, part.DeferredBodyCte, part.DeferredBodyLineTermination);
					part.CacheMapStamp = this.cacheMapStamp;
				}
				this.WriteEntryTag(MimeCacheMap.EntryTag.HeadersStart, part);
				part.Headers.WriteTo(this);
				bool isMultipart = part.IsMultipart;
				if (isMultipart)
				{
					if (!clearSigned)
					{
						part.SetDeferredStorageImpl(null, 0L, 0L);
						part.CacheMapStamp = this.cacheMapStamp;
					}
					if (part.HasChildren)
					{
						this.WriteEntryTag(MimeCacheMap.EntryTag.ChildrenStart, part);
						return;
					}
					this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
					this.leafPartBodyStart = this.fixedStream.End;
					this.WriteEntryTag(MimeCacheMap.EntryTag.LeafBodyStartEnd, part);
					this.leafPartBodyStart = -1L;
					return;
				}
				else
				{
					if (clearSigned)
					{
						if (part.IsEmbeddedMessage && part.InternalLastChild != null && part.DeferredStorage == null && part.FirstChild is MimePart)
						{
							MimePart mimePart = part.FirstChild as MimePart;
							if (mimePart.DeferredStorage != null)
							{
								ContentTransferEncoding contentTransferEncoding = part.ContentTransferEncoding;
								if (!MimePart.EncodingIsTransparent(contentTransferEncoding))
								{
									contentTransferEncoding = ContentTransferEncoding.SevenBit;
								}
								part.SetDeferredStorageImpl(this.fixedStorage, mimePart.DeferredDataStart, mimePart.DeferredDataStart + mimePart.DeferredDataLength, 0L, contentTransferEncoding, LineTerminationState.CRLF);
								part.CacheMapStamp = this.cacheMapStamp;
								part.ContentPersisted = true;
							}
						}
						this.WriteToCacheStream(MimeString.CrLf, 0, MimeString.CrLf.Length);
						this.WriteEntryTag(MimeCacheMap.EntryTag.LeafBodyStartEnd, part);
						return;
					}
					if (!this.create)
					{
						this.writeToFixed = (this.IsStampInvalid(part) || !part.ContentPersisted);
					}
					this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
					this.leafPartBodyStart = this.fixedStream.End;
					if (this.writeToFixed)
					{
						LineTerminationState lineTerminationState = LineTerminationState.CRLF;
						ContentTransferEncoding contentTransferEncoding2 = part.ContentTransferEncoding;
						long num;
						if (part.IsEmbeddedMessage && part.InternalLastChild != null && part.DeferredStorage == null)
						{
							num = part.FirstChild.WriteTo(this.fixedStream);
							if (!MimePart.EncodingIsTransparent(contentTransferEncoding2))
							{
								contentTransferEncoding2 = ContentTransferEncoding.SevenBit;
							}
						}
						else
						{
							num = 0L;
							if (part.DeferredDataEnd == 9223372036854775807L || part.DeferredDataEnd != part.DeferredDataStart + part.DeferredBodyOffset)
							{
								if (MimePart.IsEqualContentTransferEncoding(part.DeferredBodyCte, contentTransferEncoding2))
								{
									num = MimePart.CopyStorageToStream(part.DeferredStorage, part.DeferredDataStart + part.DeferredBodyOffset, part.DeferredDataEnd, part.DeferredBodyLineTermination, this.fixedStream, ref this.scratchBuffer, ref lineTerminationState);
								}
								else
								{
									if (this.scratchBuffer == null || this.scratchBuffer.Length < 4096)
									{
										this.scratchBuffer = new byte[4096];
									}
									using (Stream deferredRawContentReadStream = part.GetDeferredRawContentReadStream())
									{
										for (;;)
										{
											int num2 = deferredRawContentReadStream.Read(this.scratchBuffer, 0, this.scratchBuffer.Length);
											if (num2 == 0)
											{
												break;
											}
											if (lineTerminationState != LineTerminationState.NotInteresting)
											{
												lineTerminationState = MimeCommon.AdvanceLineTerminationState(lineTerminationState, this.scratchBuffer, 0, num2);
											}
											this.fixedStream.Write(this.scratchBuffer, 0, num2);
											num += (long)num2;
										}
									}
								}
							}
						}
						part.SetDeferredStorageImpl(this.fixedStorage, this.leafPartBodyStart, this.leafPartBodyStart + num, 0L, contentTransferEncoding2, lineTerminationState);
						part.CacheMapStamp = this.cacheMapStamp;
						part.ContentPersisted = true;
					}
					this.WriteEntryTag(MimeCacheMap.EntryTag.LeafBodyStartEnd, part);
					this.leafPartBodyStart = -1L;
					this.writeToFixed = this.create;
					return;
				}
			}

			private void WriteToCacheStream(byte[] array, int offset, int count)
			{
				if (!this.IsInsideOfClearSigned)
				{
					this.messageSize += (long)count;
				}
				int num = 4096 - this.bufferPosition;
				if (num > 0)
				{
					if (num > count)
					{
						num = count;
					}
					Buffer.BlockCopy(array, offset, this.buffer, this.bufferPosition, num);
					this.bufferPosition += num;
					if (count == num)
					{
						return;
					}
					offset += num;
					count -= num;
				}
				this.FlushBuffer(count);
				if (count >= 4081)
				{
					this.cacheStream.Write(array, offset, count);
				}
				else if (count != 0)
				{
					Buffer.BlockCopy(array, offset, this.buffer, 0, count);
					this.bufferPosition = count;
				}
				this.WriteEntryTag(MimeCacheMap.EntryTag.Continuation, null);
			}

			private void WriteEntryTag(MimeCacheMap.EntryTag tagID, MimePart part)
			{
				if (4096 - this.bufferPosition < 15)
				{
					this.FlushBuffer(0);
				}
				else if (MimeCacheMap.EntryTag.Continuation != tagID)
				{
					this.Write4((long)(this.bufferPosition - this.cacheLengthWritePosition - 4), ref this.cacheLengthWritePosition);
				}
				this.buffer[this.bufferPosition++] = (byte)tagID;
				int num = this.bufferPosition;
				this.bufferPosition += 2;
				int num2 = this.bufferPosition;
				bool flag = false;
				long num3 = 0L;
				switch (tagID)
				{
				case MimeCacheMap.EntryTag.HeadersStart:
					if (0L == part.DeferredBodyOffset)
					{
						num3 = -1L;
					}
					flag = true;
					break;
				case MimeCacheMap.EntryTag.LeafBodyStartEnd:
					if (this.writeToFixed)
					{
						this.Write4(this.leafPartBodyStart, ref this.bufferPosition);
						this.Write4(this.fixedStream.End, ref this.bufferPosition);
						if (!this.IsInsideOfClearSigned)
						{
							this.messageSize += this.fixedStream.End - this.leafPartBodyStart;
						}
					}
					else if (part.DeferredStorage != null)
					{
						long deferredDataLength = part.DeferredDataLength;
						long num4 = part.DeferredDataStart + part.DeferredBodyOffset;
						long num5 = num4 + deferredDataLength;
						this.Write4((long)((int)num4), ref this.bufferPosition);
						this.Write4((long)((int)num5), ref this.bufferPosition);
						if (!this.IsInsideOfClearSigned)
						{
							this.messageSize += deferredDataLength;
						}
					}
					else
					{
						this.Write4(this.leafPartBodyStart, ref this.bufferPosition);
						this.Write4(this.leafPartBodyStart, ref this.bufferPosition);
					}
					break;
				case MimeCacheMap.EntryTag.ChildrenStart:
					num3 = part.DeferredBodyOffset;
					flag = true;
					break;
				case MimeCacheMap.EntryTag.ChildrenEnd:
					if (part.DeferredStorage != null)
					{
						num3 = part.DeferredDataEnd - part.DeferredDataStart;
					}
					flag = true;
					break;
				case MimeCacheMap.EntryTag.Version:
					this.Write2(256, ref this.bufferPosition);
					break;
				}
				if (flag)
				{
					if (this.IsInsideOfClearSigned)
					{
						this.Write4((long)((int)(part.DeferredDataStart + num3)), ref this.bufferPosition);
					}
					else if (this.create)
					{
						this.Write4(this.fixedStream.End, ref this.bufferPosition);
					}
					else if (0 < part.Version || this.IsStampInvalid(part) || !part.ContentPersisted || -1L == num3 || part.DeferredStorage == null)
					{
						this.Write4(-1L, ref this.bufferPosition);
					}
					else
					{
						this.Write4((long)((int)(part.DeferredDataStart + num3)), ref this.bufferPosition);
					}
				}
				this.Write2((ushort)(this.bufferPosition - num2), ref num);
				this.cacheLengthWritePosition = this.bufferPosition;
				this.bufferPosition += 4;
			}

			private void FlushBuffer(int extraDataLength)
			{
				this.Write4((long)(this.bufferPosition - this.cacheLengthWritePosition - 4 + extraDataLength), ref this.cacheLengthWritePosition);
				this.cacheStream.Write(this.buffer, 0, this.bufferPosition);
				this.bufferPosition = 0;
			}

			private void Write4(long value, ref int offset)
			{
				this.buffer[offset++] = (byte)value;
				this.buffer[offset++] = (byte)(value >> 8);
				this.buffer[offset++] = (byte)(value >> 16);
				this.buffer[offset++] = (byte)(value >> 24);
			}

			private void Write2(ushort value, ref int offset)
			{
				this.buffer[offset++] = (byte)value;
				this.buffer[offset++] = (byte)(value >> 8);
			}

			private bool IsStampInvalid(MimePart part)
			{
				return this.cacheMapStamp == 0 || this.cacheMapStamp != part.CacheMapStamp;
			}

			private byte[] buffer = new byte[4096];

			private byte[] scratchBuffer;

			private int bufferPosition;

			private Stream cacheStream;

			private ReadableWritableDataStorage fixedStorage;

			private StreamOnDataStorage fixedStream;

			private int cacheLengthWritePosition;

			private long leafPartBodyStart = -1L;

			private long clearSignedBaseDelta = long.MaxValue;

			private bool writeToFixed;

			private bool create;

			private int cacheMapStamp;

			private long messageSize;
		}

		internal abstract class MapReadStream : MimeCacheMap.MapBaseStream
		{
			protected MapReadStream(Stream cacheStream)
			{
				this.cacheStream = cacheStream;
				this.cacheStartPosition = this.cacheStream.Position;
				int num;
				if (MimeCacheMap.EntryTag.Version != this.ReadTag(out num))
				{
					MimeCacheMap.SignalDataCorruption(1);
				}
				if (256 != this.Read2(num))
				{
					throw new MimeException(InternalStrings.UnsupportedMapDataVersion);
				}
			}

			public override long Position
			{
				get
				{
					if (0L == this.cacheStream.Position)
					{
						return 0L;
					}
					throw new NotSupportedException(InternalStrings.NonZeroPositionDoesntMakeSense);
				}
				set
				{
					if (0L != value || !this.cacheStream.CanSeek)
					{
						throw new NotSupportedException(InternalStrings.SettingPositionDoesntMakeSense);
					}
					this.Reset();
				}
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				if (origin != SeekOrigin.Begin || 0L != offset)
				{
					throw new NotSupportedException(InternalStrings.SeekingSupportedToBeginningOnly);
				}
				this.Position = 0L;
				return 0L;
			}

			internal MimeCacheMap.EntryTag ReadTag(out int tagDataOffset)
			{
				if (this.count < 15)
				{
					this.FillBuffer();
					if (this.count == 0 && this.endOfCacheStream)
					{
						tagDataOffset = 0;
						return (MimeCacheMap.EntryTag)(-1);
					}
				}
				if (this.count < 3)
				{
					MimeCacheMap.SignalDataCorruption(1);
				}
				ushort num = this.Read2(this.offset + 1);
				int num2 = (int)(3 + num + 4);
				if (this.count < num2)
				{
					MimeCacheMap.SignalDataCorruption(2);
				}
				MimeCacheMap.EntryTag result = (MimeCacheMap.EntryTag)this.buffer[this.offset];
				tagDataOffset = this.offset + 1 + 2;
				this.offset += num2;
				this.count -= num2;
				this.cacheCount = this.Read4(this.offset - 4);
				return result;
			}

			protected void FillBuffer()
			{
				if (this.count != 0)
				{
					Buffer.BlockCopy(this.buffer, this.offset, this.buffer, 0, this.count);
				}
				for (;;)
				{
					int num = this.cacheStream.Read(this.buffer, this.count, 4096 - this.count);
					if (num == 0)
					{
						break;
					}
					this.count += num;
					if (this.count >= 4096)
					{
						goto IL_71;
					}
				}
				this.endOfCacheStream = true;
				IL_71:
				this.offset = 0;
			}

			protected void MoveCacheOut(byte[] array, ref int offset, ref int count)
			{
				int num = Math.Min(Math.Min(this.count, this.cacheCount), count);
				if (num != 0)
				{
					Buffer.BlockCopy(this.buffer, this.offset, array, offset, num);
					offset += num;
					count -= num;
					this.offset += num;
					this.count -= num;
					this.cacheCount -= num;
				}
			}

			protected int SkipToMatchingEnd()
			{
				int num = 0;
				int num2;
				for (;;)
				{
					if (this.count < this.cacheCount)
					{
						this.cacheStream.Seek((long)(this.cacheCount - this.count), SeekOrigin.Current);
						this.offset = 0;
						this.count = 0;
					}
					else
					{
						this.offset += this.cacheCount;
						this.count -= this.cacheCount;
					}
					this.cacheCount = 0;
					switch (this.ReadTag(out num2))
					{
					case MimeCacheMap.EntryTag.LeafBodyStartEnd:
						if (num == 0)
						{
							goto Block_3;
						}
						break;
					case MimeCacheMap.EntryTag.ChildrenStart:
						num++;
						break;
					case MimeCacheMap.EntryTag.ChildrenEnd:
						num--;
						if (num == 0)
						{
							goto Block_4;
						}
						break;
					}
					if (this.count == 0 && this.endOfCacheStream)
					{
						MimeCacheMap.SignalDataCorruption(1);
					}
				}
				Block_3:
				return this.Read4(num2 + 4);
				Block_4:
				return this.Read4(num2);
			}

			protected int Read4(int offset)
			{
				return (int)this.buffer[offset] | (int)this.buffer[offset + 1] << 8 | (int)this.buffer[offset + 2] << 16 | (int)this.buffer[offset + 3] << 24;
			}

			protected ushort Read2(int offset)
			{
				return (ushort)((int)this.buffer[offset] | (int)this.buffer[offset + 1] << 8);
			}

			protected virtual void Reset()
			{
				this.cacheStream.Position = this.cacheStartPosition;
				this.count = 0;
				this.offset = 0;
				this.cacheCount = 0;
				this.endOfCacheStream = false;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			protected byte[] buffer = new byte[4096];

			protected int count;

			protected int offset;

			protected Stream cacheStream;

			protected int cacheCount;

			protected bool endOfCacheStream;

			private long cacheStartPosition;
		}

		internal sealed class MapLoadStream : MimeCacheMap.MapReadStream
		{
			public MapLoadStream(Stream cacheStream) : base(cacheStream)
			{
			}

			public int PartCount
			{
				get
				{
					return this.list.Count;
				}
			}

			public override int Read(byte[] array, int offset, int count)
			{
				int num = offset;
				while (0 < count)
				{
					base.MoveCacheOut(array, ref offset, ref count);
					if (this.cacheCount == 0)
					{
						int num2;
						switch (base.ReadTag(out num2))
						{
						case MimeCacheMap.EntryTag.HeadersStart:
							this.list.Append(new MimeCacheMap.MapLoadStream.PartOffsetsList.Entry
							{
								HeadersStart = base.Read4(num2)
							});
							break;
						case MimeCacheMap.EntryTag.LeafBodyStartEnd:
						{
							if (this.list.Count == 0)
							{
								MimeCacheMap.SignalDataCorruption(1);
							}
							MimeCacheMap.MapLoadStream.PartOffsetsList.Entry value = this.list[this.list.Count - 1];
							value.BodyStart = base.Read4(num2);
							value.PartEnd = base.Read4(num2 + 4);
							this.list[this.list.Count - 1] = value;
							break;
						}
						case MimeCacheMap.EntryTag.ChildrenStart:
						{
							if (this.list.Count == 0)
							{
								MimeCacheMap.SignalDataCorruption(2);
							}
							MimeCacheMap.MapLoadStream.PartOffsetsList.Entry value = this.list[this.list.Count - 1];
							value.BodyStart = base.Read4(num2);
							this.list[this.list.Count - 1] = value;
							this.indexOffsetsToClose = this.list.Count - 1;
							break;
						}
						case MimeCacheMap.EntryTag.ChildrenEnd:
							this.SetPartEnd(base.Read4(num2));
							break;
						}
					}
					else if (this.count < 15)
					{
						base.FillBuffer();
					}
					if (this.count == 0 && this.endOfCacheStream)
					{
						break;
					}
				}
				if (offset == num && this.cacheCount != 0)
				{
					MimeCacheMap.SignalDataCorruption(3);
				}
				return offset - num;
			}

			public void Resolve(MimePart part, int index, ReadableDataStorage fixedStorage)
			{
				if (index >= this.list.Count)
				{
					MimeCacheMap.SignalDataCorruption(1);
				}
				MimeCacheMap.MapLoadStream.PartOffsetsList.Entry entry = this.list[index];
				if (-1 != entry.BodyStart)
				{
					int num;
					int num2;
					if (-1 != entry.HeadersStart)
					{
						num = entry.HeadersStart;
						num2 = entry.BodyStart - num;
					}
					else
					{
						num = entry.BodyStart;
						num2 = 0;
					}
					if (num > entry.PartEnd)
					{
						MimeCacheMap.SignalDataCorruption(2);
					}
					if (num2 > entry.PartEnd - num)
					{
						MimeCacheMap.SignalDataCorruption(3);
					}
					if ((long)entry.PartEnd > fixedStorage.Length)
					{
						MimeCacheMap.SignalDataCorruption(4);
					}
					part.SetDeferredStorageImpl(fixedStorage, (long)num, (long)entry.PartEnd, (long)num2, part.ContentTransferEncoding, LineTerminationState.Unknown);
				}
			}

			protected override void Reset()
			{
				this.list.Reset();
				this.indexOffsetsToClose = -1;
				base.Reset();
			}

			private void SetPartEnd(int partEnd)
			{
				if (this.indexOffsetsToClose < 0)
				{
					MimeCacheMap.SignalDataCorruption(1);
				}
				while (this.indexOffsetsToClose >= 0 && this.list[this.indexOffsetsToClose].PartEnd != 0)
				{
					this.indexOffsetsToClose--;
				}
				if (this.indexOffsetsToClose < 0)
				{
					MimeCacheMap.SignalDataCorruption(2);
				}
				MimeCacheMap.MapLoadStream.PartOffsetsList.Entry value = this.list[this.indexOffsetsToClose];
				value.PartEnd = partEnd;
				this.list[this.indexOffsetsToClose] = value;
				this.indexOffsetsToClose--;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private MimeCacheMap.MapLoadStream.PartOffsetsList list = default(MimeCacheMap.MapLoadStream.PartOffsetsList);

			private int indexOffsetsToClose = -1;

			private struct PartOffsetsList
			{
				public int Count
				{
					get
					{
						return this.totalOffsets;
					}
				}

				public MimeCacheMap.MapLoadStream.PartOffsetsList.Entry this[int index]
				{
					get
					{
						if (index != 0)
						{
							return this.partOffsets[index - 1];
						}
						return this.rootPartOffsets;
					}
					set
					{
						if (index == 0)
						{
							this.rootPartOffsets = value;
							return;
						}
						this.partOffsets[index - 1] = value;
					}
				}

				public void Append(MimeCacheMap.MapLoadStream.PartOffsetsList.Entry offsets)
				{
					if (this.totalOffsets == 0)
					{
						this.rootPartOffsets = offsets;
					}
					else
					{
						if (this.partOffsets == null)
						{
							this.partOffsets = new MimeCacheMap.MapLoadStream.PartOffsetsList.Entry[6];
						}
						else if (this.totalOffsets - 1 == this.partOffsets.Length)
						{
							MimeCacheMap.MapLoadStream.PartOffsetsList.Entry[] destinationArray = new MimeCacheMap.MapLoadStream.PartOffsetsList.Entry[this.totalOffsets * 2];
							Array.Copy(this.partOffsets, 0, destinationArray, 0, this.totalOffsets - 1);
							this.partOffsets = destinationArray;
						}
						this.partOffsets[this.totalOffsets - 1] = offsets;
					}
					this.totalOffsets++;
				}

				public void Reset()
				{
					this.totalOffsets = 0;
				}

				private MimeCacheMap.MapLoadStream.PartOffsetsList.Entry rootPartOffsets;

				private MimeCacheMap.MapLoadStream.PartOffsetsList.Entry[] partOffsets;

				private int totalOffsets;

				public struct Entry
				{
					public int HeadersStart;

					public int BodyStart;

					public int PartEnd;
				}
			}
		}

		internal sealed class MapMergeStream : MimeCacheMap.MapReadStream
		{
			public MapMergeStream(Stream cacheStream, Stream fixedStream) : base(cacheStream)
			{
				this.fixedStream = fixedStream;
			}

			public override long Length
			{
				get
				{
					if (this.cacheStream == null)
					{
						throw new ObjectDisposedException(InternalStrings.CannotAccessClosedStream);
					}
					if (!this.cacheStream.CanSeek)
					{
						throw new NotSupportedException(InternalStrings.MergedLengthNotSupportedOnNonseekableCacheStream);
					}
					if (this.beingUsed)
					{
						throw new NotSupportedException(InternalStrings.LengthNotSupportedDuringReads);
					}
					this.Reset();
					int num = 0;
					do
					{
						int num2 = Math.Min(this.count, this.cacheCount);
						if (num2 != 0)
						{
							num += num2;
							this.offset += num2;
							this.count -= num2;
							this.cacheCount -= num2;
						}
						if (this.cacheCount == 0)
						{
							int num3;
							switch (base.ReadTag(out num3))
							{
							case MimeCacheMap.EntryTag.HeadersStart:
							{
								int num4 = base.Read4(num3);
								if (-1 != num4)
								{
									num += base.SkipToMatchingEnd() - num4;
								}
								break;
							}
							case MimeCacheMap.EntryTag.LeafBodyStartEnd:
							{
								int num5 = base.Read4(num3);
								num += base.Read4(num3 + 4) - num5;
								break;
							}
							}
						}
						else if (this.count < 15)
						{
							base.FillBuffer();
						}
					}
					while (this.count != 0 || !this.endOfCacheStream);
					if (this.cacheCount != 0)
					{
						MimeCacheMap.SignalDataCorruption(1);
					}
					this.Reset();
					return (long)num;
				}
			}

			public override int Read(byte[] array, int offset, int count)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (offset < 0)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				if (count < 0)
				{
					throw new ArgumentOutOfRangeException("count");
				}
				if (array.Length - offset < count)
				{
					throw new ArgumentException(InternalStrings.ArgumentInvalidOffLen);
				}
				if (this.fixedStream == null)
				{
					throw new ObjectDisposedException(InternalStrings.CannotAccessClosedStream);
				}
				this.beingUsed = true;
				int num = offset;
				while (0 < count)
				{
					if (0 < this.fixedCount)
					{
						int num2 = Math.Min(this.fixedCount, count);
						this.fixedStream.Position = (long)this.fixedPosition;
						this.fixedPosition += num2;
						count -= num2;
						this.fixedCount -= num2;
						while (0 < num2)
						{
							int num3 = this.fixedStream.Read(array, offset, num2);
							if (num3 == 0)
							{
								MimeCacheMap.SignalDataCorruption(1);
							}
							num2 -= num3;
							offset += num3;
						}
						if (count == 0)
						{
							break;
						}
					}
					base.MoveCacheOut(array, ref offset, ref count);
					if (this.cacheCount == 0)
					{
						int num4;
						switch (base.ReadTag(out num4))
						{
						case MimeCacheMap.EntryTag.HeadersStart:
						{
							int num5 = base.Read4(num4);
							if (-1 != num5)
							{
								this.fixedCount = base.SkipToMatchingEnd() - num5;
								if (0 < this.fixedCount)
								{
									this.fixedPosition = num5;
								}
							}
							break;
						}
						case MimeCacheMap.EntryTag.LeafBodyStartEnd:
						{
							int num6 = base.Read4(num4);
							this.fixedCount = base.Read4(num4 + 4) - num6;
							if (0 < this.fixedCount)
							{
								this.fixedPosition = num6;
							}
							break;
						}
						}
					}
					else if (this.count < 15)
					{
						base.FillBuffer();
					}
					if (this.count == 0 && this.endOfCacheStream && this.fixedCount == 0)
					{
						break;
					}
				}
				if (offset == num)
				{
					if (this.cacheCount != 0)
					{
						MimeCacheMap.SignalDataCorruption(2);
					}
					this.beingUsed = false;
				}
				return offset - num;
			}

			protected override void Reset()
			{
				this.fixedCount = 0;
				this.fixedPosition = 0;
				this.beingUsed = false;
				base.Reset();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private Stream fixedStream;

			private int fixedCount;

			private int fixedPosition;

			private bool beingUsed;
		}
	}
}
