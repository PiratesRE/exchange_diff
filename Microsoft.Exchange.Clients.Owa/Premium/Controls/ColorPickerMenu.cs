using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ColorPickerMenu : ContextMenu
	{
		private ColorPickerMenu(UserContext userContext, string id) : base(id, userContext)
		{
		}

		internal static ColorPickerMenu Create(UserContext userContext, string id)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("id");
			}
			return new ColorPickerMenu(userContext, id);
		}

		protected override void RenderExpandoData(TextWriter output)
		{
			output.Write(" _colorIndexStart=\"");
			output.Write(CalendarColorManager.GetClientColorIndex(0));
			output.Write("\"");
		}

		protected override void RenderMenuItems(TextWriter output)
		{
		}
	}
}
