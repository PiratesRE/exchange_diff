using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ProxyHelper
	{
		internal static IEnumerable<PSObject> RPSProxyExecution(Guid cmdletUniqueId, PSCommand command, string serverFqn, ExchangeRunspaceConfiguration runspaceConfig, int serverVersion, bool asyncProxying, Task.TaskWarningLoggingDelegate writeWarning)
		{
			Uri uri = ProxyHelper.BuildCmdletProxyUri(serverFqn, runspaceConfig, serverVersion);
			IEnumerable<PSObject> result;
			try
			{
				RemoteConnectionInfo connectionInfo = ProxyHelper.BuildProxyWSManConnectionInfo(uri);
				ProxyPSCommand proxyPSCommand = new ProxyPSCommand(connectionInfo, command, asyncProxying, writeWarning);
				result = proxyPSCommand.Invoke();
			}
			catch (Exception)
			{
				CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "TargetUri", uri.ToString());
				throw;
			}
			return result;
		}

		internal static string GetCommandString(PSCommand command)
		{
			if (command.Commands.Count <= 0)
			{
				throw new ArgumentOutOfRangeException("command", "[ProxyHelper.GetCommandString] PSCommand should only have one command.");
			}
			StringBuilder stringBuilder = new StringBuilder(command.Commands[0].CommandText);
			foreach (CommandParameter commandParameter in command.Commands[0].Parameters)
			{
				ProxyHelper.BuildCommandFromParameter(commandParameter.Name, commandParameter.Value, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		private static bool IsSingleQuote(char c)
		{
			return c == '\'' || c == '‘' || c == '’' || c == '‚' || c == '‛';
		}

		private static string EscapeSingleQuotedString(string stringContent)
		{
			if (string.IsNullOrEmpty(stringContent))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(stringContent.Length);
			foreach (char c in stringContent)
			{
				stringBuilder.Append(c);
				if (ProxyHelper.IsSingleQuote(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		internal static void BuildCommandFromParameter(string parameterName, object parameterValue, StringBuilder commandBuilder)
		{
			if (parameterValue == null)
			{
				commandBuilder.AppendFormat(" -{0}:{1}", parameterName, ProxyHelper.nullString);
				return;
			}
			Type type = parameterValue.GetType();
			if (type == typeof(bool))
			{
				commandBuilder.AppendFormat(" -{0}:${1}", parameterName, parameterValue);
				return;
			}
			if (type == typeof(SwitchParameter))
			{
				commandBuilder.AppendFormat(" -{0}:${1}", parameterName, ((SwitchParameter)parameterValue).IsPresent);
				return;
			}
			string stringContent;
			if (parameterValue is IDictionary)
			{
				stringContent = ProxyHelper.TranslateCmdletProxyDictionaryParameter(parameterValue, ExchangeRunspaceConfigurationSettings.ProxyMethod.PSWS).ToString();
			}
			else
			{
				if (parameterValue is ICollection)
				{
					string arg = ProxyHelper.TranslateCmdletProxyCollectionParameter(parameterValue, ExchangeRunspaceConfigurationSettings.ProxyMethod.PSWS).ToString();
					commandBuilder.AppendFormat(" -{0}:{1}", parameterName, arg);
					return;
				}
				stringContent = parameterValue.ToString();
			}
			commandBuilder.AppendFormat(" -{0}:'{1}'", parameterName, ProxyHelper.EscapeSingleQuotedString(stringContent));
		}

		internal static Uri BuildCmdletProxyUri(string targetFqdn, ExchangeRunspaceConfiguration runspaceConfig, int targetVersion)
		{
			if (string.IsNullOrWhiteSpace(targetFqdn))
			{
				throw new ArgumentNullException("targetFqdn");
			}
			if (runspaceConfig == null)
			{
				throw new ArgumentNullException("runspaceConfig");
			}
			ExchangeRunspaceConfigurationSettings configurationSettings = runspaceConfig.ConfigurationSettings;
			ExAssert.RetailAssert(configurationSettings != null, "runspaceConfig.ConfigurationSettings should not be null.");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("https://");
			stringBuilder.Append(targetFqdn);
			if (targetVersion >= Server.E15MinVersion)
			{
				stringBuilder.Append(":444/powershell/Powershell-proxy?");
			}
			else
			{
				stringBuilder.Append("/Powershell-proxy?");
			}
			stringBuilder.AppendFormat("{0}={1}", "X-Rps-CAT", Uri.EscapeDataString(configurationSettings.UserToken.CommonAccessTokenForCmdletProxy().Serialize()));
			stringBuilder.AppendFormat(";{0}={1}", "serializationLevel", configurationSettings.CurrentSerializationLevel.ToString());
			stringBuilder.AppendFormat(";{0}={1}", "clientApplication", configurationSettings.ClientApplication.ToString());
			if (configurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP || configurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.EMC || configurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.OSP)
			{
				stringBuilder.AppendFormat(";{0}={1}", "proxyFullSerialization", "true");
			}
			string managedOrganization = ProxyHelper.GetManagedOrganization(runspaceConfig);
			if (!string.IsNullOrEmpty(managedOrganization))
			{
				stringBuilder.AppendFormat(";{0}", ProxyHelper.GetOrganizationAppendQueryIfNeeded(managedOrganization));
			}
			stringBuilder.AppendFormat(";{0}={1}", WellKnownHeader.CmdletProxyIsOn, "99C6ECEE-5A4F-47B9-AE69-49EAFB58F368");
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null && currentActivityScope.ActivityId != Guid.Empty)
			{
				stringBuilder.AppendFormat(";{0}={1}", "RequestId48CD6591-0506-4D6E-9131-797489A3260F", currentActivityScope.ActivityId);
			}
			return new Uri(stringBuilder.ToString());
		}

		internal static string GetPSWSProxySiteUri(string remoteServerFqdn)
		{
			return string.Format("https://{0}:444/psws", remoteServerFqdn);
		}

		internal static NameValueCollection GetPSWSProxyRequestHeaders(ExchangeRunspaceConfiguration runspaceConfig)
		{
			ExchangeRunspaceConfigurationSettings configurationSettings = runspaceConfig.ConfigurationSettings;
			ExAssert.RetailAssert(configurationSettings != null, "runspaceConfig.ConfigurationSettings should not be null.");
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["X-CommonAccessToken"] = configurationSettings.UserToken.CommonAccessTokenForCmdletProxy().Serialize();
			nameValueCollection["serializationLevel"] = configurationSettings.CurrentSerializationLevel.ToString();
			nameValueCollection["clientApplication"] = configurationSettings.ClientApplication.ToString();
			nameValueCollection["X-EncodeDecode-Key"] = "false";
			nameValueCollection[WellKnownHeader.CmdletProxyIsOn] = "99C6ECEE-5A4F-47B9-AE69-49EAFB58F368";
			if (configurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP || configurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.EMC || configurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.OSP)
			{
				nameValueCollection["proxyFullSerialization"] = "true";
			}
			string managedOrganization = ProxyHelper.GetManagedOrganization(runspaceConfig);
			if (!string.IsNullOrEmpty(managedOrganization))
			{
				nameValueCollection["organization"] = managedOrganization;
			}
			return nameValueCollection;
		}

		internal static string GetOrganizationAppendQueryIfNeeded(string tenantOrganization)
		{
			if (string.IsNullOrEmpty(tenantOrganization))
			{
				return string.Empty;
			}
			return string.Format("{0}={1}", "organization", tenantOrganization);
		}

		internal static object ConvertPSObjectToOriginalType(PSObject psObject, int remoteServerVersion, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (psObject == null)
			{
				throw new ArgumentNullException("psObject");
			}
			Type type = MonadCommand.ResolveType(psObject);
			if (remoteServerVersion >= Server.E15MinVersion)
			{
				if (psObject.Members["SerializationData"] == null || psObject.Members["SerializationData"].Value == null)
				{
					if (writeVerbose != null)
					{
						writeVerbose(Strings.VerboseSerializationDataNotExist);
					}
				}
				else
				{
					try
					{
						return ProxyHelper.TypeConvertor.ConvertFrom(psObject, type, null, true);
					}
					catch (Exception ex)
					{
						if (writeVerbose != null)
						{
							writeVerbose(Strings.VerboseFailedToDeserializePSObject(ex.Message));
						}
					}
				}
			}
			return MockObjectInformation.TranslateToMockObject(type, psObject);
		}

		internal static RemoteConnectionInfo BuildProxyWSManConnectionInfo(Uri connectionUri)
		{
			if (connectionUri == null)
			{
				throw new ArgumentNullException("connectionUri");
			}
			PSCredential pscredential = null;
			AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.NegotiateWithImplicitCredential;
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ProxyHelper.FaultInjection_ProxySessionCredentialAndType(ref pscredential, ref authenticationMechanism);
			}
			return new RemoteConnectionInfo(connectionUri, pscredential, ProxyHelper.ExchangeShellSchema, null, authenticationMechanism, true, 0);
		}

		public static object TranslateCmdletProxyParameter(object paramValue, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod)
		{
			if (paramValue == null)
			{
				return null;
			}
			Type type = paramValue.GetType();
			if (type.IsPrimitive)
			{
				return paramValue;
			}
			if (paramValue is IDictionary)
			{
				return ProxyHelper.TranslateCmdletProxyDictionaryParameter(paramValue, proxyMethod);
			}
			if (paramValue is ICollection)
			{
				return ProxyHelper.TranslateCmdletProxyCollectionParameter(paramValue, proxyMethod);
			}
			if (type == typeof(IIdentityParameter))
			{
				return paramValue.ToString();
			}
			if (type == typeof(SwitchParameter))
			{
				return ((SwitchParameter)paramValue).IsPresent;
			}
			return paramValue.ToString();
		}

		public static string GetFriendlyVersionInformation(int version)
		{
			ServerVersion serverVersion = new ServerVersion(version);
			return string.Format("{0}.{1:D2}.{2:D4}.{3:D4}", new object[]
			{
				serverVersion.Major,
				serverVersion.Minor,
				serverVersion.Build,
				serverVersion.Revision
			});
		}

		private static object TranslateCmdletProxyCollectionParameter(object paramValue, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod)
		{
			ICollection collection = (ICollection)paramValue;
			if (proxyMethod == ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS)
			{
				Collection<object> collection2 = new Collection<object>();
				foreach (object paramValue2 in collection)
				{
					collection2.Add(ProxyHelper.TranslateCmdletProxyParameter(paramValue2, proxyMethod));
				}
				return collection2;
			}
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object paramValue3 in collection)
			{
				num++;
				object obj = ProxyHelper.TranslateCmdletProxyParameter(paramValue3, proxyMethod);
				string arg;
				if (obj == null)
				{
					arg = ProxyHelper.nullString;
				}
				else if (obj.GetType() == typeof(bool))
				{
					arg = (((bool)obj) ? ProxyHelper.trueString : ProxyHelper.falseString);
				}
				else
				{
					arg = obj.ToString().Replace("'", "''");
				}
				stringBuilder.AppendFormat("'{0}',", arg);
			}
			if (num == 0)
			{
				return null;
			}
			return stringBuilder.ToString().Trim().TrimEnd(new char[]
			{
				','
			});
		}

		private static object TranslateCmdletProxyDictionaryParameter(object paramValue, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod)
		{
			IDictionary dictionary = (IDictionary)paramValue;
			if (proxyMethod == ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				foreach (object obj in dictionary.Keys)
				{
					dictionary2.Add(obj.ToString(), ProxyHelper.TranslateCmdletProxyParameter(dictionary[obj], proxyMethod));
				}
				return dictionary2;
			}
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			foreach (object obj2 in dictionary.Keys)
			{
				num++;
				stringBuilder.Append(obj2.ToString()).Append(" = ");
				object obj3 = dictionary[obj2];
				if (obj3 == null)
				{
					stringBuilder.Append(ProxyHelper.nullString);
				}
				else
				{
					object obj4 = ProxyHelper.TranslateCmdletProxyParameter(obj3, proxyMethod);
					if (obj4.GetType() == typeof(bool))
					{
						stringBuilder.Append(((bool)obj4) ? ProxyHelper.trueString : ProxyHelper.falseString);
					}
					else
					{
						stringBuilder.Append(obj4.ToString());
					}
				}
				stringBuilder.Append("; ");
			}
			if (num == 0)
			{
				return null;
			}
			string str = stringBuilder.ToString().Trim().TrimEnd(new char[]
			{
				';'
			});
			return str + "}";
		}

		private static string GetManagedOrganization(ExchangeRunspaceConfiguration runspaceConfiguration)
		{
			if (runspaceConfiguration.PartnerMode)
			{
				if (runspaceConfiguration.OrganizationId != null && runspaceConfiguration.OrganizationId != OrganizationId.ForestWideOrgId)
				{
					return runspaceConfiguration.OrganizationId.GetFriendlyName();
				}
				if (!string.IsNullOrEmpty(runspaceConfiguration.ConfigurationSettings.TenantOrganization))
				{
					return runspaceConfiguration.ConfigurationSettings.TenantOrganization;
				}
			}
			return null;
		}

		internal static void FaultInjection_UserSid(ref UserToken userToken)
		{
			string empty = string.Empty;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2982554941U, ref empty);
			if (!string.IsNullOrWhiteSpace(empty))
			{
				userToken.UpdateUserSidForTest(empty);
			}
		}

		internal static void FaultInjection_ProxySessionCredentialAndType(ref PSCredential proxySessionCredential, ref AuthenticationMechanism authType)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2915446077U, ref empty);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3989187901U, ref empty2);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2378575165U, ref empty3);
			if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty2))
			{
				SecureString secureString = new SecureString();
				char[] array = empty2.ToCharArray();
				foreach (char c in array)
				{
					secureString.AppendChar(c);
				}
				proxySessionCredential = new PSCredential(empty, secureString);
			}
			if (!string.IsNullOrEmpty(empty3))
			{
				authType = (AuthenticationMechanism)Enum.Parse(typeof(AuthenticationMechanism), empty3, true);
			}
		}

		internal static void FaultInjection_Identity(PropertyBag parameters)
		{
			if (parameters.Contains("Identity"))
			{
				string value = parameters["Identity"].ToString();
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3519425853U, ref value);
				parameters.Remove("Identity");
				parameters.Add("Identity", value);
			}
		}

		internal static void FaultInjection_ShouldForcedlyProxyCmdlet(Uri originalUrl, string remoteServerFQDN, ref bool shouldForcedlyProxyCmdlet)
		{
			if (originalUrl == null || (originalUrl.ToString().IndexOf("CommandInvocations", StringComparison.OrdinalIgnoreCase) < 0 && originalUrl.ToString().IndexOf("X-Rps-CAT", StringComparison.OrdinalIgnoreCase) < 0))
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2714119485U, ref shouldForcedlyProxyCmdlet);
			}
		}

		private const char quoteSingleLeft = '‘';

		private const char quoteSingleRight = '’';

		private const char quoteSingleBase = '‚';

		private const char quoteReversed = '‛';

		private static readonly string nullString = "$null";

		private static readonly string trueString = "$true";

		private static readonly string falseString = "$false";

		private static readonly string ExchangeShellSchema = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

		private static readonly SerializationTypeConverter TypeConvertor = new SerializationTypeConverter();

		public static readonly int PswsSupportProxyMinimumVersion = new ServerVersion(15, 0, 496, 0).ToInt();
	}
}
