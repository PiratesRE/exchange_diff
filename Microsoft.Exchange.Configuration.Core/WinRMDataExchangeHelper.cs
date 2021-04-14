using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class WinRMDataExchangeHelper
	{
		internal static string Serialize(Dictionary<string, string> items)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, items);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
			}
			return @string;
		}

		internal static Dictionary<string, string> DeSerialize(string serializedString)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(serializedString);
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
			Dictionary<string, string> result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				result = (dataContractJsonSerializer.ReadObject(memoryStream) as Dictionary<string, string>);
			}
			return result;
		}

		internal static string HydrateAuthenticationType(string originalAuthType, string serializedData)
		{
			return string.Format("{0};{1}", originalAuthType, serializedData);
		}

		internal static void DehydrateAuthenticationType(string authenticationType, out string realAuthenticationType, out string serializedData)
		{
			int num = authenticationType.IndexOf(';');
			realAuthenticationType = authenticationType.Substring(0, num);
			serializedData = authenticationType.Substring(num + 1, authenticationType.Length - num - 1);
		}

		internal static bool IsExchangeDataUseAuthenticationType()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.WinRMExchangeDataUseAuthenticationType.Enabled;
		}

		internal static bool IsExchangeDataUseNamedPipe()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.WinRMExchangeDataUseTypeNamedPipe.Enabled;
		}

		internal static string GetWinRMDataIdentity(string connectionUrl, string userName, string authenticationType)
		{
			string arg = string.Empty;
			if (connectionUrl != null)
			{
				try
				{
					arg = HttpUtility.UrlEncode(new Uri(connectionUrl).Query);
				}
				catch (Exception ex)
				{
					CoreLogger.TraceInformation("[WinRMDataExchangeHelper.GetWinRMDataIdentity]Error on parse connectionUrl {0}: {1}", new object[]
					{
						connectionUrl,
						ex.ToString()
					});
				}
			}
			return string.Format("<{0}><{1}><{2}>", arg, userName, authenticationType);
		}

		private static readonly ExEventLog RbacEventLogger = new ExEventLog(ExTraceGlobals.LogTracer.Category, "MSExchange RBAC");
	}
}
