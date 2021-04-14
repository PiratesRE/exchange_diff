using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal class TempDatabase
	{
		private TempDatabase()
		{
			this.CreateDatabase();
		}

		internal static TempDatabase Instance
		{
			get
			{
				if (TempDatabase.instance == null)
				{
					TempDatabase.instance = new TempDatabase();
				}
				return TempDatabase.instance;
			}
		}

		public void Delete(ComplianceJob job)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dataContext = new DataContext(sqlConnection))
				{
					Table<TempDatabase.ComplianceJobTable> table = dataContext.GetTable<TempDatabase.ComplianceJobTable>();
					IQueryable<TempDatabase.ComplianceJobTable> queryable = from jobRow in table
					where jobRow.JobId == ((ComplianceJobId)job.Identity).Guid
					select jobRow;
					if (queryable != null && queryable.Count<TempDatabase.ComplianceJobTable>() > 0)
					{
						TempDatabase.ComplianceJobTable entity = queryable.First<TempDatabase.ComplianceJobTable>();
						table.DeleteOnSubmit(entity);
						dataContext.SubmitChanges();
					}
				}
			}
		}

		public void Delete(ComplianceBinding binding)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dataContext = new DataContext(sqlConnection))
				{
					Table<TempDatabase.ComplianceJobBindingTable> table = dataContext.GetTable<TempDatabase.ComplianceJobBindingTable>();
					IQueryable<TempDatabase.ComplianceJobBindingTable> queryable = from bindingRow in table
					where bindingRow.JobRunId == binding.JobRunId && (int)bindingRow.BindingType == (int)binding.BindingType
					select bindingRow;
					if (queryable != null && queryable.Count<TempDatabase.ComplianceJobBindingTable>() > 0)
					{
						TempDatabase.ComplianceJobBindingTable entity = queryable.First<TempDatabase.ComplianceJobBindingTable>();
						table.DeleteOnSubmit(entity);
						dataContext.SubmitChanges();
					}
				}
			}
		}

		public void Delete(CompositeTask task)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dataContext = new DataContext(sqlConnection))
				{
					Table<TempDatabase.CompositeTaskTable> table = dataContext.GetTable<TempDatabase.CompositeTaskTable>();
					IQueryable<TempDatabase.CompositeTaskTable> queryable = from taskRow in table
					where taskRow.JobRunId == task.JobRunId && taskRow.TaskId == task.TaskId
					select taskRow;
					if (queryable != null && queryable.Count<TempDatabase.CompositeTaskTable>() > 0)
					{
						TempDatabase.CompositeTaskTable entity = queryable.First<TempDatabase.CompositeTaskTable>();
						table.DeleteOnSubmit(entity);
						dataContext.SubmitChanges();
					}
				}
			}
		}

		public IEnumerable<ComplianceJob> FindPagedComplianceJobs(Guid tenantId, string jobName, ref string pageCookie, out bool complete, int pageSize = 100)
		{
			complete = false;
			int num = -1;
			if (string.IsNullOrEmpty(pageCookie))
			{
				num = 0;
			}
			else if (!int.TryParse(pageCookie, out num) || num < 0)
			{
				complete = true;
				return Enumerable.Empty<ComplianceJob>();
			}
			IEnumerable<ComplianceJob> enumerable = this.FindPagedComplianceJobsInternal(tenantId, jobName, num, pageSize);
			if (enumerable.Count<ComplianceJob>() < pageSize)
			{
				complete = true;
			}
			num++;
			pageCookie = num.ToString();
			return enumerable;
		}

		internal void InsertIntoTable<TTable, TData>(TData data) where TTable : TempDatabase.IComplianceTable<TData>, new()
		{
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
				{
					using (TempDatabase.ComplianceJobStore complianceJobStore = new TempDatabase.ComplianceJobStore(sqlConnection))
					{
						TTable ttable = (default(TTable) == null) ? Activator.CreateInstance<TTable>() : default(TTable);
						ttable.SetRowData(data);
						ttable.InsertRow(complianceJobStore);
						complianceJobStore.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		internal void UpdateJobTable(ComplianceJob newJob)
		{
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
				{
					using (DataContext dataContext = new DataContext(sqlConnection))
					{
						Table<TempDatabase.ComplianceJobTable> table = dataContext.GetTable<TempDatabase.ComplianceJobTable>();
						IQueryable<TempDatabase.ComplianceJobTable> queryable = from job in table
						where job.JobId == ((ComplianceJobId)newJob.Identity).Guid
						select job;
						if (queryable != null && queryable.Count<TempDatabase.ComplianceJobTable>() > 0)
						{
							TempDatabase.ComplianceJobTable complianceJobTable = queryable.First<TempDatabase.ComplianceJobTable>();
							complianceJobTable.SetRowData(newJob);
							dataContext.SubmitChanges();
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		internal void UpdateBindingTable(ComplianceJob job)
		{
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
				{
					using (DataContext dataContext = new DataContext(sqlConnection))
					{
						Table<TempDatabase.ComplianceJobBindingTable> table = dataContext.GetTable<TempDatabase.ComplianceJobBindingTable>();
						IQueryable<TempDatabase.ComplianceJobBindingTable> queryable = from binding in table
						where binding.JobRunId == job.JobRunId
						select binding;
						foreach (TempDatabase.ComplianceJobBindingTable complianceJobBindingTable in queryable)
						{
							complianceJobBindingTable.SetRowData(job);
						}
						dataContext.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		internal void UpdateBindingTable(ComplianceBinding newBinding)
		{
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
				{
					using (DataContext dataContext = new DataContext(sqlConnection))
					{
						Table<TempDatabase.ComplianceJobBindingTable> table = dataContext.GetTable<TempDatabase.ComplianceJobBindingTable>();
						IQueryable<TempDatabase.ComplianceJobBindingTable> queryable = from binding in table
						where binding.JobRunId == newBinding.JobRunId && (int)binding.BindingType == (int)newBinding.BindingType
						select binding;
						foreach (TempDatabase.ComplianceJobBindingTable complianceJobBindingTable in queryable)
						{
							complianceJobBindingTable.SetRowData(newBinding);
						}
						dataContext.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		internal IEnumerable<T> ReadComplianceJobByName<T>(string jobName, Guid tenantId) where T : IConfigurable, new()
		{
			using (SqlConnection dbConn = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dbContext = new DataContext(dbConn))
				{
					Table<TempDatabase.ComplianceJobTable> jobs = dbContext.GetTable<TempDatabase.ComplianceJobTable>();
					IQueryable<TempDatabase.ComplianceJobTable> jobQuery = from job in jobs
					where job.DisplayName.Equals(jobName) && job.TenantId == tenantId
					select job;
					foreach (TempDatabase.ComplianceJobTable row in jobQuery)
					{
						ComplianceJob newJob = this.CreateComplianceJobFromTableRow<T>(row, dbContext);
						yield return (T)((object)newJob);
					}
				}
			}
			yield break;
		}

		internal IConfigurable ReadComplianceJob<T>(ComplianceJobId identity) where T : IConfigurable, new()
		{
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString()))
				{
					using (DataContext dataContext = new DataContext(sqlConnection))
					{
						Table<TempDatabase.ComplianceJobTable> table = dataContext.GetTable<TempDatabase.ComplianceJobTable>();
						IQueryable<TempDatabase.ComplianceJobTable> source = from job in table
						where job.JobId == identity.Guid
						select job;
						int num = source.Count<TempDatabase.ComplianceJobTable>();
						if (num == 1)
						{
							TempDatabase.ComplianceJobTable row = source.First<TempDatabase.ComplianceJobTable>();
							return this.CreateComplianceJobFromTableRow<T>(row, dataContext);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return null;
		}

		internal IEnumerable<ComplianceJob> FindComplianceJob(Guid tenantId, Guid jobRunId)
		{
			using (SqlConnection dbConn = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dbContext = new DataContext(dbConn))
				{
					Table<TempDatabase.ComplianceJobTable> jobs = dbContext.GetTable<TempDatabase.ComplianceJobTable>();
					IQueryable<TempDatabase.ComplianceJobTable> jobQuery = from job in jobs
					where job.TenantId == tenantId && job.JobRunId == jobRunId
					select job;
					foreach (TempDatabase.ComplianceJobTable row in jobQuery)
					{
						ComplianceJob newObj = this.CreateComplianceJobFromTableRow<ComplianceSearch>(row, dbContext);
						yield return newObj;
					}
				}
			}
			yield break;
		}

		internal IEnumerable<CompositeTask> FindCompositeTasks(Guid tenantId, Guid jobRunId, ComplianceBindingType? bindingType = null, int? taskId = null)
		{
			using (SqlConnection dbConn = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dbContext = new DataContext(dbConn))
				{
					Table<TempDatabase.CompositeTaskTable> tasks = dbContext.GetTable<TempDatabase.CompositeTaskTable>();
					IQueryable<TempDatabase.CompositeTaskTable> taskQuery = from task in tasks
					where task.TenantId == tenantId && task.JobRunId == jobRunId && (taskId == null || (int?)task.TaskId == taskId) && ((int?)bindingType == (int?)null || (int?)task.BindingType == (int?)bindingType)
					select task;
					foreach (TempDatabase.CompositeTaskTable row in taskQuery)
					{
						yield return row.GetCompositeTask();
					}
				}
			}
			yield break;
		}

		internal IEnumerable<ComplianceBinding> FindComplianceJobBindings(Guid tenantId, Guid runId, ComplianceBindingType? bindingType = null)
		{
			using (SqlConnection dbConn = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dbContext = new DataContext(dbConn))
				{
					Table<TempDatabase.ComplianceJobBindingTable> bindings = dbContext.GetTable<TempDatabase.ComplianceJobBindingTable>();
					IQueryable<TempDatabase.ComplianceJobBindingTable> bindingQuery = from binding in bindings
					where binding.JobRunId == runId && binding.TenantId == tenantId && ((int?)bindingType == (int?)null || (int)binding.BindingType == (int)bindingType.Value)
					select binding;
					foreach (TempDatabase.ComplianceJobBindingTable bindingRow in bindingQuery)
					{
						ComplianceBinding bindingObj = new ComplianceBinding();
						bindingObj.CopyFromRow(bindingRow);
						yield return bindingObj;
					}
				}
			}
			yield break;
		}

		private IEnumerable<ComplianceJob> FindPagedComplianceJobsInternal(Guid tenantId, string jobName, int pageIndex, int pageSize = 100)
		{
			using (SqlConnection dbConn = new SqlConnection(this.GetConnectionString()))
			{
				using (DataContext dbContext = new DataContext(dbConn))
				{
					Table<TempDatabase.ComplianceJobTable> jobs = dbContext.GetTable<TempDatabase.ComplianceJobTable>();
					IQueryable<TempDatabase.ComplianceJobTable> jobQuery = (from job in jobs
					where job.TenantId == tenantId && (jobName == null || job.DisplayName.Equals(jobName))
					select job).Skip(pageIndex * pageSize).Take(pageSize);
					foreach (TempDatabase.ComplianceJobTable row in jobQuery)
					{
						ComplianceJob newObj = this.CreateComplianceJobFromTableRow<ComplianceSearch>(row, dbContext);
						yield return newObj;
					}
				}
			}
			yield break;
		}

		private ComplianceJob CreateComplianceJobFromTableRow<T>(TempDatabase.ComplianceJobTable row, DataContext dbContext) where T : IConfigurable, new()
		{
			ComplianceJob newJob = (ComplianceJob)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			newJob.CopyFromRow(row);
			Table<TempDatabase.ComplianceJobBindingTable> table = dbContext.GetTable<TempDatabase.ComplianceJobBindingTable>();
			IQueryable<TempDatabase.ComplianceJobBindingTable> queryable = from binding in table
			where binding.JobRunId == row.JobRunId && binding.TenantId == newJob.TenantId
			select binding;
			foreach (TempDatabase.ComplianceJobBindingTable complianceJobBindingTable in queryable)
			{
				newJob.Bindings[complianceJobBindingTable.BindingType].CopyFromRow(complianceJobBindingTable);
			}
			newJob.UpdateJobResults();
			return newJob;
		}

		private string GetConnectionString()
		{
			return string.Format("Server={0};database={1};Trusted_Connection=Yes;", this.databaseServerName, "ComplianceJobTempDB");
		}

		private void CreateDatabase()
		{
			try
			{
				string userDomainName = Environment.UserDomainName;
				this.databaseServerName = userDomainName.Substring(0, userDomainName.IndexOf("dom", StringComparison.InvariantCultureIgnoreCase));
				string connectionString = string.Format("Server={0};Trusted_Connection=Yes;", this.databaseServerName);
				using (SqlConnection sqlConnection = new SqlConnection(connectionString))
				{
					using (DataContext dataContext = new DataContext(sqlConnection))
					{
						Table<TempDatabase.SysDatabaseTable> table = dataContext.GetTable<TempDatabase.SysDatabaseTable>();
						IQueryable<TempDatabase.SysDatabaseTable> source = from db in table
						where db.Name == "ComplianceJobTempDB"
						select db;
						int num = source.Count<TempDatabase.SysDatabaseTable>();
						if (num <= 0)
						{
							using (TempDatabase.ComplianceJobStore complianceJobStore = new TempDatabase.ComplianceJobStore(sqlConnection))
							{
								complianceJobStore.CreateDatabase();
							}
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw ex;
			}
		}

		private const string DatabaseName = "ComplianceJobTempDB";

		private const string TableNameComplianceJob = "ComplianceJobTempTable";

		private const string TableNameComplianceJobBinding = "ComplianceJobBindingTempTable";

		private const string TableNameCompositeTask = "CompositeTaskTempTable";

		private static TempDatabase instance;

		private string databaseServerName;

		public interface IComplianceTable<TData>
		{
			void SetRowData(TData data);

			void InsertRow(TempDatabase.ComplianceJobStore store);
		}

		[Database(Name = "ComplianceJobTempDB")]
		internal class ComplianceJobStore : DataContext
		{
			internal ComplianceJobStore(string connection) : base(connection)
			{
				this.Jobs = base.GetTable<TempDatabase.ComplianceJobTable>();
				this.Bindings = base.GetTable<TempDatabase.ComplianceJobBindingTable>();
				this.Tasks = base.GetTable<TempDatabase.CompositeTaskTable>();
			}

			internal ComplianceJobStore(SqlConnection conn) : base(conn)
			{
				this.Jobs = base.GetTable<TempDatabase.ComplianceJobTable>();
				this.Bindings = base.GetTable<TempDatabase.ComplianceJobBindingTable>();
				this.Tasks = base.GetTable<TempDatabase.CompositeTaskTable>();
			}

			internal Table<TempDatabase.ComplianceJobTable> Jobs;

			internal Table<TempDatabase.ComplianceJobBindingTable> Bindings;

			internal Table<TempDatabase.CompositeTaskTable> Tasks;
		}

		[Table(Name = "dbo.ComplianceJobTempTable")]
		internal class ComplianceJobTable : TempDatabase.IComplianceTable<ComplianceJob>
		{
			public void SetRowData(ComplianceJob job)
			{
				this.DisplayName = job.Name;
				this.JobId = ((ComplianceJobId)job.Identity).Guid;
				this.CreateTime = job.CreatedTime;
				this.LastModifiedTime = job.LastModifiedTime;
				this.StartTime = job.JobStartTime;
				this.EndTime = job.JobEndTime;
				this.Description = job.Description;
				this.JobObjectVersion = job.JobObjectVersion;
				this.TenantId = job.TenantId;
				this.JobType = job.JobType;
				this.CreatedBy = job.CreatedBy;
				this.RunBy = job.RunBy;
				this.JobRunId = job.JobRunId;
				this.TenantInfo = job.TenantInfo;
				this.JobData = job.JobData;
				this.Resume = job.Resume;
			}

			public void InsertRow(TempDatabase.ComplianceJobStore store)
			{
				store.Jobs.InsertOnSubmit(this);
			}

			[Column]
			public string DisplayName;

			[Column]
			public Guid TenantId;

			[Column(IsPrimaryKey = true)]
			public Guid JobId;

			[Column]
			public ComplianceJobObjectVersion JobObjectVersion;

			[Column]
			public string Description;

			[Column]
			public ComplianceJobType JobType;

			[Column]
			public DateTime CreateTime;

			[Column]
			public DateTime LastModifiedTime;

			[Column]
			public string CreatedBy;

			[Column]
			public byte[] TenantInfo;

			[Column]
			public byte[] JobData;

			[Column]
			public Guid JobRunId;

			[Column]
			public string RunBy;

			[Column]
			public DateTime StartTime;

			[Column]
			public DateTime EndTime;

			[Column]
			public bool Resume;
		}

		[Table(Name = "dbo.ComplianceJobBindingTempTable")]
		internal class ComplianceJobBindingTable : TempDatabase.IComplianceTable<ComplianceBinding>
		{
			public void SetRowData(ComplianceBinding binding)
			{
				this.TenantId = binding.TenantId;
				this.BindingType = binding.BindingType;
				this.Bindings = binding.Bindings;
				this.BindingOptions = binding.BindingOptions;
				this.JobStartTime = binding.JobStartTime;
				this.JobRunId = binding.JobRunId;
				this.JobStatus = binding.JobStatus;
				this.NumberBindings = binding.NumBindings;
				this.NumberBindingsFailed = binding.NumBindingsFailed;
				this.JobResults = binding.JobResults;
				this.JobMaster = binding.JobMaster;
			}

			public void InsertRow(TempDatabase.ComplianceJobStore store)
			{
				store.Bindings.InsertOnSubmit(this);
			}

			public void SetRowData(ComplianceJob job)
			{
				this.SetRowData(job.Bindings[this.BindingType]);
			}

			[Column]
			public Guid TenantId;

			[Column(IsPrimaryKey = true)]
			public Guid JobRunId;

			[Column(IsPrimaryKey = true)]
			public ComplianceBindingType BindingType;

			[Column]
			public string Bindings;

			[Column]
			public ushort BindingOptions;

			[Column]
			public DateTime JobStartTime;

			[Column]
			public string JobMaster;

			[Column]
			public int NumberBindings;

			[Column]
			public int NumberBindingsFailed;

			[Column]
			public ComplianceJobStatus JobStatus;

			[Column]
			public byte[] JobResults;
		}

		[Table(Name = "dbo.CompositeTaskTempTable")]
		internal class CompositeTaskTable : TempDatabase.IComplianceTable<CompositeTask>
		{
			public void SetRowData(CompositeTask task)
			{
				this.TenantId = task.TenantId;
				this.JobRunId = task.JobRunId;
				this.TaskId = task.TaskId;
				this.BindingType = task.BindingType;
				this.UserMaster = task.UserMaster;
				this.UserList = task.UserList;
			}

			public void InsertRow(TempDatabase.ComplianceJobStore store)
			{
				store.Tasks.InsertOnSubmit(this);
			}

			public CompositeTask GetCompositeTask()
			{
				return new CompositeTask
				{
					TenantId = this.TenantId,
					JobRunId = this.JobRunId,
					TaskId = this.TaskId,
					BindingType = this.BindingType,
					UserMaster = this.UserMaster,
					UserList = this.UserList
				};
			}

			[Column]
			public Guid TenantId;

			[Column(IsPrimaryKey = true)]
			public Guid JobRunId;

			[Column(IsPrimaryKey = true)]
			public int TaskId;

			[Column]
			public ComplianceBindingType BindingType;

			[Column]
			public string UserMaster;

			[Column]
			public string UserList;
		}

		internal class SysView : DataContext
		{
			internal SysView(string connection) : base(connection)
			{
			}

			internal Table<TempDatabase.SysDatabaseTable> Databases;
		}

		[Table(Name = "sys.databases")]
		internal class SysDatabaseTable
		{
			[Column]
			internal int DatabaseId;

			[Column]
			internal string Name;
		}
	}
}
