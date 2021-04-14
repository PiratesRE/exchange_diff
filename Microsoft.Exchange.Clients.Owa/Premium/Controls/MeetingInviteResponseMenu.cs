using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class MeetingInviteResponseMenu : ContextMenu
	{
		internal static MeetingInviteResponseMenu Create(UserContext userContext, ResponseType responseType)
		{
			return new MeetingInviteResponseMenu(userContext, responseType);
		}

		private MeetingInviteResponseMenu(UserContext userContext, ResponseType responseType)
		{
			string str = "divRTM";
			int num = (int)responseType;
			base..ctor(str + num.ToString(CultureInfo.InvariantCulture), userContext, false);
			this.responseType = responseType;
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			string str = "mir";
			int num = (int)this.responseType;
			string str2 = str + num.ToString(CultureInfo.InvariantCulture);
			base.RenderMenuItem(output, 1050381195, str2 + "e");
			base.RenderMenuItem(output, -114654491, str2 + "s");
			base.RenderMenuItem(output, -990767046, str2 + "d");
		}

		private ResponseType responseType;
	}
}
