using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	[Serializable]
	public abstract class ComplianceJob : ConfigurableObject
	{
		public ComplianceJob() : base(new SimplePropertyBag(ComplianceJobSchema.Identity, ComplianceJobSchema.ObjectState, ComplianceJobSchema.ExchangeVersion))
		{
			this.CreatedTime = ComplianceJobConstants.MinComplianceTime;
			this.LastModifiedTime = ComplianceJobConstants.MinComplianceTime;
			this.JobStartTime = ComplianceJobConstants.MinComplianceTime;
			this.JobEndTime = ComplianceJobConstants.MinComplianceTime;
			this.bindings.Add(ComplianceBindingType.ExchangeBinding, new ComplianceBinding
			{
				BindingType = ComplianceBindingType.ExchangeBinding
			});
			this.bindings.Add(ComplianceBindingType.PublicFolderBinding, new ComplianceBinding
			{
				BindingType = ComplianceBindingType.PublicFolderBinding
			});
			this.bindings.Add(ComplianceBindingType.SharePointBinding, new ComplianceBinding
			{
				BindingType = ComplianceBindingType.SharePointBinding
			});
		}

		public new ObjectId Identity
		{
			get
			{
				return base.Identity;
			}
			internal set
			{
				this.propertyBag.SetField(ComplianceJobSchema.Identity, value);
			}
		}

		public string Name
		{
			get
			{
				return (string)this[ComplianceJobSchema.DisplayName];
			}
			set
			{
				this[ComplianceJobSchema.DisplayName] = value;
			}
		}

		public DateTime CreatedTime
		{
			get
			{
				return (DateTime)this[ComplianceJobSchema.CreatedTime];
			}
			set
			{
				this[ComplianceJobSchema.CreatedTime] = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return (DateTime)this[ComplianceJobSchema.LastModifiedTime];
			}
			set
			{
				this[ComplianceJobSchema.LastModifiedTime] = value;
			}
		}

		public DateTime JobStartTime
		{
			get
			{
				return (DateTime)this[ComplianceJobSchema.JobStartTime];
			}
			set
			{
				this[ComplianceJobSchema.JobStartTime] = value;
			}
		}

		public DateTime JobEndTime
		{
			get
			{
				return (DateTime)this[ComplianceJobSchema.JobEndTime];
			}
			set
			{
				this[ComplianceJobSchema.JobEndTime] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[ComplianceJobSchema.Description];
			}
			set
			{
				this[ComplianceJobSchema.Description] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[ComplianceJobSchema.CreatedBy];
			}
			internal set
			{
				this[ComplianceJobSchema.CreatedBy] = value;
			}
		}

		public string RunBy
		{
			get
			{
				return (string)this[ComplianceJobSchema.RunBy];
			}
			internal set
			{
				this[ComplianceJobSchema.RunBy] = value;
			}
		}

		public Guid TenantId
		{
			get
			{
				if (this[ComplianceJobSchema.TenantId] != null)
				{
					return (Guid)this[ComplianceJobSchema.TenantId];
				}
				return Guid.Empty;
			}
			internal set
			{
				this[ComplianceJobSchema.TenantId] = value;
				foreach (ComplianceBindingType key in this.bindings.Keys)
				{
					this.bindings[key].TenantId = value;
				}
			}
		}

		public int NumBindings
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in this.bindings)
				{
					num += keyValuePair.Value.NumBindings;
				}
				return num;
			}
		}

		public int NumBindingsFailed
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in this.bindings)
				{
					num += keyValuePair.Value.NumBindingsFailed;
				}
				return num;
			}
		}

		public ComplianceJobStatus JobStatus
		{
			get
			{
				bool flag = false;
				HashSet<ComplianceJobStatus> hashSet = new HashSet<ComplianceJobStatus>();
				foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in this.bindings)
				{
					hashSet.Add(keyValuePair.Value.JobStatus);
					switch (keyValuePair.Value.JobStatus)
					{
					case ComplianceJobStatus.Starting:
					case ComplianceJobStatus.InProgress:
						flag = true;
						break;
					}
				}
				if (flag)
				{
					if (hashSet.Any((ComplianceJobStatus status) => status == ComplianceJobStatus.Starting))
					{
						return ComplianceJobStatus.Starting;
					}
					return ComplianceJobStatus.InProgress;
				}
				else
				{
					if (hashSet.Any((ComplianceJobStatus status) => status == ComplianceJobStatus.NotStarted))
					{
						return ComplianceJobStatus.NotStarted;
					}
					if (hashSet.Any((ComplianceJobStatus status) => status == ComplianceJobStatus.Stopped))
					{
						return ComplianceJobStatus.Stopped;
					}
					if (hashSet.All((ComplianceJobStatus status) => status == ComplianceJobStatus.Succeeded))
					{
						return ComplianceJobStatus.Succeeded;
					}
					if (hashSet.All((ComplianceJobStatus status) => status == ComplianceJobStatus.Failed))
					{
						return ComplianceJobStatus.Failed;
					}
					return ComplianceJobStatus.PartiallySucceeded;
				}
			}
			set
			{
				this[ComplianceJobSchema.JobStatus] = value;
				foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in this.bindings)
				{
					keyValuePair.Value.JobStatus = value;
				}
			}
		}

		public MultiValuedProperty<string> ExchangeBindings
		{
			get
			{
				return this.bindings[ComplianceBindingType.ExchangeBinding].BindingsArray;
			}
			internal set
			{
				this.bindings[ComplianceBindingType.ExchangeBinding].BindingsArray = ((value == null) ? null : value.ToArray());
			}
		}

		public MultiValuedProperty<string> PublicFolderBindings
		{
			get
			{
				return this.bindings[ComplianceBindingType.PublicFolderBinding].BindingsArray;
			}
			internal set
			{
				this.bindings[ComplianceBindingType.PublicFolderBinding].BindingsArray = ((value == null) ? null : value.ToArray());
			}
		}

		public MultiValuedProperty<string> SharePointBindings
		{
			get
			{
				return this.bindings[ComplianceBindingType.SharePointBinding].BindingsArray;
			}
			internal set
			{
				this.bindings[ComplianceBindingType.SharePointBinding].BindingsArray = ((value == null) ? null : value.ToArray());
			}
		}

		public bool AllExchangeBindings
		{
			get
			{
				return this.bindings[ComplianceBindingType.ExchangeBinding].AllBindings;
			}
			set
			{
				this.bindings[ComplianceBindingType.ExchangeBinding].AllBindings = value;
			}
		}

		public bool AllPublicFolderBindings
		{
			get
			{
				return this.bindings[ComplianceBindingType.PublicFolderBinding].AllBindings;
			}
			set
			{
				this.bindings[ComplianceBindingType.PublicFolderBinding].AllBindings = value;
			}
		}

		public bool AllSharePointBindings
		{
			get
			{
				return this.bindings[ComplianceBindingType.SharePointBinding].AllBindings;
			}
			set
			{
				this.bindings[ComplianceBindingType.SharePointBinding].AllBindings = value;
			}
		}

		public Guid JobRunId
		{
			get
			{
				if (this[ComplianceJobSchema.JobRunId] != null)
				{
					return (Guid)this[ComplianceJobSchema.JobRunId];
				}
				return Guid.Empty;
			}
			internal set
			{
				this[ComplianceJobSchema.JobRunId] = value;
				foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in this.bindings)
				{
					keyValuePair.Value.JobRunId = value;
				}
				this.NewRunId = true;
			}
		}

		public bool Resume
		{
			get
			{
				return (bool)this[ComplianceJobSchema.Resume];
			}
			set
			{
				this[ComplianceJobSchema.Resume] = value;
			}
		}

		internal abstract byte[] JobData { get; set; }

		internal ComplianceJobObjectVersion JobObjectVersion
		{
			get
			{
				return (ComplianceJobObjectVersion)this[ComplianceJobSchema.JobObjectVersion];
			}
			set
			{
				this[ComplianceJobSchema.JobObjectVersion] = value;
			}
		}

		internal ComplianceJobType JobType
		{
			get
			{
				return (ComplianceJobType)this[ComplianceJobSchema.JobType];
			}
			set
			{
				this[ComplianceJobSchema.JobType] = value;
			}
		}

		internal byte[] TenantInfo
		{
			get
			{
				return (byte[])this[ComplianceJobSchema.TenantInfo];
			}
			set
			{
				this[ComplianceJobSchema.TenantInfo] = value;
			}
		}

		internal Dictionary<ComplianceBindingType, ComplianceBinding> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		internal bool NewRunId { get; set; }

		internal bool IsRunning()
		{
			switch (this.JobStatus)
			{
			case ComplianceJobStatus.Starting:
			case ComplianceJobStatus.InProgress:
				return true;
			default:
				return false;
			}
		}

		internal void CopyFromRow(TempDatabase.ComplianceJobTable row)
		{
			this.Name = row.DisplayName;
			this.CreatedTime = row.CreateTime;
			this.LastModifiedTime = row.LastModifiedTime;
			this.JobStartTime = row.StartTime;
			this.JobEndTime = row.EndTime;
			this.Identity = new ComplianceJobId(row.JobId);
			this.Description = row.Description;
			this.JobObjectVersion = row.JobObjectVersion;
			this.TenantId = row.TenantId;
			this.JobType = row.JobType;
			this.CreatedBy = row.CreatedBy;
			this.RunBy = row.RunBy;
			this.JobRunId = row.JobRunId;
			this.TenantInfo = row.TenantInfo;
			this.JobData = row.JobData;
			this.Resume = row.Resume;
			this.NewRunId = false;
		}

		internal abstract void UpdateJobResults();

		private Dictionary<ComplianceBindingType, ComplianceBinding> bindings = new Dictionary<ComplianceBindingType, ComplianceBinding>();
	}
}
