using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes
{
	public class RwsDatamartConnectionProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			string text = string.Empty;
			try
			{
				text = this.GetConnectionString();
				using (SqlConnection sqlConnection = new SqlConnection(text))
				{
					sqlConnection.Open();
					base.Result.StateAttribute21 = text;
					base.Result.StateAttribute22 = "Connected Successfully.";
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("Get exception when try to open connection to datamart. Exception: {0}. The connection string is {1}.", ex.Message, text), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionProbe.cs", 80);
				base.Result.StateAttribute21 = text;
				throw;
			}
		}

		private static TValue LoadSettingFromRegistry<TValue>(string key)
		{
			TValue result = default(TValue);
			string text = string.Format(CultureInfo.InvariantCulture, "SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Cmdlet\\Reporting", new object[]
			{
				"v15"
			});
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text))
			{
				object obj = null;
				if (registryKey != null)
				{
					obj = registryKey.GetValue(key);
				}
				if (registryKey == null || obj == null)
				{
					throw new Exception(string.Format("Cannot find the tenant datamart server registry key, {0}", text));
				}
				result = (TValue)((object)obj);
			}
			return result;
		}

		private string GetConnectionString()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			int num = 0;
			string result;
			try
			{
				text = RwsDatamartConnectionProbe.LoadSettingFromRegistry<string>("DataMartTenantsServer");
				if (!ExEnvironment.IsTest)
				{
					string text3 = Environment.MachineName.Substring(0, 3).ToUpper();
					if (!text.ToUpper().StartsWith("CDM-TENANTDS."))
					{
						throw new ArgumentException("Invalid value of Registry Key DataMartTenantsServer. Value = [" + text + "]");
					}
					text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.exmgmt.local", new object[]
					{
						"CDM-TENANTDS",
						text3
					});
				}
				text2 = RwsDatamartConnectionProbe.LoadSettingFromRegistry<string>("DataMartTenantsDatabase");
				num = RwsDatamartConnectionProbe.LoadSettingFromRegistry<int>("SQLConnectionTimeout");
				string text4 = string.Format(CultureInfo.InvariantCulture, "Server={0};Database={1};Integrated Security=SSPI;Connection Timeout={2}", new object[]
				{
					text,
					text2,
					num
				});
				result = text4;
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("Failed to get the connection string, exception: {0}, TenantsDatamartServerKey : {1}, TenantsDatamartDatabaseKey: {2},TenantsDatamartConnectionTimeoutKey: {3}. ", new object[]
				{
					ex.Message,
					text,
					text2,
					num
				}), null, "GetConnectionString", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionProbe.cs", 167);
				throw;
			}
			return result;
		}

		private const string ReportingCmdletKeyRootFormat = "SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Cmdlet\\Reporting";

		private const string ConnectionStringFormat = "Server={0};Database={1};Integrated Security=SSPI;Connection Timeout={2}";

		private const string TenantsDatamartServerKey = "DataMartTenantsServer";

		private const string TenantsDatamartDatabaseKey = "DataMartTenantsDatabase";

		private const string TenantsDatamartConnectionTimeoutKey = "SQLConnectionTimeout";
	}
}
