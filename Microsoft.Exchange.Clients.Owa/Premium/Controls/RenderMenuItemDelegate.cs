using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal delegate void RenderMenuItemDelegate(TextWriter output, Strings.IDs displayString, ThemeFileId imageFileId, string id, string command, bool disabled, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu);
}
