using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class CalendarItemPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.Item;
			type = null;
			action = "Open";
			state = string.Empty;
			CalendarItemBase calendarItemBase = null;
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "id");
			UserContext userContext = owaContext.UserContext;
			OwaStoreObjectId preFormActionId = OwaStoreObjectId.CreateFromString(queryStringParameter);
			bool flag = false;
			Item item = null;
			bool flag2 = false;
			try
			{
				calendarItemBase = Utilities.GetItemForRequest<CalendarItemBase>(owaContext, out item, false, new PropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights
				});
				flag2 = (item != null);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "calendar item could not be found.");
				string queryStringParameter2 = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "gid", false);
				if (string.IsNullOrEmpty(queryStringParameter2))
				{
					throw;
				}
				byte[] globalObjectIdBytes = null;
				VersionedId versionedId = null;
				try
				{
					globalObjectIdBytes = Convert.FromBase64String(queryStringParameter2);
				}
				catch (FormatException innerException)
				{
					throw new OwaInvalidRequestException("Invalid global object id for conflicting calendar item", innerException);
				}
				string queryStringParameter3 = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "fid", false);
				OwaStoreObjectId owaStoreObjectId = null;
				if (!string.IsNullOrEmpty(queryStringParameter3))
				{
					owaStoreObjectId = OwaStoreObjectId.CreateFromString(queryStringParameter3);
				}
				else
				{
					owaStoreObjectId = userContext.CalendarFolderOwaId;
				}
				using (CalendarFolder folder = Utilities.GetFolder<CalendarFolder>(userContext, owaStoreObjectId, new PropertyDefinition[0]))
				{
					versionedId = folder.GetCalendarItemId(globalObjectIdBytes);
				}
				if (versionedId == null)
				{
					throw;
				}
				MailboxSession session = (MailboxSession)owaStoreObjectId.GetSession(userContext);
				calendarItemBase = CalendarItemBase.Bind(session, versionedId, new StorePropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights
				});
				flag = true;
			}
			finally
			{
				using (item)
				{
					using (calendarItemBase)
					{
						if (calendarItemBase != null)
						{
							if (!Utilities.IsPublic(calendarItemBase) && calendarItemBase.IsMeeting && !calendarItemBase.IsOrganizer())
							{
								type = "IPM.Schedule.Meeting.Request";
							}
							else
							{
								type = "IPM.Appointment";
								if (flag2 || !ItemUtility.UserCanEditItem(calendarItemBase) || Utilities.IsItemInExternalSharedInFolder(userContext, calendarItemBase))
								{
									action = string.Empty;
								}
							}
							if (!flag2)
							{
								if (flag)
								{
									owaContext.PreFormActionId = OwaStoreObjectId.CreateFromStoreObject(calendarItemBase);
								}
								else
								{
									owaContext.PreFormActionId = preFormActionId;
								}
							}
						}
					}
				}
			}
			return null;
		}
	}
}
