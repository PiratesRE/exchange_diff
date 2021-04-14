using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal class ComplianceJobProvider : IConfigDataProvider
	{
		public ComplianceJobProvider(OrganizationId orgId)
		{
			this.organizationId = orgId;
		}

		public string Source
		{
			get
			{
				return "ComplianceJobTempDB";
			}
		}

		public void Delete(IConfigurable instance)
		{
			ComplianceJob complianceJob = (ComplianceJob)instance;
			foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in complianceJob.Bindings)
			{
				TempDatabase.Instance.Delete(keyValuePair.Value);
			}
			this.DeleteTasks(complianceJob.TenantId, complianceJob.JobRunId, null);
			TempDatabase.Instance.Delete(complianceJob);
		}

		public IEnumerable<ComplianceJob> FindComplianceJob(Guid tenantId, Guid runId)
		{
			return TempDatabase.Instance.FindComplianceJob(tenantId, runId);
		}

		public IEnumerable<CompositeTask> FindCompositeTasks(Guid tenantId, Guid runId, ComplianceBindingType? bindingType = null, int? taskId = null)
		{
			return TempDatabase.Instance.FindCompositeTasks(tenantId, runId, bindingType, taskId);
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			int defaultPageSize = 100;
			bool completed = false;
			string pageCookie = string.Empty;
			int itemsToFetch = pageSize ?? int.MaxValue;
			while (itemsToFetch > 0 && !completed)
			{
				int actualPageSize = (defaultPageSize > itemsToFetch) ? itemsToFetch : defaultPageSize;
				IEnumerable<ComplianceJob> jobs = TempDatabase.Instance.FindPagedComplianceJobs(this.organizationId.OrganizationalUnit.ObjectGuid, null, ref pageCookie, out completed, actualPageSize);
				foreach (ComplianceJob job in jobs)
				{
					itemsToFetch--;
					yield return (T)((object)job);
				}
			}
			yield break;
		}

		public IConfigurable FindJobsByName<T>(string jobName) where T : IConfigurable, new()
		{
			IEnumerable<T> source = TempDatabase.Instance.ReadComplianceJobByName<T>(jobName, this.organizationId.OrganizationalUnit.ObjectGuid);
			if (source.Count<T>() == 0)
			{
				return default(T);
			}
			IConfigurable configurable = source.First<T>();
			((ConfigurableObject)configurable).ResetChangeTracking(true);
			return configurable;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			IConfigurable configurable = TempDatabase.Instance.ReadComplianceJob<T>((ComplianceJobId)identity);
			((ConfigurableObject)configurable).ResetChangeTracking(true);
			return configurable;
		}

		public void Save(IConfigurable instance)
		{
			if (instance is ComplianceJob)
			{
				this.SaveComplianceJob(instance as ComplianceJob);
				return;
			}
			if (instance is CompositeTask)
			{
				this.SaveComplianceTask(instance as CompositeTask);
				return;
			}
			throw new NotImplementedException();
		}

		public void SaveComplianceJob(ComplianceJob job)
		{
			if (job.ObjectState == ObjectState.New)
			{
				this.AddComplianceJob(job);
			}
			else if (job.ObjectState == ObjectState.Changed)
			{
				if (job.NewRunId)
				{
					ComplianceJob instance = (ComplianceJob)this.Read<ComplianceSearch>(job.Identity);
					this.Delete(instance);
					this.AddComplianceJob(job);
				}
				else
				{
					TempDatabase.Instance.UpdateJobTable(job);
					TempDatabase.Instance.UpdateBindingTable(job);
				}
			}
			job.NewRunId = false;
			job.ResetChangeTracking(true);
		}

		internal void UpdateWorkloadResults(Guid runId, byte[] jobResults, ComplianceBindingType bindingType, ComplianceJobStatus status)
		{
			IEnumerable<ComplianceBinding> enumerable = TempDatabase.Instance.FindComplianceJobBindings(this.organizationId.OrganizationalUnit.ObjectGuid, runId, new ComplianceBindingType?(bindingType));
			if (enumerable != null)
			{
				ComplianceBinding complianceBinding = enumerable.First<ComplianceBinding>();
				if (jobResults != null)
				{
					complianceBinding.JobResults = jobResults;
				}
				if (complianceBinding.JobStatus != ComplianceJobStatus.StatusUnknown)
				{
					complianceBinding.JobStatus = status;
				}
				switch (status)
				{
				case ComplianceJobStatus.Succeeded:
				case ComplianceJobStatus.Failed:
				case ComplianceJobStatus.PartiallySucceeded:
					this.DeleteTasks(this.organizationId.OrganizationalUnit.ObjectGuid, runId, new ComplianceBindingType?(bindingType));
					break;
				}
				TempDatabase.Instance.UpdateBindingTable(complianceBinding);
			}
		}

		private void AddComplianceJob(ComplianceJob job)
		{
			job.TenantId = this.organizationId.OrganizationalUnit.ObjectGuid;
			job.TenantInfo = this.organizationId.GetBytes(Encoding.UTF8);
			TempDatabase.Instance.InsertIntoTable<TempDatabase.ComplianceJobTable, ComplianceJob>(job);
			foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in job.Bindings)
			{
				TempDatabase.Instance.InsertIntoTable<TempDatabase.ComplianceJobBindingTable, ComplianceBinding>(keyValuePair.Value);
			}
		}

		private void SaveComplianceTask(CompositeTask task)
		{
			if (task.ObjectState == ObjectState.New)
			{
				task.TenantId = this.organizationId.OrganizationalUnit.ObjectGuid;
				TempDatabase.Instance.InsertIntoTable<TempDatabase.CompositeTaskTable, CompositeTask>(task);
				task.ResetChangeTracking(true);
				return;
			}
			throw new NotImplementedException();
		}

		private void DeleteTasks(Guid tenantId, Guid runId, ComplianceBindingType? type = null)
		{
			IEnumerable<CompositeTask> enumerable = TempDatabase.Instance.FindCompositeTasks(tenantId, runId, type, null);
			foreach (CompositeTask task in enumerable)
			{
				TempDatabase.Instance.Delete(task);
			}
		}

		private const string DatabaseName = "ComplianceJobTempDB";

		private readonly OrganizationId organizationId;
	}
}
