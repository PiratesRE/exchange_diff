using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Serializable]
	public class TransportAgent : ConfigurableObject
	{
		internal TransportAgent(string identity, bool enabled, int priority, string agentFactory, string assemblyPath) : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag[SimpleProviderObjectSchema.Identity] = new TransportAgentObjectId(identity);
			this.Enabled = enabled;
			this.Priority = priority;
			this.TransportAgentFactory = agentFactory;
			this.AssemblyPath = LocalLongFullPath.Parse(assemblyPath);
			base.ResetChangeTracking();
		}

		[Parameter(Mandatory = true)]
		public bool Enabled
		{
			get
			{
				return (bool)this.propertyBag[TransportAgentSchema.Enabled];
			}
			private set
			{
				this.propertyBag[TransportAgentSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int Priority
		{
			get
			{
				return (int)this.propertyBag[TransportAgentSchema.Priority];
			}
			private set
			{
				this.propertyBag[TransportAgentSchema.Priority] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string TransportAgentFactory
		{
			get
			{
				return (string)this.propertyBag[TransportAgentSchema.AgentFactory];
			}
			private set
			{
				this.propertyBag[TransportAgentSchema.AgentFactory] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public LocalLongFullPath AssemblyPath
		{
			get
			{
				return (LocalLongFullPath)this.propertyBag[TransportAgentSchema.AssemblyPath];
			}
			private set
			{
				this.propertyBag[TransportAgentSchema.AssemblyPath] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TransportAgent.schema;
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<TransportAgentSchema>();
	}
}
