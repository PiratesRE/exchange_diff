using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Add", "ManagementRoleEntry", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class AddManagementRoleEntry : AddRemoveManagementRoleEntryActionBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewManagementRoleEntry(this.addedEntry.ToString(), this.DataObject.Id.ToString());
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "ParentRoleEntry")]
		public RoleEntryIdParameter ParentRoleEntry
		{
			get
			{
				return (RoleEntryIdParameter)base.Fields["ParentRoleEntry"];
			}
			set
			{
				base.Fields["ParentRoleEntry"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParentRoleEntry")]
		public RoleIdParameter Role
		{
			get
			{
				return (RoleIdParameter)base.Fields[RbacCommonParameters.ParameterRole];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterRole] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string PSSnapinName
		{
			get
			{
				return (string)base.Fields[RbacCommonParameters.ParameterPSSnapinName];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterPSSnapinName] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ManagementRoleEntryType Type
		{
			get
			{
				return (ManagementRoleEntryType)base.Fields[RbacCommonParameters.ParameterType];
			}
			set
			{
				base.VerifyValues<ManagementRoleEntryType>(AddManagementRoleEntry.AllowedRoleEntryTypes, value);
				base.Fields[RbacCommonParameters.ParameterType] = value;
			}
		}

		[Parameter]
		public SwitchParameter Overwrite
		{
			get
			{
				return (SwitchParameter)(base.Fields["Overwrite"] ?? false);
			}
			set
			{
				base.Fields["Overwrite"] = value;
			}
		}

		protected override bool IsTopLevelUnscopedRoleModificationAllowed()
		{
			return this.UnScopedTopLevel;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (base.ParameterSetName == "Identity")
			{
				this.Identity.Parameters = this.Parameters;
				this.Identity.PSSnapinName = this.PSSnapinName;
				if (base.Fields.IsModified(RbacCommonParameters.ParameterType))
				{
					this.Identity.Type = this.Type;
				}
			}
			else
			{
				ExchangeRoleEntryPresentation exchangeRoleEntryPresentation = (ExchangeRoleEntryPresentation)base.GetDataObject<ExchangeRoleEntryPresentation>(this.ParentRoleEntry, base.DataSession, null, new LocalizedString?(Strings.ErrorRoleEntryNotFound(this.ParentRoleEntry.RoleId.ToString(), this.ParentRoleEntry.CmdletOrScriptName)), new LocalizedString?(Strings.ErrorRoleEntryNotUnique(this.ParentRoleEntry.RoleId.ToString(), this.ParentRoleEntry.CmdletOrScriptName)));
				this.Identity = new RoleEntryIdParameter(this.Role, exchangeRoleEntryPresentation.Name, exchangeRoleEntryPresentation.Type);
				string[] array = new string[exchangeRoleEntryPresentation.Parameters.Count];
				exchangeRoleEntryPresentation.Parameters.CopyTo(array, 0);
				this.Parameters = array;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalApplyChangeAndValidate()
		{
			if (this.DataObject.IsUnscopedTopLevel)
			{
				if (this.ParentRoleEntry != null)
				{
					base.WriteError(new InvalidOperationException(Strings.ParameterNotAllowedWithTopLevelRole("ParentRoleEntry", RoleType.UnScoped.ToString())), ErrorCategory.InvalidArgument, this.DataObject.Id);
				}
				if (this.Role != null)
				{
					base.WriteError(new InvalidOperationException(Strings.ParameterNotAllowedWithTopLevelRole("Role", RoleType.UnScoped.ToString())), ErrorCategory.InvalidArgument, this.DataObject.Id);
				}
				ManagementRoleEntryType managementRoleEntryType = ManagementRoleEntryType.Cmdlet;
				if (base.Fields.Contains(RbacCommonParameters.ParameterType))
				{
					if (this.Type == ManagementRoleEntryType.ApplicationPermission || this.Type == ManagementRoleEntryType.All)
					{
						base.WriteError(new InvalidOperationException(Strings.EntryNoAllowedInRoleType(this.Type.ToString(), RoleType.UnScoped.ToString())), ErrorCategory.InvalidArgument, this.DataObject.Id);
					}
					managementRoleEntryType = this.Type;
				}
				else if (Regex.IsMatch(this.Identity.CmdletOrScriptName, "ps\\d?.$", RegexOptions.IgnoreCase))
				{
					managementRoleEntryType = ManagementRoleEntryType.Script;
				}
				if (managementRoleEntryType == ManagementRoleEntryType.Cmdlet && string.IsNullOrEmpty(this.PSSnapinName))
				{
					base.WriteError(new InvalidOperationException(string.Format(Strings.ProvideSnapinNameForCmdletEntryForRole(RoleType.UnScoped.ToString()), new object[0])), ErrorCategory.InvalidArgument, this.DataObject.Id);
				}
				ManagementRoleEntryType managementRoleEntryType2 = managementRoleEntryType;
				switch (managementRoleEntryType2)
				{
				case ManagementRoleEntryType.Cmdlet:
					base.VerifyCmdletEntry(this.Identity.CmdletOrScriptName, this.PSSnapinName, this.Parameters);
					this.addedEntry = new CmdletRoleEntry(this.Identity.CmdletOrScriptName, this.PSSnapinName, this.Parameters);
					break;
				case ManagementRoleEntryType.Script:
					base.VerifyScriptEntry(this.Identity.CmdletOrScriptName, this.Parameters, this.SkipScriptExistenceCheck);
					this.addedEntry = new ScriptRoleEntry(this.Identity.CmdletOrScriptName, this.Parameters);
					break;
				default:
					if (managementRoleEntryType2 == ManagementRoleEntryType.WebService)
					{
						this.VerifyWebServiceEntry(this.Identity.CmdletOrScriptName);
						this.addedEntry = new WebServiceRoleEntry(this.Identity.CmdletOrScriptName, new string[0]);
					}
					break;
				}
				this.roleEntryOnDataObject = RoleHelper.GetRoleEntry(this.DataObject, this.Identity.CmdletOrScriptName, this.Identity.PSSnapinName, managementRoleEntryType, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else
			{
				if (this.UnScopedTopLevel)
				{
					base.WriteError(new InvalidOperationException(Strings.ParameterAllowedOnlyForTopLevelRoleManipulation("UnScopedTopLevel", RoleType.UnScoped.ToString())), ErrorCategory.InvalidOperation, null);
				}
				RoleEntryIdParameter roleEntryIdParameter = new RoleEntryIdParameter(base.ParentRole.Id, this.Identity.CmdletOrScriptName, this.Identity.Type);
				roleEntryIdParameter.PSSnapinName = this.PSSnapinName;
				ExchangeRoleEntryPresentation exchangeRoleEntryPresentation = (ExchangeRoleEntryPresentation)base.GetDataObject<ExchangeRoleEntryPresentation>(roleEntryIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRoleEntryNotFound(roleEntryIdParameter.RoleId.ToString(), roleEntryIdParameter.CmdletOrScriptName)), new LocalizedString?(Strings.ErrorRoleEntryNotUnique(roleEntryIdParameter.RoleId.ToString(), roleEntryIdParameter.CmdletOrScriptName)));
				string[] array;
				if (base.Fields.IsModified(RbacCommonParameters.ParameterParameters))
				{
					array = this.Parameters;
				}
				else
				{
					array = new string[exchangeRoleEntryPresentation.Parameters.Count];
					exchangeRoleEntryPresentation.Parameters.CopyTo(array, 0);
				}
				try
				{
					ManagementRoleEntryType type = exchangeRoleEntryPresentation.Type;
					switch (type)
					{
					case ManagementRoleEntryType.Cmdlet:
						if (base.IsEntryValidationRequired())
						{
							base.VerifyCmdletEntry(exchangeRoleEntryPresentation.Name, exchangeRoleEntryPresentation.PSSnapinName, array);
						}
						this.addedEntry = new CmdletRoleEntry(exchangeRoleEntryPresentation.Name, exchangeRoleEntryPresentation.PSSnapinName, array);
						break;
					case ManagementRoleEntryType.Script:
						if (base.IsEntryValidationRequired())
						{
							base.VerifyScriptEntry(exchangeRoleEntryPresentation.Name, array, false);
						}
						this.addedEntry = new ScriptRoleEntry(exchangeRoleEntryPresentation.Name, array);
						break;
					case ManagementRoleEntryType.Cmdlet | ManagementRoleEntryType.Script:
						break;
					case ManagementRoleEntryType.ApplicationPermission:
						this.addedEntry = new ApplicationPermissionRoleEntry(exchangeRoleEntryPresentation.Name, array);
						break;
					default:
						if (type == ManagementRoleEntryType.WebService)
						{
							this.VerifyWebServiceEntry(exchangeRoleEntryPresentation.Name);
							this.addedEntry = new WebServiceRoleEntry(exchangeRoleEntryPresentation.Name, new string[0]);
						}
						break;
					}
				}
				catch (FormatException ex)
				{
					base.WriteError(new ArgumentException(new LocalizedString(ex.Message)), ErrorCategory.InvalidArgument, this.DataObject.Id);
				}
				this.roleEntryOnDataObject = RoleHelper.GetRoleEntry(this.DataObject, this.Identity.CmdletOrScriptName, this.Identity.PSSnapinName, exchangeRoleEntryPresentation.Type, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (!this.Overwrite.IsPresent && null != this.roleEntryOnDataObject)
			{
				this.WriteWarning(Strings.WarningRoleEntryAlreadyExists(this.DataObject.Id.ToString(), this.Identity.CmdletOrScriptName));
			}
			this.InternalAddRemoveRoleEntry(this.DataObject.RoleEntries);
			TaskLogger.LogExit();
		}

		protected override void InternalAddRemoveRoleEntry(MultiValuedProperty<RoleEntry> roleEntries)
		{
			if (!this.addedEntry.Equals(this.roleEntryOnDataObject))
			{
				if (null != this.roleEntryOnDataObject)
				{
					roleEntries.Remove(this.roleEntryOnDataObject);
				}
				if (!roleEntries.Contains(this.addedEntry))
				{
					roleEntries.Add(this.addedEntry);
				}
			}
		}

		protected override string GetRoleEntryString()
		{
			return this.addedEntry.ToString();
		}

		protected override LocalizedException GetRoleSaveException(string roleEntry, string role, string exception)
		{
			return new ExRBACSaveAddRoleEntry(roleEntry, role, exception);
		}

		private void VerifyWebServiceEntry(string webMethodName)
		{
			if (base.Fields.IsModified(RbacCommonParameters.ParameterParameters) || base.Fields.IsModified(RbacCommonParameters.ParameterPSSnapinName))
			{
				base.WriteError(new InvalidOperationException(Strings.WebServiceRoleEntryNotSupportParametersAndSnapin), ErrorCategory.InvalidArgument, this.DataObject.Id);
			}
		}

		private const string ParameterSetParentRoleEntry = "ParentRoleEntry";

		private const string ParameterParentRoleEntry = "ParentRoleEntry";

		private const string ParameterOverwrite = "Overwrite";

		private RoleEntry addedEntry;

		private RoleEntry roleEntryOnDataObject;

		private static readonly ManagementRoleEntryType[] AllowedRoleEntryTypes = new ManagementRoleEntryType[]
		{
			ManagementRoleEntryType.Cmdlet,
			ManagementRoleEntryType.Script,
			ManagementRoleEntryType.ApplicationPermission,
			ManagementRoleEntryType.WebService
		};
	}
}
