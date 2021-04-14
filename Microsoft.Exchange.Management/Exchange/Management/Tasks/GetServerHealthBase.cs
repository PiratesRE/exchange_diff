using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class GetServerHealthBase : Task
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Alias(new string[]
		{
			"Server"
		})]
		public ServerIdParameter Identity
		{
			get
			{
				return (ServerIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HealthSet
		{
			get
			{
				return this.healthSet;
			}
			set
			{
				this.healthSet = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter HaImpactingOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["HaImpactingOnly"] ?? false);
			}
			set
			{
				base.Fields["HaImpactingOnly"] = value;
			}
		}

		private string healthSet;
	}
}
