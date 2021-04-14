using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public static class CrimsonHelper
	{
		public static string GetChannelName<T>()
		{
			return string.Format("Microsoft-Exchange-ActiveMonitoring/{0}", typeof(T).Name);
		}

		public static ResultSeverityLevel ConvertResultTypeToSeverityLevel(ResultType resultType)
		{
			ResultSeverityLevel result;
			switch (resultType)
			{
			case ResultType.Succeeded:
				result = ResultSeverityLevel.Informational;
				break;
			case ResultType.Failed:
				result = ResultSeverityLevel.Error;
				break;
			default:
				result = ResultSeverityLevel.Warning;
				break;
			}
			return result;
		}

		public static bool ParseIntStringAsBool(string strValue)
		{
			return int.Parse(strValue, CultureInfo.InvariantCulture) != 0;
		}

		public static CrimsonBookmarker ConstructBookmarker(string resultClassName, string serviceName)
		{
			if (string.IsNullOrEmpty(serviceName))
			{
				serviceName = "*";
			}
			return new CrimsonBookmarker(string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\V15\\ActiveMonitoring\\{1}\\{0}\\Bookmark", resultClassName, serviceName));
		}

		public static EventBookmark ReadBookmark(string resultClassName, string serviceName, string bookmarkName)
		{
			if (string.IsNullOrEmpty(bookmarkName))
			{
				bookmarkName = "Default";
			}
			CrimsonBookmarker crimsonBookmarker = CrimsonHelper.ConstructBookmarker(resultClassName, serviceName);
			return crimsonBookmarker.Read(bookmarkName);
		}

		public static void WriteBookmark(string resultClassName, string serviceName, string bookmarkName, EventBookmark bookmark)
		{
			if (string.IsNullOrEmpty(bookmarkName))
			{
				bookmarkName = "Default";
			}
			CrimsonBookmarker crimsonBookmarker = CrimsonHelper.ConstructBookmarker(resultClassName, serviceName);
			crimsonBookmarker.Write(bookmarkName, bookmark);
		}

		public static void DeleteBookmark(string resultClassName, string serviceName, string bookmarkName)
		{
			if (string.IsNullOrEmpty(bookmarkName))
			{
				bookmarkName = "Default";
			}
			CrimsonBookmarker crimsonBookmarker = CrimsonHelper.ConstructBookmarker(resultClassName, serviceName);
			crimsonBookmarker.Delete(bookmarkName);
		}

		public static string BuildXPathQueryString(string channelName, string serviceName, DateTime? startTime, DateTime? endTime, string propertyConstraint)
		{
			string text = string.Empty;
			if (startTime != null)
			{
				text = string.Format("@SystemTime&gt;='{0}'", startTime.Value.ToUniversalTime().ToString("o"));
			}
			string text2 = string.Empty;
			if (endTime != null)
			{
				text2 = string.Format("@SystemTime&lt;='{0}'", endTime.Value.ToUniversalTime().ToString("o"));
			}
			string text3 = null;
			if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
			{
				text3 = string.Format("System[TimeCreated[{0}{1}{2}]]", text, (startTime != null && endTime != null) ? " and " : string.Empty, text2);
			}
			string str = string.Empty;
			if (!string.IsNullOrEmpty(text3) && (!string.IsNullOrEmpty(serviceName) || !string.IsNullOrEmpty(propertyConstraint)))
			{
				str = " and ";
			}
			string text4 = string.Empty;
			string arg = string.Empty;
			if (!string.IsNullOrEmpty(serviceName))
			{
				text4 = string.Format("(ServiceName='{0}')", serviceName);
				if (!string.IsNullOrEmpty(propertyConstraint))
				{
					arg = " and ";
				}
			}
			string str2 = string.Empty;
			if (!string.IsNullOrEmpty(text4) || !string.IsNullOrEmpty(propertyConstraint))
			{
				str2 = string.Format("UserData[EventXML[{0}{1}{2}]]", text4, arg, propertyConstraint);
			}
			string text5 = text3 + str + str2;
			string result = "*";
			if (!string.IsNullOrEmpty(text5))
			{
				string format = "\r\n                    <QueryList>\r\n                      <Query Id=\"0\" Path=\"{0}\">\r\n                        <Select Path=\"{0}\">\r\n                        *[{1}]\r\n                        </Select>\r\n                      </Query>\r\n                    </QueryList>";
				result = string.Format(format, channelName, text5);
			}
			return result;
		}

		public static string NullDecode(string inputStr)
		{
			string result = inputStr;
			if (!string.IsNullOrEmpty(inputStr))
			{
				char c = "[null]"[0];
				if (inputStr[0] == c)
				{
					int num = inputStr.IndexOf("[null]");
					if (num != -1)
					{
						if (num == 0 && inputStr.Length == "[null]".Length)
						{
							result = null;
						}
						else
						{
							result = inputStr.Substring(1, inputStr.Length - 2);
						}
					}
				}
			}
			return result;
		}

		public static string NullCode(string inputStr)
		{
			string result = inputStr;
			if (inputStr == null)
			{
				result = "[null]";
			}
			else if (!string.IsNullOrEmpty(inputStr))
			{
				char c = "[null]"[0];
				char c2 = "[null]"["[null]".Length - 1];
				if (inputStr[0] == c && inputStr.Contains("[null]"))
				{
					result = string.Format("{0}{1}{2}", c, inputStr, c2);
				}
			}
			return result;
		}

		public static double ParseDouble(string input)
		{
			double result = 0.0;
			try
			{
				result = double.Parse(input, CultureInfo.InvariantCulture);
			}
			catch (FormatException ex)
			{
				string arg = string.Format("'{0}' not a valid double value. Defaulting to 0.0", input);
				WTFDiagnostics.TraceError<string, string>(WTFLog.DataAccess, TracingContext.Default, "[ParseError] : {0} {1}.", arg, ex.ToString(), null, "ParseDouble", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonHelper.cs", 364);
			}
			return result;
		}

		public static float ParseFloat(string input)
		{
			float result = 0f;
			try
			{
				result = float.Parse(input, CultureInfo.InvariantCulture);
			}
			catch (FormatException ex)
			{
				string arg = string.Format("'{1}' not a valid float value. Defaulting to 0", input);
				WTFDiagnostics.TraceError<string, string>(WTFLog.DataAccess, TracingContext.Default, "[ParseError] : {0} {1}.", arg, ex.ToString(), null, "ParseFloat", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonHelper.cs", 386);
			}
			return result;
		}

		public static void ClearCrimsonChannel<T>()
		{
			if (!NativeMethods.EvtClearLog(IntPtr.Zero, CrimsonHelper.GetChannelName<T>(), null, 0))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		public static string ConvertDictionaryToXml(Dictionary<string, string> properties)
		{
			string result = string.Empty;
			if (properties != null && properties.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				stringBuilder.AppendFormat("<Properties>\n", new object[0]);
				foreach (string text in properties.Keys)
				{
					string arg = SecurityElement.Escape(properties[text].ToString());
					stringBuilder.AppendFormat("   <{0}>{1}</{0}>", text, arg);
				}
				stringBuilder.AppendFormat("</Properties>", new object[0]);
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static Dictionary<string, string> ConvertXmlToDictionary(string customXml)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(customXml))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(customXml);
				using (XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Properties"))
				{
					if (elementsByTagName != null && elementsByTagName.Count > 0)
					{
						XmlNode xmlNode = elementsByTagName.Item(0);
						using (XmlNodeList childNodes = xmlNode.ChildNodes)
						{
							foreach (object obj in childNodes)
							{
								XmlNode xmlNode2 = (XmlNode)obj;
								dictionary.Add(xmlNode2.Name, xmlNode2.InnerText);
							}
						}
					}
				}
			}
			return dictionary;
		}

		public static string Serialize(Dictionary<string, object> tempResults, bool replaceFlag)
		{
			StringBuilder stringBuilder = new StringBuilder(tempResults.Count * 16);
			foreach (KeyValuePair<string, object> keyValuePair in tempResults)
			{
				string text = "[null]";
				if (keyValuePair.Value != null)
				{
					if (keyValuePair.Value is DateTime)
					{
						text = ((DateTime)keyValuePair.Value).ToString("o", CultureInfo.InvariantCulture);
					}
					else if (keyValuePair.Value is int)
					{
						text = ((int)keyValuePair.Value).ToString(CultureInfo.InvariantCulture);
					}
					else if (keyValuePair.Value is float)
					{
						text = ((float)keyValuePair.Value).ToString(CultureInfo.InvariantCulture);
					}
					else if (keyValuePair.Value is double)
					{
						text = ((double)keyValuePair.Value).ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						text = keyValuePair.Value.ToString();
					}
					text = text.Replace('|', '_');
				}
				stringBuilder.Append(text);
				stringBuilder.Append('|');
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length--;
			}
			if (replaceFlag)
			{
				stringBuilder.Replace("\"", "\"\"");
				stringBuilder.Replace(Environment.NewLine, "\\\\r\\\\n");
				stringBuilder.Replace("\n", "\\\\n");
			}
			return stringBuilder.ToString();
		}

		public static string ClearResultString(string str)
		{
			StringBuilder stringBuilder = new StringBuilder(str);
			if (str.StartsWith("\""))
			{
				stringBuilder = stringBuilder.Remove(0, 1);
			}
			if (str.EndsWith("\""))
			{
				stringBuilder.Length--;
			}
			return stringBuilder.ToString();
		}

		private const string NullCodeString = "[null]";

		private const string BookmarkBaseLocationFormat = "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\ActiveMonitoring\\{1}\\{0}\\Bookmark";
	}
}
