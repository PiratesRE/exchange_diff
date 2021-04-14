using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands.Anonymous
{
	internal class FindItemAnonymous : BaseAnonymousCommand<FindItemJsonRequest, FindItemJsonResponse>
	{
		public FindItemAnonymous(FindItemJsonRequest request) : base(request)
		{
		}

		protected override void ValidateRequestBody()
		{
			base.Request.Body.Validate();
		}

		protected override void UpdateRequestBody(PublishedCalendar publishedFolder)
		{
			ItemResponseShape itemResponseShape = new ItemResponseShape();
			itemResponseShape.BaseShape = ShapeEnum.IdOnly;
			itemResponseShape.AdditionalProperties = new PropertyPath[]
			{
				new PropertyUri(PropertyUriEnum.ItemId),
				new PropertyUri(PropertyUriEnum.ItemParentId),
				new PropertyUri(PropertyUriEnum.Sensitivity),
				new PropertyUri(PropertyUriEnum.IsCancelled),
				new PropertyUri(PropertyUriEnum.IsMeeting),
				new PropertyUri(PropertyUriEnum.LegacyFreeBusyStatus),
				new PropertyUri(PropertyUriEnum.CalendarItemType),
				new PropertyUri(PropertyUriEnum.Start),
				new PropertyUri(PropertyUriEnum.End),
				new PropertyUri(PropertyUriEnum.IsAllDayEvent),
				new PropertyUri(PropertyUriEnum.Location),
				new PropertyUri(PropertyUriEnum.Subject)
			};
			base.Request.Body.ItemShape = itemResponseShape;
			CalendarPageView calendarPageView = (CalendarPageView)base.Request.Body.Paging;
			CalendarPageView calendarPageView2 = new CalendarPageView();
			calendarPageView2.StartDate = calendarPageView.StartDate;
			calendarPageView2.EndDate = calendarPageView.EndDate;
			base.Request.Body.Paging = calendarPageView2;
		}

		protected override FindItemJsonResponse InternalExecute(PublishedCalendar publishedFolder)
		{
			object[][] data;
			try
			{
				data = this.GetData(publishedFolder);
			}
			catch (ArgumentException exception)
			{
				return this.CreateErrorResponse(exception, ResponseCodeType.ErrorInvalidRequest);
			}
			ServiceResult<FindItemParentWrapper> result = this.CreateServiceResponse(new FindItemAnonymous.AnonymousQueryView(data, int.MaxValue), publishedFolder);
			FindItemResponse findItemResponse = new FindItemResponse();
			findItemResponse.ProcessServiceResult(result);
			return new FindItemJsonResponse
			{
				Body = findItemResponse
			};
		}

		protected override FindItemJsonResponse CreateErrorResponse(Exception exception, ResponseCodeType codeType)
		{
			base.TraceError("FindItemAnonymous:CreateErrorResponse. Exception:{0}", new object[]
			{
				exception
			});
			FindItemResponse findItemResponse = new FindItemResponse();
			ServiceError error = new ServiceError(base.GetExceptionMessage(exception), codeType, 0, ExchangeVersion.Latest);
			findItemResponse.AddResponse(new ResponseMessage(ServiceResultCode.Error, error));
			return new FindItemJsonResponse
			{
				Body = findItemResponse
			};
		}

		private object[][] GetData(PublishedCalendar publishedFolder)
		{
			CalendarPageView calendarPageView = (CalendarPageView)base.Request.Body.Paging;
			return publishedFolder.GetCalendarView(calendarPageView.StartDateEx, calendarPageView.EndDateEx, FindItemAnonymous.propertiesToFetch);
		}

		private ServiceResult<FindItemParentWrapper> CreateServiceResponse(FindItemAnonymous.AnonymousQueryView anonymousView, PublishedCalendar publishedFolder)
		{
			ServiceResult<FindItemParentWrapper> result;
			using (Folder calendarFolder = publishedFolder.GetCalendarFolder())
			{
				PropertyListForViewRowDeterminer classDeterminer = PropertyListForViewRowDeterminer.BuildForItems(base.Request.Body.ItemShape, calendarFolder);
				IdAndSession idAndSession = new IdAndSession(calendarFolder.Id, calendarFolder.Session);
				ItemType[] items = anonymousView.ConvertToItems(FindItemAnonymous.propertiesToFetch, classDeterminer, idAndSession);
				BasePageResult paging = new BasePageResult(anonymousView);
				FindItemParentWrapper value = new FindItemParentWrapper(items, paging);
				result = new ServiceResult<FindItemParentWrapper>(value);
			}
			return result;
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
			CalendarItemBaseSchema.CalendarItemType
		};

		internal class AnonymousQueryView : NormalQueryView
		{
			public AnonymousQueryView(object[][] view, int rowsToGet) : base(view, rowsToGet)
			{
			}

			protected override void ThrowIfFindCountLimitExceeded(uint viewLength)
			{
			}

			protected override void CheckClientConnection()
			{
				if (!HttpContext.Current.Response.IsClientConnected)
				{
					BailOut.SetHTTPStatusAndClose(HttpStatusCode.NoContent);
				}
			}
		}
	}
}
