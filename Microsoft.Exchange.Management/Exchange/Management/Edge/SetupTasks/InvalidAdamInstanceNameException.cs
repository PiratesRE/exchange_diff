using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidAdamInstanceNameException : LocalizedException
	{
		public InvalidAdamInstanceNameException(string instanceName) : base(Strings.InvalidAdamInstanceName(instanceName))
		{
			this.instanceName = instanceName;
		}

		public InvalidAdamInstanceNameException(string instanceName, Exception innerException) : base(Strings.InvalidAdamInstanceName(instanceName), innerException)
		{
			this.instanceName = instanceName;
		}

		protected InvalidAdamInstanceNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.instanceName = (string)info.GetValue("instanceName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("instanceName", this.instanceName);
		}

		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
		}

		private readonly string instanceName;
	}
}
