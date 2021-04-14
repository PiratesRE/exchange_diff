using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditCalendarItem : EditItemForm, IRegistryOnlyForm
	{
		protected static AttachmentWellType EditCalendarAttachmentWellType
		{
			get
			{
				return AttachmentWellType.ReadWrite;
			}
		}

		protected static int StoreObjectTypeCalendarItem
		{
			get
			{
				return 15;
			}
		}

		protected static int ImportanceLow
		{
			get
			{
				return 0;
			}
		}

		protected static int ImportanceNormal
		{
			get
			{
				return 1;
			}
		}

		protected static int ImportanceHigh
		{
			get
			{
				return 2;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return true;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.infobar.SetInfobarClass("infobarEdit");
			this.infobar.SetShouldHonorHideByDefault(true);
			this.calendarItemBase = base.Initialize<CalendarItemBase>(false, new PropertyDefinition[]
			{
				CalendarItemBaseSchema.CalendarItemType,
				StoreObjectSchema.EffectiveRights
			});
			if (this.calendarItemBase != null)
			{
				if (string.Equals(base.OwaContext.FormsRegistryContext.Action, "Open", StringComparison.OrdinalIgnoreCase))
				{
					this.newItemType = NewItemType.ExplicitDraft;
				}
				else
				{
					this.newItemType = NewItemType.ImplicitDraft;
					base.DeleteExistingDraft = true;
				}
			}
			else
			{
				this.calendarItemBase = Utilities.CreateDraftMeetingRequestFromQueryString(base.UserContext, base.Request, new PropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights
				});
				if (this.calendarItemBase != null)
				{
					this.newItemType = NewItemType.ImplicitDraft;
					base.DeleteExistingDraft = true;
					base.Item = this.calendarItemBase;
				}
			}
			if (this.calendarItemBase != null)
			{
				this.isMeeting = this.calendarItemBase.IsMeeting;
				this.startTime = this.calendarItemBase.StartTime;
				this.endTime = this.calendarItemBase.EndTime;
				if (this.calendarItemBase.IsAllDayEvent && !this.IsRecurringMaster)
				{
					this.endTime = this.endTime.IncrementDays(-1);
				}
				this.recipientWell = new CalendarItemRecipientWell(this.calendarItemBase);
				this.bodyMarkup = BodyConversionUtilities.GetBodyFormatOfEditItem(this.calendarItemBase, this.newItemType, base.UserContext.UserOptions);
				this.toolbar = new EditCalendarItemToolbar(this.calendarItemBase, this.isMeeting, this.bodyMarkup, this.IsPublicItem);
				this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
				this.isBeingCanceled = (Utilities.GetQueryStringParameter(base.Request, "c", false) != null);
				string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "er", false);
				if (queryStringParameter != null)
				{
					if (this.calendarItemBase.CalendarItemType != CalendarItemType.RecurringMaster || !(this.calendarItemBase is CalendarItem))
					{
						throw new OwaInvalidRequestException("Invalid calendar item type.  Only recurring masters support specifying an end range");
					}
					this.endRange = ExDateTime.MinValue;
					try
					{
						this.endRange = DateTimeUtilities.ParseIsoDate(queryStringParameter, base.UserContext.TimeZone);
					}
					catch (OwaParsingErrorException)
					{
						ExTraceGlobals.CalendarDataTracer.TraceDebug<string>((long)this.GetHashCode(), "Invalid end range provided on URL '{0}'", queryStringParameter);
						throw new OwaInvalidRequestException(string.Format("Invalid end range provided on URL '{0}'", queryStringParameter));
					}
					if (this.endRange != ExDateTime.MinValue)
					{
						CalendarItem calendarItem = (CalendarItem)this.calendarItemBase;
						this.occurrenceCount = MeetingUtilities.CancelRecurrence(calendarItem, this.endRange);
						if (this.occurrenceCount == 0)
						{
							this.isBeingCanceled = true;
						}
					}
				}
				string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "od", false);
				if (queryStringParameter2 != null)
				{
					try
					{
						this.occurrenceDate = DateTimeUtilities.ParseIsoDate(queryStringParameter2, base.UserContext.TimeZone).Date;
						goto IL_303;
					}
					catch (OwaParsingErrorException)
					{
						ExTraceGlobals.CalendarDataTracer.TraceDebug<string>((long)this.GetHashCode(), "Invalid occurrence date specified on URL '{0}'", queryStringParameter2);
						throw new OwaInvalidRequestException(string.Format("Invalid occurrence date provided on URL '{0}'", queryStringParameter2));
					}
				}
				this.occurrenceDate = DateTimeUtilities.GetLocalTime().Date;
				IL_303:
				CalendarUtilities.AddCalendarInfobarMessages(this.infobar, this.calendarItemBase, null, base.UserContext);
				if (this.isBeingCanceled)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(1328030972), InfobarMessageType.Informational);
				}
				this.disableOccurrenceReminderUI = MeetingUtilities.CheckShouldDisableOccurrenceReminderUI(this.calendarItemBase);
				if (this.disableOccurrenceReminderUI && !this.IsPublicItem)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-891369593), InfobarMessageType.Informational);
				}
				if (!(this.calendarItemBase is CalendarItem))
				{
					this.recurrenceUtilities = new RecurrenceUtilities(null, base.SanitizingResponse);
					return;
				}
				if (0 < this.occurrenceCount)
				{
					EndDateRecurrenceRange range = new EndDateRecurrenceRange(((CalendarItem)this.calendarItemBase).Recurrence.Range.StartDate, this.endRange.IncrementDays(-1));
					this.recurrenceUtilities = new RecurrenceUtilities(new Recurrence(((CalendarItem)this.calendarItemBase).Recurrence.Pattern, range), base.SanitizingResponse);
					return;
				}
				this.recurrenceUtilities = new RecurrenceUtilities(((CalendarItem)this.calendarItemBase).Recurrence, base.SanitizingResponse);
				return;
			}
			else
			{
				this.isMeeting = (Utilities.GetQueryStringParameter(base.Request, "mr", false) != null);
				if (this.isMeeting && this.IsPublicItem)
				{
					throw new OwaInvalidRequestException("Can't create meeting in Owa Public Folders");
				}
				this.isAllDayEvent = (Utilities.GetQueryStringParameter(base.Request, "evt", false) != null);
				bool flag = Utilities.GetQueryStringParameter(base.Request, "tm", false) != null || this.isAllDayEvent;
				string queryStringParameter3 = Utilities.GetQueryStringParameter(base.Request, "st", false);
				if (queryStringParameter3 != null)
				{
					try
					{
						this.startTime = DateTimeUtilities.ParseIsoDate(queryStringParameter3, base.UserContext.TimeZone);
					}
					catch (OwaParsingErrorException)
					{
						ExTraceGlobals.CalendarDataTracer.TraceDebug<string>((long)this.GetHashCode(), "Invalid start date provided on URL '{0}'", queryStringParameter3);
						throw new OwaInvalidRequestException(string.Format("Invalid start date provided on URL '{0}'", queryStringParameter3));
					}
				}
				if (flag || this.startTime == ExDateTime.MinValue)
				{
					ExDateTime localTime = DateTimeUtilities.GetLocalTime();
					if (this.startTime == ExDateTime.MinValue)
					{
						this.startTime = new ExDateTime(base.UserContext.TimeZone, localTime.Year, localTime.Month, localTime.Day, localTime.Hour, localTime.Minute, 0);
					}
					else
					{
						this.startTime = new ExDateTime(base.UserContext.TimeZone, this.startTime.Year, this.startTime.Month, this.startTime.Day, localTime.Hour, localTime.Minute, 0);
					}
					if (this.isAllDayEvent && this.startTime.Hour == 23)
					{
						if (this.startTime.Minute >= 30)
						{
							this.startTime = this.startTime.Date;
						}
						else
						{
							this.startTime = new ExDateTime(base.UserContext.TimeZone, this.startTime.Year, this.startTime.Month, this.startTime.Day, 23, 0, 0);
							this.endTime = new ExDateTime(base.UserContext.TimeZone, this.startTime.Year, this.startTime.Month, this.startTime.Day, 23, 30, 0);
						}
					}
					else if (this.startTime.Minute != 0 && this.startTime.Minute != 30)
					{
						this.startTime = this.startTime.AddMinutes((double)(30 - this.startTime.Minute % 30));
					}
				}
				if (this.endTime == ExDateTime.MinValue)
				{
					this.endTime = this.startTime.AddMinutes(60.0);
				}
				this.recipientWell = new CalendarItemRecipientWell();
				this.bodyMarkup = base.UserContext.UserOptions.ComposeMarkup;
				this.toolbar = new EditCalendarItemToolbar(null, this.isMeeting, this.bodyMarkup, this.IsPublicItem);
				this.toolbar.ToolbarType = (base.IsPreviewForm ? ToolbarType.Preview : ToolbarType.Form);
				this.recurrenceUtilities = new RecurrenceUtilities(null, base.SanitizingResponse);
				return;
			}
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			bool flag = BodyConversionUtilities.GenerateEditableMessageBodyAndRenderInfobarMessages(this.calendarItemBase, writer, this.newItemType, base.OwaContext, ref this.shouldPromptUserForUnblockingOnFormLoad, ref this.hasInlineImages, this.infobar, base.IsRequestCallbackForWebBeacons, this.bodyMarkup);
			if (flag)
			{
				this.calendarItemBase.Load();
			}
		}

		protected void CreateAttachmentHelpers()
		{
			if (this.calendarItemBase != null)
			{
				this.attachmentInformation = AttachmentWell.GetAttachmentInformation(this.calendarItemBase, base.AttachmentLinks, base.UserContext.IsPublicLogon);
				InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentInformation);
				if (infobarRenderingHelper.HasLevelOne)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
				}
			}
		}

		protected Markup BodyMarkup
		{
			get
			{
				return this.bodyMarkup;
			}
		}

		protected void RenderStartTime()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.SanitizingResponse, this.startTime);
		}

		protected void RenderEndTime()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.SanitizingResponse, this.endTime);
		}

		protected void RenderOccurrenceDate()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.SanitizingResponse, this.occurrenceDate);
		}

		protected void RenderStartTimeDropdownList()
		{
			TimeDropDownList.RenderTimePicker(base.SanitizingResponse, this.startTime, "divSTime");
		}

		protected void RenderEndTimeDropdownList()
		{
			TimeDropDownList.RenderTimePicker(base.SanitizingResponse, this.endTime, "divETime");
		}

		protected void RenderStartDate()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.SanitizingResponse, "divSDate", this.startTime.Date);
		}

		protected void RenderEndDate()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.SanitizingResponse, "divEDate", this.endTime.Date);
		}

		protected void RenderReminderDropdownList()
		{
			CalendarUtilities.RenderReminderDropdownList(base.SanitizingResponse, this.calendarItemBase, this.ReminderIsSet, this.DisableOccurrenceReminderUI || this.IsPublicItem);
		}

		protected void RenderBusyTypeDropdownList()
		{
			CalendarUtilities.RenderBusyTypeDropdownList(base.SanitizingResponse, this.calendarItemBase, false);
		}

		protected void RenderToolbar()
		{
			this.toolbar.Render(base.SanitizingResponse);
		}

		protected void BuildCalendarInfobar()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "clr", false);
			if (queryStringParameter == null)
			{
				return;
			}
			CalendarUtilities.BuildCalendarInfobar(this.infobar, base.UserContext, this.FolderId ?? base.UserContext.CalendarFolderOwaId, CalendarColorManager.ParseColorIndexString(queryStringParameter, true), true);
		}

		internal OwaStoreObjectId FolderId
		{
			get
			{
				if (this.folderId == null)
				{
					if (base.TargetFolderId != null)
					{
						this.folderId = base.TargetFolderId;
					}
					else if (base.Item != null)
					{
						this.folderId = base.ParentFolderId;
					}
				}
				return this.folderId;
			}
		}

		protected bool IsFolderIdNull
		{
			get
			{
				return this.FolderId == null;
			}
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, base.Item);
		}

		protected void RenderCategories()
		{
			if (base.Item != null)
			{
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
			}
		}

		protected void RenderSender()
		{
			RenderingUtilities.RenderSender(base.UserContext, base.SanitizingResponse, this.calendarItemBase);
		}

		protected void RenderSubject(bool isTitle)
		{
			if (isTitle)
			{
				string untitled = this.isMeeting ? LocalizedStrings.GetNonEncoded(-1500721828) : LocalizedStrings.GetNonEncoded(-1178892512);
				RenderingUtilities.RenderSubject(base.SanitizingResponse, this.calendarItemBase, untitled);
				return;
			}
			RenderingUtilities.RenderSubject(base.SanitizingResponse, this.calendarItemBase);
		}

		protected bool DisableReminderCheckBox
		{
			get
			{
				return this.DisableOccurrenceReminderUI || this.IsPublicItem;
			}
		}

		protected bool DisablePrivateCheckBox
		{
			get
			{
				return !this.IsSingleOrRecurringMaster || this.IsPublicItem || this.IsInSharedFolder;
			}
		}

		protected bool IsInSharedFolder
		{
			get
			{
				return this.FolderId != null && this.FolderId.IsOtherMailbox;
			}
		}

		protected void RenderJavascriptEncodedClassName()
		{
			if (this.calendarItemBase != null)
			{
				Utilities.JavascriptEncode(this.calendarItemBase.ClassName, base.SanitizingResponse);
				return;
			}
			Utilities.JavascriptEncode("IPM.Appointment");
		}

		protected void RenderJavascriptEncodedCalendarItemBaseChangeKey()
		{
			if (this.calendarItemBase != null)
			{
				Utilities.JavascriptEncode(this.calendarItemBase.Id.ChangeKeyAsBase64String(), base.SanitizingResponse);
			}
		}

		protected void RenderJavascriptEncodedCalendarItemBaseMasterId()
		{
			if (this.calendarItemBase != null && (this.calendarItemBase.CalendarItemType == CalendarItemType.Occurrence || this.calendarItemBase.CalendarItemType == CalendarItemType.Exception))
			{
				OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromStoreObject(this.calendarItemBase);
				Utilities.JavascriptEncode(owaStoreObjectId.ProviderLevelItemId.ToString(), base.SanitizingResponse);
			}
		}

		protected override void RenderJavaScriptEncodedTargetFolderId()
		{
			if (this.FolderId != null)
			{
				Utilities.JavascriptEncode(this.FolderId.ToBase64String(), base.SanitizingResponse);
			}
		}

		protected string Location
		{
			get
			{
				if (this.calendarItemBase == null)
				{
					return string.Empty;
				}
				return Utilities.HtmlEncode(this.calendarItemBase.Location);
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected bool IsMeeting
		{
			get
			{
				return this.isMeeting;
			}
		}

		protected bool MeetingRequestWasSent
		{
			get
			{
				return this.calendarItemBase != null && this.IsMeeting && this.calendarItemBase.MeetingRequestWasSent;
			}
		}

		protected bool IsPrivate
		{
			get
			{
				if (this.calendarItemBase != null)
				{
					object obj = this.calendarItemBase.TryGetProperty(ItemSchema.Sensitivity);
					return obj is Sensitivity && (Sensitivity)obj == Sensitivity.Private;
				}
				return false;
			}
		}

		protected bool ReminderIsSet
		{
			get
			{
				if (this.calendarItemBase != null)
				{
					object obj = this.calendarItemBase.TryGetProperty(ItemSchema.ReminderIsSet);
					return obj is bool && (bool)obj;
				}
				return base.UserContext.UserOptions.EnableReminders && !this.IsPublicItem;
			}
		}

		protected bool IsAllDayEvent
		{
			get
			{
				if (this.calendarItemBase != null)
				{
					return this.calendarItemBase.IsAllDayEvent;
				}
				return this.isAllDayEvent;
			}
		}

		protected bool IsResponseRequested
		{
			get
			{
				if (this.calendarItemBase != null)
				{
					object obj = this.calendarItemBase.TryGetProperty(ItemSchema.IsResponseRequested);
					return !(obj is bool) || (bool)obj;
				}
				return true;
			}
		}

		protected bool HasAttachments
		{
			get
			{
				return this.calendarItemBase != null && this.calendarItemBase.AttachmentCollection != null && 0 < this.calendarItemBase.AttachmentCollection.Count;
			}
		}

		protected bool IsBeingCanceled
		{
			get
			{
				return this.isBeingCanceled;
			}
		}

		protected bool DisableOccurrenceReminderUI
		{
			get
			{
				return this.disableOccurrenceReminderUI;
			}
		}

		protected bool IsRecurringMaster
		{
			get
			{
				return this.calendarItemBase != null && this.calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster;
			}
		}

		protected string When
		{
			get
			{
				if (0 < this.occurrenceCount)
				{
					if (this.calendarItemBase.CalendarItemType != CalendarItemType.RecurringMaster || !(this.calendarItemBase is CalendarItem))
					{
						throw new OwaInvalidRequestException("Invalid calendar item type.  Only recurring masters support specifying an end range");
					}
					CalendarItem calendarItem = (CalendarItem)this.calendarItemBase;
					EndDateRecurrenceRange range = new EndDateRecurrenceRange(calendarItem.Recurrence.Range.StartDate, this.endRange.IncrementDays(-1));
					Recurrence recurrence = new Recurrence(calendarItem.Recurrence.Pattern, range);
					return Utilities.HtmlEncode(CalendarUtilities.GenerateWhen(base.UserContext, calendarItem.StartTime, calendarItem.EndTime, recurrence));
				}
				else
				{
					if (!(this.calendarItemBase is CalendarItem))
					{
						return "&nbsp;";
					}
					CalendarItem calendarItem2 = (CalendarItem)this.calendarItemBase;
					if (calendarItem2.Recurrence != null)
					{
						return Utilities.HtmlEncode(calendarItem2.GenerateWhen());
					}
					return "&nbsp;";
				}
			}
		}

		public ArrayList AttachmentInformation
		{
			get
			{
				return this.attachmentInformation;
			}
		}

		protected RecurrenceUtilities RecurrenceUtilities
		{
			get
			{
				return this.recurrenceUtilities;
			}
		}

		protected bool ShouldPromptUser
		{
			get
			{
				return this.shouldPromptUserForUnblockingOnFormLoad;
			}
		}

		protected bool HasInlineImages
		{
			get
			{
				return this.hasInlineImages;
			}
		}

		protected bool IsCalendarItemBaseNull
		{
			get
			{
				return this.calendarItemBase == null;
			}
		}

		protected bool IsSingleOrRecurringMaster
		{
			get
			{
				return this.calendarItemBase == null || this.calendarItemBase.CalendarItemType == CalendarItemType.Single || this.calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster;
			}
		}

		private const int DefaultMeetingLength = 60;

		private CalendarItemBase calendarItemBase;

		private EditCalendarItemToolbar toolbar;

		private Infobar infobar = new Infobar();

		private CalendarItemRecipientWell recipientWell;

		private ArrayList attachmentInformation;

		private bool isAllDayEvent;

		private bool isMeeting;

		private ExDateTime startTime = ExDateTime.MinValue;

		private ExDateTime endTime = ExDateTime.MinValue;

		private ExDateTime endRange = ExDateTime.MinValue;

		private RecurrenceUtilities recurrenceUtilities;

		private Markup bodyMarkup;

		private bool shouldPromptUserForUnblockingOnFormLoad;

		private bool hasInlineImages;

		private NewItemType newItemType;

		private bool disableOccurrenceReminderUI;

		private bool isBeingCanceled;

		private int occurrenceCount;

		private ExDateTime occurrenceDate = ExDateTime.MinValue;

		private OwaStoreObjectId folderId;

		private static class QueryParameters
		{
			public const string MeetingRequest = "mr";

			public const string AllDayEvent = "evt";

			public const string Time = "tm";

			public const string StartTime = "st";

			public const string Cancel = "c";

			public const string OccurrenceDate = "od";

			public const string EndRange = "er";

			public const string Color = "clr";
		}
	}
}
