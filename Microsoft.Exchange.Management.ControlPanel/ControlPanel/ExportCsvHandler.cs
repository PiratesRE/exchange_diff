using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ExportCsvHandler : IDownloadHandler
	{
		static ExportCsvHandler()
		{
			ExportCsvHandler.GetListExportCsvDefaultResultSize = ConfigUtil.ReadInt("GetListExportCsvDefaultResultSize", 20000);
		}

		public PowerShellResults ProcessRequest(HttpContext context)
		{
			string text = context.Request.QueryString["schema"];
			string text2 = context.Request.Form["workflowOutput"];
			string text3 = context.Request.Form["titlesCSV"];
			string text4 = context.Request.Form["PropertyList"];
			string filter = context.Request.Form["filter"];
			if (string.IsNullOrEmpty(text2))
			{
				throw new BadQueryParameterException("workflowOutput");
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new BadQueryParameterException("schema");
			}
			if (string.IsNullOrEmpty(text3))
			{
				throw new BadQueryParameterException("titlesCSV");
			}
			if (string.IsNullOrEmpty(text4))
			{
				throw new BadQueryParameterException("PropertyList");
			}
			string[] columnList = text2.Split(new char[]
			{
				','
			});
			Stream outputStream = context.Response.OutputStream;
			PowerShellResults<JsonDictionary<object>> powerShellResults;
			try
			{
				context.Response.ContentType = "text/csv";
				context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", HttpUtility.UrlEncode(Strings.ExportCsvFileName + ".csv")));
				context.Response.ContentEncoding = Encoding.UTF8;
				context.Response.Charset = "utf-8";
				outputStream.Write(new byte[]
				{
					239,
					187,
					191
				}, 0, 3);
				byte[] bytes = Encoding.UTF8.GetBytes(new StringBuilder(text3.PadRight(256)).AppendLine().ToString());
				outputStream.Write(bytes, 0, bytes.Length);
				powerShellResults = this.GetPowerShellResult(text2, text4, text, filter);
				if (context.Response.IsClientConnected && powerShellResults.Succeeded && powerShellResults.Output.Length > 0)
				{
					JsonDictionary<object>[] output = powerShellResults.Output;
					this.WriteOnePageToFile(outputStream, output, columnList);
				}
			}
			catch (IOException exception)
			{
				powerShellResults = new PowerShellResults<JsonDictionary<object>>();
				powerShellResults.ErrorRecords = new ErrorRecord[]
				{
					new ErrorRecord(exception)
				};
			}
			catch (HttpException exception2)
			{
				powerShellResults = new PowerShellResults<JsonDictionary<object>>();
				powerShellResults.ErrorRecords = new ErrorRecord[]
				{
					new ErrorRecord(exception2)
				};
			}
			finally
			{
				if (outputStream != null)
				{
					outputStream.Close();
				}
			}
			return powerShellResults;
		}

		private void WriteOnePageToFile(Stream outputStream, JsonDictionary<object>[] output, string[] columnList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (output != null && output.Length > 0)
			{
				for (int i = 0; i < output.Length; i++)
				{
					Dictionary<string, object> row = output[i];
					this.GetOneRowResult(stringBuilder, row, columnList);
				}
			}
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			outputStream.Write(bytes, 0, bytes.Length);
		}

		private void GetOneRowResult(StringBuilder sb, Dictionary<string, object> row, string[] columnList)
		{
			int num = columnList.Length;
			for (int i = 0; i < columnList.Length; i++)
			{
				object obj = row[columnList[i]];
				string text = (obj == null) ? string.Empty : obj.ToString();
				if (text != null)
				{
					if (text.IndexOfAny(ExportCsvHandler.SpecialChars) < 0)
					{
						sb.Append(text);
					}
					else
					{
						sb.Append("\"");
						sb.Append(text.Replace("\"", "\"\""));
						sb.Append("\"");
					}
				}
				if (--num > 0)
				{
					sb.Append(",");
				}
			}
			sb.AppendLine(string.Empty);
		}

		private PowerShellResults<JsonDictionary<object>> GetPowerShellResult(string outputColumn, string properties, string schema, string filter)
		{
			IDDIService iddiservice = (IDDIService)new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=" + schema).ServiceInstance;
			DDIParameters ddiparameters = new DDIParameters();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["workflowOutput"] = outputColumn;
			dictionary["PropertyList"] = properties;
			if (!string.IsNullOrEmpty(filter))
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				Dictionary<string, object> dictionary2 = javaScriptSerializer.Deserialize<Dictionary<string, object>>(filter);
				foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			ddiparameters.Parameters = new JsonDictionary<object>(dictionary);
			return iddiservice.GetList(ddiparameters, null);
		}

		public static bool IsExportCsv
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				return httpContext != null && httpContext.Request.QueryString["handlerClass"] == "ExportCsvHandler";
			}
		}

		private const string TitlesCSV = "titlesCSV";

		private const string Schema = "schema";

		private const string PropertyList = "PropertyList";

		private const string Filter = "filter";

		private const string SearchText = "SearchText";

		public static readonly int GetListExportCsvDefaultResultSize;

		private static readonly char[] SpecialChars = new char[]
		{
			',',
			'"',
			'\r',
			'\t',
			'\n'
		};
	}
}
