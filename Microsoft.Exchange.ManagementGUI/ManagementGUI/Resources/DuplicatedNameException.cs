using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ManagementGUI.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicatedNameException : ArgumentException
	{
		public DuplicatedNameException(string name) : base(Strings.DuplicatedName(name))
		{
			this.name = name;
		}

		public DuplicatedNameException(string name, Exception innerException) : base(Strings.DuplicatedName(name), innerException)
		{
			this.name = name;
		}

		protected DuplicatedNameException(SerializationInfo info, StreamingContext context) : base(info, context)
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
