using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class BusyTypeDropDownList : DropDownList
	{
		public BusyTypeDropDownList(string id, BusyType busyType)
		{
			int num = (int)busyType;
			base..ctor(id, num.ToString(CultureInfo.InvariantCulture), null);
			this.busyType = busyType;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write(this.GetBusyTypeHtml(this.busyType));
		}

		private SanitizedHtmlString GetBusyTypeHtml(BusyType busyType)
		{
			SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>();
			switch (busyType)
			{
			case BusyType.Free:
				this.sessionContext.RenderThemeImage(sanitizingStringWriter, ThemeFileId.Available, "free", new object[0]);
				sanitizingStringWriter.Write(LocalizedStrings.GetNonEncoded(-971703552));
				goto IL_DC;
			case BusyType.Busy:
				this.sessionContext.RenderThemeImage(sanitizingStringWriter, ThemeFileId.Busy, "busy", new object[0]);
				sanitizingStringWriter.Write(LocalizedStrings.GetNonEncoded(2052801377));
				goto IL_DC;
			case BusyType.OOF:
				this.sessionContext.RenderThemeImage(sanitizingStringWriter, ThemeFileId.OutOfOffice, "oof", new object[0]);
				sanitizingStringWriter.Write(LocalizedStrings.GetNonEncoded(2047193656));
				goto IL_DC;
			}
			this.sessionContext.RenderThemeImage(sanitizingStringWriter, ThemeFileId.Tentative, "tntv", new object[0]);
			sanitizingStringWriter.Write(LocalizedStrings.GetNonEncoded(1797669216));
			IL_DC:
			return sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[BusyTypeDropDownList.busyTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem((int)BusyTypeDropDownList.busyTypes[i], this.GetBusyTypeHtml(BusyTypeDropDownList.busyTypes[i]).ToString(), true);
			}
			return array;
		}

		private static readonly BusyType[] busyTypes = new BusyType[]
		{
			BusyType.Free,
			BusyType.Tentative,
			BusyType.Busy,
			BusyType.OOF
		};

		private BusyType busyType;
	}
}
