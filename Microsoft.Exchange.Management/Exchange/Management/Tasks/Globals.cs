using System;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.ServicesServerTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class Globals
	{
		public static ExchangeResourceManager ResourceManager
		{
			get
			{
				return Globals.resourceManager;
			}
		}

		public static Guid ComponentGuid
		{
			get
			{
				return ExTraceGlobals.TaskTracer.Category;
			}
		}

		public static string PowerShellStringFromMultivaluedParameter(object[] values)
		{
			if (values == null || values.Length == 0)
			{
				return "$null";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < values.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				string text = (values[i] == null) ? null : values[i].ToString();
				if (text != null)
				{
					text = text.Replace("`", "``");
					text = text.Replace("$", "`$");
					text = text.Replace("\"", "`\"");
					stringBuilder.AppendFormat("\"{0}\"", text);
				}
				else
				{
					stringBuilder.Append("");
				}
			}
			return stringBuilder.ToString();
		}

		public static string PowerShellArrayFromStringArray(string[] values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("@(");
			bool flag = true;
			foreach (string text in values)
			{
				string arg = text.Replace("'", "''");
				if (flag)
				{
					stringBuilder.AppendFormat("'{0}'", arg);
				}
				else
				{
					stringBuilder.AppendFormat(",'{0}'", arg);
				}
				flag = false;
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private static readonly ExchangeResourceManager resourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Tasks.Strings", Assembly.GetExecutingAssembly());
	}
}
