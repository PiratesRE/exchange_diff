using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal class ComplianceBinding
	{
		public ComplianceBinding()
		{
			this.JobStartTime = ComplianceJobConstants.MinComplianceTime;
		}

		public Guid TenantId { get; set; }

		public Guid JobRunId { get; set; }

		public string Bindings { get; set; }

		public ushort BindingOptions { get; set; }

		internal ComplianceBindingType BindingType { get; set; }

		internal DateTime JobStartTime { get; set; }

		internal string JobMaster { get; set; }

		internal int NumBindings { get; set; }

		internal int NumBindingsFailed { get; set; }

		internal ComplianceJobStatus JobStatus { get; set; }

		internal byte[] JobResults { get; set; }

		internal bool AllBindings
		{
			get
			{
				return (this.BindingOptions & 1) == 1;
			}
			set
			{
				if (value)
				{
					this.BindingOptions |= 1;
					return;
				}
				this.BindingOptions &= 65534;
			}
		}

		internal string[] BindingsArray
		{
			get
			{
				if (this.Bindings == null)
				{
					return null;
				}
				return Utils.JsonStringToStringArray(this.Bindings);
			}
			set
			{
				if (value == null)
				{
					this.Bindings = null;
					return;
				}
				this.Bindings = Utils.StringArrayToJsonString(value);
			}
		}

		internal void CopyFromRow(TempDatabase.ComplianceJobBindingTable row)
		{
			this.TenantId = row.TenantId;
			this.JobRunId = row.JobRunId;
			this.BindingOptions = row.BindingOptions;
			this.Bindings = row.Bindings;
			this.BindingType = row.BindingType;
			this.JobStartTime = row.JobStartTime;
			this.JobResults = row.JobResults;
			this.JobStatus = row.JobStatus;
			this.NumBindings = row.NumberBindings;
			this.NumBindingsFailed = row.NumberBindingsFailed;
			this.JobMaster = row.JobMaster;
		}

		internal const ushort BindingOptionAllBindings = 1;
	}
}
