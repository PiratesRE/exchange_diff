using System;
using System.Globalization;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaPerformanceData
	{
		public string RequestType
		{
			get
			{
				return this.requestType;
			}
			set
			{
				this.requestType = value;
			}
		}

		public string RowId
		{
			get
			{
				return this.clientRowId;
			}
			set
			{
				this.clientRowId = value;
			}
		}

		public long TotalLatency
		{
			get
			{
				return this.totalLatency;
			}
			set
			{
				this.totalLatency = value;
			}
		}

		public int RpcLatency
		{
			get
			{
				return this.rpcLatency;
			}
			set
			{
				this.rpcLatency = value;
			}
		}

		public uint RpcCount
		{
			set
			{
				this.rpcCount = (long)((ulong)value);
			}
		}

		public uint LdapCount
		{
			set
			{
				this.ldapCount = (long)((ulong)value);
			}
		}

		public int LdapLatency
		{
			set
			{
				this.ldapLatency = value;
			}
		}

		public long KilobytesAllocated
		{
			set
			{
				this.kilobytesAllocated = value;
			}
		}

		public void TraceOther(string trace)
		{
			if (this.other == null)
			{
				this.other = new StringBuilder();
			}
			if (this.other.Length != 0)
			{
				this.other.Append(",");
			}
			this.other.Append(trace ?? "(null)");
		}

		public OwaPerformanceData(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.requestUrl = request.Url;
			this.clientRowId = request.Headers["X-OWA-PerfConsoleRowId"];
		}

		public void RetrieveFinishJS(int rowId, out string js)
		{
			js = "fnshRq('srv" + rowId + "');";
		}

		public void SetFormRequestType(string formName)
		{
			if (formName != null)
			{
				string[] array = formName.Split(new char[]
				{
					'/'
				});
				if (array != null)
				{
					formName = array[array.Length - 1];
					int num = formName.LastIndexOf('.');
					if (num >= 0)
					{
						formName = formName.Substring(0, num);
					}
				}
				this.RequestType = formName;
			}
		}

		public void SetOehRequestType(string handlerName, string methodName)
		{
			int num = handlerName.IndexOf("EventHandler", 0, StringComparison.OrdinalIgnoreCase);
			if (num > 0)
			{
				handlerName = handlerName.Substring(0, num);
			}
			this.RequestType = handlerName + " " + methodName;
		}

		public void RetrieveHtmlForPerfData(int rowId, out string html, bool finished, int index)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append("<tr id='srv");
			stringBuilder.Append(rowId);
			if (finished)
			{
				stringBuilder.Append("' pndFn='1'");
			}
			stringBuilder.Append("'>");
			stringBuilder.Append("<td class='prfCell noLBrd'>");
			stringBuilder.Append(index);
			stringBuilder.Append("</td>");
			this.AppendDefaultTdToHtmlRender(string.Concat(new object[]
			{
				"<a href='#' _url='",
				this.RequestUrl,
				"'>",
				this.requestType,
				"</a>"
			}), stringBuilder);
			this.AppendDefaultTdToHtmlRender("(Unknown)", stringBuilder);
			this.AppendDefaultTdToHtmlRender("(Unknown)", stringBuilder);
			this.AppendDefaultTdToHtmlRender(this.totalLatency, stringBuilder, "&nbsp;ms");
			this.AppendDefaultTdToHtmlRender(this.rpcCount, stringBuilder);
			this.AppendDefaultTdToHtmlRender(this.rpcLatency, stringBuilder, "&nbsp;ms");
			this.AppendDefaultTdToHtmlRender(this.ldapCount, stringBuilder);
			this.AppendDefaultTdToHtmlRender(this.ldapLatency, stringBuilder, "&nbsp;ms");
			this.AppendDefaultTdToHtmlRender(this.kilobytesAllocated, stringBuilder, "&nbsp;kB");
			this.AppendDefaultTdToHtmlRender((this.other != null) ? Utilities.HtmlEncode(this.other.ToString()) : null, stringBuilder);
			stringBuilder.Append("</tr>");
			html = stringBuilder.ToString();
		}

		private void AppendDefaultTdToHtmlRender(long content, StringBuilder builder)
		{
			this.AppendDefaultTdToHtmlRender((content == long.MinValue || content < 0L) ? null : content.ToString(), builder);
		}

		private void AppendDefaultTdToHtmlRender(long content, StringBuilder builder, string add)
		{
			this.AppendDefaultTdToHtmlRender((content == long.MinValue || content < 0L) ? null : (content.ToString() + add), builder);
		}

		private void AppendDefaultTdToHtmlRender(int content, StringBuilder builder)
		{
			this.AppendDefaultTdToHtmlRender((content == int.MinValue || content < 0) ? null : content.ToString(), builder);
		}

		private void AppendDefaultTdToHtmlRender(int content, StringBuilder builder, string add)
		{
			this.AppendDefaultTdToHtmlRender((content == int.MinValue || content < 0) ? null : (content.ToString() + add), builder);
		}

		private void AppendDefaultTdToHtmlRender(string content, StringBuilder builder)
		{
			builder.Append("<td class='prfCell'>");
			if (content != null)
			{
				builder.Append(content);
			}
			builder.Append("</td>");
		}

		private void AppendAttributeTdToHtmlRender(string attribute, StringBuilder builder)
		{
			builder.Append("<td class='prfCell' ");
			if (attribute != null)
			{
				builder.Append(attribute);
			}
			builder.Append("></td>");
		}

		public void RetrieveJSforPerfData(int rowId, out string js)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append("[null");
			if (this.requestType != null)
			{
				stringBuilder.Append(",'" + this.requestType + "'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			stringBuilder.Append(",'(Unknown)','(Unknown)'");
			if (this.totalLatency >= 0L)
			{
				stringBuilder.Append(",'" + this.totalLatency + "&nbsp;ms'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			if (this.rpcCount >= 0L)
			{
				stringBuilder.Append(",'" + this.rpcCount + "'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			if (this.rpcLatency >= 0)
			{
				stringBuilder.Append(",'" + this.rpcLatency + "&nbsp;ms'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			if (this.ldapCount >= 0L)
			{
				stringBuilder.Append(",'" + this.ldapCount + "'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			if (this.ldapLatency >= 0)
			{
				stringBuilder.Append(",'" + this.ldapLatency + "&nbsp;ms'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			if (this.kilobytesAllocated >= 0L)
			{
				stringBuilder.Append(",'" + this.kilobytesAllocated + "&nbsp;kB'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			if (this.other != null && this.other.Length > 0)
			{
				stringBuilder.Append(",'" + Utilities.JavascriptEncode(this.other.ToString()) + "'");
			}
			else
			{
				stringBuilder.Append(",null");
			}
			stringBuilder.Append("]");
			js = string.Format(CultureInfo.InvariantCulture, "srvUpdtRw('srv{0}',{1});", new object[]
			{
				rowId,
				stringBuilder
			});
		}

		public Uri RequestUrl
		{
			get
			{
				return this.requestUrl;
			}
		}

		public void GenerateInitialPayload(StringBuilder builder, int rowId)
		{
			builder.Append("excPrfCnsl(\"");
			builder.Append("dtFrmSrv('");
			builder.Append(Utilities.JavascriptEncode(this.RequestUrl.ToString()));
			builder.Append("','srv");
			builder.Append(rowId);
			builder.Append("',null,'");
			builder.Append(this.RowId);
			if (this.RequestType != null)
			{
				builder.Append("','");
				builder.Append(Utilities.JavascriptEncode(this.RequestType));
			}
			builder.Append("');");
			builder.Append("\");");
		}

		private const string UPDATEALLROWFORMAT = "srvUpdtRw('srv{0}',{1});";

		private const string UPDATEDATEROWFORMAT = "chngDtTb({0},{1},'{2}',{3});";

		public const int ConsoleColumCount = 11;

		private long ldapCount = long.MinValue;

		private int ldapLatency = int.MinValue;

		private long rpcCount = long.MinValue;

		private int rpcLatency = int.MinValue;

		private long kilobytesAllocated = long.MinValue;

		private StringBuilder other;

		private Uri requestUrl;

		private long totalLatency = long.MinValue;

		private string clientRowId;

		private string requestType;
	}
}
