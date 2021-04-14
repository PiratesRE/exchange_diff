using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Client;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	public class StartComplianceJob<TDataObject> : ObjectActionTenantADTask<ComplianceJobIdParameter, TDataObject> where TDataObject : ComplianceJob, new()
	{
		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Resume
		{
			get
			{
				return (SwitchParameter)(base.Fields["Resume"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Resume"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			return new ComplianceJobProvider(base.ExchangeRunspaceConfig.OrganizationId);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject == null)
			{
				return;
			}
			if (base.ExchangeRunspaceConfig == null)
			{
				base.WriteError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			TDataObject dataObject = this.DataObject;
			switch (dataObject.JobStatus)
			{
			case ComplianceJobStatus.Starting:
			case ComplianceJobStatus.InProgress:
			{
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(new ComplianceJobTaskException(Strings.CannotStartRunningJob(dataObject2.Name)), ErrorCategory.InvalidOperation, this.DataObject);
				break;
			}
			}
			TDataObject dataObject3 = this.DataObject;
			dataObject3.Resume = this.Resume;
			TDataObject dataObject4 = this.DataObject;
			dataObject4.RunBy = ((ADObjectId)base.ExchangeRunspaceConfig.ExecutingUserIdentity).Name;
			TDataObject dataObject5 = this.DataObject;
			dataObject5.JobStartTime = DateTime.UtcNow;
			TDataObject dataObject6 = this.DataObject;
			dataObject6.JobRunId = CombGuidGenerator.NewGuid();
			TDataObject dataObject7 = this.DataObject;
			dataObject7.JobStatus = ComplianceJobStatus.Starting;
			base.DataSession.Save(this.DataObject);
			TDataObject dataObject8 = this.DataObject;
			MultiValuedProperty<string> exchangeBindings = dataObject8.ExchangeBindings;
			if (exchangeBindings != null && exchangeBindings.Count > 0)
			{
				try
				{
					Dictionary<string, ComplianceBindingErrorType> invalidBindings = new Dictionary<string, ComplianceBindingErrorType>();
					TDataObject dataObject9 = this.DataObject;
					dataObject9.Bindings[ComplianceBindingType.ExchangeBinding].JobMaster = base.ExchangeRunspaceConfig.IdentityName;
					TDataObject dataObject10 = this.DataObject;
					dataObject10.Bindings[ComplianceBindingType.ExchangeBinding].JobStartTime = DateTime.UtcNow;
					base.DataSession.Save(this.DataObject);
					ComplianceMessage item = this.CreateStartJobMessage();
					InterExchangeWorkloadClient interExchangeWorkloadClient = new InterExchangeWorkloadClient();
					Task<bool[]> task = interExchangeWorkloadClient.SendMessageAsync(new List<ComplianceMessage>
					{
						item
					});
					IComplianceTaskCreator instance = ComplianceTaskCreatorFactory.GetInstance(ComplianceBindingType.ExchangeBinding);
					IEnumerable<CompositeTask> enumerable = instance.CreateTasks(base.DataSession, this.DataObject, CreateTaskOptions.None, delegate(string invalidBinding, ComplianceBindingErrorType errorType)
					{
						invalidBindings.Add(invalidBinding, errorType);
					});
					int num = 0;
					foreach (CompositeTask compositeTask in enumerable)
					{
						num++;
						int percentCompleted = (num > 99) ? 99 : num;
						base.WriteProgress(Strings.ComplianceSearchCreateTaskActivity, Strings.ComplianceSearchTasksCreated(num), percentCompleted);
					}
					TimeSpan timeout = TimeSpan.FromSeconds(5.0);
					bool flag = true;
					for (int i = 0; i < 6; i++)
					{
						if (System.Threading.Tasks.Task.WaitAll(new System.Threading.Tasks.Task[]
						{
							task
						}, timeout))
						{
							flag = false;
							break;
						}
						base.WriteVerbose(Strings.ComplianceSearchInitializingMessage);
					}
					if (flag)
					{
						TDataObject dataObject11 = this.DataObject;
						dataObject11.JobStatus = ComplianceJobStatus.Failed;
						base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.ComplianceSearchTimeoutError), ErrorCategory.ConnectionError, this.DataObject);
					}
					else if (!task.Result[0])
					{
						TDataObject dataObject12 = this.DataObject;
						dataObject12.JobStatus = ComplianceJobStatus.Failed;
						base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.ComplianceSearchSendStartMessageError("Exchange")), ErrorCategory.ConnectionError, this.DataObject);
					}
					this.CreateWarningMessageForInvalidBindings(invalidBindings);
					TDataObject dataObject13 = this.DataObject;
					dataObject13.Bindings[ComplianceBindingType.ExchangeBinding].JobStatus = ComplianceJobStatus.InProgress;
					ComplianceJobProvider complianceJobProvider = (ComplianceJobProvider)base.DataSession;
					TDataObject dataObject14 = this.DataObject;
					complianceJobProvider.UpdateWorkloadResults(dataObject14.JobRunId, null, ComplianceBindingType.ExchangeBinding, ComplianceJobStatus.InProgress);
				}
				catch (Exception ex)
				{
					TDataObject dataObject15 = this.DataObject;
					dataObject15.JobStatus = ComplianceJobStatus.Failed;
					base.DataSession.Save(this.DataObject);
					throw ex;
				}
			}
			TDataObject dataObject16 = this.DataObject;
			dataObject16.Bindings[ComplianceBindingType.PublicFolderBinding].JobStatus = ComplianceJobStatus.Succeeded;
			ComplianceJobProvider complianceJobProvider2 = (ComplianceJobProvider)base.DataSession;
			TDataObject dataObject17 = this.DataObject;
			complianceJobProvider2.UpdateWorkloadResults(dataObject17.JobRunId, null, ComplianceBindingType.PublicFolderBinding, ComplianceJobStatus.Succeeded);
			TDataObject dataObject18 = this.DataObject;
			dataObject18.Bindings[ComplianceBindingType.SharePointBinding].JobStatus = ComplianceJobStatus.Succeeded;
			ComplianceJobProvider complianceJobProvider3 = (ComplianceJobProvider)base.DataSession;
			TDataObject dataObject19 = this.DataObject;
			complianceJobProvider3.UpdateWorkloadResults(dataObject19.JobRunId, null, ComplianceBindingType.SharePointBinding, ComplianceJobStatus.Succeeded);
			TaskLogger.LogExit();
		}

		protected virtual ComplianceMessage CreateStartJobMessage()
		{
			ComplianceMessage complianceMessage = new ComplianceMessage();
			complianceMessage.ComplianceMessageType = ComplianceMessageType.StartJob;
			ComplianceMessage complianceMessage2 = complianceMessage;
			TDataObject dataObject = this.DataObject;
			complianceMessage2.CorrelationId = dataObject.JobRunId;
			ComplianceMessage complianceMessage3 = complianceMessage;
			TDataObject dataObject2 = this.DataObject;
			complianceMessage3.MessageId = dataObject2.JobRunId.ToString();
			ComplianceMessage complianceMessage4 = complianceMessage;
			TDataObject dataObject3 = this.DataObject;
			complianceMessage4.TenantId = dataObject3.TenantInfo;
			ComplianceMessage complianceMessage5 = complianceMessage;
			Target target = new Target();
			Target target2 = target;
			TDataObject dataObject4 = this.DataObject;
			target2.Identifier = dataObject4.Bindings[ComplianceBindingType.ExchangeBinding].JobMaster;
			target.TargetType = Target.Type.MailboxSmtpAddress;
			complianceMessage5.MessageTarget = target;
			complianceMessage.MessageSource = new Target
			{
				TargetType = Target.Type.Driver
			};
			complianceMessage.MessageSourceId = string.Empty;
			return complianceMessage;
		}

		private void CreateWarningMessageForInvalidBindings(Dictionary<string, ComplianceBindingErrorType> invalidBindings)
		{
			if (invalidBindings.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("\r\n");
				foreach (KeyValuePair<string, ComplianceBindingErrorType> keyValuePair in invalidBindings)
				{
					stringBuilder.Append("\t");
					stringBuilder.Append(keyValuePair.Key);
					stringBuilder.Append(": ");
					switch (keyValuePair.Value)
					{
					case ComplianceBindingErrorType.NoError:
						stringBuilder.Append(Strings.ComplianceBindingNoError(keyValuePair.Key));
						break;
					case ComplianceBindingErrorType.BindingNotFound:
						stringBuilder.Append(Strings.ComplianceBindingNotFound(keyValuePair.Key));
						break;
					case ComplianceBindingErrorType.AmbiguousBinding:
						stringBuilder.Append(Strings.ComplianceBindingAmbiguous(keyValuePair.Key));
						break;
					case ComplianceBindingErrorType.InvalidRecipientType:
						stringBuilder.Append(Strings.ComplianceBindingInvalidRecipientType(keyValuePair.Key));
						break;
					default:
						stringBuilder.Append(Strings.ComplianceBindingUnknownError(keyValuePair.Key));
						break;
					}
					stringBuilder.Append("\r\n");
				}
				this.WriteWarning(Strings.InvalidComplianceBindingWarning(stringBuilder.ToString()));
			}
		}

		private const string ParameterResume = "Resume";
	}
}
