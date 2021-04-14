using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.SharePoint.SOAP;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentLibSynchronizer : Synchronizer
	{
		public DocumentLibSynchronizer(DocumentSyncJob job, MailboxSession mailboxSession, IResourceMonitor resourceMonitor, StoreObjectId documentLibraryFolderId, string siteUrl, Guid documentLibraryGuid, ICredentials credential, bool isOAuthCredential, bool enableHttpDebugProxy, Stream syncCycleLogStream) : base(job, mailboxSession, resourceMonitor, siteUrl, credential, isOAuthCredential, enableHttpDebugProxy, syncCycleLogStream)
		{
			if (documentLibraryFolderId == null)
			{
				throw new ArgumentNullException("documentLibraryFolderId");
			}
			if (documentLibraryGuid == Guid.Empty)
			{
				throw new ArgumentNullException("documentLibraryGuid");
			}
			this.documentLibraryFolderId = documentLibraryFolderId;
			this.documentLibraryGuid = documentLibraryGuid;
			this.workLoadSize = 20;
			this.loggingComponent = ProtocolLog.Component.DocumentSync;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.listsWebSvc != null)
				{
					this.listsWebSvc.Dispose();
					this.listsWebSvc = null;
				}
				if (this.httpClient != null)
				{
					this.httpClient.Dispose();
					this.httpClient = null;
				}
				if (this.httpSessionConfig != null && this.httpSessionConfig.RequestStream != null)
				{
					this.httpSessionConfig.RequestStream.Close();
					this.httpSessionConfig.RequestStream = null;
					this.httpSessionConfig = null;
				}
				if (this.delayTimer != null)
				{
					this.delayTimer.Dispose();
					this.delayTimer = null;
				}
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DocumentLibSynchronizer>(this);
		}

		private static XmlNode GetXmlNode(string element)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(element);
			return safeXmlDocument.DocumentElement;
		}

		private static bool TryParseValue(string input, out string result)
		{
			result = null;
			int num = input.IndexOf('#');
			int num2 = input.Length - num - 1;
			if (num2 == 0)
			{
				return false;
			}
			result = input.Substring(num + 1, num2);
			return true;
		}

		private static XmlNode GetPagingQueryOption(string pageInfo)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml("<queryOptions><DateInUtc>TRUE</DateInUtc></queryOptions>");
			XmlElement documentElement = safeXmlDocument.DocumentElement;
			XmlElement xmlElement = safeXmlDocument.CreateElement("Paging");
			xmlElement.InnerText = "TRUE";
			documentElement.AppendChild(xmlElement);
			if (!string.IsNullOrEmpty(pageInfo))
			{
				documentElement["Paging"].SetAttribute("ListItemCollectionPositionNext", pageInfo);
			}
			return documentElement;
		}

		private static LinkedItemProps GetLinkedItemProps(object[] props)
		{
			string text = props[2] as string;
			Uri linkedUri = null;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					linkedUri = new Uri(text);
				}
				catch (UriFormatException)
				{
				}
			}
			return new LinkedItemProps(((VersionedId)props[0]).ObjectId, (StoreObjectId)props[1], linkedUri, props[3] as ExDateTime?);
		}

		private bool UnderFullSync
		{
			get
			{
				return string.IsNullOrEmpty(this.inputToken);
			}
		}

		public override IAsyncResult BeginExecute(AsyncCallback executeCallback, object state)
		{
			this.executionAsyncResult = new LazyAsyncResult(null, state, executeCallback);
			this.performanceCounter.Start(OperationType.EndToEnd);
			try
			{
				this.InitializeSyncMetadata();
				base.UpdateSyncMetadataOnBeginSync();
				using (Folder folder = Folder.Bind(this.mailboxSession, this.documentLibraryFolderId))
				{
					this.inputToken = ((this.Job != null && (this.Job.SyncOption & SyncOption.FullSync) == SyncOption.FullSync) ? null : folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.SharePointChangeToken, null));
				}
			}
			catch (StorageTransientException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "BeginExecute: Read change token failed with StorageTransientException", ex);
				this.executionAsyncResult.InvokeCallback(ex);
				return this.executionAsyncResult;
			}
			catch (StoragePermanentException ex2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "BeginExecute: Read change token failed with StoragePermanentException", ex2);
				this.executionAsyncResult.InvokeCallback(ex2);
				return this.executionAsyncResult;
			}
			ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("DocumentLibSynchronizer.BeginExecute: with inputToken {0}", this.inputToken));
			this.InternalBeginGetListItemChangesSinceToken(null);
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
			this.performanceCounter.Stop(OperationType.EndToEnd, 1);
			this.resourceMonitor.ResetBudget();
			base.LastError = (lazyAsyncResult.Result as Exception);
			base.SaveSyncMetadata();
			if (base.LastError != null)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "DocumentLibSynchronizer.EndExecute: Synchronization for this document library has failed", base.LastError);
			}
			else
			{
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "DocumentLibSynchronizer.EndExecute: Synchronization for this document library has succeeded");
			}
			string[] logLine = this.performanceCounter.GetLogLine();
			foreach (string str in logLine)
			{
				ProtocolLog.LogStatistics(this.loggingComponent, this.loggingContext, "DocumentLibSynchronizer.Statistics:" + str);
			}
			base.Dispose();
		}

		protected override void InitializeSyncMetadata()
		{
			if (this.syncMetadata == null)
			{
				this.syncMetadata = UserConfigurationHelper.GetFolderConfiguration(this.mailboxSession, this.documentLibraryFolderId, "DocumentLibSynchronizerConfigurations", UserConfigurationTypes.Dictionary, true, false);
			}
		}

		protected override LocalizedString GetSyncIssueEmailErrorString(string error, out LocalizedString body)
		{
			LocalizedString result;
			using (Folder folder = Folder.Bind(this.mailboxSession, this.documentLibraryFolderId))
			{
				string valueOrDefault = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
				body = ClientStrings.FailedToSynchronizeChangesFromSharePointText(valueOrDefault, error);
				result = ClientStrings.FailedToSynchronizeChangesFromSharePoint(valueOrDefault);
			}
			return result;
		}

		protected virtual void InternalBeginGetListItemChangesSinceToken(string pageInfo)
		{
			this.performanceCounter.Start(OperationType.SharePointQuery);
			if (this.isOAuthCredential)
			{
				this.InternalBeginGetListItemChangesSinceTokenViaRest(pageInfo);
				return;
			}
			this.InternalBeginGetListItemChangesSinceTokenViaSoap(pageInfo);
		}

		private void InternalBeginGetListItemChangesSinceTokenViaSoap(string pageInfo)
		{
			this.IntializeWebService();
			this.listsWebSvc.BeginGetListItemChangesSinceToken(this.documentLibraryGuid.ToString("B"), null, null, DocumentLibSynchronizer.SelectedViewFields, ((TeamMailboxSyncConfiguration)this.Job.Config).SharePointQueryPageSize.ToString(), this.UnderFullSync ? DocumentLibSynchronizer.GetPagingQueryOption(pageInfo) : DocumentLibSynchronizer.DefaultQueryOption, this.inputToken, null, base.WrapCallbackWithUnhandledExceptionAndSendReport(new AsyncCallback(this.OnGetListItemChangesSinceTokenComplete)), pageInfo);
		}

		private void InternalBeginGetListItemChangesSinceTokenViaRest(string pageInfo)
		{
			base.InitializeHttpClient("POST");
			this.SetRestRequestStream(pageInfo);
			StringBuilder stringBuilder = new StringBuilder(this.siteUri.AbsoluteUri);
			stringBuilder.Append("/_vti_bin/client.svc/web/lists(guid'");
			stringBuilder.Append(this.documentLibraryGuid.ToString("D"));
			stringBuilder.Append("')/GetListItemChangesSinceToken");
			base.SetCommonOauthRequestHeaders();
			this.httpSessionConfig.ContentType = "application/json;odata=verbose";
			this.httpClient.BeginDownload(new Uri(stringBuilder.ToString()), this.httpSessionConfig, base.WrapCallbackWithUnhandledExceptionAndSendReportEx(new AsyncCallback(this.OnGetListItemChangesSinceTokenComplete)), pageInfo);
		}

		protected virtual void OnGetListItemChangesSinceTokenComplete(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new InvalidOperationException("OnGetListItemChangesSinceTokenComplete: asyncResult or the AsynState cannot be null here.");
			}
			if (base.HandleShutDown())
			{
				return;
			}
			string text = (string)asyncResult.AsyncState;
			Exception ex = null;
			XmlReader xmlReader = null;
			if (this.isOAuthCredential)
			{
				xmlReader = this.EndGetListItemChangesSinceTokenViaRest((ICancelableAsyncResult)asyncResult, out ex);
			}
			else
			{
				xmlReader = this.EndGetListItemChangesSinceTokenViaSoap(asyncResult, out ex);
			}
			if (ex != null)
			{
				this.executionAsyncResult.InvokeCallback(ex);
				return;
			}
			bool flag = false;
			string value = null;
			string text2 = null;
			ChangedItem changedItem = null;
			bool flag2 = false;
			using (xmlReader)
			{
				try
				{
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							if (xmlReader.Name.Equals("Changes"))
							{
								string attribute = xmlReader.GetAttribute("MoreChanges");
								if (!string.IsNullOrEmpty(attribute))
								{
									bool.TryParse(attribute, out flag);
								}
								value = xmlReader.GetAttribute("LastChangeToken");
								if (xmlReader.ReadToDescendant("Id"))
								{
									for (;;)
									{
										string attribute2 = xmlReader.GetAttribute("ChangeType");
										if (!string.IsNullOrEmpty(attribute2))
										{
											if (attribute2.Equals("InvalidToken", StringComparison.OrdinalIgnoreCase) || attribute2.Equals("Restore", StringComparison.OrdinalIgnoreCase))
											{
												break;
											}
											if (attribute2.Equals("Delete", StringComparison.OrdinalIgnoreCase))
											{
												Guid empty = Guid.Empty;
												if (this.TryParseDeletedChangeItemXml(xmlReader, out empty))
												{
													this.performanceCounter.Increment(ChangeType.SharePointDelete);
													this.deletedEntries.Add(empty);
												}
											}
											else if (!attribute2.Equals("Rename", StringComparison.OrdinalIgnoreCase))
											{
												attribute2.Equals("SystemUpdate", StringComparison.OrdinalIgnoreCase);
											}
										}
										if (!xmlReader.ReadToNextSibling("Id"))
										{
											goto IL_171;
										}
									}
									flag2 = true;
								}
							}
							IL_171:
							if (xmlReader.Name.Equals("rs:data"))
							{
								text2 = xmlReader.GetAttribute("ListItemCollectionPositionNext");
								string attribute3 = xmlReader.GetAttribute("ItemCount");
								int num = 0;
								if (!string.IsNullOrEmpty(attribute3) && int.TryParse(attribute3, out num) && num > 0 && xmlReader.ReadToDescendant("z:row"))
								{
									do
									{
										if (this.TryParseChangeItemXml(xmlReader, out changedItem))
										{
											if (changedItem is FileChange)
											{
												this.performanceCounter.Increment(ChangeType.SharePointFileChange);
												FileChange fileChange = changedItem as FileChange;
												Dictionary<Uri, FolderChange> dictionary = null;
												if (!this.changedItems.TryGetValue(fileChange.ParentLevel, out dictionary))
												{
													dictionary = new Dictionary<Uri, FolderChange>(UriComparer.Default);
													this.changedItems.Add(fileChange.ParentLevel, dictionary);
												}
												FolderChange folderChange = null;
												if (!dictionary.TryGetValue(fileChange.ParentPath, out folderChange))
												{
													folderChange = new FolderChange(new Uri(this.siteUri.GetLeftPart(UriPartial.Authority)), Guid.Empty, string.Empty, fileChange.ParentPath.AbsolutePath, null, ExDateTime.MinValue, ExDateTime.MinValue);
													dictionary.Add(fileChange.ParentPath, folderChange);
												}
												folderChange.FileChanges[fileChange.Id] = fileChange;
											}
											else
											{
												this.performanceCounter.Increment(ChangeType.SharePointFolderChange);
												FolderChange folderChange2 = changedItem as FolderChange;
												Dictionary<Uri, FolderChange> dictionary2 = null;
												if (!this.changedItems.TryGetValue(folderChange2.Level, out dictionary2))
												{
													dictionary2 = new Dictionary<Uri, FolderChange>(UriComparer.Default);
													this.changedItems.Add(folderChange2.Level, dictionary2);
												}
												if (dictionary2.ContainsKey(folderChange2.Path))
												{
													if (dictionary2[folderChange2.Path].HasFileChangesOnly)
													{
														folderChange2.FileChanges.Clear();
														foreach (KeyValuePair<Guid, FileChange> keyValuePair in dictionary2[folderChange2.Path].FileChanges)
														{
															folderChange2.FileChanges.Add(keyValuePair.Key, keyValuePair.Value);
														}
													}
													dictionary2[folderChange2.Path] = folderChange2;
												}
												else
												{
													dictionary2.Add(folderChange2.Path, folderChange2);
												}
											}
										}
									}
									while (xmlReader.ReadToNextSibling("z:row"));
								}
							}
						}
					}
				}
				catch (XmlException ex2)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "OnGetListItemChangesSinceTokenComplete: Content reading failed with XmlException", ex2);
					this.executionAsyncResult.InvokeCallback(ex2);
					return;
				}
				if (!flag2 && !this.UnderFullSync && string.IsNullOrEmpty(value))
				{
					ex = new FormatException(string.Format("Failed: SharePoint response's LastChangeToken string value is null or empty, RawXml response is {0}", this.GetRawXmlResponseString()), new ArgumentNullException("LastChangeToken"));
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "OnGetListItemChangesSinceTokenComplete: Failed with FormatException", ex);
					this.executionAsyncResult.InvokeCallback(ex);
					return;
				}
			}
			if (flag2)
			{
				this.changedItems.Clear();
				this.inputToken = null;
				this.outputToken = null;
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "FullSyncRequired because of InvalidToken or Restore");
				this.InternalBeginGetListItemChangesSinceToken(null);
			}
			else if (this.UnderFullSync)
			{
				if (string.IsNullOrEmpty(text))
				{
					this.outputToken = value;
				}
				if (string.IsNullOrEmpty(text2))
				{
					this.BeginSaveChangesToExchange(new AsyncCallback(this.OnSaveChangesToExchangeComplete), null);
					return;
				}
				if (base.HandleShutDown())
				{
					return;
				}
				text = text2;
				this.InternalBeginGetListItemChangesSinceToken(text);
				return;
			}
			else
			{
				if (base.HandleShutDown())
				{
					return;
				}
				if (flag)
				{
					this.inputToken = value;
					this.InternalBeginGetListItemChangesSinceToken(null);
					return;
				}
				this.outputToken = value;
				this.BeginSaveChangesToExchange(new AsyncCallback(this.OnSaveChangesToExchangeComplete), null);
				return;
			}
		}

		private XmlReader EndGetListItemChangesSinceTokenViaSoap(IAsyncResult asyncResult, out Exception exception)
		{
			exception = null;
			XmlReader result;
			try
			{
				XmlNode xmlNode = this.listsWebSvc.EndGetListItemChangesSinceToken(asyncResult);
				if (xmlNode == null)
				{
					exception = new FormatException("EndGetListItemChangesSinceTokenViaSoap return null listItems", new ArgumentNullException("listItems"));
					result = null;
				}
				else
				{
					this.responseContent = xmlNode;
					result = new XmlNodeReader(xmlNode);
				}
			}
			catch (WebException ex)
			{
				exception = ex;
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "EndGetListItemChangesSinceTokenViaSoap: Failed with WebException", exception);
				result = null;
			}
			catch (SoapException ex2)
			{
				exception = ex2;
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "EndGetListItemChangesSinceTokenViaSoap: Failed with SoapException", exception);
				result = null;
			}
			finally
			{
				this.performanceCounter.Stop(OperationType.SharePointQuery, 1);
			}
			return result;
		}

		private XmlTextReader EndGetListItemChangesSinceTokenViaRest(ICancelableAsyncResult asyncResult, out Exception exception)
		{
			XmlTextReader result;
			try
			{
				exception = null;
				DownloadResult downloadResult = this.httpClient.EndDownload(asyncResult);
				if (!downloadResult.IsSucceeded)
				{
					WebException ex = downloadResult.Exception as WebException;
					if (ex != null)
					{
						exception = new SharePointException((this.httpClient.LastKnownRequestedUri != null) ? this.httpClient.LastKnownRequestedUri.AbsoluteUri : string.Empty, ex, false);
					}
					else
					{
						exception = downloadResult.Exception;
					}
					result = null;
				}
				else if (downloadResult.ResponseStream == null)
				{
					exception = new SharePointException((this.httpClient.LastKnownRequestedUri != null) ? this.httpClient.LastKnownRequestedUri.AbsoluteUri : string.Empty, ServerStrings.ErrorTeamMailboxGetListItemChangesNullResponse);
					result = null;
				}
				else
				{
					downloadResult.ResponseStream.Position = 0L;
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(downloadResult.ResponseStream);
						disposeGuard.Add<XmlTextReader>(xmlTextReader);
						this.responseContent = downloadResult.ResponseStream;
						try
						{
							if (!xmlTextReader.ReadToFollowing("listitems"))
							{
								exception = new FormatException(string.Format("EndGetListItemChangesSinceTokenViaRest return null listItems. The raw XML response: {0}", this.GetRawXmlResponseString()), new ArgumentNullException("listItems"));
								ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "EndGetListItemChangesSinceTokenViaRest: Failed with FormatException", exception);
								return null;
							}
						}
						catch (XmlException ex2)
						{
							exception = new FormatException(string.Format("Bad Xml. Raw XML response: {0}, XmlException message: {1}", this.GetRawXmlResponseString(), ex2.Message), ex2);
							ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "EndGetListItemChangesSinceTokenViaRest: Failed with FormatException", exception);
							return null;
						}
						disposeGuard.Success();
						result = xmlTextReader;
					}
				}
			}
			finally
			{
				this.performanceCounter.Stop(OperationType.SharePointQuery, 1);
				if (this.httpSessionConfig.RequestStream != null)
				{
					this.httpSessionConfig.RequestStream.Close();
					this.httpSessionConfig.RequestStream = null;
				}
			}
			return result;
		}

		protected IAsyncResult BeginSaveChangesToExchange(AsyncCallback executeCallback, object state)
		{
			this.exchangeOperationsAsyncResult = new LazyAsyncResult(null, state, executeCallback);
			if (this.deletedEntries.Count <= 0)
			{
				if (this.changedItems.Count <= 0)
				{
					goto IL_AD;
				}
			}
			try
			{
				this.InternalSaveChangesToExchange();
				goto IL_BA;
			}
			catch (StorageTransientException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "BeginSaveChangesToExchange: Failed with StorageTransientException", ex);
				this.exchangeOperationsAsyncResult.InvokeCallback(ex);
				goto IL_BA;
			}
			catch (StoragePermanentException ex2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "BeginSaveChangesToExchange: Failed with StoragePermanentException", ex2);
				this.exchangeOperationsAsyncResult.InvokeCallback(ex2);
				goto IL_BA;
			}
			catch (ResourceUnhealthyException ex3)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "BeginSaveChangesToExchange: Failed with ResourceUnhealthyException", ex3);
				this.exchangeOperationsAsyncResult.InvokeCallback(ex3);
				goto IL_BA;
			}
			IL_AD:
			this.exchangeOperationsAsyncResult.InvokeCallback(null);
			IL_BA:
			return this.exchangeOperationsAsyncResult;
		}

		protected void OnSaveChangesToExchangeComplete(IAsyncResult asyncResult)
		{
			Exception ex = this.EndSaveChangesToExchange(asyncResult);
			if (ex == null)
			{
				try
				{
					using (Folder folder = Folder.Bind(this.mailboxSession, this.documentLibraryFolderId))
					{
						folder[FolderSchema.SharePointChangeToken] = ((!this.UnderFullSync && this.hasFolderUrlChange) ? string.Empty : this.outputToken);
						folder.Save();
					}
				}
				catch (StorageTransientException ex2)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "OnSaveChangesToExchangeComplete: Failed with StorageTransientException", ex2);
					this.executionAsyncResult.InvokeCallback(ex2);
				}
				catch (StoragePermanentException ex3)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "OnSaveChangesToExchangeComplete: Failed with StoragePermanentException", ex3);
					this.executionAsyncResult.InvokeCallback(ex3);
				}
			}
			this.executionAsyncResult.InvokeCallback(ex);
		}

		private Exception EndSaveChangesToExchange(IAsyncResult asyncResult)
		{
			return ((LazyAsyncResult)asyncResult).Result as Exception;
		}

		private void InternalSaveChangesToExchange()
		{
			AllItemsFolderHelper.CheckAndCreateDefaultFolders(this.mailboxSession);
			foreach (KeyValuePair<int, Dictionary<Uri, FolderChange>> keyValuePair in this.changedItems)
			{
				foreach (KeyValuePair<Uri, FolderChange> keyValuePair2 in keyValuePair.Value)
				{
					keyValuePair2.Value.Reset();
					this.sortedFolderChanges.Add(keyValuePair2.Value);
				}
			}
			this.BatchSaveChangesToExchange();
		}

		private void BatchSaveChangesToExchange()
		{
			TimeSpan zero = TimeSpan.Zero;
			this.resourceMonitor.StartChargingBudget();
			if (this.InternalBatchSaveChangesToExchange(out zero))
			{
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("BatchSaveChangesToExchange: Delay next execution by {0} milliseconds. Current budget is {1}", zero.TotalMilliseconds, this.resourceMonitor.GetBudget().ToString()));
				this.resourceMonitor.ResetBudget();
				if (zero > DocumentLibSynchronizer.MaximumAllowedDelay)
				{
					throw new StorageTransientException(new LocalizedString(string.Format("Delay suggested by ResourceMonitor of {0} milliseconds exceeded maximum allowed delay of {1} milliseconds", zero.TotalMilliseconds, DocumentLibSynchronizer.MaximumAllowedDelay.TotalMilliseconds)));
				}
				this.StartAsyncDelay(zero);
			}
			this.resourceMonitor.ResetBudget();
		}

		private bool InternalBatchSaveChangesToExchange(out TimeSpan delay)
		{
			delay = TimeSpan.Zero;
			this.CheckResourceHealth();
			using (Folder folder = Folder.Bind(this.mailboxSession, this.documentLibraryFolderId))
			{
				while (this.folderChangesCurrentIndex < this.sortedFolderChanges.Count)
				{
					if (base.HandleShutDown())
					{
						return false;
					}
					FolderChange folderChange = this.sortedFolderChanges[this.folderChangesCurrentIndex];
					if (folderChange.FolderId == null)
					{
						LinkedItemProps linkedItemProps;
						if (!folderChange.HasFileChangesOnly)
						{
							linkedItemProps = (this.newlyCreatedFolders.ContainsKey(folderChange.ParentPath) ? null : Utils.FindFolderBySharePointId(this.mailboxSession, folder, folderChange.Id, this.performanceCounter));
							if (linkedItemProps != null)
							{
								folderChange.FolderId = linkedItemProps.EntryId;
								Utils.TriggerTestInducedException(folderChange);
								this.performanceCounter.Start(OperationType.UpdateFolder);
								using (Folder folder2 = Folder.Bind(this.mailboxSession, folderChange.FolderId))
								{
									string valueOrDefault = folder2.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
									if (this.UpdateFolderProperties(folder2, folderChange))
									{
										this.hasFolderUrlChange = true;
										ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("InternalBatchSaveChangesToExchange: folder Url has changed from {0} to {1}", valueOrDefault, folderChange.Path.AbsoluteUri));
									}
								}
								this.performanceCounter.Stop(OperationType.UpdateFolder, 1);
								ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("InternalBatchSaveChangesToExchange: Updated folder RelativePath:{0}, Id:{1}", folderChange.RelativePath, folderChange.Id));
								if (this.IsThrottleDelayNeeded(out delay))
								{
									return true;
								}
								goto IL_31C;
							}
							else
							{
								StoreObjectId parentFolderId = null;
								if (!this.newlyCreatedFolders.TryGetValue(folderChange.ParentPath, out parentFolderId))
								{
									linkedItemProps = Utils.FindFolderBySharePointUri(this.mailboxSession, folder, folderChange.ParentPath, this.performanceCounter);
									if (linkedItemProps == null)
									{
										this.folderChangesCurrentIndex++;
										continue;
									}
									parentFolderId = linkedItemProps.EntryId;
								}
								Utils.TriggerTestInducedException(folderChange);
								this.performanceCounter.Start(OperationType.AddFolder);
								using (Folder folder3 = Folder.Create(this.mailboxSession, parentFolderId, StoreObjectType.ShortcutFolder, folderChange.LeafNode, CreateMode.OpenIfExists))
								{
									if (!(folder3.PropertyBag.GetValueOrDefault<Guid>(FolderSchema.LinkedId, Guid.Empty) == Guid.Empty))
									{
										this.folderChangesCurrentIndex++;
										continue;
									}
									this.UpdateFolderProperties(folder3, folderChange);
									folder3.Load();
									folderChange.FolderId = folder3.Id.ObjectId;
									this.newlyCreatedFolders[folderChange.Path] = folderChange.FolderId;
									this.performanceCounter.Stop(OperationType.AddFolder, 1);
									ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("InternalBatchSaveChangesToExchange: Created folder RelativePath:{0}, SpId:{1}, FolderId:{2}", folderChange.RelativePath, folderChange.Id, folder3.Id.ObjectId.ToString()));
									if (this.IsThrottleDelayNeeded(out delay))
									{
										return true;
									}
									goto IL_31C;
								}
							}
						}
						if (folderChange.FileChanges.Count == 0)
						{
							throw new StoragePermanentException(new LocalizedString("Unexpected empty folder change."));
						}
						linkedItemProps = Utils.FindFolderBySharePointUri(this.mailboxSession, folder, folderChange.Path, this.performanceCounter);
						if (linkedItemProps == null)
						{
							this.folderChangesCurrentIndex++;
							continue;
						}
						folderChange.FolderId = linkedItemProps.EntryId;
					}
					IL_31C:
					Semaphore semaphore = null;
					string empty = string.Empty;
					try
					{
						semaphore = Utils.GetMailboxOperationSemaphore(this.mailboxSession.MailboxGuid, out empty);
						while (folderChange.FileChangesEnumerator.MoveNext())
						{
							try
							{
								semaphore.WaitOne(Constants.MailboxSemaphoreTimeout);
								FileChange fileChange = folderChange.FileChangesEnumerator.Current;
								LinkedItemProps linkedItemProps2 = Utils.FindItemBySharePointId(this.mailboxSession, fileChange.Id, this.performanceCounter);
								if (linkedItemProps2 == null)
								{
									if (base.HandleShutDown())
									{
										return false;
									}
									this.performanceCounter.Start(OperationType.AddFile);
									if (this.TryAddShadowMessage(this.mailboxSession, folderChange.FolderId, fileChange))
									{
										this.performanceCounter.Stop(OperationType.AddFile, 1);
										if (this.IsThrottleDelayNeeded(out delay))
										{
											return true;
										}
									}
								}
								else
								{
									if (base.HandleShutDown())
									{
										return false;
									}
									this.performanceCounter.Start(OperationType.UpdateFile);
									if (!this.TryUpdateShadowMessage(this.mailboxSession, linkedItemProps2.EntryId, fileChange))
									{
										if (this.UnderFullSync)
										{
											this.permanentlyFailedMessageUpdates.Add(linkedItemProps2.EntryId);
										}
									}
									else
									{
										this.performanceCounter.Stop(OperationType.UpdateFile, 1);
										if (this.IsThrottleDelayNeeded(out delay))
										{
											return true;
										}
										if (!UriComparer.IsEqual(fileChange.Path, linkedItemProps2.LinkedUri))
										{
											this.performanceCounter.Start(OperationType.MoveFile);
											if (this.TryMoveShadowMessage(this.mailboxSession, linkedItemProps2, fileChange, folderChange.FolderId))
											{
												this.performanceCounter.Stop(OperationType.MoveFile, 1);
												if (this.IsThrottleDelayNeeded(out delay))
												{
													return true;
												}
											}
										}
									}
								}
							}
							finally
							{
								Utils.ReleaseAndCloseMailboxOperationSemaphore(semaphore, empty, false);
							}
						}
					}
					finally
					{
						if (semaphore != null)
						{
							semaphore.Close();
						}
					}
					this.folderChangesCurrentIndex++;
				}
				if (this.DeleteItemsIfNecessary(folder, out delay))
				{
					return true;
				}
			}
			this.exchangeOperationsAsyncResult.InvokeCallback(null);
			return false;
		}

		private bool UpdateFolderProperties(Folder folder, FolderChange folderChange)
		{
			bool result = Utils.HasFolderUriChanged(folder, folderChange.Path);
			folder.DisplayName = folderChange.LeafNode;
			folder[FolderSchema.LinkedId] = folderChange.Id;
			folder[FolderSchema.LinkedUrl] = folderChange.Path.AbsoluteUri;
			folder[FolderSchema.LinkedSiteUrl] = this.siteUri.AbsoluteUri;
			folder[FolderSchema.LinkedListId] = this.documentLibraryGuid;
			folder[FolderSchema.LinkedSiteAuthorityUrl] = this.siteUri.AbsoluteUri;
			if (this.UnderFullSync)
			{
				folder[FolderSchema.LinkedLastFullSyncTime] = this.lastAttemptedSyncTime;
			}
			folder.Save();
			return result;
		}

		private bool DeleteItemsIfNecessary(Folder rootFolder, out TimeSpan delay)
		{
			delay = TimeSpan.Zero;
			if (!this.sortedDeletedEntriesSet)
			{
				if (this.UnderFullSync)
				{
					this.SetSortedDeletedEntriesUnderFullSync(rootFolder);
				}
				else
				{
					this.SetSortedDeletedEntriesUnderIncrementalSync(rootFolder);
				}
				foreach (KeyValuePair<StoreObjectId, List<StoreObjectId>> keyValuePair in this.sortedDeletedEntries)
				{
					this.deleteChangeList.Add(new DocumentLibSynchronizer.DeleteChange(keyValuePair.Key, keyValuePair.Value));
				}
				this.deleteChangeListEnumerator = this.deleteChangeList.GetEnumerator();
				this.sortedDeletedEntriesSet = true;
			}
			bool flag = false;
			Exception innerException = null;
			while (this.deleteChangeListEnumerator.MoveNext())
			{
				DocumentLibSynchronizer.DeleteChange deleteChange = this.deleteChangeListEnumerator.Current;
				try
				{
					using (Folder folder = Folder.Bind(this.mailboxSession, deleteChange.FolderId))
					{
						string valueOrDefault = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
						ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("DeleteItemsIfNecessary: Delete {0} entries under folder Uri {1}", deleteChange.EntryIds.Count, valueOrDefault));
						this.performanceCounter.Start(OperationType.DeleteItem);
						AggregateOperationResult aggregateOperationResult = folder.DeleteObjects(DeleteItemFlags.HardDelete, deleteChange.EntryIds.ToArray());
						if (aggregateOperationResult.OperationResult == OperationResult.Failed)
						{
							throw new StorageTransientException(new LocalizedString("Failed to delete items/folders"), aggregateOperationResult.GroupOperationResults[aggregateOperationResult.GroupOperationResults.Length - 1].Exception);
						}
						if (aggregateOperationResult.OperationResult == OperationResult.PartiallySucceeded)
						{
							flag = true;
							for (int i = aggregateOperationResult.GroupOperationResults.Length - 1; i >= 0; i--)
							{
								if (aggregateOperationResult.GroupOperationResults[i].Exception != null)
								{
									innerException = aggregateOperationResult.GroupOperationResults[i].Exception;
								}
							}
						}
						this.performanceCounter.Stop(OperationType.DeleteItem, deleteChange.EntryIds.Count);
						if (this.IsThrottleDelayNeeded(out delay))
						{
							return true;
						}
					}
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			if (flag)
			{
				throw new StorageTransientException(new LocalizedString("Failed to delete items/folders"), innerException);
			}
			return false;
		}

		private void SetSortedDeletedEntriesUnderFullSync(Folder rootFolder)
		{
			using (QueryResult queryResult = rootFolder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, new PropertyDefinition[]
			{
				FolderSchema.Id,
				StoreObjectSchema.ParentItemId,
				FolderSchema.LinkedUrl,
				FolderSchema.LinkedLastFullSyncTime
			}))
			{
				List<StoreObjectId> list = new List<StoreObjectId>();
				list.Add(rootFolder.StoreObjectId);
				object[][] rows;
				do
				{
					rows = queryResult.GetRows(10000);
					for (int i = 0; i < rows.Length; i++)
					{
						LinkedItemProps linkedItemProps = DocumentLibSynchronizer.GetLinkedItemProps(rows[i]);
						if (linkedItemProps.LastFullSyncTime == null || linkedItemProps.LastFullSyncTime != this.lastAttemptedSyncTime)
						{
							if (!this.sortedDeletedEntries.ContainsKey(linkedItemProps.ParentId))
							{
								this.sortedDeletedEntries.Add(linkedItemProps.ParentId, new List<StoreObjectId>());
							}
							this.sortedDeletedEntries[linkedItemProps.ParentId].Add(linkedItemProps.EntryId);
							ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SetSortedDeletedEntriesUnderFullSync: Add folder:{0} to pending delete with LinkedLastFullSyncTime:{1}", (linkedItemProps.LinkedUri != null) ? linkedItemProps.LinkedUri.AbsoluteUri : string.Empty, linkedItemProps.LastFullSyncTime));
						}
						else
						{
							list.Add(linkedItemProps.EntryId);
						}
					}
				}
				while (rows.Length != 0);
				foreach (StoreObjectId folderId in list)
				{
					using (Folder folder = Folder.Bind(this.mailboxSession, folderId))
					{
						using (QueryResult queryResult2 = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
						{
							ItemSchema.Id,
							StoreObjectSchema.ParentItemId,
							MessageItemSchema.LinkedUrl,
							MessageItemSchema.LinkedLastFullSyncTime
						}))
						{
							object[][] rows2;
							do
							{
								rows2 = queryResult2.GetRows(10000);
								for (int j = 0; j < rows2.Length; j++)
								{
									LinkedItemProps linkedItemProps2 = DocumentLibSynchronizer.GetLinkedItemProps(rows2[j]);
									if ((linkedItemProps2.LastFullSyncTime == null || linkedItemProps2.LastFullSyncTime != this.lastAttemptedSyncTime) && !this.permanentlyFailedMessageUpdates.Contains(linkedItemProps2.EntryId))
									{
										if (!this.sortedDeletedEntries.ContainsKey(linkedItemProps2.ParentId))
										{
											this.sortedDeletedEntries.Add(linkedItemProps2.ParentId, new List<StoreObjectId>());
										}
										this.sortedDeletedEntries[linkedItemProps2.ParentId].Add(linkedItemProps2.EntryId);
										ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SetSortedDeletedEntriesUnderFullSync: Add message:{0} to pending delete with LinkedLastFullSyncTime:{1}", (linkedItemProps2.LinkedUri != null) ? linkedItemProps2.LinkedUri.AbsoluteUri : string.Empty, linkedItemProps2.LastFullSyncTime));
									}
								}
							}
							while (rows2.Length != 0);
						}
					}
				}
			}
		}

		private void SetSortedDeletedEntriesUnderIncrementalSync(Folder rootFolder)
		{
			foreach (Guid guid in this.deletedEntries)
			{
				LinkedItemProps linkedItemProps = Utils.FindItemBySharePointId(this.mailboxSession, guid, this.performanceCounter);
				linkedItemProps = (linkedItemProps ?? Utils.FindFolderBySharePointId(this.mailboxSession, rootFolder, guid, this.performanceCounter));
				if (linkedItemProps == null)
				{
					ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SetSortedDeletedEntriesUnderIncrementalSync: Skip delete for non-existing Id {0}", guid));
				}
				else
				{
					ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("SetSortedDeletedEntriesUnderIncrementalSync: Add to pending delete for {0}, SpId:{1} ", (linkedItemProps.LinkedUri != null) ? linkedItemProps.LinkedUri.AbsoluteUri : string.Empty, guid));
					List<StoreObjectId> list = null;
					if (!this.sortedDeletedEntries.TryGetValue(linkedItemProps.ParentId, out list))
					{
						list = new List<StoreObjectId>();
						this.sortedDeletedEntries.Add(linkedItemProps.ParentId, list);
					}
					list.Add(linkedItemProps.EntryId);
				}
			}
		}

		private bool TryAddShadowMessage(MailboxSession session, StoreObjectId parentFolderId, FileChange changedItem)
		{
			using (MessageItem messageItem = MessageItem.Create(session, parentFolderId))
			{
				string className = Utils.MessageClassFromFileExtension(changedItem.DocIcon);
				using (Attachment attachment = messageItem.AttachmentCollection.Create(AttachmentType.Reference))
				{
					attachment.FileName = changedItem.LeafNode;
					attachment[AttachmentSchema.DisplayName] = changedItem.LeafNode;
					attachment[AttachmentSchema.AttachLongPathName] = changedItem.Path.AbsoluteUri;
					attachment[AttachmentSchema.AttachMethod] = 2;
					try
					{
						attachment.Save();
					}
					catch (StoragePermanentException exception)
					{
						ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryAddShadowMessage: Skip item {0} with SpId {1} because of StoragePermanentException when saving attachment", changedItem.LeafNode, changedItem.Id), exception);
						return false;
					}
				}
				messageItem.From = new Participant(changedItem.Author, null, null);
				messageItem[ItemSchema.LastModifiedBy] = changedItem.Editor;
				messageItem.ClassName = className;
				messageItem.Subject = changedItem.LeafNode;
				messageItem[ItemSchema.ReceivedTime] = changedItem.LastModified;
				messageItem[ItemSchema.NormalizedSubject] = changedItem.LeafNode;
				messageItem[MessageItemSchema.LinkedDocumentSize] = changedItem.Size;
				messageItem[MessageItemSchema.LinkedObjectVersion] = changedItem.Version;
				messageItem[MessageItemSchema.LinkedId] = changedItem.Id;
				messageItem[MessageItemSchema.LinkedSiteUrl] = this.siteUri.AbsoluteUri;
				messageItem[MessageItemSchema.LinkedUrl] = changedItem.Path.AbsoluteUri;
				messageItem.IsRead = false;
				messageItem.IsDraft = false;
				if (!string.IsNullOrEmpty(changedItem.CheckoutUser))
				{
					messageItem[MessageItemSchema.LinkedDocumentCheckOutTo] = changedItem.CheckoutUser;
				}
				else if (messageItem.PropertyBag.GetValueOrDefault<string>(MessageItemSchema.LinkedDocumentCheckOutTo, null) != null)
				{
					messageItem.DeleteProperties(new PropertyDefinition[]
					{
						MessageItemSchema.LinkedDocumentCheckOutTo
					});
				}
				try
				{
					Utils.TriggerTestInducedException(changedItem);
					if (this.UnderFullSync)
					{
						messageItem[MessageItemSchema.LinkedLastFullSyncTime] = this.lastAttemptedSyncTime;
					}
					Utils.AddUpdateHistoryEntry((CoreItem)messageItem.CoreItem, this.UnderFullSync ? "Add_FullSync" : "Add_IncrSync", "System", ExDateTime.UtcNow);
					messageItem.Save(SaveMode.ResolveConflicts);
				}
				catch (StoragePermanentException exception2)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryAddShadowMessage: Skip item {0} with SpId {1} because of StoragePermanentException when saving message item", changedItem.LeafNode, changedItem.Id), exception2);
					return false;
				}
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("TryAddShadowMessage: Added file RelativePath:{0} SpId:{1}, ParentFolderId:{2}", changedItem.RelativePath, changedItem.Id, parentFolderId.ToString()));
			}
			return true;
		}

		private bool TryUpdateShadowMessage(MailboxSession session, StoreObjectId itemStoreId, FileChange changedItem)
		{
			bool result;
			using (MessageItem messageItem = MessageItem.Bind(session, itemStoreId))
			{
				messageItem.OpenAsReadWrite();
				using (Attachment attachment = messageItem.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.Reference))
				{
					if (attachment != null)
					{
						attachment.FileName = changedItem.LeafNode;
						attachment[AttachmentSchema.DisplayName] = changedItem.LeafNode;
						attachment[AttachmentSchema.AttachLongPathName] = changedItem.Path.AbsoluteUri;
						try
						{
							attachment.Save();
						}
						catch (StoragePermanentException exception)
						{
							ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryUpdateShadowMessage: Skip item {0} with SpId {1} because of StoragePermanentException when saving attachment", changedItem.LeafNode, changedItem.Id), exception);
							return false;
						}
					}
				}
				messageItem.Subject = changedItem.LeafNode;
				string valueOrDefault = messageItem.PropertyBag.GetValueOrDefault<string>(MessageItemSchema.LinkedObjectVersion, string.Empty);
				if (!changedItem.Version.Equals(valueOrDefault, StringComparison.OrdinalIgnoreCase))
				{
					messageItem.MarkAsUnread(true);
				}
				messageItem.From = new Participant(changedItem.Author, null, null);
				messageItem[ItemSchema.LastModifiedBy] = changedItem.Editor;
				messageItem[ItemSchema.ReceivedTime] = changedItem.LastModified;
				messageItem[ItemSchema.NormalizedSubject] = changedItem.LeafNode;
				messageItem[MessageItemSchema.LinkedDocumentSize] = changedItem.Size;
				messageItem[MessageItemSchema.LinkedSiteUrl] = this.siteUri.AbsoluteUri;
				messageItem[MessageItemSchema.LinkedObjectVersion] = changedItem.Version;
				messageItem[MessageItemSchema.LinkedUrl] = changedItem.Path.AbsoluteUri;
				if (!string.IsNullOrEmpty(changedItem.CheckoutUser))
				{
					messageItem[MessageItemSchema.LinkedDocumentCheckOutTo] = changedItem.CheckoutUser;
				}
				else if (messageItem.PropertyBag.GetValueOrDefault<string>(MessageItemSchema.LinkedDocumentCheckOutTo, null) != null)
				{
					messageItem.DeleteProperties(new PropertyDefinition[]
					{
						MessageItemSchema.LinkedDocumentCheckOutTo
					});
				}
				try
				{
					Utils.TriggerTestInducedException(changedItem);
					if (this.UnderFullSync)
					{
						messageItem[MessageItemSchema.LinkedLastFullSyncTime] = this.lastAttemptedSyncTime;
					}
					Utils.AddUpdateHistoryEntry((CoreItem)messageItem.CoreItem, this.UnderFullSync ? "Update_FullSync" : "Update_IncrSync", "System", ExDateTime.UtcNow);
					messageItem.Save(SaveMode.NoConflictResolutionForceSave);
				}
				catch (StoragePermanentException exception2)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryUpdateShadowMessage: Skip item {0} with SpId {1} because of StoragePermanentException when saving message item", changedItem.LeafNode, changedItem.Id), exception2);
					return false;
				}
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("TryUpdateShadowMessage: Updated file RelativePath:{0} Id:{1}", changedItem.RelativePath, changedItem.Id));
				result = true;
			}
			return result;
		}

		private bool TryMoveShadowMessage(MailboxSession session, LinkedItemProps sourceMessageProps, FileChange fileChange, StoreObjectId targetFolderId)
		{
			Uri parentUri = Utils.GetParentUri(sourceMessageProps.LinkedUri);
			if (parentUri == null)
			{
				throw new InvalidOperationException("sourceFolderUri should not be null for " + sourceMessageProps.LinkedUri.AbsoluteUri);
			}
			LinkedItemProps linkedItemProps = null;
			using (Folder folder = Folder.Bind(session, DefaultFolderType.Root))
			{
				linkedItemProps = Utils.FindFolderBySharePointUri(session, folder, parentUri, this.performanceCounter);
				if (linkedItemProps == null)
				{
					return false;
				}
			}
			try
			{
				using (Folder folder2 = Folder.Bind(session, linkedItemProps.EntryId))
				{
					using (Folder folder3 = Folder.Bind(session, targetFolderId))
					{
						List<PropertyDefinition> list = new List<PropertyDefinition>();
						List<object> list2 = new List<object>();
						list.Add(MessageItemSchema.LinkedUrl);
						list2.Add(fileChange.Path.AbsoluteUri);
						if (this.UnderFullSync)
						{
							list.Add(MessageItemSchema.LinkedLastFullSyncTime);
							list2.Add(this.lastAttemptedSyncTime);
						}
						folder2.MoveItems(folder3, new StoreObjectId[]
						{
							sourceMessageProps.EntryId
						}, list.ToArray(), list2.ToArray(), false);
					}
				}
			}
			catch (StoragePermanentException exception)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryMoveMessage: Skip item Id:{0} source:{1} target:{2} because of StoragePermanentException", fileChange.Id, parentUri.AbsoluteUri, fileChange.Path), exception);
				return false;
			}
			catch (ArgumentException exception2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryMoveMessage: Skip item Id:{0} source:{1} target:{2} because of ArgumentException", fileChange.Id, parentUri.AbsoluteUri, fileChange.Path), exception2);
				return false;
			}
			ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("TryMoveShadowMessage: Moved item source:{0}, target:{1}, Id:{2}", parentUri.AbsoluteUri, fileChange.Path, fileChange.Id));
			return true;
		}

		private bool TryParseDeletedChangeItemXml(XmlReader listItemsReader, out Guid deletedItemSpId)
		{
			string input = null;
			deletedItemSpId = Guid.Empty;
			string attribute = listItemsReader.GetAttribute("UniqueId");
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out input) || !Guid.TryParse(input, out deletedItemSpId))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "TryParseDeletedChangeItemXml: Failed to parse deleted item", new FormatException("TryParseDeletedChangeItemXml: Failed to parse deleted item", new ArgumentNullException("DeletedUniqueIdAttributeName")));
				return false;
			}
			ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("TryParseDeletedChangeItemXml: Item {0} is a deleted item", deletedItemSpId));
			return true;
		}

		private bool TryParseChangeItemXml(XmlReader listItemsReader, out ChangedItem resultItem)
		{
			resultItem = null;
			string text = null;
			string attribute = listItemsReader.GetAttribute("ows_UniqueId");
			Guid id;
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out text) || !Guid.TryParse(text, out id))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's spId. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's spId", new ArgumentNullException("UniqueIdAttributeName")));
				return false;
			}
			string text2 = string.Empty;
			attribute = listItemsReader.GetAttribute("ows_owshiddenversion");
			if (!string.IsNullOrEmpty(attribute))
			{
				if (!DocumentLibSynchronizer.TryParseValue(attribute, out text))
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's version. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's version", new ArgumentNullException("ObjectVersion")));
					return false;
				}
				text2 = text;
			}
			int num = 0;
			attribute = listItemsReader.GetAttribute("ows_FSObjType");
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out text) || !int.TryParse(text, out num) || num > 1 || num < 0)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's type. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's type", new ArgumentNullException("ObjectType")));
				return false;
			}
			attribute = listItemsReader.GetAttribute("ows_Last_x0020_Modified");
			ExDateTime lastModified;
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out text) || !ExDateTime.TryParse(text, out lastModified))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's lastModified. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's lastModified", new ArgumentNullException("LastModifiedAttributeName")));
				return false;
			}
			attribute = listItemsReader.GetAttribute("ows_Created_x0020_Date");
			ExDateTime whenCreated;
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out text) || !ExDateTime.TryParse(text, out whenCreated))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's whenCreated. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's whenCreated", new ArgumentNullException("WhenCreatedAttributeName")));
				return false;
			}
			string docIcon = string.Empty;
			attribute = listItemsReader.GetAttribute("ows_DocIcon");
			if (!string.IsNullOrEmpty(attribute))
			{
				if (!DocumentLibSynchronizer.TryParseValue(attribute, out text))
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's docIcon. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's docIcon", new ArgumentNullException("DocIconAttributeName")));
					return false;
				}
				docIcon = text;
			}
			string author = null;
			attribute = listItemsReader.GetAttribute("ows_Author");
			if (!string.IsNullOrEmpty(attribute))
			{
				if (!DocumentLibSynchronizer.TryParseValue(attribute, out text))
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's Author. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's Author", new ArgumentNullException("AuthorAttributeName")));
					return false;
				}
				author = text;
			}
			string editor = null;
			attribute = listItemsReader.GetAttribute("ows_Editor");
			if (!string.IsNullOrEmpty(attribute))
			{
				if (!DocumentLibSynchronizer.TryParseValue(attribute, out text))
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's Editor. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's Editor", new ArgumentNullException("EditorAttributeName")));
					return false;
				}
				editor = text;
			}
			int size = 0;
			attribute = listItemsReader.GetAttribute("ows_File_x0020_Size");
			if (num == 0 && !string.IsNullOrEmpty(attribute) && (!DocumentLibSynchronizer.TryParseValue(attribute, out text) || !int.TryParse(text, out size)))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's size. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's size", new ArgumentNullException("SizeAttributeName")));
				return false;
			}
			string checkoutUser = null;
			attribute = listItemsReader.GetAttribute("ows_CheckoutUser");
			if (!string.IsNullOrEmpty(attribute))
			{
				if (!DocumentLibSynchronizer.TryParseValue(attribute, out text))
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's CheckoutUser. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's CheckoutUser", new ArgumentNullException("CheckoutUserAttributeName")));
					return false;
				}
				checkoutUser = text;
			}
			string leafNode = null;
			attribute = listItemsReader.GetAttribute("ows_FileLeafRef");
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out leafNode))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's LeafRef. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's LeafRef", new ArgumentNullException("LeafRefAttributeName")));
				return false;
			}
			string relativePath = null;
			attribute = listItemsReader.GetAttribute("ows_FileRef");
			if (string.IsNullOrEmpty(attribute) || !DocumentLibSynchronizer.TryParseValue(attribute, out relativePath))
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Failed to parse changed item's Ref. RawXML {0}", attribute), new FormatException("TryParseChangeItemXml: Failed to parse changed item's Ref", new ArgumentNullException("RefAttributeName")));
				return false;
			}
			if (num == 0)
			{
				resultItem = new FileChange(new Uri(this.siteUri.GetLeftPart(UriPartial.Authority)), id, text2, docIcon, author, editor, checkoutUser, relativePath, leafNode, whenCreated, lastModified, size);
			}
			else if (num == 1)
			{
				resultItem = new FolderChange(new Uri(this.siteUri.GetLeftPart(UriPartial.Authority)), id, text2, relativePath, leafNode, whenCreated, lastModified);
			}
			ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, string.Format("TryParseChangeItemXml: Success in parsing changed item id:{0}, version {1}, type {2}, LastModified:{3}, WhenChanged:{4}, LeafNode:{5}, RelativePath:{6}, Path:{7}", new object[]
			{
				resultItem.Id,
				text2,
				num,
				resultItem.LastModified,
				resultItem.WhenCreated,
				resultItem.LeafNode,
				resultItem.RelativePath,
				resultItem.Path.AbsoluteUri.ToString()
			}));
			return true;
		}

		private string GetRawXmlResponseString()
		{
			XmlNode xmlNode = null;
			string result = string.Empty;
			if (this.isOAuthCredential)
			{
				Stream stream = this.responseContent as Stream;
				stream.Position = 0L;
				byte[] array = new byte[1024];
				StringBuilder stringBuilder = new StringBuilder(1024);
				for (;;)
				{
					int num = 0;
					try
					{
						num = stream.Read(array, 0, 1024);
					}
					catch (IOException)
					{
						break;
					}
					if (num != 0 && stringBuilder.Length <= 15360)
					{
						try
						{
							stringBuilder.Append(Encoding.UTF8.GetString(array, 0, num));
							continue;
						}
						catch (ArgumentException)
						{
						}
						break;
					}
					break;
				}
				result = stringBuilder.ToString();
			}
			else
			{
				xmlNode = (this.responseContent as XmlNode);
			}
			if (xmlNode == null)
			{
				return result;
			}
			return xmlNode.OuterXml;
		}

		private void IntializeWebService()
		{
			if (this.listsWebSvc == null)
			{
				this.listsWebSvc = new Lists();
				this.listsWebSvc.Url = string.Format("{0}/_vti_bin/lists.asmx", this.siteUri.AbsoluteUri);
				this.listsWebSvc.Credentials = this.credential;
				if (this.enableHttpDebugProxy)
				{
					this.listsWebSvc.Proxy = new WebProxy("127.0.0.1", 8888);
				}
			}
		}

		private void SetRestRequestStream(string pageInfo)
		{
			if (this.httpSessionConfig.RequestStream != null)
			{
				this.httpSessionConfig.RequestStream.Close();
				this.httpSessionConfig.RequestStream = null;
			}
			StringBuilder stringBuilder = new StringBuilder("{'query':{'__metadata':{'type':'SP.ChangeLogItemQuery'},'RowLimit':'");
			stringBuilder.Append(((TeamMailboxSyncConfiguration)this.Job.Config).SharePointQueryPageSize.ToString());
			stringBuilder.Append("', 'Query':'',");
			stringBuilder.Append(DocumentLibSynchronizer.SelectedViewFieldsStringForRest);
			stringBuilder.Append("'QueryOptions': '");
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.WriteStartElement("QueryOptions");
					xmlTextWriter.WriteElementString("DateInUtc", "TRUE");
					xmlTextWriter.WriteStartElement("Paging");
					if (!string.IsNullOrEmpty(pageInfo))
					{
						xmlTextWriter.WriteAttributeString("ListItemCollectionPositionNext", pageInfo);
					}
					xmlTextWriter.WriteString("TRUE");
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteStartElement("ViewAttributes");
					xmlTextWriter.WriteAttributeString("Scope", "RecursiveAll");
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					stringBuilder.Append("', 'ChangeToken':'");
					stringBuilder.Append(this.inputToken);
					stringBuilder.Append("'} }");
					byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
					this.httpSessionConfig.RequestStream = new MemoryStream(bytes, 0, bytes.Length);
				}
			}
		}

		private void CheckResourceHealth()
		{
			this.resourceMonitor.CheckResourceHealth();
		}

		private bool IsThrottleDelayNeeded(out TimeSpan delay)
		{
			delay = TimeSpan.Zero;
			if (this.performanceCounter.CurrentIoOperations >= this.workLoadSize)
			{
				this.performanceCounter.CurrentIoOperations = 0;
				DelayInfo delay2 = this.resourceMonitor.GetDelay();
				if (delay2.Delay > TimeSpan.Zero)
				{
					delay = delay2.Delay;
					return true;
				}
			}
			return false;
		}

		private void StartAsyncDelay(TimeSpan delay)
		{
			if (this.delayTimer == null)
			{
				this.delayTimer = new Timer(new TimerCallback(this.DelayCallback), null, -1, -1);
			}
			this.performanceCounter.Start(OperationType.Throttle);
			this.delayTimer.Change(delay, TimeSpan.FromMilliseconds(-1.0));
		}

		private void DelayCallback(object state)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					this.InternalDelayCallback();
				});
			}
			catch (GrayException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "DelayCallback: Failed with unexpected exception", ex);
				this.executionAsyncResult.InvokeCallback(ex);
			}
		}

		private void InternalDelayCallback()
		{
			try
			{
				this.performanceCounter.Stop(OperationType.Throttle, 1);
				if (!base.HandleShutDown())
				{
					this.BatchSaveChangesToExchange();
				}
			}
			catch (StorageTransientException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "InternalDelayCallback: Failed with StorageTransientException", ex);
				this.exchangeOperationsAsyncResult.InvokeCallback(ex);
			}
			catch (StoragePermanentException ex2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "InternalDelayCallback: Failed with StoragePermanentException", ex2);
				this.exchangeOperationsAsyncResult.InvokeCallback(ex2);
			}
			catch (ResourceUnhealthyException ex3)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "InternalDelayCallback: Failed with ResourceUnhealthyException", ex3);
				this.exchangeOperationsAsyncResult.InvokeCallback(ex3);
			}
		}

		private const string DefaultQueryOptionString = "<queryOptions><DateInUtc>TRUE</DateInUtc></queryOptions>";

		private const string DeletedUniqueIdAttributeName = "UniqueId";

		private const string UniqueIdAttributeName = "ows_UniqueId";

		private const string ObjectVersion = "ows_owshiddenversion";

		private const string ObjectType = "ows_FSObjType";

		private const string LastModifiedAttributeName = "ows_Last_x0020_Modified";

		private const string WhenCreatedAttributeName = "ows_Created_x0020_Date";

		private const string LeafRefAttributeName = "ows_FileLeafRef";

		private const string RefAttributeName = "ows_FileRef";

		private const string DocIconAttributeName = "ows_DocIcon";

		private const string AuthorAttributeName = "ows_Author";

		private const string EditorAttributeName = "ows_Editor";

		private const string SizeAttributeName = "ows_File_x0020_Size";

		private const string CheckoutUserAttributeName = "ows_CheckoutUser";

		private const int DefaultWorkLoadSize = 20;

		private static readonly XmlNode DefaultQueryOption = DocumentLibSynchronizer.GetXmlNode("<queryOptions><DateInUtc>TRUE</DateInUtc></queryOptions>");

		private static readonly TimeSpan MaximumAllowedDelay = TimeSpan.FromSeconds(30.0);

		private static readonly string SelectedViewFieldsString = string.Format("<ViewFields>\r\n                <FieldRef Name='{0}' />\r\n                <FieldRef Name='{1}' />\r\n                <FieldRef Name='{2}' />\r\n                <FieldRef Name='{3}' />\r\n                <FieldRef Name='{4}' />\r\n                <FieldRef Name='{5}' />\r\n                <FieldRef Name='{6}' />\r\n                <FieldRef Name='{7}' />\r\n                <FieldRef Name='{8}' />\r\n                <FieldRef Name='{9}' />\r\n                <FieldRef Name='{10}' />\r\n                <FieldRef Name='{11}' />\r\n                <FieldRef Name='{12}' />\r\n            </ViewFields>", new object[]
		{
			"UniqueId",
			"ows_UniqueId".Substring(4),
			"ows_owshiddenversion".Substring(4),
			"ows_FSObjType".Substring(4),
			"ows_Last_x0020_Modified".Substring(4),
			"ows_Created_x0020_Date".Substring(4),
			"ows_FileLeafRef".Substring(4),
			"ows_FileRef".Substring(4),
			"ows_DocIcon".Substring(4),
			"ows_Author".Substring(4),
			"ows_Editor".Substring(4),
			"ows_File_x0020_Size".Substring(4),
			"ows_CheckoutUser".Substring(4)
		});

		private static readonly string SelectedViewFieldsStringForRest = string.Format("'ViewFields':'{0}',", DocumentLibSynchronizer.SelectedViewFieldsString.Replace('\'', '"'));

		private static readonly XmlNode SelectedViewFields = DocumentLibSynchronizer.GetXmlNode(DocumentLibSynchronizer.SelectedViewFieldsString);

		private readonly StoreObjectId documentLibraryFolderId;

		private readonly Guid documentLibraryGuid;

		private readonly PerformanceCounter performanceCounter = new PerformanceCounter();

		private readonly List<FolderChange> sortedFolderChanges = new List<FolderChange>();

		private readonly HashSet<StoreObjectId> permanentlyFailedMessageUpdates = new HashSet<StoreObjectId>();

		private int folderChangesCurrentIndex;

		private List<DocumentLibSynchronizer.DeleteChange> deleteChangeList = new List<DocumentLibSynchronizer.DeleteChange>();

		private IEnumerator<DocumentLibSynchronizer.DeleteChange> deleteChangeListEnumerator;

		private Dictionary<StoreObjectId, List<StoreObjectId>> sortedDeletedEntries = new Dictionary<StoreObjectId, List<StoreObjectId>>();

		private bool sortedDeletedEntriesSet;

		private bool hasFolderUrlChange;

		private Timer delayTimer;

		private Dictionary<Uri, StoreObjectId> newlyCreatedFolders = new Dictionary<Uri, StoreObjectId>(UriComparer.Default);

		private Lists listsWebSvc;

		private object responseContent;

		private string inputToken;

		protected int workLoadSize;

		protected List<Guid> deletedEntries = new List<Guid>();

		protected SortedDictionary<int, Dictionary<Uri, FolderChange>> changedItems = new SortedDictionary<int, Dictionary<Uri, FolderChange>>();

		protected string outputToken;

		private sealed class DeleteChange
		{
			public StoreObjectId FolderId { get; private set; }

			public List<StoreObjectId> EntryIds { get; private set; }

			public DeleteChange(StoreObjectId folderId, List<StoreObjectId> entryIds)
			{
				this.FolderId = folderId;
				this.EntryIds = entryIds;
			}
		}
	}
}
