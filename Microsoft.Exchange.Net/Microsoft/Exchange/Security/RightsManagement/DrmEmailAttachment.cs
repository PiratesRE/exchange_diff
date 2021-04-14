using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.Protectors;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal class DrmEmailAttachment
	{
		public DrmEmailAttachment(AttachmentType attachmentType, Stream attachmentStream, uint characterPosition, string contentId, string contentLocation, byte[] attachRendering, string displayName, string fileName, int flags, bool hidden)
		{
			if (attachmentStream == null)
			{
				throw new ArgumentNullException("attachmentStream");
			}
			if (DrmEmailAttachment.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Creating DrmEmailAttachment. AT:{0} CP:{1} CId:{2} CLoc:{3} DN:{4} FN:{5} AF:{6} AH:{7}", new object[]
				{
					attachmentType,
					characterPosition,
					contentId,
					contentLocation,
					displayName,
					fileName,
					flags,
					hidden
				});
				DrmEmailAttachment.Tracer.TraceDebug((long)this.GetHashCode(), message);
			}
			this.AttachmentType = attachmentType;
			this.AttachmentStream = attachmentStream;
			this.CharacterPosition = characterPosition;
			this.ContentId = contentId;
			this.ContentLocation = contentLocation;
			this.AttachRendering = attachRendering;
			this.DisplayName = displayName;
			this.FileName = fileName;
			this.AttachFlags = flags;
			this.AttachHidden = hidden;
			this.DvAspect = ((attachmentType == AttachmentType.OleObject) ? 1U : 4U);
			this.SizeX = 0U;
			this.SizeY = 0U;
		}

		public static DrmEmailAttachment Load(IStorage attachmentStorage, uint characterPosition, AttachmentType expectedAttachmentType, uint dvAspect, uint sizeX, uint sizeY, CreateStreamCallbackDelegate createStreamCallback)
		{
			AttachmentType attachmentType = AttachmentType.OleObject;
			Stream stream = null;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			byte[] attachRendering = null;
			int flags = 0;
			bool hidden = false;
			DrmEmailAttachment drmEmailAttachment = null;
			DrmEmailAttachment result;
			try
			{
				System.Runtime.InteropServices.ComTypes.STATSTG statstg = default(System.Runtime.InteropServices.ComTypes.STATSTG);
				attachmentStorage.Stat(ref statstg, 0);
				if (statstg.clsid == DrmEmailConstants.MsgAttGuid)
				{
					attachmentType = AttachmentType.EmbeddedMessage;
				}
				else if (statstg.clsid == DrmEmailConstants.FileAttachmentObjectGuid)
				{
					attachmentType = AttachmentType.ByValue;
				}
				if (expectedAttachmentType != AttachmentType.None && expectedAttachmentType != AttachmentType.OleObject && expectedAttachmentType != attachmentType)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentTypeMismatchRtfBody"));
				}
				stream = createStreamCallback(attachmentType);
				if (attachmentType == AttachmentType.OleObject)
				{
					IStorage storage = null;
					try
					{
						storage = DrmEmailUtils.CreateStorageOverStream(stream);
						attachmentStorage.CopyTo(0, null, IntPtr.Zero, storage);
						storage.Commit(STGC.STGC_DEFAULT);
						goto IL_20C;
					}
					finally
					{
						if (storage != null)
						{
							Marshal.ReleaseComObject(storage);
							storage = null;
						}
					}
				}
				StreamOverIStream streamOverIStream = new StreamOverIStream(null);
				BinaryReader reader = new BinaryReader(streamOverIStream);
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream2 = null;
				try
				{
					stream2 = DrmEmailUtils.EnsureStream(attachmentStorage, "AttachDesc");
					streamOverIStream.ReplaceIStream(stream2);
					DrmEmailAttachment.ReadDescription(reader, attachmentType, out empty, out empty2, out empty3, out empty4, out flags, out hidden);
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentDescEOSReached"), innerException);
				}
				finally
				{
					if (stream2 != null)
					{
						Marshal.ReleaseComObject(stream2);
						stream2 = null;
					}
				}
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream3 = null;
				try
				{
					stream3 = DrmEmailUtils.EnsureStream(attachmentStorage, "AttachPres");
					streamOverIStream.ReplaceIStream(stream3);
					DrmEmailAttachment.ReadPresentation(reader, out attachRendering);
				}
				catch (EndOfStreamException innerException2)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentPresEOSReached"), innerException2);
				}
				finally
				{
					if (stream3 != null)
					{
						Marshal.ReleaseComObject(stream3);
						stream3 = null;
					}
				}
				if (attachmentType == AttachmentType.ByValue)
				{
					Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream4 = null;
					try
					{
						stream4 = DrmEmailUtils.EnsureStream(attachmentStorage, "AttachContents");
						DrmEmailUtils.CopyStream(stream4, stream);
						goto IL_20C;
					}
					finally
					{
						if (stream4 != null)
						{
							Marshal.ReleaseComObject(stream4);
							stream4 = null;
						}
					}
				}
				IStorage storage2 = null;
				IStorage storage3 = null;
				try
				{
					storage3 = DrmEmailUtils.EnsureStorage(attachmentStorage, "MAPIMessage");
					storage2 = DrmEmailUtils.CreateStorageOverStream(stream);
					storage3.CopyTo(0, null, IntPtr.Zero, storage2);
					storage2.Commit(STGC.STGC_DEFAULT);
				}
				finally
				{
					if (storage2 != null)
					{
						Marshal.ReleaseComObject(storage2);
						storage2 = null;
					}
					if (storage3 != null)
					{
						Marshal.ReleaseComObject(storage3);
						storage3 = null;
					}
				}
				IL_20C:
				stream.Position = 0L;
				drmEmailAttachment = new DrmEmailAttachment(attachmentType, stream, characterPosition, empty, empty2, attachRendering, empty4, empty3, flags, hidden);
				drmEmailAttachment.SetAspectAndExtents(dvAspect, sizeX, sizeY);
				stream = null;
				result = drmEmailAttachment;
			}
			finally
			{
				if (drmEmailAttachment == null && stream != null)
				{
					stream.Close();
				}
			}
			return result;
		}

		public void Save(int index, IStorage attachmentStorage, EncryptedEmailMessageBinding messageBinding, bool isRtfMessage)
		{
			if (attachmentStorage == null)
			{
				throw new ArgumentNullException("attachmentStorage");
			}
			if (index < 0)
			{
				throw new ArgumentException("index must be non-negative");
			}
			if (messageBinding == null)
			{
				throw new ArgumentNullException("messageBinding");
			}
			if (this.AttachmentType == AttachmentType.OleObject)
			{
				IStorage storage = null;
				try
				{
					storage = DrmEmailUtils.OpenStorageOverStream(this.AttachmentStream);
					if (isRtfMessage)
					{
						this.UpdatePropsFromOleStorage(storage);
					}
					storage.CopyTo(0, null, IntPtr.Zero, attachmentStorage);
					return;
				}
				finally
				{
					if (storage != null)
					{
						Marshal.ReleaseComObject(storage);
						storage = null;
					}
				}
			}
			StreamOverIStream streamOverIStream = new StreamOverIStream(null);
			BufferedStream output = new BufferedStream(streamOverIStream);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			Guid guid = (this.AttachmentType == AttachmentType.EmbeddedMessage) ? DrmEmailConstants.MsgAttGuid : DrmEmailConstants.FileAttachmentObjectGuid;
			attachmentStorage.SetClass(ref guid);
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream = null;
			try
			{
				stream = attachmentStorage.CreateStream("\u0003MailAttachment", 4114, 0, 0);
				streamOverIStream.ReplaceIStream(stream);
				binaryWriter.Write(index);
				binaryWriter.Write(-1);
				binaryWriter.Flush();
				stream.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (stream != null)
				{
					Marshal.ReleaseComObject(stream);
					stream = null;
				}
			}
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream2 = null;
			try
			{
				stream2 = attachmentStorage.CreateStream("AttachDesc", 4114, 0, 0);
				streamOverIStream.ReplaceIStream(stream2);
				this.WriteDescription(binaryWriter);
				stream2.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (stream2 != null)
				{
					Marshal.ReleaseComObject(stream2);
					stream2 = null;
				}
			}
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream3 = null;
			try
			{
				stream3 = attachmentStorage.CreateStream("AttachPres", 4114, 0, 0);
				streamOverIStream.ReplaceIStream(stream3);
				this.WritePresentation(binaryWriter);
				stream3.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (stream3 != null)
				{
					Marshal.ReleaseComObject(stream3);
					stream3 = null;
				}
			}
			if (this.AttachmentType == AttachmentType.ByValue)
			{
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream4 = null;
				try
				{
					stream4 = attachmentStorage.CreateStream("AttachContents", 4114, 0, 0);
					DrmEmailMessageBinding drmEmailMessageBinding = messageBinding as DrmEmailMessageBinding;
					if (drmEmailMessageBinding == null || string.IsNullOrEmpty(this.FileName) || this.FileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || !ProtectorsManager.Instance.Protect(this.FileName, this.AttachmentStream, stream4, drmEmailMessageBinding.EncryptorHandle, drmEmailMessageBinding.DecryptorHandle, drmEmailMessageBinding.IssuanceLicense))
					{
						DrmEmailAttachment.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Did not protect attachment file {0} so copying it", this.FileName);
						this.AttachmentStream.Seek(0L, SeekOrigin.Begin);
						DrmEmailUtils.CopyStream(this.AttachmentStream, stream4);
					}
					stream4.Commit(STGC.STGC_DEFAULT);
					return;
				}
				finally
				{
					if (stream4 != null)
					{
						Marshal.ReleaseComObject(stream4);
						stream4 = null;
					}
				}
			}
			IStorage storage2 = null;
			IStorage storage3 = null;
			try
			{
				storage2 = attachmentStorage.CreateStorage("MAPIMessage", 4114, 0, 0);
				storage3 = DrmEmailUtils.OpenStorageOverStream(this.AttachmentStream);
				storage3.CopyTo(0, null, IntPtr.Zero, storage2);
				storage2.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (storage2 != null)
				{
					Marshal.ReleaseComObject(storage2);
					storage2 = null;
				}
				if (storage3 != null)
				{
					Marshal.ReleaseComObject(storage3);
					storage3 = null;
				}
			}
		}

		public void Close()
		{
			if (this.AttachmentStream != null)
			{
				this.AttachmentStream.Close();
				this.AttachmentStream = null;
			}
		}

		private static void ReadDescription(BinaryReader reader, AttachmentType attachmentType, out string contentId, out string contentLocation, out string fileName, out string displayName, out int flags, out bool hidden)
		{
			if (reader.ReadUInt16() != 515)
			{
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachDescVersionInvalid"));
			}
			DrmEmailUtils.ReadByteLengthAnsiString(reader);
			DrmEmailUtils.ReadByteLengthAnsiString(reader);
			DrmEmailUtils.ReadByteLengthAnsiString(reader);
			DrmEmailUtils.ReadByteLengthAnsiString(reader);
			DrmEmailUtils.ReadByteLengthAnsiString(reader);
			DrmEmailUtils.ReadByteLengthAnsiString(reader);
			reader.ReadUInt64();
			reader.ReadUInt64();
			uint num = reader.ReadUInt32();
			uint num2 = (attachmentType == AttachmentType.ByValue) ? 1U : 5U;
			if (num != num2)
			{
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachDescAttachTypeValWrong"));
			}
			contentId = DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			contentLocation = DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			displayName = DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			fileName = DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			DrmEmailUtils.ReadByteLengthUnicodeString(reader);
			hidden = (reader.ReadInt32() != 0);
			flags = reader.ReadInt32();
		}

		private static void ReadPresentation(BinaryReader reader, out byte[] attachRendering)
		{
			if (reader.ReadUInt16() != 256)
			{
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachPresVersionInvalid"));
			}
			attachRendering = null;
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			int num = reader.ReadInt32();
			if (num > 0)
			{
				try
				{
					if (checked(reader.BaseStream.Position + unchecked((long)num)) > reader.BaseStream.Length)
					{
						throw new EndOfStreamException(DrmStrings.ReadAttachRenderingError);
					}
				}
				catch (OverflowException innerException)
				{
					throw new EndOfStreamException(DrmStrings.ReadAttachRenderingErrorOverflow, innerException);
				}
				attachRendering = reader.ReadBytes(num);
			}
		}

		private static byte[] LoadIcon(byte[] attachRendering, string fileName, out int mm, out int iconWidth, out int iconHeight)
		{
			SafeIconHandle safeIconHandle = null;
			IntPtr intPtr = IntPtr.Zero;
			SafeWin32HGlobalHandle safeWin32HGlobalHandle = null;
			byte[] array;
			try
			{
				bool flag = true;
				if (attachRendering != null)
				{
					try
					{
						if (SafeNativeMethods.HmfpSetRenderBits((uint)attachRendering.Length, attachRendering, out safeIconHandle) == 0)
						{
							flag = false;
						}
					}
					catch (COMException arg)
					{
						DrmEmailAttachment.Tracer.TraceError<COMException>(0L, "Failed to convert attach rendering bytes to icon - using default icon. Error {0}", arg);
					}
				}
				if (flag)
				{
					DrmEmailAttachment.Tracer.TraceDebug(0L, "Loading the default icon");
					safeIconHandle = SafeNativeMethods.LoadIcon(IntPtr.Zero, DrmEmailAttachment.ErrorIconIntPtr);
				}
				if (safeIconHandle == null || safeIconHandle.IsInvalid)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				safeWin32HGlobalHandle = SafeNativeMethods.OleMetafilePictFromIconAndLabel(safeIconHandle, fileName, null, 0U);
				if (safeWin32HGlobalHandle == null || safeWin32HGlobalHandle.IsInvalid)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				intPtr = SafeNativeMethods.GlobalLock(safeWin32HGlobalHandle);
				if (intPtr == IntPtr.Zero)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				MetaFilePict metaFilePict = (MetaFilePict)Marshal.PtrToStructure(intPtr, typeof(MetaFilePict));
				mm = metaFilePict.Mm;
				iconWidth = metaFilePict.XExt;
				iconHeight = metaFilePict.YExt;
				uint metaFileBitsEx = SafeNativeMethods.GetMetaFileBitsEx(metaFilePict.MetaFileHandle, 0U, null);
				if (metaFileBitsEx == 0U)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				array = new byte[metaFileBitsEx];
				metaFileBitsEx = SafeNativeMethods.GetMetaFileBitsEx(metaFilePict.MetaFileHandle, metaFileBitsEx, array);
				if ((ulong)metaFileBitsEx != (ulong)((long)array.Length))
				{
					throw new InvalidOperationException("GetMetaFileBitsEx failed to get the image into the buffer.");
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					SafeNativeMethods.GlobalUnlock(safeWin32HGlobalHandle);
					intPtr = IntPtr.Zero;
				}
				if (safeWin32HGlobalHandle != null && !safeWin32HGlobalHandle.IsInvalid)
				{
					safeWin32HGlobalHandle.Close();
					safeWin32HGlobalHandle = null;
				}
				if (safeIconHandle != null && !safeIconHandle.IsInvalid)
				{
					safeIconHandle.Close();
					safeIconHandle = null;
				}
			}
			return array;
		}

		private void WriteDescription(BinaryWriter writer)
		{
			writer.Write(515);
			writer.Write(DrmEmailConstants.Null22Bytes, 0, 22);
			writer.Write((this.AttachmentType == AttachmentType.ByValue) ? 1U : 5U);
			DrmEmailUtils.WriteByteLengthUnicodeString(writer, this.ContentId);
			DrmEmailUtils.WriteByteLengthUnicodeString(writer, this.ContentLocation);
			writer.Write(DrmEmailConstants.Null22Bytes, 0, 2);
			DrmEmailUtils.WriteByteLengthUnicodeString(writer, this.DisplayName);
			DrmEmailUtils.WriteByteLengthUnicodeString(writer, this.FileName);
			writer.Write(DrmEmailConstants.Null22Bytes, 0, 1);
			StringBuilder stringBuilder = new StringBuilder(this.FileName);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				if (DrmEmailAttachment.InvalidPathChars.Contains(stringBuilder[i]))
				{
					stringBuilder[i] = '_';
				}
			}
			DrmEmailUtils.WriteByteLengthUnicodeString(writer, Path.GetExtension(stringBuilder.ToString()));
			writer.Write(DrmEmailConstants.Null22Bytes, 0, 3);
			writer.Write(this.AttachHidden ? 1 : 0);
			int value = this.AttachHidden ? (this.AttachFlags | 32 | 64) : (this.AttachFlags | 32);
			writer.Write(value);
			writer.Flush();
		}

		private void WritePresentation(BinaryWriter writer)
		{
			writer.Write(256);
			byte[] array = null;
			int value;
			int value2;
			int value3;
			try
			{
				array = DrmEmailAttachment.LoadIcon(this.AttachRendering, this.FileName, out value, out value2, out value3);
			}
			catch (COMException ex)
			{
				DrmEmailAttachment.Tracer.TraceError<COMException>((long)this.GetHashCode(), "Failed to load icon. COMException in processing embedded message: {0}", ex);
				throw new RightsManagementException(RightsManagementFailureCode.AttachmentProtectionFailed, DrmStrings.FailedToLoadIconForAttachment, ex);
			}
			writer.Write(value);
			writer.Write(value2);
			writer.Write(value3);
			writer.Write(array.Length);
			writer.Write(array);
			writer.Flush();
		}

		private void UpdatePropsFromOleStorage(IStorage attachmentStorage)
		{
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream = null;
			Exception ex = null;
			try
			{
				stream = DrmEmailUtils.EnsureStream(attachmentStorage, "\u0003MailStream");
				using (BinaryReader binaryReader = new BinaryReader(new StreamOverIStream(stream)))
				{
					uint dvAspect = binaryReader.ReadUInt32();
					uint sizeX = binaryReader.ReadUInt32();
					uint sizeY = binaryReader.ReadUInt32();
					this.SetAspectAndExtents(dvAspect, sizeX, sizeY);
				}
			}
			catch (EndOfStreamException ex2)
			{
				ex = ex2;
			}
			catch (InvalidRpmsgFormatException ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (ex != null)
				{
					DrmEmailAttachment.Tracer.TraceError<Exception>(0L, "Failed to open storage on the OLE attachment. Error {0}. Using default values for dvAspect and sizex/sizey", ex);
				}
				if (stream != null)
				{
					Marshal.ReleaseComObject(stream);
				}
			}
		}

		private void SetAspectAndExtents(uint dvAspect, uint sizeX, uint sizeY)
		{
			this.DvAspect = dvAspect;
			this.SizeX = sizeX;
			this.SizeY = sizeY;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", new object[]
			{
				this.DisplayName,
				this.AttachmentType
			});
		}

		private const int DrmSaveState = 32;

		private const int WasRendered = 64;

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private static readonly IntPtr ErrorIconIntPtr = new IntPtr(32513);

		private static readonly HashSet<char> InvalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());

		public AttachmentType AttachmentType;

		public Stream AttachmentStream;

		public readonly uint CharacterPosition;

		public uint DvAspect;

		public uint SizeX;

		public uint SizeY;

		public readonly string ContentId;

		public readonly string ContentLocation;

		public readonly byte[] AttachRendering;

		public readonly string DisplayName;

		public readonly string FileName;

		public readonly int AttachFlags;

		public readonly bool AttachHidden;
	}
}
