using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class CalendarViewPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.NotSet;
			type = string.Empty;
			state = string.Empty;
			action = string.Empty;
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.Type = owaContext.FormsRegistryContext.Type;
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			bool flag = false;
			bool flag2 = false;
			CalendarItemBaseData calendarItemBaseData = EditCalendarItemHelper.GetUserContextData(owaContext.UserContext);
			StoreObjectId storeObjectId = QueryStringUtilities.CreateItemStoreObjectId(owaContext.UserContext.MailboxSession, owaContext.HttpContext.Request, false);
			if (calendarItemBaseData != null && calendarItemBaseData.Id != null && storeObjectId != null && !calendarItemBaseData.Id.Equals(storeObjectId))
			{
				EditCalendarItemHelper.ClearUserContextData(owaContext.UserContext);
				calendarItemBaseData = null;
			}
			if (calendarItemBaseData != null)
			{
				flag = calendarItemBaseData.IsOrganizer;
				flag2 = calendarItemBaseData.IsMeeting;
			}
			else
			{
				if (storeObjectId == null)
				{
					throw new OwaLostContextException("Lost changes since last save.");
				}
				CalendarItemBase calendarItemBase2;
				CalendarItemBase calendarItemBase = calendarItemBase2 = CalendarItemBase.Bind(owaContext.UserContext.MailboxSession, storeObjectId);
				try
				{
					flag = calendarItemBase.IsOrganizer();
					flag2 = calendarItemBase.IsMeeting;
				}
				finally
				{
					if (calendarItemBase2 != null)
					{
						((IDisposable)calendarItemBase2).Dispose();
					}
				}
			}
			if (flag2 && !flag)
			{
				preFormActionResponse.Action = "Read";
			}
			else
			{
				preFormActionResponse.Action = "Open";
			}
			string text = "ae,a,t,s";
			for (int i = 0; i < owaContext.HttpContext.Request.QueryString.Count; i++)
			{
				string text2 = owaContext.HttpContext.Request.QueryString.Keys[i];
				if (text.IndexOf(text2, StringComparison.Ordinal) == -1)
				{
					preFormActionResponse.AddParameter(text2, owaContext.HttpContext.Request.QueryString[text2]);
				}
			}
			return preFormActionResponse;
		}
	}
}
