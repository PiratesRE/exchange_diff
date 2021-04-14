using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal sealed class EwsClient : BaseServiceClient<ExchangeServiceBinding, ISourceDataProvider>, IEwsClient, ISourceDataProvider
	{
		public EwsClient(Uri serviceEndpoint, IServiceCallingContext<ExchangeServiceBinding> serviceCallingContext, CancellationToken abortTokenForTasks) : base(serviceEndpoint, serviceCallingContext, abortTokenForTasks)
		{
		}

		public EwsClient(Uri serviceEndpoint, IServiceCallingContext<ExchangeServiceBinding> serviceCallingContext) : base(serviceEndpoint, serviceCallingContext, CancellationToken.None)
		{
		}

		public override ISourceDataProvider FunctionalInterface
		{
			get
			{
				return this;
			}
		}

		public string GetRootFolder(string mailboxEmailAddress, bool isArchive)
		{
			GetFolderType getFolder = new GetFolderType();
			FolderResponseShapeType folderResponseShapeType = new FolderResponseShapeType();
			folderResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			getFolder.FolderShape = folderResponseShapeType;
			getFolder.FolderIds = new DistinguishedFolderIdType[]
			{
				isArchive ? new DistinguishedFolderIdType
				{
					Id = DistinguishedFolderIdNameType.archiveroot,
					Mailbox = new EmailAddressType
					{
						EmailAddress = mailboxEmailAddress
					}
				} : new DistinguishedFolderIdType
				{
					Id = DistinguishedFolderIdNameType.root,
					Mailbox = new EmailAddressType
					{
						EmailAddress = mailboxEmailAddress
					}
				}
			};
			string rootFolder = null;
			this.CallEws(() => this.ServiceBinding.GetFolder(getFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					if (!isArchive || responseMessage.ResponseCode != ResponseCodeType.ErrorFolderNotFound)
					{
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetRootFolders, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
					Tracer.TraceInformation("The archive root folder of mailbox '{0}' doesn't exist.", new object[]
					{
						mailboxEmailAddress
					});
				}
				else
				{
					FolderInfoResponseMessageType folderInfoResponseMessageType = (FolderInfoResponseMessageType)responseMessage;
					rootFolder = folderInfoResponseMessageType.Folders[0].FolderId.Id;
				}
				return false;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetRootFolders, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return rootFolder;
		}

		public void GetAllFolders(string mailboxEmailAddress, string parentFolderId, bool isDeepTraversal, bool isArchive, Dictionary<string, string> resultFolderInformations)
		{
			FindFolderType findFolder = new FindFolderType();
			FolderResponseShapeType folderResponseShapeType = new FolderResponseShapeType();
			folderResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			folderResponseShapeType.AdditionalProperties = new BasePathToElementType[]
			{
				new PathToExtendedFieldType
				{
					PropertyTag = "0x66B5",
					PropertyType = MapiPropertyTypeType.String
				}
			};
			findFolder.FolderShape = folderResponseShapeType;
			if (!string.IsNullOrEmpty(parentFolderId))
			{
				findFolder.ParentFolderIds = new BaseFolderIdType[]
				{
					new FolderIdType
					{
						Id = parentFolderId
					}
				};
			}
			else
			{
				findFolder.ParentFolderIds = new BaseFolderIdType[]
				{
					new DistinguishedFolderIdType
					{
						Id = (isArchive ? DistinguishedFolderIdNameType.archiveroot : DistinguishedFolderIdNameType.root),
						Mailbox = new EmailAddressType
						{
							EmailAddress = mailboxEmailAddress
						}
					}
				};
			}
			findFolder.Traversal = (isDeepTraversal ? FolderQueryTraversalType.Deep : FolderQueryTraversalType.Shallow);
			bool morePage = true;
			int offset = 0;
			while (morePage)
			{
				findFolder.Item = new IndexedPageViewType
				{
					BasePoint = IndexBasePointType.Beginning,
					Offset = offset
				};
				this.CallEws(() => this.ServiceBinding.FindFolder(findFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					FindFolderResponseMessageType findFolderResponseMessageType = (FindFolderResponseMessageType)responseMessage;
					if (responseMessage.ResponseClass == ResponseClassType.Error)
					{
						if (responseMessage.ResponseCode != ResponseCodeType.ErrorFolderNotFound || !isArchive)
						{
							throw new RetryException(new ExportException(ExportErrorType.FailedToGetFolderHierarchy, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
						}
						Tracer.TraceInformation("Mailbox '{0}' doesn't have an archive mailbox. Ignoring the error when retrieving archive folder hierarchy.", new object[]
						{
							mailboxEmailAddress
						});
						morePage = false;
					}
					else
					{
						foreach (BaseFolderType baseFolderType in findFolderResponseMessageType.RootFolder.Folders)
						{
							string str = string.Empty;
							if (baseFolderType.ExtendedProperty != null && baseFolderType.ExtendedProperty.Length == 1)
							{
								str = ((baseFolderType.ExtendedProperty[0].Item as string) ?? string.Empty);
							}
							resultFolderInformations.Add(baseFolderType.FolderId.Id, (isArchive ? "\\archive" : "\\root") + str);
						}
						if (findFolderResponseMessageType.RootFolder.IncludesLastItemInRange || !findFolderResponseMessageType.RootFolder.IncludesLastItemInRangeSpecified)
						{
							morePage = false;
						}
						else
						{
							offset = findFolderResponseMessageType.RootFolder.IndexedPagingOffset;
						}
					}
					return false;
				}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetFolderHierarchy, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			}
		}

		public List<ItemInformation> ExportItems(string mailboxEmailAddress, IList<ItemId> messageIds, bool isDocIdHintFlightingEnabled = false)
		{
			ExportItemsType exportItemsType = new ExportItemsType();
			exportItemsType.ItemIds = (from messageId in messageIds
			select new ItemIdType
			{
				Id = messageId.Id
			}).ToArray<ItemIdType>();
			List<ItemInformation> items = new List<ItemInformation>(exportItemsType.ItemIds.Length);
			List<ItemInformation> newRetryItems = new List<ItemInformation>();
			List<ItemInformation> savedErrorItemInformations = new List<ItemInformation>();
			List<ItemId> retryItems = new List<ItemId>();
			int startIndex = 0;
			this.CallEws(() => this.ServiceBinding.ExportItems(exportItemsType), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				ExportItemsResponseMessageType exportItemsResponseMessageType = (ExportItemsResponseMessageType)responseMessage;
				if (exportItemsResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					Tracer.TraceError("ExportItems with error response message. ResponseCode : {0}", new object[]
					{
						exportItemsResponseMessageType.ResponseCode
					});
					if (exportItemsResponseMessageType.ResponseCode == ResponseCodeType.ErrorItemNotFound)
					{
						retryItems.Add(new ItemId
						{
							Id = messageIds[messageIndex].IdMarkerDocumentId,
							Size = messageIds[messageIndex].Size,
							ParentFolder = messageIds[messageIndex].ParentFolder,
							PrimaryItemId = messageIds[messageIndex].PrimaryItemId,
							UniqueHash = messageIds[messageIndex].UniqueHash,
							InternetMessageId = messageIds[messageIndex].InternetMessageId,
							Subject = messageIds[messageIndex].Subject,
							Sender = messageIds[messageIndex].Sender,
							SentTime = messageIds[messageIndex].SentTime,
							ReceivedTime = messageIds[messageIndex].ReceivedTime,
							BodyPreview = messageIds[messageIndex].BodyPreview,
							Importance = messageIds[messageIndex].Importance,
							IsRead = messageIds[messageIndex].IsRead,
							HasAttachment = messageIds[messageIndex].HasAttachment,
							ToRecipients = messageIds[messageIndex].ToRecipients,
							CcRecipients = messageIds[messageIndex].CcRecipients,
							BccRecipients = messageIds[messageIndex].BccRecipients,
							DocumentId = messageIds[messageIndex].DocumentId,
							ToGroupExpansionRecipients = messageIds[messageIndex].ToGroupExpansionRecipients,
							CcGroupExpansionRecipients = messageIds[messageIndex].CcGroupExpansionRecipients,
							BccGroupExpansionRecipients = messageIds[messageIndex].BccGroupExpansionRecipients,
							DGGroupExpansionError = messageIds[messageIndex].DGGroupExpansionError
						});
						savedErrorItemInformations.Add(new ItemInformation
						{
							Id = messageIds[messageIndex],
							Data = null,
							Error = new ExportException(ExportErrorType.NotFound)
						});
					}
					else
					{
						items.Add(new ItemInformation
						{
							Id = messageIds[messageIndex],
							Data = null,
							Error = new ExportException(ExportErrorType.FailedToExportItem, exportItemsResponseMessageType.ResponseCode.ToString())
						});
					}
				}
				else if (exportItemsResponseMessageType.ResponseClass == ResponseClassType.Warning)
				{
					Tracer.TraceWarning("EwsClient.ExportItems: Message response warning. ResponseCode:{0}; MessageText:'{1}'", new object[]
					{
						exportItemsResponseMessageType.ResponseCode,
						exportItemsResponseMessageType.MessageText
					});
					if (exportItemsResponseMessageType.ResponseCode == ResponseCodeType.ErrorBatchProcessingStopped)
					{
						Tracer.TraceWarning("EwsClient.ExportItems: Hitting ErrorBatchProcessingStopped, startIndex:{0}; messageIndex={1}", new object[]
						{
							startIndex,
							messageIndex
						});
						if (messageIndex == 0)
						{
							throw new RetryException(new ExportException(ExportErrorType.FailedToExportItem, "Internal Error: Hitting ErrorBatchProcessingStopped as the first response."), false);
						}
						startIndex += messageIndex;
						exportItemsType.ItemIds = exportItemsType.ItemIds.Skip(messageIndex).ToArray<ItemIdType>();
						throw new RetryException(new ExportException(ExportErrorType.FailedToExportItem, "Hitting ErrorBatchProcessingStopped"), true);
					}
				}
				else
				{
					items.Add(new ItemInformation
					{
						Id = messageIds[startIndex + messageIndex],
						Data = exportItemsResponseMessageType.Data,
						Error = null
					});
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToExportItem, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			if (retryItems.Count > 0)
			{
				if (isDocIdHintFlightingEnabled)
				{
					exportItemsType.ItemIds = (from retryId in retryItems
					select new ItemIdType
					{
						Id = retryId.Id
					}).ToArray<ItemIdType>();
					startIndex = 0;
					this.CallEws(() => this.ServiceBinding.ExportItems(exportItemsType), delegate(ResponseMessageType responseMessage, int messageIndex)
					{
						ExportItemsResponseMessageType exportItemsResponseMessageType = (ExportItemsResponseMessageType)responseMessage;
						if (exportItemsResponseMessageType.ResponseClass == ResponseClassType.Error)
						{
							Tracer.TraceError("ExportItems with error response message. ResponseCode : {0}", new object[]
							{
								exportItemsResponseMessageType.ResponseCode
							});
							if (exportItemsResponseMessageType.ResponseCode == ResponseCodeType.ErrorItemNotFound)
							{
								newRetryItems.Add(new ItemInformation
								{
									Id = retryItems[messageIndex],
									Data = null,
									Error = new ExportException(ExportErrorType.NotFound)
								});
							}
							else
							{
								newRetryItems.Add(savedErrorItemInformations[messageIndex]);
							}
						}
						else if (exportItemsResponseMessageType.ResponseClass == ResponseClassType.Warning)
						{
							Tracer.TraceWarning("EwsClient.ExportItems: Message response warning during retry. ResponseCode:{0}; MessageText:'{1}'", new object[]
							{
								exportItemsResponseMessageType.ResponseCode,
								exportItemsResponseMessageType.MessageText
							});
							if (exportItemsResponseMessageType.ResponseCode == ResponseCodeType.ErrorBatchProcessingStopped)
							{
								Tracer.TraceWarning("EwsClient.ExportItems: Hitting ErrorBatchProcessingStopped during retry, startIndex:{0}; messageIndex={1}", new object[]
								{
									startIndex,
									messageIndex
								});
								if (messageIndex == 0)
								{
									throw new RetryException(new ExportException(ExportErrorType.FailedToExportItem, "Internal Error: Hitting ErrorBatchProcessingStopped during retry as the first response."), false);
								}
								startIndex += messageIndex;
								exportItemsType.ItemIds = exportItemsType.ItemIds.Skip(messageIndex).ToArray<ItemIdType>();
								throw new RetryException(new ExportException(ExportErrorType.FailedToExportItem, "Hitting ErrorBatchProcessingStopped during retry"), true);
							}
						}
						else
						{
							newRetryItems.Add(new ItemInformation
							{
								Id = retryItems[startIndex + messageIndex],
								Data = exportItemsResponseMessageType.Data,
								Error = null
							});
						}
						return true;
					}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToExportItem, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
					items.AddRange(newRetryItems);
				}
				else
				{
					items.AddRange(savedErrorItemInformations);
				}
			}
			return items;
		}

		public List<ItemId> SearchMailboxes(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, ref string pageItemReference, out List<ErrorRecord> failedMailboxes, bool isArchive, string searchConfiguration = null)
		{
			bool isUnsearchable = false;
			bool flag = false;
			return this.SearchMailboxes(mailboxEmailAddress, query, language, mailboxIds, isUnsearchable, ref pageItemReference, out failedMailboxes, out flag, isArchive, searchConfiguration);
		}

		public List<ItemId> SearchMailboxes(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, bool isUnsearchable, ref string pageItemReference, out List<ErrorRecord> failedMailboxes, out bool newSchemaSearchSucceeded, bool isArchive, string searchConfiguration = null)
		{
			SearchMailboxesType searchMailboxesType = new SearchMailboxesType();
			if (pageItemReference != null)
			{
				searchMailboxesType.PageItemReference = pageItemReference;
			}
			searchMailboxesType.PageSize = ConstantProvider.SearchMailboxesPageSize;
			searchMailboxesType.PageSizeSpecified = true;
			searchMailboxesType.PageDirection = SearchPageDirectionType.Next;
			searchMailboxesType.PageDirectionSpecified = true;
			searchMailboxesType.Deduplication = false;
			searchMailboxesType.DeduplicationSpecified = true;
			searchMailboxesType.ResultType = SearchResultType.PreviewOnly;
			searchMailboxesType.SearchQueries = new MailboxQueryType[]
			{
				new MailboxQueryType
				{
					Query = query,
					MailboxSearchScopes = (from mailboxId in mailboxIds
					select new MailboxSearchScopeType
					{
						Mailbox = mailboxId,
						SearchScope = (isArchive ? MailboxSearchLocationType.ArchiveOnly : MailboxSearchLocationType.PrimaryOnly)
					}).ToArray<MailboxSearchScopeType>()
				}
			};
			List<MailboxSearchScopeType> list = new List<MailboxSearchScopeType>();
			foreach (MailboxSearchScopeType mailboxSearchScopeType in searchMailboxesType.SearchQueries[0].MailboxSearchScopes)
			{
				MailboxSearchScopeType mailboxSearchScopeType2 = new MailboxSearchScopeType
				{
					Mailbox = mailboxSearchScopeType.Mailbox,
					SearchScope = mailboxSearchScopeType.SearchScope
				};
				if (mailboxSearchScopeType2.Mailbox.StartsWith("\\"))
				{
					mailboxSearchScopeType2.ExtendedAttributes = new ExtendedAttributeType[]
					{
						new ExtendedAttributeType
						{
							Name = "SearchScopeType",
							Value = "AutoDetect"
						}
					};
				}
				list.Add(mailboxSearchScopeType2);
			}
			searchMailboxesType.SearchQueries[0].MailboxSearchScopes = list.ToArray();
			searchMailboxesType.SortBy = new FieldOrderType
			{
				Item = new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemDateTimeReceived
				},
				Order = SortDirectionType.Descending
			};
			if (!string.IsNullOrEmpty(language))
			{
				searchMailboxesType.Language = language;
			}
			PathToExtendedFieldType documentIdProp = new PathToExtendedFieldType();
			documentIdProp.PropertyTag = "0x6815";
			documentIdProp.PropertyType = MapiPropertyTypeType.Integer;
			List<PathToExtendedFieldType> list2 = new List<PathToExtendedFieldType>();
			list2.Add(documentIdProp);
			list2.AddRange(EwsClient.mDGExpansionAdditionalProperties);
			PathToExtendedFieldType senderSmtpAddressProp = new PathToExtendedFieldType();
			senderSmtpAddressProp.PropertyTag = "0x5d01";
			senderSmtpAddressProp.PropertyType = MapiPropertyTypeType.String;
			list2.Add(senderSmtpAddressProp);
			PreviewItemResponseShapeType previewItemResponseShapeType = new PreviewItemResponseShapeType();
			previewItemResponseShapeType.AdditionalProperties = list2.ToArray();
			searchMailboxesType.PreviewItemResponseShape = previewItemResponseShapeType;
			List<ItemId> itemIds = null;
			string pageReference = null;
			List<ErrorRecord> failedMailboxesTmp = new List<ErrorRecord>(1);
			Func<ResponseMessageType, int, bool> func = delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				SearchMailboxesResponseMessageType searchMailboxesResponseMessageType = (SearchMailboxesResponseMessageType)responseMessage;
				if (searchMailboxesResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					if (searchMailboxesResponseMessageType.ResponseCode == ResponseCodeType.ErrorAccessDenied)
					{
						Tracer.TraceError("EwsClient.SearchMailboxes: ResponseCodeType.ErrorAccessDenied.", new object[0]);
						throw new ExportException(ExportErrorType.Unauthorized, searchMailboxesResponseMessageType.MessageText);
					}
					if (!isArchive || searchMailboxesResponseMessageType.ResponseCode != ResponseCodeType.ErrorInvalidOperation)
					{
						Tracer.TraceError("EwsClient.SearchMailboxes: SearchMailbox with error response message. ResponseCode : {0}", new object[]
						{
							searchMailboxesResponseMessageType.ResponseCode
						});
						throw new RetryException(new ExportException(ExportErrorType.FailedToSearchMailboxes, searchMailboxesResponseMessageType.ResponseCode.ToString() + " : " + searchMailboxesResponseMessageType.MessageText), false);
					}
				}
				else
				{
					itemIds = new List<ItemId>(searchMailboxesResponseMessageType.SearchMailboxesResult.PageItemCount);
					if (searchMailboxesResponseMessageType.SearchMailboxesResult.PageItemCount > 0)
					{
						int num = searchMailboxesResponseMessageType.SearchMailboxesResult.Items.Length;
						pageReference = ((num > 0) ? searchMailboxesResponseMessageType.SearchMailboxesResult.Items[num - 1].SortValue : null);
						foreach (SearchPreviewItemType searchPreviewItemType in searchMailboxesResponseMessageType.SearchMailboxesResult.Items)
						{
							ItemId itemId = new ItemId
							{
								Id = searchPreviewItemType.Id.Id,
								ParentFolder = searchPreviewItemType.ParentId.Id,
								Size = (uint)searchPreviewItemType.Size,
								UniqueHash = searchPreviewItemType.UniqueHash,
								Subject = searchPreviewItemType.Subject,
								Sender = searchPreviewItemType.Sender,
								SentTime = searchPreviewItemType.SentTime,
								ReceivedTime = searchPreviewItemType.ReceivedTime,
								BodyPreview = searchPreviewItemType.Preview,
								Importance = searchPreviewItemType.Importance.ToString(),
								IsRead = searchPreviewItemType.Read,
								HasAttachment = searchPreviewItemType.HasAttachment,
								ToRecipients = string.Join(";", searchPreviewItemType.ToRecipients ?? new string[0]),
								CcRecipients = string.Join(";", searchPreviewItemType.CcRecipients ?? new string[0]),
								BccRecipients = string.Join(";", searchPreviewItemType.BccRecipients ?? new string[0]),
								NeedsDGExpansion = false
							};
							if (searchPreviewItemType.ExtendedProperties != null)
							{
								NonEmptyArrayOfExtendedPropertyType extendedProperties = searchPreviewItemType.ExtendedProperties;
								ExtendedPropertyType[] items2 = extendedProperties.Items;
								foreach (ExtendedPropertyType extendedPropertyType in items2)
								{
									if (extendedPropertyType.ExtendedFieldURI != null && extendedPropertyType.Item != null)
									{
										if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyName, EwsClient.GroupExpansionRecipientsPropertyName, StringComparison.OrdinalIgnoreCase) == 0)
										{
											string text = (string)extendedPropertyType.Item;
											if (text != null)
											{
												if (text.Length > 254)
												{
													itemId.NeedsDGExpansion = true;
												}
												else
												{
													this.ParseDGExpansion(text, "EwsClient.ParseDGExpansion, called From EwsClient.SearchMailboxes: DataCorrupted in GroupExpansionRecipients. exception: {0}", ref itemId);
												}
											}
										}
										else if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyName, EwsClient.GroupExpansionErrorPropertyName, StringComparison.OrdinalIgnoreCase) == 0 && !string.IsNullOrEmpty((string)extendedPropertyType.Item))
										{
											try
											{
												itemId.DGGroupExpansionError = new DistributionGroupExpansionError?((DistributionGroupExpansionError)Enum.Parse(typeof(DistributionGroupExpansionError), (string)extendedPropertyType.Item));
											}
											catch (ArgumentException)
											{
												Tracer.TraceWarning("EwsClient.SearchMailboxes: The parsing of DistributionGroupExpansionError threw an argument exception. Input string: {0}", new object[]
												{
													extendedPropertyType.Item
												});
											}
										}
										if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyTag, documentIdProp.PropertyTag, StringComparison.OrdinalIgnoreCase) == 0)
										{
											string text2 = (string)extendedPropertyType.Item;
											if (text2 != null)
											{
												int documentId = Convert.ToInt32(text2);
												itemId.DocumentId = documentId;
											}
										}
										if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyTag, senderSmtpAddressProp.PropertyTag, StringComparison.OrdinalIgnoreCase) == 0)
										{
											string text3 = (string)extendedPropertyType.Item;
											if (text3 != null)
											{
												itemId.SenderSmtpAddress = text3;
											}
										}
									}
								}
							}
							itemIds.Add(itemId);
						}
					}
					else
					{
						pageReference = null;
					}
					if (searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes != null && searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes.Length > 0)
					{
						foreach (FailedSearchMailboxType failedSearchMailboxType in searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes)
						{
							failedMailboxesTmp.Add(new ErrorRecord
							{
								ErrorType = ExportErrorType.FailedToSearchMailboxes,
								Time = DateTime.UtcNow,
								Item = null,
								DiagnosticMessage = failedSearchMailboxType.ErrorMessage,
								SourceId = failedSearchMailboxType.Mailbox
							});
						}
					}
				}
				return false;
			};
			if (searchConfiguration != null && (mailboxIds == null || mailboxIds.Count<string>() == 0))
			{
				ExtendedAttributeType unsearchableExtendedAttribute = null;
				if (isUnsearchable)
				{
					unsearchableExtendedAttribute = new ExtendedAttributeType
					{
						Name = "SearchType",
						Value = "NonIndexedItemPreview"
					};
				}
				SearchMailboxesResponseMessageType arg;
				newSchemaSearchSucceeded = this.SearchMailboxesWithNewSchema(ref searchMailboxesType, out arg, unsearchableExtendedAttribute, searchConfiguration, mailboxEmailAddress);
				func(arg, 0);
			}
			else
			{
				newSchemaSearchSucceeded = false;
				this.CallEws(() => this.ServiceBinding.SearchMailboxes(searchMailboxesType), func, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToSearchMailboxes, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
			}
			pageItemReference = pageReference;
			failedMailboxes = failedMailboxesTmp;
			return itemIds;
		}

		public void GetSearchResultEstimation(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, out int mailboxesSearchedCount, out long itemCount, out long totalSize, out List<ErrorRecord> failedMailboxes, string searchConfiguration = null)
		{
			bool isUnsearchable = false;
			bool flag = false;
			this.GetSearchResultEstimation(mailboxEmailAddress, query, language, mailboxIds, out mailboxesSearchedCount, isUnsearchable, out itemCount, out totalSize, out failedMailboxes, out flag, null);
		}

		public void GetSearchResultEstimation(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, out int mailboxesSearchedCount, bool isUnsearchable, out long itemCount, out long totalSize, out List<ErrorRecord> failedMailboxes, out bool newSchemaSearchSucceeded, string searchConfiguration = null)
		{
			int mbxCount = 0;
			long count = 0L;
			long size = 0L;
			mailboxIds.Count<string>();
			SearchMailboxesType searchMailboxesType = new SearchMailboxesType();
			searchMailboxesType.PageSize = 1;
			searchMailboxesType.PageSizeSpecified = true;
			searchMailboxesType.ResultType = SearchResultType.PreviewOnly;
			SearchMailboxesType searchMailboxesType2 = searchMailboxesType;
			MailboxQueryType[] array = new MailboxQueryType[1];
			MailboxQueryType[] array2 = array;
			int num = 0;
			MailboxQueryType mailboxQueryType = new MailboxQueryType();
			mailboxQueryType.Query = query;
			mailboxQueryType.MailboxSearchScopes = (from mailboxId in mailboxIds
			select new MailboxSearchScopeType
			{
				Mailbox = mailboxId,
				SearchScope = MailboxSearchLocationType.All
			}).ToArray<MailboxSearchScopeType>();
			array2[num] = mailboxQueryType;
			searchMailboxesType2.SearchQueries = array;
			if (!string.IsNullOrEmpty(language))
			{
				searchMailboxesType.Language = language;
			}
			List<ErrorRecord> failedMailboxesTmp = new List<ErrorRecord>(1);
			Func<ResponseMessageType, int, bool> func = delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				SearchMailboxesResponseMessageType searchMailboxesResponseMessageType = (SearchMailboxesResponseMessageType)responseMessage;
				if (searchMailboxesResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					Tracer.TraceError("SearchMailbox with error response message. ResponseCode : {0}", new object[]
					{
						searchMailboxesResponseMessageType.ResponseCode
					});
					throw new RetryException(new ExportException(ExportErrorType.FailedToSearchMailboxes, searchMailboxesResponseMessageType.ResponseCode.ToString() + " : " + searchMailboxesResponseMessageType.MessageText), false);
				}
				count = searchMailboxesResponseMessageType.SearchMailboxesResult.ItemCount;
				size = searchMailboxesResponseMessageType.SearchMailboxesResult.Size;
				if (searchMailboxesResponseMessageType.SearchMailboxesResult.MailboxStats != null && searchMailboxesResponseMessageType.SearchMailboxesResult.MailboxStats.Count<MailboxStatisticsItemType>() > 0)
				{
					mbxCount = searchMailboxesResponseMessageType.SearchMailboxesResult.MailboxStats.Count<MailboxStatisticsItemType>();
				}
				if (searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes != null && searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes.Length > 0)
				{
					foreach (FailedSearchMailboxType failedSearchMailboxType in searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes)
					{
						failedMailboxesTmp.Add(new ErrorRecord
						{
							ErrorType = ExportErrorType.FailedToSearchMailboxes,
							Time = DateTime.UtcNow,
							Item = null,
							DiagnosticMessage = failedSearchMailboxType.ErrorMessage,
							SourceId = failedSearchMailboxType.Mailbox
						});
					}
				}
				return false;
			};
			SearchMailboxesResponseMessageType arg = null;
			newSchemaSearchSucceeded = false;
			if (searchConfiguration != null && (mailboxIds == null || mailboxIds.Count<string>() == 0))
			{
				ExtendedAttributeType unsearchableExtendedAttribute = null;
				if (isUnsearchable)
				{
					unsearchableExtendedAttribute = new ExtendedAttributeType
					{
						Name = "SearchType",
						Value = "NonIndexedItemStatistics"
					};
				}
				this.SearchMailboxesWithNewSchema(ref searchMailboxesType, out arg, unsearchableExtendedAttribute, searchConfiguration, mailboxEmailAddress);
				func(arg, 0);
				newSchemaSearchSucceeded = true;
			}
			else
			{
				this.CallEws(() => this.ServiceBinding.SearchMailboxes(searchMailboxesType), func, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToSearchMailboxes, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
			}
			mailboxesSearchedCount = mbxCount;
			itemCount = count;
			totalSize = size;
			failedMailboxes = failedMailboxesTmp;
		}

		public List<KeywordStatisticsSearchResultType> GetKeywordStatistics(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, out List<ErrorRecord> failedMailboxes, string searchConfiguration = null)
		{
			SearchMailboxesType searchMailboxesType = new SearchMailboxesType();
			searchMailboxesType.ResultType = SearchResultType.StatisticsOnly;
			SearchMailboxesType searchMailboxesType2 = searchMailboxesType;
			MailboxQueryType[] array = new MailboxQueryType[1];
			MailboxQueryType[] array2 = array;
			int num = 0;
			MailboxQueryType mailboxQueryType = new MailboxQueryType();
			mailboxQueryType.Query = query;
			mailboxQueryType.MailboxSearchScopes = (from mailboxId in mailboxIds
			select new MailboxSearchScopeType
			{
				Mailbox = mailboxId,
				SearchScope = MailboxSearchLocationType.All
			}).ToArray<MailboxSearchScopeType>();
			array2[num] = mailboxQueryType;
			searchMailboxesType2.SearchQueries = array;
			if (!string.IsNullOrEmpty(language))
			{
				searchMailboxesType.Language = language;
			}
			List<ErrorRecord> failedMailboxesTmp = new List<ErrorRecord>(1);
			List<KeywordStatisticsSearchResultType> keywordStatistics = new List<KeywordStatisticsSearchResultType>(1);
			Func<ResponseMessageType, int, bool> func = delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				SearchMailboxesResponseMessageType searchMailboxesResponseMessageType = (SearchMailboxesResponseMessageType)responseMessage;
				if (searchMailboxesResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					Tracer.TraceError("SearchMailbox with error response message. ResponseCode : {0}", new object[]
					{
						searchMailboxesResponseMessageType.ResponseCode
					});
					keywordStatistics.Clear();
					failedMailboxesTmp.Clear();
					throw new RetryException(new ExportException(ExportErrorType.FailedToSearchMailboxes, searchMailboxesResponseMessageType.ResponseCode.ToString() + " : " + searchMailboxesResponseMessageType.MessageText), false);
				}
				if (searchMailboxesResponseMessageType.SearchMailboxesResult.KeywordStats != null)
				{
					keywordStatistics.AddRange(searchMailboxesResponseMessageType.SearchMailboxesResult.KeywordStats);
				}
				if (searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes != null && searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes.Length > 0)
				{
					foreach (FailedSearchMailboxType failedSearchMailboxType in searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes)
					{
						failedMailboxesTmp.Add(new ErrorRecord
						{
							ErrorType = ExportErrorType.FailedToSearchMailboxes,
							Time = DateTime.UtcNow,
							Item = null,
							DiagnosticMessage = failedSearchMailboxType.ErrorMessage,
							SourceId = failedSearchMailboxType.Mailbox
						});
					}
				}
				return true;
			};
			if (searchConfiguration != null && (mailboxIds == null || mailboxIds.Count<string>() == 0))
			{
				SearchMailboxesResponseMessageType arg;
				this.SearchMailboxesWithNewSchema(ref searchMailboxesType, out arg, null, searchConfiguration, mailboxEmailAddress);
				func(arg, 0);
			}
			else
			{
				this.CallEws(() => this.ServiceBinding.SearchMailboxes(searchMailboxesType), func, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToSearchMailboxes, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
			}
			failedMailboxes = failedMailboxesTmp;
			return keywordStatistics;
		}

		public long GetUnsearchableItemStatistics(string mailboxEmailAddress, string mailboxId)
		{
			GetNonIndexableItemStatisticsType getNonIndexableItemStatistics = new GetNonIndexableItemStatisticsType();
			getNonIndexableItemStatistics.Mailboxes = new string[]
			{
				mailboxId
			};
			long itemCount = 0L;
			this.CallEws(() => this.ServiceBinding.GetNonIndexableItemStatistics(getNonIndexableItemStatistics), delegate(ResponseMessageType responseMessage)
			{
				GetNonIndexableItemStatisticsResponseMessageType getNonIndexableItemStatisticsResponseMessageType = (GetNonIndexableItemStatisticsResponseMessageType)responseMessage;
				if (getNonIndexableItemStatisticsResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					Tracer.TraceError("GetNonIndexableItemStatistics with error response message. ResponseCode : {0}", new object[]
					{
						getNonIndexableItemStatisticsResponseMessageType.ResponseCode
					});
					throw new RetryException(new ExportException(ExportErrorType.FailedToGetUnsearchableItemStatistics, getNonIndexableItemStatisticsResponseMessageType.ResponseCode.ToString() + " : " + getNonIndexableItemStatisticsResponseMessageType.MessageText), false);
				}
				if (getNonIndexableItemStatisticsResponseMessageType.NonIndexableItemStatistics != null && getNonIndexableItemStatisticsResponseMessageType.NonIndexableItemStatistics.Length > 0)
				{
					foreach (NonIndexableItemStatisticType nonIndexableItemStatisticType in getNonIndexableItemStatisticsResponseMessageType.NonIndexableItemStatistics)
					{
						itemCount += nonIndexableItemStatisticType.ItemCount;
					}
				}
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetUnsearchableItemStatistics, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
			return itemCount;
		}

		public List<UnsearchableItemId> GetUnsearchableItems(string mailboxEmailAddress, string mailboxId, ref string pageItemReference)
		{
			GetNonIndexableItemDetailsType getNonIndexableItemDetails = new GetNonIndexableItemDetailsType();
			if (pageItemReference != null)
			{
				getNonIndexableItemDetails.PageItemReference = pageItemReference;
			}
			getNonIndexableItemDetails.PageSize = ConstantProvider.SearchMailboxesPageSize;
			getNonIndexableItemDetails.PageSizeSpecified = true;
			getNonIndexableItemDetails.PageDirection = SearchPageDirectionType.Next;
			getNonIndexableItemDetails.PageDirectionSpecified = true;
			getNonIndexableItemDetails.Mailboxes = new string[]
			{
				mailboxId
			};
			List<UnsearchableItemId> itemIds = null;
			string pageReference = null;
			this.CallEws(() => this.ServiceBinding.GetNonIndexableItemDetails(getNonIndexableItemDetails), delegate(ResponseMessageType responseMessage)
			{
				GetNonIndexableItemDetailsResponseMessageType getNonIndexableItemDetailsResponseMessageType = (GetNonIndexableItemDetailsResponseMessageType)responseMessage;
				if (getNonIndexableItemDetailsResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					if (getNonIndexableItemDetailsResponseMessageType.ResponseCode == ResponseCodeType.ErrorAccessDenied)
					{
						Tracer.TraceError("EwsClient.GetNonIndexableItemDetails: ResponseCodeType.ErrorAccessDenied.", new object[0]);
						throw new ExportException(ExportErrorType.Unauthorized, getNonIndexableItemDetailsResponseMessageType.MessageText);
					}
					Tracer.TraceError("EwsClient.GetNonIndexableItemDetails: with error response message. ResponseCode : {0}", new object[]
					{
						getNonIndexableItemDetailsResponseMessageType.ResponseCode
					});
					throw new RetryException(new ExportException(ExportErrorType.FailedToGetUnsearchableItems, getNonIndexableItemDetailsResponseMessageType.ResponseCode.ToString() + " : " + getNonIndexableItemDetailsResponseMessageType.MessageText), false);
				}
				else
				{
					if (getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult != null && getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult.Items != null && getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult.Items.Length > 0)
					{
						itemIds = new List<UnsearchableItemId>(getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult.Items.Length);
						foreach (NonIndexableItemDetailType nonIndexableItemDetailType in getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult.Items)
						{
							string additionalInfo = nonIndexableItemDetailType.AdditionalInfo;
							int documentId = -1;
							if (!string.IsNullOrEmpty(additionalInfo))
							{
								int num = additionalInfo.IndexOf("4887312c-8b40-4fec-a252-f2749065c0e5");
								string s = string.Empty;
								if (num > 0)
								{
									nonIndexableItemDetailType.AdditionalInfo = additionalInfo.Substring(0, num);
									if (additionalInfo.Length > num + 1 + "4887312c-8b40-4fec-a252-f2749065c0e5".Length)
									{
										s = additionalInfo.Substring(num + "4887312c-8b40-4fec-a252-f2749065c0e5".Length);
										if (!int.TryParse(s, out documentId))
										{
											documentId = -1;
										}
									}
								}
							}
							itemIds.Add(new UnsearchableItemId
							{
								Id = nonIndexableItemDetailType.ItemId.Id,
								DocumentId = documentId,
								ErrorCode = nonIndexableItemDetailType.ErrorCode.ToString(),
								ErrorDescription = nonIndexableItemDetailType.ErrorDescription,
								LastAttemptTime = nonIndexableItemDetailType.LastAttemptTime,
								AdditionalInformation = nonIndexableItemDetailType.AdditionalInfo
							});
						}
						pageReference = getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult.Items[getNonIndexableItemDetailsResponseMessageType.NonIndexableItemDetailsResult.Items.Length - 1].SortValue;
						return;
					}
					pageReference = null;
					return;
				}
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetUnsearchableItems, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
			pageItemReference = pageReference;
			return itemIds;
		}

		public void FillInUnsearchableItemIds(string mailboxEmailAddress, IList<UnsearchableItemId> itemIds)
		{
			GetItemType getItem = new GetItemType();
			ItemResponseShapeType itemResponseShapeType = new ItemResponseShapeType();
			itemResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			PathToUnindexedFieldType pathToUnindexedFieldType = new PathToUnindexedFieldType();
			pathToUnindexedFieldType.FieldURI = UnindexedFieldURIType.itemParentFolderId;
			PathToUnindexedFieldType pathToUnindexedFieldType2 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType2.FieldURI = UnindexedFieldURIType.itemSize;
			PathToUnindexedFieldType pathToUnindexedFieldType3 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType3.FieldURI = UnindexedFieldURIType.itemSubject;
			PathToUnindexedFieldType pathToUnindexedFieldType4 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType4.FieldURI = UnindexedFieldURIType.itemDateTimeSent;
			PathToUnindexedFieldType pathToUnindexedFieldType5 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType5.FieldURI = UnindexedFieldURIType.itemDateTimeReceived;
			PathToUnindexedFieldType pathToUnindexedFieldType6 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType6.FieldURI = UnindexedFieldURIType.itemPreview;
			PathToUnindexedFieldType pathToUnindexedFieldType7 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType7.FieldURI = UnindexedFieldURIType.itemImportance;
			PathToUnindexedFieldType pathToUnindexedFieldType8 = new PathToUnindexedFieldType();
			pathToUnindexedFieldType8.FieldURI = UnindexedFieldURIType.itemHasAttachments;
			PathToExtendedFieldType senderProp = new PathToExtendedFieldType();
			senderProp.PropertyTag = "0x0C1A";
			senderProp.PropertyType = MapiPropertyTypeType.String;
			PathToExtendedFieldType messageFlagsProp = new PathToExtendedFieldType();
			messageFlagsProp.PropertyTag = "0x0E07";
			messageFlagsProp.PropertyType = MapiPropertyTypeType.Integer;
			PathToExtendedFieldType toRecipientsProp = new PathToExtendedFieldType();
			toRecipientsProp.PropertyTag = "0x0E04";
			toRecipientsProp.PropertyType = MapiPropertyTypeType.String;
			PathToExtendedFieldType ccRecipientsProp = new PathToExtendedFieldType();
			ccRecipientsProp.PropertyTag = "0x0E03";
			ccRecipientsProp.PropertyType = MapiPropertyTypeType.String;
			PathToExtendedFieldType bccRecipientsProp = new PathToExtendedFieldType();
			bccRecipientsProp.PropertyTag = "0x0E02";
			bccRecipientsProp.PropertyType = MapiPropertyTypeType.String;
			PathToExtendedFieldType internetMessageIdProp = new PathToExtendedFieldType();
			internetMessageIdProp.PropertyTag = "0x1035";
			internetMessageIdProp.PropertyType = MapiPropertyTypeType.String;
			PathToExtendedFieldType conversationTopicProp = new PathToExtendedFieldType();
			conversationTopicProp.PropertyTag = "0x0070";
			conversationTopicProp.PropertyType = MapiPropertyTypeType.String;
			PathToExtendedFieldType bodyTagProp = new PathToExtendedFieldType();
			bodyTagProp.PropertyTag = "0x3014";
			bodyTagProp.PropertyType = MapiPropertyTypeType.Binary;
			itemResponseShapeType.AdditionalProperties = new BasePathToElementType[]
			{
				pathToUnindexedFieldType,
				pathToUnindexedFieldType2,
				pathToUnindexedFieldType3,
				pathToUnindexedFieldType4,
				pathToUnindexedFieldType5,
				pathToUnindexedFieldType6,
				pathToUnindexedFieldType7,
				pathToUnindexedFieldType8,
				senderProp,
				messageFlagsProp,
				toRecipientsProp,
				ccRecipientsProp,
				bccRecipientsProp,
				internetMessageIdProp,
				conversationTopicProp,
				bodyTagProp,
				EwsClient.mDGExpansionAdditionalProperties[0],
				EwsClient.mDGExpansionAdditionalProperties[1]
			};
			getItem.ItemShape = itemResponseShapeType;
			getItem.ItemIds = (from messageId in itemIds
			select new ItemIdType
			{
				Id = messageId.Id
			}).ToArray<ItemIdType>();
			int startIndex = 0;
			this.CallEws(() => this.ServiceBinding.GetItem(getItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				ItemInfoResponseMessageType itemInfoResponseMessageType = (ItemInfoResponseMessageType)responseMessage;
				if (itemInfoResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					Tracer.TraceError("EwsClient.FillInUnsearchableItemIds: GetItem failed to get the item. Item id: {0}. ResponseCode: {1}. Response message text: {2}", new object[]
					{
						itemIds[messageIndex],
						itemInfoResponseMessageType.ResponseCode,
						itemInfoResponseMessageType.MessageText
					});
					itemIds[startIndex + messageIndex].Id = null;
				}
				else if (itemInfoResponseMessageType.ResponseClass == ResponseClassType.Warning)
				{
					Tracer.TraceWarning("EwsClient.FillInUnsearchableItemIds: Message response warning. ResponseCode:{0}; MessageText:'{1}'", new object[]
					{
						itemInfoResponseMessageType.ResponseCode,
						itemInfoResponseMessageType.MessageText
					});
					if (itemInfoResponseMessageType.ResponseCode == ResponseCodeType.ErrorBatchProcessingStopped)
					{
						Tracer.TraceWarning("EwsClient.FillInUnsearchableItemIds: Hitting ErrorBatchProcessingStopped, startIndex:{0}; messageIndex={1}", new object[]
						{
							startIndex,
							messageIndex
						});
						if (messageIndex == 0)
						{
							throw new RetryException(new ExportException(ExportErrorType.Unknown, "Internal Error: Hitting ErrorBatchProcessingStopped as the first response."), false);
						}
						startIndex += messageIndex;
						getItem.ItemIds = getItem.ItemIds.Skip(messageIndex).ToArray<BaseItemIdType>();
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetItemForUnsearchableItem, "Hitting ErrorBatchProcessingStopped"), true);
					}
				}
				else if (itemInfoResponseMessageType.Items != null && itemInfoResponseMessageType.Items.Items != null && itemInfoResponseMessageType.Items.Items.Length > 0)
				{
					ItemType itemType = itemInfoResponseMessageType.Items.Items[0];
					UnsearchableItemId unsearchableItemId = itemIds[startIndex + messageIndex];
					unsearchableItemId.ParentFolder = itemType.ParentFolderId.Id;
					unsearchableItemId.Size = (uint)itemType.Size;
					unsearchableItemId.Subject = itemType.Subject;
					unsearchableItemId.SentTime = itemType.DateTimeSent;
					unsearchableItemId.ReceivedTime = itemType.DateTimeReceived;
					unsearchableItemId.BodyPreview = itemType.Preview;
					unsearchableItemId.Importance = itemType.Importance.ToString();
					unsearchableItemId.HasAttachment = itemType.HasAttachments;
					unsearchableItemId.Sender = (((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, senderProp)) ?? string.Empty);
					string s = ((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, messageFlagsProp)) ?? "0";
					int num;
					unsearchableItemId.IsRead = (int.TryParse(s, out num) && (num & 1) != 0);
					unsearchableItemId.ToRecipients = (((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, toRecipientsProp)) ?? string.Empty);
					unsearchableItemId.CcRecipients = (((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, ccRecipientsProp)) ?? string.Empty);
					unsearchableItemId.BccRecipients = (((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, bccRecipientsProp)) ?? string.Empty);
					string text = ((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, internetMessageIdProp)) ?? string.Empty;
					string text2 = ((string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, conversationTopicProp)) ?? string.Empty;
					string text3 = (string)EwsClient.GetPropertyTagValue(itemType.ExtendedProperty, bodyTagProp);
					byte[] array = string.IsNullOrEmpty(text3) ? null : Convert.FromBase64String(text3);
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
					{
						unsearchableItemId.UniqueHash = new UniqueItemHash(text, text2, (array == null) ? null : BodyTagInfo.FromByteArray(array), false).ToString();
					}
					else
					{
						unsearchableItemId.UniqueHash = null;
					}
					if (itemType.ExtendedProperty != null)
					{
						foreach (ExtendedPropertyType extendedPropertyType in itemType.ExtendedProperty)
						{
							if (extendedPropertyType.ExtendedFieldURI != null && extendedPropertyType.Item != null)
							{
								if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyName, EwsClient.GroupExpansionRecipientsPropertyName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									string text4 = (string)extendedPropertyType.Item;
									if (text4 != null)
									{
										ItemId itemId = new ItemId();
										itemId.DGGroupExpansionError = unsearchableItemId.DGGroupExpansionError;
										this.ParseDGExpansion(text4, "EwsClient.ParseDGExpansion called from, EwsClient.FillInUnsearchableItemIds: DataCorrupted in GroupExpansionRecipients. exception: {0}", ref itemId);
										unsearchableItemId.ToGroupExpansionRecipients = itemId.ToGroupExpansionRecipients;
										unsearchableItemId.CcGroupExpansionRecipients = itemId.CcGroupExpansionRecipients;
										unsearchableItemId.BccGroupExpansionRecipients = itemId.BccGroupExpansionRecipients;
										unsearchableItemId.DGGroupExpansionError = itemId.DGGroupExpansionError;
									}
								}
								else if (string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyName, EwsClient.GroupExpansionErrorPropertyName, StringComparison.OrdinalIgnoreCase) == 0 && !string.IsNullOrEmpty((string)extendedPropertyType.Item))
								{
									try
									{
										unsearchableItemId.DGGroupExpansionError = new DistributionGroupExpansionError?((DistributionGroupExpansionError)Enum.Parse(typeof(DistributionGroupExpansionError), (string)extendedPropertyType.Item));
									}
									catch (ArgumentException)
									{
										Tracer.TraceWarning("EwsClient.FillInUnsearchableItemIds: The parsing of DistributionGroupExpansionError threw an argument exception. Input string: {0}", new object[]
										{
											extendedPropertyType.Item
										});
									}
								}
							}
						}
					}
				}
				else
				{
					Tracer.TraceWarning("EwsClient.FillInUnsearchableItemIds: GetItem got an empty item for item id: {0}", new object[]
					{
						itemIds[messageIndex]
					});
					itemIds[startIndex + messageIndex].Id = null;
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetItemForUnsearchableItem, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
		}

		public BaseFolderType GetFolderById(string mailboxEmailAddress, BaseFolderIdType folderId)
		{
			GetFolderType getFolder = new GetFolderType();
			FolderResponseShapeType folderResponseShapeType = new FolderResponseShapeType();
			folderResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			folderResponseShapeType.AdditionalProperties = new BasePathToElementType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.folderDisplayName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.folderParentFolderId
				}
			};
			getFolder.FolderShape = folderResponseShapeType;
			getFolder.FolderIds = new BaseFolderIdType[]
			{
				folderId
			};
			List<BaseFolderType> folders = new List<BaseFolderType>(1);
			this.CallEws(() => this.ServiceBinding.GetFolder(getFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					if (responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
					{
						folders.Clear();
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetFolderById, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
				}
				else
				{
					FolderInfoResponseMessageType folderInfoResponseMessageType = (FolderInfoResponseMessageType)responseMessage;
					folders.Add(folderInfoResponseMessageType.Folders[0]);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetFolderById, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			if (folders.Count <= 0)
			{
				return null;
			}
			return folders[0];
		}

		public BaseFolderType GetFolderByName(string mailboxEmailAddress, BaseFolderIdType parentFolderId, string folderDisplayName)
		{
			FindFolderType findFolder = new FindFolderType();
			FolderResponseShapeType folderResponseShapeType = new FolderResponseShapeType();
			folderResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			folderResponseShapeType.AdditionalProperties = new BasePathToElementType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.folderDisplayName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.folderParentFolderId
				}
			};
			findFolder.FolderShape = folderResponseShapeType;
			findFolder.Restriction = new RestrictionType
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
							Value = folderDisplayName
						}
					}
				}
			};
			findFolder.ParentFolderIds = new BaseFolderIdType[]
			{
				parentFolderId
			};
			List<BaseFolderType> folders = new List<BaseFolderType>(1);
			this.CallEws(() => this.ServiceBinding.FindFolder(findFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					if (responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
					{
						folders.Clear();
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetFolderByName, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
				}
				else
				{
					FindFolderResponseMessageType findFolderResponseMessageType = (FindFolderResponseMessageType)responseMessage;
					folders.AddRange(findFolderResponseMessageType.RootFolder.Folders);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetFolderByName, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			if (folders.Count <= 0)
			{
				return null;
			}
			return folders[0];
		}

		public List<BaseFolderType> CreateFolder(string mailboxEmailAddress, BaseFolderIdType parentFolderId, BaseFolderType[] folders)
		{
			CreateFolderType createFolder = new CreateFolderType();
			createFolder.Folders = folders;
			createFolder.ParentFolderId = new TargetFolderIdType
			{
				Item = parentFolderId
			};
			List<BaseFolderType> newFolders = new List<BaseFolderType>(folders.Length);
			this.CallEws(() => this.ServiceBinding.CreateFolder(createFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					throw new ExportException(ExportErrorType.FailedToCreateFolder, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText);
				}
				FolderInfoResponseMessageType folderInfoResponseMessageType = (FolderInfoResponseMessageType)responseMessage;
				newFolders.AddRange(folderInfoResponseMessageType.Folders);
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToCreateFolder, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return newFolders;
		}

		public void DeleteFolder(string mailboxEmailAddress, BaseFolderIdType[] folderIds)
		{
			DeleteFolderType deleteFolder = new DeleteFolderType();
			deleteFolder.FolderIds = folderIds;
			deleteFolder.DeleteType = DisposalType.HardDelete;
			this.CallEws(() => this.ServiceBinding.DeleteFolder(deleteFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error && responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
				{
					throw new RetryException(new ExportException(ExportErrorType.FailedToDeleteFolder, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToDeleteFolder, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
		}

		public void MoveFolder(string mailboxEmailAddress, BaseFolderIdType targetFolderId, BaseFolderIdType[] folderIds)
		{
			MoveFolderType moveFolder = new MoveFolderType();
			moveFolder.FolderIds = folderIds;
			moveFolder.ToFolderId = new TargetFolderIdType
			{
				Item = targetFolderId
			};
			this.CallEws(() => this.ServiceBinding.MoveFolder(moveFolder), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					throw new RetryException(new ExportException(ExportErrorType.FailedToMoveFolder, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToMoveFolder, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
		}

		public List<ItemType> RetrieveItems(string mailboxEmailAddress, BaseFolderIdType parentFolderId, BasePathToElementType[] additionalProperties, RestrictionType restriction, bool isAssociated, int? pageItemCount, int offset)
		{
			FindItemType findItem = new FindItemType();
			ItemResponseShapeType itemResponseShapeType = new ItemResponseShapeType();
			itemResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			itemResponseShapeType.AdditionalProperties = additionalProperties;
			findItem.ItemShape = itemResponseShapeType;
			if (restriction != null)
			{
				findItem.Restriction = restriction;
			}
			if (isAssociated)
			{
				findItem.Traversal = ItemQueryTraversalType.Associated;
			}
			IndexedPageViewType indexedPageViewType = new IndexedPageViewType
			{
				BasePoint = IndexBasePointType.Beginning,
				Offset = offset
			};
			if (pageItemCount != null && pageItemCount != null)
			{
				indexedPageViewType.MaxEntriesReturned = pageItemCount.Value;
				indexedPageViewType.MaxEntriesReturnedSpecified = true;
			}
			findItem.Item = indexedPageViewType;
			findItem.ParentFolderIds = new BaseFolderIdType[]
			{
				parentFolderId
			};
			List<ItemType> itemsList = new List<ItemType>();
			this.CallEws(() => this.ServiceBinding.FindItem(findItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					if (responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
					{
						itemsList.Clear();
						throw new RetryException(new ExportException(ExportErrorType.FailedToRetrieveItems, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
				}
				else
				{
					FindItemResponseMessageType findItemResponseMessageType = (FindItemResponseMessageType)responseMessage;
					if (findItemResponseMessageType.RootFolder != null && findItemResponseMessageType.RootFolder.Item != null)
					{
						ArrayOfRealItemsType arrayOfRealItemsType = findItemResponseMessageType.RootFolder.Item as ArrayOfRealItemsType;
						if (arrayOfRealItemsType != null && arrayOfRealItemsType.Items != null)
						{
							itemsList.AddRange(arrayOfRealItemsType.Items);
						}
					}
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToRetrieveItems, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return itemsList;
		}

		public List<ItemType> CreateItems(string mailboxEmailAddress, BaseFolderIdType parentFolderId, ItemType[] items)
		{
			CreateItemType createItem = new CreateItemType();
			List<ItemType> itemsToSubmit = new List<ItemType>(items);
			createItem.SavedItemFolderId = new TargetFolderIdType
			{
				Item = parentFolderId
			};
			createItem.MessageDisposition = MessageDispositionType.SaveOnly;
			createItem.MessageDispositionSpecified = true;
			createItem.Items = new NonEmptyArrayOfAllItemsType();
			List<ItemType> itemsList = new List<ItemType>();
			this.CallEws(delegate()
			{
				itemsToSubmit.RemoveAll((ItemType t) => t == null);
				createItem.Items.Items = itemsToSubmit.ToArray();
				return this.ServiceBinding.CreateItem(createItem);
			}, delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass != ResponseClassType.Error)
				{
					ItemInfoResponseMessageType itemInfoResponseMessageType = (ItemInfoResponseMessageType)responseMessage;
					ArrayOfRealItemsType items2 = itemInfoResponseMessageType.Items;
					if (items2 != null && items2.Items != null)
					{
						if (itemsToSubmit.Count > messageIndex)
						{
							itemsToSubmit[messageIndex] = null;
						}
						itemsList.AddRange(items2.Items);
					}
					return true;
				}
				if (responseMessage.ResponseCode == ResponseCodeType.ErrorQuotaExceeded)
				{
					throw new ExportException(ExportErrorType.TargetOutOfSpace, responseMessage.ResponseCode.ToString());
				}
				throw new ExportException(ExportErrorType.FailedToCreateItems, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText);
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToCreateItems, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return itemsList;
		}

		public List<ItemType> UpdateItems(string mailboxEmailAddress, BaseFolderIdType parentFolderId, ItemChangeType[] itemChanges)
		{
			UpdateItemType updateItem = new UpdateItemType();
			updateItem.SavedItemFolderId = new TargetFolderIdType
			{
				Item = parentFolderId
			};
			updateItem.MessageDisposition = MessageDispositionType.SaveOnly;
			updateItem.MessageDispositionSpecified = true;
			updateItem.ConflictResolution = ConflictResolutionType.AutoResolve;
			updateItem.ItemChanges = itemChanges;
			List<ItemType> itemsList = new List<ItemType>();
			this.CallEws(() => this.ServiceBinding.UpdateItem(updateItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass != ResponseClassType.Error)
				{
					UpdateItemResponseMessageType updateItemResponseMessageType = (UpdateItemResponseMessageType)responseMessage;
					ArrayOfRealItemsType items = updateItemResponseMessageType.Items;
					if (items != null && items.Items != null)
					{
						itemsList.AddRange(items.Items);
					}
					return true;
				}
				if (responseMessage.ResponseCode == ResponseCodeType.ErrorQuotaExceeded)
				{
					throw new ExportException(ExportErrorType.TargetOutOfSpace, string.Format("EDiscoveryError:E005::Mailbox:{0}:: {1}", mailboxEmailAddress, responseMessage.ResponseCode.ToString()));
				}
				throw new ExportException(ExportErrorType.FailedToUpdateItems, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText);
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToUpdateItems, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return itemsList;
		}

		public void DeleteItems(string mailboxEmailAddress, BaseItemIdType[] itemIds)
		{
			DeleteItemType deleteItem = new DeleteItemType();
			deleteItem.DeleteType = DisposalType.HardDelete;
			deleteItem.ItemIds = itemIds;
			this.CallEws(() => this.ServiceBinding.DeleteItem(deleteItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error && responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
				{
					throw new RetryException(new ExportException(ExportErrorType.FailedToDeleteItems, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToDeleteItems, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
		}

		public List<ItemInformation> UploadItems(string mailboxEmailAddress, FolderIdType parentFolderId, IList<ItemInformation> items, bool alwaysCreateNew)
		{
			UploadItemsType uploadItems = new UploadItemsType();
			uploadItems.Items = (from item in items
			select new UploadItemType
			{
				CreateAction = (alwaysCreateNew ? CreateActionType.CreateNew : CreateActionType.UpdateOrCreate),
				ItemId = new ItemIdType
				{
					Id = item.Id.Id
				},
				Data = item.Data,
				ParentFolderId = parentFolderId
			}).ToArray<UploadItemType>();
			List<ItemInformation> returnedItems = new List<ItemInformation>(uploadItems.Items.Length);
			this.CallEws(() => this.ServiceBinding.UploadItems(uploadItems), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				UploadItemsResponseMessageType uploadItemsResponseMessageType = (UploadItemsResponseMessageType)responseMessage;
				if (uploadItemsResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					Tracer.TraceError("UploadItems with error response message. ResponseCode : {0}", new object[]
					{
						uploadItemsResponseMessageType.ResponseCode
					});
					if (uploadItemsResponseMessageType.ResponseCode == ResponseCodeType.ErrorQuotaExceeded)
					{
						throw new ExportException(ExportErrorType.TargetOutOfSpace, uploadItemsResponseMessageType.ResponseCode.ToString());
					}
					returnedItems.Add(new ItemInformation
					{
						Id = items[messageIndex].Id,
						Data = items[messageIndex].Data,
						Error = new ExportException(ExportErrorType.FailedToUploadItems, uploadItemsResponseMessageType.ResponseCode.ToString())
					});
				}
				else
				{
					returnedItems.Add(new ItemInformation
					{
						Id = items[messageIndex].Id,
						Data = items[messageIndex].Data,
						Error = null
					});
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToUploadItems, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return returnedItems;
		}

		public List<SourceInformation.SourceConfiguration> RetrieveSearchConfiguration(string searchName, out string language, string mailboxEmailAddress = null)
		{
			language = null;
			string queryLanguage = null;
			GetDiscoverySearchConfigurationType getDiscoverySearchConfigurationType = new GetDiscoverySearchConfigurationType();
			getDiscoverySearchConfigurationType.SearchId = searchName;
			getDiscoverySearchConfigurationType.ExpandGroupMembership = true;
			getDiscoverySearchConfigurationType.ExpandGroupMembershipSpecified = true;
			List<SourceInformation.SourceConfiguration> sources = null;
			if (!this.TryRetrieveConfigurationWithNewSchema(searchName, out sources, out language, mailboxEmailAddress))
			{
				this.CallEws(() => this.ServiceBinding.GetDiscoverySearchConfiguration(getDiscoverySearchConfigurationType), delegate(ResponseMessageType responseMessage)
				{
					GetDiscoverySearchConfigurationResponseMessageType getDiscoverySearchConfigurationResponseMessageType = (GetDiscoverySearchConfigurationResponseMessageType)responseMessage;
					if (getDiscoverySearchConfigurationResponseMessageType.ResponseClass == ResponseClassType.Error)
					{
						if (getDiscoverySearchConfigurationResponseMessageType.ResponseCode == ResponseCodeType.ErrorAccessDenied)
						{
							Tracer.TraceError("EwsClient.RetrieveSearchConfiguration: ResponseCodeType.ErrorAccessDenied.", new object[0]);
							throw new ExportException(ExportErrorType.Unauthorized, getDiscoverySearchConfigurationResponseMessageType.MessageText);
						}
						Tracer.TraceError("EwsClient.RetrieveSearchConfiguration: Error response message. ResponseCode : {0}", new object[]
						{
							getDiscoverySearchConfigurationResponseMessageType.ResponseCode
						});
						throw new RetryException(new ExportException(ExportErrorType.FailedToRetrieveSearchConfiguration, getDiscoverySearchConfigurationResponseMessageType.ResponseCode.ToString() + " : " + getDiscoverySearchConfigurationResponseMessageType.MessageText), false);
					}
					else
					{
						if (getDiscoverySearchConfigurationResponseMessageType.DiscoverySearchConfigurations == null || getDiscoverySearchConfigurationResponseMessageType.DiscoverySearchConfigurations.Length == 0)
						{
							throw new RetryException(new ExportException(ExportErrorType.FailedToRetrieveSearchConfiguration), false);
						}
						DiscoverySearchConfigurationType discoverySearchConfigurationType = getDiscoverySearchConfigurationResponseMessageType.DiscoverySearchConfigurations[0];
						if (discoverySearchConfigurationType.SearchableMailboxes == null || discoverySearchConfigurationType.SearchableMailboxes.Length == 0)
						{
							throw new RetryException(new ExportException(ExportErrorType.FailedToRetrieveSearchConfigurationNoSearchableMailboxesFound), false);
						}
						queryLanguage = discoverySearchConfigurationType.Language;
						sources = new List<SourceInformation.SourceConfiguration>(discoverySearchConfigurationType.SearchableMailboxes.Length);
						foreach (SearchableMailboxType searchableMailboxType in discoverySearchConfigurationType.SearchableMailboxes)
						{
							sources.Add(new SourceInformation.SourceConfiguration(searchableMailboxType.DisplayName, searchableMailboxType.PrimarySmtpAddress, discoverySearchConfigurationType.SearchQuery, null, searchableMailboxType.ReferenceId));
						}
						return;
					}
				}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToRetrieveSearchConfiguration, exception), ExchangeVersionType.Exchange2013, null);
			}
			language = queryLanguage;
			return sources;
		}

		public ItemType GetItem(string mailboxEmailAddress, string itemId)
		{
			GetItemType getItem = new GetItemType();
			ItemResponseShapeType itemResponseShapeType = new ItemResponseShapeType();
			itemResponseShapeType.BaseShape = DefaultShapeNamesType.Default;
			getItem.ItemShape = itemResponseShapeType;
			getItem.ItemIds = new ItemIdType[]
			{
				new ItemIdType
				{
					Id = itemId
				}
			};
			ItemType returnedItem = null;
			this.CallEws(() => this.ServiceBinding.GetItem(getItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				ItemInfoResponseMessageType itemInfoResponseMessageType = (ItemInfoResponseMessageType)responseMessage;
				if (itemInfoResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					if (responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
					{
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetItem, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
					Tracer.TraceError("EwsClient.GetItem: GetItem failed to find the item. Message id: {0}", new object[]
					{
						itemId
					});
				}
				else if (itemInfoResponseMessageType.Items != null && itemInfoResponseMessageType.Items.Items != null && itemInfoResponseMessageType.Items.Items.Length > 0)
				{
					returnedItem = itemInfoResponseMessageType.Items.Items[0];
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetItem, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return returnedItem;
		}

		public void UpdateDGExpansion(string mailboxEmailAddress, IList<ItemId> itemIds)
		{
			GetItemType getItem = new GetItemType();
			ItemResponseShapeType itemResponseShapeType = new ItemResponseShapeType();
			itemResponseShapeType.BaseShape = DefaultShapeNamesType.IdOnly;
			itemResponseShapeType.AdditionalProperties = EwsClient.mDGExpansionAdditionalProperties;
			getItem.ItemShape = itemResponseShapeType;
			getItem.ItemIds = (from messageId in itemIds
			select new ItemIdType
			{
				Id = messageId.Id
			}).ToArray<ItemIdType>();
			int startIndex = 0;
			ItemType returnedItem = null;
			this.CallEws(() => this.ServiceBinding.GetItem(getItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				ItemInfoResponseMessageType itemInfoResponseMessageType = (ItemInfoResponseMessageType)responseMessage;
				if (itemInfoResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					if (responseMessage.ResponseCode != ResponseCodeType.ErrorItemNotFound)
					{
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetItem, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
					Tracer.TraceError("EwsClient.UpdateDGExpansion: GetItem failed to find the item. Message id: {0}", new object[]
					{
						itemIds[startIndex + messageIndex].Id
					});
				}
				else if (itemInfoResponseMessageType.Items != null && itemInfoResponseMessageType.Items.Items != null && itemInfoResponseMessageType.Items.Items.Length > 0)
				{
					returnedItem = itemInfoResponseMessageType.Items.Items[0];
					if (returnedItem.ExtendedProperty != null)
					{
						foreach (ExtendedPropertyType extendedPropertyType in returnedItem.ExtendedProperty)
						{
							if (extendedPropertyType.ExtendedFieldURI != null && extendedPropertyType.Item != null && string.Compare(extendedPropertyType.ExtendedFieldURI.PropertyName, EwsClient.GroupExpansionRecipientsPropertyName, StringComparison.OrdinalIgnoreCase) == 0)
							{
								string text = (string)extendedPropertyType.Item;
								if (text != null)
								{
									ItemId itemId = itemIds[startIndex + messageIndex];
									this.ParseDGExpansion(text, "EwsClient.ParseDGExpansion called from, EwsClient.UpdateDGExpansion: DataCorrupted in GroupExpansionRecipients. exception: {0}", ref itemId);
								}
							}
						}
					}
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetItem, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
		}

		public void SendEmails(string mailboxEmailAddress, MessageType[] emails)
		{
			CreateItemType createItem = new CreateItemType();
			createItem.MessageDisposition = MessageDispositionType.SendOnly;
			createItem.MessageDispositionSpecified = true;
			createItem.Items = new NonEmptyArrayOfAllItemsType
			{
				Items = emails
			};
			this.CallEws(() => this.ServiceBinding.CreateItem(createItem), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error)
				{
					throw new RetryException(new ExportException(ExportErrorType.FailedToCreateItems, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToCreateItems, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
		}

		public AttachmentType GetAttachment(string mailboxEmailAddress, string attachmentId)
		{
			GetAttachmentType getAttachment = new GetAttachmentType();
			getAttachment.AttachmentIds = new AttachmentIdType[]
			{
				new AttachmentIdType
				{
					Id = attachmentId
				}
			};
			AttachmentType returnedAttachment = null;
			this.CallEws(() => this.ServiceBinding.GetAttachment(getAttachment), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				AttachmentInfoResponseMessageType attachmentInfoResponseMessageType = (AttachmentInfoResponseMessageType)responseMessage;
				if (attachmentInfoResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					if (responseMessage.ResponseCode != ResponseCodeType.ErrorInvalidAttachmentId)
					{
						throw new RetryException(new ExportException(ExportErrorType.FailedToGetAttachment, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
					}
					Tracer.TraceError("EwsClient.GetAttachment: GetAttachment failed to find the attachment. Attachment id: {0}", new object[]
					{
						attachmentId
					});
				}
				else if (attachmentInfoResponseMessageType.Attachments != null && attachmentInfoResponseMessageType.Attachments.Count<AttachmentType>() > 0)
				{
					returnedAttachment = attachmentInfoResponseMessageType.Attachments[0];
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToGetAttachment, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return returnedAttachment;
		}

		public List<AttachmentType> CreateAttachments(string mailboxEmailAddress, string itemId, IList<AttachmentType> attachments)
		{
			CreateAttachmentType createAttachment = new CreateAttachmentType();
			createAttachment.ParentItemId = new ItemIdType
			{
				Id = itemId
			};
			createAttachment.Attachments = attachments.ToArray<AttachmentType>();
			List<AttachmentType> attachmentsList = new List<AttachmentType>();
			this.CallEws(() => this.ServiceBinding.CreateAttachment(createAttachment), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass != ResponseClassType.Error)
				{
					AttachmentInfoResponseMessageType attachmentInfoResponseMessageType = (AttachmentInfoResponseMessageType)responseMessage;
					if (attachmentInfoResponseMessageType.Attachments != null && attachmentInfoResponseMessageType.Attachments.Length > 0)
					{
						attachmentsList.AddRange(attachmentInfoResponseMessageType.Attachments);
					}
					return true;
				}
				if (responseMessage.ResponseCode == ResponseCodeType.ErrorQuotaExceeded)
				{
					throw new ExportException(ExportErrorType.TargetOutOfSpace, responseMessage.ResponseCode.ToString());
				}
				throw new ExportException(ExportErrorType.FailedToCreateAttachments, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText);
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToCreateAttachments, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
			return attachmentsList;
		}

		public void DeleteAttachments(string mailboxEmailAddress, IList<string> attachmentIds)
		{
			DeleteAttachmentType deleteAttachment = new DeleteAttachmentType();
			deleteAttachment.AttachmentIds = (from attachmentId in attachmentIds
			select new AttachmentIdType
			{
				Id = attachmentId
			}).ToArray<AttachmentIdType>();
			this.CallEws(() => this.ServiceBinding.DeleteAttachment(deleteAttachment), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Error && responseMessage.ResponseCode != ResponseCodeType.ErrorInvalidAttachmentId)
				{
					throw new RetryException(new ExportException(ExportErrorType.FailedToDeleteAttachments, responseMessage.ResponseCode.ToString() + " : " + responseMessage.MessageText), false);
				}
				return true;
			}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToDeleteAttachments, exception), ExchangeVersionType.Exchange2010_SP2, mailboxEmailAddress);
		}

		public string GetPhysicalPartitionIdentifier(ItemId itemId)
		{
			return EwsClient.InternalGetPhysicalPartitionIdentifier(itemId);
		}

		public object GetItemHashObject(string itemHash)
		{
			try
			{
				return UniqueItemHash.Parse(itemHash);
			}
			catch (Exception ex)
			{
				Tracer.TraceError("EwsClient.GetItemHashObject: UniqueItemHash.Parse on '{0}' throw exception: {1}", new object[]
				{
					itemHash,
					ex
				});
			}
			return null;
		}

		internal static string InternalGetPhysicalPartitionIdentifier(ItemId itemId)
		{
			string result;
			try
			{
				byte[] array = Convert.FromBase64String(itemId.Id);
				result = Encoding.UTF8.GetString(array, 4, (int)BitConverter.ToInt16(array, 2));
			}
			catch (ArgumentException ex)
			{
				Tracer.TraceError("EwsClient.GetPhysicalPartitionIdentifier: ItemId.Id: {0}; Exception: {1}", new object[]
				{
					itemId.Id,
					ex
				});
				result = null;
			}
			catch (FormatException ex2)
			{
				Tracer.TraceError("EwsClient.GetPhysicalPartitionIdentifier: ItemId.Id: {0}; Exception: {1}", new object[]
				{
					itemId.Id,
					ex2
				});
				result = null;
			}
			return result;
		}

		private static object GetPropertyTagValue(ExtendedPropertyType[] extendedPropertyValues, PathToExtendedFieldType property)
		{
			int num = Convert.ToInt32(property.PropertyTag, 16);
			foreach (ExtendedPropertyType extendedPropertyType in extendedPropertyValues)
			{
				try
				{
					int num2 = Convert.ToInt32(extendedPropertyType.ExtendedFieldURI.PropertyTag, 16);
					if (num2 == num)
					{
						return extendedPropertyType.Item;
					}
				}
				catch (ArgumentException)
				{
					Tracer.TraceError("EwsClient.GetPropertyTagValue: ArgumentException caught and ignored.", new object[0]);
				}
				catch (FormatException)
				{
					Tracer.TraceError("EwsClient.GetPropertyTagValue: FormatException caught and ignored.", new object[0]);
				}
				catch (OverflowException)
				{
					Tracer.TraceError("EwsClient.GetPropertyTagValue: OverflowException caught and ignored.", new object[0]);
				}
			}
			return null;
		}

		private ExportException WrapExportException(ExportErrorType errorType, Exception exception)
		{
			ExportException ex = exception as ExportException;
			if (ex == null || ex.ErrorType != errorType)
			{
				ex = new ExportException(errorType, exception);
			}
			ex.ServiceEndpoint = base.ServiceEndpoint;
			ex.ServiceHttpContext = base.ServiceBinding.HttpContext;
			ex.ScenarioData = base.ServiceBinding.UserAgent;
			return ex;
		}

		private void CallEws(Func<BaseResponseMessageType> delegateEwsCall, Func<ResponseMessageType, int, bool> responseMessageProcessor, Func<Exception, Exception> exceptionHandler, ExchangeVersionType requestServerVersion, string mailboxEmailAddress)
		{
			if (this.Connect())
			{
				base.ServiceBinding.RequestServerVersionValue = new RequestServerVersion
				{
					Version = requestServerVersion
				};
				base.ServiceCallingContext.SetServiceApiContext(base.ServiceBinding, mailboxEmailAddress);
				base.CurrentMailbox = mailboxEmailAddress;
				try
				{
					base.InternalCallService<BaseResponseMessageType>(delegateEwsCall, delegate(BaseResponseMessageType response)
					{
						List<ResponseMessageType> list = new List<ResponseMessageType>(response.ResponseMessages.Items);
						foreach (ResponseMessageType responseMessageType in from t in response.ResponseMessages.Items
						orderby (int)t.ResponseClass
						select t)
						{
							int arg = list.IndexOf(responseMessageType);
							if (responseMessageType.ResponseClass == ResponseClassType.Error)
							{
								Tracer.TraceError("EwsClient.CallEws: Message response error. ResponseCode:{0}; MessageText:'{1}'", new object[]
								{
									responseMessageType.ResponseCode,
									responseMessageType.MessageText
								});
								if (responseMessageType.ResponseCode == ResponseCodeType.ErrorInternalServerTransientError || responseMessageType.ResponseCode == ResponseCodeType.ErrorMailboxStoreUnavailable || responseMessageType.ResponseCode == ResponseCodeType.ErrorServerBusy || responseMessageType.ResponseCode == ResponseCodeType.ErrorTimeoutExpired)
								{
									Tracer.TraceError("EwsClient.CallEws: Transient exception causing retry. ResponseCode: {0}.", new object[]
									{
										responseMessageType.ResponseCode
									});
									throw new RetryException(new ExportException(ExportErrorType.ExchangeWebServiceCallFailed, (responseMessageType.ResponseCode != ResponseCodeType.ErrorMailboxStoreUnavailable) ? responseMessageType.MessageText : string.Format("The mailbox database is temporarily unavailable. {0}", responseMessageType.MessageText)), false);
								}
								if (responseMessageProcessor == null)
								{
									Tracer.TraceError("EwsClient.CallEws: Error response message received and responseMessageProcessor is null.", new object[0]);
									throw new ExportException(ExportErrorType.ExchangeWebServiceCallFailed, responseMessageType.MessageText);
								}
							}
							try
							{
								if (responseMessageProcessor != null && !responseMessageProcessor(responseMessageType, arg))
								{
									break;
								}
							}
							catch (RetryException ex)
							{
								if (BaseServiceClient<ExchangeServiceBinding, ISourceDataProvider>.IsRebindableError(responseMessageType.ResponseCode) && this.Rebind())
								{
									throw;
								}
								if (BaseServiceClient<ExchangeServiceBinding, ISourceDataProvider>.IsTransientError(responseMessageType.ResponseCode))
								{
									throw;
								}
								throw ex.InnerException;
							}
						}
					}, exceptionHandler, () => base.ServiceCallingContext.AuthorizeServiceBinding(base.ServiceBinding), delegate(Uri redirectedUrl)
					{
						base.ServiceCallingContext.SetServiceUrlAffinity(base.ServiceBinding, redirectedUrl);
						base.ServiceCallingContext.SetServiceUrl(base.ServiceBinding, redirectedUrl);
					});
					return;
				}
				catch (ExportException)
				{
					Tracer.TraceError("EwsClient.CallEws: {0}", new object[]
					{
						base.ServiceBinding.HttpContext
					});
					throw;
				}
			}
			throw new ExportException(ExportErrorType.ExchangeWebServiceCallFailed, "Unable to connect to Exchange web service at: " + base.ServiceEndpoint.ToString());
		}

		private void CallEws(Func<ResponseMessageType> delegateEwsCall, Action<ResponseMessageType> responseMessageProcessor, Func<Exception, Exception> exceptionHandler, ExchangeVersionType requestServerVersion, string mailboxEmailAddress)
		{
			this.CallEws(() => new BaseResponseMessageType
			{
				ResponseMessages = new ArrayOfResponseMessagesType
				{
					Items = new ResponseMessageType[]
					{
						delegateEwsCall()
					}
				}
			}, delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				responseMessageProcessor(responseMessage);
				return false;
			}, exceptionHandler, requestServerVersion, mailboxEmailAddress);
		}

		private bool TryRetrieveConfigurationWithNewSchema(string configuration, out List<SourceInformation.SourceConfiguration> sources, out string language, string mailboxEmailAddress = null)
		{
			sources = null;
			language = null;
			string currentLanguage = null;
			List<SourceInformation.SourceConfiguration> currentSources = new List<SourceInformation.SourceConfiguration>();
			try
			{
				SearchMailboxesType searchMailboxesType = new SearchMailboxesType();
				searchMailboxesType.PageSize = 0;
				searchMailboxesType.PageSizeSpecified = true;
				searchMailboxesType.ResultType = SearchResultType.PreviewOnly;
				searchMailboxesType.SearchQueries = new MailboxQueryType[]
				{
					new MailboxQueryType
					{
						Query = string.Empty,
						MailboxSearchScopes = new MailboxSearchScopeType[]
						{
							new MailboxSearchScopeType
							{
								Mailbox = configuration,
								SearchScope = MailboxSearchLocationType.All,
								ExtendedAttributes = new ExtendedAttributeType[]
								{
									new ExtendedAttributeType
									{
										Name = "SearchScopeType",
										Value = "SavedSearchId"
									},
									new ExtendedAttributeType
									{
										Name = "SearchType",
										Value = "ExpandSources"
									}
								}
							}
						}
					}
				};
				this.CallEws(() => this.ServiceBinding.SearchMailboxes(searchMailboxesType), delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					SearchMailboxesResponseMessageType searchMailboxesResponseMessageType = (SearchMailboxesResponseMessageType)responseMessage;
					if (searchMailboxesResponseMessageType.ResponseClass == ResponseClassType.Error)
					{
						throw new Exception();
					}
					if (searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes != null)
					{
						if (searchMailboxesResponseMessageType.SearchMailboxesResult.FailedMailboxes.Any((FailedSearchMailboxType t) => t.Mailbox == configuration))
						{
							throw new Exception();
						}
					}
					MailboxQueryType mailboxQueryType = searchMailboxesResponseMessageType.SearchMailboxesResult.SearchQueries[0];
					MailboxStatisticsItemType[] mailboxStats = searchMailboxesResponseMessageType.SearchMailboxesResult.MailboxStats;
					SearchPreviewItemType[] items = searchMailboxesResponseMessageType.SearchMailboxesResult.Items;
					if (mailboxStats != null && mailboxStats.Length > 0 && mailboxQueryType != null)
					{
						MailboxStatisticsItemType[] array = mailboxStats;
						for (int i = 0; i < array.Length; i++)
						{
							MailboxStatisticsItemType mailbox = array[i];
							SearchPreviewItemType searchPreviewItemType = items.FirstOrDefault((SearchPreviewItemType t) => t.Mailbox.MailboxId == mailbox.MailboxId);
							if (searchPreviewItemType != null)
							{
								currentSources.Add(new SourceInformation.SourceConfiguration(mailbox.DisplayName, searchPreviewItemType.Mailbox.PrimarySmtpAddress, mailboxQueryType.Query, null, mailbox.MailboxId)
								{
									SearchName = configuration
								});
								Uri uri;
								if (!string.IsNullOrEmpty(searchPreviewItemType.OwaLink) && Uri.TryCreate(searchPreviewItemType.OwaLink, UriKind.Absolute, out uri) && !string.IsNullOrEmpty(uri.Query))
								{
									NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
									string[] values = nameValueCollection.GetValues("Lang");
									if (values != null && values.Length > 0)
									{
										currentLanguage = values[0];
									}
								}
							}
						}
					}
					return false;
				}, (Exception exception) => this.WrapExportException(ExportErrorType.FailedToSearchMailboxes, exception), ExchangeVersionType.Exchange2013, mailboxEmailAddress);
				if (currentSources.Count > 0)
				{
					language = currentLanguage;
					sources = currentSources;
				}
				return true;
			}
			catch
			{
			}
			return false;
		}

		private bool SearchMailboxesWithNewSchema(ref SearchMailboxesType searchMailboxesType, out SearchMailboxesResponseMessageType responseType, ExtendedAttributeType unsearchableExtendedAttribute, string searchName = null, string mailboxEmailAddress = null)
		{
			responseType = null;
			SearchMailboxesResponseMessageType currentResponseType = null;
			SearchMailboxesType currentRequestType = searchMailboxesType;
			MailboxSearchScopeType[] mailboxSearchScopes = currentRequestType.SearchQueries[0].MailboxSearchScopes;
			List<MailboxSearchScopeType> list = new List<MailboxSearchScopeType>();
			if (!string.IsNullOrEmpty(searchName))
			{
				if (unsearchableExtendedAttribute == null)
				{
					list.Add(new MailboxSearchScopeType
					{
						Mailbox = searchName,
						ExtendedAttributes = new ExtendedAttributeType[]
						{
							new ExtendedAttributeType
							{
								Name = "SearchScopeType",
								Value = "SavedSearchId"
							}
						}
					});
				}
				else
				{
					list.Add(new MailboxSearchScopeType
					{
						Mailbox = searchName,
						SearchScope = MailboxSearchLocationType.All,
						ExtendedAttributes = new ExtendedAttributeType[]
						{
							new ExtendedAttributeType
							{
								Name = "SearchScopeType",
								Value = "SavedSearchId"
							},
							unsearchableExtendedAttribute
						}
					});
				}
			}
			foreach (MailboxSearchScopeType mailboxSearchScopeType in currentRequestType.SearchQueries[0].MailboxSearchScopes)
			{
				MailboxSearchScopeType mailboxSearchScopeType2 = new MailboxSearchScopeType
				{
					Mailbox = mailboxSearchScopeType.Mailbox,
					SearchScope = mailboxSearchScopeType.SearchScope
				};
				if (mailboxSearchScopeType2.Mailbox.StartsWith("\\"))
				{
					mailboxSearchScopeType2.ExtendedAttributes = new ExtendedAttributeType[]
					{
						new ExtendedAttributeType
						{
							Name = "SearchScopeType",
							Value = "AutoDetect"
						}
					};
				}
				list.Add(mailboxSearchScopeType2);
			}
			currentRequestType.SearchQueries[0].MailboxSearchScopes = list.ToArray();
			this.CallEws(() => this.ServiceBinding.SearchMailboxes(currentRequestType), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				SearchMailboxesResponseMessageType searchMailboxesResponseMessageType = (SearchMailboxesResponseMessageType)responseMessage;
				if (searchMailboxesResponseMessageType.ResponseClass == ResponseClassType.Error)
				{
					throw new RetryException(new ExportException(ExportErrorType.FailedToSearchMailboxes, searchMailboxesResponseMessageType.ResponseCode.ToString() + " : " + searchMailboxesResponseMessageType.MessageText), false);
				}
				currentResponseType = searchMailboxesResponseMessageType;
				return false;
			}, delegate(Exception exception)
			{
				throw this.WrapExportException(ExportErrorType.FailedToSearchMailboxes, exception);
			}, ExchangeVersionType.Exchange2013, mailboxEmailAddress);
			if (currentResponseType != null)
			{
				responseType = currentResponseType;
				return true;
			}
			return false;
		}

		private void ParseDGExpansion(string value, string traceFormat, ref ItemId itemId)
		{
			try
			{
				GroupExpansionRecipients groupExpansionRecipients = GroupExpansionRecipients.Parse(value);
				itemId.ToGroupExpansionRecipients = groupExpansionRecipients.ToRecipients;
				itemId.CcGroupExpansionRecipients = groupExpansionRecipients.CcRecipients;
				itemId.BccGroupExpansionRecipients = groupExpansionRecipients.BccRecipients;
			}
			catch (Exception ex)
			{
				Tracer.TraceError(string.Format(traceFormat, ex.Message), new object[0]);
				return;
			}
			if (itemId.DGGroupExpansionError == null)
			{
				itemId.DGGroupExpansionError = new DistributionGroupExpansionError?(DistributionGroupExpansionError.NoError);
			}
		}

		private static readonly string GroupExpansionErrorPropertyName = "GroupExpansionError";

		private static readonly string GroupExpansionRecipientsPropertyName = "GroupExpansionRecipients";

		private static PathToExtendedFieldType[] mDGExpansionAdditionalProperties = new PathToExtendedFieldType[]
		{
			new PathToExtendedFieldType
			{
				PropertySetId = "403FC56B-CD30-47C5-86F8-EDE9E35A022B",
				PropertyName = EwsClient.GroupExpansionErrorPropertyName,
				PropertyType = MapiPropertyTypeType.Integer
			},
			new PathToExtendedFieldType
			{
				PropertySetId = "403FC56B-CD30-47C5-86F8-EDE9E35A022B",
				PropertyName = EwsClient.GroupExpansionRecipientsPropertyName,
				PropertyType = MapiPropertyTypeType.String
			}
		};
	}
}
