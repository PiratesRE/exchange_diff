using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsGroupFoundMultipleTimesException : ExchangeSettingsException
	{
		public ExchangeSettingsGroupFoundMultipleTimesException(string name) : base(Strings.ExchangeSettingsGroupFoundMultipleTimes(name))
		{
			this.name = name;
		}

		public ExchangeSettingsGroupFoundMultipleTimesException(string name, Exception innerException) : base(Strings.ExchangeSettingsGroupFoundMultipleTimes(name), innerException)
		{
			this.name = name;
		}

		protected ExchangeSettingsGroupFoundMultipleTimesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
