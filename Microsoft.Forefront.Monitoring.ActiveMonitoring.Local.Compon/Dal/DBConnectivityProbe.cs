using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Hygiene.Data;
using Microsoft.Exchange.Hygiene.WebStoreDataProvider;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class DBConnectivityProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "select top 1 [nvc_DBVersion] from dbo.[DBVersion]";
			string text2 = base.Definition.Attributes["Text"];
			string[] array = text2.Split(new char[]
			{
				','
			});
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Assembly assembly = Assembly.LoadFrom(Path.Combine(directoryName, "microsoft.exchange.hygiene.webstoredataprovider.dll"));
			Type type = assembly.GetType("Microsoft.Exchange.Hygiene.WebStoreDataProvider.WstDataProviderSingleCopy");
			Type type2 = assembly.GetType("Microsoft.Exchange.Hygiene.WebStoreDataProvider.WstDataProviderSingleCopy+ExecutionResults");
			foreach (string text3 in array)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException();
				}
				string text4 = null;
				string text5 = null;
				try
				{
					DatabaseType db = (DatabaseType)Enum.Parse(typeof(DatabaseType), text3.Trim());
					DBConnectivityProbe.GetDBName(db, out text4, out text5);
					object results = type.InvokeMember("ExecuteQuery", BindingFlags.InvokeMethod, Type.DefaultBinder, string.Empty, new object[]
					{
						text4,
						text5,
						text
					});
					DBConnectivityProbe.CheckAndGenerateError(results, stringBuilder, type2);
				}
				catch (Exception ex)
				{
					stringBuilder.AppendFormat("==============================\r\nFailed to connect to {0}, exception = {1}\r\n", text4, ex.ToString());
				}
			}
			if (stringBuilder.Length > 0)
			{
				throw new Exception(stringBuilder.ToString());
			}
		}

		private static void CheckAndGenerateError(object results, StringBuilder errorMsg, Type queryResultType)
		{
			Array array = (Array)results;
			foreach (object obj in array)
			{
				object value = queryResultType.GetProperty("ReturnValue").GetValue(obj, null);
				object value2 = queryResultType.GetProperty("ExecutionException").GetValue(obj, null);
				object value3 = queryResultType.GetProperty("DatabaseName").GetValue(obj, null);
				object value4 = queryResultType.GetProperty("SQLServerName").GetValue(obj, null);
				if ((int)value != 0 || value2 != null)
				{
					errorMsg.AppendFormat("---------------------\r\nDatabaseName={0}, SQL ServerName={1}, ReturnValue={2}, exception={3}\r\n", new object[]
					{
						value3.ToString(),
						value4.ToString(),
						value.ToString(),
						(value2 == null) ? "null" : value2.ToString()
					});
				}
			}
		}

		private static void GetDBName(DatabaseType db, out string storeName, out string connectionString)
		{
			connectionString = null;
			storeName = null;
			switch (db)
			{
			case DatabaseType.Directory:
				storeName = new WstDataProviderDirectory().StoreName;
				connectionString = new WstDataProviderDirectory().ConnectionString;
				return;
			case DatabaseType.Spam:
				storeName = new WstDataProviderSpam().StoreName;
				connectionString = new WstDataProviderSpam().ConnectionString;
				return;
			case DatabaseType.Domain:
				storeName = new WstDataProviderDomain().StoreName;
				connectionString = new WstDataProviderDomain().ConnectionString;
				return;
			case DatabaseType.Reporting:
				storeName = new WstDataProviderMtrt().StoreName;
				connectionString = new WstDataProviderMtrt().ConnectionString;
				return;
			case DatabaseType.Mtrt:
				storeName = new WstDataProviderMtrt().StoreName;
				connectionString = new WstDataProviderMtrt().ConnectionString;
				return;
			case DatabaseType.Kes:
				storeName = new WstDataProviderKes().StoreName;
				connectionString = new WstDataProviderKes().ConnectionString;
				return;
			case DatabaseType.BackgroundJobBackend:
				storeName = new WstDataProviderBackgroundJobMgrBackend().StoreName;
				connectionString = new WstDataProviderBackgroundJobMgrBackend().ConnectionString;
				return;
			}
			throw new Exception("Invalid DB type is specified");
		}
	}
}
