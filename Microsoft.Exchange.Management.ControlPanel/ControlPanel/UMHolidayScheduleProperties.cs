using System;
using System.Globalization;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("UMHolidayScheduleProperties", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	public sealed class UMHolidayScheduleProperties : UMBasePopupProperties
	{
		protected override void SetTitleAndCaption(PopupForm form)
		{
			if (base.IsNewRequest)
			{
				form.Caption = Strings.UMHolidayScheduleNewCaption;
				form.Title = Strings.UMHolidayScheduleNewTitle;
				return;
			}
			form.Title = Strings.UMHolidayScheduleEditTitle;
			base.CaptionTextField = "Name";
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "UMHolidayScheduleProperties";
			scriptDescriptor.AddComponentProperty("HolidayPromptUploader", base.ContentContainer.FindControl("auGreetingFile").ClientID);
			scriptDescriptor.AddComponentProperty("StartDateChooser", base.ContentContainer.FindControl("dcStartDate").ClientID);
			scriptDescriptor.AddComponentProperty("EndDateChooser", base.ContentContainer.FindControl("dcEndDate").ClientID);
			scriptDescriptor.AddElementProperty("Name", base.ContentContainer.FindControl("txtName").ClientID);
			scriptDescriptor.AddElementProperty("StartDateForDisplay", base.ContentContainer.FindControl("txtStartDateDisplay").ClientID);
			scriptDescriptor.AddElementProperty("EndDateForDisplay", base.ContentContainer.FindControl("txtEndDateDisplay").ClientID);
			scriptDescriptor.AddProperty("UserDateFormat", EcpDateTimeHelper.GetUserDateFormat());
			scriptDescriptor.AddProperty("TodaysDate", ExDateTime.Now.ToUserExDateTime().ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
			return scriptDescriptor;
		}
	}
}
