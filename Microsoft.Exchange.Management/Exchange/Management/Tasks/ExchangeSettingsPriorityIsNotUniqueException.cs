using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsPriorityIsNotUniqueException : ExchangeSettingsException
	{
		public ExchangeSettingsPriorityIsNotUniqueException(string name, int priority) : base(Strings.ExchangeSettingsPriorityIsNotUnique(name, priority))
		{
			this.name = name;
			this.priority = priority;
		}

		public ExchangeSettingsPriorityIsNotUniqueException(string name, int priority, Exception innerException) : base(Strings.ExchangeSettingsPriorityIsNotUnique(name, priority), innerException)
		{
			this.name = name;
			this.priority = priority;
		}

		protected ExchangeSettingsPriorityIsNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.priority = (int)info.GetValue("priority", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("priority", this.priority);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
		}

		private readonly string name;

		private readonly int priority;
	}
}
