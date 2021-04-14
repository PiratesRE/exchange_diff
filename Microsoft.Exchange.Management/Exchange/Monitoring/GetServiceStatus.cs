using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Get", "ServiceStatus", DefaultParameterSetName = "Identity")]
	public sealed class GetServiceStatus : ReportingTask<OrganizationIdParameter>
	{
		[Parameter(Mandatory = false)]
		public uint MaintenanceWindowDays
		{
			get
			{
				return (uint)(base.Fields["MaintenanceWindowDays"] ?? 14U);
			}
			set
			{
				base.Fields["MaintenanceWindowDays"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.ValidateSProcExists("[Exchange2010].[ServiceStatusCurrentStatus]");
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			if (dataObject != null)
			{
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)dataObject;
				if (adorganizationalUnit.OrganizationId.OrganizationalUnit != null && adorganizationalUnit.OrganizationId.ConfigurationUnit != null)
				{
					this.GetTenantServiceStatus(adorganizationalUnit.OrganizationId.OrganizationalUnit);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter();
			if (dataObjects != null)
			{
				if (this.Identity != null)
				{
					base.WriteResult<T>(dataObjects);
				}
				else if (base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					this.GetTenantServiceStatus(null);
				}
				else
				{
					if (base.CurrentOrganizationId.OrganizationalUnit != null && base.CurrentOrganizationId.ConfigurationUnit != null)
					{
						this.GetTenantServiceStatus(base.CurrentOrganizationId.OrganizationalUnit);
					}
					base.WriteResult<T>(dataObjects);
				}
			}
			TaskLogger.LogExit();
		}

		private void GetTenantServiceStatus(ADObjectId tenantId)
		{
			SqlDataReader sqlDataReader = null;
			string text = (tenantId != null) ? tenantId.Name : "<all>";
			base.TraceInfo("Getting service status for tenant: {0}", new object[]
			{
				text
			});
			try
			{
				string sqlConnectionString = base.GetSqlConnectionString();
				using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
				{
					base.TraceInfo("Opening SQL connecttion: {0}", new object[]
					{
						sqlConnectionString
					});
					sqlConnection.Open();
					SqlCommand sqlCommand = new SqlCommand("[Exchange2010].[ServiceStatusCurrentStatus]", sqlConnection);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					if (tenantId != null)
					{
						sqlCommand.Parameters.Add("@TenantGuid", SqlDbType.UniqueIdentifier).Value = tenantId.ObjectGuid;
					}
					base.TraceInfo("Executing stored procedure: {0}", new object[]
					{
						"[Exchange2010].[ServiceStatusCurrentStatus]"
					});
					sqlDataReader = sqlCommand.ExecuteReader();
					base.TraceInfo("Processing current service status data for tenant: {0}", new object[]
					{
						text
					});
					this.ProcessCurrentServiceStatusData(tenantId, sqlDataReader);
					base.TraceInfo("Finished processing current service status data for tenant: {0}", new object[]
					{
						text
					});
				}
			}
			catch (SqlException ex)
			{
				base.WriteError(new SqlReportingConnectionException(ex.Message), (ErrorCategory)1000, null);
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
		}

		private void ProcessCurrentServiceStatusData(ADObjectId tenantId, SqlDataReader reader)
		{
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			Hashtable hashtable3 = new Hashtable();
			while (reader.Read())
			{
				string text = (string)reader["TenantName"];
				Guid guid = (Guid)reader["TenantGuid"];
				bool flag = (int)reader["IsOrganizationConfig"] == 1;
				string entityName = (string)reader["ManagedEntityFullName"];
				ExDateTime problemTime = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)reader["DateTime"]);
				int impactedUserCount = (int)reader["AllMailboxCount"];
				ServiceStatus serviceStatus;
				List<string> value;
				if (hashtable.Contains(guid))
				{
					serviceStatus = (ServiceStatus)hashtable[guid];
					value = (List<string>)hashtable2[guid];
				}
				else
				{
					serviceStatus = new ServiceStatus();
					ADObjectId adobjectId;
					if (flag)
					{
						adobjectId = base.RootOrgContainerId;
					}
					else
					{
						adobjectId = base.ResolveTenantIdentity(text, guid, tenantId, ref hashtable3);
					}
					if (adobjectId == null)
					{
						continue;
					}
					serviceStatus.Identity = adobjectId;
					hashtable.Add(guid, serviceStatus);
					value = new List<string>();
					hashtable2.Add(guid, value);
				}
				if (this.TryMapEntityToTenantServiceStatus(text, entityName, problemTime, impactedUserCount, ref serviceStatus, ref value))
				{
					serviceStatus.MaintenanceWindowDays = this.MaintenanceWindowDays;
				}
			}
			foreach (object obj in hashtable.Values)
			{
				ServiceStatus dataObject = (ServiceStatus)obj;
				base.WriteResult(dataObject);
			}
		}

		private bool TryMapEntityToTenantServiceStatus(string tenantName, string entityName, ExDateTime problemTime, int impactedUserCount, ref ServiceStatus tenantServiceStatus, ref List<string> tenantEntityList)
		{
			if (string.IsNullOrEmpty(entityName))
			{
				this.WriteWarning(Strings.NoEntityLinkedToTenantInReportingDB(tenantName));
				return false;
			}
			string adSite = string.Empty;
			string text = entityName.Remove(entityName.IndexOf(":"));
			string key;
			if ((key = text) != null)
			{
				if (<PrivateImplementationDetails>{37FB2C37-8946-4F00-B324-D759A1883C9F}.$$method0x6002eea-1 == null)
				{
					<PrivateImplementationDetails>{37FB2C37-8946-4F00-B324-D759A1883C9F}.$$method0x6002eea-1 = new Dictionary<string, int>(7)
					{
						{
							"Microsoft.Exchange.2010.Mailbox.DatabaseService",
							0
						},
						{
							"Microsoft.Exchange.2010.ClientAccessActiveSyncService",
							1
						},
						{
							"Microsoft.Exchange.2010.ClientAccessOutlookWebAccessService",
							2
						},
						{
							"Microsoft.Exchange.2010.ClientAccessImap4Service",
							3
						},
						{
							"Microsoft.Exchange.2010.ClientAccessPop3Service",
							4
						},
						{
							"Microsoft.Exchange.2010.ClientAccessWebServicesService",
							5
						},
						{
							"Microsoft.Exchange.2010.HubTransportService",
							6
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{37FB2C37-8946-4F00-B324-D759A1883C9F}.$$method0x6002eea-1.TryGetValue(key, out num))
				{
					Status tenantStatus;
					switch (num)
					{
					case 0:
						tenantStatus = this.GetTenantStatus(tenantServiceStatus, StatusType.MailboxDatabaseOffline);
						if (!tenantEntityList.Contains(entityName))
						{
							tenantEntityList.Add(entityName);
							tenantStatus.ImpactedUserCount += impactedUserCount;
						}
						tenantStatus.StatusMessage = Strings.MailboxDatabaseCasImap4CasPop3CasWebServicesServiceProblemMessage(tenantStatus.ImpactedUserCount);
						break;
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
						tenantStatus = this.GetTenantStatus(tenantServiceStatus, StatusType.ClientAccessOffline);
						adSite = this.GetADSiteName(entityName);
						if (!tenantEntityList.Exists((string name) => (name.StartsWith("Microsoft.Exchange.2010.ClientAccessActiveSyncService:") && name.EndsWith(adSite)) || (name.StartsWith("Microsoft.Exchange.2010.ClientAccessOutlookWebAccessService:") && name.EndsWith(adSite)) || (name.StartsWith("Microsoft.Exchange.2010.ClientAccessImap4Service:") && name.EndsWith(adSite)) || (name.StartsWith("Microsoft.Exchange.2010.ClientAccessPop3Service:") && name.EndsWith(adSite)) || (name.StartsWith("Microsoft.Exchange.2010.ClientAccessWebServicesService:") && name.EndsWith(adSite))))
						{
							tenantStatus.ImpactedUserCount += impactedUserCount;
						}
						if (!tenantEntityList.Contains(entityName))
						{
							tenantEntityList.Add(entityName);
						}
						if (text == "Microsoft.Exchange.2010.ClientAccessActiveSyncService")
						{
							tenantStatus.StatusMessage = Strings.CasActiveSyncServiceProblemMessage(tenantStatus.ImpactedUserCount);
						}
						else
						{
							tenantStatus.StatusMessage = Strings.MailboxDatabaseCasImap4CasPop3CasWebServicesServiceProblemMessage(tenantStatus.ImpactedUserCount);
						}
						break;
					case 6:
						tenantStatus = this.GetTenantStatus(tenantServiceStatus, StatusType.TransportOffline);
						adSite = this.GetADSiteName(entityName);
						if (!tenantEntityList.Exists((string name) => name.StartsWith("Microsoft.Exchange.2010.HubTransportService:") && name.EndsWith(adSite)))
						{
							tenantStatus.ImpactedUserCount += impactedUserCount;
						}
						if (!tenantEntityList.Contains(entityName))
						{
							tenantEntityList.Add(entityName);
						}
						tenantStatus.StatusMessage = Strings.TransportServiceProblemMessage(tenantStatus.ImpactedUserCount);
						break;
					default:
						goto IL_240;
					}
					if (tenantStatus.StartDateTime == (DateTime)ExDateTime.MinValue || (DateTime)problemTime < tenantStatus.StartDateTime)
					{
						tenantStatus.StartDateTime = (DateTime)problemTime;
					}
					return true;
				}
			}
			IL_240:
			this.WriteWarning(Strings.UnknownEntityLinkedToTenantInReportingDB(tenantName, entityName));
			return false;
		}

		private string GetADSiteName(string entityName)
		{
			string result = string.Empty;
			int num = entityName.LastIndexOf(" - ");
			if (num > -1 && num + 3 < entityName.Length)
			{
				result = entityName.Substring(num + 3);
			}
			return result;
		}

		private Status GetTenantStatus(ServiceStatus tenantServiceStatus, StatusType statusType)
		{
			Status status = tenantServiceStatus.StatusList.Find((Status s) => s.StatusType == statusType);
			if (status == null)
			{
				status = new Status(statusType);
				if (!tenantServiceStatus.StatusList.Contains(status))
				{
					tenantServiceStatus.StatusList.Add(status);
				}
			}
			return status;
		}

		private const string CmdletNoun = "ServiceStatus";

		private const string SPName = "[Exchange2010].[ServiceStatusCurrentStatus]";

		private const uint DefaultMaintenanceWindowDays = 14U;
	}
}
