using System;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.Calendar)]
	[OwaEventNamespace("CalendarView")]
	internal sealed class CalendarViewEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterEnum(typeof(CalendarViewType));
			OwaEventRegistry.RegisterEnum(typeof(ReadingPanePosition));
			OwaEventRegistry.RegisterHandler(typeof(CalendarViewEventHandler));
		}

		[OwaEventParameter("w", typeof(int))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("PersistWidth")]
		public void PersistWidth()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.PersistWidth");
			this.PersistWidthOrHeight(true);
		}

		[OwaEventParameter("h", typeof(int))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("PersistHeight")]
		public void PersistHeight()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.PersistHeight");
			this.PersistWidthOrHeight(false);
		}

		private static void MoveDates(ExDateTime[] days, CalendarViewType viewType, int direction)
		{
			if (CalendarViewType.Min == viewType)
			{
				if (days.Length == 1)
				{
					days[0] = days[0].IncrementDays(direction);
					return;
				}
				for (int i = 0; i < days.Length; i++)
				{
					days[i] = days[i].IncrementDays(7 * direction);
				}
				return;
			}
			else
			{
				if (CalendarViewType.WorkWeek == viewType || CalendarViewType.Weekly == viewType)
				{
					for (int j = 0; j < days.Length; j++)
					{
						days[j] = days[j].IncrementDays(7 * direction);
					}
					return;
				}
				if (viewType == CalendarViewType.Monthly)
				{
					days[0] = days[0].AddMonths(direction);
				}
				return;
			}
		}

		private void PersistWidthOrHeight(bool isWidth)
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				using (Folder calendarFolder = this.GetCalendarFolder(false))
				{
					FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(calendarFolder);
					try
					{
						if (isWidth)
						{
							folderViewStates.ViewWidth = (int)base.GetParameter("w");
						}
						else
						{
							folderViewStates.ViewHeight = (int)base.GetParameter("h");
						}
						folderViewStates.Save();
					}
					catch (ArgumentOutOfRangeException ex)
					{
						throw new OwaInvalidRequestException(ex.Message, ex);
					}
				}
			}
		}

		[OwaEvent("PersistReadingPane")]
		[OwaEventParameter("s", typeof(ReadingPanePosition))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("isMD", typeof(bool))]
		public void PersistReadingPane()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.PersistReadingPane");
			using (Folder calendarFolder = this.GetCalendarFolder(false))
			{
				if (Utilities.IsPublic(calendarFolder) || Utilities.IsOtherMailbox(calendarFolder) || Utilities.CanModifyFolderProperties(calendarFolder))
				{
					FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(calendarFolder);
					if ((bool)base.GetParameter("isMD"))
					{
						folderViewStates.ReadingPanePositionMultiDay = (ReadingPanePosition)base.GetParameter("s");
					}
					else
					{
						folderViewStates.ReadingPanePosition = (ReadingPanePosition)base.GetParameter("s");
					}
					folderViewStates.Save();
				}
			}
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("nvs", typeof(bool), false, true)]
		[OwaEventParameter("srp", typeof(bool), false, true)]
		[OwaEventParameter("rfrshGSCalFldId", typeof(bool), false, true)]
		[OwaEventParameter("nfId", typeof(OwaStoreObjectId), true, true)]
		[OwaEventParameter("days", typeof(ExDateTime), true, true)]
		[OwaEventParameter("dir", typeof(int), false, true)]
		[OwaEvent("GetViewPayload")]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		public void GetViewPayload()
		{
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("fId");
			bool flag = base.IsParameterSet("rfrshGSCalFldId") && (bool)base.GetParameter("rfrshGSCalFldId");
			if (flag)
			{
				foreach (OwaStoreObjectId owaStoreObjectId in array)
				{
					if (owaStoreObjectId.IsGSCalendar)
					{
						ExchangePrincipal exchangePrincipal;
						base.UserContext.DelegateSessionManager.TryGetExchangePrincipal(owaStoreObjectId.MailboxOwnerLegacyDN, out exchangePrincipal);
						if (exchangePrincipal != null)
						{
							try
							{
								MailboxSession mailboxSession;
								if (Utilities.TryGetDefaultFolderId(base.UserContext, exchangePrincipal, DefaultFolderType.Calendar, out mailboxSession) == null)
								{
									mailboxSession.RefreshDefaultFolder(DefaultFolderType.Calendar);
								}
								goto IL_A6;
							}
							catch (OwaSharedFromOlderVersionException)
							{
								goto IL_A6;
							}
						}
						ExTraceGlobals.CalendarCallTracer.TraceError<string>((long)this.GetHashCode(), "Cannot find exchangePrincipal for {0}", owaStoreObjectId.MailboxOwnerLegacyDN);
					}
					IL_A6:;
				}
			}
			if (base.IsParameterSet("nfId"))
			{
				OwaStoreObjectId[] array3 = (OwaStoreObjectId[])base.GetParameter("nfId");
				if (array.Length + array3.Length > 5)
				{
					throw new OwaInvalidRequestException("Too many folders");
				}
				if (base.UserContext.IsPushNotificationsEnabled)
				{
					foreach (OwaStoreObjectId folderId in array3)
					{
						CalendarAdapter.KeepMapiNotification(base.UserContext, folderId);
					}
				}
			}
			this.RenderPayload(array);
		}

		private void RenderPayload(params OwaStoreObjectId[] folderIds)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.RenderPayload");
			ExDateTime[] array = (ExDateTime[])base.GetParameter("days");
			int num = folderIds.Length;
			if (num > 5)
			{
				throw new OwaInvalidRequestException("Too many folders");
			}
			if (num <= 0)
			{
				throw new OwaInvalidRequestException("Must pass at least one folder");
			}
			if (array == null)
			{
				array = new ExDateTime[]
				{
					DateTimeUtilities.GetLocalTime().Date
				};
			}
			if (array.Length == 0)
			{
				throw new OwaInvalidRequestException("Empty days array is not allowed");
			}
			ReadingPanePosition requestReadingPanePosition = Microsoft.Exchange.Clients.Owa.Core.ReadingPanePosition.Min;
			if (base.IsParameterSet("srp"))
			{
				requestReadingPanePosition = (((bool)base.GetParameter("srp")) ? Microsoft.Exchange.Clients.Owa.Core.ReadingPanePosition.Right : Microsoft.Exchange.Clients.Owa.Core.ReadingPanePosition.Off);
			}
			bool flag = false;
			if (base.IsParameterSet("nvs"))
			{
				flag = (bool)base.GetParameter("nvs");
			}
			CalendarViewType viewType = (CalendarViewType)base.GetParameter("vt");
			using (CalendarAdapterCollection calendarAdapterCollection = new CalendarAdapterCollection(base.UserContext, folderIds, viewType))
			{
				int num2 = 0;
				if (base.IsParameterSet("dir"))
				{
					num2 = (int)base.GetParameter("dir");
				}
				if (num2 != 0)
				{
					CalendarViewEventHandler.MoveDates(array, calendarAdapterCollection.ViewType, num2);
				}
				array = CalendarUtilities.GetViewDays(base.UserContext, array, calendarAdapterCollection.ViewType, calendarAdapterCollection.PropertyFolderId, calendarAdapterCollection.FolderViewStates);
				this.ValidateDays(array, calendarAdapterCollection.ViewType, num2);
				CalendarAdapter[] adapters = calendarAdapterCollection.GetAdapters(array, true);
				CalendarViewPayloadWriter calendarViewPayloadWriter;
				if (calendarAdapterCollection.ViewType == CalendarViewType.Monthly)
				{
					calendarViewPayloadWriter = new MonthlyViewPayloadWriter(base.UserContext, this.Writer, array, adapters);
				}
				else
				{
					calendarViewPayloadWriter = new DailyViewPayloadWriter(base.UserContext, this.Writer, array, adapters);
				}
				base.ResponseContentType = OwaEventContentType.Javascript;
				calendarViewPayloadWriter.Render(calendarAdapterCollection.ViewWidth, calendarAdapterCollection.ViewType, calendarAdapterCollection.ReadingPanePosition, requestReadingPanePosition);
				if (!flag)
				{
					calendarAdapterCollection.SaveViewStates(array);
				}
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.CalendarViewsRefreshed.Increment();
			}
		}

		[OwaEventParameter("days", typeof(ExDateTime), true, true)]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		[OwaEvent("GetPublishedViewPayload", false, true)]
		[OwaEventParameter("dir", typeof(int), false, true)]
		[OwaEventParameter("tz", typeof(string), false, true)]
		public void GetPublishedViewPayload()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.GetPublishedViewPayload");
			AnonymousSessionContext anonymousSessionContext = base.SessionContext as AnonymousSessionContext;
			if (anonymousSessionContext == null)
			{
				throw new OwaInvalidRequestException("This request can only be sent to Calendar VDir");
			}
			CalendarViewType calendarViewType = (CalendarViewType)base.GetParameter("vt");
			ExDateTime[] array = (ExDateTime[])base.GetParameter("days");
			if (array == null)
			{
				array = new ExDateTime[]
				{
					DateTimeUtilities.GetLocalTime().Date
				};
			}
			int num = 0;
			if (base.IsParameterSet("dir"))
			{
				num = (int)base.GetParameter("dir");
			}
			if (num != 0)
			{
				CalendarViewEventHandler.MoveDates(array, calendarViewType, num);
			}
			this.ValidateDays(array, calendarViewType, num);
			ExTimeZone exTimeZone = null;
			if (base.IsParameterSet("tz"))
			{
				string text = (string)base.GetParameter("tz");
				if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text, out exTimeZone))
				{
					anonymousSessionContext.TimeZone = exTimeZone;
					this.HttpContext.Response.Cookies["timezone"].Value = text;
					this.HttpContext.Response.Cookies["timezone"].Expires = (DateTime)ExDateTime.Now.AddYears(1);
				}
			}
			using (PublishedCalendarAdapter publishedCalendarAdapter = new PublishedCalendarAdapter(anonymousSessionContext))
			{
				publishedCalendarAdapter.LoadData(CalendarUtilities.QueryProperties, array, calendarViewType, exTimeZone);
				CalendarViewPayloadWriter calendarViewPayloadWriter;
				if (calendarViewType == CalendarViewType.Monthly)
				{
					calendarViewPayloadWriter = new MonthlyViewPayloadWriter(base.SessionContext, this.Writer, array, new CalendarAdapterBase[]
					{
						publishedCalendarAdapter
					});
				}
				else
				{
					calendarViewPayloadWriter = new DailyViewPayloadWriter(base.SessionContext, this.Writer, array, new CalendarAdapterBase[]
					{
						publishedCalendarAdapter
					});
				}
				base.ResponseContentType = OwaEventContentType.Javascript;
				calendarViewPayloadWriter.Render(0, calendarViewType, Microsoft.Exchange.Clients.Owa.Core.ReadingPanePosition.Off, Microsoft.Exchange.Clients.Owa.Core.ReadingPanePosition.Off);
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.CalendarViewsRefreshed.Increment();
			}
		}

		private void ValidateDays(ExDateTime[] days, CalendarViewType viewType, int direction)
		{
			if (viewType != CalendarViewType.Monthly && days.Length > 7)
			{
				throw new OwaInvalidRequestException("There are too many days in the request for OWA to process.");
			}
			foreach (ExDateTime exDateTime in days)
			{
				if ((exDateTime.Year <= 1 && direction < 0) || (exDateTime.Year >= 9999 && direction > 0))
				{
					throw new OwaInvalidRequestException(string.Format("The specified date \"{0}\" is out of range", exDateTime.ToLongDateString()));
				}
			}
		}

		[OwaEventParameter("Ntfy", typeof(bool), false, true)]
		[OwaEventParameter("days", typeof(ExDateTime), true, true)]
		[OwaEventParameter("chkms", typeof(bool), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("ST", typeof(ExDateTime))]
		[OwaEventParameter("nvs", typeof(bool), false, true)]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		[OwaEventParameter("ET", typeof(ExDateTime))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("srp", typeof(bool), false, true)]
		[OwaEvent("Move")]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		public void MoveAppointment()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			OwaStoreObjectId owaStoreObjectId2 = (OwaStoreObjectId)base.GetParameter("fId");
			CalendarItemBase calendarItemBase = null;
			bool flag = false;
			if (base.IsParameterSet("chkms"))
			{
				flag = (bool)base.GetParameter("chkms");
			}
			try
			{
				calendarItemBase = Utilities.GetItem<CalendarItemBase>(base.UserContext, owaStoreObjectId, changeKey, new PropertyDefinition[]
				{
					CalendarItemBaseSchema.IsMeeting,
					CalendarItemBaseSchema.MeetingRequestWasSent,
					StoreObjectSchema.EffectiveRights
				});
				if (!ItemUtility.UserCanEditItem(calendarItemBase) || Utilities.IsItemInExternalSharedInFolder(base.UserContext, calendarItemBase))
				{
					base.ResponseContentType = OwaEventContentType.Html;
					this.Writer.Write("<div id=divOp _sOp=np></div>");
				}
				else if (flag && calendarItemBase.IsMeeting && calendarItemBase.MeetingRequestWasSent)
				{
					base.ResponseContentType = OwaEventContentType.Html;
					if (owaStoreObjectId2.IsOtherMailbox)
					{
						this.Writer.Write("<div id=divOp _sOp=sfsu></div>");
					}
					else
					{
						this.Writer.Write("<div id=divOp _sOp=su></div>");
					}
				}
				else if (flag && (owaStoreObjectId2.IsOtherMailbox || owaStoreObjectId2.IsGSCalendar))
				{
					this.Writer.Write("<div id=divOp _sOp=sfma></div>");
				}
				else
				{
					ExDateTime exDateTime = (ExDateTime)base.GetParameter("ST");
					ExDateTime exDateTime2 = (ExDateTime)base.GetParameter("ET");
					if (exDateTime > exDateTime2)
					{
						exDateTime2 = exDateTime;
					}
					calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(43167U, LastChangeAction.MoveAppointmentTime);
					calendarItemBase.StartTime = exDateTime;
					calendarItemBase.EndTime = exDateTime2;
					calendarItemBase.SetClientIntentBasedOnModifiedProperties(new ClientIntentFlags?(ClientIntentFlags.ModifiedTime));
					if (!Utilities.IsPublic(calendarItemBase) && calendarItemBase.IsMeeting && calendarItemBase.MeetingRequestWasSent)
					{
						ExTraceGlobals.CalendarTracer.TraceDebug(0L, "Sending meeting update");
						if (!calendarItemBase.IsOrganizer())
						{
							throw new OwaEventHandlerException(LocalizedStrings.GetNonEncoded(1360823576));
						}
						calendarItemBase.SendMeetingMessages(true, null, false, true, null, null);
					}
					else
					{
						ConflictResolutionResult conflictResolutionResult = calendarItemBase.Save(SaveMode.ResolveConflicts);
						if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
						{
							throw new OwaEventHandlerException("Could not save calendar item due to conflict resolution failure", LocalizedStrings.GetNonEncoded(-482397486), OwaEventHandlerErrorCode.ConflictResolution);
						}
					}
					bool flag2 = false;
					if (base.IsParameterSet("Ntfy"))
					{
						flag2 = (bool)base.GetParameter("Ntfy");
					}
					if (flag2)
					{
						calendarItemBase.Load();
						PrincipalNotificationMessage principalNotificationMessage = new PrincipalNotificationMessage(owaStoreObjectId.ToBase64String(), owaStoreObjectId2, base.UserContext, this.HttpContext, PrincipalNotificationMessage.ActionType.Move, false, calendarItemBase.IsMeeting);
						principalNotificationMessage.SendNotificationMessage();
					}
					this.RenderPayload(new OwaStoreObjectId[]
					{
						owaStoreObjectId2
					});
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug((long)this.GetHashCode(), "Calendar item could not be found");
				throw;
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
					calendarItemBase = null;
				}
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("rcr", typeof(bool))]
		[OwaEvent("CCCopyMove")]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("fC", typeof(bool))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		public void CopyOrMoveAppointment()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.CrossCalendarCopyMove");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			OwaStoreObjectId owaStoreObjectId2 = (OwaStoreObjectId)base.GetParameter("fId");
			bool flag = (bool)base.GetParameter("fC");
			StoreObjectId storeObjectId = owaStoreObjectId2.StoreObjectId;
			CalendarItemBase calendarItemBase = null;
			if (Utilities.IsELCRootFolder(owaStoreObjectId2, base.UserContext))
			{
				throw new OwaInvalidRequestException("Cannot move messages to the root ELC folder.");
			}
			if (owaStoreObjectId.IsGSCalendar)
			{
				throw new OwaInvalidRequestException("Item id cannot be GS Calendar folder id");
			}
			if (owaStoreObjectId2.IsGSCalendar)
			{
				throw new OwaInvalidRequestException("Cannot copy/move to GS Calendar");
			}
			MailboxSession mailboxSession = owaStoreObjectId2.GetSessionForFolderContent(base.UserContext) as MailboxSession;
			if (mailboxSession == null)
			{
				throw new OwaInvalidRequestException("Cannot copy/move to public folder by this function");
			}
			if (storeObjectId.ObjectType != StoreObjectType.CalendarFolder)
			{
				throw new OwaInvalidRequestException("This function only support to copy/move to calendar folder");
			}
			try
			{
				calendarItemBase = Utilities.GetItem<CalendarItemBase>(base.UserContext, owaStoreObjectId, changeKey, new PropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights
				});
				if (calendarItemBase.ParentId.Equals(storeObjectId))
				{
					this.RenderCopyMoveFail(-612884101);
				}
				else
				{
					MailboxSession mailboxSession2 = calendarItemBase.Session as MailboxSession;
					if (mailboxSession2 == null)
					{
						throw new OwaInvalidRequestException("Cannot copy/move from public folder by this function");
					}
					if (!StringComparer.OrdinalIgnoreCase.Equals(mailboxSession2.MailboxOwner.LegacyDn, mailboxSession.MailboxOwner.LegacyDn))
					{
						if (flag)
						{
							this.RenderCopyMoveFail(-1539006262);
						}
						else
						{
							this.RenderCopyMoveFail(1992576846);
						}
					}
					else
					{
						bool flag2 = Utilities.CanCreateItemInFolder(base.UserContext, owaStoreObjectId2) && !Utilities.IsExternalSharedInFolder(base.UserContext, owaStoreObjectId2);
						if (!flag)
						{
							flag2 = (flag2 && CalendarUtilities.UserCanDeleteCalendarItem(calendarItemBase) && !Utilities.IsItemInExternalSharedInFolder(base.UserContext, calendarItemBase));
						}
						if (!flag2)
						{
							ExTraceGlobals.CalendarTracer.TraceDebug(0L, "User have no rights to remove item from source folder or create new item in target folder.");
							this.Writer.Write("<div id=divOp _sOp=\"np\"></div>");
						}
						else
						{
							if (flag)
							{
								calendarItemBase.CopyToFolder(mailboxSession, storeObjectId);
							}
							else
							{
								calendarItemBase.MoveToFolder(mailboxSession, storeObjectId);
							}
							this.Writer.Write("<div id=divOp _sOp=\"rf\"></div>");
							TargetFolderMRU.AddAndGetFolders(owaStoreObjectId2, base.UserContext);
						}
					}
				}
			}
			catch (CalendarItemExistsException)
			{
				this.RenderCopyMoveFail(-95861205);
			}
			catch (FutureMeetingException)
			{
				this.RenderCopyMoveFail(((bool)base.GetParameter("rcr")) ? 225743507 : -989731968);
			}
			catch (PrimaryCalendarFolderException)
			{
				this.RenderCopyMoveFail(-812685732);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug((long)this.GetHashCode(), "Calendar item could not be found");
				throw;
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
					calendarItemBase = null;
				}
			}
		}

		private void RenderCopyMoveFail(Strings.IDs errorMessageId)
		{
			this.Writer.Write("<div id=divOp _sOp=\"copymovefail\" _sString=\"");
			this.Writer.Write(LocalizedStrings.GetHtmlEncoded(errorMessageId, base.UserContext.UserCulture));
			this.Writer.Write("\"></div>");
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("days", typeof(ExDateTime), true, true)]
		[OwaEvent("ChangeEndRecurrence")]
		[OwaEventParameter("ET", typeof(ExDateTime))]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		[OwaEventParameter("nvs", typeof(bool), false, true)]
		[OwaEventParameter("srp", typeof(bool), false, true)]
		public void ChangeEndRecurrence()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			ExDateTime endRange = (ExDateTime)base.GetParameter("ET");
			CalendarItem calendarItem = null;
			try
			{
				calendarItem = Utilities.GetItem<CalendarItem>(base.UserContext, owaStoreObjectId, changeKey, new PropertyDefinition[]
				{
					CalendarItemBaseSchema.IsMeeting,
					CalendarItemBaseSchema.MeetingRequestWasSent,
					StoreObjectSchema.EffectiveRights
				});
				if (!ItemUtility.UserCanEditItem(calendarItem) || Utilities.IsItemInExternalSharedInFolder(base.UserContext, calendarItem))
				{
					this.Writer.Write("<div id=divOp _sOp=\"np\"></div>");
				}
				else
				{
					int num = MeetingUtilities.CancelRecurrence(calendarItem, endRange);
					if (num != -2147483648)
					{
						if (calendarItem.IsMeeting && calendarItem.MeetingRequestWasSent && calendarItem.IsOrganizer())
						{
							base.ResponseContentType = OwaEventContentType.Html;
							this.Writer.Write("<div id=divOp _sOp=er></div>");
						}
						else if (0 < num)
						{
							EndDateRecurrenceRange range = new EndDateRecurrenceRange(calendarItem.Recurrence.Range.StartDate, endRange.IncrementDays(-1));
							calendarItem.Recurrence = new Recurrence(calendarItem.Recurrence.Pattern, range);
							Utilities.SaveItem(calendarItem);
							this.RenderPayload(new OwaStoreObjectId[]
							{
								(OwaStoreObjectId)base.GetParameter("fId")
							});
						}
						else
						{
							calendarItem.DeleteMeeting(DeleteItemFlags.MoveToDeletedItems);
							this.RenderPayload(new OwaStoreObjectId[]
							{
								(OwaStoreObjectId)base.GetParameter("fId")
							});
						}
					}
				}
			}
			finally
			{
				if (calendarItem != null)
				{
					calendarItem.Dispose();
					calendarItem = null;
				}
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("vt", typeof(CalendarViewType))]
		[OwaEventParameter("nvs", typeof(bool), false, true)]
		[OwaEvent("Delete")]
		[OwaEventParameter("srp", typeof(bool), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("days", typeof(ExDateTime), true, true)]
		[OwaEventParameter("Prm", typeof(bool), false, true)]
		[OwaEventParameter("Ntfy", typeof(bool), false, true)]
		[OwaEventParameter("FD", typeof(bool), false, true)]
		public void Delete()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarViewEventHandler.Delete");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			OwaStoreObjectId owaStoreObjectId2 = (OwaStoreObjectId)base.GetParameter("fId");
			Item item = null;
			CalendarItemBase calendarItemBase = null;
			bool flag = true;
			try
			{
				item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, new PropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights,
					CalendarItemBaseSchema.MeetingRequestWasSent,
					CalendarItemBaseSchema.IsOrganizer
				});
				calendarItemBase = (item as CalendarItemBase);
				if (calendarItemBase != null)
				{
					if (!CalendarUtilities.UserCanDeleteCalendarItem(calendarItemBase) || Utilities.IsItemInExternalSharedInFolder(base.UserContext, calendarItemBase))
					{
						base.ResponseContentType = OwaEventContentType.Html;
						this.Writer.Write("<div id=divOp _sOp=np></div>");
						flag = false;
					}
					else if (!base.IsParameterSet("FD") && calendarItemBase.IsMeeting && calendarItemBase.MeetingRequestWasSent && calendarItemBase.IsOrganizer())
					{
						base.ResponseContentType = OwaEventContentType.Html;
						this.Writer.Write("<div id=divOp _sOp=sc></div>");
						flag = false;
					}
					else if (!base.IsParameterSet("FD") && !calendarItemBase.IsOrganizer() && !calendarItemBase.IsCancelled && !MeetingUtilities.IsCalendarItemEndTimeInPast(calendarItemBase))
					{
						base.ResponseContentType = OwaEventContentType.Html;
						this.Writer.Write("<div id=divOp _sOp=sr></div>");
						flag = false;
					}
					else if (!base.IsParameterSet("FD") && (owaStoreObjectId2.IsOtherMailbox || owaStoreObjectId2.IsGSCalendar))
					{
						base.ResponseContentType = OwaEventContentType.Html;
						this.Writer.Write("<div id=divOp _sOp=sn></div>");
						flag = false;
					}
				}
				else if (!ItemUtility.UserCanDeleteItem(item))
				{
					base.ResponseContentType = OwaEventContentType.Html;
					this.Writer.Write("<div id=divOp _sOp=np></div>");
					flag = false;
				}
				if (flag)
				{
					bool flag2 = false;
					if (base.IsParameterSet("Ntfy"))
					{
						flag2 = (bool)base.GetParameter("Ntfy");
					}
					if (flag2)
					{
						PrincipalNotificationMessage principalNotificationMessage = new PrincipalNotificationMessage(owaStoreObjectId.ToBase64String(), owaStoreObjectId2, base.UserContext, this.HttpContext, PrincipalNotificationMessage.ActionType.Delete, false, calendarItemBase.IsMeeting);
						principalNotificationMessage.SendNotificationMessage();
					}
					bool flag3 = false;
					if (base.IsParameterSet("Prm"))
					{
						flag3 = (bool)base.GetParameter("Prm");
					}
					DeleteItemFlags deleteItemFlags = DeleteItemFlags.None;
					if (!owaStoreObjectId2.IsPublic)
					{
						if (calendarItemBase.IsOrganizer())
						{
							if (calendarItemBase.IsMeeting)
							{
								goto IL_269;
							}
							deleteItemFlags = DeleteItemFlags.CancelCalendarItem;
							MeetingCancellation meetingCancellation = null;
							try
							{
								try
								{
									calendarItemBase.OpenAsReadWrite();
									meetingCancellation = calendarItemBase.CancelMeeting(null, null);
								}
								catch (ObjectNotFoundException)
								{
								}
								catch (AccessDeniedException)
								{
								}
								goto IL_269;
							}
							finally
							{
								if (meetingCancellation != null)
								{
									meetingCancellation.Dispose();
								}
								meetingCancellation = null;
							}
						}
						deleteItemFlags = DeleteItemFlags.DeclineCalendarItemWithoutResponse;
					}
					IL_269:
					calendarItemBase.DeleteMeeting((flag3 ? DeleteItemFlags.SoftDelete : DeleteItemFlags.MoveToDeletedItems) | deleteItemFlags);
					this.RenderPayload(new OwaStoreObjectId[]
					{
						owaStoreObjectId2
					});
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug((long)this.GetHashCode(), "Calendar item could not be found.");
				throw;
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
		}

		[OwaEventParameter("Idx", typeof(int))]
		[OwaEvent("SetCalendarColor")]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		public void SetCalendarColor()
		{
			base.ThrowIfCannotActAsOwner();
			int serverColorIndex = CalendarColorManager.GetServerColorIndex((int)base.GetParameter("Idx"));
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(base.UserContext, base.UserContext.MailboxSession, NavigationNodeGroupSection.Calendar);
			NavigationNodeFolder[] array = null;
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("fId");
			if (owaStoreObjectId.IsGSCalendar)
			{
				array = navigationNodeCollection.FindGSCalendarsByLegacyDN(owaStoreObjectId.MailboxOwnerLegacyDN);
			}
			else
			{
				array = navigationNodeCollection.FindFoldersById(owaStoreObjectId.StoreObjectId);
			}
			if (array == null || (array.Length == 0 && !owaStoreObjectId.IsGSCalendar))
			{
				using (Folder calendarFolder = this.GetCalendarFolder(false))
				{
					NavigationNodeGroupType groupType = NavigationNodeGroupType.MyFoldersGroup;
					bool flag = base.UserContext.IsInOtherMailbox(calendarFolder);
					if (flag || Utilities.IsOneOfTheFolderFlagsSet(calendarFolder, new ExtendedFolderFlags[]
					{
						ExtendedFolderFlags.SharedIn,
						ExtendedFolderFlags.ExchangeCrossOrgShareFolder
					}))
					{
						groupType = NavigationNodeGroupType.SharedFoldersGroup;
					}
					array = new NavigationNodeFolder[]
					{
						navigationNodeCollection.AddFolderToDefaultGroup(base.UserContext, groupType, calendarFolder, flag)
					};
				}
			}
			if (array != null)
			{
				foreach (NavigationNodeFolder navigationNodeFolder in array)
				{
					navigationNodeFolder.NavigationNodeCalendarColor = serverColorIndex;
				}
				navigationNodeCollection.Save(base.UserContext.MailboxSession);
			}
		}

		[OwaEvent("GetShareCalendarMenu")]
		public void GetShareCalendarMenu()
		{
			new ShareCalendarContextMenu(base.UserContext).Render(this.Writer);
		}

		private Folder GetCalendarFolder(bool forContent)
		{
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			Folder result;
			if (forContent)
			{
				result = Utilities.GetFolderForContent<Folder>(base.UserContext, folderId, CalendarUtilities.RenderPayloadFolderProperties);
			}
			else
			{
				result = Utilities.GetFolder<Folder>(base.UserContext, folderId, CalendarUtilities.RenderPayloadFolderProperties);
			}
			return result;
		}

		private const int MaxDailyViewDayCount = 7;

		public const string EventNamespace = "CalendarView";

		public const string MethodPersistWidth = "PersistWidth";

		public const string MethodPersistHeight = "PersistHeight";

		public const string MethodPersistReadingPane = "PersistReadingPane";

		public const string MethodGetViewPayload = "GetViewPayload";

		public const string MethodGetPublishedViewPayload = "GetPublishedViewPayload";

		public const string MethodMove = "Move";

		public const string MethodCrossCalendarCopyMove = "CCCopyMove";

		public const string MethodChangeEndRecurrence = "ChangeEndRecurrence";

		public const string MethodDelete = "Delete";

		public const string MethodSetCalendarViewColor = "SetCalendarColor";

		public const string MethodGetShareCalendarMenu = "GetShareCalendarMenu";

		public const string IsMultiDayView = "isMD";

		public const string ReadingPanePosition = "s";

		public const string FolderId = "fId";

		public const string TimeZone = "tz";

		public const string NotifiedFolderIds = "nfId";

		public const string SourceFolderId = "srcFId";

		public const string FolderName = "fN";

		public const string Width = "w";

		public const string Height = "h";

		public const string Days = "days";

		public const string ViewType = "vt";

		public const string Dir = "dir";

		public const string Id = "Id";

		public const string IsCpy = "fC";

		public const string ChangeKey = "CK";

		public const string StartTime = "ST";

		public const string EndTime = "ET";

		public const string DontSaveViewState = "nvs";

		public const string CheckMeetingRequestWasSent = "chkms";

		public const string ShowReadingPane = "srp";

		public const string ForceDelete = "FD";

		public const string IsPermanentDelete = "Prm";

		public const string NotifyOwner = "Ntfy";

		public const string CalendarColorIndex = "Idx";

		public const string IsRecurrence = "rcr";

		public const string RefreshFolderStoreIdForGSCalendar = "rfrshGSCalFldId";
	}
}
