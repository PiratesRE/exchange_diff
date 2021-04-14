using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EwsAuditClient
	{
		public EwsAuditClient(IEwsConnectionManager connectionManager, TimeSpan timeout, Trace tracer)
		{
			this.Tracer = tracer;
			this.CallTimeout = timeout;
			this.connectionManager = connectionManager;
			this.InitializeExchangeBinding();
			this.DiscoverUrl();
		}

		public Trace Tracer { get; private set; }

		public TimeSpan CallTimeout { get; private set; }

		public void CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType parentFolder, DistinguishedFolderIdNameType targetFolder, out FolderIdType targetFolderId)
		{
			targetFolderId = null;
			GetFolderType getFolder = new GetFolderType
			{
				FolderShape = new FolderResponseShapeType
				{
					BaseShape = DefaultShapeNamesType.Default
				},
				FolderIds = new BaseFolderIdType[]
				{
					new DistinguishedFolderIdType
					{
						Id = targetFolder
					}
				}
			};
			FolderIdType localTargetFolderId = null;
			bool retry = false;
			do
			{
				this.CallEwsWithRetries((LID)47292U, () => this.binding.GetFolder(getFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					FolderInfoResponseMessageType folderInfoResponseMessageType = responseMessage as FolderInfoResponseMessageType;
					if (folderInfoResponseMessageType != null && folderInfoResponseMessageType.ResponseClass == ResponseClassType.Success && folderInfoResponseMessageType.Folders != null && folderInfoResponseMessageType.Folders.Length == 1)
					{
						localTargetFolderId = folderInfoResponseMessageType.Folders[0].FolderId;
						if (this.IsTraceEnabled(TraceType.DebugTrace))
						{
							this.Tracer.TraceDebug<string, string>(0L, "Found the folder {0} with the id {1}", targetFolder.ToString(), localTargetFolderId.Id);
						}
						retry = false;
						return true;
					}
					return false;
				}, delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					if (responseMessage != null && responseMessage.ResponseClass == ResponseClassType.Error && responseMessage.ResponseCode == ResponseCodeType.ErrorFolderNotFound && !retry)
					{
						DiagnosticContext.TraceLocation((LID)37196U);
						if (this.IsTraceEnabled(TraceType.DebugTrace))
						{
							this.Tracer.TraceDebug(0L, "Folder not found, an attempt will be made to create it", new object[]
							{
								targetFolder.ToString(),
								responseMessage.ResponseClass,
								responseMessage.ResponseCode,
								responseMessage.MessageText
							});
						}
						return true;
					}
					return false;
				});
				if (localTargetFolderId == null && !retry)
				{
					DiagnosticContext.TraceLocation((LID)61628U);
					if (!this.InternalCreateFolder(new CreateFolderType
					{
						ParentFolderId = new TargetFolderIdType
						{
							Item = new DistinguishedFolderIdType
							{
								Id = parentFolder,
								Mailbox = this.Mailbox
							}
						},
						Folders = new BaseFolderType[]
						{
							new FolderType
							{
								DistinguishedFolderId = targetFolder,
								DistinguishedFolderIdSpecified = true
							}
						}
					}, delegate(ResponseMessageType responseMessage, int messageIndex)
					{
						if (responseMessage != null && responseMessage.ResponseClass == ResponseClassType.Error && responseMessage.ResponseCode == ResponseCodeType.ErrorInvalidOperation)
						{
							DiagnosticContext.TraceLocation((LID)53580U);
							retry = true;
							return true;
						}
						return false;
					}, out localTargetFolderId))
					{
						localTargetFolderId = null;
					}
				}
			}
			while (retry);
			ExAssert.RetailAssert(localTargetFolderId != null, "Well known folder ID must be valid at this point");
			targetFolderId = localTargetFolderId;
		}

		public void CreateItem(CreateItemType item)
		{
			this.CallEwsWithRetries((LID)63676U, () => this.binding.CreateItem(item), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage != null && responseMessage.ResponseClass == ResponseClassType.Success)
				{
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug(0L, "Successfully created EWS.CreateItemType");
					}
					return true;
				}
				return false;
			}, null);
		}

		public bool CreateFolder(string folderName, FolderIdType parentFolderId, out FolderIdType folderId)
		{
			DiagnosticContext.TraceLocation((LID)39100U);
			folderId = null;
			CreateFolderType cft = new CreateFolderType
			{
				ParentFolderId = new TargetFolderIdType
				{
					Item = parentFolderId
				},
				Folders = new BaseFolderType[]
				{
					new FolderType
					{
						DisplayName = folderName
					}
				}
			};
			return this.InternalCreateFolder(cft, null, out folderId);
		}

		public bool FindFolder(string folderName, BaseFolderIdType parentFolderId, out FolderIdType folderId)
		{
			folderId = null;
			FindFolderType findFolderType = new FindFolderType
			{
				FolderShape = new FolderResponseShapeType
				{
					BaseShape = DefaultShapeNamesType.IdOnly
				},
				Item = new IndexedPageViewType
				{
					Offset = 0,
					BasePoint = IndexBasePointType.Beginning
				},
				ParentFolderIds = new BaseFolderIdType[]
				{
					parentFolderId
				},
				Traversal = FolderQueryTraversalType.Shallow,
				Restriction = new RestrictionType
				{
					Item = new IsEqualToType
					{
						Item = new PathToUnindexedFieldType
						{
							FieldURI = UnindexedFieldURIType.folderDisplayName
						},
						FieldURIOrConstant = new FieldURIOrConstantType
						{
							Item = new ConstantValueType
							{
								Value = folderName
							}
						}
					}
				}
			};
			FolderIdType localTargetFolderId = null;
			this.CallEwsWithRetries((LID)55484U, () => this.binding.FindFolder(findFolderType), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				FindFolderResponseMessageType findFolderResponseMessageType = responseMessage as FindFolderResponseMessageType;
				if (findFolderResponseMessageType != null && findFolderResponseMessageType.ResponseClass == ResponseClassType.Success && findFolderResponseMessageType.RootFolder != null && findFolderResponseMessageType.RootFolder.Folders != null && findFolderResponseMessageType.RootFolder.Folders.Length == 1)
				{
					localTargetFolderId = findFolderResponseMessageType.RootFolder.Folders[0].FolderId;
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug<string, string>(0L, "Found the folder '{0}' with the id {1}", folderName, localTargetFolderId.Id);
					}
					return true;
				}
				if (findFolderResponseMessageType != null && findFolderResponseMessageType.ResponseClass == ResponseClassType.Error && findFolderResponseMessageType.ResponseCode == ResponseCodeType.ErrorFolderNotFound)
				{
					localTargetFolderId = null;
					return true;
				}
				return false;
			}, null);
			folderId = localTargetFolderId;
			return localTargetFolderId != null;
		}

		public BaseFolderType[] GetSubFolders(BaseFolderIdType parentFolderId, FolderResponseShapeType folderShape)
		{
			if (folderShape == null)
			{
				folderShape = EwsAuditClient.DefaultFolderShape;
			}
			IndexedPageViewType pageView = new IndexedPageViewType
			{
				Offset = 0,
				BasePoint = IndexBasePointType.Beginning,
				MaxEntriesReturnedSpecified = true,
				MaxEntriesReturned = 256
			};
			FindFolderType findFolderType = new FindFolderType
			{
				FolderShape = folderShape,
				Item = pageView,
				ParentFolderIds = new BaseFolderIdType[]
				{
					parentFolderId
				},
				Traversal = FolderQueryTraversalType.Shallow
			};
			HashSet<string> hashSet = null;
			List<BaseFolderType> list = null;
			bool haveMore;
			do
			{
				findFolderType.Item = pageView;
				BaseFolderType[] localFoldersList = null;
				haveMore = false;
				this.CallEwsWithRetries((LID)43196U, () => this.binding.FindFolder(findFolderType), delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					FindFolderResponseMessageType findFolderResponseMessageType = responseMessage as FindFolderResponseMessageType;
					if (findFolderResponseMessageType != null && findFolderResponseMessageType.ResponseClass == ResponseClassType.Success && findFolderResponseMessageType.RootFolder != null)
					{
						localFoldersList = findFolderResponseMessageType.RootFolder.Folders;
						haveMore = !findFolderResponseMessageType.RootFolder.IncludesLastItemInRange;
						if (haveMore)
						{
							pageView.Offset = findFolderResponseMessageType.RootFolder.IndexedPagingOffset;
						}
						return true;
					}
					return false;
				}, null);
				if (localFoldersList != null && localFoldersList.Length > 0)
				{
					foreach (BaseFolderType baseFolderType in localFoldersList)
					{
						if (baseFolderType != null && baseFolderType.FolderId != null && !string.IsNullOrEmpty(baseFolderType.FolderId.Id))
						{
							if (hashSet == null)
							{
								hashSet = new HashSet<string>();
							}
							else if (hashSet.Contains(baseFolderType.FolderId.Id))
							{
								goto IL_18B;
							}
							if (list == null)
							{
								list = new List<BaseFolderType>(localFoldersList.Length);
							}
							hashSet.Add(baseFolderType.FolderId.Id);
							list.Add(baseFolderType);
						}
						IL_18B:;
					}
				}
			}
			while (haveMore);
			if (list != null && list.Count > 0)
			{
				return list.ToArray();
			}
			return Array<BaseFolderType>.Empty;
		}

		public ItemType[] FindItems(BaseFolderIdType parentFolderId, ItemResponseShapeType itemShape, FieldOrderType[] sortOrder, RestrictionType restriction)
		{
			FindItemType findItemType = new FindItemType
			{
				ItemShape = EwsAuditClient.IdOnlyItemShape,
				ParentFolderIds = new BaseFolderIdType[]
				{
					parentFolderId
				},
				Traversal = ItemQueryTraversalType.Shallow,
				Restriction = restriction,
				SortOrder = sortOrder
			};
			return this.InternalFindItems(findItemType, itemShape);
		}

		public ItemType[] FindItemsWithFAST(BaseFolderIdType parentFolderId, ItemResponseShapeType itemShape, FieldOrderType[] sortOrder, QueryStringType queryString)
		{
			FindItemType findItemType = new FindItemType
			{
				ItemShape = EwsAuditClient.IdOnlyItemShape,
				ParentFolderIds = new BaseFolderIdType[]
				{
					parentFolderId
				},
				Traversal = ItemQueryTraversalType.Shallow,
				QueryString = queryString,
				SortOrder = sortOrder
			};
			return this.InternalFindItems(findItemType, itemShape);
		}

		public IList<ItemInformation> ExportItems(IEnumerable<string> itemIds)
		{
			ExportItemsType itemsType = new ExportItemsType();
			itemsType.ItemIds = (from id in itemIds
			select new ItemIdType
			{
				Id = id
			}).ToArray<ItemIdType>();
			List<ItemInformation> items = new List<ItemInformation>();
			this.CallEwsWithRetries((LID)0U, () => this.binding.ExportItems(itemsType), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				ExportItemsResponseMessageType exportItemsResponseMessageType = responseMessage as ExportItemsResponseMessageType;
				if (exportItemsResponseMessageType != null && exportItemsResponseMessageType.ResponseClass == ResponseClassType.Success && exportItemsResponseMessageType.Data != null)
				{
					items.Add(new ItemInformation
					{
						Id = exportItemsResponseMessageType.ItemId.Id,
						Data = exportItemsResponseMessageType.Data,
						Error = null
					});
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug(0L, "Items exported successfully");
					}
				}
				return false;
			}, null);
			return items;
		}

		public bool UploadItems(FolderIdType parentFolderId, IEnumerable<ItemInformation> items, bool alwaysCreateNew)
		{
			UploadItemsType itemsType = new UploadItemsType();
			CreateActionType createActionType = alwaysCreateNew ? CreateActionType.CreateNew : CreateActionType.UpdateOrCreate;
			itemsType.Items = (from item in items
			select new UploadItemType
			{
				CreateAction = createActionType,
				ItemId = new ItemIdType
				{
					Id = item.Id
				},
				Data = item.Data,
				ParentFolderId = parentFolderId
			}).ToArray<UploadItemType>();
			int successCount = 0;
			this.CallEwsWithRetries((LID)0U, () => this.binding.UploadItems(itemsType), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				UploadItemsResponseMessageType uploadItemsResponseMessageType = responseMessage as UploadItemsResponseMessageType;
				if (uploadItemsResponseMessageType != null && uploadItemsResponseMessageType.ResponseClass == ResponseClassType.Success)
				{
					successCount++;
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug(0L, "Items uploaded successfully");
					}
				}
				return false;
			}, null);
			return successCount == itemsType.Items.Length;
		}

		public void RefreshUrl(bool reloadBinding)
		{
			if (reloadBinding)
			{
				DiagnosticContext.TraceLocation((LID)59580U);
				this.connectionManager.ReloadPrincipal();
				this.InitializeExchangeBinding();
			}
			if (this.lastUrlDiscoverTime.Add(this.ExpiryInterval) < ExDateTime.UtcNow)
			{
				DiagnosticContext.TraceLocation((LID)35004U);
				if (this.IsTraceEnabled(TraceType.DebugTrace))
				{
					this.Tracer.TraceDebug<ExDateTime, string>(0L, "Need to refresh service binding, last discover time : {0}, current url: {1}", this.lastUrlDiscoverTime, this.binding.Url.ToString());
				}
				this.DiscoverUrl();
			}
		}

		private EmailAddressType Mailbox
		{
			get
			{
				return new EmailAddressType
				{
					EmailAddress = this.connectionManager.GetSmtpAddress()
				};
			}
		}

		private bool IsTraceEnabled(TraceType traceType)
		{
			return this.Tracer != null && this.Tracer.IsTraceEnabled(traceType);
		}

		private void InitializeExchangeBinding()
		{
			this.binding = this.connectionManager.CreateBinding(new RemoteCertificateValidationCallback(this.CertificateErrorHandler));
			this.binding.Timeout = (int)this.CallTimeout.TotalMilliseconds;
			this.lastUrlDiscoverTime = ExDateTime.MinValue;
		}

		private void DiscoverUrl()
		{
			string text = null;
			if (this.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.Tracer.TraceDebug<string>(0L, "Will try to discover the URL for EWS with the Backendlocator for  mailbox {0}", this.connectionManager.GetPrincipalInfoForTracing());
			}
			Exception innerException = null;
			try
			{
				Uri backEndWebServicesUrl = this.connectionManager.GetBackEndWebServicesUrl();
				if (backEndWebServicesUrl != null)
				{
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug<string, string>(0L, "Found Uri from the back end locator.{0}, {1}", backEndWebServicesUrl.ToString(), this.connectionManager.GetPrincipalInfoForTracing());
					}
					text = backEndWebServicesUrl.ToString();
				}
				else if (this.IsTraceEnabled(TraceType.ErrorTrace))
				{
					this.Tracer.TraceError<string>(0L, "Unable to discover internal URL for EWS for mailbox {0}. BackEndLocator call returned null", this.connectionManager.GetPrincipalInfoForTracing());
				}
			}
			catch (LocalizedException ex)
			{
				DiagnosticContext.TraceLocation((LID)51388U);
				innerException = ex;
				if (this.IsTraceEnabled(TraceType.ErrorTrace))
				{
					this.Tracer.TraceError<string, LocalizedException>(0L, "Unable to discover internal URL for EWS for mailbox {0} due to exception {1}", this.connectionManager.GetPrincipalInfoForTracing(), ex);
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				DiagnosticContext.TraceLocation((LID)45244U);
				throw new FailedToFindEwsEndpointException(this.connectionManager.GetPrincipalInfoForTracing(), innerException);
			}
			this.binding.Url = text;
			this.lastUrlDiscoverTime = ExDateTime.UtcNow;
			if (this.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.Tracer.TraceDebug(0L, "Refreshed service binding (url and adminauditlogs folder), new url: " + text);
			}
		}

		private bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			SslPolicyInfo.Instance.LastValidationTime = DateTime.UtcNow;
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				return true;
			}
			if (SslConfiguration.AllowInternalUntrustedCerts)
			{
				if (this.IsTraceEnabled(TraceType.DebugTrace))
				{
					this.Tracer.TraceDebug(0L, "Accepting SSL certificate because registry config AllowInternalUntrustedCerts tells to ignore errors");
				}
				return true;
			}
			if (this.IsTraceEnabled(TraceType.ErrorTrace))
			{
				this.Tracer.TraceError<SslPolicyErrors>(0L, "Failed because SSL certificate contains the following errors: {0}", sslPolicyErrors);
			}
			SslPolicyInfo.Instance.Add(new SslError(sslPolicyErrors, SslConfiguration.AllowInternalUntrustedCerts, SslConfiguration.AllowExternalUntrustedCerts));
			return false;
		}

		private bool InternalCreateFolder(CreateFolderType cft, Func<ResponseMessageType, int, bool> responseErrorProcessor, out FolderIdType targetFolderId)
		{
			targetFolderId = null;
			FolderIdType localTargetFolderId = null;
			this.CallEwsWithRetries((LID)37052U, () => this.binding.CreateFolder(cft), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				FolderInfoResponseMessageType folderInfoResponseMessageType = responseMessage as FolderInfoResponseMessageType;
				if (folderInfoResponseMessageType != null && folderInfoResponseMessageType.ResponseClass == ResponseClassType.Success && folderInfoResponseMessageType.Folders != null && folderInfoResponseMessageType.Folders.Length == 1)
				{
					localTargetFolderId = folderInfoResponseMessageType.Folders[0].FolderId;
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug(0L, "Folder created successfully");
					}
					return true;
				}
				return false;
			}, responseErrorProcessor);
			targetFolderId = localTargetFolderId;
			return localTargetFolderId != null;
		}

		private IEnumerable<BaseItemIdType> InternalFindItemIds(FindItemType findItemType)
		{
			IndexedPageViewType pageView = new IndexedPageViewType
			{
				Offset = 0,
				BasePoint = IndexBasePointType.Beginning,
				MaxEntriesReturnedSpecified = true,
				MaxEntriesReturned = 512
			};
			findItemType.ItemShape = EwsAuditClient.IdOnlyItemShape;
			findItemType.Item = pageView;
			bool haveMore;
			do
			{
				ArrayOfRealItemsType localItemsList = null;
				haveMore = false;
				this.CallEwsWithRetries((LID)53436U, () => this.binding.FindItem(findItemType), delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					FindItemResponseMessageType findItemResponseMessageType = responseMessage as FindItemResponseMessageType;
					if (findItemResponseMessageType != null && findItemResponseMessageType.ResponseClass == ResponseClassType.Success && findItemResponseMessageType.RootFolder != null && findItemResponseMessageType.RootFolder.Item != null)
					{
						localItemsList = (findItemResponseMessageType.RootFolder.Item as ArrayOfRealItemsType);
						haveMore = !findItemResponseMessageType.RootFolder.IncludesLastItemInRange;
						if (haveMore)
						{
							pageView.Offset = findItemResponseMessageType.RootFolder.IndexedPagingOffset;
						}
						return true;
					}
					return false;
				}, null);
				if (localItemsList != null && localItemsList.Items != null && localItemsList.Items.Length > 0)
				{
					new List<BaseItemIdType>(localItemsList.Items.Length);
					foreach (ItemType item in localItemsList.Items)
					{
						if (item != null && item.ItemId != null && !string.IsNullOrEmpty(item.ItemId.Id))
						{
							yield return item.ItemId;
						}
					}
				}
			}
			while (haveMore);
			yield break;
		}

		private ItemType[] InternalFindItems(FindItemType findItemType, ItemResponseShapeType itemShape)
		{
			if (itemShape == null)
			{
				itemShape = EwsAuditClient.DefaultItemShape;
			}
			List<ItemType> items = null;
			IEnumerable<BaseItemIdType> enumerable = this.InternalFindItemIds(findItemType);
			using (IEnumerator<BaseItemIdType> enumerator = enumerable.GetEnumerator())
			{
				List<BaseItemIdType> list = new List<BaseItemIdType>(64);
				bool flag;
				do
				{
					flag = enumerator.MoveNext();
					if (flag)
					{
						list.Add(enumerator.Current);
					}
					if (list.Count == 64 || (list.Count > 0 && !flag))
					{
						GetItemType getItemType = new GetItemType
						{
							ItemShape = itemShape,
							ItemIds = list.ToArray()
						};
						this.CallEwsWithRetries((LID)41148U, () => this.binding.GetItem(getItemType), delegate(ResponseMessageType responseMessage, int messageIndex)
						{
							ItemInfoResponseMessageType itemInfoResponseMessageType = responseMessage as ItemInfoResponseMessageType;
							if (itemInfoResponseMessageType != null && itemInfoResponseMessageType.ResponseClass == ResponseClassType.Success && itemInfoResponseMessageType.Items != null)
							{
								ItemType[] items = itemInfoResponseMessageType.Items.Items;
								foreach (ItemType item in items)
								{
									if (items == null)
									{
										items = new List<ItemType>();
									}
									items.Add(item);
								}
								return false;
							}
							return false;
						}, delegate(ResponseMessageType responseMessage, int messageIndex)
						{
							if (responseMessage != null && responseMessage.ResponseClass == ResponseClassType.Error)
							{
								ResponseCodeType responseCode = responseMessage.ResponseCode;
								if (responseCode == ResponseCodeType.ErrorItemNotFound || responseCode == ResponseCodeType.ErrorMessageSizeExceeded)
								{
									return true;
								}
							}
							return false;
						});
						list.Clear();
					}
				}
				while (flag);
			}
			if (items != null && items.Count > 0)
			{
				return items.ToArray();
			}
			return Array<ItemType>.Empty;
		}

		private void CallEwsWithRetries(LID lid, Func<BaseResponseMessageType> delegateEwsCall, Func<ResponseMessageType, int, bool> responseMessageProcessor, Func<ResponseMessageType, int, bool> responseErrorProcessor)
		{
			ExDateTime t = ExDateTime.UtcNow.Add((this.CallTimeout < EwsAuditClient.TotalRetryTimeWindow) ? EwsAuditClient.TotalRetryTimeWindow : this.CallTimeout);
			bool flag = false;
			Exception ex = null;
			int num = -1;
			do
			{
				if (flag)
				{
					this.WaitBeforeRetry(num);
				}
				ex = null;
				flag = false;
				num++;
				try
				{
					DiagnosticContext.TraceDword((LID)57532U, (uint)num);
					BaseResponseMessageType baseResponseMessageType;
					using (DiagnosticContext.TraceLatency(lid))
					{
						baseResponseMessageType = delegateEwsCall();
					}
					int i = 0;
					while (i < baseResponseMessageType.ResponseMessages.Items.Length)
					{
						ResponseMessageType responseMessageType = baseResponseMessageType.ResponseMessages.Items[i];
						if (responseMessageProcessor != null && responseMessageProcessor(responseMessageType, i))
						{
							DiagnosticContext.TraceLocation((LID)32956U);
							if (this.IsTraceEnabled(TraceType.DebugTrace))
							{
								this.Tracer.TraceDebug(0L, "Successfully executed EWS call");
								break;
							}
							break;
						}
						else
						{
							if (responseMessageType.ResponseClass == ResponseClassType.Error)
							{
								DiagnosticContext.TraceDwordAndString((LID)33100U, (uint)responseMessageType.ResponseCode, responseMessageType.ResponseCode.ToString());
								if (responseMessageType.ResponseCode == ResponseCodeType.ErrorCrossSiteRequest)
								{
									if (this.IsTraceEnabled(TraceType.DebugTrace))
									{
										this.Tracer.TraceDebug(0L, "Crosssite request error , recreate exchange binding and reset the url caches");
									}
									flag = true;
									this.RefreshUrl(true);
									break;
								}
								if (!EwsAuditClient.TransientServiceErrors.Contains(responseMessageType.ResponseCode))
								{
									if (responseErrorProcessor == null || !responseErrorProcessor(responseMessageType, i))
									{
										if (this.IsTraceEnabled(TraceType.ErrorTrace))
										{
											this.Tracer.TraceError<string, string, string>(0L, "Permanent error encountered:  {0}, {1}, {2}", responseMessageType.ResponseClass.ToString(), responseMessageType.ResponseCode.ToString(), responseMessageType.MessageText.ToString());
										}
										DiagnosticContext.TraceLocation((LID)65212U);
										throw new AuditLogServiceException(responseMessageType.ResponseClass.ToString(), responseMessageType.ResponseCode.ToString(), responseMessageType.MessageText.ToString());
									}
									DiagnosticContext.TraceLocation((LID)48828U);
									if (this.IsTraceEnabled(TraceType.DebugTrace))
									{
										this.Tracer.TraceDebug(0L, "Error encountered, but was handled by error callback:  {0}, {1}, {2}, response index={3}", new object[]
										{
											responseMessageType.ResponseClass.ToString(),
											responseMessageType.ResponseCode.ToString(),
											responseMessageType.MessageText.ToString(),
											i
										});
									}
								}
								else
								{
									DiagnosticContext.TraceLocation((LID)40636U);
									flag = true;
									ex = new AuditLogServiceException(responseMessageType.ResponseClass.ToString(), responseMessageType.ResponseCode.ToString(), responseMessageType.MessageText.ToString());
									if (this.IsTraceEnabled(TraceType.DebugTrace))
									{
										this.Tracer.TraceDebug(0L, "Transient error encountered, will attempt to retry, Exception: " + ex);
									}
								}
							}
							i++;
						}
					}
				}
				catch (CommunicationException ex2)
				{
					DiagnosticContext.TraceLocation((LID)57020U);
					ex = ex2;
					flag = true;
				}
				catch (TimeoutException ex3)
				{
					DiagnosticContext.TraceLocation((LID)44732U);
					ex = ex3;
					flag = false;
				}
				catch (SoapException ex4)
				{
					DiagnosticContext.TraceLocation((LID)61116U);
					ex = ex4;
					flag = false;
				}
				catch (WebException ex5)
				{
					DiagnosticContext.TraceLocation((LID)36540U);
					ex = ex5;
					flag = false;
				}
				catch (IOException ex6)
				{
					DiagnosticContext.TraceLocation((LID)52924U);
					ex = ex6;
					flag = false;
				}
				catch (InvalidOperationException ex7)
				{
					DiagnosticContext.TraceLocation((LID)46780U);
					ex = ex7;
					flag = false;
				}
				catch (LocalizedException ex8)
				{
					DiagnosticContext.TraceLocation((LID)63164U);
					ex = ex8;
					flag = false;
				}
			}
			while (flag && t > ExDateTime.UtcNow);
			if (ex != null)
			{
				if (this.IsTraceEnabled(TraceType.ErrorTrace))
				{
					this.Tracer.TraceError<int, Exception>(0L, "Failed to make EWS call to audit log, Total attempts made {0}, Exception: {1} ", num, ex);
				}
				DiagnosticContext.TraceLocation((LID)38588U);
				throw this.GetSpecificException(ex);
			}
		}

		private void WaitBeforeRetry(int retryAttempt)
		{
			if (retryAttempt == 0)
			{
				return;
			}
			TimeSpan timeSpan;
			if (retryAttempt < 0 || retryAttempt >= EwsAuditClient.RetrySchedule.Length)
			{
				timeSpan = EwsAuditClient.RetrySchedule[EwsAuditClient.RetrySchedule.Length - 1];
			}
			else
			{
				timeSpan = EwsAuditClient.RetrySchedule[retryAttempt];
			}
			if (timeSpan > TimeSpan.Zero)
			{
				if (this.IsTraceEnabled(TraceType.DebugTrace))
				{
					this.Tracer.TraceDebug<TimeSpan>(0L, "Sleeping before next retry for {0}.", timeSpan);
				}
				DiagnosticContext.TraceDword((LID)57676U, (uint)timeSpan.TotalMilliseconds);
				Thread.Sleep(timeSpan);
			}
		}

		private Exception GetSpecificException(Exception exception)
		{
			SoapException ex = exception as SoapException;
			if (ex != null && ex.Code != null && !string.IsNullOrEmpty(ex.Code.Name))
			{
				ResponseCodeType responseCodeType;
				bool flag = Enum.TryParse<ResponseCodeType>(ex.Code.Name, out responseCodeType);
				if (flag)
				{
					if (this.IsTraceEnabled(TraceType.ErrorTrace))
					{
						this.Tracer.TraceError<XmlQualifiedName, string>(0L, "EwsAuditClient EWS response Code: {0} Detail: {1} ", ex.Code, ex.Detail.InnerText);
					}
					ResponseCodeType responseCodeType2 = responseCodeType;
					if (responseCodeType2 == ResponseCodeType.ErrorAccessDenied || responseCodeType2 == ResponseCodeType.ErrorTokenSerializationDenied)
					{
						return new AuditLogAccessDeniedException(ex);
					}
				}
				return new AuditLogServiceException(string.Empty, ex.Code.Name, ex.Message, exception);
			}
			if (exception is AuditLogException)
			{
				return exception;
			}
			return new AuditLogException(Strings.FailedToAccessAuditLogWithInnerException, exception);
		}

		private const int MaxFoldersPerPage = 256;

		private const int MaxItemsPerPage = 64;

		private const int MaxItemsPerFindItemPage = 512;

		internal static readonly TimeSpan DefaultSoapClientTimeout = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan TotalRetryTimeWindow = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan[] RetrySchedule = new TimeSpan[]
		{
			TimeSpan.FromMilliseconds(10.0),
			TimeSpan.FromMilliseconds(100.0),
			TimeSpan.FromMilliseconds(200.0),
			TimeSpan.FromMilliseconds(500.0),
			TimeSpan.FromSeconds(1.0),
			TimeSpan.FromSeconds(2.0)
		};

		internal static readonly List<ResponseCodeType> TransientServiceErrors = new List<ResponseCodeType>
		{
			ResponseCodeType.ErrorADOperation,
			ResponseCodeType.ErrorADUnavailable,
			ResponseCodeType.ErrorConnectionFailed,
			ResponseCodeType.ErrorInsufficientResources,
			ResponseCodeType.ErrorInternalServerTransientError,
			ResponseCodeType.ErrorMailboxMoveInProgress,
			ResponseCodeType.ErrorMailboxStoreUnavailable,
			ResponseCodeType.ErrorServerBusy,
			ResponseCodeType.ErrorCrossSiteRequest,
			ResponseCodeType.ErrorExceededConnectionCount,
			ResponseCodeType.ErrorNotEnoughMemory,
			ResponseCodeType.ErrorStaleObject,
			ResponseCodeType.ErrorTooManyObjectsOpened,
			ResponseCodeType.ErrorFolderSavePropertyError,
			ResponseCodeType.ErrorItemSavePropertyError,
			ResponseCodeType.ErrorTimeoutExpired
		};

		private static readonly FolderResponseShapeType DefaultFolderShape = new FolderResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.Default
		};

		internal static readonly PathToExtendedFieldType TextBodyField = new PathToExtendedFieldType
		{
			PropertyTag = 4096.ToString(),
			PropertyType = MapiPropertyTypeType.String
		};

		private static readonly ItemResponseShapeType DefaultItemShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			AdditionalProperties = new BasePathToElementType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemDateTimeCreated
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemDateTimeReceived
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemItemClass
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemSubject
				},
				EwsAuditClient.TextBodyField
			}
		};

		private static readonly ItemResponseShapeType IdOnlyItemShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly
		};

		private IExchangeService binding;

		private ExDateTime lastUrlDiscoverTime = ExDateTime.MinValue;

		private readonly TimeSpan ExpiryInterval = TimeSpan.FromMinutes(5.0);

		private IEwsConnectionManager connectionManager;
	}
}
