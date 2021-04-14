using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class OptionsNavigation
	{
		private static Dictionary<string, Strings.IDs> CreateOptionsTextMap()
		{
			return new Dictionary<string, Strings.IDs>
			{
				{
					"Regional",
					146859578
				},
				{
					"Messaging",
					-201939914
				},
				{
					"JunkEmail",
					-2053927452
				},
				{
					"Calendar",
					1292798904
				},
				{
					"Oof",
					917218743
				},
				{
					"General",
					951662406
				},
				{
					"About",
					539783673
				},
				{
					"Eas",
					-1231836625
				},
				{
					"ChangePassword",
					-392390655
				}
			};
		}

		public OptionsNavigation(string type)
		{
			this.currentOptionType = type;
		}

		public void Render(TextWriter writer, UserContext userContext)
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsNavigation.Render()");
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			writer.Write("<div><table cellspacing=0 cellpadding=0 class=\"on\"><caption>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1286941817));
			writer.Write("</caption>");
			this.RenderOptionNavigation(writer, "Regional");
			this.RenderOptionNavigation(writer, "Messaging");
			if (userContext.IsFeatureEnabled(Feature.JunkEMail))
			{
				this.RenderOptionNavigation(writer, "JunkEmail");
			}
			if (userContext.IsFeatureEnabled(Feature.Calendar))
			{
				this.RenderOptionNavigation(writer, "Calendar");
			}
			this.RenderOptionNavigation(writer, "Oof");
			if (userContext.IsFeatureEnabled(Feature.ChangePassword))
			{
				this.RenderOptionNavigation(writer, "ChangePassword");
			}
			this.RenderOptionNavigation(writer, "General");
			if (userContext.IsMobileSyncEnabled())
			{
				this.RenderOptionNavigation(writer, "Eas");
			}
			this.RenderOptionNavigation(writer, "About");
			writer.Write("<tr><td height=\"100%\">&nbsp;</td></tr>");
			writer.Write("</table></div>");
		}

		private void RenderOptionNavigation(TextWriter writer, string navigationOptionType)
		{
			bool flag = this.currentOptionType == navigationOptionType;
			writer.Write("<tr><td nowrap class=\"opt{0}\">", flag ? " s" : string.Empty);
			string htmlEncoded = LocalizedStrings.GetHtmlEncoded(OptionsNavigation.OptionText[navigationOptionType]);
			if (flag)
			{
				writer.Write(htmlEncoded);
			}
			else
			{
				writer.Write("<a href=\"#\" onclick=\"return onClkON('");
				writer.Write(navigationOptionType);
				writer.Write("');\" title=\"");
				writer.Write(htmlEncoded);
				writer.Write("\">");
				writer.Write(htmlEncoded);
				writer.Write("</a>");
			}
			writer.Write("</td></tr>");
		}

		internal static readonly Dictionary<string, Strings.IDs> OptionText = OptionsNavigation.CreateOptionsTextMap();

		private string currentOptionType = "Messaging";
	}
}
