using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes
{
	public class RwsSyntheticTenantMailboxUsageDetailProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (DateTime.UtcNow < new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 50, 0))
			{
				base.Result.StateAttribute11 = string.Format("Probe Start from Utc {0} (< 23:50:00 of that day), bypass data check", DateTime.UtcNow);
				return;
			}
			string account = base.Definition.Account;
			string arg = RwsCryptographyHelper.Decrypt(base.Definition.AccountPassword);
			string azureConnString = string.Format(base.Definition.Endpoint, account, arg);
			List<RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant> list = null;
			try
			{
				list = this.GetSyntheticTenantFromAzure(azureConnString);
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute13 = ex.ToString();
			}
			if (list == null || list.Count <= 0)
			{
				base.Result.StateAttribute12 = "No available synthetic tenant to verify, bypass the check ...";
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant> list2 = new List<RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant>();
			foreach (RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant syntheticTenant in list)
			{
				string empty = string.Empty;
				if (!this.VerifySyntheticTenantMailboxUsageDetail(syntheticTenant, out empty))
				{
					stringBuilder.Append(empty);
					stringBuilder.Append("\r\n");
					list2.Add(syntheticTenant);
				}
			}
			if (list2.Count <= 0)
			{
				base.Result.StateAttribute11 = "Synthetic Tenant data check complete on MailboxUsageDetail table. All data is good.";
				return;
			}
			base.Result.StateAttribute11 = "Synthetic Tenant data check complete on MailboxUsageDetail table. Some data is wrong here";
			throw new Exception(stringBuilder.ToString());
		}

		private bool VerifySyntheticTenantMailboxUsageDetail(RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant tenant, out string errMsg)
		{
			errMsg = string.Empty;
			string connectionString = "Data Source=localhost;Initial Catalog=CDM-TENANTDS;User Id=sa;Password=Weareall1n;";
			List<SqlParameter> list = new List<SqlParameter>();
			string text = string.Empty;
			if (base.Definition.TargetGroup == "E2E")
			{
				text = string.Format("select count(*) AS mailbox_number from (select * from [dbo].[UDF_MailboxUsageDetail](1000, 'datetime = ''{0}'' and tenantguid = ''{1}'' ORDER BY datetime DESC')) T;", (DateTime.UtcNow - new TimeSpan(24, 0, 0)).ToShortDateString(), tenant.TenantGuid);
			}
			else if (base.Definition.TargetGroup == "Design")
			{
				text = "\r\n                        select count(*) AS mailbox_number from\r\n                        (\r\n                        select * from [CDM-TENANTDS-PUMPER01.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_1] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER02.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_2] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER03.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_3] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER04.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_4] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER05.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_5] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER06.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_6] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER07.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_7] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER08.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_8] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER09.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_9] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER10.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_10] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER11.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_11] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        union all\r\n                        select * from [CDM-TENANTDS-PUMPER12.exmgmt.local].[cdm-tenantds].[dbo].[MailboxUsageDetail_12] where [TenantGuid] = @TenantGuid AND [DateTime] = @DateTime\r\n                        ) T\r\n                         ";
				SqlParameter item = new SqlParameter("@DateTime", (DateTime.UtcNow - new TimeSpan(24, 0, 0)).ToShortDateString());
				SqlParameter item2 = new SqlParameter("@TenantGuid", tenant.TenantGuid);
				list.Add(item);
				list.Add(item2);
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
				{
					sqlCommand.CommandText = text;
					sqlCommand.CommandType = CommandType.Text;
					base.Result.StateAttribute3 = text;
					if (list != null && list.Count > 0)
					{
						foreach (SqlParameter value in list)
						{
							sqlCommand.Parameters.Add(value);
						}
					}
					sqlConnection.Open();
					using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
					{
						if (sqlDataReader.HasRows)
						{
							int num = 0;
							if (sqlDataReader.Read())
							{
								num = sqlDataReader.GetInt32(0);
							}
							int num2 = 0;
							Dictionary<string, string> dictionary = null;
							using (XmlReader xmlReader = XmlReader.Create(new StringReader(tenant.MeasurementPropertyBag)))
							{
								xmlReader.Read();
								dictionary = new Dictionary<string, string>(xmlReader.AttributeCount);
								for (int i = 0; i < xmlReader.AttributeCount; i++)
								{
									xmlReader.MoveToAttribute(i);
									dictionary.Add(xmlReader.Name, xmlReader.Value);
								}
							}
							if (dictionary != null && dictionary.ContainsKey("MailboxNumber"))
							{
								int.TryParse(dictionary["MailboxNumber"], out num2);
							}
							if (num2 == 0)
							{
								ProbeResult result = base.Result;
								result.StateAttribute12 += string.Format("Mailbox number for tenant (tenant guid = {0}) is 0, skip the validation", tenant.TenantGuid);
								return true;
							}
							int num3 = 1;
							if (!int.TryParse(base.Definition.Attributes["DuplicateCopyNumber"], out num3))
							{
								num3 = 1;
							}
							base.Result.StateAttribute16 = (double)num;
							base.Result.StateAttribute17 = (double)num2;
							if (num != num2 * num3)
							{
								errMsg = string.Format("Validation ({0}) for synthetic tenant (tenant guid = {1}) failed in MailboxUsageDetail table, row count = {2}, mailbox number = {3}", new object[]
								{
									base.Definition.TargetGroup,
									tenant.TenantGuid,
									num,
									num2
								});
								return false;
							}
							return true;
						}
					}
				}
			}
			errMsg = string.Format("Synthetic tenant [Guid = {0}] have no data available in MailboxUsageDetail table", tenant.TenantGuid);
			return false;
		}

		private List<RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant> GetSyntheticTenantFromAzure(string azureConnString)
		{
			List<RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant> list = new List<RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant>();
			string commandText = "\r\n                            SELECT [TenantGuid]\r\n                                  ,[TenantName]\r\n                                  ,[Category]\r\n                                  ,[TimeRangeInDay]\r\n                                  ,[MeasurementPropertyBag]\r\n                              FROM [Database_CFRSyntheticTenantInfo]\r\n                              where Category = 'MailboxUsageDetail';\r\n                            ";
			using (SqlConnection sqlConnection = new SqlConnection(azureConnString))
			{
				SqlCommand sqlCommand = sqlConnection.CreateCommand();
				sqlCommand.CommandText = commandText;
				sqlCommand.CommandType = CommandType.Text;
				sqlConnection.Open();
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				if (sqlDataReader.HasRows)
				{
					while (sqlDataReader.Read())
					{
						list.Add(new RwsSyntheticTenantMailboxUsageDetailProbe.SyntheticTenant
						{
							TenantGuid = (sqlDataReader.IsDBNull(0) ? string.Empty : sqlDataReader.GetString(0)),
							TenantName = (sqlDataReader.IsDBNull(1) ? string.Empty : sqlDataReader.GetString(1)),
							Category = (sqlDataReader.IsDBNull(2) ? string.Empty : sqlDataReader.GetString(2)),
							TimeRangeInDay = (sqlDataReader.IsDBNull(3) ? 0 : sqlDataReader.GetInt32(3)),
							MeasurementPropertyBag = (sqlDataReader.IsDBNull(4) ? string.Empty : sqlDataReader.GetString(4))
						});
					}
				}
			}
			return list;
		}

		internal struct SyntheticTenant
		{
			internal string TenantGuid;

			internal string TenantName;

			internal string Category;

			internal int TimeRangeInDay;

			internal string MeasurementPropertyBag;
		}
	}
}
