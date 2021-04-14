using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class ComStorage : IDisposable
	{
		private ComStorage(Interop.IStorage iStorage)
		{
			this.iStorage = iStorage;
			this.isDisposed = false;
		}

		public Guid StorageClass
		{
			get
			{
				this.CheckDisposed("ComStorage::get_Class");
				System.Runtime.InteropServices.ComTypes.STATSTG statstg;
				this.GetStat(out statstg);
				return statstg.clsid;
			}
			set
			{
				this.CheckDisposed("ComStorage::set_Class");
				Util.InvokeComCall(MsgStorageErrorCode.FailedWrite, delegate
				{
					this.iStorage.SetClass(ref value);
				});
			}
		}

		protected Interop.IStorage IStorage
		{
			get
			{
				this.CheckDisposed("ComStorage::get_Storage");
				return this.iStorage;
			}
		}

		public static ComStorage CreateFileStorage(string filename, ComStorage.OpenMode openMode)
		{
			Util.ThrowOnNullArgument(filename, "filename");
			ComStorage.CheckOpenMode(openMode, "openMode", ComStorage.validCreateModes);
			object obj = null;
			ComStorage result;
			try
			{
				Guid iidistorage = Interop.IIDIStorage;
				int num = Interop.StgCreateStorageEx(filename, (uint)openMode, 0, 0U, IntPtr.Zero, IntPtr.Zero, ref iidistorage, out obj);
				if (num != 0)
				{
					throw new MsgStorageException(MsgStorageErrorCode.CreateFileFailed, MsgStorageStrings.FailedCreateStorage(filename), num);
				}
				Interop.IStorage storage = obj as Interop.IStorage;
				if (storage == null)
				{
					throw new MsgStorageException(MsgStorageErrorCode.CreateFileFailed, MsgStorageStrings.FailedCreateStorage(filename));
				}
				obj = null;
				result = new ComStorage(storage);
			}
			finally
			{
				if (obj != null)
				{
					Marshal.ReleaseComObject(obj);
				}
			}
			return result;
		}

		public static ComStorage CreateStorageOnStream(Stream stream, ComStorage.OpenMode openMode)
		{
			Util.ThrowOnNullArgument(stream, "stream");
			ComStorage.CheckOpenMode(openMode, "openMode", ComStorage.validCreateModes);
			ComStorage comStorage = null;
			Interop.IStorage storage = null;
			try
			{
				LockBytesOnStream lockBytes = new LockBytesOnStream(stream);
				int num = Interop.StgCreateDocfileOnILockBytes(lockBytes, (uint)openMode, 0, out storage);
				if (num != 0)
				{
					throw new MsgStorageException(MsgStorageErrorCode.CreateStorageOnStreamFailed, MsgStorageStrings.FailedCreateStorage("ILockBytes"), num);
				}
				comStorage = new ComStorage(storage);
			}
			finally
			{
				if (comStorage == null && storage != null)
				{
					Marshal.ReleaseComObject(storage);
				}
			}
			return comStorage;
		}

		public static ComStorage OpenFileStorage(string filename, ComStorage.OpenMode openMode)
		{
			Util.ThrowOnNullArgument(filename, "filename");
			ComStorage.CheckOpenMode(openMode, "openMode", ComStorage.validOpenModes);
			object obj = null;
			ComStorage result;
			try
			{
				Guid iidistorage = Interop.IIDIStorage;
				int num = Interop.StgOpenStorageEx(filename, (uint)openMode, 0, 0U, IntPtr.Zero, IntPtr.Zero, ref iidistorage, out obj);
				if (num != 0)
				{
					throw new MsgStorageException(MsgStorageErrorCode.OpenStorageFileFailed, MsgStorageStrings.FailedOpenStorage(filename), num);
				}
				Interop.IStorage storage = obj as Interop.IStorage;
				if (storage == null)
				{
					throw new MsgStorageException(MsgStorageErrorCode.OpenStorageFileFailed, MsgStorageStrings.FailedOpenStorage(filename));
				}
				obj = null;
				result = new ComStorage(storage);
			}
			finally
			{
				if (obj != null)
				{
					Marshal.ReleaseComObject(obj);
				}
			}
			return result;
		}

		public static ComStorage OpenStorageOnStream(Stream stream, ComStorage.OpenMode openMode)
		{
			Util.ThrowOnNullArgument(stream, "stream");
			ComStorage.CheckOpenMode(openMode, "openMode", ComStorage.validOpenModes);
			ComStorage comStorage = null;
			Interop.IStorage storage = null;
			try
			{
				LockBytesOnStream lockBytes = new LockBytesOnStream(stream);
				int num = Interop.StgOpenStorageOnILockBytes(lockBytes, IntPtr.Zero, (uint)openMode, IntPtr.Zero, 0U, out storage);
				if (num != 0)
				{
					throw new MsgStorageException(MsgStorageErrorCode.OpenStorageOnStreamFailed, MsgStorageStrings.FailedOpenStorage("ILockBytes"), num);
				}
				comStorage = new ComStorage(storage);
			}
			finally
			{
				if (comStorage == null && storage != null)
				{
					Marshal.ReleaseComObject(storage);
				}
			}
			return comStorage;
		}

		public static void CopyStorageContent(ComStorage source, ComStorage destination)
		{
			Util.ThrowOnNullArgument(source, "source");
			Util.ThrowOnNullArgument(destination, "destination");
			Util.InvokeComCall(MsgStorageErrorCode.FailedCopyStorage, delegate
			{
				source.IStorage.CopyTo(0U, null, IntPtr.Zero, destination.IStorage);
			});
		}

		public ComStream CreateStream(string streamName, ComStorage.OpenMode openMode)
		{
			this.CheckDisposed("ComStorage::CreateStream");
			Util.ThrowOnNullArgument(streamName, "streamName");
			ComStorage.CheckOpenMode(openMode, "openMode", ComStorage.validCreateModes);
			Interop.IStream iStream = null;
			Util.InvokeComCall(MsgStorageErrorCode.FailedCreateStream, delegate
			{
				this.iStorage.CreateStream(streamName, (uint)openMode, 0U, 0U, out iStream);
			});
			return new ComStream(iStream);
		}

		public ComStream OpenStream(string streamName, ComStorage.OpenMode openMode)
		{
			this.CheckDisposed("ComStorage::OpenStream");
			Util.ThrowOnNullArgument(streamName, "streamName");
			ComStorage.CheckOpenMode(openMode, "openMode", ComStorage.validOpenModes);
			Interop.IStream iStream = null;
			Util.InvokeComCall(MsgStorageErrorCode.FailedOpenStream, delegate
			{
				this.iStorage.OpenStream(streamName, IntPtr.Zero, (uint)openMode, 0U, out iStream);
			});
			return new ComStream(iStream);
		}

		public ComStorage CreateStorage(string storageName, ComStorage.OpenMode mode)
		{
			this.CheckDisposed("ComStorage::CreateStorage");
			Util.ThrowOnNullArgument(storageName, "storageName");
			ComStorage.CheckOpenMode(mode, "mode", ComStorage.validCreateModes);
			Interop.IStorage iNewStorage = null;
			Util.InvokeComCall(MsgStorageErrorCode.FailedCreateSubstorage, delegate
			{
				this.iStorage.CreateStorage(storageName, (uint)mode, 0U, 0U, out iNewStorage);
			});
			return new ComStorage(iNewStorage);
		}

		public ComStorage OpenStorage(string storageName, ComStorage.OpenMode mode)
		{
			this.CheckDisposed("ComStorage::OpenStorage");
			Util.ThrowOnNullArgument(storageName, "storageName");
			ComStorage.CheckOpenMode(mode, "mode", ComStorage.validOpenModes);
			Interop.IStorage iOpenStorage = null;
			Util.InvokeComCall(MsgStorageErrorCode.FailedOpenSubstorage, delegate
			{
				this.iStorage.OpenStorage(storageName, null, (uint)mode, IntPtr.Zero, 0U, out iOpenStorage);
			});
			return new ComStorage(iOpenStorage);
		}

		public void CopyElement(string sourceElementName, ComStorage targetStorage, string targetElementName)
		{
			this.CheckDisposed("ComStorage::OpenStorage");
			Util.ThrowOnNullArgument(sourceElementName, "sourceElementName");
			Util.ThrowOnNullArgument(targetStorage, "targetStorage");
			Util.ThrowOnNullArgument(targetElementName, "targetElementName");
			Util.InvokeComCall(MsgStorageErrorCode.FailedCopyStorage, delegate
			{
				this.iStorage.MoveElementTo(sourceElementName, targetStorage.IStorage, targetElementName, 1U);
			});
		}

		public void MoveElement(string sourceElementName, ComStorage targetStorage, string targetElementName)
		{
			this.CheckDisposed("ComStorage::OpenStorage");
			Util.ThrowOnNullArgument(sourceElementName, "sourceElementName");
			Util.ThrowOnNullArgument(targetStorage, "targetStorage");
			Util.ThrowOnNullArgument(targetElementName, "targetElementName");
			Util.InvokeComCall(MsgStorageErrorCode.FailedCopyStorage, delegate
			{
				this.iStorage.MoveElementTo(sourceElementName, targetStorage.IStorage, targetElementName, 0U);
			});
		}

		public void Flush()
		{
			this.CheckDisposed("ComStorage::Flush");
			Util.InvokeComCall(MsgStorageErrorCode.FailedWrite, delegate
			{
				this.iStorage.Commit(0U);
			});
		}

		public void GetStat(out System.Runtime.InteropServices.ComTypes.STATSTG statStg)
		{
			this.CheckDisposed("ComStorage::GetStat");
			System.Runtime.InteropServices.ComTypes.STATSTG internalStatStg = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			Util.InvokeComCall(MsgStorageErrorCode.FailedRead, delegate
			{
				this.iStorage.Stat(out internalStatStg, 1U);
			});
			statStg = internalStatStg;
		}

		public void WriteBytesToStream(string streamName, byte[] data)
		{
			this.WriteBytesToStream(streamName, data, data.Length);
		}

		public unsafe void WriteBytesToStream(string streamName, byte[] data, int length)
		{
			this.CheckDisposed("ComStream::WriteToStream");
			Util.ThrowOnNullArgument(streamName, "streamName");
			Util.ThrowOnNullArgument(data, "data");
			Util.InvokeComCall(MsgStorageErrorCode.FailedWrite, delegate
			{
				Interop.IStream stream = null;
				try
				{
					int num = 0;
					this.iStorage.CreateStream(streamName, 17U, 0U, 0U, out stream);
					if (length != 0)
					{
						try
						{
							fixed (byte* ptr = &data[0])
							{
								stream.Write(ptr, length, out num);
							}
						}
						finally
						{
							byte* ptr = null;
						}
					}
					if (num != length)
					{
						throw new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.FailedWrite(streamName));
					}
				}
				finally
				{
					if (stream != null)
					{
						Marshal.ReleaseComObject(stream);
					}
				}
			});
		}

		public unsafe int ReadFromStream(string streamName, byte[] buffer, int size)
		{
			this.CheckDisposed("ComStream::ReadFromStream");
			Util.ThrowOnNullArgument(streamName, "streamName");
			Util.ThrowOnNullArgument(buffer, "buffer");
			Util.ThrowOnOutOfRange(size >= 0, "size");
			int result = 0;
			Util.InvokeComCall(MsgStorageErrorCode.FailedRead, delegate
			{
				Interop.IStream stream = null;
				try
				{
					this.iStorage.OpenStream(streamName, IntPtr.Zero, 16U, 0U, out stream);
					int result = 0;
					if (size != 0)
					{
						try
						{
							fixed (byte* ptr = &buffer[0])
							{
								stream.Read(ptr, size, out result);
							}
						}
						finally
						{
							byte* ptr = null;
						}
					}
					result = result;
				}
				finally
				{
					if (stream != null)
					{
						Marshal.ReleaseComObject(stream);
					}
				}
			});
			return result;
		}

		public unsafe byte[] ReadFromStreamMaxLength(string streamName, int maxSize)
		{
			byte[] result = null;
			Util.InvokeComCall(MsgStorageErrorCode.FailedRead, delegate
			{
				Interop.IStream stream = null;
				try
				{
					this.iStorage.OpenStream(streamName, IntPtr.Zero, 16U, 0U, out stream);
					System.Runtime.InteropServices.ComTypes.STATSTG statstg;
					stream.Stat(out statstg, 1U);
					if (statstg.cbSize > (long)maxSize)
					{
						throw new MsgStorageException(MsgStorageErrorCode.StorageStreamTooLong, MsgStorageStrings.StreamTooBig(streamName, statstg.cbSize));
					}
					int num = (int)statstg.cbSize;
					int num2 = 0;
					result = new byte[num];
					if (result.Length != 0)
					{
						try
						{
							fixed (byte* ptr = &result[0])
							{
								stream.Read(ptr, result.Length, out num2);
							}
						}
						finally
						{
							byte* ptr = null;
						}
					}
					if (num2 != result.Length)
					{
						throw new MsgStorageException(MsgStorageErrorCode.StorageStreamTruncated, MsgStorageStrings.FailedRead(streamName));
					}
				}
				finally
				{
					if (stream != null)
					{
						Marshal.ReleaseComObject(stream);
					}
				}
			});
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(methodName);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing && this.iStorage != null)
			{
				Marshal.ReleaseComObject(this.iStorage);
			}
			this.iStorage = null;
		}

		private static void CheckOpenMode(ComStorage.OpenMode value, string paramName, params ComStorage.OpenMode[] validValues)
		{
			if (validValues.Length != 0)
			{
				for (int num = 0; num != validValues.Length; num++)
				{
					if (value == validValues[num])
					{
						return;
					}
				}
				string message = string.Format("Invalid parameter value: {0}", value.ToString());
				throw new ArgumentOutOfRangeException(paramName, message);
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private static ComStorage.OpenMode[] validCreateModes = new ComStorage.OpenMode[]
		{
			ComStorage.OpenMode.ReadWrite,
			ComStorage.OpenMode.Write,
			(ComStorage.OpenMode)4114,
			ComStorage.OpenMode.CreateWrite
		};

		private static ComStorage.OpenMode[] validOpenModes = new ComStorage.OpenMode[]
		{
			ComStorage.OpenMode.Read,
			ComStorage.OpenMode.ReadWrite,
			ComStorage.OpenMode.Write
		};

		private Interop.IStorage iStorage;

		private bool isDisposed;

		[Flags]
		public enum OpenMode
		{
			Read = 16,
			Write = 17,
			ReadWrite = 18,
			Create = 4096,
			CreateWrite = 4113
		}
	}
}
