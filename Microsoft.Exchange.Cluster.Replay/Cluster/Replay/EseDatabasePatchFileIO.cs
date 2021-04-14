using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class EseDatabasePatchFileIO : DisposeTrackableBase
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.IncrementalReseederTracer;
			}
		}

		public static EseDatabasePatchFileIO OpenRead(string edbFilePath)
		{
			return new EseDatabasePatchFileIO(edbFilePath, null, true, false);
		}

		public static EseDatabasePatchFileIO OpenWrite(string edbFilePath, EseDatabasePatchState header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			return new EseDatabasePatchFileIO(edbFilePath, header, false, false);
		}

		public static void GetNames(string edbFilePath, out string doneName, out string inprogressName)
		{
			using (EseDatabasePatchFileIO eseDatabasePatchFileIO = new EseDatabasePatchFileIO(edbFilePath, null, true, true))
			{
				doneName = eseDatabasePatchFileIO.m_patchFileNameDone;
				inprogressName = eseDatabasePatchFileIO.m_patchFileNameInProgress;
			}
		}

		public static bool IsLegacyPatchFilePresent(string edbFilePath)
		{
			string path;
			string path2;
			EseDatabasePatchFileIO.GetLegacyNames(edbFilePath, out path, out path2);
			return File.Exists(path) || File.Exists(path2);
		}

		public static void DeleteAll(string edbFilePath)
		{
			using (EseDatabasePatchFileIO eseDatabasePatchFileIO = new EseDatabasePatchFileIO(edbFilePath, null, true, true))
			{
				EseDatabasePatchFileIO.DeletePatchFile(eseDatabasePatchFileIO.m_patchFileNameInProgress);
				EseDatabasePatchFileIO.DeletePatchFile(eseDatabasePatchFileIO.m_patchFileNameDone);
				string fileFullPath;
				string fileFullPath2;
				EseDatabasePatchFileIO.GetLegacyNames(edbFilePath, out fileFullPath, out fileFullPath2);
				EseDatabasePatchFileIO.DeletePatchFile(fileFullPath2);
				EseDatabasePatchFileIO.DeletePatchFile(fileFullPath);
			}
		}

		public void FinishWriting()
		{
			this.AssertWriteOperationValid();
			this.m_fileStream.Flush(true);
			this.DisposeFileStream();
			File.Move(this.m_patchFileNameInProgress, this.m_patchFileNameDone);
		}

		public string InProgressName
		{
			get
			{
				return this.m_patchFileNameInProgress;
			}
		}

		public string DoneName
		{
			get
			{
				return this.m_patchFileNameDone;
			}
		}

		public EseDatabasePatchState ReadHeader()
		{
			this.AssertReadOperationValid();
			EseDatabasePatchState eseDatabasePatchState = null;
			this.SeekToStart();
			byte[] array = new byte[32768L];
			this.ReadFromFile(array, array.Length);
			int num = 0;
			int num2 = BitConverter.ToInt32(array, num);
			num += 4;
			if (num2 > array.Length - 4 || num2 <= 0)
			{
				EseDatabasePatchFileIO.Tracer.TraceError<int, int>((long)this.GetHashCode(), "ReadHeader(): Serialized header state in bytes ({0}) is invalid. Must be >=0 and < pre-allocated fixed byte size of {1} bytes.", num2, array.Length);
				throw new PagePatchInvalidFileException(this.m_fileStream.Name);
			}
			byte[] array2 = new byte[num2];
			Array.Copy(array, num, array2, 0, num2);
			Exception ex = null;
			try
			{
				eseDatabasePatchState = SerializationServices.Deserialize<EseDatabasePatchState>(array2);
			}
			catch (SerializationException ex2)
			{
				ex = ex2;
			}
			catch (TargetInvocationException ex3)
			{
				ex = ex3;
			}
			catch (DecoderFallbackException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				EseDatabasePatchFileIO.Tracer.TraceError<Exception>((long)this.GetHashCode(), "ReadHeader deserialize failed: {0}", ex);
				throw new PagePatchInvalidFileException(this.m_fileStream.Name, ex);
			}
			this.m_header = eseDatabasePatchState;
			return eseDatabasePatchState;
		}

		public void WriteHeader()
		{
			this.AssertWriteOperationValid();
			if (this.m_header.NumPagesToPatch > 3276800)
			{
				throw new PagePatchTooManyPagesToPatchException(this.m_header.NumPagesToPatch, 3276800);
			}
			byte[] array = new byte[32768L];
			byte[] array2 = SerializationServices.Serialize(this.m_header);
			DiagCore.RetailAssert(array2.Length <= array.Length - 4, "Serialized header state in bytes ({0}) is greater than pre-allocated fixed byte size of {1} bytes.", new object[]
			{
				array2.Length,
				array.Length
			});
			int num = 0;
			Array.Copy(BitConverter.GetBytes(array2.Length), 0, array, num, 4);
			num += 4;
			Array.Copy(array2, 0, array, num, array2.Length);
			this.SeekToStart();
			this.m_fileStream.Write(array, 0, array.Length);
			this.m_fileStream.Flush(true);
		}

		public IEnumerable<long> ReadPageNumbers()
		{
			this.AssertReadOperationValid();
			if (this.m_header == null)
			{
				this.ReadHeader();
			}
			int numPages = this.m_header.NumPagesToPatch;
			byte[] pgNoBytes = new byte[8];
			for (int i = 0; i < numPages; i++)
			{
				this.ZeroBuffer(pgNoBytes);
				this.SeekToPageListAtIndex(i);
				this.ReadFromFile(pgNoBytes, 8);
				yield return BitConverter.ToInt64(pgNoBytes, 0);
			}
			yield break;
		}

		public void WritePageNumbers(IEnumerable<long> pageNumbers)
		{
			this.AssertWriteOperationValid();
			this.SeekToPageListStart();
			foreach (long value in pageNumbers)
			{
				this.m_fileStream.Write(BitConverter.GetBytes(value), 0, 8);
			}
			this.m_pageNumbersWritten = true;
		}

		public byte[] ReadPageData(int index)
		{
			this.AssertReadOperationValid();
			if (this.m_header == null)
			{
				this.ReadHeader();
			}
			this.SeekToPageDataAtIndex(index);
			byte[] array = new byte[this.m_header.PageSizeBytes];
			this.ReadFromFile(array, array.Length);
			return array;
		}

		public void WritePageData(byte[] data)
		{
			this.AssertWriteOperationValid();
			DiagCore.AssertOrWatson(this.m_pageNumbersWritten, "WritePageNumbers() should be called first!", new object[0]);
			if (data.LongLength != this.m_header.PageSizeBytes)
			{
				throw new PagePatchInvalidPageSizeException(data.LongLength, this.m_header.PageSizeBytes);
			}
			this.m_fileStream.Write(data, 0, data.Length);
		}

		private EseDatabasePatchFileIO(string edbFilePath, EseDatabasePatchState header, bool readOnly, bool skipIO)
		{
			this.m_fReadOnly = readOnly;
			this.m_fSkipIO = skipIO;
			this.m_header = header;
			this.m_patchFileNameInProgress = edbFilePath + ".patch-progress";
			this.m_patchFileNameDone = edbFilePath + ".patch-done";
			if (!skipIO)
			{
				if (this.m_fReadOnly)
				{
					EseDatabasePatchFileIO.Tracer.TraceDebug<string>((long)this.GetHashCode(), "EseDatabasePatchFileIO: Opening the completed patch file in read-only mode: {0}", this.m_patchFileNameDone);
					this.m_fileStream = File.OpenRead(this.m_patchFileNameDone);
					return;
				}
				if (File.Exists(this.m_patchFileNameDone))
				{
					EseDatabasePatchFileIO.Tracer.TraceDebug<string>((long)this.GetHashCode(), "EseDatabasePatchFileIO: Re-opening the completed patch file in write-mode: {0}", this.m_patchFileNameDone);
					this.m_fileStream = File.Open(this.m_patchFileNameDone, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
					return;
				}
				if (File.Exists(this.m_patchFileNameInProgress))
				{
					EseDatabasePatchFileIO.Tracer.TraceDebug<string>((long)this.GetHashCode(), "EseDatabasePatchFileIO: Re-opening the in-progress patch file in write-mode: {0}", this.m_patchFileNameInProgress);
					this.m_fileStream = File.Open(this.m_patchFileNameInProgress, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
					return;
				}
				EseDatabasePatchFileIO.Tracer.TraceDebug<string>((long)this.GetHashCode(), "EseDatabasePatchFileIO: Creating the in-progress patch file: {0}", this.m_patchFileNameInProgress);
				this.m_fileStream = File.Open(this.m_patchFileNameInProgress, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeFileStream();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EseDatabasePatchFileIO>(this);
		}

		private static void DeletePatchFile(string fileFullPath)
		{
			Exception ex = FileCleanup.Delete(fileFullPath);
			if (ex != null)
			{
				EseDatabasePatchFileIO.Tracer.TraceError<string, Exception>(0L, "Failed to clean up one of the patch files: {0} ; Error: {1}", fileFullPath, ex);
				throw new PagePatchFileDeletionException(fileFullPath, ex.Message, ex);
			}
		}

		private static void GetLegacyNames(string edbFilePath, out string oldDoneName, out string oldInProgressName)
		{
			oldDoneName = edbFilePath + ".reseed";
			oldInProgressName = edbFilePath + ".reseed-progress";
		}

		private void SeekToStart()
		{
			this.m_fileStream.Seek(0L, SeekOrigin.Begin);
		}

		private void SeekToPageListStart()
		{
			this.m_fileStream.Seek(32768L, SeekOrigin.Begin);
		}

		private void SeekToPageListAtIndex(int index)
		{
			this.SeekToPageListStart();
			this.m_fileStream.Seek((long)(index * 8), SeekOrigin.Current);
		}

		private void SeekToPageDataAtIndex(int index)
		{
			this.SeekToPageListAtIndex(this.m_header.NumPagesToPatch);
			this.m_fileStream.Seek((long)index * this.m_header.PageSizeBytes, SeekOrigin.Current);
		}

		private void ZeroBuffer(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = 0;
			}
		}

		private void DisposeFileStream()
		{
			if (this.m_fileStream != null && !this.m_fileStreamClosed)
			{
				this.m_fileStream.Dispose();
				this.m_fileStreamClosed = true;
			}
		}

		private void ReadFromFile(byte[] buffer, int numBytesToRead)
		{
			int num = this.m_fileStream.Read(buffer, 0, numBytesToRead);
			if (num != numBytesToRead)
			{
				throw new PagePatchFileReadException(this.m_fileStream.Name, (long)num, (long)numBytesToRead);
			}
		}

		private void AssertWriteOperationValid()
		{
			DiagCore.AssertOrWatson(!this.m_fSkipIO, "m_fSkipIO was true but we're doing a write operation?", new object[0]);
			DiagCore.AssertOrWatson(!this.m_fReadOnly, "m_fReadOnly was true but we're doing a write operation?", new object[0]);
		}

		private void AssertReadOperationValid()
		{
			DiagCore.AssertOrWatson(!this.m_fSkipIO, "m_fSkipIO was true but we're doing a read operation?", new object[0]);
		}

		private const string PatchFileInProgressSuffix = ".patch-progress";

		private const string PatchFileDoneSuffix = ".patch-done";

		private const string OldReseedSuffix = ".reseed-progress";

		private const string OldReseedDoneSuffix = ".reseed";

		public const long HeaderSizeBytes = 32768L;

		public const int MaxNumPagesSupported = 3276800;

		private readonly bool m_fReadOnly;

		private readonly bool m_fSkipIO;

		private bool m_pageNumbersWritten;

		private bool m_fileStreamClosed;

		private FileStream m_fileStream;

		private EseDatabasePatchState m_header;

		private readonly string m_patchFileNameInProgress;

		private readonly string m_patchFileNameDone;
	}
}
