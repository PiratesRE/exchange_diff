using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Get", "ManagementRole", DefaultParameterSetName = "Identity")]
	public sealed class GetManagementRole : GetMultitenancySystemConfigurationObjectTask<RoleIdParameter, ExchangeRole>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "Recurse", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Script", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "GetChildren", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override RoleIdParameter Identity
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string Cmdlet
		{
			get
			{
				return (string)base.Fields["Cmdlet"];
			}
			set
			{
				CmdletRoleEntry.ValidateName(value);
				base.Fields["Cmdlet"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string[] CmdletParameters
		{
			get
			{
				return (string[])base.Fields["CmdletParameters"];
			}
			set
			{
				RoleEntry.FormatParameters(value);
				base.Fields["CmdletParameters"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Script")]
		public string Script
		{
			get
			{
				return (string)base.Fields["Script"];
			}
			set
			{
				RoleEntry.ValidateName(value);
				base.Fields["Script"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Script")]
		public string[] ScriptParameters
		{
			get
			{
				return (string[])base.Fields["ScriptParameters"];
			}
			set
			{
				RoleEntry.FormatParameters(value);
				base.Fields["ScriptParameters"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Recurse")]
		public SwitchParameter Recurse
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recurse"] ?? false);
			}
			set
			{
				base.Fields["Recurse"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "GetChildren")]
		public SwitchParameter GetChildren
		{
			get
			{
				return (SwitchParameter)(base.Fields["GetChildren"] ?? false);
			}
			set
			{
				base.Fields["GetChildren"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Recurse")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "GetChildren")]
		public RoleType RoleType
		{
			get
			{
				return (RoleType)base.Fields["RoleType"];
			}
			set
			{
				base.VerifyValues<RoleType>(GetManagementRole.AllowedRoleTypes, value);
				base.Fields["RoleType"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return this.internalFilter;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			List<QueryFilter> list = new List<QueryFilter>();
			if (base.Fields.IsModified("RoleType"))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleSchema.RoleType, this.RoleType));
			}
			if (base.Fields.IsModified("Cmdlet"))
			{
				list.Add(RBACHelper.ConstructRoleEntryFilter(this.Cmdlet, ManagementRoleEntryType.Cmdlet));
			}
			if (base.Fields.IsModified("Script"))
			{
				list.Add(RBACHelper.ConstructRoleEntryFilter(this.Script, ManagementRoleEntryType.Script));
			}
			if (this.CmdletParameters != null)
			{
				list.Add(RBACHelper.ConstructRoleEntryParameterFilter(this.CmdletParameters));
			}
			if (this.ScriptParameters != null)
			{
				list.Add(RBACHelper.ConstructRoleEntryParameterFilter(this.ScriptParameters));
			}
			if (this.ScriptParameters != null || base.Fields.IsModified("Script"))
			{
				list.Add(RBACHelper.ScriptEnabledRoleEntryTypeFilter);
			}
			if (1 < list.Count)
			{
				this.internalFilter = new AndFilter(list.ToArray());
			}
			else if (1 == list.Count)
			{
				this.internalFilter = list[0];
			}
			else
			{
				this.internalFilter = null;
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ExchangeRole exchangeRole = (ExchangeRole)dataObject;
			bool flag = base.Fields.IsModified("RoleType") && this.RoleType != exchangeRole.RoleType;
			if (this.Cmdlet != null || this.CmdletParameters != null)
			{
				flag |= !RoleHelper.DoesRoleMatchingNameAndParameters(exchangeRole, 'c', this.Cmdlet, this.CmdletParameters);
			}
			else if (this.Script != null || this.ScriptParameters != null)
			{
				flag |= !RoleHelper.DoesRoleMatchingNameAndParameters(exchangeRole, 's', this.Script, this.ScriptParameters);
			}
			if (flag)
			{
				base.WriteVerbose(Strings.VerboseSkipObject(exchangeRole.DistinguishedName));
			}
			else
			{
				base.WriteResult(dataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Recurse.IsPresent || this.GetChildren.IsPresent)
			{
				ExchangeRole exchangeRole = (ExchangeRole)base.GetDataObject<ExchangeRole>(this.Identity, this.ConfigurationSession, this.RootId, new LocalizedString?(Strings.ErrorRoleNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(this.Identity.ToString())));
				this.WriteResult<ExchangeRole>(this.ConfigurationSession.FindPaged<ExchangeRole>(exchangeRole.Id, this.Recurse ? QueryScope.SubTree : QueryScope.OneLevel, this.InternalFilter, null, 0));
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private const string ParameterCmdlet = "Cmdlet";

		private const string ParameterCmdletParameters = "CmdletParameters";

		private const string ParameterScript = "Script";

		private const string ParameterScriptParameters = "ScriptParameters";

		private const string ParameterRoleType = "RoleType";

		private const string ParameterRecurse = "Recurse";

		private const string ParameterGetChildren = "GetChildren";

		private const string ParameterSetScript = "Script";

		private const string ParameterSetRecurse = "Recurse";

		private const string ParameterSetGetChildren = "GetChildren";

		private QueryFilter internalFilter;

		private static readonly RoleType[] AllowedRoleTypes = (RoleType[])Enum.GetValues(typeof(RoleType));
	}
}
