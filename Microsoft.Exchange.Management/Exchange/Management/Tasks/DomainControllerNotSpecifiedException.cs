using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DomainControllerNotSpecifiedException : LocalizedException
	{
		public DomainControllerNotSpecifiedException() : base(Strings.DomainControllerNotSpecifiedException)
		{
		}

		public DomainControllerNotSpecifiedException(Exception innerException) : base(Strings.DomainControllerNotSpecifiedException, innerException)
		{
		}

		protected DomainControllerNotSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
