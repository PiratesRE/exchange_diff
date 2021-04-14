using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes
{
	public class RwsCNAMEMappingProbe : ProbeWorkItem
	{
		private void DnsMappingSaveToAzure(string azureConnString, List<string> addressArrayToResolve)
		{
			if (string.IsNullOrEmpty(azureConnString))
			{
				throw new ArgumentException("One or more parameter is null or empty", "azureConnString");
			}
			if (addressArrayToResolve == null || addressArrayToResolve.Count <= 0)
			{
				throw new ArgumentException("One or more parameter is null or empty", "addressArrayToResolve");
			}
			foreach (string text in addressArrayToResolve)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("Resolving address {0}. ", text), null, "DnsMappingSaveToAzure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsCNAMEMappingProbe.cs", 48);
				this.ResolveAddressAndSaveToAzure(azureConnString, text);
			}
		}

		private void ResolveAddressAndSaveToAzure(string azureConnString, string addressToResolve)
		{
			if (string.IsNullOrEmpty(azureConnString))
			{
				throw new ArgumentException("One or more parameter is null or empty", "azureConnString");
			}
			if (string.IsNullOrEmpty(addressToResolve))
			{
				throw new ArgumentException("One or more parameter is null or empty", "addressToResolve");
			}
			IPHostEntry iphostEntry = null;
			try
			{
				iphostEntry = Dns.GetHostEntry(addressToResolve);
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute11 = ex.ToString();
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("Get exception when call GetHostEntry <{0}>. Exception: {1}. ", addressToResolve, ex.ToString()), null, "ResolveAddressAndSaveToAzure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsCNAMEMappingProbe.cs", 87);
			}
			string value = (iphostEntry == null) ? "ResolveFailed" : iphostEntry.HostName;
			string machineName = Environment.MachineName;
			string value2 = string.Empty;
			if (iphostEntry != null && iphostEntry.AddressList != null && iphostEntry.AddressList.Length > 0)
			{
				value2 = iphostEntry.AddressList[0].ToString();
			}
			DateTime utcNow = DateTime.UtcNow;
			string queryString = "\r\n                        IF EXISTS (select * from [Database_CFRCNAMEMapping] where [CNAME] = @CNAME and [Uploader] = @Uploader and [ExtensionAttribute1] = @ExtensionAttribute1)\r\n                        BEGIN\r\n                            update [Database_CFRCNAMEMapping] \r\n\t                        set [ResolveToHost] = @ResolveToHost,\r\n\t\t                        [ResolveToIp] = @ResolveToIp,\r\n\t\t                        [UpdateTime] = @UpdateTime\r\n\t                        where [CNAME] = @CNAME and [Uploader] = @Uploader and [ExtensionAttribute1] = @ExtensionAttribute1;\r\n                        END\r\n                        ELSE\r\n                        BEGIN\r\n                            INSERT INTO [Database_CFRCNAMEMapping]\r\n                                   ([CNAME]\r\n                                   ,[ResolveToHost]\r\n                                   ,[ResolveToIp]\r\n                                   ,[Uploader]\r\n                                   ,[UpdateTime]\r\n                                   ,[ExtensionAttribute1])\r\n                             VALUES\r\n                                   (@CNAME\r\n                                   ,@ResolveToHost\r\n                                   ,@ResolveToIp\r\n                                   ,@Uploader\r\n                                   ,@UpdateTime\r\n                                   ,@ExtensionAttribute1)\r\n                        END\r\n                            ";
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@CNAME", addressToResolve);
			SqlParameter item2 = new SqlParameter("@ResolveToHost", value);
			SqlParameter item3 = new SqlParameter("@ResolveToIp", value2);
			SqlParameter item4 = new SqlParameter("@Uploader", machineName);
			SqlParameter item5 = new SqlParameter("@UpdateTime", utcNow);
			SqlParameter item6 = new SqlParameter("@ExtensionAttribute1", utcNow.ToShortDateString().ToString());
			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
			list.Add(item5);
			list.Add(item6);
			this.SqlExecute(azureConnString, queryString, list);
		}

		private int SqlExecute(string connString, string queryString, IList<SqlParameter> paramArray)
		{
			if (string.IsNullOrEmpty(connString))
			{
				throw new ArgumentException("Connection String could not be null or empty", "connString");
			}
			if (string.IsNullOrEmpty(queryString))
			{
				throw new ArgumentException("Query String could not be null or empty", "queryString");
			}
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(connString))
			{
				using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
				{
					if (paramArray != null)
					{
						foreach (SqlParameter value in paramArray)
						{
							sqlCommand.Parameters.Add(value);
						}
					}
					sqlConnection.Open();
					result = sqlCommand.ExecuteNonQuery();
				}
			}
			return result;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			string account = base.Definition.Account;
			string arg = RwsCryptographyHelper.Decrypt(base.Definition.AccountPassword);
			string azureConnString = string.Format(base.Definition.Endpoint, account, arg);
			string text = base.Definition.Attributes["TargetDCList"];
			string[] array = text.Split(new char[]
			{
				';'
			});
			List<string> list = new List<string>();
			foreach (string text2 in array)
			{
				string arg2 = text2.Trim();
				list.Add(string.Format("CDM-TENANTDS.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-SCALED.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER01.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER02.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER03.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER04.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER05.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER06.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER07.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER08.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER09.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER10.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER11.{0}.exmgmt.local", arg2));
				list.Add(string.Format("CDM-TENANTDS-PUMPER12.{0}.exmgmt.local", arg2));
			}
			this.DnsMappingSaveToAzure(azureConnString, list);
		}
	}
}
