using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DeleteItem : DeleteCommandBase<DeleteItemRequest, DeleteItemResponseMessage>
	{
		public DeleteItem(CallContext callContext, DeleteItemRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(DeleteItem.DeleteItemActionName, typeof(DeleteItemMetadata), new Type[0]);
		}

		internal override void PreExecuteCommand()
		{
			this.objectIds = base.Request.Ids.OfType<ServiceObjectId>().ToList<ServiceObjectId>();
			this.disposalType = base.Request.DeleteType;
			if (!string.IsNullOrEmpty(base.Request.SendMeetingCancellations))
			{
				this.sendMeetingCancellations = new CalendarItemOperationType.CreateOrDelete?(SendMeetingInvitations.ConvertToEnum(base.Request.SendMeetingCancellations));
			}
			if (!string.IsNullOrEmpty(base.Request.AffectedTaskOccurrences))
			{
				this.affectedTaskOccurrences = new AffectedTaskOccurrencesType?(AffectedTaskOccurrences.ConvertToEnum(base.Request.AffectedTaskOccurrences));
			}
			ServiceCommandBase.ThrowIfNullOrEmpty<ServiceObjectId>(this.objectIds, "objectIds", "DeleteItem::PreExecute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			DeleteItemResponse deleteItemResponse = new DeleteItemResponse();
			deleteItemResponse.AddResponses(base.Results);
			return deleteItemResponse;
		}

		protected override IdAndSession GetIdAndSession(ServiceObjectId objectId)
		{
			BaseItemId baseItemId = objectId as BaseItemId;
			return base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(baseItemId);
		}

		protected override StoreObject GetStoreObject(IdAndSession idAndSession)
		{
			return idAndSession.GetRootXsoItem(new PropertyDefinition[]
			{
				MessageItemSchema.Flags
			});
		}

		protected override ServiceResult<DeleteItemResponseMessage> TryExecuteDelete(StoreObject storeObject, DeleteItemFlags deleteItemFlags, IdAndSession idAndSession, out LocalizedException localizedException, bool returnNewItemIds)
		{
			DeleteItemResponseMessage deleteItemResponseMessage = new DeleteItemResponseMessage();
			if (base.Request.SuppressReadReceipts)
			{
				deleteItemFlags |= DeleteItemFlags.SuppressReadReceipt;
			}
			Task task = storeObject as Task;
			bool flag;
			if (task != null && this.affectedTaskOccurrences.Value == AffectedTaskOccurrencesType.SpecifiedOccurrenceOnly)
			{
				task.OpenAsReadWrite();
				try
				{
					task.DeleteCurrentOccurrence();
				}
				catch (InvalidOperationException)
				{
					throw new CannotDeleteTaskOccurrenceException();
				}
				task.Save(SaveMode.ResolveConflicts);
				flag = true;
				localizedException = null;
			}
			else
			{
				CalendarItemBase calendarItemBase = storeObject as CalendarItemBase;
				if (calendarItemBase != null)
				{
					calendarItemBase.OpenAsReadWrite();
					DeleteItem.TrySendCancellations(calendarItemBase, this.sendMeetingCancellations.Value);
				}
				AggregateOperationResult aggregateOperationResult = base.ExecuteDelete(storeObject, deleteItemFlags, idAndSession, returnNewItemIds);
				ExTraceGlobals.DeleteItemCallTracer.TraceDebug<OperationResult>(0L, "Delete item operation result '{0}'", aggregateOperationResult.OperationResult);
				flag = (aggregateOperationResult.OperationResult == OperationResult.Succeeded);
				localizedException = (flag ? null : aggregateOperationResult.GroupOperationResults[0].Exception);
				if (returnNewItemIds)
				{
					ItemId movedItemId = null;
					if (aggregateOperationResult != null && aggregateOperationResult.GroupOperationResults != null && aggregateOperationResult.GroupOperationResults.Length > 0 && aggregateOperationResult.GroupOperationResults[0].OperationResult == OperationResult.Succeeded && aggregateOperationResult.GroupOperationResults[0].ResultObjectIds != null && aggregateOperationResult.GroupOperationResults[0].ResultObjectIds.Count > 0)
					{
						StoreId storeItemId = IdConverter.CombineStoreObjectIdWithChangeKey(aggregateOperationResult.GroupOperationResults[0].ResultObjectIds[base.CurrentStep], aggregateOperationResult.GroupOperationResults[0].ResultChangeKeys[base.CurrentStep]);
						movedItemId = IdConverter.ConvertStoreItemIdToItemId(storeItemId, storeObject.Session);
					}
					deleteItemResponseMessage.MovedItemId = movedItemId;
				}
			}
			if (!flag)
			{
				return null;
			}
			return new ServiceResult<DeleteItemResponseMessage>(deleteItemResponseMessage);
		}

		protected override ServiceResult<DeleteItemResponseMessage> CreateEmptyResponseType()
		{
			return new ServiceResult<DeleteItemResponseMessage>(new DeleteItemResponseMessage());
		}

		protected override void ExecuteCommandValidations(StoreObject storeObject)
		{
			if (ServiceCommandBase.IsAssociated((Item)storeObject) && this.disposalType != DisposalType.HardDelete)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorInvalidOperationDisposalTypeAssociatedItem);
			}
			if (storeObject is Task)
			{
				if (this.affectedTaskOccurrences == null)
				{
					throw new AffectedTaskOccurrencesRequiredException();
				}
			}
			else if (storeObject is CalendarItemBase)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, DeleteItemMetadata.ActionType, "DeleteCalendarItem");
				if (this.sendMeetingCancellations == null)
				{
					throw new SendMeetingCancellationsRequiredException();
				}
				if (this.sendMeetingCancellations.Value != CalendarItemOperationType.CreateOrDelete.SendToNone && storeObject.Session is PublicFolderSession)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)2990730164U);
				}
			}
		}

		protected override void LogDelegateSession(string principal)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, DeleteItemMetadata.SessionType, LogonType.Delegated);
			if (!string.IsNullOrEmpty(principal))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, DeleteItemMetadata.Principal, principal);
			}
		}

		private static void TrySendCancellations(CalendarItemBase calendarItemBase, CalendarItemOperationType.CreateOrDelete sendMeetingCancellations)
		{
			switch (sendMeetingCancellations)
			{
			case CalendarItemOperationType.CreateOrDelete.SendToNone:
				return;
			case CalendarItemOperationType.CreateOrDelete.SendOnlyToAll:
				break;
			case CalendarItemOperationType.CreateOrDelete.SendToAllAndSaveCopy:
				if (!ServiceCommandBase.IsOrganizerMeeting(calendarItemBase))
				{
					return;
				}
				using (MeetingCancellation meetingCancellation = calendarItemBase.CancelMeeting(null, null))
				{
					if (meetingCancellation.Recipients.Count > 0)
					{
						meetingCancellation.Send();
					}
					return;
				}
				break;
			default:
				goto IL_89;
			}
			if (!ServiceCommandBase.IsOrganizerMeeting(calendarItemBase))
			{
				return;
			}
			using (MeetingCancellation meetingCancellation2 = calendarItemBase.CancelMeeting(null, null))
			{
				if (meetingCancellation2.Recipients.Count > 0)
				{
					meetingCancellation2.SendWithoutSavingMessage();
				}
				return;
			}
			IL_89:
			throw new CalendarExceptionInvalidAttributeValue(new PropertyUri(PropertyUriEnum.CalendarItemType));
		}

		internal override bool IgnoreObjectNotFoundError { get; set; }

		private static readonly string DeleteItemActionName = typeof(DeleteItem).Name;
	}
}
