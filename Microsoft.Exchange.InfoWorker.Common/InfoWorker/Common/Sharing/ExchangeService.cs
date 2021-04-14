using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Sharing;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class ExchangeService : IDisposable
	{
		public ExchangeService(SoapHttpClientAuthenticator authenticator, SharingSubscriptionData subscriptionData, IWebProxy webProxy, string componentId) : this(authenticator, subscriptionData, webProxy, componentId, true)
		{
		}

		public ExchangeService(SoapHttpClientAuthenticator authenticator, SharingSubscriptionData subscriptionData, IWebProxy webProxy, string componentId, bool normalization)
		{
			this.folderId = new FolderIdType
			{
				Id = subscriptionData.RemoteFolderId,
				ChangeKey = null
			};
			this.binding = new ExchangeServiceBinding(componentId, new RemoteCertificateValidationCallback(CertificateErrorHandler.CertValidationCallback), normalization);
			this.binding.Url = subscriptionData.SharingUrl.ToString();
			this.binding.RequestServerVersionValue = ExchangeService.RequestServerVersionExchange2010;
			this.binding.Authenticator = authenticator;
			this.binding.Proxy = webProxy;
			this.binding.UserAgent = componentId;
			this.binding.HttpHeaders[WellKnownHeader.AnchorMailbox] = subscriptionData.SharerIdentity;
		}

		public string Url
		{
			get
			{
				return this.binding.Url;
			}
			set
			{
				this.binding.Url = value;
			}
		}

		public void Dispose()
		{
			this.binding.Dispose();
		}

		public Changes GetChanges(int numberOfItems, string syncState)
		{
			SyncFolderItemsType request = new SyncFolderItemsType
			{
				ItemShape = ExchangeService.IdOnlyShape,
				SyncFolderId = new TargetFolderIdType
				{
					Item = this.folderId
				},
				SyncState = syncState,
				Ignore = null,
				MaxChangesReturned = numberOfItems,
				SyncScope = SyncFolderItemsScopeType.NormalItems
			};
			SyncFolderItemsResponseMessageType syncFolderItemsResponseMessageType = this.SyncFolderItems(request);
			if (string.IsNullOrEmpty(syncFolderItemsResponseMessageType.SyncState))
			{
				ExchangeService.Tracer.TraceError<ExchangeService>((long)this.GetHashCode(), "{0}: sync state is missing", this);
				throw new UnexpectedRemoteDataException(new ResponseMessageType[]
				{
					syncFolderItemsResponseMessageType
				});
			}
			List<ItemChange> list = new List<ItemChange>(numberOfItems);
			if (syncFolderItemsResponseMessageType.Changes != null && syncFolderItemsResponseMessageType.Changes.Items != null)
			{
				int i = 0;
				while (i < syncFolderItemsResponseMessageType.Changes.Items.Length)
				{
					ItemChangeType itemChangeType;
					switch (syncFolderItemsResponseMessageType.Changes.ItemsElementName[i])
					{
					case ItemsChoiceType2.Create:
						itemChangeType = ItemChangeType.Create;
						goto IL_120;
					case ItemsChoiceType2.Delete:
						itemChangeType = ItemChangeType.Delete;
						goto IL_120;
					case ItemsChoiceType2.ReadFlagChange:
						break;
					case ItemsChoiceType2.Update:
						itemChangeType = ItemChangeType.Update;
						goto IL_120;
					default:
						ExchangeService.Tracer.TraceDebug<ExchangeService, ItemsChoiceType2>((long)this.GetHashCode(), "{0}: unknown change type, ignoring: {1}", this, syncFolderItemsResponseMessageType.Changes.ItemsElementName[i]);
						break;
					}
					IL_25F:
					i++;
					continue;
					IL_120:
					ItemIdType itemIdType = null;
					switch (itemChangeType)
					{
					case ItemChangeType.Create:
					case ItemChangeType.Update:
					{
						SyncFolderItemsCreateOrUpdateType syncFolderItemsCreateOrUpdateType = syncFolderItemsResponseMessageType.Changes.Items[i] as SyncFolderItemsCreateOrUpdateType;
						if (syncFolderItemsCreateOrUpdateType == null || syncFolderItemsCreateOrUpdateType.Item == null || syncFolderItemsCreateOrUpdateType.Item.ItemId == null)
						{
							ExchangeService.Tracer.TraceError<ExchangeService, int>((long)this.GetHashCode(), "{0}: ignoring item {1} change because of missing data", this, i);
							goto IL_25F;
						}
						itemIdType = syncFolderItemsCreateOrUpdateType.Item.ItemId;
						break;
					}
					case ItemChangeType.Delete:
					{
						SyncFolderItemsDeleteType syncFolderItemsDeleteType = syncFolderItemsResponseMessageType.Changes.Items[i] as SyncFolderItemsDeleteType;
						if (syncFolderItemsDeleteType == null || syncFolderItemsDeleteType.ItemId == null)
						{
							ExchangeService.Tracer.TraceError<ExchangeService, int>((long)this.GetHashCode(), "{0}: ignoring item {1} change because of missing data", this, i);
						}
						itemIdType = syncFolderItemsDeleteType.ItemId;
						break;
					}
					}
					if (itemIdType == null || string.IsNullOrEmpty(itemIdType.Id))
					{
						ExchangeService.Tracer.TraceError<ExchangeService, int>((long)this.GetHashCode(), "{0}: ignoring item {1} change without id", this, i);
						goto IL_25F;
					}
					ExchangeService.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: item {1} change {2} of item {3}", new object[]
					{
						this,
						i,
						itemChangeType,
						itemIdType.Id
					});
					list.Add(new ItemChange(itemChangeType, itemIdType));
					goto IL_25F;
				}
			}
			return new Changes(syncFolderItemsResponseMessageType.SyncState, syncFolderItemsResponseMessageType.IncludesLastItemInRangeSpecified && !syncFolderItemsResponseMessageType.IncludesLastItemInRange, list.ToArray());
		}

		public BaseFolderType GetFolder(string remoteFolderId)
		{
			GetFolderType request = new GetFolderType
			{
				FolderShape = ExchangeService.GetFolderShape,
				FolderIds = new BaseFolderIdType[]
				{
					new FolderIdType
					{
						Id = remoteFolderId
					}
				}
			};
			FolderInfoResponseMessageType folder = this.GetFolder(request);
			if (folder.Folders == null || folder.Folders.Length != 1)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, string>((long)this.GetHashCode(), "{0}: received unexpected response for folder : {1}", this, remoteFolderId);
				throw new UnexpectedRemoteDataException(new ResponseMessageType[]
				{
					folder
				});
			}
			return folder.Folders[0];
		}

		public ItemType[] GetItem(ItemIdType[] itemIds, StoreObjectType folderType)
		{
			ItemResponseShapeType itemShape;
			if (folderType == StoreObjectType.CalendarFolder)
			{
				itemShape = ExchangeService.CalendarGetItemShape;
			}
			else
			{
				itemShape = ExchangeService.ContactGetItemShape;
			}
			GetItemType request = new GetItemType
			{
				ItemShape = itemShape,
				ItemIds = itemIds
			};
			ItemInfoResponseMessageType[] item = this.GetItem(request);
			List<ItemType> list = new List<ItemType>(item.Length);
			foreach (ItemInfoResponseMessageType itemInfoResponseMessageType in item)
			{
				if (itemInfoResponseMessageType.Items != null && itemInfoResponseMessageType.Items.Items != null && itemInfoResponseMessageType.Items.Items.Length != 0)
				{
					list.AddRange(itemInfoResponseMessageType.Items.Items);
				}
			}
			return list.ToArray();
		}

		private FolderInfoResponseMessageType GetFolder(GetFolderType request)
		{
			return this.ExecuteWebMethodSingleResponse<FolderInfoResponseMessageType>(() => this.binding.GetFolder(request));
		}

		private SyncFolderItemsResponseMessageType SyncFolderItems(SyncFolderItemsType request)
		{
			return this.ExecuteWebMethodSingleResponse<SyncFolderItemsResponseMessageType>(() => this.binding.SyncFolderItems(request));
		}

		private ItemInfoResponseMessageType[] GetItem(GetItemType request)
		{
			return this.ExecuteWebMethodMultiResponse<ItemInfoResponseMessageType>(() => this.binding.GetItem(request));
		}

		private T[] ExecuteWebMethodMultiResponse<T>(Func<BaseResponseMessageType> webMethodDelegate) where T : ResponseMessageType
		{
			ResponseMessageType[] array = this.ExecuteWebMethod(webMethodDelegate);
			List<T> list = new List<T>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				ResponseMessageType responseMessageType = array[i];
				if (responseMessageType == null)
				{
					ExchangeService.Tracer.TraceError<ExchangeService, int>((long)this.GetHashCode(), "{0}: response item {1} is not present", this, i);
				}
				else if (responseMessageType.ResponseClass != ResponseClassType.Success)
				{
					ExchangeService.Tracer.TraceError((long)this.GetHashCode(), "{0}: response item {1} failed: ResponseClass={2}, ResponseCode={3}, MessageText={4}", new object[]
					{
						this,
						i,
						responseMessageType.ResponseClass,
						responseMessageType.ResponseCode,
						responseMessageType.MessageText
					});
				}
				else
				{
					T t = responseMessageType as T;
					if (t == null)
					{
						ExchangeService.Tracer.TraceError<ExchangeService, int, ResponseMessageType>((long)this.GetHashCode(), "{0}: response item {1} is not of expected type: {2}", this, i, responseMessageType);
					}
					else
					{
						list.Add(t);
					}
				}
			}
			if (list.Count == 0)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, string>((long)this.GetHashCode(), "{0}: Request for {1} did not return any successful items.", this, webMethodDelegate.Method.Name);
				throw new UnexpectedRemoteDataException(array);
			}
			return list.ToArray();
		}

		private T ExecuteWebMethodSingleResponse<T>(Func<BaseResponseMessageType> webMethodDelegate) where T : ResponseMessageType
		{
			ResponseMessageType[] array = this.ExecuteWebMethod(webMethodDelegate);
			if (array.Length != 1)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, string, int>((long)this.GetHashCode(), "{0}: Request for {1} did return unexpected number of items when only one was expected: {2}.", this, webMethodDelegate.Method.Name, array.Length);
				throw new UnexpectedRemoteDataException(array);
			}
			ResponseMessageType responseMessageType = array[0];
			if (responseMessageType == null)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, string>((long)this.GetHashCode(), "{0}: Request for {1} did return not return item.", this, webMethodDelegate.Method.Name);
				throw new UnexpectedRemoteDataException(new ResponseMessageType[]
				{
					responseMessageType
				});
			}
			if (responseMessageType.ResponseClass == ResponseClassType.Error || responseMessageType.ResponseCode == ResponseCodeType.ErrorCrossSiteRequest)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, string>((long)this.GetHashCode(), "{0}: Request for {1} failed because wrong CAS server.", this, webMethodDelegate.Method.Name);
				throw new FailedCommunicationException();
			}
			if (responseMessageType.ResponseClass != ResponseClassType.Success || responseMessageType.ResponseCode != ResponseCodeType.NoError)
			{
				ExchangeService.Tracer.TraceError((long)this.GetHashCode(), "{0}: Request for {1} failed. ResponseClass={2}, ResponseCode={3}.", new object[]
				{
					this,
					webMethodDelegate.Method.Name,
					responseMessageType.ResponseClass,
					responseMessageType.ResponseCode
				});
				throw new UnexpectedRemoteDataException(new ResponseMessageType[]
				{
					responseMessageType
				});
			}
			T t = responseMessageType as T;
			if (t == null)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, T, Type>((long)this.GetHashCode(), "{0}: response item {1} is not of expected type: {2}", this, t, typeof(T));
				throw new UnexpectedRemoteDataException(new ResponseMessageType[]
				{
					responseMessageType
				});
			}
			return t;
		}

		private ResponseMessageType[] ExecuteWebMethod(Func<BaseResponseMessageType> webMethodDelegate)
		{
			ExchangeService.Tracer.TraceDebug<ExchangeService, string>((long)this.GetHashCode(), "{0}: calling {1}", this, webMethodDelegate.Method.Name);
			BaseResponseMessageType response = null;
			int num = 0;
			for (;;)
			{
				Exception exception = null;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						ExchangeService.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Calling {0}", webMethodDelegate.Method.Name);
						try
						{
							response = webMethodDelegate();
						}
						catch (SoapException exception)
						{
							SoapException exception = exception;
						}
						catch (WebException exception2)
						{
							SoapException exception = exception2;
						}
						catch (IOException exception3)
						{
							SoapException exception = exception3;
						}
						catch (InvalidOperationException exception4)
						{
							SoapException exception = exception4;
						}
					});
				}
				catch (GrayException ex)
				{
					ExchangeService.Tracer.TraceError<ExchangeService, string, GrayException>((long)this.GetHashCode(), "{0}: Failed request {1} with gray exception {2}", this, webMethodDelegate.Method.Name, ex);
					throw new UnexpectedRemoteDataException(ex);
				}
				if (exception == null)
				{
					goto IL_10A;
				}
				ExchangeService.Tracer.TraceError<ExchangeService, string, Exception>((long)this.GetHashCode(), "{0}: failed request {1} with exception {2}", this, webMethodDelegate.Method.Name, exception);
				if (num >= 3 || HttpWebRequestExceptionHandler.IsConnectionException(exception, ExchangeService.Tracer))
				{
					break;
				}
				num++;
			}
			throw new FailedCommunicationException(CS$<>8__locals2.exception);
			IL_10A:
			if (response == null || response.ResponseMessages == null || response.ResponseMessages.Items == null || response.ResponseMessages.Items.Length == 0)
			{
				ExchangeService.Tracer.TraceError<ExchangeService, string>((long)this.GetHashCode(), "{0}: received empty response from request {1}", this, webMethodDelegate.Method.Name);
				throw new UnexpectedRemoteDataException();
			}
			return response.ResponseMessages.Items;
		}

		private const int MaximumTransientFailureRetries = 3;

		private static readonly ItemResponseShapeType CalendarGetItemShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			IncludeMimeContent = false,
			IncludeMimeContentSpecified = false,
			BodyType = BodyTypeResponseType.HTML,
			BodyTypeSpecified = true,
			AdditionalProperties = new PathToUnindexedFieldType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemSubject
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemBody
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemSensitivity
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarStart
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarEnd
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarIsAllDayEvent
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarLocation
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarIsCancelled
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarIsRecurring
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarCalendarItemType
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarDuration
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarTimeZone
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarRecurrence
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarModifiedOccurrences
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarDeletedOccurrences
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarStartTimeZone
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarEndTimeZone
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.calendarLegacyFreeBusyStatus
				}
			}
		};

		private static readonly ItemResponseShapeType ContactGetItemShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			IncludeMimeContent = false,
			IncludeMimeContentSpecified = false,
			BodyType = BodyTypeResponseType.HTML,
			BodyTypeSpecified = false,
			AdditionalProperties = new BasePathToElementType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemBody
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemCategories
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.itemSensitivity
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsAssistantName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsBirthday
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsBusinessHomePage
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsChildren
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsCompanies
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsCompanyName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsCompleteName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsCulture
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsDepartment
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsDisplayName
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsEmailAddress,
					FieldIndex = EmailAddressKeyType.EmailAddress1.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsEmailAddress,
					FieldIndex = EmailAddressKeyType.EmailAddress2.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsEmailAddress,
					FieldIndex = EmailAddressKeyType.EmailAddress3.ToString()
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsFileAs
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsFileAsMapping
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsGeneration
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsGivenName
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsImAddress,
					FieldIndex = ImAddressKeyType.ImAddress1.ToString()
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsInitials
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsJobTitle
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsManager
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsMiddleName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsMileage
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsNickname
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsOfficeLocation
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.AssistantPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.BusinessFax.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.BusinessPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.BusinessPhone2.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.Callback.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.CarPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.CompanyMainPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.HomeFax.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.HomePhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.HomePhone2.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.Isdn.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.MobilePhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.OtherFax.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.OtherTelephone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.Pager.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.PrimaryPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.RadioPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.Telex.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhoneNumber,
					FieldIndex = PhoneNumberKeyType.TtyTddPhone.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressCity,
					FieldIndex = PhysicalAddressKeyType.Business.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressCountryOrRegion,
					FieldIndex = PhysicalAddressKeyType.Business.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressPostalCode,
					FieldIndex = PhysicalAddressKeyType.Business.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressState,
					FieldIndex = PhysicalAddressKeyType.Business.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressStreet,
					FieldIndex = PhysicalAddressKeyType.Business.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressCity,
					FieldIndex = PhysicalAddressKeyType.Home.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressCountryOrRegion,
					FieldIndex = PhysicalAddressKeyType.Home.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressPostalCode,
					FieldIndex = PhysicalAddressKeyType.Home.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressState,
					FieldIndex = PhysicalAddressKeyType.Home.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressStreet,
					FieldIndex = PhysicalAddressKeyType.Home.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressCity,
					FieldIndex = PhysicalAddressKeyType.Other.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressCountryOrRegion,
					FieldIndex = PhysicalAddressKeyType.Other.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressPostalCode,
					FieldIndex = PhysicalAddressKeyType.Other.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressState,
					FieldIndex = PhysicalAddressKeyType.Other.ToString()
				},
				new PathToIndexedFieldType
				{
					FieldURI = DictionaryURIType.contactsPhysicalAddressStreet,
					FieldIndex = PhysicalAddressKeyType.Other.ToString()
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsPostalAddressIndex
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsProfession
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsSpouseName
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsSurname
				},
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.contactsWeddingAnniversary
				}
			}
		};

		private static readonly ItemResponseShapeType IdOnlyShape = new ItemResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			IncludeMimeContent = false,
			IncludeMimeContentSpecified = false,
			BodyType = BodyTypeResponseType.Best,
			BodyTypeSpecified = false,
			AdditionalProperties = null
		};

		private static readonly FolderResponseShapeType GetFolderShape = new FolderResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly,
			AdditionalProperties = new PathToUnindexedFieldType[]
			{
				new PathToUnindexedFieldType
				{
					FieldURI = UnindexedFieldURIType.folderSharingEffectiveRights
				}
			}
		};

		private static readonly RequestServerVersion RequestServerVersionExchange2010 = new RequestServerVersion
		{
			Version = ExchangeVersionType.Exchange2010
		};

		private static readonly Trace Tracer = ExTraceGlobals.ExchangeServiceTracer;

		private ExchangeServiceBinding binding;

		private FolderIdType folderId;
	}
}
