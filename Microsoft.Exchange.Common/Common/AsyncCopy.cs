using System;
using System.IO;
using System.Threading;

namespace Microsoft.Exchange.Common
{
	public class AsyncCopy : IDisposable
	{
		public event CopyProgressUpdatedEventDelegate CopyProgressUpdatedEvent;

		public CopyState CopyState
		{
			get
			{
				return this.copyState;
			}
			set
			{
				this.copyState = value;
			}
		}

		public AsyncCopy(int numberBuffers, int bufferSize)
		{
			this.numberBuffers = numberBuffers;
			this.bufferSize = bufferSize;
			this.copyState = CopyState.SuccessfulCopy;
			this.buffers = new AsyncBuffer[numberBuffers];
			for (int i = 0; i < numberBuffers; i++)
			{
				this.buffers[i].Buffer = new byte[bufferSize];
			}
			this.copyComplete = new ManualResetEvent(false);
		}

		public void Dispose()
		{
			this.CloseStreams();
		}

		public void CloseStreams()
		{
			lock (this)
			{
				this.closeStreamsCalled = true;
			}
			this.CloseStreamsInternal();
		}

		public void AsyncCopyFile(string fromFile, string toFile, bool supersede)
		{
			CopyMode copyMode = supersede ? CopyMode.Supersede : CopyMode.NewFileOnly;
			this.AsyncCopyFile(fromFile, toFile, copyMode);
		}

		public void AsyncCopyFile(string fromFile, string toFile, CopyMode copyMode)
		{
			if (this.closeStreamsCalled)
			{
				throw new ObjectDisposedException("AsyncCopy Handles");
			}
			bool flag = false;
			try
			{
				FileStream fileStream = null;
				FileStream fileStream2 = null;
				this.copyState = CopyState.OpeningInputFile;
				fileStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read, FileShare.Read, this.bufferSize, true);
				ulong num2;
				try
				{
					long num = 0L;
					if (copyMode == CopyMode.AllowAppend)
					{
						FileInfo fileInfo = new FileInfo(toFile);
						if (fileInfo.Exists)
						{
							num = fileInfo.Length;
							FileInfo fileInfo2 = new FileInfo(fromFile);
							if (fileInfo2.Length == num)
							{
								this.copyState = CopyState.SuccessfulCopy;
								return;
							}
							if (fileInfo2.Length < num)
							{
								this.copyState = CopyState.InvalidOperation;
								return;
							}
						}
					}
					FileMode mode;
					switch (copyMode)
					{
					case CopyMode.NewFileOnly:
						mode = FileMode.CreateNew;
						break;
					case CopyMode.Supersede:
						mode = FileMode.Create;
						break;
					case CopyMode.AllowAppend:
						mode = FileMode.Append;
						break;
					default:
						mode = FileMode.Create;
						break;
					}
					this.copyState = CopyState.OpeningOutputFile;
					fileStream2 = new FileStream(toFile, mode, FileAccess.Write, FileShare.None, this.bufferSize, true);
					if (num > 0L)
					{
						fileStream.Seek(num, SeekOrigin.Begin);
					}
				}
				finally
				{
					lock (this)
					{
						this.readStream = fileStream;
						this.writeStream = fileStream2;
						flag = this.closeStreamsCalled;
						num2 = this.copyCount;
					}
				}
				if (flag)
				{
					throw new ObjectDisposedException("AsyncCopy Handles");
				}
				this.exceptionReceived = null;
				this.outstandingReads = 0;
				this.outstandingWrites = 0;
				this.copyComplete.Reset();
				for (int i = 0; i < this.numberBuffers; i++)
				{
					this.buffers[i].AsyncResult = null;
					this.buffers[i].Reading = false;
					this.buffers[i].WriteDeferred = false;
					this.DoNext(i, false, num2);
				}
				this.copyComplete.WaitOne();
				if (this.exceptionReceived != null)
				{
					throw new AsyncCopyGetException(this.exceptionReceived);
				}
				this.copyState = CopyState.FlushingOutputFile;
				lock (this)
				{
					if (this.writeStream != null)
					{
						this.writeStream.Flush();
					}
				}
				this.copyState = CopyState.SuccessfulCopy;
			}
			finally
			{
				lock (this)
				{
					if (flag || this.closeStreamsCalled)
					{
						this.copyState = CopyState.CopyStopped;
					}
				}
				this.CloseStreamsInternal();
			}
		}

		private static void StreamDelegate(IAsyncResult ar)
		{
			ulong num = 0UL;
			if (!ar.CompletedSynchronously)
			{
				AsyncCopy asyncCopy = (AsyncCopy)ar.AsyncState;
				int i;
				lock (asyncCopy)
				{
					i = 0;
					while (i < asyncCopy.numberBuffers && asyncCopy.buffers[i].AsyncResult != ar)
					{
						i++;
					}
					if (!asyncCopy.closeStreamsCalled)
					{
						num = asyncCopy.copyCount;
					}
				}
				asyncCopy.DoNext(i, true, num);
				bool flag2;
				do
				{
					flag2 = false;
					lock (asyncCopy)
					{
						if (asyncCopy.exceptionReceived == null && asyncCopy.writeStream != null)
						{
							for (i = 0; i < asyncCopy.numberBuffers; i++)
							{
								if (asyncCopy.buffers[i].WriteDeferred && asyncCopy.buffers[i].FileOffset == asyncCopy.writeStream.Position)
								{
									asyncCopy.buffers[i].WriteDeferred = false;
									flag2 = true;
									break;
								}
							}
						}
					}
					if (flag2)
					{
						asyncCopy.DoNext(i, false, num);
					}
				}
				while (flag2);
			}
		}

		private void CloseStreamsInternal()
		{
			FileStream fileStream = null;
			FileStream fileStream2 = null;
			lock (this)
			{
				this.copyCount += 1UL;
				fileStream2 = this.writeStream;
				this.writeStream = null;
				fileStream = this.readStream;
				this.readStream = null;
			}
			this.copyComplete.Set();
			try
			{
				if (fileStream2 != null)
				{
					fileStream2.Close();
				}
			}
			catch (IOException)
			{
			}
			try
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			catch (IOException)
			{
			}
		}

		private void DoNext(int bufNum, bool ioComplete, ulong copyCount)
		{
			CopyState copyState = CopyState.SuccessfulCopy;
			try
			{
				for (;;)
				{
					if (!ioComplete && !this.buffers[bufNum].Reading)
					{
						lock (this)
						{
							if (copyCount != this.copyCount || this.exceptionReceived != null || this.readStream.Position >= this.readStream.Length)
							{
								goto IL_3C2;
							}
							long position = this.readStream.Position;
							int num = this.bufferSize;
							if (position + (long)num > this.readStream.Length)
							{
								num = (int)(this.readStream.Length - position);
							}
							this.buffers[bufNum].FileOffset = position;
							this.buffers[bufNum].ReadLength = num;
							this.buffers[bufNum].Reading = true;
							copyState = CopyState.ReadingInputFile;
							this.buffers[bufNum].AsyncResult = this.readStream.BeginRead(this.buffers[bufNum].Buffer, 0, num, new AsyncCallback(AsyncCopy.StreamDelegate), this);
							this.outstandingReads++;
							ioComplete = this.buffers[bufNum].AsyncResult.CompletedSynchronously;
							if (!ioComplete)
							{
								goto IL_3C2;
							}
						}
					}
					if (ioComplete && this.buffers[bufNum].Reading)
					{
						int num2 = this.buffers[bufNum].ReadLength;
						lock (this)
						{
							try
							{
								IAsyncResult asyncResult = this.buffers[bufNum].AsyncResult;
								copyState = CopyState.CompletingRead;
								if (this.readStream != null)
								{
									num2 = this.readStream.EndRead(asyncResult);
								}
							}
							finally
							{
								this.outstandingReads--;
								this.buffers[bufNum].AsyncResult = null;
							}
						}
						if (num2 != this.buffers[bufNum].ReadLength)
						{
							break;
						}
						ioComplete = false;
					}
					if (!ioComplete && this.buffers[bufNum].Reading)
					{
						lock (this)
						{
							if (copyCount != this.copyCount || this.exceptionReceived != null)
							{
								goto IL_3C2;
							}
							if (this.buffers[bufNum].FileOffset != this.writeStream.Position)
							{
								this.buffers[bufNum].WriteDeferred = true;
								goto IL_3C2;
							}
							this.buffers[bufNum].Reading = false;
							copyState = CopyState.WritingOutputFile;
							this.buffers[bufNum].AsyncResult = this.writeStream.BeginWrite(this.buffers[bufNum].Buffer, 0, this.buffers[bufNum].ReadLength, new AsyncCallback(AsyncCopy.StreamDelegate), this);
							this.outstandingWrites++;
							ioComplete = this.buffers[bufNum].AsyncResult.CompletedSynchronously;
							if (!ioComplete)
							{
								goto IL_3C2;
							}
						}
					}
					if (ioComplete && !this.buffers[bufNum].Reading)
					{
						lock (this)
						{
							try
							{
								IAsyncResult asyncResult = this.buffers[bufNum].AsyncResult;
								copyState = CopyState.CompletingWrite;
								if (this.writeStream != null)
								{
									this.writeStream.EndWrite(asyncResult);
								}
							}
							finally
							{
								this.outstandingWrites--;
								this.buffers[bufNum].AsyncResult = null;
								if (this.CopyProgressUpdatedEvent != null)
								{
									this.CopyProgressUpdatedEvent(this, this.buffers[bufNum].ReadLength);
								}
							}
						}
						ioComplete = false;
					}
				}
				throw new EndOfStreamException();
				IL_3C2:;
			}
			catch (IOException ex)
			{
				lock (this)
				{
					if (this.exceptionReceived == null)
					{
						this.exceptionReceived = ex;
						this.copyState = copyState;
					}
				}
			}
			catch (ObjectDisposedException ex2)
			{
				lock (this)
				{
					if (this.exceptionReceived == null)
					{
						this.exceptionReceived = ex2;
						this.copyState = CopyState.CopyStopped;
					}
				}
			}
			lock (this)
			{
				if (copyCount == this.copyCount && this.outstandingReads + this.outstandingWrites == 0 && (this.exceptionReceived != null || this.writeStream.Position >= this.readStream.Length))
				{
					this.copyComplete.Set();
				}
			}
		}

		private FileStream readStream;

		private FileStream writeStream;

		private int outstandingReads;

		private int outstandingWrites;

		private readonly int numberBuffers;

		private readonly int bufferSize;

		private readonly AsyncBuffer[] buffers;

		private Exception exceptionReceived;

		private CopyState copyState;

		private ulong copyCount;

		private bool closeStreamsCalled;

		private ManualResetEvent copyComplete;
	}
}
