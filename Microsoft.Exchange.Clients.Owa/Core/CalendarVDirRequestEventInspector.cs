using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class CalendarVDirRequestEventInspector : RequestEventInspectorBase
	{
		internal override void Init()
		{
		}

		internal override void OnBeginRequest(object sender, EventArgs e, out bool stopExecution)
		{
			stopExecution = false;
		}

		internal override void OnPostAuthorizeRequest(object sender, EventArgs e)
		{
			CalendarVDirRequestDispatcher.DispatchRequest(OwaContext.Current);
		}

		internal override void OnEndRequest(OwaContext owaContext)
		{
		}
	}
}
