using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Set", "ManagementRoleEntry", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetManagementRoleEntry : ManagementRoleEntryActionBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetManagementRoleEntry(this.modifiedEntry.ToString(), this.DataObject.Id.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UnScopedTopLevel
		{
			get
			{
				return (SwitchParameter)(base.Fields["UnScopedTopLevel"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UnScopedTopLevel"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Parameters
		{
			get
			{
				return (string[])base.Fields[RbacCommonParameters.ParameterParameters];
			}
			set
			{
				RoleEntry.FormatParameters(value);
				base.Fields[RbacCommonParameters.ParameterParameters] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AddParameter
		{
			get
			{
				return (SwitchParameter)(base.Fields["AddParameter"] ?? false);
			}
			set
			{
				base.Fields["AddParameter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoveParameter
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveParameter"] ?? false);
			}
			set
			{
				base.Fields["RemoveParameter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter SkipScriptExistenceCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipScriptExistenceCheck"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipScriptExistenceCheck"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (!base.Fields.IsModified(RbacCommonParameters.ParameterParameters))
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorParametersMandatory), ErrorCategory.InvalidArgument, null);
			}
			if (this.AddParameter.IsPresent && this.RemoveParameter.IsPresent)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorCannotTurnOnBothAndAndRemove), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalApplyChangeAndValidate()
		{
			TaskLogger.LogEnter();
			if (!this.DataObject.IsUnscopedTopLevel && this.UnScopedTopLevel)
			{
				base.WriteError(new InvalidOperationException(Strings.ParameterAllowedOnlyForTopLevelRoleManipulation("UnScopedTopLevel", RoleType.UnScoped.ToString())), ErrorCategory.InvalidOperation, null);
			}
			string[] parameters = null;
			this.originalEntry = RoleHelper.GetMandatoryRoleEntry(this.DataObject, this.Identity.CmdletOrScriptName, this.Identity.PSSnapinName, new Task.TaskErrorLoggingDelegate(base.WriteError));
			string[] parameters2;
			if (!this.AddParameter.IsPresent && !this.RemoveParameter.IsPresent)
			{
				parameters2 = this.Parameters;
				parameters = this.Parameters;
			}
			else
			{
				MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
				foreach (string item in this.originalEntry.Parameters)
				{
					multiValuedProperty.Add(item);
				}
				if (this.Parameters != null)
				{
					List<string> list = null;
					if (this.AddParameter.IsPresent)
					{
						parameters = this.Parameters;
						foreach (string item2 in this.Parameters)
						{
							if (multiValuedProperty.Contains(item2))
							{
								if (list == null)
								{
									list = new List<string>();
								}
								list.Add(item2);
							}
							else
							{
								multiValuedProperty.Add(item2);
							}
						}
					}
					else
					{
						foreach (string item3 in this.Parameters)
						{
							if (!multiValuedProperty.Contains(item3))
							{
								if (list == null)
								{
									list = new List<string>();
								}
								list.Add(item3);
							}
							else
							{
								multiValuedProperty.Remove(item3);
							}
						}
					}
					if (list != null && 0 < list.Count)
					{
						LocalizedString value = this.AddParameter.IsPresent ? Strings.ErrorAddExistentParameters(this.Identity.RoleId.ToString(), this.Identity.CmdletOrScriptName, string.Join(",", list.ToArray())) : Strings.ErrorRemoveNonExistentParameters(this.Identity.RoleId.ToString(), this.Identity.CmdletOrScriptName, string.Join(",", list.ToArray()));
						base.WriteError(new InvalidOperationException(value), ErrorCategory.InvalidOperation, this.DataObject.Id);
					}
				}
				parameters2 = multiValuedProperty.ToArray();
			}
			try
			{
				if (this.originalEntry is CmdletRoleEntry)
				{
					string pssnapinName = ((CmdletRoleEntry)this.originalEntry).PSSnapinName;
					if (base.IsEntryValidationRequired())
					{
						base.VerifyCmdletEntry(this.originalEntry.Name, pssnapinName, parameters);
					}
					this.modifiedEntry = new CmdletRoleEntry(this.originalEntry.Name, pssnapinName, parameters2);
				}
				else if (this.originalEntry is ScriptRoleEntry)
				{
					if (base.IsEntryValidationRequired())
					{
						base.VerifyScriptEntry(this.originalEntry.Name, parameters, this.SkipScriptExistenceCheck);
					}
					this.modifiedEntry = new ScriptRoleEntry(this.originalEntry.Name, parameters2);
				}
				else if (this.originalEntry is ApplicationPermissionRoleEntry)
				{
					this.modifiedEntry = new ApplicationPermissionRoleEntry(this.originalEntry.Name, parameters2);
				}
				else
				{
					if (!(this.originalEntry is WebServiceRoleEntry))
					{
						throw new NotSupportedException("this.originalEntry is not a valid type.");
					}
					base.WriteError(new InvalidOperationException(Strings.CannotSetWebServiceRoleEntry(this.originalEntry.Name)), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (FormatException ex)
			{
				base.WriteError(new ArgumentException(new LocalizedString(ex.Message)), ErrorCategory.InvalidArgument, this.DataObject.Id);
			}
			if (!this.modifiedEntry.Equals(this.originalEntry))
			{
				if (null != this.originalEntry)
				{
					this.DataObject.RoleEntries.Remove(this.originalEntry);
				}
				this.DataObject.RoleEntries.Add(this.modifiedEntry);
				this.DataObject.ApplyChangesToDownlevelData(base.ParentRole ?? this.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override bool IsTopLevelUnscopedRoleModificationAllowed()
		{
			return this.UnScopedTopLevel;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!base.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private const string ParameterAddParameter = "AddParameter";

		private const string ParameterRemoveParameter = "RemoveParameter";

		private RoleEntry modifiedEntry;

		private RoleEntry originalEntry;
	}
}
