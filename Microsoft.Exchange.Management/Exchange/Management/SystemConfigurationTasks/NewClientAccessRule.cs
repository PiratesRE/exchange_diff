using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ClientAccessRule", SupportsShouldProcess = true)]
	public sealed class NewClientAccessRule : NewMultitenancySystemConfigurationObjectTask<ADClientAccessRule>
	{
		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return this.DataObject.Priority;
			}
			set
			{
				this.DataObject.Priority = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DatacenterAdminsOnly
		{
			get
			{
				return this.DataObject.DatacenterAdminsOnly;
			}
			set
			{
				this.DataObject.DatacenterAdminsOnly = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ClientAccessRulesAction Action
		{
			get
			{
				return this.DataObject.Action;
			}
			set
			{
				this.DataObject.Action = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> AnyOfClientIPAddressesOrRanges
		{
			get
			{
				return this.DataObject.AnyOfClientIPAddressesOrRanges;
			}
			set
			{
				this.DataObject.AnyOfClientIPAddressesOrRanges = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> ExceptAnyOfClientIPAddressesOrRanges
		{
			get
			{
				return this.DataObject.ExceptAnyOfClientIPAddressesOrRanges;
			}
			set
			{
				this.DataObject.ExceptAnyOfClientIPAddressesOrRanges = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IntRange> AnyOfSourceTcpPortNumbers
		{
			get
			{
				return this.DataObject.AnyOfSourceTcpPortNumbers;
			}
			set
			{
				this.DataObject.AnyOfSourceTcpPortNumbers = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IntRange> ExceptAnyOfSourceTcpPortNumbers
		{
			get
			{
				return this.DataObject.ExceptAnyOfSourceTcpPortNumbers;
			}
			set
			{
				this.DataObject.ExceptAnyOfSourceTcpPortNumbers = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UsernameMatchesAnyOfPatterns
		{
			get
			{
				return this.DataObject.UsernameMatchesAnyOfPatterns;
			}
			set
			{
				this.DataObject.UsernameMatchesAnyOfPatterns = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExceptUsernameMatchesAnyOfPatterns
		{
			get
			{
				return this.DataObject.ExceptUsernameMatchesAnyOfPatterns;
			}
			set
			{
				this.DataObject.ExceptUsernameMatchesAnyOfPatterns = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UserIsMemberOf
		{
			get
			{
				return this.DataObject.UserIsMemberOf;
			}
			set
			{
				this.DataObject.UserIsMemberOf = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExceptUserIsMemberOf
		{
			get
			{
				return this.DataObject.ExceptUserIsMemberOf;
			}
			set
			{
				this.DataObject.ExceptUserIsMemberOf = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessAuthenticationMethod> AnyOfAuthenticationTypes
		{
			get
			{
				return this.DataObject.AnyOfAuthenticationTypes;
			}
			set
			{
				this.DataObject.AnyOfAuthenticationTypes = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessAuthenticationMethod> ExceptAnyOfAuthenticationTypes
		{
			get
			{
				return this.DataObject.ExceptAnyOfAuthenticationTypes;
			}
			set
			{
				this.DataObject.ExceptAnyOfAuthenticationTypes = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessProtocol> AnyOfProtocols
		{
			get
			{
				return this.DataObject.AnyOfProtocols;
			}
			set
			{
				this.DataObject.AnyOfProtocols = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ClientAccessProtocol> ExceptAnyOfProtocols
		{
			get
			{
				return this.DataObject.ExceptAnyOfProtocols;
			}
			set
			{
				this.DataObject.ExceptAnyOfProtocols = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UserRecipientFilter
		{
			get
			{
				return this.DataObject.UserRecipientFilter;
			}
			set
			{
				this.DataObject.UserRecipientFilter = value;
			}
		}

		private ADObjectId GetBaseContainer()
		{
			return base.CurrentOrgContainerId.GetChildId(ADClientAccessRuleCollection.ContainerName);
		}

		private ADObjectId GetObjectId()
		{
			return this.GetBaseContainer().GetChildId(base.Name);
		}

		protected override void WriteResult(IConfigurable result)
		{
			((ADClientAccessRule)result).Priority = this.DataObject.Priority;
			base.WriteResult(result);
		}

		protected override void InternalProcessRecord()
		{
			int priority = 0;
			bool flag = false;
			int clientAccessRulesLimit = AppSettings.Current.ClientAccessRulesLimit;
			ClientAccessRulesPriorityManager clientAccessRulesPriorityManager = new ClientAccessRulesPriorityManager(ClientAccessRulesStorageManager.GetClientAccessRules((IConfigurationSession)base.DataSession));
			if (clientAccessRulesPriorityManager.ADClientAccessRules.Count >= clientAccessRulesLimit)
			{
				base.WriteError(new InvalidOperationException(RulesTasksStrings.ClientAccessRulesLimitError(clientAccessRulesLimit)), ErrorCategory.InvalidOperation, null);
			}
			if (!ClientAccessRulesStorageManager.IsADRuleValid(this.DataObject))
			{
				base.WriteError(new InvalidOperationException(RulesTasksStrings.ClientAccessRulesAuthenticationTypeInvalid), ErrorCategory.InvalidOperation, null);
			}
			this.DataObject.InternalPriority = clientAccessRulesPriorityManager.GetInternalPriority(this.Priority, this.DatacenterAdminsOnly, out priority, out flag);
			this.DataObject.RuleName = this.DataObject.Name;
			this.DataObject.Priority = priority;
			if (flag)
			{
				ClientAccessRulesStorageManager.SaveRules((IConfigurationSession)base.DataSession, clientAccessRulesPriorityManager.ADClientAccessRules);
			}
			base.CreateParentContainerIfNeeded(this.DataObject);
			base.InternalProcessRecord();
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			this.DataObject.SetId(this.GetObjectId());
			return this.DataObject;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return RulesTasksStrings.ConfirmationMessageNewClientAccessRule(base.Name);
			}
		}
	}
}
