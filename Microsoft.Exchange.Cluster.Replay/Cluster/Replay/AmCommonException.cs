using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmCommonException : AmServerException
	{
		public AmCommonException(string error) : base(ReplayStrings.AmCommonException(error))
		{
			this.error = error;
		}

		public AmCommonException(string error, Exception innerException) : base(ReplayStrings.AmCommonException(error), innerException)
		{
			this.error = error;
		}

		protected AmCommonException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
