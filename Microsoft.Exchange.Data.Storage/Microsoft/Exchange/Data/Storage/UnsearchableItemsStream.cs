using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnsearchableItemsStream : Stream, IDisposeTrackable, IDisposable
	{
		internal static void SetTestDataSource(UnsearchableItemsStream.TestDataSource method)
		{
			UnsearchableItemsStream.testDataSource = method;
		}

		internal UnsearchableItemsStream(MailboxSession session)
		{
			this.session = session;
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
			string serverFqdn = session.MailboxOwner.MailboxInfo.Location.ServerFqdn;
			this.serverGuid = session.MailboxOwner.MailboxInfo.Location.ServerGuid;
			if (UnsearchableItemsStream.testDataSource == null)
			{
				bool flag = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.exRpcAdmin = ExRpcAdmin.Create("Client=CI", serverFqdn, null, null, null);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToGetUnsearchableItems, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("UnsearchableItemsStream::Constructor. Failed to create ExRpcAdmin.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToGetUnsearchableItems, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("UnsearchableItemsStream::Constructor. Failed to create ExRpcAdmin.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session != null)
						{
							session.EndMapiCall();
							if (flag)
							{
								session.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			this.lastMaxDocId = 0U;
			this.GetDataBlock();
		}

		private void GetDataBlock()
		{
			PropValue[][] array = null;
			if (UnsearchableItemsStream.testDataSource != null)
			{
				array = UnsearchableItemsStream.testDataSource(this.lastMaxDocId);
			}
			else
			{
				StoreSession storeSession = this.session;
				bool flag = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.exRpcAdmin.CiEnumerateFailedItemsByMailbox(this.session.MailboxOwner.MailboxInfo.GetDatabaseGuid(), this.serverGuid, this.session.MailboxOwner.MailboxInfo.MailboxGuid, this.lastMaxDocId, out array);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToGetUnsearchableItems, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("UnsearchableItemsStream::GetDataBlock. Failed to read data from CiEnumerateFailedItemsByMailbox.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToGetUnsearchableItems, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("UnsearchableItemsStream::GetDataBlock. Failed to read data from CiEnumerateFailedItemsByMailbox.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			if (array == null)
			{
				this.eof = true;
				return;
			}
			int num = array[0][2].GetBytes().Length;
			int num2 = array.Length * (num + 4);
			int i = 0;
			Stream stream = this.dataStream;
			if (stream != null)
			{
				i = (int)(stream.Length - stream.Position);
				if (i > 0)
				{
					num2 += i;
				}
			}
			MemoryStream memoryStream = new MemoryStream(num2);
			if (stream != null)
			{
				while (i > 0)
				{
					byte[] buffer = new byte[i];
					int num3 = stream.Read(buffer, 0, i);
					memoryStream.Write(buffer, 0, num3);
					i -= num3;
				}
				stream.Dispose();
				this.dataStream = null;
			}
			foreach (PropValue[] array3 in array)
			{
				uint @int = (uint)array3[0].GetInt();
				this.lastMaxDocId = @int;
				byte[] bytes = array3[2].GetBytes();
				memoryStream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
				memoryStream.Write(bytes, 0, bytes.Length);
			}
			long position = memoryStream.Position;
			memoryStream.Flush();
			memoryStream.Position = 0L;
			memoryStream.SetLength(position);
			this.dataStream = memoryStream;
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		protected override void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.InternalDispose(disposing);
				this.isDisposed = true;
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.dataStream != null)
				{
					this.dataStream.Dispose();
					this.dataStream = null;
				}
				Util.DisposeIfPresent(this.exRpcAdmin);
				Util.DisposeIfPresent(this.disposeTracker);
			}
			base.Dispose(disposing);
		}

		internal bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UnsearchableItemsStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed("CanRead");
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed("CanSeek");
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed("CanWrite");
				return false;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Read");
			if (this.dataStream == null)
			{
				return 0;
			}
			while ((long)count > this.dataStream.Length - this.dataStream.Position && !this.eof)
			{
				this.GetDataBlock();
			}
			return this.dataStream.Read(buffer, offset, count);
		}

		public override void Flush()
		{
			this.CheckDisposed("Flush");
			throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "Flush"));
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed("Length");
				throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "Length"));
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed("Position");
				throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "get_Position"));
			}
			set
			{
				this.CheckDisposed("Position");
				throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "set_Position"));
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed("Seek");
			throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "Seek"));
		}

		public override void SetLength(long length)
		{
			this.CheckDisposed("SetLength");
			throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "SetLength"));
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Write");
			throw new NotSupportedException(string.Format("{0} Not supported for UnsearchableItemsStream", "Write"));
		}

		public static byte[] GetEntryId(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			byte[] result;
			try
			{
				result = reader.ReadBytes(count);
			}
			catch (EndOfStreamException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExFailedToGetUnsearchableItems, innerException);
			}
			return result;
		}

		private const string ErrorString = "{0} Not supported for UnsearchableItemsStream";

		private bool isDisposed;

		private DisposeTracker disposeTracker;

		private readonly MailboxSession session;

		private readonly Guid serverGuid;

		private readonly ExRpcAdmin exRpcAdmin;

		private MemoryStream dataStream;

		private bool eof;

		private uint lastMaxDocId;

		private static UnsearchableItemsStream.TestDataSource testDataSource;

		internal delegate PropValue[][] TestDataSource(uint lastDocId);
	}
}
