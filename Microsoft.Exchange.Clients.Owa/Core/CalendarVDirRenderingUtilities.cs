using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class CalendarVDirRenderingUtilities
	{
		public static void RenderInlineScripts(TextWriter writer)
		{
			Utilities.RenderScriptTagStart(writer);
			Utilities.RenderBootUpScripts(writer);
			Utilities.RenderCDNEndpointVariable(writer);
			Utilities.RenderScriptTagEnd(writer);
		}
	}
}
