using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class MapiStream : MapiBase
	{
		internal MapiStream() : base(MapiObjectType.Stream)
		{
			this.streamSizeInvalid = false;
			this.shouldAllowRead = true;
			this.shouldAllowWrite = true;
			this.usingTempStream = false;
		}

		internal Stream Stream
		{
			get
			{
				return this.stream;
			}
			private set
			{
				this.stream = value;
			}
		}

		public bool StreamSizeInvalid
		{
			get
			{
				return this.streamSizeInvalid;
			}
		}

		public StorePropTag Ptag
		{
			get
			{
				return this.propTag;
			}
		}

		internal CodePage CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		private bool ShouldAllowRead
		{
			get
			{
				return this.shouldAllowRead;
			}
			set
			{
				this.shouldAllowRead = value;
			}
		}

		internal bool ShouldAllowWrite
		{
			get
			{
				return this.shouldAllowWrite;
			}
			private set
			{
				this.shouldAllowWrite = value;
			}
		}

		internal bool ShouldAppend
		{
			get
			{
				return this.shouldAppend;
			}
			private set
			{
				this.shouldAppend = value;
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
			private set
			{
				this.isDirty = value;
			}
		}

		internal bool ReleaseMayNeedExclusiveLock
		{
			get
			{
				return this.shouldAllowWrite && this.IsDirty && (ConfigurationSchema.ConfigurableSharedLockStage.Value < 2 || base.ParentObject.MapiObjectType == MapiObjectType.Folder || base.ParentObject.MapiObjectType == MapiObjectType.Logon || base.ParentObject.MapiObjectType == MapiObjectType.Attachment || base.ParentObject.MapiObjectType == MapiObjectType.EmbeddedMessage);
			}
		}

		public static Stream CreateBufferedStream(Stream stream, int bufferSize)
		{
			BufferPoolCollection.BufferSize bufferSize2;
			BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(bufferSize, out bufferSize2);
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize2);
			return new PooledBufferedStream(stream, bufferPool, bufferSize, true);
		}

		internal ErrorCode ConfigureStream(MapiContext context, MapiStream other, StreamFlags flags, CodePage codePage)
		{
			other.CheckDisposed();
			if (other.propTag.PropType == PropertyType.Binary)
			{
				codePage = CodePage.None;
			}
			if (other.CodePage != codePage)
			{
				return ErrorCode.CreateInvalidParameter((LID)33784U);
			}
			if (flags != StreamFlags.AllowRead)
			{
				return ErrorCode.CreateInvalidParameter((LID)50168U);
			}
			Stream stream = null;
			try
			{
				this.codePage = codePage;
				this.propTag = other.propTag;
				base.ParentObject = other.ParentObject;
				base.Logon = other.Logon;
				this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamOpen, (LID)51047U);
				other.ConvertToTempStreamIfNeeded();
				stream = TempStream.CloneStream(other.Stream);
				this.IsDirty = false;
				stream.Seek(0L, SeekOrigin.Begin);
				this.originalLength = stream.Length;
				this.shouldAppend = false;
				this.shouldAllowRead = true;
				this.shouldAllowWrite = false;
				this.stream = stream;
				stream = null;
				this.usingTempStream = true;
				base.IsValid = true;
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
			}
			return ErrorCode.NoError;
		}

		internal ErrorCode ConfigureStream(MapiContext context, MapiPropBagBase parentPropertyBag, StreamFlags flags, StorePropTag propTag, CodePage codePage)
		{
			if (base.IsDisposed)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "mapiStream:ConfigureStream(): Configure called on a Dispose'd MapiStream! Throwing ExExceptionInvalidObject!");
				return ErrorCode.CreateInvalidObject((LID)48376U);
			}
			if (parentPropertyBag == null || !parentPropertyBag.IsValid || parentPropertyBag.Logon == null || parentPropertyBag.Logon.Session == null)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "mapiStream:ConfigureStream(): parentPropBag is null or invalid, throwing ExExceptionInvalidParameter");
				return ErrorCode.CreateInvalidParameter((LID)64760U);
			}
			if (PropertyType.Unicode != propTag.PropType && PropertyType.Binary != propTag.PropType && PropertyType.Object != propTag.PropType)
			{
				ExTraceGlobals.GeneralTracer.TraceError<StorePropTag>(0L, "mapistream:StorePropTag(): Only ptags for the following types are supported: PT_UNICODE, PT_STRING8, PT_BINARY, PT_OBJECT. Invalid ptag {0}. throwing ExExceptionInvalidParameter", propTag);
				return ErrorCode.CreateInvalidParameter((LID)40184U);
			}
			if (propTag.PropType == PropertyType.Binary)
			{
				codePage = CodePage.None;
			}
			Stream stream = null;
			try
			{
				this.codePage = codePage;
				this.propTag = propTag;
				bool flag = propTag.PropType == PropertyType.Unicode && propTag.ExternalType == PropertyType.String8 && codePage != CodePage.Unicode;
				Encoding encoding;
				if (flag)
				{
					if (!Charset.TryGetEncoding((int)codePage, out encoding))
					{
						ExTraceGlobals.GeneralTracer.TraceError<CodePage>(0L, "MapiStream:CodePage(): Unrecognized or invalid code page id: {0}. Throwing ExExceptionInvalidParameter", codePage);
						DiagnosticContext.TraceDword((LID)34664U, (uint)codePage);
						return ErrorCode.CreateInvalidParameter((LID)46328U);
					}
				}
				else
				{
					encoding = Charset.Unicode.GetEncoding();
				}
				bool flag2;
				if ((StreamFlags.AllowCreate & flags) == (StreamFlags)0)
				{
					Stream stream2 = null;
					try
					{
						ErrorCode dataReader = parentPropertyBag.GetDataReader(context, propTag, out stream2);
						if (dataReader != ErrorCode.NoError && dataReader != ErrorCodeValue.NotSupported)
						{
							return dataReader.Propagate((LID)33528U);
						}
						if (stream2 != null)
						{
							if (flag)
							{
								using (Stream readOnlyConverterStream = MapiStream.GetReadOnlyConverterStream(stream2, CodePage.Unicode, codePage))
								{
									MapiStream.CopyStreamToTempStream(readOnlyConverterStream, out stream);
									flag2 = true;
									goto IL_2CA;
								}
							}
							if (parentPropertyBag is MapiFolder || parentPropertyBag is MapiLogon)
							{
								MapiStream.CopyStreamToTempStream(stream2, out stream);
								flag2 = true;
							}
							else if (!ConfigurationSchema.DisableBypassTempStream.Value)
							{
								int num = (int)stream2.Length;
								int bufferSize;
								if (num <= 16300)
								{
									bufferSize = 16300;
								}
								else if (num <= 32600)
								{
									bufferSize = 32600;
								}
								else
								{
									bufferSize = 81500;
								}
								stream = MapiStream.CreateBufferedStream(stream2, bufferSize);
								stream2 = null;
								flag2 = false;
							}
							else
							{
								MapiStream.CopyStreamToTempStream(stream2, out stream);
								flag2 = true;
							}
						}
						else
						{
							object onePropValue = parentPropertyBag.GetOnePropValue(context, propTag);
							if (onePropValue == null)
							{
								return ErrorCode.CreateNotFound((LID)36088U);
							}
							if (onePropValue is string)
							{
								stream = TempStream.CreateInstance();
								byte[] bytes = encoding.GetBytes((string)onePropValue);
								stream.Write(bytes, 0, bytes.Length);
								flag2 = true;
							}
							else
							{
								stream = TempStream.CreateInstance();
								byte[] array = (byte[])onePropValue;
								stream.Write(array, 0, array.Length);
								flag2 = true;
							}
						}
						IL_2CA:
						goto IL_2EA;
					}
					finally
					{
						if (stream2 != null)
						{
							stream2.Dispose();
							stream2 = null;
						}
					}
				}
				stream = TempStream.CreateInstance();
				this.IsDirty = true;
				flag2 = true;
				IL_2EA:
				stream.Seek(0L, this.shouldAppend ? SeekOrigin.End : SeekOrigin.Begin);
				this.originalLength = stream.Length;
				this.shouldAppend = (StreamFlags.AllowAppend == (StreamFlags.AllowAppend & flags));
				this.shouldAllowRead = (StreamFlags.AllowRead == (StreamFlags.AllowRead & flags));
				this.shouldAllowWrite = (StreamFlags.AllowWrite == (StreamFlags.AllowWrite & flags));
				base.ParentObject = parentPropertyBag;
				base.Logon = parentPropertyBag.Logon;
				this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamOpen, (LID)47975U);
				this.stream = stream;
				stream = null;
				this.usingTempStream = flag2;
				base.IsValid = true;
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
			}
			return ErrorCode.NoError;
		}

		public void Commit(MapiContext context)
		{
			long num = -1L;
			try
			{
				base.ThrowIfNotValid(null);
				if (this.shouldAllowWrite && this.IsDirty && !this.streamSizeInvalid)
				{
					MapiStreamLock.CanAccess((LID)34728U, this, 0UL, 0UL, true);
					if (this.stream.CanSeek)
					{
						num = this.Stream.Position;
						this.stream.Seek(0L, SeekOrigin.Begin);
					}
					this.stream.Flush();
					bool flag = this.propTag.PropType == PropertyType.Unicode && this.propTag.ExternalType == PropertyType.String8 && this.codePage != CodePage.Unicode;
					Stream stream;
					ErrorCode dataWriter = base.ParentObject.GetDataWriter(context, this.propTag, out stream);
					if (dataWriter != ErrorCode.NoError && dataWriter != ErrorCodeValue.NotSupported)
					{
						throw new StoreException((LID)49912U, dataWriter);
					}
					using (stream)
					{
						if (stream != null)
						{
							if (flag)
							{
								using (Stream writeOnlyConverterStream = MapiStream.GetWriteOnlyConverterStream(stream, this.CodePage, CodePage.Unicode))
								{
									MapiStream.CopyStream(this.stream, writeOnlyConverterStream);
									goto IL_203;
								}
							}
							MapiStream.CopyStream(this.stream, stream);
						}
						else
						{
							object value;
							if (this.propTag.PropType == PropertyType.Unicode)
							{
								TextToText textToText = new TextToText(TextToTextConversionMode.ConvertCodePageOnly);
								textToText.InputEncoding = (flag ? Charset.GetEncoding((int)this.codePage) : Charset.Unicode.GetEncoding());
								TextReader textReader = new ConverterReader(this.Stream, textToText);
								value = textReader.ReadToEnd();
							}
							else
							{
								byte[] array = new byte[this.Stream.Length];
								int num2;
								for (int i = 0; i < array.Length; i += num2)
								{
									num2 = this.Stream.Read(array, i, array.Length - i);
								}
								value = array;
							}
							ErrorCode errorCode = base.ParentObject.SetOneProp(context, this.propTag, value);
							if (errorCode != ErrorCode.NoError)
							{
								throw new StoreException((LID)36624U, errorCode);
							}
						}
						IL_203:;
					}
					this.IsDirty = false;
				}
			}
			finally
			{
				if (num != -1L)
				{
					this.Stream.Seek(num, SeekOrigin.Begin);
				}
			}
		}

		public ErrorCode Clone(MapiContext context, out MapiStream stream)
		{
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty | FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.StreamRead, (LID)64359U);
			stream = null;
			MapiStream mapiStream = null;
			base.ThrowIfNotValid(null);
			ErrorCode first = base.ParentObject.OpenStream(context, StreamFlags.AllowCreate | (this.shouldAppend ? StreamFlags.AllowAppend : ((StreamFlags)0)) | StreamFlags.AllowRead | StreamFlags.AllowWrite, this.propTag, this.CodePage, out mapiStream);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)35712U);
			}
			mapiStream.ConvertToTempStreamIfNeeded();
			if (!this.ShouldAllowRead || !mapiStream.ShouldAllowWrite)
			{
				return ErrorCode.CreateNoAccess((LID)56568U);
			}
			if (this.StreamSizeInvalid || mapiStream.StreamSizeInvalid)
			{
				return ErrorCode.CreateMaxSubmissionExceeded((LID)35228U);
			}
			if (this.Stream != null)
			{
				long position = this.Stream.Position;
				this.Stream.Flush();
				MapiStream.CopyStream(this.Stream, mapiStream.Stream);
				mapiStream.Stream.Flush();
				mapiStream.Seek(context, 0L, SeekOrigin.Begin);
				mapiStream.IsDirty = true;
				this.Seek(context, position, SeekOrigin.Begin);
			}
			stream = mapiStream;
			return ErrorCode.NoError;
		}

		internal void CopyTo(MapiContext context, MapiStream destinationStream, long numBytesToCopy, out long numBytesRead, out long numBytesWritten)
		{
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamRead, (LID)39783U);
			destinationStream.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.StreamWrite, (LID)56167U);
			numBytesRead = 0L;
			numBytesWritten = 0L;
			long num = -1L;
			long num2 = -1L;
			destinationStream.ConvertToTempStreamIfNeeded();
			try
			{
				base.ThrowIfNotValid(null);
				if (!this.ShouldAllowRead || !destinationStream.ShouldAllowWrite)
				{
					throw new ExExceptionAccessDenied((LID)44280U, "this didn't allow read, destination didn't allow write!");
				}
				if (this.StreamSizeInvalid || destinationStream.StreamSizeInvalid)
				{
					throw new ExExceptionMaxSubmissionExceeded((LID)45468U, "Exceeded the size limitation");
				}
				if (this.Stream != null)
				{
					num = this.Stream.Position;
					num2 = destinationStream.Stream.Position;
					int num3 = (int)numBytesToCopy;
					if ((long)num3 > this.Stream.Length)
					{
						num3 = (int)this.Stream.Length;
					}
					byte[] buffer = new byte[num3];
					this.Stream.Flush();
					this.Stream.Seek(0L, SeekOrigin.Begin);
					this.Stream.Read(buffer, 0, num3);
					numBytesRead = (long)((int)this.Stream.Position);
					destinationStream.Stream.Write(buffer, 0, (int)numBytesRead);
					destinationStream.Stream.Flush();
					destinationStream.IsDirty = true;
					numBytesWritten = (long)((int)(destinationStream.Stream.Position - num2));
				}
			}
			finally
			{
				if (num != -1L && this.Stream != null)
				{
					this.Stream.Seek(num, SeekOrigin.Begin);
				}
				if (num2 != -1L)
				{
					destinationStream.Stream.Seek(num2, SeekOrigin.Begin);
				}
			}
		}

		public int Read(MapiContext context, byte[] buffer, int offset, int count)
		{
			if (!this.ShouldAllowRead)
			{
				throw new ExExceptionAccessDenied((LID)52472U, "Stream does not allow read -- throwing!");
			}
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamRead, (LID)43879U);
			MapiStreamLock.CanAccess((LID)56232U, this, (ulong)this.Stream.Position, (ulong)((long)count), false);
			if (count == 0)
			{
				return 0;
			}
			int num = this.Stream.Read(buffer, offset, count);
			if (num == 0 && this.Stream.Position < this.Stream.Length)
			{
				throw new StoreException((LID)47132U, ErrorCodeValue.CorruptData);
			}
			return num;
		}

		public byte[] Read(MapiContext context, int numBytesToRead, out int actualBytesRead)
		{
			actualBytesRead = 0;
			base.ThrowIfNotValid(null);
			if (!this.ShouldAllowRead)
			{
				throw new ExExceptionAccessDenied((LID)62712U, "Stream does not allow read -- throwing!");
			}
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamRead, (LID)60263U);
			byte[] array;
			if (this.Stream == null)
			{
				array = new byte[0];
			}
			else
			{
				MapiStreamLock.CanAccess((LID)43944U, this, (ulong)this.Stream.Position, (ulong)((long)numBytesToRead), false);
				array = new byte[numBytesToRead];
				while (numBytesToRead > 0)
				{
					int num = this.Stream.Read(array, actualBytesRead, numBytesToRead);
					if (num == 0)
					{
						break;
					}
					actualBytesRead += num;
					numBytesToRead -= num;
				}
			}
			return array;
		}

		public int Write(MapiContext context, byte[] bytesToWrite)
		{
			return this.Write(context, bytesToWrite, 0, bytesToWrite.Length);
		}

		public int Write(MapiContext context, byte[] bytesToWrite, int offset, int length)
		{
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.StreamWrite, (LID)35687U);
			base.ThrowIfNotValid(null);
			if (!this.ShouldAllowWrite)
			{
				throw new ExExceptionStreamAccessDenied((LID)38136U, "Stream doesn't allow write.");
			}
			if (this.streamSizeInvalid)
			{
				return length;
			}
			this.ConvertToTempStreamIfNeeded();
			long position = this.Stream.Position;
			if (this.shouldAppend && position < this.originalLength)
			{
				throw new ExExceptionAccessDenied((LID)58616U, "Stream writes not allowed before the original length of the stream");
			}
			MapiStreamLock.CanAccess((LID)35752U, this, (ulong)this.Stream.Position, (ulong)((long)length), true);
			this.Stream.Write(bytesToWrite, offset, length);
			this.IsDirty = true;
			int result = (int)(this.Stream.Position - position);
			this.streamSizeInvalid |= base.ParentObject.IsStreamSizeInvalid(context, this.Stream.Length);
			return result;
		}

		public long Seek(MapiContext context, long offset, SeekOrigin origin)
		{
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamSeek, (LID)52071U);
			base.ThrowIfNotValid(null);
			if (this.Stream == null)
			{
				return 0L;
			}
			long num = 0L;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = offset;
				break;
			case SeekOrigin.Current:
				num = this.Stream.Position + offset;
				break;
			case SeekOrigin.End:
				num = this.Stream.Length + offset;
				break;
			}
			if (num < 0L || num > 2147483647L)
			{
				if (origin == SeekOrigin.Begin)
				{
					throw new ExExceptionStreamSeekError((LID)34040U, "Seek offset out of range");
				}
				throw new ExExceptionFail((LID)50424U, "Seek offset out of range");
			}
			else
			{
				if (num > this.Stream.Length && this.shouldAllowWrite)
				{
					this.SetSize(context, num);
				}
				if (num <= this.Stream.Length)
				{
					this.Stream.Seek(offset, origin);
					return this.Stream.Position;
				}
				this.Stream.Seek(0L, SeekOrigin.End);
				return num;
			}
		}

		public long GetSize(MapiContext context)
		{
			base.ThrowIfNotValid(null);
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamGetSize, (LID)45927U);
			if (this.Stream == null)
			{
				return 0L;
			}
			return this.Stream.Length;
		}

		public long SetSize(MapiContext context, long newSize)
		{
			base.ThrowIfNotValid(null);
			if (!this.shouldAllowWrite)
			{
				throw new ExExceptionStreamAccessDenied((LID)47352U, "Write to stream not allowed.");
			}
			this.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.StreamSetSize, (LID)62311U);
			if (this.shouldAppend && newSize < this.originalLength)
			{
				throw new ExExceptionAccessDenied((LID)63736U, "Cannot set the size of the stream smaller than the current!");
			}
			this.streamSizeInvalid |= base.ParentObject.IsStreamSizeInvalid(context, newSize);
			if (this.streamSizeInvalid)
			{
				throw new ExExceptionMaxSubmissionExceeded((LID)53148U, "Exceeded the size limitation");
			}
			if (newSize < this.originalLength)
			{
				MapiStreamLock.CanAccess((LID)52136U, this, (ulong)newSize, (ulong)(this.originalLength - newSize), true);
			}
			else
			{
				MapiStreamLock.CanAccess((LID)62376U, this, (ulong)this.originalLength, (ulong)(newSize - this.originalLength), true);
			}
			this.ConvertToTempStreamIfNeeded();
			this.Stream.SetLength(newSize);
			this.IsDirty = true;
			return this.Stream.Length;
		}

		internal ErrorCode LockRegion(MapiContext context, ulong offset, ulong length, bool exclusive)
		{
			return MapiStreamLock.LockRegion(this, offset, length, exclusive);
		}

		internal ErrorCode UnlockRegion(MapiContext context, ulong offset, ulong length, bool exclusive)
		{
			this.Stream.Flush();
			return MapiStreamLock.UnLockRegion(this, offset, length, exclusive);
		}

		internal void CheckRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, AccessCheckOperation operation, LID lid)
		{
			if (base.ParentObject == null)
			{
				return;
			}
			base.ParentObject.CheckPropertyRights(context, requestedRights, operation, lid);
		}

		public override void OnRelease(MapiContext context)
		{
			this.Commit(context);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiStream>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			try
			{
				if (calledFromDispose)
				{
					try
					{
						if (this.stream != null)
						{
							this.stream.Dispose();
						}
					}
					finally
					{
						MapiStreamLock.Release(this);
					}
				}
			}
			finally
			{
				base.InternalDispose(calledFromDispose);
			}
			this.propTag = StorePropTag.Invalid;
			this.originalLength = 0L;
			this.shouldAppend = false;
			this.shouldAllowRead = true;
			this.shouldAllowWrite = true;
			this.isDirty = false;
			this.stream = null;
		}

		private static int CopyStream(Stream source, Stream destination)
		{
			return TempStream.CopyStream(source, destination);
		}

		private static int CopyStreamToTempStream(Stream source, out Stream destination)
		{
			Stream stream = null;
			int result;
			try
			{
				stream = TempStream.CreateInstance();
				int num = MapiStream.CopyStream(source, stream);
				destination = stream;
				stream = null;
				result = num;
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

		private static Stream GetReadOnlyConverterStream(Stream sourceStream, CodePage sourceCodePage, CodePage targetCodePage)
		{
			return new ConverterStream(sourceStream, new TextToText(TextToTextConversionMode.ConvertCodePageOnly)
			{
				InputEncoding = CodePageMap.GetEncoding((int)sourceCodePage),
				OutputEncoding = CodePageMap.GetEncoding((int)targetCodePage)
			}, ConverterStreamAccess.Read);
		}

		private static Stream GetWriteOnlyConverterStream(Stream targetStream, CodePage sourceCodePage, CodePage targetCodePage)
		{
			return new ConverterStream(targetStream, new TextToText(TextToTextConversionMode.ConvertCodePageOnly)
			{
				OutputEncoding = CodePageMap.GetEncoding((int)targetCodePage),
				InputEncoding = CodePageMap.GetEncoding((int)sourceCodePage)
			}, ConverterStreamAccess.Write);
		}

		private void ConvertToTempStreamIfNeeded()
		{
			if (this.usingTempStream)
			{
				return;
			}
			if (this.stream == null)
			{
				return;
			}
			long position = this.stream.Position;
			this.stream.Position = 0L;
			Stream stream = null;
			try
			{
				MapiStream.CopyStreamToTempStream(this.stream, out stream);
				stream.Position = position;
				this.stream.Dispose();
				this.stream = stream;
				stream = null;
				this.usingTempStream = true;
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		private bool streamSizeInvalid;

		private Stream stream;

		private StorePropTag propTag;

		private CodePage codePage;

		private bool shouldAllowRead;

		private bool shouldAllowWrite;

		private long originalLength;

		private bool shouldAppend;

		private bool isDirty;

		private bool usingTempStream;
	}
}
