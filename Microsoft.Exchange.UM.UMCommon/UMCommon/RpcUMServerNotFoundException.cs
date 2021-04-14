using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RpcUMServerNotFoundException : UMServerNotFoundException
	{
		public RpcUMServerNotFoundException() : base(Strings.RpcUMServerNotFoundException)
		{
		}

		public RpcUMServerNotFoundException(Exception innerException) : base(Strings.RpcUMServerNotFoundException, innerException)
		{
		}

		protected RpcUMServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
