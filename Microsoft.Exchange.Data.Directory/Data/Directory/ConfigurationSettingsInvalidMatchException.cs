using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsInvalidMatchException : ConfigurationSettingsException
	{
		public ConfigurationSettingsInvalidMatchException(string expression) : base(DirectoryStrings.ConfigurationSettingsInvalidMatch(expression))
		{
			this.expression = expression;
		}

		public ConfigurationSettingsInvalidMatchException(string expression, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsInvalidMatch(expression), innerException)
		{
			this.expression = expression;
		}

		protected ConfigurationSettingsInvalidMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.expression = (string)info.GetValue("expression", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("expression", this.expression);
		}

		public string Expression
		{
			get
			{
				return this.expression;
			}
		}

		private readonly string expression;
	}
}
