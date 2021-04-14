using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SiteSynchronizer : Synchronizer
	{
		public SiteSynchronizer(DocumentSyncJob job, MailboxSession mailboxSession, IResourceMonitor resourceMonitor, string siteUrl, ICredentials credential, bool isOAuthCredential, bool enableHttpDebugProxy, Stream syncCycleLogStream) : base(job, mailboxSession, resourceMonitor, siteUrl, credential, isOAuthCredential, enableHttpDebugProxy, syncCycleLogStream)
		{
			this.loggingComponent = ProtocolLog.Component.DocumentSync;
		}

		public override IAsyncResult BeginExecute(AsyncCallback executeCallback, object state)
		{
			this.executeStopwatch = Stopwatch.StartNew();
			this.executionAsyncResult = new LazyAsyncResult(null, state, executeCallback);
			try
			{
				this.InitializeSyncMetadata();
				base.UpdateSyncMetadataOnBeginSync();
			}
			catch (StorageTransientException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SiteSynchronizer.BeginExecute:failed with StorageTransientException", ex);
				this.executionAsyncResult.InvokeCallback(ex);
				return this.executionAsyncResult;
			}
			catch (StoragePermanentException ex2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SiteSynchronizer.BeginExecute:failed with StoragePermanentException", ex2);
				this.executionAsyncResult.InvokeCallback(ex2);
				return this.executionAsyncResult;
			}
			this.OnGetDocumentLibraryListsComplete(this.executionAsyncResult);
			return this.executionAsyncResult;
		}

		public override void EndExecute(IAsyncResult asyncResult)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new InvalidOperationException("EndExecute: asyncResult or the AsynState cannot be null here.");
			}
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			this.executeStopwatch.Stop();
			base.LastError = (lazyAsyncResult.Result as Exception);
			base.SaveSyncMetadata();
			base.PublishMonitoringResult();
			if (base.LastError == null)
			{
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "SiteSynchronizer.EndExecute: Synchronization of hierarchy for this site mailbox has succeeded");
				ProtocolLog.LogStatistics(this.loggingComponent, this.loggingContext, string.Format("SiteSynchronizer.Statistics: Succeeded with ElapsMillisec:{0}, DocumentLibCount {1}", this.executeStopwatch.ElapsedMilliseconds, this.documentLibraryInfos.Count));
			}
			else
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SiteSynchronizer.EndExecute: Synchronization of hierarchy for this site mailbox has failed", base.LastError);
			}
			base.Dispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SiteSynchronizer>(this);
		}

		protected override void InitializeSyncMetadata()
		{
			if (this.syncMetadata == null)
			{
				this.syncMetadata = UserConfigurationHelper.GetMailboxConfiguration(this.mailboxSession, "SiteSynchronizerConfigurations", UserConfigurationTypes.Dictionary, true);
			}
		}

		protected override LocalizedString GetSyncIssueEmailErrorString(string error, out LocalizedString body)
		{
			body = ClientStrings.FailedToSynchronizeHierarchyChangesFromSharePointText(this.siteUri.AbsoluteUri, error);
			return ClientStrings.FailedToSynchronizeChangesFromSharePoint(this.siteUri.AbsoluteUri);
		}

		protected virtual void OnGetDocumentLibraryListsComplete(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new InvalidOperationException("OnGetDocumentLibraryListsComplete: asyncResult cannot be null here.");
			}
			ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SiteSynchronizer.BeginExecute", new object[0]));
			bool flag = (base.SyncOption & SyncOption.CurrentDocumentLibsOnly) == SyncOption.CurrentDocumentLibsOnly;
			Dictionary<Guid, List> dictionary = new Dictionary<Guid, List>();
			if (!flag)
			{
				try
				{
					this.InitializeDocumentLibraryList(dictionary);
					if (base.HandleShutDown())
					{
						return;
					}
				}
				catch (WebException e)
				{
					SharePointException value = new SharePointException(this.siteUri.AbsoluteUri, e, false);
					this.executionAsyncResult.InvokeCallback(value);
					return;
				}
				catch (IOException value2)
				{
					this.executionAsyncResult.InvokeCallback(value2);
					return;
				}
				catch (ClientRequestException value3)
				{
					this.executionAsyncResult.InvokeCallback(value3);
					return;
				}
				catch (ServerException value4)
				{
					this.executionAsyncResult.InvokeCallback(value4);
					return;
				}
			}
			List<StoreObjectId> list = new List<StoreObjectId>();
			List<List> list2 = new List<List>();
			Dictionary<StoreObjectId, List> dictionary2 = new Dictionary<StoreObjectId, List>();
			try
			{
				using (Folder folder = Folder.Bind(this.mailboxSession, DefaultFolderType.Root))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, new ExistsFilter(FolderSchema.LinkedId), null, new PropertyDefinition[]
					{
						FolderSchema.LinkedId,
						FolderSchema.Id,
						FolderSchema.DisplayName,
						FolderSchema.LinkedUrl,
						FolderSchema.LinkedSiteAuthorityUrl
					}))
					{
						object[][] rows;
						do
						{
							rows = queryResult.GetRows(10000);
							for (int i = 0; i < rows.Length; i++)
							{
								Guid guid = (Guid)rows[i][0];
								StoreObjectId objectId = ((VersionedId)rows[i][1]).ObjectId;
								string a = rows[i][2] as string;
								string text = rows[i][3] as string;
								string text2 = rows[i][4] as string;
								Uri uri = null;
								if (!string.IsNullOrEmpty(text2))
								{
									uri = new Uri(text2);
								}
								if (string.IsNullOrEmpty(text))
								{
									this.executionAsyncResult.InvokeCallback(new ArgumentNullException("LinkedUrl is null for folder " + objectId.ToString()));
								}
								if (!flag)
								{
									List list3 = null;
									if (dictionary.TryGetValue(guid, out list3))
									{
										if (!string.Equals(a, list3.Title, StringComparison.Ordinal) || (uri != null && !UriComparer.IsEqual(uri, this.siteUri)) || (this.Job.SyncOption & SyncOption.FullSync) == SyncOption.FullSync)
										{
											dictionary2[objectId] = dictionary[guid];
										}
										this.documentLibraryInfos.Enqueue(new DocumentLibraryInfo(objectId, list3.Id, list3.RootFolder.ServerRelativeUrl));
										dictionary.Remove(guid);
									}
									else
									{
										list.Add(objectId);
									}
								}
								else
								{
									this.documentLibraryInfos.Enqueue(new DocumentLibraryInfo(objectId, guid, new Uri(text).AbsolutePath));
								}
							}
						}
						while (rows.Length != 0);
						list2.AddRange(dictionary.Values);
					}
					if (list.Count > 0)
					{
						folder.DeleteObjects(DeleteItemFlags.HardDelete, list.ToArray());
					}
					foreach (KeyValuePair<StoreObjectId, List> keyValuePair in dictionary2)
					{
						if (base.HandleShutDown())
						{
							return;
						}
						using (Folder folder2 = Folder.Bind(this.mailboxSession, keyValuePair.Key))
						{
							Uri uri2 = new Uri(new Uri(this.siteUri.GetLeftPart(UriPartial.Authority)), keyValuePair.Value.RootFolder.ServerRelativeUrl);
							bool flag2 = Utils.HasFolderUriChanged(folder2, uri2);
							if (flag2)
							{
								folder2[FolderSchema.SharePointChangeToken] = string.Empty;
								folder2[FolderSchema.LinkedUrl] = uri2.AbsoluteUri;
								folder2[FolderSchema.LinkedSiteUrl] = this.siteUri.AbsoluteUri;
								string valueOrDefault = folder2.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
								ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SiteSynchronizer: Document Library URL has changed from {0} to {1}", valueOrDefault, uri2.AbsoluteUri));
							}
							folder2.DisplayName = keyValuePair.Value.Title;
							folder2[FolderSchema.LinkedSiteAuthorityUrl] = this.siteUri.AbsoluteUri;
							folder2.Save();
						}
					}
					foreach (List list4 in list2)
					{
						if (base.HandleShutDown())
						{
							return;
						}
						using (Folder folder3 = Folder.Create(this.mailboxSession, folder.Id.ObjectId, StoreObjectType.ShortcutFolder, list4.Title, CreateMode.OpenIfExists))
						{
							folder3[FolderSchema.LinkedId] = list4.Id;
							Uri uri3 = new Uri(new Uri(this.siteUri.GetLeftPart(UriPartial.Authority)), list4.RootFolder.ServerRelativeUrl);
							folder3[FolderSchema.LinkedUrl] = uri3.AbsoluteUri;
							folder3[FolderSchema.LinkedSiteUrl] = this.siteUri.AbsoluteUri;
							folder3[FolderSchema.LinkedListId] = list4.Id;
							folder3[FolderSchema.IsDocumentLibraryFolder] = true;
							folder3[FolderSchema.LinkedSiteAuthorityUrl] = this.siteUri.AbsoluteUri;
							folder3.Save();
							folder3.Load();
							ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SiteSynchronizer: Create or update top level document library Name:{0}, LinkedUrl: {1}, LinkedId {2}, ListId {3}", new object[]
							{
								folder3.DisplayName,
								folder3[FolderSchema.LinkedUrl],
								folder3[FolderSchema.LinkedId],
								folder3[FolderSchema.LinkedListId]
							}));
							this.documentLibraryInfos.Enqueue(new DocumentLibraryInfo(folder3.Id.ObjectId, list4.Id, list4.RootFolder.ServerRelativeUrl));
						}
					}
				}
				base.SyncResult = this.documentLibraryInfos;
				this.executionAsyncResult.InvokeCallback(null);
			}
			catch (StorageTransientException value5)
			{
				this.executionAsyncResult.InvokeCallback(value5);
			}
			catch (StoragePermanentException value6)
			{
				this.executionAsyncResult.InvokeCallback(value6);
			}
		}

		private void InitializeDocumentLibraryList(Dictionary<Guid, List> documentLibraries)
		{
			using (ClientContext clientContext = new ClientContext(this.siteUri.AbsoluteUri))
			{
				clientContext.Credentials = this.credential;
				if (this.isOAuthCredential)
				{
					clientContext.FormDigestHandlingEnabled = false;
					clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
					{
						args.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, "Bearer");
						args.WebRequestExecutor.RequestHeaders.Add("client-request-id", Guid.NewGuid().ToString());
						args.WebRequestExecutor.RequestHeaders.Add("return-client-request-id", "true");
						args.WebRequestExecutor.WebRequest.PreAuthenticate = true;
						args.WebRequestExecutor.WebRequest.UserAgent = Utils.GetUserAgentStringForSiteMailboxRequests();
					};
				}
				if (this.enableHttpDebugProxy)
				{
					clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
					{
						args.WebRequestExecutor.WebRequest.Proxy = new WebProxy("127.0.0.1", 8888);
					};
				}
				clientContext.Load<ListCollection>(clientContext.Web.Lists, new Expression<Func<ListCollection, object>>[]
				{
					(ListCollection items) => ClientObjectQueryableExtension.Include<List>(items, new Expression<Func<List, object>>[]
					{
						(List item) => (object)item.Id,
						(List item) => (object)item.BaseType,
						(List item) => (object)item.OnQuickLaunch,
						(List item) => item.Title,
						(List item) => item.RootFolder
					})
				});
				clientContext.ExecuteQuery();
				foreach (List list in clientContext.Web.Lists)
				{
					if (list.BaseType == 1 && list.OnQuickLaunch)
					{
						documentLibraries.Add(list.Id, list);
					}
				}
			}
		}

		private readonly Queue<DocumentLibraryInfo> documentLibraryInfos = new Queue<DocumentLibraryInfo>();
	}
}
