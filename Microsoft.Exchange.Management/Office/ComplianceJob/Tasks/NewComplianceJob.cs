using System;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	public abstract class NewComplianceJob<TDataObject> : NewTenantADTaskBase<TDataObject> where TDataObject : ComplianceJob, new()
	{
		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return this.objectToSave.Name;
			}
			set
			{
				this.objectToSave.Name = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string[] ExchangeBinding
		{
			get
			{
				return this.objectToSave.ExchangeBindings.ToArray();
			}
			set
			{
				this.objectToSave.ExchangeBindings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] PublicFolderBinding
		{
			get
			{
				return this.objectToSave.PublicFolderBindings.ToArray();
			}
			set
			{
				this.objectToSave.PublicFolderBindings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] SharePointBinding
		{
			get
			{
				return this.objectToSave.SharePointBindings.ToArray();
			}
			set
			{
				this.objectToSave.SharePointBindings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllExchangeBindings
		{
			get
			{
				return this.objectToSave.AllExchangeBindings;
			}
			set
			{
				this.objectToSave.AllExchangeBindings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllPublicFolderBindings
		{
			get
			{
				return this.objectToSave.AllPublicFolderBindings;
			}
			set
			{
				this.objectToSave.AllPublicFolderBindings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllSharePointBindings
		{
			get
			{
				return this.objectToSave.AllSharePointBindings;
			}
			set
			{
				this.objectToSave.AllSharePointBindings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return this.objectToSave.Description;
			}
			set
			{
				this.objectToSave.Description = value;
			}
		}

		public NewComplianceJob()
		{
		}

		protected override void InternalProcessRecord()
		{
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			this.PrepareDataObjectToSave();
			if (this.objectToSave != null)
			{
				try
				{
					this.PreSaveValidate(this.objectToSave);
					if (base.HasErrors)
					{
						return;
					}
					if (this.objectToSave.Identity != null)
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(this.objectToSave, base.DataSession, typeof(ComplianceJob)));
					}
					base.DataSession.Save(this.objectToSave);
					this.DataObject = this.objectToSave;
					if (!base.HasErrors)
					{
						this.WriteResult();
					}
				}
				catch (SqlException exception)
				{
					base.WriteError(exception, ErrorCategory.WriteError, null);
				}
				catch (ArgumentException exception2)
				{
					base.WriteError(exception2, ErrorCategory.WriteError, null);
				}
				finally
				{
					if (this.objectToSave.Identity != null)
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
					}
				}
			}
			base.InternalEndProcessing();
			TaskLogger.LogExit();
		}

		private void PreSaveValidate(ComplianceJob savedObject)
		{
			if (((ComplianceJobProvider)base.DataSession).FindJobsByName<ComplianceSearch>(savedObject.Name) != null)
			{
				TDataObject dataObject = this.DataObject;
				base.WriteError(new ComplianceSearchNameIsNotUniqueException(dataObject.Name), ErrorCategory.InvalidArgument, savedObject);
			}
		}

		private void PrepareDataObjectToSave()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			if (!(base.DataSession is ComplianceJobProvider))
			{
				base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.UnableToDetermineCreatingUser), ErrorCategory.InvalidOperation, null);
			}
			this.objectToSave.Identity = new ComplianceJobId();
			this.objectToSave.CreatedBy = ((ADObjectId)base.ExchangeRunspaceConfig.ExecutingUserIdentity).Name;
			this.objectToSave.CreatedTime = DateTime.UtcNow;
			this.objectToSave.LastModifiedTime = DateTime.UtcNow;
			this.objectToSave.JobObjectVersion = ComplianceJobObjectVersion.Version1;
			this.objectToSave.JobRunId = CombGuidGenerator.NewGuid();
			this.objectToSave.JobStatus = ComplianceJobStatus.NotStarted;
		}

		protected TDataObject objectToSave = Activator.CreateInstance<TDataObject>();
	}
}
