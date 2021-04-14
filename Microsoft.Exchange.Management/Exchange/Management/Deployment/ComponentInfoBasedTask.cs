using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ComponentInfoBasedTask : Task
	{
		protected ComponentInfoBasedTask()
		{
			this.monadConnection = new MonadConnection("pooled=false");
			this.monadConnection.Open();
			if (ExManagementApplicationLogger.IsLowEventCategoryEnabled(4))
			{
				this.IsCmdletLogEntriesEnabled = true;
				base.Fields["CmdletLogEntriesEnabled"] = new bool?(true);
			}
			base.Fields["InstallationMode"] = InstallationModes.Unknown;
			base.Fields["IsDatacenter"] = new SwitchParameter(false);
			base.Fields["IsDatacenterDedicated"] = new SwitchParameter(false);
			base.Fields["IsPartnerHosted"] = new SwitchParameter(false);
			object[] customAttributes = base.GetType().GetCustomAttributes(typeof(CmdletAttribute), false);
			this.taskVerb = ((CmdletAttribute)customAttributes[0]).VerbName;
			this.taskNoun = ((CmdletAttribute)customAttributes[0]).NounName;
			this.implementsResume = true;
			this.isResuming = false;
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new ComponentInfoBaseTaskModuleFactory();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.monadConnection != null)
			{
				this.monadConnection.Close();
			}
			base.Dispose(disposing);
		}

		private bool IsCmdletLogEntriesEnabled { get; set; }

		protected abstract LocalizedString Description { get; }

		protected virtual bool IsInnerRunspaceRBACEnabled
		{
			get
			{
				return false;
			}
		}

		protected virtual bool IsInnerRunspaceThrottlingEnabled
		{
			get
			{
				return false;
			}
		}

		protected virtual ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientApplication
		{
			get
			{
				return ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown;
			}
		}

		protected bool ShouldWriteLogFile
		{
			get
			{
				return this.shouldWriteLogFile;
			}
			set
			{
				this.shouldWriteLogFile = value;
			}
		}

		protected bool IsTenantOrganization
		{
			get
			{
				return this.isTenantOrganization;
			}
			set
			{
				this.isTenantOrganization = value;
			}
		}

		protected bool ShouldLoadDatacenterConfigFile
		{
			get
			{
				return this.shouldLoadDatacenterConfigFile;
			}
			set
			{
				this.shouldLoadDatacenterConfigFile = value;
			}
		}

		protected virtual InstallationModes InstallationMode
		{
			get
			{
				return (InstallationModes)base.Fields["InstallationMode"];
			}
			set
			{
				base.Fields["InstallationMode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDatacenter
		{
			get
			{
				return (SwitchParameter)base.Fields["IsDatacenter"];
			}
			set
			{
				base.Fields["IsDatacenter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDatacenterDedicated
		{
			get
			{
				return (SwitchParameter)base.Fields["IsDatacenterDedicated"];
			}
			set
			{
				base.Fields["IsDatacenterDedicated"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsPartnerHosted
		{
			get
			{
				return (SwitchParameter)base.Fields["IsPartnerHosted"];
			}
			set
			{
				base.Fields["IsPartnerHosted"] = value;
			}
		}

		internal List<string> ComponentInfoFileNames
		{
			get
			{
				return this.componentInfoFileNames;
			}
			set
			{
				this.componentInfoFileNames = value;
			}
		}

		internal SetupComponentInfoCollection ComponentInfoList
		{
			get
			{
				return this.componentInfoList;
			}
			set
			{
				this.componentInfoList = value;
			}
		}

		internal bool ImplementsResume
		{
			get
			{
				return this.implementsResume;
			}
			set
			{
				this.implementsResume = value;
			}
		}

		protected string Platform
		{
			get
			{
				if (IntPtr.Size != 8)
				{
					return "i386";
				}
				return "amd64";
			}
		}

		[Parameter(Mandatory = false)]
		public LongPath UpdatesDir
		{
			get
			{
				return (LongPath)base.Fields["UpdatesDir"];
			}
			set
			{
				base.Fields["UpdatesDir"] = value;
			}
		}

		protected virtual void CheckInstallationMode()
		{
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (ExchangePropertyContainer.IsCmdletLogEnabled(base.SessionState))
			{
				this.IsCmdletLogEntriesEnabled = true;
			}
			this.PropagateExchangePropertyContainer();
		}

		private void PropagateExchangePropertyContainer()
		{
			ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication = this.ClientApplication;
			if (this.IsInnerRunspaceThrottlingEnabled && base.ExchangeRunspaceConfig != null && clientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown)
			{
				clientApplication = base.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication;
			}
			ADServerSettings adServerSettingsOverride = null;
			base.CurrentTaskContext.TryGetItem<ADServerSettings>("CmdletServerSettings", ref adServerSettingsOverride);
			ExchangePropertyContainer.PropagateExchangePropertyContainer(base.SessionState, this.monadConnection.RunspaceProxy, this.IsInnerRunspaceRBACEnabled, this.IsInnerRunspaceThrottlingEnabled, adServerSettingsOverride, clientApplication);
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			if (this.IsCmdletLogEntriesEnabled)
			{
				this.IsCmdletLogEntriesEnabled = false;
			}
		}

		protected override void InternalStopProcessing()
		{
			base.InternalStopProcessing();
			if (this.IsCmdletLogEntriesEnabled)
			{
				this.IsCmdletLogEntriesEnabled = false;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.CheckInstallationMode();
			ConfigurationStatus configurationStatus = new ConfigurationStatus(this.taskNoun);
			RolesUtility.GetConfiguringStatus(ref configurationStatus);
			if (this.ImplementsResume && configurationStatus.Action != InstallationModes.Unknown && configurationStatus.Watermark != null)
			{
				this.isResuming = true;
				if (configurationStatus.Action != this.InstallationMode && (configurationStatus.Action != InstallationModes.Install || this.InstallationMode != InstallationModes.Uninstall))
				{
					base.WriteError(new IllegalResumptionException(configurationStatus.Action.ToString(), this.InstallationMode.ToString()), ErrorCategory.InvalidOperation, null);
				}
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.ComponentInfoFileNames == null || this.ComponentInfoFileNames.Count == 0)
				{
					throw new NoComponentInfoFilesException();
				}
				this.ComponentInfoList = new SetupComponentInfoCollection();
				try
				{
					foreach (string path in this.ComponentInfoFileNames)
					{
						string fileName = Path.Combine(Role.SetupComponentInfoFilePath, path);
						this.ComponentInfoList.Add(RolesUtility.ReadSetupComponentInfoFile(fileName));
					}
				}
				catch (FileNotFoundException exception)
				{
					base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
				}
				catch (XmlDeserializationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidData, null);
				}
				this.GenerateAndExecuteTaskScript(this.IsTenantOrganization ? InstallationCircumstances.TenantOrganization : InstallationCircumstances.Standalone);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected virtual void PopulateContextVariables()
		{
			DateTime dateTime = (DateTime)ExDateTime.Now;
			base.Fields["InvocationID"] = string.Format("{0}{1:0000}{2}", dateTime.ToString("yyyyMMdd-HHmmss"), dateTime.Millisecond, ComponentInfoBasedTask.random.Next());
			base.Fields["ProductPlatform"] = ((IntPtr.Size == 8) ? "amd64" : "i386");
			if (this.ShouldLoadDatacenterConfigFile && this.InstallationMode != InstallationModes.Uninstall)
			{
				ParameterCollection parameterCollection = RolesUtility.ReadSetupParameters(this.IsDatacenter || this.IsDatacenterDedicated);
				foreach (Parameter parameter in parameterCollection)
				{
					base.Fields["Datacenter" + parameter.Name] = parameter.EffectiveValue;
				}
			}
		}

		protected virtual void SetRunspaceVariables()
		{
		}

		protected virtual bool ShouldExecuteComponentTasks()
		{
			return base.ShouldProcess(this.taskNoun, this.taskVerb, null);
		}

		internal bool GenerateAndExecuteTaskScript(InstallationCircumstances installationCircumstance)
		{
			this.completedSteps = 0;
			bool flag = false;
			ConfigurationStatus configurationStatus = new ConfigurationStatus(this.taskNoun, this.InstallationMode);
			string text = string.Format("{0}-{1}", this.taskVerb, this.taskNoun);
			TaskLogger.LogEnter();
			bool flag2 = this.ShouldExecuteComponentTasks();
			StringBuilder stringBuilder = new StringBuilder();
			List<TaskInfo>.Enumerator enumerator = default(List<TaskInfo>.Enumerator);
			this.PopulateContextVariables();
			try
			{
				string path = string.Format("{0}-{1}.ps1", text, base.Fields["InvocationID"]);
				string text2 = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, path);
				base.WriteVerbose(Strings.WritingInformationScript(text2));
				if (this.shouldWriteLogFile)
				{
					this.logFileStream = new StreamWriter(text2);
					this.logFileStream.AutoFlush = true;
				}
				this.WriteLogFile(Strings.SetupLogHeader(this.taskNoun, this.taskVerb, (DateTime)ExDateTime.Now));
				this.WriteLogFile(Strings.VariablesSection);
				if (base.ServerSettings != null)
				{
					this.monadConnection.RunspaceProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, base.ServerSettings);
				}
				this.SetRunspaceVariables();
				SortedList<string, object> sortedList = new SortedList<string, object>();
				foreach (object obj in base.Fields)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					sortedList.Add((string)dictionaryEntry.Key, dictionaryEntry.Value);
				}
				foreach (KeyValuePair<string, object> keyValuePair in sortedList)
				{
					string text3 = this.GenerateScriptVarCommand(keyValuePair.Key, keyValuePair.Value);
					this.WriteLogFile(text3);
					if (flag2)
					{
						this.ExecuteScript(text3, false, 0, LocalizedString.Empty);
					}
				}
				this.FilterComponents();
				if (this.InstallationMode == InstallationModes.Uninstall)
				{
					base.WriteVerbose(Strings.ReversingTaskList);
					this.ComponentInfoList.Reverse();
					foreach (SetupComponentInfo setupComponentInfo in this.ComponentInfoList)
					{
						setupComponentInfo.Tasks.Reverse();
					}
				}
				List<SetupComponentInfo>.Enumerator enumerator5 = this.ComponentInfoList.GetEnumerator();
				bool flag3 = false;
				this.FindStartingTask(ref enumerator5, ref enumerator, ref flag3, this.InstallationMode, installationCircumstance);
				using (enumerator5)
				{
					bool flag4 = true;
					bool flag5 = true;
					this.WriteLogFile(Strings.ComponentTaskSection);
					this.totalSteps = this.CountStepsToBeExecuted(this.ComponentInfoList, this.InstallationMode, installationCircumstance);
					while (flag4)
					{
						SetupComponentInfo setupComponentInfo2 = enumerator5.Current;
						string name = setupComponentInfo2.Name;
						LocalizedString localizedString;
						if (string.IsNullOrEmpty(setupComponentInfo2.DescriptionId))
						{
							localizedString = Strings.SetupProgressGenericComponent;
						}
						else
						{
							try
							{
								Strings.IDs key = (Strings.IDs)Enum.Parse(typeof(Strings.IDs), setupComponentInfo2.DescriptionId, false);
								localizedString = Strings.GetLocalizedString(key);
							}
							catch (ArgumentException)
							{
								localizedString = Strings.SetupProgressGenericComponent;
							}
						}
						base.WriteVerbose(Strings.ProcessingComponent(name, localizedString));
						this.WriteLogFile(Strings.ComponentSection(name));
						while (flag5)
						{
							TaskInfo taskInfo = enumerator.Current;
							string task = taskInfo.GetTask(this.InstallationMode, installationCircumstance);
							if (string.IsNullOrEmpty(task))
							{
								flag5 = enumerator.MoveNext();
							}
							else if (!this.IsTaskIncluded(taskInfo, enumerator5.Current))
							{
								flag5 = enumerator.MoveNext();
							}
							else
							{
								string text4 = task;
								string id = taskInfo.GetID();
								int weight = taskInfo.GetWeight(this.InstallationMode);
								bool flag6 = !flag3 && taskInfo.IsFatal(this.InstallationMode);
								flag3 = false;
								string description = taskInfo.GetDescription(this.InstallationMode);
								LocalizedString localizedString2;
								if (string.IsNullOrEmpty(description))
								{
									localizedString2 = localizedString;
								}
								else
								{
									try
									{
										Strings.IDs key2 = (Strings.IDs)Enum.Parse(typeof(Strings.IDs), description, false);
										localizedString2 = Strings.GetLocalizedString(key2);
									}
									catch (ArgumentException)
									{
										localizedString2 = localizedString;
									}
								}
								this.WriteLogFile(string.Format("# [ID = {0:x}, Wt = {1}, isFatal = {2}] \"{3}\"", new object[]
								{
									taskInfo.GetID(),
									weight,
									flag6,
									localizedString2
								}));
								this.WriteLogFile(ExDateTime.Now + ":" + text4);
								if (flag2)
								{
									configurationStatus.Watermark = id;
									if (this.ImplementsResume)
									{
										RolesUtility.SetConfiguringStatus(configurationStatus);
									}
									if (!text4.Contains("\n"))
									{
										text4 = "\n\t" + text4 + "\n\n";
									}
									ExDateTime now = ExDateTime.Now;
									bool flag7 = this.ExecuteScript(text4, !flag6, weight, localizedString2);
									TimeSpan timeSpan = ExDateTime.Now - now;
									if (ComponentInfoBasedTask.monitoredCmdlets.Contains(text.ToLowerInvariant()) && timeSpan.CompareTo(this.executionTimeThreshold) > 0)
									{
										if (taskInfo is ServicePlanTaskInfo)
										{
											ServicePlanTaskInfo servicePlanTaskInfo = (ServicePlanTaskInfo)taskInfo;
											stringBuilder.AppendLine(string.Format("Task {0}__{1} had execution time: {2}.", servicePlanTaskInfo.FileId, servicePlanTaskInfo.FeatureName, timeSpan.ToString()));
										}
										else
										{
											stringBuilder.AppendLine(string.Format("Task {0} had execution time: {1}.", taskInfo.GetID(), timeSpan.ToString()));
										}
									}
									flag = (!flag7 && flag6);
									if (flag)
									{
										base.WriteVerbose(new LocalizedString(string.Format("[ERROR-REFERENCE] Id={0} Component={1}", taskInfo.GetID(), taskInfo.Component)));
										base.WriteVerbose(Strings.HaltingExecution);
										break;
									}
								}
								flag5 = enumerator.MoveNext();
							}
						}
						if (flag)
						{
							break;
						}
						flag4 = enumerator5.MoveNext();
						if (flag4)
						{
							enumerator.Dispose();
							enumerator = enumerator5.Current.Tasks.GetEnumerator();
							flag5 = enumerator.MoveNext();
						}
					}
					base.WriteVerbose(Strings.FinishedComponentTasks);
				}
			}
			catch (Exception ex)
			{
				base.WriteVerbose(Strings.ExceptionOccured(ex.ToString()));
				flag = true;
				this.OnHalt(ex);
				throw;
			}
			finally
			{
				if (!string.IsNullOrEmpty(stringBuilder.ToString()) && ComponentInfoBasedTask.monitoredCmdlets.Contains(text.ToLowerInvariant()))
				{
					if (base.Fields["TenantName"] != null)
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ExecuteTaskScriptOptic, new string[]
						{
							text,
							base.Fields["TenantName"].ToString(),
							stringBuilder.ToString()
						});
					}
					else if (base.Fields["OrganizationHierarchicalPath"] != null)
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ExecuteTaskScriptOptic, new string[]
						{
							text,
							base.Fields["OrganizationHierarchicalPath"].ToString(),
							stringBuilder.ToString()
						});
					}
					else
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ExecuteTaskScriptOptic, new string[]
						{
							text,
							string.Empty,
							stringBuilder.ToString()
						});
					}
				}
				if (flag)
				{
					if (this.IsCmdletLogEntriesEnabled)
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ComponentTaskFailed, this.GetComponentEventParameters(this.GetVerboseInformation(this.GetCmdletLogEntries())));
					}
					base.WriteProgress(this.Description, Strings.ProgressStatusFailed, 100);
				}
				else
				{
					if (this.IsCmdletLogEntriesEnabled)
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ComponentTaskExecutedSuccessfully, this.GetComponentEventParameters(this.GetVerboseInformation(this.GetCmdletLogEntries())));
					}
					base.WriteProgress(this.Description, Strings.ProgressStatusCompleted, 100);
					if (flag2)
					{
						RolesUtility.ClearConfiguringStatus(configurationStatus);
						if (text == "Start-PostSetup")
						{
							foreach (string roleName in base.Fields["Roles"].ToString().Split(new char[]
							{
								','
							}))
							{
								RolesUtility.SetPostSetupVersion(roleName, (Version)base.Fields["TargetVersion"]);
							}
							RolesUtility.SetPostSetupVersion("AdminTools", (Version)base.Fields["TargetVersion"]);
						}
					}
				}
				if (this.logFileStream != null)
				{
					this.logFileStream.Close();
					this.logFileStream = null;
				}
				enumerator.Dispose();
				TaskLogger.LogExit();
			}
			return !flag;
		}

		protected virtual void OnHalt(Exception e)
		{
		}

		protected virtual void FilterComponents()
		{
			this.ComponentInfoList.RemoveAll(delegate(SetupComponentInfo component)
			{
				if (!component.IsDatacenterOnly && !component.IsPartnerHostedOnly && !component.IsDatacenterDedicatedOnly)
				{
					return false;
				}
				if (this.IsDatacenter)
				{
					return !component.IsDatacenterOnly;
				}
				if (this.IsDatacenterDedicated)
				{
					return !component.IsDatacenterDedicatedOnly;
				}
				return !this.IsPartnerHosted || !component.IsPartnerHostedOnly;
			});
		}

		private void WriteLogFile(string line)
		{
			if (this.shouldWriteLogFile)
			{
				this.logFileStream.WriteLine(line);
			}
		}

		private string GenerateScriptVarCommand(object varName, object varValue)
		{
			string result = string.Empty;
			if (varValue == null)
			{
				result = string.Format("$Role{0} = $null", varName);
			}
			else if (varValue is bool || varValue is SwitchParameter)
			{
				result = string.Format("$Role{0} = ${1}", varName, varValue);
			}
			else if (varValue is string[])
			{
				result = string.Format("$Role{0} = {1}", varName, Globals.PowerShellArrayFromStringArray((string[])varValue));
			}
			else if (varValue is MultiValuedProperty<string>)
			{
				result = string.Format("$Role{0} = {1}", varName, Globals.PowerShellArrayFromStringArray(((MultiValuedProperty<string>)varValue).ToArray()));
			}
			else if (varValue is MultiValuedProperty<Capability>)
			{
				MultiValuedProperty<Capability> multiValuedProperty = varValue as MultiValuedProperty<Capability>;
				MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>();
				foreach (Capability capability in multiValuedProperty)
				{
					multiValuedProperty2.Add(capability.ToString());
				}
				result = string.Format("$Role{0} = {1}", varName, Globals.PowerShellArrayFromStringArray(multiValuedProperty2.ToArray()));
			}
			else if (varValue is Version)
			{
				Version version = (Version)varValue;
				result = string.Format("$Role{0} = '{1}.{2:D2}.{3:D4}.{4:D3}'", new object[]
				{
					varName,
					version.Major,
					version.Minor,
					version.Build,
					version.Revision
				});
			}
			else
			{
				result = string.Format("$Role{0} = '{1}'", varName, varValue.ToString().Replace("'", "''"));
			}
			return result;
		}

		private void FindStartingTask(ref List<SetupComponentInfo>.Enumerator componentEnumerator, ref List<TaskInfo>.Enumerator taskEnumerator, ref bool nextTaskNonFatal, InstallationModes installationMode, InstallationCircumstances installationCircumstance)
		{
			bool flag = false;
			if (this.ImplementsResume && this.isResuming && installationMode != InstallationModes.BuildToBuildUpgrade)
			{
				ConfigurationStatus configurationStatus = new ConfigurationStatus(this.taskNoun, this.InstallationMode);
				RolesUtility.GetConfiguringStatus(ref configurationStatus);
				base.WriteVerbose(Strings.LookingForTask(configurationStatus.Action.ToString(), configurationStatus.Watermark));
				while (!flag && componentEnumerator.MoveNext())
				{
					taskEnumerator = componentEnumerator.Current.Tasks.GetEnumerator();
					while (!flag && taskEnumerator.MoveNext())
					{
						if (taskEnumerator.Current.GetID() == configurationStatus.Watermark)
						{
							flag = true;
							if (this.InstallationMode == InstallationModes.Uninstall && configurationStatus.Action == InstallationModes.Install)
							{
								nextTaskNonFatal = true;
							}
						}
						else if (!string.IsNullOrEmpty(taskEnumerator.Current.GetTask(installationMode, installationCircumstance)))
						{
							this.completedSteps += taskEnumerator.Current.GetWeight(installationMode);
						}
					}
				}
				if (!flag)
				{
					base.WriteVerbose(Strings.CouldNotFindTask);
					this.completedSteps = 0;
				}
			}
			if (!flag)
			{
				componentEnumerator = this.ComponentInfoList.GetEnumerator();
				while (componentEnumerator.MoveNext())
				{
					SetupComponentInfo setupComponentInfo = componentEnumerator.Current;
					taskEnumerator = setupComponentInfo.Tasks.GetEnumerator();
					if (taskEnumerator.MoveNext())
					{
						flag = true;
						break;
					}
					base.WriteVerbose(Strings.ComponentEmpty(componentEnumerator.Current.Name));
				}
				if (!flag)
				{
					throw new EmptyTaskListException();
				}
			}
		}

		protected bool ExecuteScript(string script, bool handleError, int subSteps, LocalizedString statusDescription)
		{
			bool flag = false;
			if (this.IsCmdletLogEntriesEnabled)
			{
				this.GetCmdletLogEntries().IncreaseIndentation();
			}
			try
			{
				flag = this.InternalExecuteScript(script, handleError, subSteps, statusDescription);
			}
			finally
			{
				if (this.IsCmdletLogEntriesEnabled)
				{
					if (!flag)
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ScriptExecutionFailed, this.GetScriptEventParameters(this.GetVerboseInformation(this.GetCmdletLogEntries())));
					}
					else
					{
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ScriptExecutionSuccessfully, this.GetScriptEventParameters(this.GetVerboseInformation(this.GetCmdletLogEntries())));
					}
					this.GetCmdletLogEntries().DecreaseIndentation();
				}
			}
			return flag;
		}

		private bool InternalExecuteScript(string script, bool handleError, int subSteps, LocalizedString statusDescription)
		{
			bool result = false;
			WorkUnit workUnit = new WorkUnit();
			bool newSubProgressReceived = false;
			int completedSubSteps = 0;
			try
			{
				script.TrimEnd(new char[]
				{
					'\n'
				});
				string script2 = script.Replace("\n", "\r\n");
				if (handleError)
				{
					base.WriteVerbose(Strings.ExecutingScriptNonFatal(script2));
				}
				else
				{
					base.WriteVerbose(Strings.ExecutingScript(script2));
				}
				script = string.Format("$error.Clear(); {0}", script);
				MonadCommand monadCommand = new MonadCommand(script, this.monadConnection);
				monadCommand.CommandType = CommandType.Text;
				monadCommand.ProgressReport += delegate(object sender, ProgressReportEventArgs e)
				{
					if (subSteps == 0)
					{
						return;
					}
					completedSubSteps = subSteps * e.ProgressRecord.PercentComplete / 100;
					newSubProgressReceived = true;
				};
				bool flag = false;
				try
				{
					TaskLogger.IncreaseIndentation();
					TaskLogger.LogErrorAsWarning = handleError;
					MonadAsyncResult monadAsyncResult = monadCommand.BeginExecute(new WorkUnit[]
					{
						workUnit
					});
					while (!flag)
					{
						flag = monadAsyncResult.AsyncWaitHandle.WaitOne(200, false);
						if (newSubProgressReceived)
						{
							base.WriteProgress(this.Description, statusDescription, (this.completedSteps + completedSubSteps) * 100 / this.totalSteps);
							newSubProgressReceived = false;
						}
						if (base.Stopping)
						{
							break;
						}
					}
					if (base.Stopping)
					{
						monadCommand.Cancel();
					}
					else
					{
						monadCommand.EndExecute(monadAsyncResult);
					}
				}
				catch (CommandExecutionException ex)
				{
					if (ex.InnerException != null)
					{
						throw new ScriptExecutionException(Strings.ErrorCommandExecutionException(script, ex.InnerException.ToString()), ex.InnerException);
					}
					throw;
				}
				finally
				{
					TaskLogger.DecreaseIndentation();
				}
				this.completedSteps += subSteps;
				result = true;
			}
			catch (CmdletInvocationException ex2)
			{
				result = false;
				if (!handleError)
				{
					throw;
				}
				base.WriteVerbose(Strings.IgnoringException(ex2.ToString()));
				base.WriteVerbose(Strings.WillContinueProcessing);
			}
			if (workUnit.Errors.Count > 0)
			{
				result = false;
				int count = workUnit.Errors.Count;
				base.WriteVerbose(Strings.ErrorDuringTaskExecution(count));
				for (int i = 0; i < count; i++)
				{
					ErrorRecord errorRecord = workUnit.Errors[i];
					base.WriteVerbose(Strings.ErrorRecordReport(errorRecord.ToString(), i));
					if (!handleError)
					{
						base.WriteVerbose(Strings.ErrorRecordReport(errorRecord.Exception.ToString(), i));
						ScriptExecutionException exception = new ScriptExecutionException(Strings.ErrorCommandExecutionException(script, errorRecord.Exception.ToString()), errorRecord.Exception);
						this.WriteError(exception, errorRecord.CategoryInfo.Category, errorRecord.TargetObject, false);
					}
				}
				if (handleError)
				{
					base.WriteVerbose(Strings.WillIgnoreNoncriticalErrors);
					base.WriteVerbose(Strings.WillContinueProcessing);
				}
			}
			return result;
		}

		private static string GetValueString(object value)
		{
			if (value == null)
			{
				return "$null";
			}
			ICollection collection = value as ICollection;
			if (collection == null)
			{
				return value.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value.ToString() + " {");
			foreach (object value2 in collection)
			{
				stringBuilder.Append(ComponentInfoBasedTask.GetValueString(value2));
				stringBuilder.Append("; ");
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		private int CountStepsToBeExecuted(SetupComponentInfoCollection componentList, InstallationModes installationMode, InstallationCircumstances installationCircumstance)
		{
			int num = 0;
			int num2 = 0;
			foreach (SetupComponentInfo setupComponentInfo in componentList)
			{
				foreach (TaskInfo taskInfo in setupComponentInfo.Tasks)
				{
					if (!string.IsNullOrEmpty(taskInfo.GetTask(installationMode, installationCircumstance)) && this.IsTaskIncluded(taskInfo, setupComponentInfo))
					{
						num2++;
						num += taskInfo.GetWeight(installationMode);
					}
				}
			}
			base.WriteVerbose(Strings.FoundTasksToExecute(num2));
			return num;
		}

		private bool IsTaskIncluded(TaskInfo task, SetupComponentInfo parentComponent)
		{
			return !this.IsDatacenterDedicated || !parentComponent.IsDatacenterDedicatedOnly || !task.ExcludeInDatacenterDedicated;
		}

		protected string GetFqdnOrNetbiosName()
		{
			string result;
			try
			{
				result = Dns.GetHostEntry(Dns.GetHostName()).HostName;
			}
			catch (SocketException)
			{
				result = Environment.MachineName;
			}
			return result;
		}

		protected string GetNetBIOSName(string name)
		{
			string text = null;
			int num = name.IndexOf('.');
			if (num != -1)
			{
				try
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1551, "GetNetBIOSName", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\ComponentInfoBasedTask.cs");
					topologyConfigurationSession.UseConfigNC = false;
					topologyConfigurationSession.UseGlobalCatalog = true;
					ADComputer adcomputer = topologyConfigurationSession.FindComputerByHostName(name);
					if (adcomputer != null)
					{
						text = adcomputer.Name;
					}
				}
				catch (DataSourceOperationException)
				{
				}
				catch (DataSourceTransientException)
				{
				}
				catch (DataValidationException)
				{
				}
				if (text == null)
				{
					text = name.Substring(0, num);
				}
			}
			else
			{
				text = name;
			}
			return text;
		}

		internal CmdletLogEntries GetCmdletLogEntries()
		{
			return ExchangePropertyContainer.GetCmdletLogEntries(base.SessionState);
		}

		private string[] GetScriptEventParameters(string verboseInfo)
		{
			return new string[]
			{
				base.ProcessId.ToString(),
				Thread.CurrentThread.ManagedThreadId.ToString(),
				verboseInfo
			};
		}

		private string[] GetComponentEventParameters(string verboseInfo)
		{
			return new string[]
			{
				base.ProcessId.ToString(),
				Thread.CurrentThread.ManagedThreadId.ToString(),
				(base.MyInvocation.MyCommand != null) ? base.MyInvocation.MyCommand.Name : base.MyInvocation.InvocationName,
				verboseInfo
			};
		}

		private string GetVerboseInformation(CmdletLogEntries logEntries)
		{
			StringBuilder stringBuilder = new StringBuilder(31000);
			int num = 0;
			foreach (string text in logEntries.GetCurrentIndentationEntries())
			{
				if (num + (text.Length + 1) > 31000)
				{
					break;
				}
				num += text.Length + 1;
				stringBuilder.Append("\n");
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		private const int MaxLogEntryStringSize = 31000;

		internal MonadConnection monadConnection;

		private int completedSteps;

		private int totalSteps;

		private static Random random = new Random();

		private bool shouldWriteLogFile = true;

		private StreamWriter logFileStream;

		private bool shouldLoadDatacenterConfigFile = true;

		private readonly TimeSpan executionTimeThreshold = new TimeSpan(0, 0, 5);

		private static readonly List<string> monitoredCmdlets = new List<string>
		{
			"new-organization",
			"remove-organization"
		};

		private bool isTenantOrganization;

		private List<string> componentInfoFileNames;

		private SetupComponentInfoCollection componentInfoList;

		private readonly string taskVerb;

		private readonly string taskNoun;

		private bool implementsResume;

		private bool isResuming;
	}
}
