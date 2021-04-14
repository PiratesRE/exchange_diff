using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class SetTransportServiceBase : SetTopologySystemConfigurationObjectTask<ServerIdParameter, TransportServer, Server>
	{
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan QueueLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["QueueLogMaxAge"];
			}
			set
			{
				base.Fields["QueueLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> QueueLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["QueueLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["QueueLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> QueueLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["QueueLogMaxFileSize"];
			}
			set
			{
				base.Fields["QueueLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath QueueLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["QueueLogPath"];
			}
			set
			{
				base.Fields["QueueLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan WlmLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["WlmLogMaxAge"];
			}
			set
			{
				base.Fields["WlmLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> WlmLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["WlmLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["WlmLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> WlmLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["WlmLogMaxFileSize"];
			}
			set
			{
				base.Fields["WlmLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath WlmLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["WlmLogPath"];
			}
			set
			{
				base.Fields["WlmLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan AgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["AgentLogMaxAge"];
			}
			set
			{
				base.Fields["AgentLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["AgentLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["AgentLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["AgentLogMaxFileSize"];
			}
			set
			{
				base.Fields["AgentLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath AgentLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["AgentLogPath"];
			}
			set
			{
				base.Fields["AgentLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AgentLogEnabled
		{
			get
			{
				return (bool)base.Fields["AgentLogEnabled"];
			}
			set
			{
				base.Fields["AgentLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan FlowControlLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["FlowControlLogMaxAge"];
			}
			set
			{
				base.Fields["FlowControlLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> FlowControlLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["FlowControlLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["FlowControlLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> FlowControlLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["FlowControlLogMaxFileSize"];
			}
			set
			{
				base.Fields["FlowControlLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath FlowControlLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["FlowControlLogPath"];
			}
			set
			{
				base.Fields["FlowControlLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FlowControlLogEnabled
		{
			get
			{
				return (bool)base.Fields["FlowControlLogEnabled"];
			}
			set
			{
				base.Fields["FlowControlLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ProcessingSchedulerLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["ProcessingSchedulerLogMaxAge"];
			}
			set
			{
				base.Fields["ProcessingSchedulerLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["ProcessingSchedulerLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["ProcessingSchedulerLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["ProcessingSchedulerLogMaxFileSize"];
			}
			set
			{
				base.Fields["ProcessingSchedulerLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ProcessingSchedulerLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["ProcessingSchedulerLogPath"];
			}
			set
			{
				base.Fields["ProcessingSchedulerLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ProcessingSchedulerLogEnabled
		{
			get
			{
				return (bool)base.Fields["ProcessingSchedulerLogEnabled"];
			}
			set
			{
				base.Fields["ProcessingSchedulerLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ResourceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["ResourceLogMaxAge"];
			}
			set
			{
				base.Fields["ResourceLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["ResourceLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["ResourceLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["ResourceLogMaxFileSize"];
			}
			set
			{
				base.Fields["ResourceLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ResourceLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["ResourceLogPath"];
			}
			set
			{
				base.Fields["ResourceLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ResourceLogEnabled
		{
			get
			{
				return (bool)base.Fields["ResourceLogEnabled"];
			}
			set
			{
				base.Fields["ResourceLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DnsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["DnsLogMaxAge"];
			}
			set
			{
				base.Fields["DnsLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["DnsLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["DnsLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["DnsLogMaxFileSize"];
			}
			set
			{
				base.Fields["DnsLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath DnsLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["DnsLogPath"];
			}
			set
			{
				base.Fields["DnsLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DnsLogEnabled
		{
			get
			{
				return (bool)base.Fields["DnsLogEnabled"];
			}
			set
			{
				base.Fields["DnsLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan JournalLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["JournalLogMaxAge"];
			}
			set
			{
				base.Fields["JournalLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> JournalLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["JournalLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["JournalLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> JournalLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["JournalLogMaxFileSize"];
			}
			set
			{
				base.Fields["JournalLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath JournalLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["JournalLogPath"];
			}
			set
			{
				base.Fields["JournalLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalLogEnabled
		{
			get
			{
				return (bool)base.Fields["JournalLogEnabled"];
			}
			set
			{
				base.Fields["JournalLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportMaintenanceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["TransportMaintenanceLogMaxAge"];
			}
			set
			{
				base.Fields["TransportMaintenanceLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["TransportMaintenanceLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["TransportMaintenanceLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["TransportMaintenanceLogMaxFileSize"];
			}
			set
			{
				base.Fields["TransportMaintenanceLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportMaintenanceLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["TransportMaintenanceLogPath"];
			}
			set
			{
				base.Fields["TransportMaintenanceLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportMaintenanceLogEnabled
		{
			get
			{
				return (bool)base.Fields["TransportMaintenanceLogEnabled"];
			}
			set
			{
				base.Fields["TransportMaintenanceLogEnabled"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTransportServer(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			Server server = (Server)base.PrepareDataObject();
			this.CheckServerRoles(server);
			return server;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			Server server = (Server)dataObject;
			this.existingPipelineTracingEnabled = server.PipelineTracingEnabled;
			base.StampChangesOn(dataObject);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Instance.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerNameModified), ErrorCategory.InvalidOperation, this.Identity);
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!this.DataObject.IsHubTransportServer && !this.DataObject.IsEdgeServer && !this.DataObject.IsFrontendTransportServer)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTaskRunningLocation), ErrorCategory.InvalidOperation, null);
			}
			if (this.DataObject.IsModified(ADTransportServerSchema.UseDowngradedExchangeServerAuth))
			{
				if (this.DataObject.IsEdgeServer && this.DataObject.UseDowngradedExchangeServerAuth)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotSetDowngradedExchangeServerAuthOnEdge), ErrorCategory.InvalidOperation, this.Identity);
				}
				if (!this.DataObject.UseDowngradedExchangeServerAuth)
				{
					foreach (ReceiveConnector receiveConnector in base.DataSession.FindPaged<ReceiveConnector>(null, this.DataObject.Identity, true, null, 0))
					{
						if (receiveConnector.SuppressXAnonymousTls)
						{
							base.WriteError(new InvalidOperationException(Strings.ErrorCannotUnsetDowngradedExchangeServerAuthIfReceiveConnectorHasSuppressXAnonmyousTlsSet(receiveConnector.Name)), ErrorCategory.InvalidOperation, this.Identity);
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.Fields.IsModified("QueueLogMaxAge"))
			{
				this.DataObject.QueueLogMaxAge = this.QueueLogMaxAge;
			}
			if (base.Fields.IsModified("QueueLogMaxDirectorySize"))
			{
				this.DataObject.QueueLogMaxDirectorySize = this.QueueLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("QueueLogMaxFileSize"))
			{
				this.DataObject.QueueLogMaxFileSize = this.QueueLogMaxFileSize;
			}
			if (base.Fields.IsModified("QueueLogPath"))
			{
				this.DataObject.QueueLogPath = this.QueueLogPath;
			}
			if (base.Fields.IsModified("WlmLogMaxAge"))
			{
				this.DataObject.WlmLogMaxAge = this.WlmLogMaxAge;
			}
			if (base.Fields.IsModified("WlmLogMaxDirectorySize"))
			{
				this.DataObject.WlmLogMaxDirectorySize = this.WlmLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("WlmLogMaxFileSize"))
			{
				this.DataObject.WlmLogMaxFileSize = this.WlmLogMaxFileSize;
			}
			if (base.Fields.IsModified("WlmLogPath"))
			{
				this.DataObject.WlmLogPath = this.WlmLogPath;
			}
			if (base.Fields.IsModified("AgentLogMaxAge"))
			{
				this.DataObject.AgentLogMaxAge = this.AgentLogMaxAge;
			}
			if (base.Fields.IsModified("AgentLogMaxDirectorySize"))
			{
				this.DataObject.AgentLogMaxDirectorySize = this.AgentLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("AgentLogMaxFileSize"))
			{
				this.DataObject.AgentLogMaxFileSize = this.AgentLogMaxFileSize;
			}
			if (base.Fields.IsModified("AgentLogPath"))
			{
				this.DataObject.AgentLogPath = this.AgentLogPath;
			}
			if (base.Fields.IsModified("AgentLogEnabled"))
			{
				this.DataObject.AgentLogEnabled = this.AgentLogEnabled;
			}
			if (base.Fields.IsModified("DnsLogMaxAge"))
			{
				this.DataObject.DnsLogMaxAge = this.DnsLogMaxAge;
			}
			if (base.Fields.IsModified("DnsLogMaxDirectorySize"))
			{
				this.DataObject.DnsLogMaxDirectorySize = this.DnsLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("DnsLogMaxFileSize"))
			{
				this.DataObject.DnsLogMaxFileSize = this.DnsLogMaxFileSize;
			}
			if (base.Fields.IsModified("DnsLogPath"))
			{
				this.DataObject.DnsLogPath = this.DnsLogPath;
			}
			if (base.Fields.IsModified("DnsLogEnabled"))
			{
				this.DataObject.DnsLogEnabled = this.DnsLogEnabled;
			}
			if (base.Fields.IsModified("FlowControlLogMaxAge"))
			{
				this.DataObject.FlowControlLogMaxAge = this.FlowControlLogMaxAge;
			}
			if (base.Fields.IsModified("FlowControlLogMaxDirectorySize"))
			{
				this.DataObject.FlowControlLogMaxDirectorySize = this.FlowControlLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("FlowControlLogMaxFileSize"))
			{
				this.DataObject.FlowControlLogMaxFileSize = this.FlowControlLogMaxFileSize;
			}
			if (base.Fields.IsModified("FlowControlLogPath"))
			{
				this.DataObject.FlowControlLogPath = this.FlowControlLogPath;
			}
			if (base.Fields.IsModified("FlowControlLogEnabled"))
			{
				this.DataObject.FlowControlLogEnabled = this.FlowControlLogEnabled;
			}
			if (base.Fields.IsModified("ProcessingSchedulerLogMaxAge"))
			{
				this.DataObject.ProcessingSchedulerLogMaxAge = this.ProcessingSchedulerLogMaxAge;
			}
			if (base.Fields.IsModified("ProcessingSchedulerLogMaxDirectorySize"))
			{
				this.DataObject.ProcessingSchedulerLogMaxDirectorySize = this.ProcessingSchedulerLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("ProcessingSchedulerLogMaxFileSize"))
			{
				this.DataObject.ProcessingSchedulerLogMaxFileSize = this.ProcessingSchedulerLogMaxFileSize;
			}
			if (base.Fields.IsModified("ProcessingSchedulerLogPath"))
			{
				this.DataObject.ProcessingSchedulerLogPath = this.ProcessingSchedulerLogPath;
			}
			if (base.Fields.IsModified("ProcessingSchedulerLogEnabled"))
			{
				this.DataObject.ProcessingSchedulerLogEnabled = this.ProcessingSchedulerLogEnabled;
			}
			if (base.Fields.IsModified("ResourceLogMaxAge"))
			{
				this.DataObject.ResourceLogMaxAge = this.ResourceLogMaxAge;
			}
			if (base.Fields.IsModified("ResourceLogMaxDirectorySize"))
			{
				this.DataObject.ResourceLogMaxDirectorySize = this.ResourceLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("ResourceLogMaxFileSize"))
			{
				this.DataObject.ResourceLogMaxFileSize = this.ResourceLogMaxFileSize;
			}
			if (base.Fields.IsModified("ResourceLogPath"))
			{
				this.DataObject.ResourceLogPath = this.ResourceLogPath;
			}
			if (base.Fields.IsModified("ResourceLogEnabled"))
			{
				this.DataObject.ResourceLogEnabled = this.ResourceLogEnabled;
			}
			if (base.Fields.IsModified("JournalLogMaxAge"))
			{
				this.DataObject.JournalLogMaxAge = this.JournalLogMaxAge;
			}
			if (base.Fields.IsModified("JournalLogMaxDirectorySize"))
			{
				this.DataObject.JournalLogMaxDirectorySize = this.JournalLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("JournalLogMaxFileSize"))
			{
				this.DataObject.JournalLogMaxFileSize = this.JournalLogMaxFileSize;
			}
			if (base.Fields.IsModified("JournalLogPath"))
			{
				this.DataObject.JournalLogPath = this.JournalLogPath;
			}
			if (base.Fields.IsModified("JournalLogEnabled"))
			{
				this.DataObject.JournalLogEnabled = this.JournalLogEnabled;
			}
			if (base.Fields.IsModified("TransportMaintenanceLogMaxAge"))
			{
				this.DataObject.TransportMaintenanceLogMaxAge = this.TransportMaintenanceLogMaxAge;
			}
			if (base.Fields.IsModified("TransportMaintenanceLogMaxDirectorySize"))
			{
				this.DataObject.TransportMaintenanceLogMaxDirectorySize = this.TransportMaintenanceLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("TransportMaintenanceLogMaxFileSize"))
			{
				this.DataObject.TransportMaintenanceLogMaxFileSize = this.TransportMaintenanceLogMaxFileSize;
			}
			if (base.Fields.IsModified("TransportMaintenanceLogPath"))
			{
				this.DataObject.TransportMaintenanceLogPath = this.TransportMaintenanceLogPath;
			}
			if (base.Fields.IsModified("TransportMaintenanceLogEnabled"))
			{
				this.DataObject.TransportMaintenanceLogEnabled = this.TransportMaintenanceLogEnabled;
			}
			bool flag = base.IsObjectStateChanged() && this.DataObject.IsModified(TransportServerSchema.PipelineTracingPath);
			base.InternalProcessRecord();
			if (this.DataObject.PipelineTracingEnabled && !this.existingPipelineTracingEnabled)
			{
				this.WriteWarning(Strings.WarningSecurePipelineTracingPath);
			}
			else if ((!this.DataObject.PipelineTracingEnabled && this.existingPipelineTracingEnabled) || flag)
			{
				this.WriteWarning(Strings.WarningCleanExistingPipelineTracingContent);
			}
			TaskLogger.LogExit();
		}

		private void CheckServerRoles(Server server)
		{
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			Server server2;
			try
			{
				server2 = topologyConfigurationSession.ReadLocalServer();
			}
			catch (TransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ResourceUnavailable, null);
				return;
			}
			if (server2 == null || !server2.IsEdgeServer)
			{
				if (server.IsEdgeServer)
				{
					base.WriteError(new CannotSetEdgeTransportServerOnAdException(), ErrorCategory.InvalidOperation, null);
					return;
				}
			}
			else if (server2.IsEdgeServer && !server.IsEdgeServer)
			{
				base.WriteError(new CannotSetHubTransportServerOnAdamException(), ErrorCategory.InvalidOperation, null);
			}
		}

		private const string QueueLogMaxAgeKey = "QueueLogMaxAge";

		private const string QueueLogMaxDirectorySizeKey = "QueueLogMaxDirectorySize";

		private const string QueueLogMaxFileSizeKey = "QueueLogMaxFileSize";

		private const string QueueLogPathKey = "QueueLogPath";

		private const string WlmLogMaxAgeKey = "WlmLogMaxAge";

		private const string WlmLogMaxDirectorySizeKey = "WlmLogMaxDirectorySize";

		private const string WlmLogMaxFileSizeKey = "WlmLogMaxFileSize";

		private const string WlmLogPathKey = "WlmLogPath";

		private const string AgentLogMaxAgeKey = "AgentLogMaxAge";

		private const string AgentLogMaxDirectorySizeKey = "AgentLogMaxDirectorySize";

		private const string AgentLogMaxFileSizeKey = "AgentLogMaxFileSize";

		private const string AgentLogPathKey = "AgentLogPath";

		private const string AgentLogEnabledKey = "AgentLogEnabled";

		private const string FlowControlLogMaxAgeKey = "FlowControlLogMaxAge";

		private const string FlowControlLogMaxDirectorySizeKey = "FlowControlLogMaxDirectorySize";

		private const string FlowControlLogMaxFileSizeKey = "FlowControlLogMaxFileSize";

		private const string FlowControlLogPathKey = "FlowControlLogPath";

		private const string FlowControlLogEnabledKey = "FlowControlLogEnabled";

		private const string ProcessingSchedulerLogMaxAgeKey = "ProcessingSchedulerLogMaxAge";

		private const string ProcessingSchedulerLogMaxDirectorySizeKey = "ProcessingSchedulerLogMaxDirectorySize";

		private const string ProcessingSchedulerLogMaxFileSizeKey = "ProcessingSchedulerLogMaxFileSize";

		private const string ProcessingSchedulerLogPathKey = "ProcessingSchedulerLogPath";

		private const string ProcessingSchedulerLogEnabledKey = "ProcessingSchedulerLogEnabled";

		private const string ResourceLogMaxAgeKey = "ResourceLogMaxAge";

		private const string ResourceLogMaxDirectorySizeKey = "ResourceLogMaxDirectorySize";

		private const string ResourceLogMaxFileSizeKey = "ResourceLogMaxFileSize";

		private const string ResourceLogPathKey = "ResourceLogPath";

		private const string ResourceLogEnabledKey = "ResourceLogEnabled";

		private const string DnsLogMaxAgeKey = "DnsLogMaxAge";

		private const string DnsLogMaxDirectorySizeKey = "DnsLogMaxDirectorySize";

		private const string DnsLogMaxFileSizeKey = "DnsLogMaxFileSize";

		private const string DnsLogPathKey = "DnsLogPath";

		private const string DnsLogEnabledKey = "DnsLogEnabled";

		private const string JournalLogMaxAgeKey = "JournalLogMaxAge";

		private const string JournalLogMaxDirectorySizeKey = "JournalLogMaxDirectorySize";

		private const string JournalLogMaxFileSizeKey = "JournalLogMaxFileSize";

		private const string JournalLogPathKey = "JournalLogPath";

		private const string JournalLogEnabledKey = "JournalLogEnabled";

		private const string TransportMaintenanceLogMaxAgeKey = "TransportMaintenanceLogMaxAge";

		private const string TransportMaintenanceLogMaxDirectorySizeKey = "TransportMaintenanceLogMaxDirectorySize";

		private const string TransportMaintenanceLogMaxFileSizeKey = "TransportMaintenanceLogMaxFileSize";

		private const string TransportMaintenanceLogPathKey = "TransportMaintenanceLogPath";

		private const string TransportMaintenanceLogEnabledKey = "TransportMaintenanceLogEnabled";

		private bool existingPipelineTracingEnabled;
	}
}
