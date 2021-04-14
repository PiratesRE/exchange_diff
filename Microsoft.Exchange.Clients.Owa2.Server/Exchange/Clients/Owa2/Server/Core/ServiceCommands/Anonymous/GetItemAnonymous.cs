using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands.Anonymous
{
	internal class GetItemAnonymous : BaseAnonymousCommand<GetItemJsonRequest, GetItemJsonResponse>
	{
		public GetItemAnonymous(GetItemJsonRequest request) : base(request)
		{
		}

		protected override void UpdateRequestBody(PublishedCalendar publishedFolder)
		{
			ItemResponseShape itemResponseShape = new ItemResponseShape();
			itemResponseShape.BaseShape = ShapeEnum.IdOnly;
			List<PropertyPath> list = new List<PropertyPath>
			{
				new PropertyUri(PropertyUriEnum.ItemId),
				new PropertyUri(PropertyUriEnum.ItemParentId),
				new PropertyUri(PropertyUriEnum.Sensitivity),
				new PropertyUri(PropertyUriEnum.IsCancelled),
				new PropertyUri(PropertyUriEnum.LegacyFreeBusyStatus),
				new PropertyUri(PropertyUriEnum.CalendarItemType),
				new PropertyUri(PropertyUriEnum.Start),
				new PropertyUri(PropertyUriEnum.End),
				new PropertyUri(PropertyUriEnum.IsAllDayEvent),
				new PropertyUri(PropertyUriEnum.EnhancedLocation),
				new PropertyUri(PropertyUriEnum.Subject),
				new PropertyUri(PropertyUriEnum.Recurrence)
			};
			if (publishedFolder.DetailLevel == DetailLevelEnumType.FullDetails)
			{
				list.Add(new PropertyUri(PropertyUriEnum.Body));
			}
			itemResponseShape.AdditionalProperties = list.ToArray();
			itemResponseShape.BodyType = base.Request.Body.ItemShape.BodyType;
			WellKnownShapes.SetDefaultsOnItemResponseShape(itemResponseShape, base.Context.UserAgent.Layout, null);
			base.Request.Body.ItemShape = itemResponseShape;
		}

		protected override void ValidateRequestBody()
		{
			base.Request.Body.Validate();
		}

		protected override GetItemJsonResponse InternalExecute(PublishedCalendar publishedFolder)
		{
			base.TraceDebug("GetItemAnonymous:InternalExecute", new object[0]);
			if (publishedFolder.DetailLevel == DetailLevelEnumType.AvailabilityOnly)
			{
				return this.CreateErrorResponse(new InvalidOperationException("Item details are not allowed"), ResponseCodeType.ErrorInvalidRequest);
			}
			StoreObjectId itemId = null;
			bool flag;
			try
			{
				this.GetStoreObjectId(out itemId, out flag);
			}
			catch (StoragePermanentException exception)
			{
				return this.CreateErrorResponse(exception, ResponseCodeType.ErrorInvalidRequest);
			}
			GetItemJsonResponse result;
			try
			{
				base.TraceDebug("Get item from published folder", new object[0]);
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					CalendarItemBase item = publishedFolder.GetItem(itemId, GetItemAnonymous.propertiesToFetch);
					disposeGuard.Add<CalendarItemBase>(item);
					if (flag)
					{
						base.TraceDebug("Request was for a Recurring Master", new object[0]);
						if (!(item is CalendarItemOccurrence))
						{
							return this.CreateErrorResponse(new Exception("Invalid RecurrenceMasterId"), ResponseCodeType.ErrorInvalidRequest);
						}
						itemId = ((CalendarItemOccurrence)item).MasterId.ObjectId;
						item = publishedFolder.GetItem(itemId, GetItemAnonymous.propertiesToFetch);
						disposeGuard.Add<CalendarItemBase>(item);
					}
					EwsCalendarItemType serviceObject = this.CreateServiceObject(item);
					if (item.Sensitivity == Sensitivity.Private)
					{
						base.TraceDebug("Clear sensitive information", new object[0]);
						this.ClearSensitiveInformation(serviceObject);
					}
					result = this.CreateSuccessResponse(serviceObject);
				}
			}
			catch (ObjectNotFoundException exception2)
			{
				result = this.CreateErrorResponse(exception2, ResponseCodeType.ErrorInvalidRequest);
			}
			return result;
		}

		protected override GetItemJsonResponse CreateErrorResponse(Exception exception, ResponseCodeType codeType)
		{
			base.TraceError("GetItemAnonymous:CreateErrorResponse. Exception:{0}", new object[]
			{
				exception
			});
			GetItemResponse getItemResponse = new GetItemResponse();
			ServiceError error = new ServiceError(base.GetExceptionMessage(exception), codeType, 0, ExchangeVersion.Latest);
			getItemResponse.AddResponse(new ResponseMessage(ServiceResultCode.Error, error));
			return new GetItemJsonResponse
			{
				Body = getItemResponse
			};
		}

		private void GetStoreObjectId(out StoreObjectId objectId, out bool isRecurringMaster)
		{
			objectId = ServiceIdConverter.ConvertFromConcatenatedId(base.Request.Body.Ids[0].GetId(), BasicTypes.Item, new List<AttachmentId>(0)).ToStoreObjectId();
			isRecurringMaster = (base.Request.Body.Ids[0] is RecurringMasterItemId);
		}

		private EwsCalendarItemType CreateServiceObject(CalendarItemBase calendarItem)
		{
			EwsCalendarItemType ewsCalendarItemType = (EwsCalendarItemType)ItemType.CreateFromStoreObjectType(calendarItem.StoreObjectId.ObjectType);
			IdAndSession idAndSession = new IdAndSession(calendarItem.StoreObjectId, calendarItem.Session);
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(idAndSession.Id, idAndSession.Session, base.Request.Body.ItemShape, StaticParticipantResolver.DefaultInstance);
			toServiceObjectPropertyList.CharBuffer = new char[32768];
			toServiceObjectPropertyList.ConvertStoreObjectPropertiesToServiceObject(idAndSession, calendarItem, ewsCalendarItemType);
			if (ewsCalendarItemType.Body == null)
			{
				ewsCalendarItemType.Body = new BodyContentType
				{
					BodyType = ((base.Request.Body.ItemShape.BodyType == BodyResponseType.Text) ? BodyType.Text : BodyType.HTML),
					Value = string.Empty
				};
			}
			return ewsCalendarItemType;
		}

		private void ClearSensitiveInformation(EwsCalendarItemType serviceObject)
		{
			serviceObject.Subject = ClientStrings.PrivateAppointmentSubject.ToString(base.Context.Culture);
			serviceObject.Location = string.Empty;
			serviceObject.EnhancedLocation = null;
			if (serviceObject.Body != null)
			{
				serviceObject.Body.Value = string.Empty;
			}
		}

		private GetItemJsonResponse CreateSuccessResponse(EwsCalendarItemType serviceObject)
		{
			base.TraceDebug("create successful response", new object[0]);
			GetItemJsonResponse getItemJsonResponse = new GetItemJsonResponse();
			getItemJsonResponse.Body = new GetItemResponse();
			getItemJsonResponse.Body.AddResponse(new ItemInfoResponseMessage(ServiceResultCode.Success, null, serviceObject));
			return getItemJsonResponse;
		}

		private static readonly PropertyDefinition[] propertiesToFetch = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ParentItemId,
			ItemSchema.Subject,
			ItemSchema.Sensitivity,
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime,
			CalendarItemBaseSchema.IsAllDayEvent,
			CalendarItemBaseSchema.FreeBusyStatus,
			CalendarItemBaseSchema.Location,
			CalendarItemBaseSchema.CalendarItemType,
			CalendarItemBaseSchema.RecurrenceType,
			CalendarItemBaseSchema.RecurrencePattern
		};
	}
}
