using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RegistryParameterException : LocalizedException
	{
		public RegistryParameterException(string errorMsg) : base(ReplayStrings.RegistryParameterException(errorMsg))
		{
			this.errorMsg = errorMsg;
		}

		public RegistryParameterException(string errorMsg, Exception innerException) : base(ReplayStrings.RegistryParameterException(errorMsg), innerException)
		{
			this.errorMsg = errorMsg;
		}

		protected RegistryParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string errorMsg;
	}
}
