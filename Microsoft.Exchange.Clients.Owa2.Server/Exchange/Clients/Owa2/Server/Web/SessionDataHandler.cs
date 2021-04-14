using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class SessionDataHandler : IHttpAsyncHandler, IHttpHandler
	{
		static SessionDataHandler()
		{
			OwsLogRegistry.Register("SessionData_Overall", typeof(SessionDataMetadata), new Type[0]);
			OwsLogRegistry.Register("SessionData_FindItem", typeof(FindConversationAndItemMetadata), new Type[0]);
			OwsLogRegistry.Register("SessionData_FindConversation", typeof(FindConversationAndItemMetadata), new Type[0]);
			OwsLogRegistry.Register("SessionData_FindFolders", typeof(FindFoldersMetadata), new Type[0]);
		}

		protected virtual Stream OutputStream
		{
			get
			{
				return this.requestContext.HttpContext.Response.OutputStream;
			}
		}

		protected virtual Encoding Encoding
		{
			get
			{
				return this.requestContext.HttpContext.Response.ContentEncoding;
			}
		}

		protected virtual bool BufferOutput
		{
			get
			{
				return this.requestContext.HttpContext.Response.BufferOutput;
			}
			set
			{
				this.requestContext.HttpContext.Response.BufferOutput = value;
			}
		}

		protected virtual string ContentType
		{
			get
			{
				return this.requestContext.HttpContext.Response.ContentType;
			}
			set
			{
				this.requestContext.HttpContext.Response.ContentType = value;
			}
		}

		protected virtual string LogEventId
		{
			get
			{
				return "SessionData_Overall";
			}
		}

		protected bool IsSessionDataPreloadEnabled
		{
			get
			{
				UserContext userContext = this.requestContext.UserContext as UserContext;
				return userContext != null && userContext.FeaturesManager.ServerSettings.OwaSessionDataPreload.Enabled;
			}
		}

		protected virtual bool IsSessionDataPreloadRequest
		{
			get
			{
				return false;
			}
		}

		protected Microsoft.Exchange.Clients.Owa2.Server.Core.RequestContext RequestContext
		{
			get
			{
				return this.requestContext;
			}
		}

		bool IHttpHandler.IsReusable
		{
			get
			{
				return false;
			}
		}

		public static string EmitPayload(string payloadName, string actionData)
		{
			if (string.IsNullOrEmpty(payloadName))
			{
				throw new ArgumentNullException("payloadName");
			}
			if (string.IsNullOrEmpty(actionData))
			{
				return null;
			}
			return string.Format("{0}{1}={2};\r\n", "PageDataPayload.", payloadName, actionData);
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			bool shouldContinue = true;
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					this.GetAndInitializeContextMetadata(context);
					this.OnBeforeBeginProcessing(context, out shouldContinue);
				});
			}
			catch (GrayException arg)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<GrayException>(0L, "SessionDataHandler.BeginProcessRequest: GrayException: {0}", arg);
				throw;
			}
			AsyncEnumerator asyncEnumerator = new AsyncEnumerator(cb, extraData, (AsyncEnumerator aeInner) => this.StartRequestProcessing(context, shouldContinue, aeInner));
			return asyncEnumerator.AsyncResult;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			UserContext userContext = null;
			try
			{
				HttpContext.Current = this.requestContext.HttpContext;
				RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(this.requestContext.HttpContext, this.sessionDataRequestDetailsLogger);
				HttpCallContext.Singleton.LocalId = new Guid?(((ActivityScope)this.sessionDataRequestDetailsLogger.ActivityScope).ActivityScopeImpl.LocalId);
				this.AddSessionDataLogInfo();
				((AsyncResult)result).End();
				userContext = UserContextManager.GetUserContext(this.requestContext.HttpContext, true);
				this.OnBeforeEndProcessing(userContext);
				RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.SessionDataHandlerEnd);
			}
			catch (AsyncExceptionWrapper ex)
			{
				Exception rootException = AsyncExceptionWrapperHelper.GetRootException(ex);
				this.HandleSessionDataException(userContext, rootException);
			}
			catch (AsyncLocalizedExceptionWrapper ex2)
			{
				Exception rootException2 = AsyncExceptionWrapperHelper.GetRootException(ex2);
				this.HandleSessionDataException(userContext, rootException2);
			}
		}

		private void HandleSessionDataException(UserContext userContext, Exception rootException)
		{
			OwaServerLogger.AppendToLog(new TraceLogEvent("SessionDataHandlerId", userContext, "SessionDataHandler", rootException.ToString()));
			if (this.compositeSDRequest && !this.compositeSDRequestRetryAttempt)
			{
				this.HandleCompositeSessionDataException(rootException);
			}
			if (rootException.GetType() == typeof(AccessDeniedException) && this.requestContext.UserContext.IsExplicitLogon)
			{
				ErrorHandlerUtilities.HandleException(this.requestContext, new OwaExplicitLogonException("User has no access rights to the mailbox", "ErrorExplicitLogonAccessDenied "));
				return;
			}
			ErrorHandlerUtilities.HandleException(this.requestContext, rootException);
		}

		protected void HandleCompositeSessionDataException(Exception exception)
		{
			this.WriteOutput("Content-Name: SdError");
			this.WriteOutput("\r\n\r\n");
			Exception baseException = ErrorHandlerUtilities.GetBaseException(exception);
			if (baseException is WrongServerException || baseException is DatabaseNotFoundException)
			{
				this.WriteOutput("RetryCrrRequest");
			}
			this.WriteOutput(baseException.GetType().FullName);
			this.Flush();
		}

		protected virtual void OnBeforeBeginProcessing(HttpContext context, out bool shouldContinue)
		{
			if (!this.Initialize(context.Response))
			{
				shouldContinue = false;
			}
			this.SendCompositeUserSpecificInjector(context);
			shouldContinue = true;
			if (!this.IsSessionDataPreloadEnabled)
			{
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "[SessionDataHandler] SessionDataCache is not supported");
				return;
			}
			this.RequestContext.UserContext.SessionDataCache.UseIfReady(this.sessionDataRequestDetailsLogger, context.Response.OutputStream, out this.usedSessionData);
			if (this.usedSessionData)
			{
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "[SessionDataHandler] SessionDataCache used. Ending the request.");
				shouldContinue = false;
				return;
			}
			ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "[SessionDataHandler] SessionDataCache NOT used. Continuing the request.");
		}

		protected virtual void OnBeforeEndProcessing(UserContext userContext)
		{
		}

		protected virtual void Flush()
		{
			this.requestContext.HttpContext.Response.Flush();
		}

		private static void WriteJsonVariable<T>(Stream outputStream, Encoding encoding, string variableName, T jsonObject, bool shouldAddSeparator)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
			byte[] bytes;
			if (shouldAddSeparator)
			{
				bytes = encoding.GetBytes(",");
				outputStream.Write(bytes, 0, bytes.Length);
			}
			bytes = encoding.GetBytes("\"" + variableName + "\"");
			outputStream.Write(bytes, 0, bytes.Length);
			bytes = encoding.GetBytes(":");
			outputStream.Write(bytes, 0, bytes.Length);
			dataContractJsonSerializer.WriteObject(outputStream, jsonObject);
		}

		private static TimeZoneContextType GetTimeZoneHeader(string timeZoneStr)
		{
			return new TimeZoneContextType
			{
				TimeZoneDefinition = new TimeZoneDefinitionType
				{
					Id = timeZoneStr
				}
			};
		}

		private static JsonRequestHeaders GetHeaders(string timeZoneStr)
		{
			return new JsonRequestHeaders
			{
				TimeZoneContext = SessionDataHandler.GetTimeZoneHeader(timeZoneStr),
				RequestServerVersion = ExchangeVersionType.Exchange2012
			};
		}

		private static FindFolderJsonRequest GetFindFolderJsonRequest(string timeZoneStr, DistinguishedFolderIdName[] foldersToMoveToTop)
		{
			return new FindFolderJsonRequest
			{
				Header = SessionDataHandler.GetHeaders(timeZoneStr),
				Body = new FindFolderRequest
				{
					ShapeName = WellKnownShapeName.Folder.ToString(),
					Traversal = FolderQueryTraversal.Deep,
					ReturnParentFolder = true,
					ParentFolderIds = new BaseFolderId[]
					{
						new DistinguishedFolderId
						{
							Id = DistinguishedFolderIdName.msgfolderroot
						}
					},
					Paging = new IndexedPageView
					{
						Offset = 0,
						MaxRows = Microsoft.Exchange.Services.Global.FindCountLimit,
						Origin = BasePagingType.PagingOrigin.Beginning
					},
					RequiredFolders = WellKnownShapes.RequiredDistinguishedFolders,
					FoldersToMoveToTop = foldersToMoveToTop
				}
			};
		}

		private static FindConversationJsonRequest GetFindConversationJsonRequest(string timeZoneStr, FolderViewState folderViewState, bool shouldUseRenewTime, bool isClutterFilteringEnabled)
		{
			FindConversationJsonRequest findConversationJsonRequest = new FindConversationJsonRequest();
			findConversationJsonRequest.Header = SessionDataHandler.GetHeaders(timeZoneStr);
			findConversationJsonRequest.Body = new FindConversationRequest();
			findConversationJsonRequest.Body.ConversationShape = new ConversationResponseShape(ShapeEnum.IdOnly, new PropertyPath[0]);
			if (folderViewState.SortColumn == FolderViewColumnId.To)
			{
				findConversationJsonRequest.Body.ShapeName = WellKnownShapeName.ConversationUberListView.ToString();
			}
			else
			{
				findConversationJsonRequest.Body.ShapeName = (isClutterFilteringEnabled ? WellKnownShapeName.InferenceConversationListView.ToString() : WellKnownShapeName.ConversationListView.ToString());
			}
			findConversationJsonRequest.Body.ParentFolderId = new TargetFolderId
			{
				BaseFolderId = new DistinguishedFolderId
				{
					Id = DistinguishedFolderIdName.inbox
				}
			};
			findConversationJsonRequest.Body.Paging = new IndexedPageView
			{
				Offset = 0,
				MaxRows = 25,
				Origin = BasePagingType.PagingOrigin.Beginning
			};
			if (shouldUseRenewTime && folderViewState.SortColumn == FolderViewColumnId.DateTime)
			{
				findConversationJsonRequest.Body.SortOrder = new SortResults[2];
				findConversationJsonRequest.Body.SortOrder[0] = new SortResults();
				findConversationJsonRequest.Body.SortOrder[0].SortByProperty = new PropertyUri(PropertyUriEnum.ConversationLastDeliveryOrRenewTime);
				findConversationJsonRequest.Body.SortOrder[0].Order = (SortDirection)folderViewState.SortOrder;
				findConversationJsonRequest.Body.SortOrder[1] = new SortResults();
				findConversationJsonRequest.Body.SortOrder[1].SortByProperty = new PropertyUri(PropertyUriEnum.ConversationLastDeliveryTime);
				findConversationJsonRequest.Body.SortOrder[1].Order = (SortDirection)folderViewState.SortOrder;
			}
			else
			{
				findConversationJsonRequest.Body.SortOrder = SortResults.FromXsoSortBy(folderViewState.ConvertToSortBy());
			}
			if (isClutterFilteringEnabled)
			{
				findConversationJsonRequest.Body.ClutterFilter = ClutterFilter.NoClutter;
			}
			return findConversationJsonRequest;
		}

		private static FindItemJsonRequest GetFindItemJsonRequest(string timeZoneStr, FolderViewState folderViewState, bool shouldUseRenewTime, bool isClutterFilteringEnabled)
		{
			FindItemJsonRequest findItemJsonRequest = new FindItemJsonRequest();
			findItemJsonRequest.Header = SessionDataHandler.GetHeaders(timeZoneStr);
			findItemJsonRequest.Body = new FindItemRequest();
			findItemJsonRequest.Body.ItemShape = new ItemResponseShape();
			findItemJsonRequest.Body.ItemShape.BaseShape = ShapeEnum.IdOnly;
			findItemJsonRequest.Body.ShapeName = WellKnownShapeName.MailListItem.ToString();
			findItemJsonRequest.Body.ParentFolderIds = new BaseFolderId[]
			{
				new DistinguishedFolderId
				{
					Id = DistinguishedFolderIdName.inbox
				}
			};
			findItemJsonRequest.Body.Paging = new IndexedPageView
			{
				Offset = 0,
				MaxRows = 25,
				Origin = BasePagingType.PagingOrigin.Beginning
			};
			if (shouldUseRenewTime && folderViewState.SortColumn == FolderViewColumnId.DateTime)
			{
				findItemJsonRequest.Body.SortOrder = new SortResults[2];
				findItemJsonRequest.Body.SortOrder[0] = new SortResults();
				findItemJsonRequest.Body.SortOrder[0].SortByProperty = new PropertyUri(PropertyUriEnum.ReceivedOrRenewTime);
				findItemJsonRequest.Body.SortOrder[0].Order = (SortDirection)folderViewState.SortOrder;
				findItemJsonRequest.Body.SortOrder[1] = new SortResults();
				findItemJsonRequest.Body.SortOrder[1].SortByProperty = new PropertyUri(PropertyUriEnum.DateTimeReceived);
				findItemJsonRequest.Body.SortOrder[1].Order = (SortDirection)folderViewState.SortOrder;
			}
			else
			{
				findItemJsonRequest.Body.SortOrder = SortResults.FromXsoSortBy(folderViewState.ConvertToSortBy());
			}
			if (isClutterFilteringEnabled)
			{
				findItemJsonRequest.Body.ClutterFilter = ClutterFilter.NoClutter;
			}
			return findItemJsonRequest;
		}

		private static GetItemJsonRequest GetGetItemJsonRequest(string timeZoneStr, OwaUserConfiguration owaUserConfiguration, LayoutType layout, ItemId itemId, bool includeCssScopeName, bool useSafeHtml)
		{
			GetItemJsonRequest getItemJsonRequest = new GetItemJsonRequest();
			getItemJsonRequest.Header = SessionDataHandler.GetHeaders(timeZoneStr);
			getItemJsonRequest.Body = new GetItemRequest();
			getItemJsonRequest.Body.ShapeName = WellKnownShapeName.ItemNormalizedBody.ToString();
			getItemJsonRequest.Body.ItemShape = new ItemResponseShape();
			getItemJsonRequest.Body.ItemShape.BaseShape = ShapeEnum.IdOnly;
			WellKnownShapes.SetDefaultsOnItemResponseShape(getItemJsonRequest.Body.ItemShape, layout, owaUserConfiguration);
			if (includeCssScopeName || useSafeHtml)
			{
				getItemJsonRequest.Body.ItemShape.CssScopeClassName = WellKnownShapes.GenerateRandomCssScopeName();
			}
			getItemJsonRequest.Body.ItemShape.UseSafeHtml = useSafeHtml;
			getItemJsonRequest.Body.Ids = new BaseItemId[]
			{
				itemId
			};
			return getItemJsonRequest;
		}

		private static GetConversationItemsJsonRequest GetGetConversationItemsJsonRequest(string timeZoneStr, OwaUserConfiguration owaUserConfiguration, LayoutType layout, ItemId conversationId, bool includeCssScopeClassName, bool useSafeHtml)
		{
			GetConversationItemsJsonRequest getConversationItemsJsonRequest = new GetConversationItemsJsonRequest();
			getConversationItemsJsonRequest.Header = SessionDataHandler.GetHeaders(timeZoneStr);
			getConversationItemsJsonRequest.Body = new GetConversationItemsRequest();
			getConversationItemsJsonRequest.Body.ShapeName = WellKnownShapeName.ItemPartUniqueBody.ToString();
			getConversationItemsJsonRequest.Body.ItemShape = new ItemResponseShape();
			getConversationItemsJsonRequest.Body.ItemShape.BaseShape = ShapeEnum.IdOnly;
			WellKnownShapes.SetDefaultsOnItemResponseShape(getConversationItemsJsonRequest.Body.ItemShape, layout, owaUserConfiguration);
			if (includeCssScopeClassName || useSafeHtml)
			{
				getConversationItemsJsonRequest.Body.ItemShape.CssScopeClassName = WellKnownShapes.GenerateRandomCssScopeName();
			}
			getConversationItemsJsonRequest.Body.ItemShape.UseSafeHtml = useSafeHtml;
			getConversationItemsJsonRequest.Body.SortOrder = ((owaUserConfiguration.UserOptions.ConversationSortOrder == ConversationSortOrder.ChronologicalNewestOnBottom) ? ConversationNodeSortOrder.DateOrderAscending : ConversationNodeSortOrder.DateOrderDescending);
			getConversationItemsJsonRequest.Body.MailboxScope = MailboxSearchLocation.PrimaryOnly;
			getConversationItemsJsonRequest.Body.MaxItemsToReturn = 20;
			getConversationItemsJsonRequest.Body.Conversations = new ConversationRequestType[]
			{
				new ConversationRequestType
				{
					ConversationId = conversationId
				}
			};
			return getConversationItemsJsonRequest;
		}

		private void InternalCleanUp(HttpContext context, CallContext callContext, RequestDetailsLogger requestDetailsLogger)
		{
			OwaServerTraceLogger.SaveTraces();
			CallContext.SetCurrent(null);
			if (callContext != null)
			{
				callContext.Dispose();
				context.Items.Remove("CallContext");
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(this.requestContext.HttpContext, this.sessionDataRequestDetailsLogger);
			HttpCallContext.Singleton.LocalId = new Guid?(((ActivityScope)this.sessionDataRequestDetailsLogger.ActivityScope).ActivityScopeImpl.LocalId);
			if (requestDetailsLogger != null)
			{
				requestDetailsLogger.Dispose();
			}
		}

		private static void DetachCallContextAndLoggerFromRequest(HttpContext context, CallContext callContext)
		{
			CallContext.SetCurrent(null);
			if (callContext != null)
			{
				context.Items.Remove("CallContext");
				callContext.IsDetachedFromRequest = true;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(context, null);
		}

		private static bool IsClutterFilteringEnabled(OwaUserConfiguration owaUserConfiguration, FeaturesManager featuresManager)
		{
			return owaUserConfiguration != null && owaUserConfiguration.SegmentationSettings.PredictedActions && owaUserConfiguration.UserOptions.ShowInferenceUiElements && featuresManager != null && featuresManager.ConfigurationSnapshot.OwaServer.InferenceUI.Enabled;
		}

		private bool Initialize(HttpResponse response)
		{
			response.ContentType = "application/x-javascript";
			response.Cache.SetNoServerCaching();
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.Cache.SetNoStore();
			return true;
		}

		private IEnumerator<int> StartRequestProcessing(HttpContext context, bool shouldContinue, AsyncEnumerator ae)
		{
			if (!shouldContinue)
			{
				ae.End();
			}
			else
			{
				RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.SessionDataProcessingBegin);
				context.Request.Headers.Add("X-OWA-ActionName", "SessionData");
				RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetOwaUserContextBegin);
				UserContext userContext = UserContextManager.GetUserContext(context);
				RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetOwaUserContextEnd);
				string userContextKey = string.Empty;
				if (userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ServerSettings.OwaMailboxSessionCloning.Enabled)
				{
					userContextKey = userContext.Key.ToString();
					SessionDataHandler.TryPreLoadMailboxSession(userContext);
					RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.TryPreLoadMailboxSessionEnd);
				}
				Message message = Message.CreateMessage(MessageVersion.Default, "GetOwaUserConfiguration");
				message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
				RequestDetailsLogger requestDetailsLogger = this.InitializeAndSetRequestDetailsLogger("SessionData_GetOwaUserConfiguration");
				CallContext callContext = this.CreateAndSetCallContext(message, userContextKey, userContext.FeaturesManager);
				OWAService owaService = new OWAService();
				OwaUserConfiguration owaUserConfiguration;
				try
				{
					RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetOwaUserConfigurationBegin);
					owaUserConfiguration = owaService.GetOwaUserConfiguration();
					this.AddUserConfigurationData(requestDetailsLogger, owaUserConfiguration);
				}
				finally
				{
					this.InternalCleanUp(context, callContext, requestDetailsLogger);
				}
				string timeZoneStr = owaUserConfiguration.UserOptions.TimeZone;
				EWSSettings.RequestTimeZone = SessionDataHandler.GetTimeZoneHeader(timeZoneStr).TimeZoneDefinition.ExTimeZone;
				this.WriteOutput("{");
				SessionDataHandler.WriteJsonVariable<OwaUserConfiguration>(this.OutputStream, this.Encoding, "owaUserConfig", owaUserConfiguration, false);
				RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetOwaUserConfigurationEnd);
				bool isMowa = OfflineClientRequestUtilities.IsRequestFromMOWAClient(callContext.HttpContext.Request, callContext.HttpContext.Request.UserAgent);
				bool mowaEnabled = callContext.AccessingPrincipal != null && callContext.AccessingPrincipal.MailboxInfo.Configuration.IsMowaEnabled;
				if (isMowa && !mowaEnabled)
				{
					callContext.Dispose();
					requestDetailsLogger.Dispose();
					this.WriteOutput("}");
					ae.End();
				}
				else
				{
					bool shouldFetchFirstMessage = true;
					DistinguishedFolderIdName[] foldersToMoveToTop = null;
					bool shouldUseRenewTime = false;
					if (userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ClientServerSettings.SuperStart.Enabled)
					{
						shouldFetchFirstMessage = owaUserConfiguration.UserOptions.ShowReadingPaneOnFirstLoad;
						foldersToMoveToTop = WellKnownShapes.FoldersToMoveToTop;
					}
					bool isOffline = OfflineClientRequestUtilities.IsRequestFromOfflineClient(callContext.HttpContext.Request);
					string disableSuperSort = RequestDispatcherUtilities.GetStringUrlParameter(context, "disableSuperSort");
					if (this.pageContext.UserAgent.Layout == LayoutType.Mouse && !isOffline && disableSuperSort != "true" && userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ClientServerSettings.SuperSort.Enabled)
					{
						shouldUseRenewTime = true;
					}
					if (!isOffline)
					{
						try
						{
							context.Request.Headers.Add("X-OWA-ActionName", "SessionData");
							message = Message.CreateMessage(MessageVersion.Default, "FindFolder", SessionDataHandler.GetFindFolderJsonRequest(timeZoneStr, foldersToMoveToTop));
							message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
							requestDetailsLogger = this.InitializeAndSetRequestDetailsLogger("SessionData_FindFolders");
							callContext = this.CreateAndSetCallContext(message, userContextKey, userContext.FeaturesManager);
							OWAService findFolderRequestService = new OWAService();
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.FindFoldersBegin);
							IAsyncResult findFolderAsyncResult = ae.AddAsync<IAsyncResult>(findFolderRequestService.BeginFindFolder(SessionDataHandler.GetFindFolderJsonRequest(timeZoneStr, foldersToMoveToTop), ae.GetAsyncCallback(), null));
							CallContext.SetCurrent(null);
							yield return 1;
							CallContext.SetCurrent(callContext);
							FindFolderJsonResponse ffJsonResponse = findFolderRequestService.EndFindFolder(findFolderAsyncResult);
							if (ffJsonResponse.Body.ResponseMessages.Items != null && ffJsonResponse.Body.ResponseMessages.Items.Length > 0 && ffJsonResponse.Body.ResponseMessages.Items[0] != null && ffJsonResponse.Body.ResponseMessages.Items[0].ResponseClass == ResponseClass.Success && ffJsonResponse.Body.ResponseMessages.Items[0].ResponseCode == ResponseCodeType.NoError)
							{
								FindFolderResponseMessage findFolderResponseMessage = (FindFolderResponseMessage)ffJsonResponse.Body.ResponseMessages.Items[0];
								int totalItemsInView = findFolderResponseMessage.RootFolder.TotalItemsInView;
								int? childFolderCount = findFolderResponseMessage.RootFolder.ParentFolder.ChildFolderCount;
								requestDetailsLogger.Set(FindFoldersMetadata.TotalFolderCount, totalItemsInView);
								requestDetailsLogger.Set(FindFoldersMetadata.TopLevelChildFolderCount, childFolderCount);
							}
							SessionDataHandler.WriteJsonVariable<FindFolderJsonResponse>(this.OutputStream, this.Encoding, "findFolders", ffJsonResponse, true);
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.FindFoldersEnd);
						}
						finally
						{
							this.InternalCleanUp(context, callContext, requestDetailsLogger);
						}
					}
					FolderViewState folderViewState = this.GetInboxFolderViewState(owaUserConfiguration, this.pageContext.UserAgent);
					IAsyncResult findConversationOrItemAsyncResult = null;
					OWAService findConversationOrItemRequestService = null;
					HttpContext.Current = this.requestContext.HttpContext;
					ItemId firstItemOrConversationId = null;
					string firstItemOrConversationLastModifiedTime = null;
					bool isClutterFilteringEnabled = SessionDataHandler.IsClutterFilteringEnabled(owaUserConfiguration, userContext.FeaturesManager);
					if (folderViewState.View == FolderViewType.ConversationView)
					{
						try
						{
							this.bootWithConversationView = true;
							EWSSettings.RequestTimeZone = SessionDataHandler.GetTimeZoneHeader(timeZoneStr).TimeZoneDefinition.ExTimeZone;
							context.Request.Headers.Add("X-OWA-ActionName", "SessionData");
							message = Message.CreateMessage(MessageVersion.Default, "FindConversation", SessionDataHandler.GetFindConversationJsonRequest(timeZoneStr, folderViewState, shouldUseRenewTime, isClutterFilteringEnabled));
							message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
							requestDetailsLogger = this.InitializeAndSetRequestDetailsLogger("SessionData_FindConversation");
							callContext = this.CreateAndSetCallContext(message, userContextKey, userContext.FeaturesManager);
							if (owaUserConfiguration.SegmentationSettings.PredictedActions && owaUserConfiguration.UserOptions.ShowInferenceUiElements)
							{
								callContext.OwaCallback = NoOpOwaCallback.Prototype;
							}
							findConversationOrItemRequestService = new OWAService();
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.FindConversationOrItemBegin);
							findConversationOrItemAsyncResult = ae.AddAsync<IAsyncResult>(findConversationOrItemRequestService.BeginFindConversation(SessionDataHandler.GetFindConversationJsonRequest(timeZoneStr, folderViewState, shouldUseRenewTime, isClutterFilteringEnabled), ae.GetAsyncCallback(), null));
							CallContext.SetCurrent(null);
							yield return 1;
							CallContext.SetCurrent(callContext);
							FindConversationJsonResponse jsonResponse = findConversationOrItemRequestService.EndFindConversation(findConversationOrItemAsyncResult);
							if (shouldFetchFirstMessage)
							{
								ConversationType firstConversationInFolder = SessionDataHandler.GetFirstConversationInFolder(jsonResponse);
								if (firstConversationInFolder != null)
								{
									firstItemOrConversationId = firstConversationInFolder.ConversationId;
									firstItemOrConversationLastModifiedTime = firstConversationInFolder.LastModifiedTime;
								}
							}
							SessionDataHandler.WriteJsonVariable<FindConversationJsonResponse>(this.OutputStream, this.Encoding, "findConversation", jsonResponse, true);
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.FindConversationOrItemEnd);
						}
						finally
						{
							this.InternalCleanUp(context, callContext, requestDetailsLogger);
						}
					}
					else
					{
						try
						{
							this.bootWithConversationView = false;
							EWSSettings.RequestTimeZone = SessionDataHandler.GetTimeZoneHeader(timeZoneStr).TimeZoneDefinition.ExTimeZone;
							context.Request.Headers.Add("X-OWA-ActionName", "SessionData");
							message = Message.CreateMessage(MessageVersion.Default, "FindItem", SessionDataHandler.GetFindItemJsonRequest(timeZoneStr, folderViewState, shouldUseRenewTime, isClutterFilteringEnabled));
							message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
							requestDetailsLogger = this.InitializeAndSetRequestDetailsLogger("SessionData_FindItem");
							callContext = this.CreateAndSetCallContext(message, userContextKey, userContext.FeaturesManager);
							if (isClutterFilteringEnabled)
							{
								callContext.OwaCallback = new NoOpOwaCallback();
							}
							findConversationOrItemRequestService = new OWAService();
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.FindConversationOrItemBegin);
							findConversationOrItemAsyncResult = ae.AddAsync<IAsyncResult>(findConversationOrItemRequestService.BeginFindItem(SessionDataHandler.GetFindItemJsonRequest(timeZoneStr, folderViewState, shouldUseRenewTime, isClutterFilteringEnabled), ae.GetAsyncCallback(), null));
							CallContext.SetCurrent(null);
							yield return 1;
							CallContext.SetCurrent(callContext);
							FindItemJsonResponse jsonResponse2 = findConversationOrItemRequestService.EndFindItem(findConversationOrItemAsyncResult);
							if (shouldFetchFirstMessage)
							{
								firstItemOrConversationId = SessionDataHandler.GetFirstItemIdInFolder(jsonResponse2);
							}
							SessionDataHandler.WriteJsonVariable<FindItemJsonResponse>(this.OutputStream, this.Encoding, "findMailFolderItem", jsonResponse2, true);
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.FindConversationOrItemEnd);
						}
						finally
						{
							this.InternalCleanUp(context, callContext, requestDetailsLogger);
						}
					}
					HttpContext.Current = this.requestContext.HttpContext;
					int timeoutInMilliseconds = RequestDispatcherUtilities.GetIntUrlParameter(context, "PageTimeout", 1000);
					bool inModernMailFlight = userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ClientServerSettings.ModernMail.Enabled;
					bool disableScopedStyles = RequestDispatcherUtilities.GetIntUrlParameter(context, "DisableScopedStyles", 0) != 0;
					bool inSuperNormalFlight = userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ClientServerSettings.SuperNormal.Enabled && !disableScopedStyles;
					bool inSafeHtmlFlight = userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ClientServerSettings.SafeHtml.Enabled;
					if (!isOffline && !inModernMailFlight && firstItemOrConversationId != null && timeoutInMilliseconds > 0 && shouldFetchFirstMessage)
					{
						bool flag = false;
						HttpContext.Current = this.requestContext.HttpContext;
						if (folderViewState.View == FolderViewType.ConversationView)
						{
							try
							{
								EWSSettings.RequestTimeZone = SessionDataHandler.GetTimeZoneHeader(timeZoneStr).TimeZoneDefinition.ExTimeZone;
								GetConversationItemsJsonRequest getConversationItemsJsonRequest = SessionDataHandler.GetGetConversationItemsJsonRequest(timeZoneStr, owaUserConfiguration, this.pageContext.UserAgent.Layout, firstItemOrConversationId, inSuperNormalFlight, inSafeHtmlFlight);
								context.Request.Headers.Add("X-OWA-ActionName", "SessionData");
								message = Message.CreateMessage(MessageVersion.Default, "GetConversationItems", getConversationItemsJsonRequest);
								message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
								requestDetailsLogger = this.InitializeAndSetRequestDetailsLogger("SessionData_GetConversationItems");
								callContext = this.CreateAndSetCallContext(message, userContextKey, userContext.FeaturesManager);
								OWAService owaservice = new OWAService();
								RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetConversationItemsOrItemBegin);
								IAsyncResult asyncResult = owaservice.BeginGetConversationItems(getConversationItemsJsonRequest, null, null);
								asyncResult.AsyncWaitHandle.WaitOne(timeoutInMilliseconds);
								if (asyncResult.IsCompleted)
								{
									CallContext.SetCurrent(callContext);
									GetConversationItemsJsonResponse getConversationItemsJsonResponse = owaservice.EndGetConversationItems(asyncResult);
									ConversationResponseType firstConversationResponse = SessionDataHandler.GetFirstConversationResponse(getConversationItemsJsonResponse);
									if (firstConversationResponse != null)
									{
										firstConversationResponse.LastModifiedTime = firstItemOrConversationLastModifiedTime;
									}
									SessionDataHandler.WriteJsonVariable<GetConversationItemsJsonResponse>(this.OutputStream, this.Encoding, "getConversationItems", getConversationItemsJsonResponse, true);
									RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetConversationItemsOrItemEnd);
								}
								else
								{
									flag = true;
									SessionDataHandler.DetachCallContextAndLoggerFromRequest(context, callContext);
									SessionDataHandler.ExecuteActionOnMethodCompletion(asyncResult, delegate
									{
										this.InternalCleanUp(context, callContext, requestDetailsLogger);
									});
								}
								goto IL_1204;
							}
							finally
							{
								if (!flag)
								{
									this.InternalCleanUp(context, callContext, requestDetailsLogger);
								}
							}
						}
						try
						{
							EWSSettings.RequestTimeZone = SessionDataHandler.GetTimeZoneHeader(timeZoneStr).TimeZoneDefinition.ExTimeZone;
							GetItemJsonRequest getItemJsonRequest = SessionDataHandler.GetGetItemJsonRequest(timeZoneStr, owaUserConfiguration, this.pageContext.UserAgent.Layout, firstItemOrConversationId, inSuperNormalFlight, inSafeHtmlFlight);
							context.Request.Headers.Add("X-OWA-ActionName", "SessionData");
							message = Message.CreateMessage(MessageVersion.Default, "GetItem", getItemJsonRequest);
							message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
							requestDetailsLogger = this.InitializeAndSetRequestDetailsLogger("SessionData_GetItem");
							callContext = this.CreateAndSetCallContext(message, userContextKey, userContext.FeaturesManager);
							OWAService owaservice = new OWAService();
							RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetConversationItemsOrItemBegin);
							IAsyncResult asyncResult = owaservice.BeginGetItem(getItemJsonRequest, null, null);
							bool flag2 = asyncResult.AsyncWaitHandle.WaitOne(timeoutInMilliseconds);
							if (flag2 && asyncResult.IsCompleted)
							{
								CallContext.SetCurrent(callContext);
								SessionDataHandler.WriteJsonVariable<GetItemJsonResponse>(this.OutputStream, this.Encoding, "getMailFolderItem", owaservice.EndGetItem(asyncResult), true);
								RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.GetConversationItemsOrItemEnd);
							}
							else
							{
								flag = true;
								SessionDataHandler.DetachCallContextAndLoggerFromRequest(context, callContext);
								SessionDataHandler.ExecuteActionOnMethodCompletion(asyncResult, delegate
								{
									this.InternalCleanUp(context, callContext, requestDetailsLogger);
								});
							}
						}
						finally
						{
							if (!flag)
							{
								this.InternalCleanUp(context, callContext, requestDetailsLogger);
							}
						}
					}
					IL_1204:
					this.WriteOutput("}");
					RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.SessionDataProcessingEnd);
					ae.End();
				}
			}
			yield break;
		}

		private void SendCompositeUserSpecificInjector(HttpContext context)
		{
			this.compositeSDRequest = "1".Equals(context.Request.QueryString["crr"]);
			this.compositeSDRequestRetryAttempt = "1".Equals(context.Request.QueryString["crrretry"]);
			if (this.compositeSDRequest)
			{
				if (!this.compositeSDRequestRetryAttempt)
				{
					this.ContentType = "application/octet-stream";
					this.BufferOutput = false;
				}
				new StringBuilder(1024);
				this.WriteOutput("SESSION DATA\r\n");
				this.WriteOutput("Version: 1");
				this.WriteOutput("\r\n\r\n");
				IUserContext userContext = this.requestContext.UserContext as IUserContext;
				string currentOwaVersion = userContext.CurrentOwaVersion;
				StringBuilder usr = new StringBuilder(1024);
				UserSpecificResourceInjectorBase.WriteScriptBlock(delegate(string s)
				{
					usr.Append(s);
				}, this.pageContext, userContext, UserResourcesFinder.GetUserDataEmbeddedLinks(this.pageContext.ManifestType, this.pageContext.UserAgent.Layout, currentOwaVersion), currentOwaVersion);
				this.WriteOutput("Content-Name: UserSpecificResources\r\n");
				this.WriteOutput("Content-Size: " + (usr.Length + "\r\n\r\n".Length).ToString());
				this.WriteOutput("\r\n\r\n");
				this.WriteOutput(usr.ToString());
				this.WriteOutput("\r\n\r\n");
				if (!this.compositeSDRequestRetryAttempt)
				{
					this.Flush();
				}
				this.WriteOutput("Content-Name: SessionData");
				this.WriteOutput("\r\n\r\n");
			}
		}

		private static void TryPreLoadMailboxSession(UserContext userContext)
		{
			if (!userContext.IsMailboxSessionCreated && !userContext.MailboxSessionLockedByCurrentThread())
			{
				try
				{
					userContext.LockAndReconnectMailboxSession(100);
				}
				catch (OwaLockTimeoutException ex)
				{
					ExTraceGlobals.SessionDataHandlerTracer.TraceError(0L, string.Format("Exception while Preloading Mailbox Session, UserContextKey: {0}, Exception: {1}", userContext.Key.ToString(), ex.Message));
				}
				finally
				{
					if (userContext.MailboxSessionLockedByCurrentThread())
					{
						userContext.UnlockAndDisconnectMailboxSession();
					}
				}
			}
		}

		private static void ExecuteActionOnMethodCompletion(IAsyncResult methodAsyncResult, Action action)
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				try
				{
					if (!methodAsyncResult.IsCompleted)
					{
						methodAsyncResult.AsyncWaitHandle.WaitOne();
					}
				}
				finally
				{
					action();
				}
			});
		}

		private static ConversationType GetFirstConversationInFolder(FindConversationJsonResponse jsonResponse)
		{
			if (jsonResponse.Body.ResponseClass == ResponseClass.Success && jsonResponse.Body.ResponseCode == ResponseCodeType.NoError && jsonResponse.Body.Conversations.Length > 0 && jsonResponse.Body.Conversations[0] != null)
			{
				return jsonResponse.Body.Conversations[0];
			}
			return null;
		}

		private static ItemId GetFirstItemIdInFolder(FindItemJsonResponse jsonResponse)
		{
			if (jsonResponse.Body.ResponseMessages.Items != null && jsonResponse.Body.ResponseMessages.Items.Length > 0 && jsonResponse.Body.ResponseMessages.Items[0] != null && jsonResponse.Body.ResponseMessages.Items[0].ResponseClass == ResponseClass.Success && jsonResponse.Body.ResponseMessages.Items[0].ResponseCode == ResponseCodeType.NoError)
			{
				FindItemResponseMessage findItemResponseMessage = jsonResponse.Body.ResponseMessages.Items[0] as FindItemResponseMessage;
				if (findItemResponseMessage != null && findItemResponseMessage.ParentFolder != null && findItemResponseMessage.ParentFolder.Items != null && findItemResponseMessage.ParentFolder.Items.Length > 0)
				{
					return findItemResponseMessage.ParentFolder.Items[0].ItemId;
				}
			}
			return null;
		}

		private static ConversationResponseType GetFirstConversationResponse(GetConversationItemsJsonResponse jsonResponse)
		{
			if (jsonResponse.Body.ResponseMessages != null && jsonResponse.Body.ResponseMessages.Items != null && jsonResponse.Body.ResponseMessages.Items.Length > 0 && jsonResponse.Body.ResponseMessages.Items[0] != null)
			{
				GetConversationItemsResponseMessage getConversationItemsResponseMessage = jsonResponse.Body.ResponseMessages.Items[0] as GetConversationItemsResponseMessage;
				if (getConversationItemsResponseMessage != null)
				{
					return getConversationItemsResponseMessage.Conversation;
				}
			}
			return null;
		}

		private FolderViewState GetInboxFolderViewState(OwaUserConfiguration owaUserConfiguration, UserAgent userAgent)
		{
			if (owaUserConfiguration != null && owaUserConfiguration.SessionSettings != null && owaUserConfiguration.SessionSettings.DefaultFolderIds != null && owaUserConfiguration.SessionSettings.DefaultFolderIds.Length > 0)
			{
				FolderId folderId = owaUserConfiguration.SessionSettings.DefaultFolderIds[5];
				if (folderId != null)
				{
					FolderViewState folderViewState = FolderViewState.FindViewState(owaUserConfiguration.ViewStateConfiguration.FolderViewState, folderId.Id);
					if (this.pageContext.UserAgent.Layout != LayoutType.Mouse && folderViewState != FolderViewState.Default)
					{
						folderViewState.SortColumn = FolderViewColumnId.DateTime;
						folderViewState.SortOrder = SortOrder.Descending;
					}
					return folderViewState;
				}
			}
			return FolderViewState.Default;
		}

		private RequestDetailsLogger InitializeAndSetRequestDetailsLogger(string message)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(HttpContext.Current, null);
			HttpCallContext.Singleton.Clear();
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
			if (this.correlationId == null)
			{
				this.correlationId = HttpUtilities.GetRequestCorrelationId(HttpContext.Current);
			}
			requestDetailsLogger.Set(OwaServerLogger.LoggerData.CorrelationId, this.correlationId);
			requestDetailsLogger.Set(ExtensibleLoggerMetadata.EventId, message);
			requestDetailsLogger.Set(OwaServerLogger.LoggerData.AppCache, this.pageContext.IsAppCacheEnabledClient);
			requestDetailsLogger.Set(OwaServerLogger.LoggerData.UserAgent, this.pageContext.UserAgent.RawString);
			OwaServerLogger.LogUserContextData(HttpContext.Current, requestDetailsLogger);
			return requestDetailsLogger;
		}

		private CallContext CreateAndSetCallContext(Message message, string userContextKey, IFeaturesManager featuresManager)
		{
			CallContext callContext = CallContextUtilities.CreateAndSetCallContext(message, WorkloadType.Owa, false, userContextKey);
			callContext.FeaturesManager = featuresManager;
			return callContext;
		}

		private void AddSessionDataLogInfo()
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.sessionDataRequestDetailsLogger, ExtensibleLoggerMetadata.EventId, this.LogEventId);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.sessionDataRequestDetailsLogger, OwaServerLogger.LoggerData.AppCache, this.pageContext.IsAppCacheEnabledClient);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.sessionDataRequestDetailsLogger, OwaServerLogger.LoggerData.UserAgent, this.pageContext.UserAgent.RawString);
			if (!this.usedSessionData)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.sessionDataRequestDetailsLogger, SessionDataMetadata.BootWithConversationView, this.bootWithConversationView);
			}
		}

		private void GetAndInitializeContextMetadata(HttpContext context)
		{
			this.requestContext = Microsoft.Exchange.Clients.Owa2.Server.Core.RequestContext.Current;
			this.sessionDataRequestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(this.requestContext.HttpContext);
			RequestDetailsLogger.LogEvent(this.sessionDataRequestDetailsLogger, SessionDataMetadata.SessionDataHandlerBegin);
			IUserContext userContext = this.requestContext.UserContext as IUserContext;
			this.pageContext = new UserSpecificResourceInjector.PageContext(context, userContext.CurrentOwaVersion);
		}

		private void AddUserConfigurationData(RequestDetailsLogger requestDetailsLogger, OwaUserConfiguration owaUserConfiguration)
		{
			if (owaUserConfiguration != null && owaUserConfiguration.UserOptions != null)
			{
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ConversationSortOrder, owaUserConfiguration.UserOptions.ConversationSortOrder.ToString());
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.GlobalReadingPanePosition, owaUserConfiguration.UserOptions.GlobalReadingPanePosition);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.HideDeletedItems, owaUserConfiguration.UserOptions.HideDeletedItems);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.IsFavoritesFolderTreeCollapsed, owaUserConfiguration.UserOptions.IsFavoritesFolderTreeCollapsed);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.IsMailRootFolderTreeCollapsed, owaUserConfiguration.UserOptions.IsMailRootFolderTreeCollapsed);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.MailFolderPaneExpanded, owaUserConfiguration.UserOptions.MailFolderPaneExpanded);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ShowPreviewTextInListView, owaUserConfiguration.UserOptions.ShowPreviewTextInListView);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ShowReadingPaneOnFirstLoad, owaUserConfiguration.UserOptions.ShowReadingPaneOnFirstLoad);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.NavigationPaneViewOption, owaUserConfiguration.UserOptions.NavigationPaneViewOption);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ShowSenderOnTopInListView, owaUserConfiguration.UserOptions.ShowSenderOnTopInListView);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.EmptyDeletedItemsOnLogoff, owaUserConfiguration.UserOptions.EmptyDeletedItemsOnLogoff);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.IsOptimizedForAccessibility, owaUserConfiguration.UserOptions.IsOptimizedForAccessibility);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.MarkAsReadDelaytime, owaUserConfiguration.UserOptions.MarkAsReadDelaytime);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.IsPeopleIKnowFolderTreeCollapsed, owaUserConfiguration.UserOptions.IsPeopleIKnowFolderTreeCollapsed);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.NextSelection, owaUserConfiguration.UserOptions.NextSelection);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.PreviewMarkAsRead, owaUserConfiguration.UserOptions.PreviewMarkAsRead);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.PrimaryNavigationCollapsed, owaUserConfiguration.UserOptions.PrimaryNavigationCollapsed);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ReportJunkSelected, owaUserConfiguration.UserOptions.ReportJunkSelected);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ShowInferenceUiElements, owaUserConfiguration.UserOptions.ShowInferenceUiElements);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ShowTreeInListView, owaUserConfiguration.UserOptions.ShowTreeInListView);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.PeopleIKnowUse, owaUserConfiguration.UserOptions.PeopleIKnowUse);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.SearchScope, owaUserConfiguration.UserOptions.SearchScope);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.GlobalFolderViewState, owaUserConfiguration.ViewStateConfiguration.GlobalFolderViewState);
				requestDetailsLogger.Set(OwaUserConfigurationLogMetadata.ArchiveFolderId, owaUserConfiguration.UserOptions.ArchiveFolderId);
			}
		}

		private void WriteOutput(string output)
		{
			byte[] bytes = this.Encoding.GetBytes(output);
			this.OutputStream.Write(bytes, 0, bytes.Length);
		}

		public const string PayloadName = "PageDataPayload";

		public const string FindFolderPayloadName = "findFolders";

		public const string FindConversationPayloadName = "findConversation";

		public const string FindItemPayloadName = "findMailFolderItem";

		public const string GetConversationItemsPayloadName = "getConversationItems";

		public const string GetItemPayloadName = "getMailFolderItem";

		public const string OwaUserConfigPayloadName = "owaUserConfig";

		public const int DefaultMailboxSessionPreloadTimeoutMs = 100;

		public const string SessionDataOverallEventId = "SessionData_Overall";

		public const string SessionDataGetOwaUserConfigurationEventId = "SessionData_GetOwaUserConfiguration";

		public const string SessionDataFindFoldersEventId = "SessionData_FindFolders";

		public const string SessionDataFindConversationEventId = "SessionData_FindConversation";

		public const string SessionDataFindItemEventId = "SessionData_FindItem";

		public const string SessionDataGetConversationItemsEventId = "SessionData_GetConversationItems";

		public const string SessionDataGetItemEventId = "SessionData_GetItem";

		public const int MaxItemRowsToFetch = 25;

		public const string PayloadNamePrefix = "PageDataPayload.";

		public const int MaxGetConvOrItemWaitTime = 1000;

		public const string SectionSeparator = "\r\n\r\n";

		private Microsoft.Exchange.Clients.Owa2.Server.Core.RequestContext requestContext;

		private string correlationId;

		private RequestDetailsLogger sessionDataRequestDetailsLogger;

		private bool bootWithConversationView;

		private IPageContext pageContext;

		private bool usedSessionData;

		private bool compositeSDRequest;

		private bool compositeSDRequestRetryAttempt;
	}
}
