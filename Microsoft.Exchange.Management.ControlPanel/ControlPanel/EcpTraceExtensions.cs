using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class EcpTraceExtensions
	{
		public static string ToTraceString(this PSCommand command)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Command command2 in command.Commands)
			{
				stringBuilder.Append(command2.CommandText);
				stringBuilder.Append(" ");
				foreach (CommandParameter commandParameter in command2.Parameters)
				{
					string format = (commandParameter.Value != null && commandParameter.Value.GetType() == typeof(bool)) ? "-{0}:{1} " : "-{0} {1} ";
					stringBuilder.AppendFormat(format, commandParameter.Name, EcpTraceExtensions.FormatParameterValue(commandParameter.Value));
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		internal static string FormatParameterValue(object value)
		{
			if (value is IDictionary)
			{
				return (value as IDictionary).ToTraceString();
			}
			if (value is IEnumerable && !(value is string))
			{
				return (value as IEnumerable).ToTraceString();
			}
			return EcpTraceExtensions.FormatNonListParameterValue(value);
		}

		private static string FormatNonListParameterValue(object value)
		{
			if (value == null)
			{
				return "$null";
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.DBNull:
				return "$null";
			case TypeCode.Boolean:
				if (!(bool)value)
				{
					return "$false";
				}
				return "$true";
			case TypeCode.Char:
				return "[char]'" + value.ToString() + "'";
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return value.ToString();
			default:
			{
				string text;
				if (value is string)
				{
					text = (value as string);
				}
				else if (value is Identity)
				{
					text = (value as Identity).RawIdentity;
				}
				else
				{
					text = value.ToString();
					if (text == value.GetType().FullName)
					{
						return "[" + text + "]";
					}
				}
				if (text != null)
				{
					return "'" + text.Replace("'", "''") + "'";
				}
				return "$null";
			}
			}
		}

		public static EcpTraceFormatter<PSCommand> GetTraceFormatter(this PSCommand command)
		{
			return new EcpTraceFormatter<PSCommand>(command);
		}

		public static string ToTraceString(this ErrorRecord[] records)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ErrorRecord errorRecord in records)
			{
				stringBuilder.AppendLine(errorRecord.Exception.ToString());
			}
			return stringBuilder.ToString();
		}

		public static string ToLogString(this ErrorRecord[] records)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < records.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(records[i].Exception.GetType().ToString());
				stringBuilder.Append(":");
				stringBuilder.Append(records[i].Exception.GetFullMessage().ToString());
			}
			return stringBuilder.ToString();
		}

		public static EcpTraceFormatter<ErrorRecord[]> GetTraceFormatter(this ErrorRecord[] records)
		{
			return new EcpTraceFormatter<ErrorRecord[]>(records);
		}

		public static string ToTraceString(this PowerShellResults<JsonDictionary<object>> results)
		{
			return results.ToJsonString(DDIService.KnownTypes.Value);
		}

		public static string ToTraceString(this IDictionary collection)
		{
			if (collection == null)
			{
				return "$null";
			}
			StringBuilder stringBuilder = new StringBuilder("@{");
			foreach (object obj in collection)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				stringBuilder.Append(string.Format("{0}={1},", EcpTraceExtensions.FormatNonListParameterValue(dictionaryEntry.Key), EcpTraceExtensions.FormatNonListParameterValue(dictionaryEntry.Value)));
			}
			if (stringBuilder[stringBuilder.Length - 1] == ',')
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static string ToTraceString(this IEnumerable pipeline)
		{
			if (pipeline == null)
			{
				return "$null";
			}
			if (pipeline is string)
			{
				return pipeline as string;
			}
			StringBuilder stringBuilder = new StringBuilder("@(");
			foreach (object value in pipeline)
			{
				stringBuilder.Append(EcpTraceExtensions.FormatNonListParameterValue(value) + ",");
			}
			if (stringBuilder[stringBuilder.Length - 1] == ',')
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static EcpTraceFormatter<Exception> GetTraceFormatter(this Exception exception)
		{
			return new EcpTraceFormatter<Exception>(exception);
		}
	}
}
