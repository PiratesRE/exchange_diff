using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsInvalidPriorityException : ConfigurationSettingsException
	{
		public ConfigurationSettingsInvalidPriorityException(int priority) : base(DirectoryStrings.ConfigurationSettingsInvalidPriority(priority))
		{
			this.priority = priority;
		}

		public ConfigurationSettingsInvalidPriorityException(int priority, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsInvalidPriority(priority), innerException)
		{
			this.priority = priority;
		}

		protected ConfigurationSettingsInvalidPriorityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.priority = (int)info.GetValue("priority", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("priority", this.priority);
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
		}

		private readonly int priority;
	}
}
