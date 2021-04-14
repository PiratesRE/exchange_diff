using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DefaultDatabaseContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public DefaultDatabaseContainerNotFoundException(string agName) : base(DirectoryStrings.DefaultDatabaseContainerNotFoundException(agName))
		{
			this.agName = agName;
		}

		public DefaultDatabaseContainerNotFoundException(string agName, Exception innerException) : base(DirectoryStrings.DefaultDatabaseContainerNotFoundException(agName), innerException)
		{
			this.agName = agName;
		}

		protected DefaultDatabaseContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.agName = (string)info.GetValue("agName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("agName", this.agName);
		}

		public string AgName
		{
			get
			{
				return this.agName;
			}
		}

		private readonly string agName;
	}
}
