using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class MoveItemPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("OwaContext");
			}
			applicationElement = ApplicationElement.NotSet;
			type = null;
			state = null;
			action = null;
			HttpRequest request = owaContext.HttpContext.Request;
			UserContext userContext = owaContext.UserContext;
			if (!Utilities.IsPostRequest(request))
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			string type2 = owaContext.FormsRegistryContext.Type;
			NavigationModule navigationModuleFromStoreType = MoveItemHelper.GetNavigationModuleFromStoreType(type2);
			ApplicationElement applicationElementFromStoreType = MoveItemHelper.GetApplicationElementFromStoreType(type2);
			if ((navigationModuleFromStoreType == NavigationModule.Calendar && !userContext.IsFeatureEnabled(Feature.Calendar)) || (navigationModuleFromStoreType == NavigationModule.Contacts && !userContext.IsFeatureEnabled(Feature.Contacts)))
			{
				throw new OwaSegmentationException("The " + type + " feature is disabled");
			}
			StoreObjectId[] storeObjectIdsFromForm = RequestParser.GetStoreObjectIdsFromForm(request, true);
			StoreObjectId folderIdFromQueryString = RequestParser.GetFolderIdFromQueryString(request, true);
			StoreObjectId targetFolderIdFromQueryString = RequestParser.GetTargetFolderIdFromQueryString(request, true);
			string formParameter = Utilities.GetFormParameter(request, "hidt");
			string[] array = formParameter.Split(new char[]
			{
				','
			});
			if (array.Length != storeObjectIdsFromForm.Length)
			{
				throw new OwaInvalidRequestException("The counts of the items and their types are not identical.");
			}
			ItemOperations.Result result = null;
			if (navigationModuleFromStoreType == NavigationModule.Mail && applicationElementFromStoreType == ApplicationElement.Item)
			{
				result = ItemOperations.GetNextViewItem(userContext, ItemOperations.Action.Delete, storeObjectIdsFromForm[0], folderIdFromQueryString);
			}
			if (!MoveItemPreFormAction.DoMove(targetFolderIdFromQueryString, storeObjectIdsFromForm, array, owaContext))
			{
				PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
				preFormActionResponse.ApplicationElement = ApplicationElement.Dialog;
				preFormActionResponse.Type = owaContext.FormsRegistryContext.Type;
				preFormActionResponse.Action = owaContext.FormsRegistryContext.Action;
				preFormActionResponse.AddParameter("fid", folderIdFromQueryString.ToBase64String());
				return preFormActionResponse;
			}
			if (result != null)
			{
				owaContext[OwaContextProperty.InfobarMessage] = null;
			}
			userContext.ForceNewSearch = true;
			return ItemOperations.GetPreFormActionResponse(userContext, result);
		}

		private static bool DoMove(StoreObjectId folderId, StoreObjectId[] itemIds, string[] itemClassNames, OwaContext owaContext)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("OwaContext");
			}
			UserContext userContext = owaContext.UserContext;
			Folder folder = null;
			Exception ex = null;
			try
			{
				folder = Folder.Bind(userContext.MailboxSession, folderId, new PropertyDefinition[]
				{
					StoreObjectSchema.ContainerClass
				});
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				InfobarMessage.PutExceptionInfoIntoContextInfobarMessage(ex, owaContext);
				((InfobarMessage)owaContext[OwaContextProperty.InfobarMessage]).IsActionResult = true;
				return false;
			}
			bool flag = true;
			if (folder.ClassName != null)
			{
				for (int i = 0; i < itemClassNames.Length; i++)
				{
					if (!MoveItemPreFormAction.IsMoveAllowed(itemClassNames[i], folder.ClassName))
					{
						flag = false;
						break;
					}
				}
			}
			folder.Dispose();
			if (!flag)
			{
				MoveItemPreFormAction.AddInfobarMessageToContext(432946224, InfobarMessageType.Error, owaContext);
				return false;
			}
			AggregateOperationResult aggregateOperationResult = userContext.MailboxSession.Move(folderId, itemIds);
			OperationResult operationResult = aggregateOperationResult.OperationResult;
			if (operationResult == OperationResult.Succeeded)
			{
				MoveItemPreFormAction.AddInfobarMessageToContext(-1303045509, InfobarMessageType.Informational, owaContext);
				return true;
			}
			if (operationResult == OperationResult.PartiallySucceeded)
			{
				MoveItemPreFormAction.AddInfobarMessageToContext(-324890159, InfobarMessageType.Error, owaContext);
				return false;
			}
			MoveItemPreFormAction.AddInfobarMessageToContext(-842347964, InfobarMessageType.Error, owaContext);
			return false;
		}

		private static void AddInfobarMessageToContext(Strings.IDs messageStringId, InfobarMessageType type, OwaContext owaContext)
		{
			InfobarMessage infobarMessage = InfobarMessage.CreateLocalized(messageStringId, type);
			infobarMessage.IsActionResult = true;
			owaContext[OwaContextProperty.InfobarMessage] = infobarMessage;
		}

		private static bool IsMoveAllowed(string itemType, string folderType)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException("itemType");
			}
			if (folderType == null)
			{
				throw new ArgumentNullException("folderType");
			}
			return (!ObjectClass.IsCalendarFolder(folderType) || ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(itemType)) && (!ObjectClass.IsContactsFolder(folderType) || ObjectClass.IsContact(itemType) || ObjectClass.IsDistributionList(itemType));
		}
	}
}
