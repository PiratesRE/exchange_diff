using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotConnectToClusterErrorException : LocalizedException
	{
		public CouldNotConnectToClusterErrorException(string machineName, string error) : base(Strings.CouldNotConnectToClusterError(machineName, error))
		{
			this.machineName = machineName;
			this.error = error;
		}

		public CouldNotConnectToClusterErrorException(string machineName, string error, Exception innerException) : base(Strings.CouldNotConnectToClusterError(machineName, error), innerException)
		{
			this.machineName = machineName;
			this.error = error;
		}

		protected CouldNotConnectToClusterErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.machineName = (string)info.GetValue("machineName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("machineName", this.machineName);
			info.AddValue("error", this.error);
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string machineName;

		private readonly string error;
	}
}
