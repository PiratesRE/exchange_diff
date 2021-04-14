using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.ExchangeService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ExchangeServiceTest : IExchangeServiceTest
	{
		public void CreateItem(string folderId, string subject, string body)
		{
			try
			{
				this.Initialization();
				MessageType messageType = new MessageType
				{
					Body = new BodyContentType
					{
						BodyType = BodyType.Text,
						Value = body
					},
					Subject = subject
				};
				CreateItemRequest request = new CreateItemRequest
				{
					ItemsArray = new MessageType[]
					{
						messageType
					},
					MessageDisposition = "SaveOnly",
					SavedItemFolderId = new TargetFolderId(new FolderId(folderId, IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder)))
				};
				using (IDisposableResponse<CreateItemResponse> disposableResponse = this.exchangeService.CreateItem(request, null))
				{
					CreateItemResponse response = disposableResponse.Response;
					if (response.ResponseMessages == null || response.ResponseMessages.Items == null || response.ResponseMessages.Items.Length != 1 || response.ResponseMessages.Items[0].ResponseClass != ResponseClass.Success)
					{
						throw new Exception("CreateItem failed.");
					}
				}
			}
			finally
			{
				this.Cleanup();
			}
		}

		public string CreateUnifiedMailbox()
		{
			this.CloseStopWatch();
			string userPrincipalName;
			using (CallContext callContext = this.CreateCallContext(HttpContext.Current, EwsOperationContextBase.Current.RequestMessage))
			{
				using (IExchangeService exchangeService = ExchangeServiceFactory.Default.CreateForEws(callContext))
				{
					CreateUnifiedMailboxRequest request = new CreateUnifiedMailboxRequest();
					CreateUnifiedMailboxResponse createUnifiedMailboxResponse = exchangeService.CreateUnifiedMailbox(request, null);
					userPrincipalName = createUnifiedMailboxResponse.UserPrincipalName;
				}
			}
			return userPrincipalName;
		}

		public bool IsOffice365Domain(string emailAddress)
		{
			bool isOffice365Domain;
			try
			{
				this.Initialization();
				IsOffice365DomainRequest request = new IsOffice365DomainRequest
				{
					EmailAddress = emailAddress
				};
				IsOffice365DomainResponse isOffice365DomainResponse = this.exchangeService.IsOffice365Domain(request, null);
				isOffice365Domain = isOffice365DomainResponse.IsOffice365Domain;
			}
			finally
			{
				this.Cleanup();
			}
			return isOffice365Domain;
		}

		public AggregatedAccountType AddAggregatedAccount(string emailAddress, string userName, string password, string incomingServer, string incomingPort, string incomingProtocol, string security, string authentication, string outgoingServer, string outgoingPort, string outgoingProtocol, string interval)
		{
			AggregatedAccountType result;
			try
			{
				this.Initialization();
				AddAggregatedAccountRequest request = new AddAggregatedAccountRequest
				{
					EmailAddress = emailAddress,
					UserName = userName,
					Password = password,
					IncomingServer = incomingServer,
					IncomingPort = incomingPort,
					IncomingProtocol = incomingProtocol,
					Security = security,
					OutgoingServer = outgoingServer,
					OutgoingPort = outgoingPort,
					OutgoingProtocol = outgoingProtocol
				};
				this.AssignIfParameterSet(authentication, delegate(string val)
				{
					request.Authentication = val;
				});
				this.AssignIfParameterSet(interval, delegate(string val)
				{
					request.IncrementalSyncInterval = val;
				});
				AddAggregatedAccountResponse addAggregatedAccountResponse = this.exchangeService.AddAggregatedAccount(request, null);
				result = ((addAggregatedAccountResponse != null) ? addAggregatedAccountResponse.Account : null);
			}
			finally
			{
				this.Cleanup();
			}
			return result;
		}

		public void RemoveAggregatedAccount(string emailAddress)
		{
			try
			{
				this.Initialization();
				RemoveAggregatedAccountRequest request = new RemoveAggregatedAccountRequest
				{
					EmailAddress = emailAddress
				};
				this.exchangeService.RemoveAggregatedAccount(request, null);
			}
			finally
			{
				this.Cleanup();
			}
		}

		public void SetAggregatedAccount(string authentication, string emailAddress, string userName, string password, string interval, string incomingServer, string incomingPort, string incomingProtocol, string security)
		{
			try
			{
				this.Initialization();
				SetAggregatedAccountRequest request = new SetAggregatedAccountRequest();
				this.AssignIfParameterSet(authentication, delegate(string val)
				{
					request.Authentication = val;
				});
				this.AssignIfParameterSet(emailAddress, delegate(string val)
				{
					request.EmailAddress = val;
				});
				this.AssignIfParameterSet(userName, delegate(string val)
				{
					request.UserName = val;
				});
				this.AssignIfParameterSet(incomingPort, delegate(string val)
				{
					request.IncomingPort = val;
				});
				this.AssignIfParameterSet(incomingServer, delegate(string val)
				{
					request.IncomingServer = val;
				});
				this.AssignIfParameterSet(interval, delegate(string val)
				{
					request.IncrementalSyncInterval = val;
				});
				this.AssignIfParameterSet(password, delegate(string val)
				{
					request.Password = val;
				});
				this.AssignIfParameterSet(security, delegate(string val)
				{
					request.Security = val;
				});
				this.exchangeService.SetAggregatedAccount(request, null);
			}
			finally
			{
				this.Cleanup();
			}
		}

		public AggregatedAccountType[] GetAggregatedAccount()
		{
			AggregatedAccountType[] aggregatedAccounts;
			try
			{
				this.Initialization();
				GetAggregatedAccountRequest request = new GetAggregatedAccountRequest();
				GetAggregatedAccountResponse aggregatedAccount = this.exchangeService.GetAggregatedAccount(request, null);
				aggregatedAccounts = aggregatedAccount.AggregatedAccounts;
			}
			finally
			{
				this.Cleanup();
			}
			return aggregatedAccounts;
		}

		public ConversationType[] FindConversation(string parentFolderId)
		{
			return this.InternalFindConversationForUnifiedMailbox(delegate(FindConversationRequest request)
			{
				request.ParentFolderId = new TargetFolderId(new FolderId(parentFolderId, IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder)));
			});
		}

		public ConversationType[] FindConversation(Guid[] mailboxGuids, DistinguishedFolderIdName defaultFolder)
		{
			return this.InternalFindConversationForUnifiedMailbox(delegate(FindConversationRequest request)
			{
				request.ParentFolderId = new TargetFolderId(new DistinguishedFolderId
				{
					Id = defaultFolder
				});
				request.MailboxGuids = mailboxGuids;
			});
		}

		public ItemType[] FindItem(string parentFolderId)
		{
			ItemType[] items;
			try
			{
				this.Initialization();
				FindItemRequest findItemRequest = new FindItemRequest();
				findItemRequest.Paging = new IndexedPageView
				{
					Origin = BasePagingType.PagingOrigin.Beginning,
					MaxRows = 100,
					Offset = 0
				};
				findItemRequest.ParentFolderIds = new BaseFolderId[]
				{
					new FolderId(parentFolderId, IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder))
				};
				findItemRequest.ItemShape = this.UseOutlookServiceItemShape();
				FindItemResponse findItemResponse = this.exchangeService.FindItem(findItemRequest, null);
				items = (findItemResponse.ResponseMessages.Items[0] as FindItemResponseMessage).ParentFolder.Items;
			}
			finally
			{
				this.Cleanup();
			}
			return items;
		}

		public ItemType[] FindItem(string[] folderIds)
		{
			return this.InternalFindItemForUnifiedMailbox(delegate(FindItemRequest request)
			{
				request.ParentFolderIds = new FolderId[folderIds.Length];
				for (int i = 0; i < folderIds.Length; i++)
				{
					request.ParentFolderIds[i] = new FolderId(folderIds[i], IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder));
				}
			});
		}

		public BaseFolderType[] FindFolder(DistinguishedFolderIdName distinguishedFolder)
		{
			return this.InternalFindFolder(delegate(FindFolderRequest request)
			{
				request.MailboxGuid = Guid.Empty;
				request.ParentFolderIds = new BaseFolderId[]
				{
					new DistinguishedFolderId
					{
						Id = distinguishedFolder
					}
				};
			});
		}

		public BaseFolderType[] FindFolder(string mailboxGuid)
		{
			return this.InternalFindFolder(delegate(FindFolderRequest request)
			{
				request.MailboxGuid = new Guid(mailboxGuid);
				request.ParentFolderIds = null;
			});
		}

		public Guid GetMailboxGuid()
		{
			Guid mailboxGuid;
			try
			{
				this.Initialization();
				mailboxGuid = this.callContext.SessionCache.GetMailboxIdentityMailboxSession().MailboxGuid;
			}
			finally
			{
				this.Cleanup();
			}
			return mailboxGuid;
		}

		public void SubscribeToConversationChanges(string subscriptionId, string parentFolderId)
		{
			this.InternalSubscribeToConversationChanges(subscriptionId, delegate(SubscribeToConversationChangesRequest request)
			{
				if (parentFolderId != null)
				{
					request.ParentFolderId = new TargetFolderId(new FolderId(parentFolderId, IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder)));
				}
			});
		}

		public void SubscribeToConversationChanges(string subscriptionId, Guid[] aggregatedMailboxGuids, DistinguishedFolderIdName defaultFolder)
		{
			this.InternalSubscribeToConversationChanges(subscriptionId, delegate(SubscribeToConversationChangesRequest request)
			{
				request.MailboxGuids = aggregatedMailboxGuids;
				request.ParentFolderId = new TargetFolderId(new DistinguishedFolderId
				{
					Id = defaultFolder
				});
			});
		}

		public ConversationNotification GetNextConversationChange(string subscriptionId)
		{
			ExDateTime t = ExDateTime.UtcNow.AddMinutes(2.0);
			while (ExDateTime.UtcNow < t)
			{
				lock (ExchangeServiceTest.conversationChanges)
				{
					Queue<ConversationNotification> queue = null;
					if (ExchangeServiceTest.conversationChanges.TryGetValue(subscriptionId, out queue) && queue.Count > 0)
					{
						return queue.Dequeue();
					}
				}
				Thread.Sleep(100);
			}
			return null;
		}

		public void SubscribeToCalendarChanges(string subscriptionId, string parentFolderId)
		{
			try
			{
				this.Initialization();
				SubscribeToCalendarChangesRequest subscribeToCalendarChangesRequest = new SubscribeToCalendarChangesRequest();
				subscribeToCalendarChangesRequest.SubscriptionId = subscriptionId;
				if (parentFolderId != null)
				{
					subscribeToCalendarChangesRequest.ParentFolderId = new TargetFolderId(new FolderId(parentFolderId, IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder)));
				}
				Action<CalendarChangeNotificationType> callback = delegate(CalendarChangeNotificationType type)
				{
					lock (ExchangeServiceTest.calendarChanges)
					{
						Queue<CalendarChangeNotificationType> queue = null;
						if (!ExchangeServiceTest.calendarChanges.TryGetValue(subscriptionId, out queue))
						{
							queue = new Queue<CalendarChangeNotificationType>();
							ExchangeServiceTest.calendarChanges.Add(subscriptionId, queue);
						}
						queue.Enqueue(type);
					}
				};
				this.exchangeService.SubscribeToCalendarChanges(subscribeToCalendarChangesRequest, callback, null);
			}
			finally
			{
				this.Cleanup();
			}
		}

		public CalendarChangeNotificationType? GetNextCalendarChange(string subscriptionId)
		{
			ExDateTime t = ExDateTime.UtcNow.AddMinutes(2.0);
			while (ExDateTime.UtcNow < t)
			{
				lock (ExchangeServiceTest.calendarChanges)
				{
					Queue<CalendarChangeNotificationType> queue = null;
					if (ExchangeServiceTest.calendarChanges.TryGetValue(subscriptionId, out queue) && queue.Count > 0)
					{
						return new CalendarChangeNotificationType?(queue.Dequeue());
					}
				}
				Thread.Sleep(100);
			}
			return null;
		}

		public InstantSearchPayloadType PerformInstantSearch(string deviceId, string searchSessionId, string kqlQuery, FolderId[] folderScope)
		{
			InstantSearchPayloadType payload2;
			try
			{
				PerformInstantSearchRequest request = new PerformInstantSearchRequest();
				request.DeviceId = deviceId;
				request.SearchSessionId = searchSessionId;
				request.KqlQuery = kqlQuery;
				request.FolderScope = folderScope;
				request.MaximumResultCount = 10;
				request.ItemType = InstantSearchItemType.MailConversation;
				request.QueryOptions = QueryOptionsType.Results;
				this.Initialization();
				Action<InstantSearchPayloadType> searchPayloadCallback = delegate(InstantSearchPayloadType payload)
				{
					lock (ExchangeServiceTest.instantSearchPayloads)
					{
						Queue<InstantSearchPayloadType> queue = null;
						if (!ExchangeServiceTest.instantSearchPayloads.TryGetValue(request.SearchSessionId, out queue))
						{
							queue = new Queue<InstantSearchPayloadType>();
							ExchangeServiceTest.instantSearchPayloads.Add(request.SearchSessionId, queue);
						}
						queue.Enqueue(payload);
					}
				};
				PerformInstantSearchResponse performInstantSearchResponse = this.exchangeService.PerformInstantSearch(request, searchPayloadCallback, null);
				payload2 = performInstantSearchResponse.Payload;
			}
			finally
			{
				this.Cleanup();
			}
			return payload2;
		}

		public InstantSearchPayloadType GetNextInstantSearchPayload(string sessionId)
		{
			ExDateTime t = ExDateTime.UtcNow.AddMinutes(2.0);
			while (ExDateTime.UtcNow < t)
			{
				lock (ExchangeServiceTest.instantSearchPayloads)
				{
					Queue<InstantSearchPayloadType> queue = null;
					if (ExchangeServiceTest.instantSearchPayloads.TryGetValue(sessionId, out queue) && queue.Count > 0)
					{
						return queue.Dequeue();
					}
				}
				Thread.Sleep(100);
			}
			return null;
		}

		public bool EndInstantSearchSession(string deviceId, string sessionId)
		{
			bool result;
			try
			{
				this.Initialization();
				this.exchangeService.EndInstantSearchSession(deviceId, sessionId, null);
				result = true;
			}
			finally
			{
				this.Cleanup();
			}
			return result;
		}

		public ConversationType[] InternalFindConversationForUnifiedMailbox(Action<FindConversationRequest> initializeRequest)
		{
			ConversationType[] conversations;
			try
			{
				this.Initialization();
				FindConversationRequest findConversationRequest = new FindConversationRequest();
				initializeRequest(findConversationRequest);
				findConversationRequest.Paging = new IndexedPageView
				{
					Origin = BasePagingType.PagingOrigin.Beginning,
					MaxRows = 100,
					Offset = 0
				};
				findConversationRequest.ConversationShape = this.UseOutlookServiceConversationShape();
				FindConversationResponseMessage findConversationResponseMessage = this.exchangeService.FindConversation(findConversationRequest, null);
				conversations = findConversationResponseMessage.Conversations;
			}
			finally
			{
				this.Cleanup();
			}
			return conversations;
		}

		public ItemType[] InternalFindItemForUnifiedMailbox(Action<FindItemRequest> initializeRequest)
		{
			ItemType[] items;
			try
			{
				this.Initialization();
				FindItemRequest findItemRequest = new FindItemRequest();
				initializeRequest(findItemRequest);
				findItemRequest.Paging = new IndexedPageView
				{
					Origin = BasePagingType.PagingOrigin.Beginning,
					MaxRows = 100,
					Offset = 0
				};
				findItemRequest.ItemShape = this.UseOutlookServiceItemShape();
				FindItemResponse findItemResponse = this.exchangeService.FindItem(findItemRequest, null);
				items = (findItemResponse.ResponseMessages.Items[0] as FindItemResponseMessage).ParentFolder.Items;
			}
			finally
			{
				this.Cleanup();
			}
			return items;
		}

		private BaseFolderType[] InternalFindFolder(Action<FindFolderRequest> initializeRequest)
		{
			BaseFolderType[] folders;
			try
			{
				this.Initialization();
				FindFolderRequest findFolderRequest = new FindFolderRequest();
				initializeRequest(findFolderRequest);
				findFolderRequest.Traversal = FolderQueryTraversal.Deep;
				PropertyPath[] additionalProperties = new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ParentFolderId),
					new PropertyUri(PropertyUriEnum.FolderDisplayName),
					new PropertyUri(PropertyUriEnum.FolderClass),
					new PropertyUri(PropertyUriEnum.DistinguishedFolderId)
				};
				findFolderRequest.FolderShape = new FolderResponseShape(ShapeEnum.IdOnly, additionalProperties);
				FindFolderResponse findFolderResponse = this.exchangeService.FindFolder(findFolderRequest, null);
				FindFolderResponseMessage findFolderResponseMessage = findFolderResponse.ResponseMessages.Items[0] as FindFolderResponseMessage;
				folders = findFolderResponseMessage.RootFolder.Folders;
			}
			finally
			{
				this.Cleanup();
			}
			return folders;
		}

		public void InternalSubscribeToConversationChanges(string subscriptionId, Action<SubscribeToConversationChangesRequest> initializeRequest)
		{
			try
			{
				this.Initialization();
				SubscribeToConversationChangesRequest subscribeToConversationChangesRequest = new SubscribeToConversationChangesRequest();
				subscribeToConversationChangesRequest.SubscriptionId = subscriptionId;
				subscribeToConversationChangesRequest.ConversationShape = this.UseOutlookServiceConversationShape();
				initializeRequest(subscribeToConversationChangesRequest);
				Action<ConversationNotification> callback = delegate(ConversationNotification notification)
				{
					lock (ExchangeServiceTest.conversationChanges)
					{
						Queue<ConversationNotification> queue = null;
						if (!ExchangeServiceTest.conversationChanges.TryGetValue(subscriptionId, out queue))
						{
							queue = new Queue<ConversationNotification>();
							ExchangeServiceTest.conversationChanges.Add(subscriptionId, queue);
						}
						queue.Enqueue(notification);
					}
				};
				this.exchangeService.SubscribeToConversationChanges(subscribeToConversationChangesRequest, callback, null);
			}
			finally
			{
				this.Cleanup();
			}
		}

		private CallContext CreateCallContext(HttpContext httpContext, Message message)
		{
			JsonMessageHeaderProcessor headerProcessor = new JsonMessageHeaderProcessor();
			message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
			MSAIdentity msaidentity = httpContext.User.Identity as MSAIdentity;
			CallContext callContext;
			if (msaidentity == null)
			{
				callContext = CallContext.CreateFromRequest(headerProcessor, message);
			}
			else
			{
				ExchangeServiceTest.InitIfNeeded(httpContext);
				BudgetKey key = new StringBudgetKey(msaidentity.MemberName, false, BudgetType.Ews);
				callContext = CallContext.CreateForExchangeService(httpContext, ExchangeServiceTest.appWideStoreSessionCache, ExchangeServiceTest.acceptedDomainCache, ExchangeServiceTest.userWorkloadManager, EwsBudget.Acquire(key), Thread.CurrentThread.CurrentCulture);
			}
			HttpContext.Current.Items["CallContext"] = callContext;
			return callContext;
		}

		private void CloseStopWatch()
		{
			HttpContext.Current.Items["ServicesStopwatch"] = null;
		}

		private void AssignIfParameterSet(string parameter, Action<string> assignValue)
		{
			if (!string.IsNullOrEmpty(parameter))
			{
				assignValue(parameter);
			}
		}

		private void Initialization()
		{
			this.CloseStopWatch();
			this.callContext = this.CreateCallContext(HttpContext.Current, EwsOperationContextBase.Current.RequestMessage);
			ExchangeVersion.Current = ExchangeVersion.Latest;
			this.exchangeService = ExchangeServiceFactory.Default.CreateForEws(this.callContext);
		}

		private void Cleanup()
		{
			this.callContext.Dispose();
			this.exchangeService.Dispose();
		}

		private static void InitIfNeeded(HttpContext httpContext)
		{
			if (!ExchangeServiceTest.initialized)
			{
				lock (ExchangeServiceTest.staticLock)
				{
					if (!ExchangeServiceTest.initialized)
					{
						HttpApplicationState application = httpContext.Application;
						ExchangeServiceTest.appWideStoreSessionCache = (application["WS_APPWideMailboxCacheKey"] as AppWideStoreSessionCache);
						ExchangeServiceTest.acceptedDomainCache = (application["WS_AcceptedDomainCacheKey"] as AcceptedDomainCache);
						ExchangeServiceTest.userWorkloadManager = (application["WS_WorkloadManagerKey"] as UserWorkloadManager);
						ExchangeServiceTest.initialized = true;
					}
				}
			}
		}

		private ConversationResponseShape UseOutlookServiceConversationShape()
		{
			return new ConversationResponseShape
			{
				BaseShape = ShapeEnum.IdOnly,
				AdditionalProperties = new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ItemParentId),
					new PropertyUri(PropertyUriEnum.ConversationGuidId),
					new PropertyUri(PropertyUriEnum.Topic),
					new PropertyUri(PropertyUriEnum.ConversationPreview),
					new PropertyUri(PropertyUriEnum.ConversationGlobalUniqueSenders),
					new PropertyUri(PropertyUriEnum.ConversationLastModifiedTime),
					new PropertyUri(PropertyUriEnum.ConversationGlobalUnreadCount),
					new PropertyUri(PropertyUriEnum.ConversationGlobalFlagStatus),
					new PropertyUri(PropertyUriEnum.ConversationGlobalHasIrm),
					new PropertyUri(PropertyUriEnum.ConversationGlobalLastDeliveryOrRenewTime),
					new PropertyUri(PropertyUriEnum.InstanceKey),
					new PropertyUri(PropertyUriEnum.ConversationGlobalItemIds),
					new PropertyUri(PropertyUriEnum.ConversationGlobalRichContent),
					new PropertyUri(PropertyUriEnum.ConversationLastDeliveryTime)
				}
			};
		}

		private ItemResponseShape UseOutlookServiceItemShape()
		{
			return new ItemResponseShape
			{
				BaseShape = ShapeEnum.Default
			};
		}

		public bool GetFolderFidAndMailboxFromEwsId(string ewsId, out long fid, out Guid mailboxGuid)
		{
			bool folderFidAndMailboxFromEwsId;
			try
			{
				this.Initialization();
				folderFidAndMailboxFromEwsId = this.exchangeService.GetFolderFidAndMailboxFromEwsId(ewsId, out fid, out mailboxGuid);
			}
			finally
			{
				this.Cleanup();
			}
			return folderFidAndMailboxFromEwsId;
		}

		public long GetFolderFidFromEwsId(string ewsId)
		{
			long folderFidFromEwsId;
			try
			{
				this.Initialization();
				folderFidFromEwsId = this.exchangeService.GetFolderFidFromEwsId(ewsId);
			}
			finally
			{
				this.Cleanup();
			}
			return folderFidFromEwsId;
		}

		public string GetEwsIdFromFolderFid(long fid, Guid mailboxGuid)
		{
			string ewsIdFromFolderFid;
			try
			{
				this.Initialization();
				ewsIdFromFolderFid = this.exchangeService.GetEwsIdFromFolderFid(fid, mailboxGuid);
			}
			finally
			{
				this.Cleanup();
			}
			return ewsIdFromFolderFid;
		}

		public void SubscribeToHierarchyChanges(string subscriptionId, Guid mailboxGuid)
		{
			try
			{
				this.Initialization();
				SubscribeToHierarchyChangesRequest subscribeToHierarchyChangesRequest = new SubscribeToHierarchyChangesRequest();
				subscribeToHierarchyChangesRequest.SubscriptionId = subscriptionId;
				subscribeToHierarchyChangesRequest.MailboxGuid = mailboxGuid;
				Action<HierarchyNotification> callback = delegate(HierarchyNotification notification)
				{
					lock (ExchangeServiceTest.hierarchyChanges)
					{
						Queue<HierarchyNotification> queue = null;
						if (!ExchangeServiceTest.hierarchyChanges.TryGetValue(subscriptionId, out queue))
						{
							queue = new Queue<HierarchyNotification>();
							ExchangeServiceTest.hierarchyChanges.Add(subscriptionId, queue);
						}
						queue.Enqueue(notification);
					}
				};
				this.exchangeService.SubscribeToHierarchyChanges(subscribeToHierarchyChangesRequest, callback, null);
			}
			finally
			{
				this.Cleanup();
			}
		}

		public HierarchyNotification GetNextHierarchyChange(string subscriptionId)
		{
			ExDateTime t = ExDateTime.UtcNow.AddMinutes(2.0);
			while (ExDateTime.UtcNow < t)
			{
				lock (ExchangeServiceTest.hierarchyChanges)
				{
					Queue<HierarchyNotification> queue = null;
					if (ExchangeServiceTest.hierarchyChanges.TryGetValue(subscriptionId, out queue) && queue.Count > 0)
					{
						return queue.Dequeue();
					}
				}
				Thread.Sleep(100);
			}
			return null;
		}

		public void SubscribeToMessageChanges(string subscriptionId, string parentFolderId)
		{
			this.InternalSubscribeToMessageChanges(subscriptionId, delegate(SubscribeToMessageChangesRequest request)
			{
				if (parentFolderId != null)
				{
					request.ParentFolderId = new TargetFolderId(new FolderId(parentFolderId, IdConverter.BuildChangeKeyString(null, StoreObjectType.Folder)));
				}
			});
		}

		public void SubscribeToMessageChanges(string subscriptionId, Guid[] aggregatedMailboxGuids, DistinguishedFolderIdName defaultFolder)
		{
			this.InternalSubscribeToMessageChanges(subscriptionId, delegate(SubscribeToMessageChangesRequest request)
			{
				request.MailboxGuids = aggregatedMailboxGuids;
				request.ParentFolderId = new TargetFolderId(new DistinguishedFolderId
				{
					Id = defaultFolder
				});
			});
		}

		public MessageNotification GetNextMessageChange(string subscriptionId)
		{
			ExDateTime t = ExDateTime.UtcNow.AddMinutes(2.0);
			while (ExDateTime.UtcNow < t)
			{
				lock (ExchangeServiceTest.messageChanges)
				{
					Queue<MessageNotification> queue = null;
					if (ExchangeServiceTest.messageChanges.TryGetValue(subscriptionId, out queue) && queue.Count > 0)
					{
						return queue.Dequeue();
					}
				}
				Thread.Sleep(100);
			}
			return null;
		}

		public void InternalSubscribeToMessageChanges(string subscriptionId, Action<SubscribeToMessageChangesRequest> initializeRequest)
		{
			try
			{
				this.Initialization();
				SubscribeToMessageChangesRequest subscribeToMessageChangesRequest = new SubscribeToMessageChangesRequest();
				subscribeToMessageChangesRequest.SubscriptionId = subscriptionId;
				subscribeToMessageChangesRequest.MessageShape = this.UseOutlookServiceItemShape();
				initializeRequest(subscribeToMessageChangesRequest);
				Action<MessageNotification> callback = delegate(MessageNotification notification)
				{
					lock (ExchangeServiceTest.messageChanges)
					{
						Queue<MessageNotification> queue = null;
						if (!ExchangeServiceTest.messageChanges.TryGetValue(subscriptionId, out queue))
						{
							queue = new Queue<MessageNotification>();
							ExchangeServiceTest.messageChanges.Add(subscriptionId, queue);
						}
						queue.Enqueue(notification);
					}
				};
				this.exchangeService.SubscribeToMessageChanges(subscribeToMessageChangesRequest, callback, null);
			}
			finally
			{
				this.Cleanup();
			}
		}

		private static object staticLock = new object();

		private static bool initialized = false;

		private static AppWideStoreSessionCache appWideStoreSessionCache;

		private static AcceptedDomainCache acceptedDomainCache;

		private static UserWorkloadManager userWorkloadManager;

		private CallContext callContext;

		private IExchangeService exchangeService;

		private static Dictionary<string, Queue<ConversationNotification>> conversationChanges = new Dictionary<string, Queue<ConversationNotification>>();

		private static Dictionary<string, Queue<CalendarChangeNotificationType>> calendarChanges = new Dictionary<string, Queue<CalendarChangeNotificationType>>();

		private static Dictionary<string, Queue<HierarchyNotification>> hierarchyChanges = new Dictionary<string, Queue<HierarchyNotification>>();

		private static Dictionary<string, Queue<MessageNotification>> messageChanges = new Dictionary<string, Queue<MessageNotification>>();

		private static Dictionary<string, Queue<InstantSearchPayloadType>> instantSearchPayloads = new Dictionary<string, Queue<InstantSearchPayloadType>>();
	}
}
