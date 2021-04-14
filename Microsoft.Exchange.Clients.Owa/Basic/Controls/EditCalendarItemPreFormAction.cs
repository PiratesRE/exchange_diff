using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class EditCalendarItemPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext", "owaContext is null.");
			}
			if (owaContext.HttpContext == null)
			{
				throw new ArgumentNullException("owaContext", "owaContext.HttpContext is null.");
			}
			applicationElement = ApplicationElement.NotSet;
			type = null;
			state = null;
			action = null;
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			preFormActionResponse.Type = "IPM.Appointment";
			preFormActionResponse.Action = string.Empty;
			preFormActionResponse.State = string.Empty;
			this.request = owaContext.HttpContext.Request;
			this.userContext = owaContext.UserContext;
			InfobarMessage infobarMessage = null;
			StoreObjectId storeObjectId = null;
			string changeKey = null;
			string action2 = owaContext.FormsRegistryContext.Action;
			string text;
			if (string.Equals(action2, "New", StringComparison.Ordinal))
			{
				text = "new";
			}
			else
			{
				if (!Utilities.IsPostRequest(this.request))
				{
					return this.userContext.LastClientViewState.ToPreFormActionResponse();
				}
				text = Utilities.GetFormParameter(this.request, "hidcmdpst");
				string formParameter = Utilities.GetFormParameter(this.request, "hidid", false);
				changeKey = Utilities.GetFormParameter(this.request, "hidchk", false);
				if (!string.IsNullOrEmpty(formParameter))
				{
					storeObjectId = Utilities.CreateStoreObjectId(this.userContext.MailboxSession, formParameter);
				}
			}
			StoreObjectId calendarFolderId = EditCalendarItemHelper.GetCalendarFolderId(this.request, this.userContext);
			bool syncCalendarItem = true;
			if (text.Equals("attch", StringComparison.Ordinal))
			{
				syncCalendarItem = false;
			}
			bool flag = true;
			try
			{
				EditCalendarItemHelper.CalendarItemUpdateFlags storeUpdateFlags = EditCalendarItemHelper.CalendarItemUpdateFlags.None;
				if (!text.Equals("cls", StringComparison.Ordinal))
				{
					storeUpdateFlags = EditCalendarItemHelper.GetCalendarItem(this.userContext, storeObjectId, calendarFolderId, changeKey, syncCalendarItem, out this.calendarItemBase);
				}
				switch (this.DoAction(text, storeUpdateFlags, ref preFormActionResponse, out infobarMessage))
				{
				case EditCalendarItemPreFormAction.RedirectTo.None:
					throw new OwaInvalidRequestException("Unhandled redirection.");
				case EditCalendarItemPreFormAction.RedirectTo.AddressBook:
					preFormActionResponse = EditMessageHelper.RedirectToPeoplePicker(owaContext, this.calendarItemBase, AddressBook.Mode.EditCalendar);
					EditCalendarItemHelper.CreateUserContextData(this.userContext, this.calendarItemBase);
					break;
				case EditCalendarItemPreFormAction.RedirectTo.ADPage:
					preFormActionResponse = EditMessageHelper.RedirectToRecipient(owaContext, this.calendarItemBase, AddressBook.Mode.EditCalendar);
					EditCalendarItemHelper.CreateUserContextData(this.userContext, this.calendarItemBase);
					break;
				case EditCalendarItemPreFormAction.RedirectTo.AttachmentManager:
					if (this.calendarItemBase.Id == null)
					{
						CalendarUtilities.SaveCalendarItem(this.calendarItemBase, this.userContext, out infobarMessage);
					}
					using (CalendarItemBase calendarItemBase = EditCalendarItemHelper.CreateDraft(this.userContext, null))
					{
						CalendarItemBaseData userContextData = EditCalendarItemHelper.GetUserContextData(this.userContext);
						userContextData.CopyTo(calendarItemBase);
						string text2;
						EditCalendarItemHelper.UpdateCalendarItemValues(calendarItemBase, this.userContext, this.request, out text2);
						EditCalendarItemHelper.CreateUserContextData(this.userContext, calendarItemBase);
						if (this.calendarItemBase.Id != null)
						{
							CalendarItemBaseData userContextData2 = EditCalendarItemHelper.GetUserContextData(this.userContext);
							userContextData2.Id = this.calendarItemBase.Id.ObjectId;
							userContextData2.ChangeKey = this.calendarItemBase.Id.ChangeKeyAsBase64String();
						}
					}
					if (infobarMessage == null)
					{
						this.RedirectToAttachmentManager(owaContext, preFormActionResponse);
					}
					else
					{
						owaContext.PreFormActionData = this.calendarItemBase;
						flag = false;
						this.RedirectToEdit(owaContext, infobarMessage, preFormActionResponse);
					}
					break;
				case EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView:
					EditCalendarItemHelper.ClearUserContextData(this.userContext);
					preFormActionResponse = this.userContext.LastClientViewState.ToPreFormActionResponse();
					break;
				case EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem:
					EditCalendarItemHelper.CreateUserContextData(this.userContext, this.calendarItemBase);
					owaContext.PreFormActionData = this.calendarItemBase;
					flag = false;
					this.RedirectToEdit(owaContext, infobarMessage, preFormActionResponse);
					break;
				case EditCalendarItemPreFormAction.RedirectTo.EditRecurrence:
					EditCalendarItemHelper.CreateUserContextData(this.userContext, this.calendarItemBase);
					owaContext.PreFormActionData = CalendarItemBaseData.Create(this.calendarItemBase);
					preFormActionResponse.Action = "EditRecurrence";
					if (this.calendarItemBase.IsDirty)
					{
						preFormActionResponse.AddParameter("cd", "1");
					}
					break;
				case EditCalendarItemPreFormAction.RedirectTo.SchedulingTab:
					EditCalendarItemHelper.CreateUserContextData(this.userContext, this.calendarItemBase);
					owaContext.PreFormActionData = this.calendarItemBase;
					this.RedirectToSchedulingTab(owaContext, infobarMessage, preFormActionResponse);
					flag = false;
					break;
				default:
					throw new OwaInvalidRequestException("Unhandled redirection enum value in EditCalendarItemPreFormAction.");
				}
			}
			catch
			{
				flag = true;
				EditCalendarItemHelper.ClearUserContextData(this.userContext);
				throw;
			}
			finally
			{
				if (flag && this.calendarItemBase != null)
				{
					this.calendarItemBase.Dispose();
					this.calendarItemBase = null;
				}
			}
			return preFormActionResponse;
		}

		private EditCalendarItemPreFormAction.RedirectTo DoAction(string commandPost, EditCalendarItemHelper.CalendarItemUpdateFlags storeUpdateFlags, ref PreFormActionResponse response, out InfobarMessage infobarMessage)
		{
			infobarMessage = null;
			EditCalendarItemHelper.CalendarItemUpdateFlags calendarItemUpdateFlags = EditCalendarItemHelper.CalendarItemUpdateFlags.None;
			string text = null;
			EditCalendarItemPreFormAction.CalendarTab calendarTab = (EditCalendarItemPreFormAction.CalendarTab)RequestParser.TryGetIntValueFromForm(this.request, "hidtab", 0);
			if (Utilities.IsPostRequest(this.request) && !string.Equals(commandPost, "new", StringComparison.Ordinal) && !string.Equals(commandPost, "cls", StringComparison.Ordinal))
			{
				if (calendarTab == EditCalendarItemPreFormAction.CalendarTab.Scheduling)
				{
					calendarItemUpdateFlags = EditCalendarItemHelper.UpdateImportance(this.calendarItemBase, this.request);
				}
				else
				{
					calendarItemUpdateFlags = EditCalendarItemHelper.UpdateCalendarItemValues(this.calendarItemBase, this.userContext, this.request, out text);
				}
				if (!string.IsNullOrEmpty(text))
				{
					infobarMessage = InfobarMessage.CreateText(text, InfobarMessageType.Error);
					return EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
				}
				if (this.calendarItemBase.AttendeesChanged)
				{
					calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.AttendeesChanged;
				}
			}
			EditCalendarItemHelper.CalendarItemUpdateFlags calendarItemUpdateFlags2 = calendarItemUpdateFlags | storeUpdateFlags;
			EditCalendarItemHelper.CancelRangeType cancelRangeType = EditCalendarItemHelper.CancelRangeType.None;
			ExDateTime cancelFromDateTime = ExDateTime.MinValue;
			string formParameter = Utilities.GetFormParameter(this.request, "delprompt", false);
			if (!string.IsNullOrEmpty(formParameter))
			{
				if (string.Equals(formParameter, "2", StringComparison.Ordinal))
				{
					cancelRangeType = EditCalendarItemHelper.CancelRangeType.All;
				}
				else if (string.Equals(formParameter, "1", StringComparison.Ordinal))
				{
					cancelRangeType = EditCalendarItemHelper.CancelRangeType.FromDate;
					cancelFromDateTime = CalendarUtilities.ParseDateTimeFromForm(this.request, "seldelY", "seldelM", "seldelD", null, this.userContext);
				}
				else
				{
					if (!string.Equals(formParameter, "0", StringComparison.Ordinal))
					{
						throw new OwaInvalidRequestException("Invalid cancel prompt radio button value.");
					}
					cancelRangeType = EditCalendarItemHelper.CancelRangeType.Occurrence;
				}
			}
			EditCalendarItemPreFormAction.RedirectTo result = EditCalendarItemPreFormAction.RedirectTo.None;
			if (commandPost != null)
			{
				if (<PrivateImplementationDetails>{83F8DD10-61AE-4283-B829-2F464F055E61}.$$method0x600016f-1 == null)
				{
					<PrivateImplementationDetails>{83F8DD10-61AE-4283-B829-2F464F055E61}.$$method0x600016f-1 = new Dictionary<string, int>(23)
					{
						{
							"addmrrrcp",
							0
						},
						{
							"addanrrcp",
							1
						},
						{
							"delmrrrcp",
							2
						},
						{
							"addrBook",
							3
						},
						{
							"attch",
							4
						},
						{
							"cls",
							5
						},
						{
							"chknm",
							6
						},
						{
							"delete",
							7
						},
						{
							"editseries",
							8
						},
						{
							"invite",
							9
						},
						{
							"new",
							10
						},
						{
							"pntgl",
							11
						},
						{
							"rcr",
							12
						},
						{
							"schedule",
							13
						},
						{
							"changeschedule",
							14
						},
						{
							"rmrcp",
							15
						},
						{
							"sv",
							16
						},
						{
							"saveclose",
							17
						},
						{
							"snd",
							18
						},
						{
							"sndmod",
							19
						},
						{
							"sndall",
							20
						},
						{
							"sndcancel",
							21
						},
						{
							"viewRcptWhenEdt",
							22
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{83F8DD10-61AE-4283-B829-2F464F055E61}.$$method0x600016f-1.TryGetValue(commandPost, out num))
				{
					switch (num)
					{
					case 0:
					case 1:
						this.AddOrReplaceAttendee();
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					case 2:
					{
						ResolvedRecipientDetail[] resolvedRecipientDetails = ResolvedRecipientDetail.ParseFromForm(this.request, "hidaddrcp", true);
						AutoCompleteCache autoCompleteCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
						if (autoCompleteCache != null)
						{
							autoCompleteCache.RemoveResolvedRecipients(resolvedRecipientDetails);
							autoCompleteCache.Commit(true);
						}
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					}
					case 3:
						result = EditCalendarItemPreFormAction.RedirectTo.AddressBook;
						break;
					case 4:
						result = EditCalendarItemPreFormAction.RedirectTo.AttachmentManager;
						break;
					case 5:
						EditCalendarItemHelper.ClearUserContextData(this.userContext);
						result = EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView;
						break;
					case 6:
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					case 7:
						result = this.DoDelete(out infobarMessage);
						break;
					case 8:
					{
						StoreObjectId masterStoreObjectId = CalendarUtilities.GetMasterStoreObjectId(this.calendarItemBase);
						this.calendarItemBase.Dispose();
						this.calendarItemBase = Utilities.GetItem<CalendarItemBase>(this.userContext, masterStoreObjectId, new PropertyDefinition[0]);
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					}
					case 9:
						this.calendarItemBase.IsMeeting = true;
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					case 10:
						EditCalendarItemHelper.SetCalendarItemFromQueryParams(this.calendarItemBase, this.request, this.userContext);
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					case 11:
						if (calendarTab == EditCalendarItemPreFormAction.CalendarTab.Scheduling)
						{
							result = EditCalendarItemPreFormAction.RedirectTo.SchedulingTab;
						}
						else
						{
							result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						}
						break;
					case 12:
						result = EditCalendarItemPreFormAction.RedirectTo.EditRecurrence;
						break;
					case 13:
						this.CheckUnresolvedAttendees(ref calendarItemUpdateFlags2);
						if ((calendarItemUpdateFlags2 & EditCalendarItemHelper.CalendarItemUpdateFlags.HasUnresolvedAttendees) != EditCalendarItemHelper.CalendarItemUpdateFlags.None)
						{
							infobarMessage = InfobarMessage.CreateLocalized(-1648336616, InfobarMessageType.Error);
							result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						}
						else
						{
							result = EditCalendarItemPreFormAction.RedirectTo.SchedulingTab;
						}
						break;
					case 14:
					{
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						string formParameter2 = Utilities.GetFormParameter(this.request, "hidsttm", false);
						string formParameter3 = Utilities.GetFormParameter(this.request, "seldur", false);
						if (!string.IsNullOrEmpty(formParameter2) && !string.IsNullOrEmpty(formParameter2))
						{
							string[] array = formParameter2.Split(new char[]
							{
								','
							});
							int hour;
							int minute;
							int day;
							int month;
							int year;
							if (array.Length == 5 && int.TryParse(array[0], out hour) && int.TryParse(array[1], out minute) && int.TryParse(array[2], out day) && int.TryParse(array[3], out month) && int.TryParse(array[4], out year))
							{
								ExDateTime exDateTime = new ExDateTime(this.userContext.TimeZone, year, month, day, hour, minute, 0, 0);
								ExDateTime endTime = exDateTime;
								int num2;
								if (int.TryParse(formParameter3, out num2))
								{
									endTime = exDateTime.AddMinutes((double)num2);
								}
								this.calendarItemBase.StartTime = exDateTime;
								this.calendarItemBase.EndTime = endTime;
								if (exDateTime.TimeOfDay.TotalSeconds != 0.0 || endTime.TimeOfDay.TotalSeconds != 0.0)
								{
									this.calendarItemBase.IsAllDayEvent = false;
								}
							}
						}
						string formParameter4 = Utilities.GetFormParameter(this.request, "hidselrm", false);
						if (!string.IsNullOrEmpty(formParameter4))
						{
							Participant participant = null;
							RecipientAddress recipientAddress = AnrManager.ResolveAnrString(formParameter4, this.userContext.UserOptions.CheckNameInContactsFirst, this.userContext);
							if (recipientAddress != null)
							{
								Utilities.CreateExchangeParticipant(out participant, recipientAddress.DisplayName, recipientAddress.RoutingAddress, recipientAddress.RoutingType, recipientAddress.AddressOrigin, recipientAddress.StoreObjectId, recipientAddress.EmailAddressIndex);
								if (participant != null)
								{
									bool flag = false;
									foreach (Attendee attendee in this.calendarItemBase.AttendeeCollection)
									{
										if (attendee.AttendeeType == AttendeeType.Resource && attendee.Participant == participant)
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										CalendarUtilities.AddAttendee(this.calendarItemBase, participant, AttendeeType.Resource);
									}
								}
							}
						}
						break;
					}
					case 15:
					{
						string formParameter5 = Utilities.GetFormParameter(this.request, "hidrmrcp", false);
						if (!string.IsNullOrEmpty(formParameter5))
						{
							CalendarUtilities.RemoveAttendeeByRecipientIdString(this.calendarItemBase, formParameter5);
						}
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					}
					case 16:
						if (CalendarUtilities.SaveCalendarItem(this.calendarItemBase, this.userContext, out infobarMessage))
						{
							EditCalendarItemHelper.ClearUserContextData(this.userContext);
						}
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						break;
					case 17:
						if (cancelRangeType == EditCalendarItemHelper.CancelRangeType.None)
						{
							result = this.DoSave(out infobarMessage);
						}
						else if (!EditCalendarItemHelper.CancelCalendarItem(this.userContext, this.calendarItemBase, cancelRangeType, cancelFromDateTime, out infobarMessage))
						{
							result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						}
						else
						{
							result = EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView;
						}
						break;
					case 18:
						result = this.DoSend(out infobarMessage, ref calendarItemUpdateFlags2, response);
						break;
					case 19:
						ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "Sending calendarItem to modified recips");
						result = this.DoSendAndSave(false, out infobarMessage);
						break;
					case 20:
						ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "Sending calendarItem to all recips");
						result = this.DoSendAndSave(true, out infobarMessage);
						break;
					case 21:
						if (!EditCalendarItemHelper.CancelCalendarItem(this.userContext, this.calendarItemBase, cancelRangeType, cancelFromDateTime, out infobarMessage))
						{
							result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
						}
						else
						{
							result = EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView;
						}
						break;
					case 22:
						result = EditCalendarItemPreFormAction.RedirectTo.ADPage;
						break;
					default:
						goto IL_785;
					}
					return result;
				}
			}
			IL_785:
			throw new OwaInvalidRequestException("Invalid command form parameter");
		}

		private void CheckUnresolvedAttendees(ref EditCalendarItemHelper.CalendarItemUpdateFlags updateFlags)
		{
			if ((updateFlags & EditCalendarItemHelper.CalendarItemUpdateFlags.HasUnresolvedAttendees) == EditCalendarItemHelper.CalendarItemUpdateFlags.None && Utilities.GetFormParameter(this.request, "hidunrslrcp", false) == "1")
			{
				updateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.HasUnresolvedAttendees;
			}
		}

		private EditCalendarItemPreFormAction.RedirectTo DoSend(out InfobarMessage infobarMessage, ref EditCalendarItemHelper.CalendarItemUpdateFlags updateFlags, PreFormActionResponse response)
		{
			infobarMessage = null;
			this.CheckUnresolvedAttendees(ref updateFlags);
			bool flag = Utilities.GetQueryStringParameter(this.request, "fsnd", false) != null;
			EditCalendarItemPreFormAction.RedirectTo result;
			string text;
			if ((updateFlags & EditCalendarItemHelper.CalendarItemUpdateFlags.HasUnresolvedAttendees) != EditCalendarItemHelper.CalendarItemUpdateFlags.None)
			{
				infobarMessage = InfobarMessage.CreateLocalized(81519353, InfobarMessageType.Error);
				result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
			}
			else if (Utilities.RecipientsOnlyHaveEmptyPDL<Attendee>(this.userContext, this.calendarItemBase.AttendeeCollection))
			{
				infobarMessage = InfobarMessage.CreateLocalized(1389137820, InfobarMessageType.Error);
				result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
			}
			else if (!flag && CalendarItemUtilities.BuildSendConfirmDialogPrompt(this.calendarItemBase, out text))
			{
				response.AddParameter("sndpt", "1");
				result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
			}
			else
			{
				bool flag2 = (updateFlags & EditCalendarItemHelper.CalendarItemUpdateFlags.AttendeesChanged) != EditCalendarItemHelper.CalendarItemUpdateFlags.None;
				if (this.calendarItemBase.AttendeeCollection.Count == 0 && !flag2)
				{
					infobarMessage = InfobarMessage.CreateLocalized(-1902165978, InfobarMessageType.Error);
					result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
				}
				else
				{
					bool flag3 = (updateFlags & EditCalendarItemHelper.CalendarItemUpdateFlags.TimeChanged) != EditCalendarItemHelper.CalendarItemUpdateFlags.None || (updateFlags & EditCalendarItemHelper.CalendarItemUpdateFlags.LocationChanged) != EditCalendarItemHelper.CalendarItemUpdateFlags.None;
					if (flag2 && !flag3 && this.calendarItemBase.MeetingRequestWasSent)
					{
						string str = HttpUtility.UrlEncode(Utilities.JavascriptEncode(this.calendarItemBase.Id.ToBase64String()));
						SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
						sanitizingStringBuilder.Append("<div class=\"iem vsp\"><span tabindex=0>");
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(425963094));
						sanitizingStringBuilder.Append("</span><ul>");
						sanitizingStringBuilder.Append("<li><a href=\"#\" onClick=\"return onClkSndM('");
						sanitizingStringBuilder.Append(str);
						sanitizingStringBuilder.Append("')\">");
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-246582747));
						sanitizingStringBuilder.Append("</a></li>");
						sanitizingStringBuilder.Append("<li><a href=\"#\" onClick=\"return onClkSndA('");
						sanitizingStringBuilder.Append(str);
						sanitizingStringBuilder.Append("')\">");
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(667292261));
						sanitizingStringBuilder.Append("</a></li>");
						sanitizingStringBuilder.Append("<li><a href=\"#\" onClick=\"return onClkTb('close')\">");
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-950591140));
						sanitizingStringBuilder.Append("</a></li>");
						sanitizingStringBuilder.Append("</ul></div>");
						infobarMessage = InfobarMessage.CreatePromptHtml(SanitizedHtmlString.FromStringId(-1388726449), sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), SanitizedHtmlString.Empty);
						result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
					}
					else
					{
						ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "Sending calendarItem");
						result = this.DoSendAndSave(true, out infobarMessage);
					}
				}
			}
			return result;
		}

		private EditCalendarItemPreFormAction.RedirectTo DoSendAndSave(bool sendToAll, out InfobarMessage infobarMessage)
		{
			EditCalendarItemPreFormAction.RedirectTo result;
			if (CalendarUtilities.SendMeetingMessages(this.userContext, this.calendarItemBase, sendToAll, out infobarMessage))
			{
				result = EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView;
			}
			else
			{
				result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
			}
			return result;
		}

		private EditCalendarItemPreFormAction.RedirectTo DoSave(out InfobarMessage infobarMessage)
		{
			EditCalendarItemPreFormAction.RedirectTo result;
			if (CalendarUtilities.SaveCalendarItem(this.calendarItemBase, this.userContext, out infobarMessage))
			{
				EditCalendarItemHelper.ClearUserContextData(this.userContext);
				result = EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView;
			}
			else
			{
				result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
			}
			return result;
		}

		private EditCalendarItemPreFormAction.RedirectTo DoDelete(out InfobarMessage infobarMessage)
		{
			infobarMessage = null;
			bool flag = this.calendarItemBase.CalendarItemType != CalendarItemType.Single;
			bool flag2 = this.calendarItemBase.IsMeeting && this.calendarItemBase.MeetingRequestWasSent;
			EditCalendarItemPreFormAction.RedirectTo result;
			if (!flag && (!flag2 || this.calendarItemBase.AttendeeCollection.Count == 0))
			{
				if (Utilities.IsItemInDefaultFolder(this.calendarItemBase, DefaultFolderType.DeletedItems))
				{
					Utilities.DeleteItems(this.userContext, DeleteItemFlags.SoftDelete, new StoreId[]
					{
						this.calendarItemBase.Id
					});
				}
				else
				{
					Utilities.DeleteItems(this.userContext, DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						this.calendarItemBase.Id
					});
				}
				ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "Deleting calendarItem");
				result = EditCalendarItemPreFormAction.RedirectTo.CalendarDailyView;
				this.userContext.ForceNewSearch = true;
			}
			else
			{
				infobarMessage = EditCalendarItemHelper.BuildCancellationPrompt(this.calendarItemBase, this.userContext);
				result = EditCalendarItemPreFormAction.RedirectTo.EditCalendarItem;
			}
			return result;
		}

		private void AddOrReplaceAttendee()
		{
			AttendeeType attendeeType = (AttendeeType)RequestParser.TryGetIntValueFromForm(this.request, "hidaddrcptype", -1);
			if (attendeeType != (AttendeeType)(-1))
			{
				ResolvedRecipientDetail[] array = ResolvedRecipientDetail.ParseFromForm(this.request, "hidaddrcp", false);
				if (array != null)
				{
					CalendarUtilities.AddResolvedAttendees(this.calendarItemBase, attendeeType, array, this.userContext);
				}
				if (CalendarUtilities.CheckIsLocationGenerated(this.calendarItemBase) || string.IsNullOrEmpty(this.calendarItemBase.Location))
				{
					CalendarUtilities.GenerateAndSetLocation(this.calendarItemBase);
				}
			}
			string formParameter = Utilities.GetFormParameter(this.request, "hidrmrcp", false);
			if (!string.IsNullOrEmpty(formParameter))
			{
				CalendarUtilities.RemoveAttendeeByRecipientIdString(this.calendarItemBase, formParameter);
			}
		}

		private void RedirectToEdit(OwaContext owaContext, InfobarMessage infobarMessage, PreFormActionResponse response)
		{
			if (this.calendarItemBase.Id != null)
			{
				owaContext.PreFormActionId = OwaStoreObjectId.CreateFromStoreObject(this.calendarItemBase);
			}
			if (infobarMessage != null)
			{
				owaContext[OwaContextProperty.InfobarMessage] = infobarMessage;
			}
			response.ApplicationElement = ApplicationElement.Item;
			response.Type = "IPM.Appointment";
			response.State = string.Empty;
			string action = owaContext.FormsRegistryContext.Action;
			if (string.CompareOrdinal(action, "Open") == 0 || string.CompareOrdinal(action, "Delete") == 0 || string.CompareOrdinal(action, "New") == 0)
			{
				response.Action = action;
				return;
			}
			response.Action = "Open";
		}

		private void RedirectToSchedulingTab(OwaContext owaContext, InfobarMessage infobarMessage, PreFormActionResponse response)
		{
			if (CalendarUtilities.IsCalendarItemDirty(this.calendarItemBase, owaContext.UserContext))
			{
				response.AddParameter("cd", "1");
			}
			if (Utilities.GetQueryStringParameter(this.request, "cp", false) != null)
			{
				response.AddParameter("cp", "1");
			}
			response.Action = "Schedule";
			response.ApplicationElement = ApplicationElement.Item;
			response.Type = "IPM.Appointment";
			response.State = string.Empty;
		}

		private void RedirectToAttachmentManager(OwaContext owaContext, PreFormActionResponse response)
		{
			if (this.calendarItemBase.Id != null)
			{
				owaContext.PreFormActionId = OwaStoreObjectId.CreateFromStoreObject(this.calendarItemBase);
				response.AddParameter("id", this.calendarItemBase.Id.ObjectId.ToBase64String());
			}
			response.ApplicationElement = ApplicationElement.Dialog;
			response.Type = "Attach";
		}

		internal const string ItemIdFormParameter = "hidid";

		internal const string ChangeKeyFormParameter = "hidchk";

		internal const string IsBeingCanceledQueryParameter = "cp";

		private const string CommandFormParameter = "hidcmdpst";

		private const string UnresolvedRecipientsFormParameter = "hidunrslrcp";

		private const string RemoveAttendeeFormParameter = "hidrmrcp";

		private const string AddAttendeeTypeFormParameter = "hidaddrcptype";

		private const string AddAttendeeFormParameter = "hidaddrcp";

		private CalendarItemBase calendarItemBase;

		private UserContext userContext;

		private HttpRequest request;

		private enum RedirectTo
		{
			None,
			AddressBook,
			ADPage,
			AttachmentManager,
			CalendarDailyView,
			EditCalendarItem,
			EditRecurrence,
			SchedulingTab
		}

		public enum CalendarTab
		{
			Appointment,
			Scheduling
		}
	}
}
