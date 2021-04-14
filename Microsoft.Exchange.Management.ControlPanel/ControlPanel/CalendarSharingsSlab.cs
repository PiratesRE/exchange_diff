using System;
using System.Web;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CalendarSharingsSlab : SlabControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.Title = CalendarSharingsSlab.GetDisplayName(this.Context.Request, "id");
		}

		internal static string GetDisplayName(HttpRequest request, string idParameter)
		{
			string text = request.QueryString[idParameter];
			string result = string.Empty;
			bool flag = false;
			if (!text.IsNullOrBlank())
			{
				Identity identity = Identity.ParseIdentity(text);
				if (identity != null)
				{
					if (string.Compare(identity.DisplayName, identity.RawIdentity) == 0)
					{
						identity = identity.ResolveByType(IdentityType.MailboxFolder);
					}
					if (identity != null)
					{
						result = identity.DisplayName;
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				throw new BadQueryParameterException(idParameter);
			}
			return result;
		}
	}
}
