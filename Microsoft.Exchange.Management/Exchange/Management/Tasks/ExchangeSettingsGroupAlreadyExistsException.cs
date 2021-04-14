using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsGroupAlreadyExistsException : ExchangeSettingsException
	{
		public ExchangeSettingsGroupAlreadyExistsException(string name) : base(Strings.ExchangeSettingsGroupAlreadyExists(name))
		{
			this.name = name;
		}

		public ExchangeSettingsGroupAlreadyExistsException(string name, Exception innerException) : base(Strings.ExchangeSettingsGroupAlreadyExists(name), innerException)
		{
			this.name = name;
		}

		protected ExchangeSettingsGroupAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
