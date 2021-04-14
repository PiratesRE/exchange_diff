using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.DataProviders
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRequestException : StoragePermanentException
	{
		public InvalidRequestException(LocalizedString violation) : base(Strings.InvalidRequest(violation))
		{
			this.violation = violation;
		}

		public InvalidRequestException(LocalizedString violation, Exception innerException) : base(Strings.InvalidRequest(violation), innerException)
		{
			this.violation = violation;
		}

		protected InvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.violation = (LocalizedString)info.GetValue("violation", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("violation", this.violation);
		}

		public LocalizedString Violation
		{
			get
			{
				return this.violation;
			}
		}

		private readonly LocalizedString violation;
	}
}
