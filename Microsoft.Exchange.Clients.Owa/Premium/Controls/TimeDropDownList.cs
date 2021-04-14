using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class TimeDropDownList : DropDownList
	{
		private TimeDropDownList(ExDateTime selectedTime, string id, string valueId, ExDateTime endTime) : base(id, false, null, null)
		{
			this.selectedTime = selectedTime;
			this.valueId = valueId;
			this.endTime = endTime;
		}

		public static void RenderTimePicker(TextWriter writer, ExDateTime selectedTime, string id)
		{
			TimeDropDownList.RenderTimePicker(writer, selectedTime, id, true);
		}

		public static void RenderTimePicker(TextWriter writer, ExDateTime selectedTime, string id, bool isEnabled)
		{
			TimeDropDownList.RenderTimePicker(writer, selectedTime, id, isEnabled, true);
		}

		public static void RenderTimePicker(TextWriter writer, ExDateTime selectedTime, string id, bool isEnabled, bool isItemEditable)
		{
			TimeDropDownList.RenderTimePicker(writer, selectedTime, id, isEnabled, isItemEditable, ExDateTime.MinValue);
		}

		public static void RenderTimePicker(TextWriter writer, ExDateTime selectedTime, string id, bool isEnabled, bool isItemEditable, ExDateTime endTime)
		{
			new TimeDropDownList(selectedTime, id, "txtTime", endTime)
			{
				Enabled = isEnabled,
				isItemEditable = isItemEditable
			}.Render(writer);
		}

		protected override void RenderExpandoData(TextWriter writer)
		{
			base.RenderExpandoData(writer);
			ISessionContext sessionContext = OwaContext.Current.SessionContext;
			writer.Write(" sTm=\"");
			writer.Write(DateTimeUtilities.GetJavascriptDate(this.selectedTime));
			writer.Write("\"");
			if (this.endTime != ExDateTime.MinValue)
			{
				writer.Write(" sEndTm=\"");
				writer.Write(DateTimeUtilities.GetJavascriptDate(this.endTime));
				writer.Write("\"");
			}
			writer.Write(" sStf=\"");
			Utilities.SanitizeHtmlEncode(sessionContext.TimeFormat, writer);
			writer.Write("\"");
			writer.Write(" sAmPmRx=\"");
			TimeDropDownList.RenderAmPmRegularExpression(writer);
			writer.Write("\"");
			writer.Write(" L_InvldTm=\"");
			writer.Write(SanitizedHtmlString.FromStringId(-863308736));
			writer.Write("\"");
			writer.Write(" L_Dec=\"");
			writer.Write(SanitizedHtmlString.FromStringId(-1032346272));
			writer.Write("\"");
			writer.Write(" L_AM=\"");
			writer.Write(Utilities.SanitizeHtmlEncode(Culture.AMDesignator));
			writer.Write("\"");
			writer.Write(" L_PM=\"");
			writer.Write(Utilities.SanitizeHtmlEncode(Culture.PMDesignator));
			writer.Write("\"");
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write("<input type=\"text\" id=");
			writer.Write(this.valueId);
			if (!this.isItemEditable)
			{
				writer.Write(" readonly=\"true\" ");
			}
			if (base.Enabled)
			{
				writer.Write(" value=\"");
			}
			else
			{
				writer.Write(" disabled value=\"");
			}
			writer.Write(this.selectedTime.ToString(this.sessionContext.TimeFormat));
			writer.Write("\">");
		}

		protected override void RenderListItems(TextWriter writer)
		{
		}

		private static void RenderAmPmRegularExpression(TextWriter writer)
		{
			string amdesignator = Culture.AMDesignator;
			string pmdesignator = Culture.PMDesignator;
			if (!string.IsNullOrEmpty(amdesignator) && !string.IsNullOrEmpty(pmdesignator) && amdesignator[0] != pmdesignator[0])
			{
				writer.Write("(");
				writer.Write(Utilities.SanitizeHtmlEncode(Culture.AMDesignator));
				writer.Write("|");
				Utilities.SanitizeHtmlEncode(amdesignator.Substring(0, 1), writer);
				writer.Write(")|(");
				writer.Write(Utilities.SanitizeHtmlEncode(Culture.PMDesignator));
				writer.Write("|");
				Utilities.SanitizeHtmlEncode(pmdesignator.Substring(0, 1), writer);
				writer.Write(")");
				return;
			}
			writer.Write("(");
			if (!string.IsNullOrEmpty(amdesignator))
			{
				writer.Write(Utilities.SanitizeHtmlEncode(Culture.AMDesignator));
			}
			writer.Write(")|(");
			if (!string.IsNullOrEmpty(pmdesignator))
			{
				writer.Write(Utilities.SanitizeHtmlEncode(Culture.PMDesignator));
			}
			writer.Write(")");
		}

		private ExDateTime selectedTime;

		private ExDateTime endTime;

		private string valueId;

		private bool isItemEditable;
	}
}
