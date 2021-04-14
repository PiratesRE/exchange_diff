using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MapiHttp
{
	internal static class MapiHttpStatusPage
	{
		internal static async Task ExecuteStatusAsync(HttpContextBase context, string endpointVdirPath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string showDebugParam;
			if (!context.TryGetShowDebugParameter(out showDebugParam))
			{
				showDebugParam = string.Empty;
			}
			bool showAdvanced = string.Compare(showDebugParam, "full", true) == 0;
			bool showDebug = showAdvanced || string.Compare(showDebugParam, "yes", true) == 0;
			stringBuilder.Append("<html>\r\n<head>\r\n<title>Exchange MAPI/HTTP Connectivity Endpoint</title>\r\n</head>\r\n<body>\r\n");
			string version = string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				15,
				0,
				1497,
				12
			});
			stringBuilder.AppendFormat("<p>Exchange MAPI/HTTP Connectivity Endpoint<br><br>Version: {0}<br>Vdir Path: {1}<br><br></p>", version, context.HtmlEncode(endpointVdirPath));
			string authIdentifier;
			if (!context.TryGetUserAuthIdentifier(out authIdentifier))
			{
				authIdentifier = string.Empty;
			}
			string userName;
			string userPrincipalName;
			string userSecurityIdentifier;
			string authenticationType;
			string organization;
			if (context.TryGetUserIdentityInfo(out userName, out userPrincipalName, out userSecurityIdentifier, out authenticationType, out organization))
			{
				stringBuilder.AppendFormat("<p><b>User:</b> {0}<br><b>UPN:</b> {1}<br><b>SID:</b> {2}<br><b>Organization:</b> {3}<br><b>Authentication:</b> {4}", new object[]
				{
					context.HtmlEncode(userName ?? string.Empty),
					context.HtmlEncode(userPrincipalName ?? string.Empty),
					context.HtmlEncode(userSecurityIdentifier ?? string.Empty),
					context.HtmlEncode(organization ?? string.Empty),
					context.HtmlEncode(authenticationType ?? string.Empty)
				});
			}
			else
			{
				stringBuilder.AppendFormat("<p><b>Identity:</b> {0}", string.IsNullOrWhiteSpace(context.User.Identity.Name) ? "Anonymous" : context.HtmlEncode(context.User.Identity.Name));
			}
			if (showAdvanced)
			{
				stringBuilder.AppendFormat("<br><b>AuthIdentifier:</b> {0}", context.HtmlEncode(authIdentifier ?? string.Empty));
			}
			stringBuilder.Append("</p><br>");
			string cafeServerName;
			if (!context.TryGetSourceCafeServer(out cafeServerName))
			{
				cafeServerName = string.Empty;
			}
			stringBuilder.AppendFormat("<p><b>Cafe:</b> {0}<br><b>Mailbox:</b> {1}</p>", context.HtmlEncode(cafeServerName.ToLower()), context.HtmlEncode(MapiHttpStatusPage.GetLocalServerFqdn()));
			if (showDebug)
			{
				stringBuilder.Append("<p><br><b>Session Contexts</b><br></p>\r\n");
				SessionContextInfo[] array = null;
				if (!string.IsNullOrEmpty(authIdentifier) && !SessionContextManager.TryGetSessionContextInfo(authIdentifier, out array))
				{
					array = Array<SessionContextInfo>.Empty;
				}
				if (array != null && array.Length > 0)
				{
					foreach (SessionContextInfo sessionContextInfo in array)
					{
						TimeSpan timeSpan = TimeSpan.Zero;
						if (sessionContextInfo.ActivityCount == 0)
						{
							timeSpan = ExDateTime.UtcNow - sessionContextInfo.LastActivityTime;
						}
						stringBuilder.AppendFormat("<p><b>Context Cookie:</b> {0}<br><b>Creation Time:</b> {1}<br><b>Last Activity Time:</b> {2}<br><b>Activity Count:</b> {3}<br><b>Rundown:</b> {4}<br><b>Rundown Time:</b> {5}<br><b>Idle Time:</b> {6}</p>", new object[]
						{
							context.HtmlEncode(sessionContextInfo.ContextCookie),
							context.HtmlEncode(sessionContextInfo.CreationTime.ToString()),
							context.HtmlEncode(sessionContextInfo.LastActivityTime.ToString()),
							sessionContextInfo.ActivityCount,
							(sessionContextInfo.RundownReason != null) ? context.HtmlEncode(sessionContextInfo.RundownReason.Value.ToString()) : "No",
							(sessionContextInfo.RundownReason != null) ? context.HtmlEncode(sessionContextInfo.RundownTime.ToString()) : string.Empty,
							(sessionContextInfo.ActivityCount == 0) ? context.HtmlEncode(timeSpan.ToString("d\\.hh\\:mm\\:ss")) : string.Empty
						});
						MapiHttpStatusPage.AddAsyncOperationInfo(context, stringBuilder, "Completed Operations", sessionContextInfo.CompletedAsyncOperations);
						MapiHttpStatusPage.AddAsyncOperationInfo(context, stringBuilder, "Failed Operations", sessionContextInfo.FailedAsyncOperations);
						MapiHttpStatusPage.AddAsyncOperationInfo(context, stringBuilder, "Active Operations", sessionContextInfo.ActiveAsyncOperations);
						stringBuilder.Append("<br><br>");
					}
				}
				else
				{
					stringBuilder.Append("<p>None</p>\r\n");
				}
			}
			stringBuilder.AppendFormat("<p><br><br><br><b>Created:</b> {0}</p>", context.HtmlEncode(ExDateTime.UtcNow.ToString()));
			stringBuilder.Append("</body></html>");
			byte[] responseData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			context.Response.ContentType = "text/html";
			context.Response.StatusCode = 200;
			await context.WriteResponseBuffersAsync(new ArraySegment<byte>(responseData), null, true);
		}

		private static string GetLocalServerFqdn()
		{
			if (MapiHttpStatusPage.localServerFqdn == null)
			{
				MapiHttpStatusPage.localServerFqdn = LocalServer.GetServer().Fqdn.ToLower();
			}
			return MapiHttpStatusPage.localServerFqdn;
		}

		private static void AddAsyncOperationInfo(HttpContextBase context, StringBuilder stringBuilder, string infoName, AsyncOperationInfo[] asyncOperationInfoArray)
		{
			if (asyncOperationInfoArray.Length > 0)
			{
				stringBuilder.AppendFormat("<p><b>{0}</b><br><br><table border=\"1\">\r\n", infoName);
				stringBuilder.Append("<tr><th>Request Type</th><th>Request Id</th><th>Sequence Cookie</th><th>Start Time</th><th>End Time</th><th>Duration</th><th>Pending Count</th><th>Last Pending</th><th>Cafe</th><th>Cafe Activity Id</th><th>Client Address</th><th>Info</th></tr>\r\n");
				foreach (AsyncOperationInfo asyncOperationInfo in asyncOperationInfoArray)
				{
					TimeSpan timeSpan;
					if (asyncOperationInfo.EndTime != null)
					{
						timeSpan = (asyncOperationInfo.EndTime - asyncOperationInfo.StartTime).Value;
					}
					else
					{
						timeSpan = ExDateTime.UtcNow - asyncOperationInfo.StartTime;
					}
					string text = (asyncOperationInfo.FailureException != null) ? context.HtmlEncode(asyncOperationInfo.FailureException.ToString()) : string.Empty;
					stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td><center>{6}</center></td><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td></tr>\r\n", new object[]
					{
						(!string.IsNullOrEmpty(asyncOperationInfo.RequestType)) ? context.HtmlEncode(asyncOperationInfo.RequestType) : "None",
						(!string.IsNullOrEmpty(asyncOperationInfo.RequestId)) ? context.HtmlEncode(asyncOperationInfo.RequestId) : "None",
						(!string.IsNullOrWhiteSpace(asyncOperationInfo.SequenceCookie)) ? context.HtmlEncode(asyncOperationInfo.SequenceCookie) : "None",
						context.HtmlEncode(asyncOperationInfo.StartTime.ToString()),
						(asyncOperationInfo.EndTime != null) ? context.HtmlEncode(asyncOperationInfo.EndTime.ToString()) : string.Empty,
						context.HtmlEncode(timeSpan.ToString("d\\.hh\\:mm\\:ss")),
						asyncOperationInfo.PendingCount,
						(asyncOperationInfo.LastPendingTime != null) ? context.HtmlEncode(asyncOperationInfo.LastPendingTime.ToString()) : string.Empty,
						(!string.IsNullOrEmpty(asyncOperationInfo.SourceCafeServer)) ? context.HtmlEncode(asyncOperationInfo.SourceCafeServer.ToLower()) : string.Empty,
						(!string.IsNullOrEmpty(asyncOperationInfo.CafeActivityId)) ? context.HtmlEncode(asyncOperationInfo.CafeActivityId) : string.Empty,
						(!string.IsNullOrEmpty(asyncOperationInfo.ClientAddress)) ? context.HtmlEncode(asyncOperationInfo.ClientAddress) : string.Empty,
						text
					});
				}
				stringBuilder.Append("</table></p>\r\n");
				return;
			}
			stringBuilder.AppendFormat("<p><b>{0}</b><br><br>None</p>", infoName);
		}

		private const string ParameterShowAdvancedActivationValue = "full";

		private static string localServerFqdn;
	}
}
