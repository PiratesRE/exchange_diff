using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnableToInitializeResourceException : LocalizedException
	{
		public UnableToInitializeResourceException(string reason) : base(Strings.UnableToInitializeResource(reason))
		{
			this.reason = reason;
		}

		public UnableToInitializeResourceException(string reason, Exception innerException) : base(Strings.UnableToInitializeResource(reason), innerException)
		{
			this.reason = reason;
		}

		protected UnableToInitializeResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
