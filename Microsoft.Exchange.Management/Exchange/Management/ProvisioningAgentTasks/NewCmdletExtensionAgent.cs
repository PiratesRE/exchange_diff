using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	[Cmdlet("New", "CmdletExtensionAgent", SupportsShouldProcess = true, DefaultParameterSetName = "NonSystemAgent")]
	public sealed class NewCmdletExtensionAgent : NewFixedNameSystemConfigurationObjectTask<CmdletExtensionAgent>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewCmdletExtensionAgent(this.Assembly, this.ClassFactory);
			}
		}

		[Parameter(Mandatory = false, Position = 0)]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Assembly
		{
			get
			{
				return this.DataObject.Assembly;
			}
			set
			{
				this.DataObject.Assembly = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string ClassFactory
		{
			get
			{
				return this.DataObject.ClassFactory;
			}
			set
			{
				this.DataObject.ClassFactory = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte Priority
		{
			get
			{
				if (base.Fields["Priority"] != null)
				{
					return (byte)base.Fields["Priority"];
				}
				return 0;
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)(base.Fields["Enabled"] ?? true);
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsSystem
		{
			get
			{
				return (bool)(base.Fields["IsSystem"] ?? false);
			}
			set
			{
				base.Fields["IsSystem"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.agentsGlobalConfig = new CmdletExtensionAgentsGlobalConfig((ITopologyConfigurationSession)base.DataSession);
			if (this.agentsGlobalConfig.ConfigurationIssues.Length > 0)
			{
				base.WriteError(new InvalidOperationException(this.agentsGlobalConfig.ConfigurationIssues[0]), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (CmdletExtensionAgent)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (this.Assembly.IndexOf("\\") != -1)
			{
				base.WriteError(new ArgumentException(Strings.ErrorAssemblyIsPath(this.Assembly)), ErrorCategory.InvalidArgument, null);
			}
			if (string.IsNullOrEmpty(this.DataObject.Name))
			{
				string[] array = this.ClassFactory.Split(new char[]
				{
					'.'
				});
				string text = array[array.Length - 1];
				if (text.Length > 64)
				{
					this.DataObject.Name = text.Substring(0, 64);
				}
				else
				{
					this.DataObject.Name = text;
				}
			}
			if (!this.agentsGlobalConfig.IsPriorityAvailable(this.Priority, null) && !this.agentsGlobalConfig.FreeUpPriorityValue(this.Priority))
			{
				base.WriteError(new ArgumentException(Strings.NotEnoughFreePrioritiesAvailable(this.Priority.ToString())), ErrorCategory.InvalidArgument, null);
			}
			this.DataObject.Priority = this.Priority;
			this.DataObject.Enabled = this.Enabled;
			this.DataObject.IsSystem = this.IsSystem;
			if (this.IsSystem && !this.Enabled)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorAgentCannotBeDisabled), ErrorCategory.InvalidOperation, null);
			}
			ADObjectId descendantId = base.RootOrgContainerId.GetDescendantId(new ADObjectId("CN=Global Settings"));
			ADObjectId descendantId2 = descendantId.GetDescendantId(new ADObjectId("CN=CmdletExtensionAgent Settings"));
			this.DataObject.SetId(descendantId2.GetChildId(this.DataObject.Name));
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.agentsGlobalConfig.IsFactoryIdentityInUse(this.DataObject.Assembly, this.DataObject.ClassFactory))
			{
				base.WriteError(new ArgumentException(Strings.FactoryIdentityInUse(this.DataObject.Assembly, this.DataObject.ClassFactory)), ErrorCategory.InvalidArgument, null);
			}
			Exception exception;
			if (!this.AssemblyAndClassFactoryFound(this.DataObject.Assembly, this.DataObject.ClassFactory, out exception))
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (this.agentsGlobalConfig.ObjectsToSave != null)
			{
				foreach (CmdletExtensionAgent instance in this.agentsGlobalConfig.ObjectsToSave)
				{
					base.DataSession.Save(instance);
				}
			}
			base.InternalProcessRecord();
			ProvisioningLayer.RefreshProvisioningBroker(this);
		}

		private bool AssemblyAndClassFactoryFound(string assemblyName, string classFactory, out Exception ex)
		{
			ProvisioningBroker.GetClassFactoryInstance(assemblyName, classFactory, out ex);
			return ex == null;
		}

		private CmdletExtensionAgentsGlobalConfig agentsGlobalConfig;
	}
}
