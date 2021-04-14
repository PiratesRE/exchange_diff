using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MaximumAllowedNumberOfGatewayIPAddressesExceededException : LocalizedException
	{
		public MaximumAllowedNumberOfGatewayIPAddressesExceededException(int maxAllowed) : base(Strings.MaximumAllowedNumberOfGatewayIPAddressesExceededId(maxAllowed))
		{
			this.maxAllowed = maxAllowed;
		}

		public MaximumAllowedNumberOfGatewayIPAddressesExceededException(int maxAllowed, Exception innerException) : base(Strings.MaximumAllowedNumberOfGatewayIPAddressesExceededId(maxAllowed), innerException)
		{
			this.maxAllowed = maxAllowed;
		}

		protected MaximumAllowedNumberOfGatewayIPAddressesExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maxAllowed = (int)info.GetValue("maxAllowed", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maxAllowed", this.maxAllowed);
		}

		public int MaxAllowed
		{
			get
			{
				return this.maxAllowed;
			}
		}

		private readonly int maxAllowed;
	}
}
