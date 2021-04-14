using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LargeGrammarFetcher
	{
		internal LargeGrammarFetcher(GrammarIdentifier grammarId)
		{
			ValidateArgument.NotNull(grammarId, "GrammarIdentifier");
			LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher: C'tor, grammarId = {0}", new object[]
			{
				grammarId
			});
			this.grammarId = grammarId;
			LargeGrammarFetcher.LocalGrammarCache.Instance.TryPrepareGrammar(this.grammarId, out this.grammarFilePath, out this.grammarAvailableEvent);
		}

		internal GrammarIdentifier GrammarId
		{
			get
			{
				return this.grammarId;
			}
		}

		internal void WaitAsync(LargeGrammarFetcher.GrammarDownloadedCallBack callback, TimeSpan timeout, object userstate)
		{
			ValidateArgument.NotNull(callback, "GrammarDownloadedCallBack");
			ValidateArgument.NotNull(timeout, "Timeout");
			LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:FetchAsync, Timeout = {0}", new object[]
			{
				timeout
			});
			this.CheckAndThrowIfAlreadyFetching();
			this.userCallback = callback;
			this.userTimeout = timeout;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteUserCallback), userstate);
		}

		internal LargeGrammarFetcher.FetchResult Wait(TimeSpan timeout)
		{
			ValidateArgument.NotNull(timeout, "Timeout");
			LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:FetchSync, Timeout = {0}", new object[]
			{
				timeout
			});
			this.CheckAndThrowIfAlreadyFetching();
			LargeGrammarFetcher.FetchResult result = null;
			try
			{
				result = this.GetFetchResult(timeout);
			}
			finally
			{
				Interlocked.Exchange(ref this.grammarFetchInProgress, 0);
			}
			return result;
		}

		private void CheckAndThrowIfAlreadyFetching()
		{
			if (Interlocked.CompareExchange(ref this.grammarFetchInProgress, 1, 0) != 0)
			{
				throw new InvalidOperationException("A Grammar Fetch Operation is already in progress");
			}
		}

		private void ExecuteUserCallback(object state)
		{
			try
			{
				this.userCallback(state, this.GetFetchResult(this.userTimeout));
			}
			finally
			{
				Interlocked.Exchange(ref this.grammarFetchInProgress, 0);
			}
		}

		private LargeGrammarFetcher.FetchResult GetFetchResult(TimeSpan timeout)
		{
			LargeGrammarFetcher.FetchResult fetchResult = null;
			if (this.grammarFilePath != null)
			{
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:GetFetchResult, FileOnDisk path = {0}", new object[]
				{
					this.grammarFilePath
				});
				fetchResult = new LargeGrammarFetcher.FetchResult(this.grammarFilePath, LargeGrammarFetcher.FetchStatus.Success);
			}
			else if (this.grammarAvailableEvent.SafeWaitHandle.IsClosed)
			{
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:GetFetchResult, WaitHandle closed", new object[0]);
				fetchResult = this.ReFetchGrammarFilePath();
			}
			else
			{
				try
				{
					if (this.grammarAvailableEvent.WaitOne(timeout))
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:GetFetchResult, Wait finished early", new object[0]);
						fetchResult = this.ReFetchGrammarFilePath();
					}
					else
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:GetFetchResult, Wait timed out", new object[0]);
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMGrammarFetcherError, null, new object[]
						{
							this.grammarId,
							CommonUtil.ToEventLogString(Strings.SpeechGrammarFetchTimeoutException(this.grammarId.ToString()))
						});
						fetchResult = new LargeGrammarFetcher.FetchResult(this.grammarFilePath, LargeGrammarFetcher.FetchStatus.Timeout);
					}
				}
				catch (ObjectDisposedException ex)
				{
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:GetFetchResult, got expected exception ={0}", new object[]
					{
						ex
					});
					fetchResult = this.ReFetchGrammarFilePath();
				}
			}
			if (fetchResult.Status == LargeGrammarFetcher.FetchStatus.Success)
			{
				UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMGrammarUsage.ToString());
			}
			else
			{
				UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMGrammarUsage.ToString());
			}
			return fetchResult;
		}

		private LargeGrammarFetcher.FetchResult ReFetchGrammarFilePath()
		{
			LargeGrammarFetcher.LocalGrammarCache.Instance.TryCheckIfFileExistsInCache(this.grammarId, out this.grammarFilePath);
			LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LargeGrammarFetcher:ReFetchGrammarFilePath, grammarFilePath={0}", new object[]
			{
				string.IsNullOrEmpty(this.grammarFilePath) ? "null" : this.grammarFilePath
			});
			return new LargeGrammarFetcher.FetchResult(this.grammarFilePath, string.IsNullOrEmpty(this.grammarFilePath) ? LargeGrammarFetcher.FetchStatus.Error : LargeGrammarFetcher.FetchStatus.Success);
		}

		private GrammarIdentifier grammarId;

		private int grammarFetchInProgress;

		private LargeGrammarFetcher.GrammarDownloadedCallBack userCallback;

		private TimeSpan userTimeout;

		private string grammarFilePath;

		private ManualResetEvent grammarAvailableEvent;

		internal enum FetchStatus
		{
			Success,
			Timeout,
			Error
		}

		internal delegate void GrammarDownloadedCallBack(object userState, LargeGrammarFetcher.FetchResult result);

		internal class FetchResult
		{
			internal string FilePath { get; private set; }

			internal LargeGrammarFetcher.FetchStatus Status { get; private set; }

			internal FetchResult(string filePath, LargeGrammarFetcher.FetchStatus status)
			{
				this.FilePath = filePath;
				this.Status = status;
			}
		}

		private class LocalGrammarCache
		{
			private LocalGrammarCache()
			{
				this.grammarsBeingDownloaded = new Dictionary<GrammarIdentifier, ManualResetEvent>();
				this.grammarsBeingUpdated = new HashSet<GrammarIdentifier>();
				this.grammarsBeingDownloadedLock = new object();
				this.grammarsBeingUpdatedLock = new object();
			}

			internal static LargeGrammarFetcher.LocalGrammarCache Instance
			{
				get
				{
					return LargeGrammarFetcher.LocalGrammarCache.grammarCache;
				}
			}

			public bool TryPrepareGrammar(GrammarIdentifier grammarId, out string path, out ManualResetEvent grammarWaitHandle)
			{
				ValidateArgument.NotNull(grammarId, "grammarId");
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.TryPrepareGrammar, Grammar='{0}'", new object[]
				{
					grammarId
				});
				path = null;
				grammarWaitHandle = null;
				if (!this.TryCheckIfFileExistsInCache(grammarId, out path))
				{
					grammarWaitHandle = this.FetchGrammarAsync(grammarId);
				}
				else
				{
					this.CheckForUpdatesAsync(grammarId);
				}
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.TryPrepareGrammar, Grammar='{0}', Path='{1}'", new object[]
				{
					grammarId,
					path ?? "<null>"
				});
				return path != null;
			}

			public bool TryCheckIfFileExistsInCache(GrammarIdentifier grammarId, out string path)
			{
				ValidateArgument.NotNull(grammarId, "grammarId");
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.TryCheckIfFileExistsInCache, Grammar='{0}'", new object[]
				{
					grammarId
				});
				path = null;
				string tmp = null;
				LargeGrammarFetcher.LocalGrammarCache.DoGrammarOperation(grammarId, delegate
				{
					tmp = LargeGrammarFetcher.GrammarFetcherUtils.GetLatestGrammarFilePathInCache(grammarId);
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.TryCheckIfFileExistsInCache, Grammar='{0}', Path='{1}'", new object[]
					{
						grammarId,
						tmp ?? "<null>"
					});
				}, delegate
				{
				});
				path = tmp;
				return path != null;
			}

			private static bool IsExpectedException(Exception e)
			{
				return e is IOException || e is UnauthorizedAccessException || e is GrammarFileNotFoundException || e is LocalizedException;
			}

			private static void DoGrammarOperation(GrammarIdentifier grammarId, Action operation, Action cleanup)
			{
				try
				{
					operation();
				}
				catch (Exception ex)
				{
					LargeGrammarFetcher.GrammarFetcherUtils.ErrorTrace("LocalGrammarCache.DoGrammarOperation, Grammar='{0}', Error:'{1}'", new object[]
					{
						grammarId,
						ex
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMGrammarFetcherError, null, new object[]
					{
						grammarId,
						CommonUtil.ToEventLogString(ex)
					});
					if (!LargeGrammarFetcher.LocalGrammarCache.IsExpectedException(ex))
					{
						ExceptionHandling.SendWatsonWithExtraData(ex, false);
						throw;
					}
				}
				finally
				{
					cleanup();
				}
			}

			private ManualResetEvent FetchGrammarAsync(GrammarIdentifier grammarId)
			{
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammarAsync, grammarId='{0}'", new object[]
				{
					grammarId
				});
				ManualResetEvent manualResetEvent = null;
				lock (this.grammarsBeingDownloadedLock)
				{
					if (!this.grammarsBeingDownloaded.ContainsKey(grammarId))
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammarAsync, Adding grammarId='{0}' to grammarsBeingDownloaded", new object[]
						{
							grammarId
						});
						manualResetEvent = new ManualResetEvent(false);
						this.grammarsBeingDownloaded.Add(grammarId, manualResetEvent);
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.FetchGrammar), grammarId);
					}
					else
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammarAsync, grammarId='{0}' already in grammarsBeingDownloaded", new object[]
						{
							grammarId
						});
						manualResetEvent = this.grammarsBeingDownloaded[grammarId];
					}
				}
				return manualResetEvent;
			}

			private void FetchGrammar(object state)
			{
				GrammarIdentifier grammarId = (GrammarIdentifier)state;
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammar grammarId='{0}'", new object[]
				{
					grammarId
				});
				LargeGrammarFetcher.LocalGrammarCache.DoGrammarOperation(grammarId, delegate
				{
					string latestGrammarFilePathInShare = LargeGrammarFetcher.GrammarFetcherUtils.GetLatestGrammarFilePathInShare(grammarId);
					if (latestGrammarFilePathInShare != null)
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DownloadGrammarFromShare(grammarId, latestGrammarFilePathInShare, DateTime.MinValue);
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammar, Downloaded grammarId='{0}' from share path='{1}'", new object[]
						{
							grammarId,
							latestGrammarFilePathInShare
						});
					}
					else
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammar, Try to download from mailbox, grammarId='{0}'", new object[]
						{
							grammarId
						});
						bool flag = LargeGrammarFetcher.GrammarFetcherUtils.DownloadGrammarFromMailbox(grammarId, DateTime.MinValue);
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammar, Download from mailbox, grammarId='{0}', grammarDownloaded='{1}'", new object[]
						{
							grammarId,
							flag
						});
						if (!flag)
						{
							throw new GrammarFileNotFoundException(grammarId.ToString());
						}
					}
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.FetchGrammar, Downloaded grammarId='{0}'", new object[]
					{
						grammarId
					});
				}, delegate
				{
					lock (this.grammarsBeingDownloadedLock)
					{
						ManualResetEvent manualResetEvent = this.grammarsBeingDownloaded[grammarId];
						manualResetEvent.Set();
						manualResetEvent.Close();
						this.grammarsBeingDownloaded.Remove(grammarId);
					}
				});
			}

			private void CheckForUpdatesAsync(GrammarIdentifier grammarId)
			{
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdatesAsync grammarId='{0}'", new object[]
				{
					grammarId
				});
				lock (this.grammarsBeingUpdatedLock)
				{
					if (!this.grammarsBeingUpdated.Contains(grammarId))
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdatesAsync, Adding grammarId='{0}' to grammarsBeingUpdated", new object[]
						{
							grammarId
						});
						this.grammarsBeingUpdated.Add(grammarId);
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.CheckForUpdates), grammarId);
					}
				}
			}

			private void CheckForUpdates(object state)
			{
				GrammarIdentifier grammarId = (GrammarIdentifier)state;
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdates, grammarId='{0}'", new object[]
				{
					grammarId
				});
				LargeGrammarFetcher.LocalGrammarCache.DoGrammarOperation(grammarId, delegate
				{
					string path;
					if (this.TryCheckIfFileExistsInCache(grammarId, out path))
					{
						bool flag = false;
						DateTime creationTimeUtc = File.GetCreationTimeUtc(path);
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdates, grammarId='{0}', cachedCreationTimeUtc ='{1}'", new object[]
						{
							grammarId,
							creationTimeUtc
						});
						string latestGrammarFilePathInShare = LargeGrammarFetcher.GrammarFetcherUtils.GetLatestGrammarFilePathInShare(grammarId);
						if (latestGrammarFilePathInShare != null)
						{
							LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdates, Downloading grammarId='{0}' from share '{1}'", new object[]
							{
								grammarId,
								latestGrammarFilePathInShare
							});
							flag = LargeGrammarFetcher.GrammarFetcherUtils.DownloadGrammarFromShare(grammarId, latestGrammarFilePathInShare, creationTimeUtc);
							LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdates, Download attempt grammarId='{0}', share='{1}', downloadedGrammar='{2}'", new object[]
							{
								grammarId,
								latestGrammarFilePathInShare,
								flag
							});
						}
						if (!flag && (DateTime.UtcNow - TimeSpan.FromDays(5.0)).CompareTo(creationTimeUtc) > 0)
						{
							LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdates, Try to download from mailbox, grammarId='{0}'", new object[]
							{
								grammarId
							});
							flag = LargeGrammarFetcher.GrammarFetcherUtils.DownloadGrammarFromMailbox(grammarId, creationTimeUtc);
							LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("LocalGrammarCache.CheckForUpdates, Download attempt grammarId='{0}', downloadedGrammar='{1}'", new object[]
							{
								grammarId,
								flag
							});
						}
					}
				}, delegate
				{
					lock (this.grammarsBeingUpdatedLock)
					{
						this.grammarsBeingUpdated.Remove(grammarId);
					}
				});
			}

			private static readonly LargeGrammarFetcher.LocalGrammarCache grammarCache = new LargeGrammarFetcher.LocalGrammarCache();

			private readonly Dictionary<GrammarIdentifier, ManualResetEvent> grammarsBeingDownloaded;

			private readonly object grammarsBeingDownloadedLock;

			private readonly HashSet<GrammarIdentifier> grammarsBeingUpdated;

			private readonly object grammarsBeingUpdatedLock;
		}

		private class GrammarFetcherUtils
		{
			public static void DebugTrace(string formatString, params object[] formatObjects)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, formatString, formatObjects);
			}

			public static void ErrorTrace(string formatString, params object[] formatObjects)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UMGrammarGeneratorTracer, null, formatString, formatObjects);
			}

			public static string GetLatestGrammarFilePathInCache(GrammarIdentifier grammarId)
			{
				ValidateArgument.NotNull(grammarId, "grammarId");
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInCache grammar='{0}'", new object[]
				{
					grammarId
				});
				FileInfo[] cachedGrammarFiles = LargeGrammarFetcher.GrammarFetcherUtils.GetCachedGrammarFiles(grammarId);
				FileInfo fileInfo = null;
				if (cachedGrammarFiles != null)
				{
					foreach (FileInfo fileInfo2 in cachedGrammarFiles)
					{
						if (fileInfo == null || fileInfo2.CreationTimeUtc > fileInfo.CreationTimeUtc)
						{
							fileInfo = fileInfo2;
						}
					}
				}
				string text = (fileInfo == null) ? null : fileInfo.FullName;
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils:GetLatestGrammarFilePathInCache grammar='{0}' returning '{1}'", new object[]
				{
					grammarId,
					text ?? "<null>"
				});
				return text;
			}

			public static bool DownloadGrammarFromShare(GrammarIdentifier grammarId, string grammarFilePath, DateTime threshold)
			{
				ValidateArgument.NotNull(grammarId, "grammarId");
				ValidateArgument.NotNullOrEmpty(grammarFilePath, "grammarFilePath");
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.DownloadGrammarFromShare, grammar='{0}', grammarFilePath='{1}', threshold='{2}'", new object[]
				{
					grammarId,
					grammarFilePath,
					threshold
				});
				bool result = false;
				DateTime dateTime = (threshold == DateTime.MinValue) ? DateTime.MaxValue : File.GetCreationTimeUtc(grammarFilePath);
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.DownloadGrammarFromShare, grammar='{0}', grammarFilePath='{1}', fileCreationTimeUtc='{2}'", new object[]
				{
					grammarId,
					grammarFilePath,
					dateTime
				});
				if (dateTime > threshold)
				{
					LargeGrammarFetcher.GrammarFetcherUtils.UpdateCache(grammarId, grammarFilePath);
					result = true;
				}
				return result;
			}

			public static bool DownloadGrammarFromMailbox(GrammarIdentifier grammarId, DateTime threshold)
			{
				ValidateArgument.NotNull(grammarId, "grammarId");
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.DownloadGrammarFromMailbox, grammar='{0}', threshold='{1}'", new object[]
				{
					grammarId,
					threshold
				});
				bool result = false;
				Guid systemMailboxGuid = GrammarIdentifier.GetSystemMailboxGuid(grammarId.OrgId);
				if (systemMailboxGuid != Guid.Empty)
				{
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.DownloadGrammarFromMailbox, grammar='{0}', mailbox='{1}'", new object[]
					{
						grammarId,
						systemMailboxGuid
					});
					GrammarMailboxFileStore grammarMailboxFileStore = GrammarMailboxFileStore.FromMailboxGuid(grammarId.OrgId, systemMailboxGuid);
					string text = grammarMailboxFileStore.DownloadGrammar(grammarId.GrammarName + ".grxml", grammarId.Culture, threshold);
					if (text != null)
					{
						LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.DownloadGrammarFromMailbox, tmpFilePath='{0}'", new object[]
						{
							text
						});
						try
						{
							LargeGrammarFetcher.GrammarFetcherUtils.UpdateCache(grammarId, text);
						}
						finally
						{
							Util.TryDeleteFile(text);
						}
						result = true;
					}
				}
				return result;
			}

			public static string GetLatestGrammarFilePathInShare(GrammarIdentifier grammarId)
			{
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInShare, grammar='{0}'", new object[]
				{
					grammarId
				});
				Guid mailboxGuid = GrammarFileDistributionShare.GetMailboxGuid(grammarId.OrgId);
				string grammarManifestPath = GrammarFileDistributionShare.GetGrammarManifestPath(grammarId.OrgId, mailboxGuid);
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInShare, grammar='{0}', manifestPath='{1}'", new object[]
				{
					grammarId,
					grammarManifestPath
				});
				string text = null;
				if (mailboxGuid != Guid.Empty && File.Exists(grammarManifestPath))
				{
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInShare, grammar='{0}', manifestPath='{1}' exists", new object[]
					{
						grammarId,
						grammarManifestPath
					});
					GrammarGenerationMetadata grammarGenerationMetadata = GrammarGenerationMetadata.Deserialize(grammarManifestPath);
					string grammarFileFolderPath = GrammarFileDistributionShare.GetGrammarFileFolderPath(grammarId.OrgId, mailboxGuid, grammarGenerationMetadata.RunId, grammarId.Culture);
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInShare, grammar='{0}', grammarFolderPath='{1}'", new object[]
					{
						grammarId,
						grammarFileFolderPath
					});
					text = Path.Combine(grammarFileFolderPath, grammarId.GrammarName + ".grxml");
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInShare, grammar='{0}', grammarFilePath='{1}'", new object[]
					{
						grammarId,
						text
					});
				}
				else
				{
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetLatestGrammarFilePathInShare, grammar='{0}', manifestPath='{1}' does not exist", new object[]
					{
						grammarId,
						grammarManifestPath
					});
				}
				return text;
			}

			private static string GetUniqueCompiledGrammarFilePath(GrammarIdentifier grammarId)
			{
				int num = 0;
				string tenantTopLevelGrammarDirPath = grammarId.TenantTopLevelGrammarDirPath;
				string text;
				do
				{
					ExAssert.RetailAssert(num < 100, "Couldn't generate unique name!");
					string path = string.Format(CultureInfo.InvariantCulture, "{0}-{1}{2}", new object[]
					{
						grammarId.GrammarName,
						num.ToString(CultureInfo.InvariantCulture),
						".cfg"
					});
					text = Path.Combine(tenantTopLevelGrammarDirPath, path);
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetUniqueCompiledGrammarFilePath, grammar='{0}', Checking filePath='{1}'", new object[]
					{
						grammarId,
						text
					});
					num++;
				}
				while (File.Exists(text));
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetUniqueCompiledGrammarFilePath, grammar='{0}', filePath='{1}'", new object[]
				{
					grammarId,
					text
				});
				return text;
			}

			public static void UpdateCache(GrammarIdentifier grammarId, string grammarFilePath)
			{
				ValidateArgument.NotNull(grammarId, "grammarId");
				ValidateArgument.NotNull(grammarFilePath, "grammarFilePath");
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.UpdateCache, grammar='{0}', grammarFilePath='{1}'", new object[]
				{
					grammarId,
					grammarFilePath
				});
				string tenantTopLevelGrammarDirPath = grammarId.TenantTopLevelGrammarDirPath;
				Directory.CreateDirectory(tenantTopLevelGrammarDirPath);
				string text = Path.Combine(tenantTopLevelGrammarDirPath, Guid.NewGuid().ToString());
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.UpdateCache, grammar='{0}', tmpCompiledGrammarFilePath='{1}'", new object[]
				{
					grammarId,
					text
				});
				Platform.Utilities.CompileGrammar(grammarFilePath, text, grammarId.Culture);
				string uniqueCompiledGrammarFilePath = LargeGrammarFetcher.GrammarFetcherUtils.GetUniqueCompiledGrammarFilePath(grammarId);
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.UpdateCache, grammar='{0}', finalFilePath='{1}'", new object[]
				{
					grammarId,
					uniqueCompiledGrammarFilePath
				});
				File.Move(text, uniqueCompiledGrammarFilePath);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMGrammarFetcherSuccess, null, new object[]
				{
					grammarId,
					uniqueCompiledGrammarFilePath
				});
				LargeGrammarFetcher.GrammarFetcherUtils.CleanUpOldGrammarFiles(grammarId, uniqueCompiledGrammarFilePath);
			}

			private static void CleanUpOldGrammarFiles(GrammarIdentifier grammarId, string currentFilePath)
			{
				Utils.TryDiskOperation(delegate
				{
					FileInfo[] cachedGrammarFiles = LargeGrammarFetcher.GrammarFetcherUtils.GetCachedGrammarFiles(grammarId);
					if (cachedGrammarFiles != null)
					{
						foreach (FileInfo fileInfo in cachedGrammarFiles)
						{
							string fullName = fileInfo.FullName;
							if (!string.Equals(fullName, currentFilePath, StringComparison.OrdinalIgnoreCase))
							{
								LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.CleanUpOldGrammarFiles, Adding sentinel file for '{0}'", new object[]
								{
									fullName
								});
								string path = fullName + ".delete";
								if (!File.Exists(path))
								{
									File.Create(path).Close();
								}
							}
						}
					}
				}, delegate(Exception exception)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_GrammarFetcherCleanupFailed, null, new object[]
					{
						grammarId.OrgId,
						CommonUtil.ToEventLogString(exception)
					});
				});
			}

			private static FileInfo[] GetCachedGrammarFiles(GrammarIdentifier grammarId)
			{
				LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetCachedGrammarFiles grammar='{0}'", new object[]
				{
					grammarId
				});
				FileInfo[] result = null;
				DirectoryInfo directoryInfo = new DirectoryInfo(grammarId.TenantTopLevelGrammarDirPath);
				if (directoryInfo.Exists)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "{0}-{1}{2}", new object[]
					{
						grammarId.GrammarName,
						"*",
						".cfg"
					});
					LargeGrammarFetcher.GrammarFetcherUtils.DebugTrace("GrammarFetcherUtils.GetCachedGrammarFiles grammar='{0}', searchPattern='{1}'", new object[]
					{
						grammarId,
						text
					});
					result = directoryInfo.GetFiles(text, SearchOption.TopDirectoryOnly);
				}
				return result;
			}

			private const string CompiledGrammarNameFormat = "{0}-{1}{2}";
		}
	}
}
