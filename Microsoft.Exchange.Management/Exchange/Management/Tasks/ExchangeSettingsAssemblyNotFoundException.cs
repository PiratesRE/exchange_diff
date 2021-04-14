using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsAssemblyNotFoundException : ExchangeSettingsException
	{
		public ExchangeSettingsAssemblyNotFoundException(string name, string path, string type) : base(Strings.ExchangeSettingsAssemblyNotFound(name, path, type))
		{
			this.name = name;
			this.path = path;
			this.type = type;
		}

		public ExchangeSettingsAssemblyNotFoundException(string name, string path, string type, Exception innerException) : base(Strings.ExchangeSettingsAssemblyNotFound(name, path, type), innerException)
		{
			this.name = name;
			this.path = path;
			this.type = type;
		}

		protected ExchangeSettingsAssemblyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.path = (string)info.GetValue("path", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("path", this.path);
			info.AddValue("type", this.type);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string name;

		private readonly string path;

		private readonly string type;
	}
}
