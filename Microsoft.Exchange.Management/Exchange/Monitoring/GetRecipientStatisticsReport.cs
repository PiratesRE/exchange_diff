using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Get", "RecipientStatisticsReport", DefaultParameterSetName = "Identity")]
	public sealed class GetRecipientStatisticsReport : ReportingTask<OrganizationIdParameter>
	{
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
			base.ValidateSProcExists("[Exchange2010].[RecipientStatisticsReport]");
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
					this.GetTenantRecipientStatistics(adorganizationalUnit.OrganizationId.OrganizationalUnit);
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
					this.GetTenantRecipientStatistics(base.RootOrgContainerId);
				}
				else if (base.CurrentOrganizationId.OrganizationalUnit != null && base.CurrentOrganizationId.ConfigurationUnit != null)
				{
					this.GetTenantRecipientStatistics(base.CurrentOrganizationId.OrganizationalUnit);
				}
			}
			TaskLogger.LogExit();
		}

		private void GetTenantRecipientStatistics(ADObjectId organizationId)
		{
			SqlDataReader sqlDataReader = null;
			base.TraceInfo("Getting service status for tenant: {0}", new object[]
			{
				organizationId.Name
			});
			Guid guid = organizationId.Equals(base.RootOrgContainerId) ? Guid.Empty : organizationId.ObjectGuid;
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
					SqlCommand sqlCommand = new SqlCommand("[Exchange2010].[RecipientStatisticsReport]", sqlConnection);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@TenantGuid", SqlDbType.UniqueIdentifier).Value = guid;
					base.TraceInfo("Executing stored procedure: {0}", new object[]
					{
						"[Exchange2010].[RecipientStatisticsReport]"
					});
					sqlDataReader = sqlCommand.ExecuteReader();
					base.TraceInfo("Processing Recipient Statistics data for tenant: {0}", new object[]
					{
						organizationId.Name
					});
					this.ProcessTenantRecipientStatistics(organizationId, sqlDataReader);
					base.TraceInfo("Finished Recipient Statistics data for tenant: {0}", new object[]
					{
						organizationId.Name
					});
				}
			}
			catch (SqlException ex)
			{
				base.WriteError(new SqlReportingConnectionException(ex.Message), (ErrorCategory)1000, null);
			}
			catch (InvalidOperationException ex2)
			{
				base.WriteError(new SqlReportingConnectionException(ex2.Message), (ErrorCategory)1001, null);
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
		}

		private void ProcessTenantRecipientStatistics(ADObjectId tenantId, SqlDataReader reader)
		{
			if (reader.Read())
			{
				base.WriteResult(new RecipientStatisticsReport
				{
					Identity = tenantId,
					TotalNumberOfMailboxes = uint.Parse(reader["AllMailboxCount"].ToString()),
					TotalNumberOfActiveMailboxes = uint.Parse(reader["ActiveMailboxCount"].ToString()),
					NumberOfContacts = uint.Parse(reader["TenantContactCount"].ToString()),
					NumberOfDistributionLists = uint.Parse(reader["TenantDLCount"].ToString()),
					LastUpdated = (DateTime)new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)reader["DateTime"])
				});
			}
		}

		private const string CmdletNoun = "RecipientStatisticsReport";

		private const string RecipientStatisticsSPName = "[Exchange2010].[RecipientStatisticsReport]";
	}
}
