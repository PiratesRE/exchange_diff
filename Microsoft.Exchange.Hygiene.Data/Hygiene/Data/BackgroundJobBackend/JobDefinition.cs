using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class JobDefinition : BackgroundJobBackendBase
	{
		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[JobDefinition.BackgroundJobIdProperty];
			}
			set
			{
				this[JobDefinition.BackgroundJobIdProperty] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[JobDefinition.NameProperty];
			}
			set
			{
				this[JobDefinition.NameProperty] = value;
			}
		}

		public string Path
		{
			get
			{
				return (string)this[JobDefinition.PathProperty];
			}
			set
			{
				this[JobDefinition.PathProperty] = value;
			}
		}

		public string CommandLine
		{
			get
			{
				return (string)this[JobDefinition.CommandLineProperty];
			}
			set
			{
				this[JobDefinition.CommandLineProperty] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[JobDefinition.DescriptionProperty];
			}
			set
			{
				this[JobDefinition.DescriptionProperty] = value;
			}
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[JobDefinition.RoleIdProperty];
			}
			set
			{
				this[JobDefinition.RoleIdProperty] = value;
			}
		}

		public bool SingleInstancePerMachine
		{
			get
			{
				return (bool)this[JobDefinition.SingleInstancePerMachineProperty];
			}
			set
			{
				this[JobDefinition.SingleInstancePerMachineProperty] = value;
			}
		}

		public SchedulingStrategyType SchedulingStrategy
		{
			get
			{
				return (SchedulingStrategyType)this[JobDefinition.SchedulingStrategyProperty];
			}
			set
			{
				this[JobDefinition.SchedulingStrategyProperty] = (byte)value;
			}
		}

		public int Timeout
		{
			get
			{
				return (int)this[JobDefinition.TimeoutProperty];
			}
			set
			{
				this[JobDefinition.TimeoutProperty] = value;
			}
		}

		public byte MaxLocalRetryCount
		{
			get
			{
				return (byte)this[JobDefinition.MaxLocalRetryCountProperty];
			}
			set
			{
				this[JobDefinition.MaxLocalRetryCountProperty] = value;
			}
		}

		public short MaxFailoverCount
		{
			get
			{
				return (short)this[JobDefinition.MaxFailoverCountProperty];
			}
			set
			{
				this[JobDefinition.MaxFailoverCountProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = ScheduleItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition NameProperty = JobDefinitionProperties.NameProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition PathProperty = JobDefinitionProperties.PathProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition CommandLineProperty = JobDefinitionProperties.CommandLineProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition DescriptionProperty = JobDefinitionProperties.DescriptionProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = JobDefinitionProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SingleInstancePerMachineProperty = JobDefinitionProperties.SingleInstancePerMachineProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingStrategyProperty = JobDefinitionProperties.SchedulingStrategyProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TimeoutProperty = JobDefinitionProperties.TimeoutProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition MaxLocalRetryCountProperty = JobDefinitionProperties.MaxLocalRetryCountProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition MaxFailoverCountProperty = JobDefinitionProperties.MaxFailoverCountProperty;
	}
}
