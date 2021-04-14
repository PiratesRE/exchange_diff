using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class TempFileFactory
	{
		internal static void StartCleanUpTimer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.StartCleanUpTimer", new object[0]);
			if (TempFileFactory.cleanUpTimer != null)
			{
				throw new InvalidOperationException("Cleanup timer already started");
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(TempFileFactory.cleanUpDelayQueue.RetryInterval.TotalSeconds / 2.0);
			TempFileFactory.cleanUpTimer = new Timer(new TimerCallback(TempFileFactory.CleanUpTimerCallback), null, timeSpan, timeSpan);
			TempFileFactory.timerSyncEvent.Set();
		}

		internal static void StopCleanUpTimer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.StopCleanUpTimer", new object[0]);
			if (TempFileFactory.cleanUpTimer != null)
			{
				TempFileFactory.cleanUpTimer.Dispose();
				TempFileFactory.cleanUpTimer = null;
				TimeSpan timeout = TimeSpan.FromSeconds(30.0);
				if (!TempFileFactory.timerSyncEvent.WaitOne(timeout, false))
				{
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.Shutdown: Didn't get timerSyncEvent within {0}s", new object[]
					{
						timeout.TotalSeconds
					});
				}
			}
		}

		internal static ITempFile CreateTempFile()
		{
			return TempFileFactory.CreateTempFile(".tmp");
		}

		internal static ITempFile CreateTempDir()
		{
			TempFileFactory.TempDir tempDir = new TempFileFactory.TempDir();
			TempFileFactory.AddFileToTable(tempDir);
			return tempDir;
		}

		internal static ITempFile CreateTempWmaFile()
		{
			return TempFileFactory.CreateTempFile(".wma");
		}

		internal static ITempFile CreateTempMp3File()
		{
			return TempFileFactory.CreateTempFile(".mp3");
		}

		internal static ITempFile CreateTempSoundFileFromAttachmentName(string attachmentName)
		{
			if (AudioFile.IsWma(attachmentName) || AudioFile.IsProtectedWma(attachmentName))
			{
				return TempFileFactory.CreateTempWmaFile();
			}
			if (AudioFile.IsMp3(attachmentName) || AudioFile.IsProtectedMp3(attachmentName))
			{
				return TempFileFactory.CreateTempMp3File();
			}
			return TempFileFactory.CreateTempWavFile();
		}

		internal static ITempFile CreateTempFileFromAttachment(Attachment attachment)
		{
			ITempFile result = null;
			string contentType;
			if ((contentType = attachment.ContentType) != null)
			{
				if (!(contentType == "image/tiff"))
				{
					if (contentType == "audio/wav")
					{
						result = TempFileFactory.CreateTempSoundFileFromAttachmentName(attachment.FileName);
					}
				}
				else
				{
					result = TempFileFactory.CreateTempTifFile();
				}
			}
			return result;
		}

		internal static ITempFile CreateTempTifFile()
		{
			return TempFileFactory.CreateTempFile(".tif");
		}

		internal static ITempFile CreateTempGrammarFile(string fileNameWithoutExtension, string extension)
		{
			string fileName = string.IsNullOrEmpty(fileNameWithoutExtension) ? Guid.NewGuid().ToString() : fileNameWithoutExtension;
			ITempFile tempFile = TempFileFactory.CreateTempFile(fileName, extension);
			TempFileFactory.AddNetworkServiceReadAccess(tempFile.FilePath, false);
			return tempFile;
		}

		internal static ITempFile CreateTempGrammarFile(string fileNameWithoutExtension)
		{
			return TempFileFactory.CreateTempGrammarFile(fileNameWithoutExtension, ".grxml");
		}

		internal static ITempFile CreateTempGrammarFile()
		{
			return TempFileFactory.CreateTempGrammarFile(null, ".grxml");
		}

		internal static ITempFile CreateTempCompiledGrammarFile(string fileNameWithoutExtension)
		{
			return TempFileFactory.CreateTempGrammarFile(fileNameWithoutExtension, ".cfg");
		}

		internal static ITempFile CreateTempCompiledGrammarFile()
		{
			return TempFileFactory.CreateTempCompiledGrammarFile(Guid.NewGuid().ToString());
		}

		internal static ITempWavFile CreateTempWavFile()
		{
			return TempFileFactory.CreateTempWavFile(false);
		}

		internal static ITempWavFile CreateTempWavFile(string extraInfo)
		{
			ITempWavFile tempWavFile = TempFileFactory.CreateTempWavFile(false);
			tempWavFile.ExtraInfo = extraInfo;
			return tempWavFile;
		}

		internal static ITempWavFile CreateTempWavFile(bool addWriteAccessToo)
		{
			return TempFileFactory.CreateTempWavFile(addWriteAccessToo, ".wav");
		}

		internal static ITempWavFile CreateTempWavFile(bool addWriteAccessToo, string fileExtension)
		{
			TempFileFactory.TempWavFile tempWavFile = new TempFileFactory.TempWavFile(fileExtension, null);
			TempFileFactory.AddFileToTable(tempWavFile);
			TempFileFactory.AddNetworkServiceReadAccess(tempWavFile.FilePath, addWriteAccessToo);
			return tempWavFile;
		}

		internal static void DisposeSessionFiles(string sessionId)
		{
			lock (TempFileFactory.tempFileTable)
			{
				if (sessionId != null)
				{
					List<ITempFile> list;
					TempFileFactory.tempFileTable.TryGetValue(sessionId, out list);
					if (list != null)
					{
						lock (TempFileFactory.cleanUpDelayQueue)
						{
							TempFileFactory.cleanUpDelayQueue.Enqueue(list);
						}
						TempFileFactory.tempFileTable.Remove(sessionId);
					}
				}
			}
		}

		internal static void AddNetworkServiceReadAccess(string filePath, bool addWriteAccessToo)
		{
			if (!File.Exists(filePath))
			{
				using (File.Create(filePath))
				{
				}
			}
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
			NTAccount identity = (NTAccount)securityIdentifier.Translate(typeof(NTAccount));
			FileSecurity accessControl = File.GetAccessControl(filePath);
			accessControl.AddAccessRule(new FileSystemAccessRule(identity, addWriteAccessToo ? (FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.AppendData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.WriteExtendedAttributes | FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.ReadPermissions) : FileSystemRights.Read, AccessControlType.Allow));
			File.SetAccessControl(filePath, accessControl);
		}

		private static void AddFileToTable(ITempFile file)
		{
			lock (TempFileFactory.tempFileTable)
			{
				string text = CallId.Id;
				if (text == null)
				{
					text = "NULL";
				}
				List<ITempFile> list;
				TempFileFactory.tempFileTable.TryGetValue(text, out list);
				if (list == null)
				{
					list = new List<ITempFile>();
					TempFileFactory.tempFileTable.Add(text, list);
				}
				list.Add(file);
			}
		}

		private static ITempFile CreateTempFile(string fileName, string fileExtension)
		{
			TempFileFactory.TempFile tempFile = new TempFileFactory.TempFile(fileName, fileExtension);
			TempFileFactory.AddFileToTable(tempFile);
			return tempFile;
		}

		private static ITempFile CreateTempFile(string fileExtension)
		{
			return TempFileFactory.CreateTempFile(Guid.NewGuid().ToString(), fileExtension);
		}

		private static void CleanUpTimerCallback(object state)
		{
			bool flag = false;
			try
			{
				flag = TempFileFactory.timerSyncEvent.WaitOne(0, false);
				if (!flag || TempFileFactory.cleanUpTimer == null)
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.CleanUpTimerCallback: Shutdown or overlaping timer calls", new object[0]);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.CleanUpTimerCallback: Sync event acquired.", new object[0]);
					List<ITempFile> list = null;
					lock (TempFileFactory.cleanUpDelayQueue)
					{
						List<ITempFile> collection;
						while ((collection = TempFileFactory.cleanUpDelayQueue.Dequeue()) != null)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.CleanUpTimerCallback: Found files to delete.", new object[0]);
							if (list == null)
							{
								list = new List<ITempFile>();
							}
							list.AddRange(collection);
						}
					}
					if (list != null)
					{
						foreach (ITempFile tempFile in list)
						{
							tempFile.Dispose();
						}
					}
				}
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.CleanUpTimerCallback: {0}", new object[]
				{
					ex
				});
				if (!GrayException.IsGrayException(ex))
				{
					throw;
				}
				ExWatson.SendReport(ex, ReportOptions.None, null);
			}
			finally
			{
				if (flag)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "TempFileFactory.CleanUpTimerCallback: Signaling the sync event...", new object[0]);
					TempFileFactory.timerSyncEvent.Set();
				}
			}
		}

		private const string NullSessionId = "NULL";

		private static Dictionary<string, List<ITempFile>> tempFileTable = new Dictionary<string, List<ITempFile>>();

		private static RetryQueue<List<ITempFile>> cleanUpDelayQueue = new RetryQueue<List<ITempFile>>(ExTraceGlobals.UtilTracer, TimeSpan.FromMinutes(1.0));

		private static AutoResetEvent timerSyncEvent = new AutoResetEvent(false);

		private static Timer cleanUpTimer;

		private class TempFile : DisposableBase, ITempFile, IDisposeTrackable, IDisposable
		{
			internal TempFile(string fileName)
			{
				this.fileName = fileName;
			}

			internal TempFile(string fileName, string fileExtension) : this(fileName + fileExtension)
			{
			}

			public string FilePath
			{
				get
				{
					return Path.Combine(Utils.UMTempPath, this.fileName);
				}
			}

			public override string ToString()
			{
				return this.FilePath;
			}

			public void KeepAlive()
			{
				this.deleteOnDispose = false;
			}

			protected virtual void DeleteFile()
			{
				File.Delete(this.FilePath);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					if (!this.deleteOnDispose)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Skipping deletion of tempfile {0}", new object[]
						{
							this.FilePath
						});
						return;
					}
					try
					{
						if (this.FilePath != null)
						{
							this.DeleteFile();
						}
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Successfully deleted tempfile {0}", new object[]
						{
							this.FilePath
						});
					}
					catch (IOException ex)
					{
						CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "Could not delete file={0}", new object[]
						{
							ex
						});
					}
					catch (UnauthorizedAccessException ex2)
					{
						CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "Could not delete file. It might be used by a different process. {0}", new object[]
						{
							ex2
						});
					}
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<TempFileFactory.TempFile>(this);
			}

			private string fileName;

			private bool deleteOnDispose = true;
		}

		private class TempWavFile : TempFileFactory.TempFile, ITempWavFile, ITempFile, IDisposeTrackable, IDisposable
		{
			internal TempWavFile(string extraInfo) : this(".wav", extraInfo)
			{
			}

			internal TempWavFile(string extension, string extraInfo) : base(Guid.NewGuid().ToString(), extension)
			{
				this.extraInfo = extraInfo;
			}

			public string ExtraInfo
			{
				get
				{
					return this.extraInfo;
				}
				set
				{
					this.extraInfo = value;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<TempFileFactory.TempWavFile>(this);
			}

			private string extraInfo;
		}

		private class TempDir : TempFileFactory.TempFile
		{
			internal TempDir() : base(Guid.NewGuid().ToString())
			{
				Directory.CreateDirectory(base.FilePath);
			}

			protected override void DeleteFile()
			{
				Directory.Delete(base.FilePath, true);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<TempFileFactory.TempDir>(this);
			}
		}
	}
}
