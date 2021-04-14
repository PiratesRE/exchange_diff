using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MwiNoTargetsAvailableException : MwiDeliveryException
	{
		public MwiNoTargetsAvailableException(string userName) : base(Strings.descMwiNoTargetsAvailableError(userName))
		{
			this.userName = userName;
		}

		public MwiNoTargetsAvailableException(string userName, Exception innerException) : base(Strings.descMwiNoTargetsAvailableError(userName), innerException)
		{
			this.userName = userName;
		}

		protected MwiNoTargetsAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		private readonly string userName;
	}
}
