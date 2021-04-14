using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReadNetworkAdapterInfoException : LocalizedException
	{
		public ReadNetworkAdapterInfoException(string exception) : base(Strings.ReadNetworkAdapterInfo(exception))
		{
			this.exception = exception;
		}

		public ReadNetworkAdapterInfoException(string exception, Exception innerException) : base(Strings.ReadNetworkAdapterInfo(exception), innerException)
		{
			this.exception = exception;
		}

		protected ReadNetworkAdapterInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exception = (string)info.GetValue("exception", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exception", this.exception);
		}

		public string Exception
		{
			get
			{
				return this.exception;
			}
		}

		private readonly string exception;
	}
}
