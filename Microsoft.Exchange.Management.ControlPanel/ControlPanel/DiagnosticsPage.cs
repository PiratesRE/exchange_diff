using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.CsmSdk;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DiagnosticsPage : EcpContentPage
	{
		protected void RenderTitle(string title)
		{
			base.Response.Output.Write("<div class='diagTitle'>{0}</div>", title);
		}

		protected void RenderGeneralInformation()
		{
			base.Response.Write("<div class='diagBlock'>");
			LocalSession localSession = LocalSession.Current;
			ExchangeRunspaceConfiguration rbacConfiguration = localSession.RbacConfiguration;
			bool flag = localSession.IsInRole("Mailbox+MailboxFullAccess");
			this.Write("Logon user:", string.Format("{0} [{1}]", localSession.Name, rbacConfiguration.ExecutingUserPrimarySmtpAddress));
			this.Write("User-Agent:", HttpContext.Current.Request.UserAgent);
			this.Write("SKU:", Util.IsDataCenter ? "DataCenter" : "On-Premise");
			if (flag)
			{
				this.Write("Mailbox server version:", localSession.Context.MailboxServerVersion);
			}
			else
			{
				this.Write("Mailbox account:", "None mailbox account.");
			}
			this.Write("Current server version:", Util.ApplicationVersion);
			this.Write("Request URL:", HttpContext.Current.GetRequestUrl().ToString());
			this.Write("Display language:", CultureInfo.CurrentCulture.IetfLanguageTag);
			if (flag)
			{
				this.Write("User time zone:", (localSession.UserTimeZone != null) ? localSession.UserTimeZone.DisplayName : "Not set.");
			}
			this.Write("RBAC roles:", this.GetRoles(localSession));
			this.Write("Features:", FlightProvider.Instance.GetAllEnabledFeatures().ToLogString<string>());
			VariantConfigurationSnapshot snapshotForCurrentUser = EacFlightUtility.GetSnapshotForCurrentUser();
			this.Write("Flights:", snapshotForCurrentUser.Flights.ToLogString<string>());
			base.Response.Write("</div>");
		}

		private string GetRoles(RbacPrincipal rbacPrincipal)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (object obj in Enum.GetValues(typeof(RoleType)))
			{
				RoleType roleType = (RoleType)obj;
				string text = roleType.ToString();
				if (rbacPrincipal.IsInRole(text))
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(text);
				}
			}
			return stringBuilder.ToString();
		}

		protected void Write(object content)
		{
			this.Write(null, content);
		}

		protected void Write(string label, object content)
		{
			DiagnosticsPage.Write(base.Response.Output, label, content, null);
		}

		internal static void Write(TextWriter output, string label, object content, string format = null)
		{
			if (format == null)
			{
				format = ((label == null) ? "<div class='diagLn'><span class='diagTxt'>{0}</span></div>" : "<div class='diagLn'><span class='diagLbl'>{0}</span> <span class='diagTxt'>{1}</span></div>");
			}
			label = ((label == null) ? string.Empty : HttpUtility.HtmlEncode(label));
			string arg = (content == null) ? string.Empty : HttpUtility.HtmlEncode(content.ToString());
			output.Write(format ?? "<div class='diagLn'><span class='diagLbl'>{0}</span> <span class='diagTxt'>{1}</span></div>", label, arg);
		}

		private const string TitleFormat = "<div class='diagTitle'>{0}</div>";

		protected const string BlockStart = "<div class='diagBlock'>";

		protected const string BlockEnd = "</div>";

		private const string LabelAndContentFormat = "<div class='diagLn'><span class='diagLbl'>{0}</span> <span class='diagTxt'>{1}</span></div>";

		private const string ContentFormat = "<div class='diagLn'><span class='diagTxt'>{0}</span></div>";
	}
}
