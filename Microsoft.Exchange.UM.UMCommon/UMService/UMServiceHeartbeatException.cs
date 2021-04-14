using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Exchange.UM.UMService.Exceptions;

namespace Microsoft.Exchange.UM.UMService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMServiceHeartbeatException : UMServiceBaseException
	{
		public UMServiceHeartbeatException(string extraInfo) : base(Strings.UMServiceHeartbeatException(extraInfo))
		{
			this.extraInfo = extraInfo;
		}

		public UMServiceHeartbeatException(string extraInfo, Exception innerException) : base(Strings.UMServiceHeartbeatException(extraInfo), innerException)
		{
			this.extraInfo = extraInfo;
		}

		protected UMServiceHeartbeatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.extraInfo = (string)info.GetValue("extraInfo", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("extraInfo", this.extraInfo);
		}

		public string ExtraInfo
		{
			get
			{
				return this.extraInfo;
			}
		}

		private readonly string extraInfo;
	}
}
