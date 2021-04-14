using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class ReportingTask<TIdentity> : GetTenantADObjectWithIdentityTaskBase<TIdentity, ADOrganizationalUnit> where TIdentity : IIdentityParameter, new()
	{
		[Parameter(Mandatory = false)]
		public Fqdn ReportingServer
		{
			get
			{
				return (Fqdn)(base.Fields["ReportingServer"] ?? this.DefaultReportingServerName);
			}
			set
			{
				base.Fields["ReportingServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ReportingDatabase
		{
			get
			{
				return (string)(base.Fields["ReportingDatabase"] ?? "pdm-TenantDS");
			}
			set
			{
				base.Fields["ReportingDatabase"] = value;
			}
		}

		public override TIdentity Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 107, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\ReportingTask.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			return tenantOrTopologyConfigurationSession;
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = new ExistsFilter(ExchangeConfigurationUnitSchema.OrganizationalUnitLink);
				QueryFilter result;
				if (base.InternalFilter != null)
				{
					result = new AndFilter(new QueryFilter[]
					{
						base.InternalFilter,
						queryFilter
					});
				}
				else
				{
					result = queryFilter;
				}
				return result;
			}
		}

		protected string GetSqlConnectionString()
		{
			return string.Format("server={0};database={1};Integrated Security=SSPI", this.ReportingServer.ToString(), this.ReportingDatabase);
		}

		protected void TraceInfo(string message)
		{
			ExTraceGlobals.TraceTracer.Information((long)this.GetHashCode(), message);
		}

		protected void TraceInfo(string format, params object[] args)
		{
			this.TraceInfo(string.Format(format, args));
		}

		protected void ValidateSProcExists(string sprocName)
		{
			try
			{
				string sqlConnectionString = this.GetSqlConnectionString();
				using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
				{
					this.TraceInfo("Opening SQL connection: {0}", new object[]
					{
						sqlConnectionString
					});
					sqlConnection.Open();
					string text = "IF (EXISTS (SELECT name FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') AND type = N'P'))\r\n                        SELECT 1\r\n                    ELSE \r\n                        SELECT 0;";
					text = string.Format(text, sprocName);
					SqlCommand sqlCommand = new SqlCommand(text, sqlConnection);
					sqlCommand.CommandType = CommandType.Text;
					this.TraceInfo("Executing SQL statement: {0}", new object[]
					{
						text
					});
					if (!Convert.ToBoolean(sqlCommand.ExecuteScalar()))
					{
						base.WriteError(new ReportsMPNotInstalledException(), (ErrorCategory)1001, null);
					}
					this.TraceInfo("Finished executing SQL statement: {0}", new object[]
					{
						text
					});
				}
			}
			catch (SqlException ex)
			{
				base.WriteError(new SqlReportingConnectionException(ex.Message), (ErrorCategory)1000, null);
			}
		}

		protected ADObjectId ResolveTenantIdentity(string tenantNameFromReportingDB, Guid tenantGuidFromReportingDB, ADObjectId tenantId, ref Hashtable unresolvedTenants)
		{
			if (unresolvedTenants == null)
			{
				throw new ArgumentNullException("unresolvedTenants");
			}
			if (tenantId != null && tenantId.ObjectGuid == tenantGuidFromReportingDB)
			{
				return tenantId;
			}
			if (unresolvedTenants.Contains(tenantGuidFromReportingDB))
			{
				return null;
			}
			ADObjectId result;
			try
			{
				this.ConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(new OrganizationIdParameter(tenantGuidFromReportingDB.ToString()), this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(tenantNameFromReportingDB)), new LocalizedString?(Strings.ErrorOrganizationNotUnique(tenantNameFromReportingDB)));
				result = adorganizationalUnit.OrganizationId.OrganizationalUnit;
			}
			catch (ManagementObjectNotFoundException ex)
			{
				base.WriteWarning(ex.Message);
				unresolvedTenants.Add(tenantGuidFromReportingDB, null);
				result = null;
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null)
			{
				IEnumerable<ADOrganizationalUnit> dataObjects = base.GetDataObjects(this.Identity);
				this.WriteResult<ADOrganizationalUnit>(dataObjects);
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private const string DefaultReportingDatabaseName = "pdm-TenantDS";

		private Fqdn DefaultReportingServerName = Fqdn.Parse("pdm-tenantds.exmgmt.local");
	}
}
