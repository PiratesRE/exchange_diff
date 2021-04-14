using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class FlagContextMenu : ContextMenu
	{
		public FlagContextMenu(UserContext userContext, ExDateTime defaultDate, FlagAction flagAction) : base("divFlgM", userContext)
		{
			this.defaultDate = defaultDate;
			this.flagAction = flagAction;
		}

		protected override void RenderExpandoData(TextWriter output)
		{
			output.Write(" iFlgDft=\"");
			output.Write((int)base.UserContext.UserOptions.FlagAction);
			output.Write("\"");
			output.Write(" iFA=\"");
			output.Write((int)this.flagAction);
			output.Write("\"");
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			base.RenderMenuItem(output, -367521373, ThemeFileId.Flag, "divFA" + FlagContextMenu.today, FlagContextMenu.today);
			base.RenderMenuItem(output, 1854511297, ThemeFileId.Flag, "divFA" + FlagContextMenu.tomorrow, FlagContextMenu.tomorrow);
			base.RenderMenuItem(output, -1821673574, ThemeFileId.Flag, "divFA" + FlagContextMenu.thisWeek, FlagContextMenu.thisWeek);
			base.RenderMenuItem(output, 1433854051, ThemeFileId.Flag, "divFA" + FlagContextMenu.nextWeek, FlagContextMenu.nextWeek);
			base.RenderMenuItem(output, 689037121, ThemeFileId.Flag, "divFA" + FlagContextMenu.noDate, FlagContextMenu.noDate);
			if (this.userContext.IsRtl)
			{
				output.Write("<div style=\"height:0px;border:1px solid transparent\"></div>");
			}
			base.RenderMenuItem(output, null, ThemeFileId.Flag, "divFA" + FlagContextMenu.specificDate, FlagContextMenu.specificDate, true, null, null, null, new ContextMenu.RenderMenuItemHtml(this.RenderSpecificDateMenuItem), true);
			ContextMenu.RenderMenuDivider(output, null);
			base.RenderMenuItem(output, -667130393, ThemeFileId.ReminderSmall, "divFlgDR", 8.ToString(CultureInfo.InvariantCulture));
			base.RenderMenuItem(output, -32068740, ThemeFileId.FlagComplete, null, 9.ToString(CultureInfo.InvariantCulture));
			base.RenderMenuItem(output, 1176926501, ThemeFileId.None, "divFlgClr", 10.ToString(CultureInfo.InvariantCulture));
			base.RenderMenuItem(output, -986488560, ThemeFileId.None, "divFlgDlt", 10.ToString(CultureInfo.InvariantCulture));
			base.RenderMenuItem(output, 394882041, ThemeFileId.None, "divFlgClrDlt", 10.ToString(CultureInfo.InvariantCulture));
			ContextMenu.RenderMenuDivider(output, null);
			base.RenderMenuItem(output, 712564181, ThemeFileId.None, null, null, false, null, null, DefaultFlagMenu.Create(this.userContext));
		}

		private void RenderSpecificDateMenuItem(TextWriter output)
		{
			RenderingUtilities.RenderInlineSpacer(output, this.userContext, 12);
			output.Write("<span class=\"flgCmbContent\"></span>");
			DatePickerDropDownCombo.RenderDatePicker(output, "divFlgD", this.defaultDate, this.defaultDate, DatePicker.Features.TodayButton);
		}

		internal static FlagAction GetFlagActionForItem(UserContext userContext, Item item)
		{
			FlagStatus property = ItemUtility.GetProperty<FlagStatus>(item, ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			ExDateTime property2 = ItemUtility.GetProperty<ExDateTime>(item, ItemSchema.UtcDueDate, ExDateTime.MinValue);
			return FlagContextMenu.GetFlagActionForItem(userContext, property2, property);
		}

		internal static FlagAction GetFlagActionForItem(UserContext userContext, ExDateTime dueDate, FlagStatus flagStatus)
		{
			if (flagStatus == FlagStatus.NotFlagged)
			{
				return FlagAction.None;
			}
			if (flagStatus == FlagStatus.Complete)
			{
				return FlagAction.MarkComplete;
			}
			if (dueDate == ExDateTime.MinValue)
			{
				return FlagAction.NoDate;
			}
			ExDateTime date = DateTimeUtilities.GetLocalTime(userContext).Date;
			if (dueDate == date)
			{
				return FlagAction.Today;
			}
			ExDateTime d = date.IncrementDays(1);
			if (dueDate == d)
			{
				return FlagAction.Tomorrow;
			}
			int dayOfWeek = DateTimeUtilities.GetDayOfWeek(userContext, date);
			ExDateTime t = date.IncrementDays(-1 * dayOfWeek);
			ExDateTime exDateTime = date.IncrementDays(7 - dayOfWeek);
			if (t <= dueDate && dueDate < exDateTime)
			{
				return FlagAction.ThisWeek;
			}
			ExDateTime t2 = exDateTime.IncrementDays(7);
			if (exDateTime <= dueDate && dueDate < t2)
			{
				return FlagAction.NextWeek;
			}
			return FlagAction.SpecificDate;
		}

		private static readonly string today = 2.ToString(CultureInfo.InvariantCulture);

		private static readonly string tomorrow = 3.ToString(CultureInfo.InvariantCulture);

		private static readonly string thisWeek = 4.ToString(CultureInfo.InvariantCulture);

		private static readonly string nextWeek = 5.ToString(CultureInfo.InvariantCulture);

		private static readonly string noDate = 6.ToString(CultureInfo.InvariantCulture);

		private static readonly string specificDate = 7.ToString(CultureInfo.InvariantCulture);

		private ExDateTime defaultDate;

		private FlagAction flagAction;
	}
}
