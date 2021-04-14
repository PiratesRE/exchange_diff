using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DDIHelper
	{
		static DDIHelper()
		{
			DDIHelper.GetListDefaultResultSize = ConfigUtil.ReadInt("GetListDefaultResultSize", 500);
			DDIHelper.GetListAsyncModeResultSize = ConfigUtil.ReadInt("GetListAsyncModeResultSize", Util.IsDataCenter ? 10000 : 20000);
		}

		internal static void RegisterStrongType2DataContractMapping(Type strongType, Type dataContractType)
		{
			if (DDIHelper.strongType2DataContractMapping.ContainsKey(strongType))
			{
				throw new ArgumentException("Mapping is already provided for strong type :" + strongType.FullName);
			}
			DDIHelper.strongType2DataContractMapping[strongType] = dataContractType;
		}

		internal static object PrepareVariableForSerialization(object value, Variable variable)
		{
			value = (DBNull.Value.Equals(value) ? null : value);
			if (variable.OutputConverter != null && variable.OutputConverter.CanConvert(value))
			{
				value = variable.OutputConverter.Convert(value);
			}
			else
			{
				value = DDIHelper.TryStrongTypeConversion(value);
			}
			if (variable.Type == typeof(bool) && value == null)
			{
				value = false;
			}
			else if (variable.Type == typeof(string) && value == null)
			{
				value = string.Empty;
			}
			else if (value is DateTime)
			{
				value = ((DateTime)value).LocalToUserDateTimeString();
			}
			return value;
		}

		private static object TryStrongTypeConversion(object obj)
		{
			if (!DDIHelper.IsSkippingStrongTypeConversion(obj))
			{
				Type type = obj.GetType();
				Type type2 = null;
				if (DDIHelper.StrongTypeToStringList.Contains(type))
				{
					obj = obj.ToString();
				}
				else if (type == typeof(MultiValuedProperty<string>))
				{
					obj = ((MultiValuedProperty<string>)obj).ToArray();
				}
				else if (!DDIHelper.strongType2DataContractMapping.TryGetValue(type, out type2))
				{
					foreach (KeyValuePair<Type, Type> keyValuePair in DDIHelper.strongType2DataContractMapping)
					{
						if (keyValuePair.Key.IsAssignableFrom(type))
						{
							type2 = keyValuePair.Value;
							break;
						}
					}
				}
				if (type2 == null && (type.IsEnum || typeof(Uri).IsAssignableFrom(type)))
				{
					obj = obj.ToString();
				}
				else if (type2 != null && type2 != null)
				{
					DDIHelper.Trace("Object: {0} of Type '{1}' is converted to '{2}'.", new object[]
					{
						obj,
						type,
						type2
					});
					obj = Activator.CreateInstance(type2, new object[]
					{
						obj
					});
				}
			}
			return obj;
		}

		internal static bool IsSkippingStrongTypeConversion(object obj)
		{
			if (obj == null)
			{
				return true;
			}
			Type type = obj.GetType();
			return type.IsPrimitive || obj is string || type.IsDataContract() || !type.Namespace.StartsWith("Microsoft.Exchange") || (DDIHelper.HasSerializableAttribute(type) && type.Namespace.StartsWith("Microsoft.Exchange.Management.ControlPanel"));
		}

		internal static bool IsDataContract(this Type t)
		{
			return DDIHelper.HasDataContractAttribute(t) || DDIHelper.HasCollectionDataContractAttribute(t);
		}

		private static bool HasDataContractAttribute(Type type)
		{
			if (!DDIHelper.dataContractHashtable.ContainsKey(type))
			{
				DDIHelper.dataContractHashtable[type] = (type.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0);
			}
			return (bool)DDIHelper.dataContractHashtable[type];
		}

		private static bool HasCollectionDataContractAttribute(Type type)
		{
			if (!DDIHelper.collectionDataContractHashtable.ContainsKey(type))
			{
				DDIHelper.collectionDataContractHashtable[type] = (type.GetCustomAttributes(typeof(CollectionDataContractAttribute), false).Length > 0);
			}
			return (bool)DDIHelper.collectionDataContractHashtable[type];
		}

		private static bool HasSerializableAttribute(Type type)
		{
			if (!DDIHelper.serializableHashtable.ContainsKey(type))
			{
				DDIHelper.serializableHashtable[type] = (type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0);
			}
			return (bool)DDIHelper.serializableHashtable[type];
		}

		internal static object ConvertDBNullToNull(DataRow row, string parameterName)
		{
			object obj = row[parameterName];
			if (DBNull.Value.Equals(obj) && row.Table != null)
			{
				obj = null;
			}
			return obj;
		}

		internal static void CheckDataTableForSingleObject(DataTable dataTable)
		{
			if (dataTable.Rows.Count != 1)
			{
				throw new InvalidOperationException();
			}
		}

		internal static DataRow GetLambdaExpressionDataRow(DataTable dataTable)
		{
			if (dataTable.Rows.Count != 1)
			{
				return null;
			}
			return dataTable.Rows[0];
		}

		internal static object GetVariableValue(ICollection<string> modifiedColumns, string variableName, DataRow input, DataTable dataTable, bool isGetListWorkflow)
		{
			if (modifiedColumns.Contains(variableName))
			{
				object obj = DDIHelper.ConvertDBNullToNull(input, variableName);
				if (obj != null || isGetListWorkflow)
				{
					return obj;
				}
			}
			DDIHelper.CheckDataTableForSingleObject(dataTable);
			return DDIHelper.ConvertDBNullToNull(dataTable.Rows[0], variableName);
		}

		internal static object GetVariableValue(VariableReference variableReference, DataRow input, DataTable dataTable)
		{
			if (variableReference.UseInput)
			{
				return DDIHelper.ConvertDBNullToNull(input, variableReference.Variable);
			}
			DDIHelper.CheckDataTableForSingleObject(dataTable);
			return DDIHelper.ConvertDBNullToNull(dataTable.Rows[0], variableReference.Variable);
		}

		internal static List<string> GetCodeBehindRegisteredDepVariables(DataColumn column)
		{
			List<string> list = new List<string>();
			List<string> list2 = column.ExtendedProperties["RbacMetaData"] as List<string>;
			if (list2 != null)
			{
				list.AddRange(list2);
			}
			return list;
		}

		internal static List<string> GetOutputDepVariables(DataColumn column)
		{
			List<string> list = new List<string>();
			Variable variable = column.ExtendedProperties["Variable"] as Variable;
			if (variable != null)
			{
				string text = variable.Value as string;
				if (DDIHelper.IsLambdaExpression(text))
				{
					list = ExpressionCalculator.BuildColumnExpression(text).DependentColumns;
				}
				else
				{
					list.AddRange(DDIHelper.GetCodeBehindRegisteredDepVariables(column));
				}
			}
			return list;
		}

		internal static IEnumerable<string> GetOutputRawVariables(IEnumerable<DataColumn> columns)
		{
			List<string> list = new List<string>();
			foreach (DataColumn dataColumn in columns)
			{
				List<string> outputDepVariables = DDIHelper.GetOutputDepVariables(dataColumn);
				if (outputDepVariables == null || outputDepVariables.Count == 0)
				{
					list.Add(dataColumn.ColumnName);
				}
				else
				{
					list.AddRange(outputDepVariables);
				}
			}
			return list.Distinct<string>();
		}

		internal static string JoinList<T>(IEnumerable<T> list, Func<T, string> formatter)
		{
			if (list == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (T t in list)
			{
				if (t != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(ClientStrings.ListSeparator);
					}
					stringBuilder.Append(formatter(t));
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool IsLambdaExpression(string value)
		{
			return !string.IsNullOrEmpty(value) && value.Contains("=>");
		}

		public static string GetLogonName(string dn, string samAccountName)
		{
			ADObjectId adobjectId = new ADObjectId(dn, Guid.Empty);
			return adobjectId.DomainId.ToString() + "\\" + samAccountName;
		}

		public static ExTimeZone GetUserTimeZone()
		{
			return EacRbacPrincipal.Instance.UserTimeZone ?? ExTimeZone.UtcTimeZone;
		}

		public static bool IsEmptyValue(object propertyValue)
		{
			bool result = false;
			if (propertyValue == null)
			{
				result = true;
			}
			else if (DBNull.Value.Equals(propertyValue))
			{
				result = true;
			}
			else if (propertyValue is IEnumerable && DDIHelper.IsEmptyCollection(propertyValue as IEnumerable))
			{
				result = true;
			}
			else if (propertyValue is Guid && Guid.Empty.Equals(propertyValue))
			{
				result = true;
			}
			else if (string.IsNullOrEmpty(propertyValue.ToString()))
			{
				result = true;
			}
			return result;
		}

		internal static bool IsLegacyObject(IVersionable versionableObjct)
		{
			return versionableObjct.ExchangeVersion.ExchangeBuild.Major < versionableObjct.MaximumSupportedExchangeObjectVersion.ExchangeBuild.Major;
		}

		public static bool IsUnlimited(object value)
		{
			if (DDIHelper.IsEmptyValue(value))
			{
				return false;
			}
			Type type = value.GetType();
			return (bool)type.GetProperty("IsUnlimited").GetValue(value, null);
		}

		public static string AsteriskAround(string inString)
		{
			if (!string.IsNullOrEmpty(inString))
			{
				return "*" + inString + "*";
			}
			return "*";
		}

		public static bool IsFFO()
		{
			bool result;
			try
			{
				result = EacEnvironment.Instance.IsForefrontForOffice;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private static bool IsEmptyCollection(IEnumerable enumerable)
		{
			bool result = true;
			if (enumerable != null)
			{
				using (IEnumerator enumerator = enumerable.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						result = false;
					}
				}
			}
			return result;
		}

		internal static void Trace(TraceType traceType, string msg)
		{
			switch (traceType)
			{
			case TraceType.DebugTrace:
				ExTraceGlobals.DDITracer.TraceDebug(0, 0L, msg);
				break;
			case TraceType.WarningTrace:
				ExTraceGlobals.DDITracer.TraceWarning(0, 0L, msg);
				break;
			case TraceType.ErrorTrace:
				ExTraceGlobals.DDITracer.TraceError(0, 0L, msg);
				break;
			case TraceType.InfoTrace:
				ExTraceGlobals.DDITracer.TraceInformation(0, 0L, msg);
				break;
			case TraceType.PerformanceTrace:
				ExTraceGlobals.DDITracer.TracePerformance(0, 0L, msg);
				break;
			case TraceType.FunctionTrace:
				ExTraceGlobals.DDITracer.TraceFunction(0, 0L, msg);
				break;
			case TraceType.PfdTrace:
				ExTraceGlobals.DDITracer.TracePfd(0, 0L, msg);
				break;
			}
			if (DDIHelper.IsWebTraceEnabled() && HttpContext.Current.ApplicationInstance != null)
			{
				HttpContext.Current.Trace.Write(msg);
			}
		}

		internal static void Trace(string msg)
		{
			DDIHelper.Trace(TraceType.DebugTrace, msg);
		}

		internal static void Trace<T>(TraceType traceType, T obj)
		{
			if (obj != null && DDIHelper.HasTraceEnabled(traceType))
			{
				DDIHelper.Trace(traceType, EcpTraceHelper.GetTraceString(obj));
			}
		}

		internal static void Trace<T>(T obj)
		{
			DDIHelper.Trace<T>(TraceType.DebugTrace, obj);
		}

		internal static void Trace(TraceType traceType, string formatString, params object[] args)
		{
			if (formatString != null && DDIHelper.HasTraceEnabled(traceType))
			{
				List<string> list = new List<string>();
				if (args != null)
				{
					foreach (object obj in args)
					{
						list.Add((obj == null) ? string.Empty : EcpTraceHelper.GetTraceString(obj));
					}
				}
				DDIHelper.Trace(traceType, string.Format(formatString, list.ToArray()));
			}
		}

		internal static void Trace(string formatString, params object[] args)
		{
			DDIHelper.Trace(TraceType.DebugTrace, formatString, args);
		}

		internal static string ToQuotationEscapedString(object item)
		{
			string result = string.Empty;
			if (item != null)
			{
				result = item.ToString().Replace("'", "''");
			}
			return result;
		}

		private static bool HasTraceEnabled(TraceType traceType)
		{
			return ExTraceGlobals.DDITracer.IsTraceEnabled(traceType) || DDIHelper.IsWebTraceEnabled();
		}

		private static bool IsWebTraceEnabled()
		{
			return HttpContext.Current != null && HttpContext.Current.Trace.IsEnabled;
		}

		internal static void CheckSchemaName(string schemalName)
		{
			if (DDIHelper.schemaNamePattern == null)
			{
				DDIHelper.schemaNamePattern = new Regex("^[0-9a-z]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}
			if (!DDIHelper.schemaNamePattern.IsMatch(schemalName))
			{
				throw new BadRequestException(new Exception("Invalid schema name: " + schemalName));
			}
		}

		internal static bool ForGetListProgress
		{
			get
			{
				return !string.IsNullOrEmpty(DDIHelper.ProgressIdForGetListAsync);
			}
		}

		internal static string ProgressIdForGetListAsync
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext == null)
				{
					return null;
				}
				return httpContext.Items["ProgressId"] as string;
			}
			set
			{
				HttpContext.Current.Items["ProgressId"] = value;
			}
		}

		public static bool IsGetListAsync
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				return httpContext != null && httpContext.Request.QueryString["getlistasync"] == "1";
			}
		}

		public static bool IsGetListPreLoad
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				return httpContext != null && object.Equals(httpContext.Items["getlistasync"], "1");
			}
		}

		public static Hashtable ConvertToAddHashTable(object identities)
		{
			return DDIHelper.ConvertToHashTable(identities, "add");
		}

		public static Hashtable ConvertToRemoveHashTable(object identities)
		{
			return DDIHelper.ConvertToHashTable(identities, "remove");
		}

		private static Hashtable ConvertToHashTable(object identities, string addOrRemoveforKeyName)
		{
			Hashtable result = new Hashtable();
			if (identities == null || identities == DBNull.Value)
			{
				return result;
			}
			object[] array = identities as object[];
			if (array == null || array.Length == 0)
			{
				DDIHelper.ThrowIllegalInput();
			}
			if (string.IsNullOrWhiteSpace(addOrRemoveforKeyName) || (!addOrRemoveforKeyName.Equals("add", StringComparison.OrdinalIgnoreCase) && !addOrRemoveforKeyName.Equals("remove", StringComparison.OrdinalIgnoreCase)))
			{
				DDIHelper.ThrowIllegalHashtableKeyType(addOrRemoveforKeyName);
			}
			Hashtable hashtable = new Hashtable();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Identity identity = array[i] as Identity;
				if (identity == null)
				{
					DDIHelper.ThrowIllegalInput();
				}
				array2[i] = identity.RawIdentity;
			}
			hashtable.Add(addOrRemoveforKeyName, array2);
			return hashtable;
		}

		private static void ThrowIllegalInput()
		{
			throw new ArgumentException("Expected object array of Microsoft.Exchange.Management.ControlPanel.Identity to be converted into Hashtable for add or remove bulk operation");
		}

		private static void ThrowIllegalHashtableKeyType(string illegalHashtableKeynameUsed)
		{
			if (string.IsNullOrEmpty(illegalHashtableKeynameUsed))
			{
				illegalHashtableKeynameUsed = "<null or empty>";
			}
			string message = string.Format("Expected 'add' or 'remove' for keyname to hashtable. Provided keyname: {0}", illegalHashtableKeynameUsed);
			throw new ArgumentException(message);
		}

		internal const string ProgressIdHttpContextKey = "ProgressId";

		internal const string GetListAsyncUrlParameterKey = "getlistasync";

		public static readonly int GetListDefaultResultSize;

		public static readonly int GetListAsyncModeResultSize;

		private static readonly Dictionary<Type, Type> strongType2DataContractMapping = new Dictionary<Type, Type>
		{
			{
				typeof(ProxyAddressTemplateCollection),
				typeof(AddressTemplateList)
			},
			{
				typeof(ProxyAddressCollection),
				typeof(EmailAddressList)
			},
			{
				typeof(DatabaseAvailabilityGroupNetworkSubnet),
				typeof(DAGNetworkSubnetItem)
			},
			{
				typeof(DatabaseAvailabilityGroupNetworkInterface),
				typeof(DAGNetworkInterfaceItem)
			},
			{
				typeof(DagNetworkObjectId),
				typeof(Identity)
			},
			{
				typeof(ADObjectId),
				typeof(Identity)
			},
			{
				typeof(MailboxId),
				typeof(MailboxIdentity)
			},
			{
				typeof(AppId),
				typeof(Identity)
			},
			{
				typeof(MigrationBatchId),
				typeof(Identity)
			},
			{
				typeof(MigrationUserId),
				typeof(Identity)
			},
			{
				typeof(MigrationEndpointId),
				typeof(Identity)
			},
			{
				typeof(MigrationReportId),
				typeof(Identity)
			},
			{
				typeof(MigrationStatisticsId),
				typeof(Identity)
			},
			{
				typeof(ServerVersion),
				typeof(EcpServerVersion)
			}
		};

		private static readonly List<Type> StrongTypeToStringList = new List<Type>
		{
			typeof(ServerVersion),
			typeof(SmtpAddress)
		};

		private static readonly Hashtable dataContractHashtable = Hashtable.Synchronized(new Hashtable());

		private static readonly Hashtable collectionDataContractHashtable = Hashtable.Synchronized(new Hashtable());

		private static readonly Hashtable serializableHashtable = Hashtable.Synchronized(new Hashtable());

		private static Regex schemaNamePattern;
	}
}
