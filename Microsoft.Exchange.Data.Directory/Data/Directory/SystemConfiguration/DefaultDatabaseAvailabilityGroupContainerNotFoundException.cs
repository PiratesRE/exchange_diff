using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DefaultDatabaseAvailabilityGroupContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public DefaultDatabaseAvailabilityGroupContainerNotFoundException(string agName) : base(DirectoryStrings.DefaultDatabaseAvailabilityGroupContainerNotFoundException(agName))
		{
			this.agName = agName;
		}

		public DefaultDatabaseAvailabilityGroupContainerNotFoundException(string agName, Exception innerException) : base(DirectoryStrings.DefaultDatabaseAvailabilityGroupContainerNotFoundException(agName), innerException)
		{
			this.agName = agName;
		}

		protected DefaultDatabaseAvailabilityGroupContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
