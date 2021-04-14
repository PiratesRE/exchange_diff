using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class DDIVUtil
	{
		internal static DataObjectStore BuildDataObjectStore(Service profileBuilder)
		{
			return new DataObjectStore(profileBuilder.DataObjects, profileBuilder.PredefinedTypes.ToArray());
		}

		internal static DataObjectStore GetStore(string xamlName, Service profile)
		{
			DataObjectStore dataObjectStore;
			if (!DDIVUtil.dataObjectStores.TryGetValue(xamlName, out dataObjectStore))
			{
				dataObjectStore = DDIVUtil.BuildDataObjectStore(profile);
				DDIVUtil.dataObjectStores[xamlName] = dataObjectStore;
			}
			return dataObjectStore;
		}

		internal static DataTable GetTable(string xamlName, Service profile, Dictionary<string, List<string>> rbacMetaData)
		{
			DataTable dataTable;
			if (!DDIVUtil.dataTables.TryGetValue(xamlName, out dataTable))
			{
				dataTable = new DataTable();
				IList<Variable> variables = profile.Variables;
				foreach (Variable profile2 in variables)
				{
					dataTable.Columns.Add(AutomatedDataHandlerBase.CreateColumn(profile2, rbacMetaData, DDIVUtil.GetStore(xamlName, profile)));
				}
				DDIVUtil.dataTables[xamlName] = dataTable;
			}
			return dataTable;
		}

		internal static Dictionary<string, List<string>> GetRbacMetaData(string xamlName, Service profile)
		{
			Dictionary<string, List<string>> dictionary;
			if (!DDIVUtil.rbacMetaDatas.TryGetValue(xamlName, out dictionary) && typeof(DDICodeBehind).IsAssignableFrom(profile.Class))
			{
				object obj = Activator.CreateInstance(profile.Class);
				profile.Class.GetMethod("ApplyMetaData").Invoke(obj, new object[0]);
				dictionary = (profile.Class.GetProperty("RbacMetaData").GetValue(obj, null) as Dictionary<string, List<string>>);
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, List<string>>();
			}
			DDIVUtil.rbacMetaDatas[xamlName] = dictionary;
			return dictionary;
		}

		internal static string RetrieveCodesInPostAndPreActions(string workflowName, string xaml, string codeBehind)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xaml);
			string result;
			using (XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(workflowName))
			{
				List<string> functionNames = DDIVUtil.RetrievePostAndPreActions(elementsByTagName[0]);
				result = DDIVUtil.RetrieveCodeFunctions(codeBehind, functionNames);
			}
			return result;
		}

		private static List<string> RetrievePostAndPreActions(XmlNode workflowNode)
		{
			List<string> list = new List<string>();
			if (workflowNode == null)
			{
				return list;
			}
			for (int i = 0; i < workflowNode.ChildNodes.Count; i++)
			{
				list.AddRange(DDIVUtil.RetrievePostAndPreActions(workflowNode.ChildNodes[i]));
			}
			if (workflowNode.Attributes != null)
			{
				if (workflowNode.Attributes["PreAction"] != null)
				{
					list.Add(workflowNode.Attributes["PreAction"].Value);
				}
				if (workflowNode.Attributes["PostAction"] != null)
				{
					list.Add(workflowNode.Attributes["PostAction"].Value);
				}
			}
			return list;
		}

		private static string RetrieveCodeFunctions(string code, List<string> functionNames)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string empty = string.Empty;
			foreach (string str in functionNames)
			{
				int num = code.IndexOf("static void  " + str);
				int num2 = code.IndexOf("end of method", num);
				stringBuilder.Append(code.Substring(num, num2 - num));
			}
			return stringBuilder.ToString();
		}

		internal static bool IsVariableUsedInCode(string code, string varName)
		{
			string pattern = string.Format("\"{0}\"\r\n[\\t a-zA-Z0-9_\\:]+instance object \\[System.Data\\]System.Data.DataRow\\:\\:get_Item\\(string\\)", varName);
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			return regex.Match(code).Success;
		}

		private const string Pattern = "\"{0}\"\r\n[\\t a-zA-Z0-9_\\:]+instance object \\[System.Data\\]System.Data.DataRow\\:\\:get_Item\\(string\\)";

		private static Dictionary<string, DataObjectStore> dataObjectStores = new Dictionary<string, DataObjectStore>();

		private static Dictionary<string, DataTable> dataTables = new Dictionary<string, DataTable>();

		private static Dictionary<string, Dictionary<string, List<string>>> rbacMetaDatas = new Dictionary<string, Dictionary<string, List<string>>>();
	}
}
