using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InconsistentADException : LocalizedException
	{
		public InconsistentADException(string reason) : base(Strings.InconsistentADError(reason))
		{
			this.reason = reason;
		}

		public InconsistentADException(string reason, Exception innerException) : base(Strings.InconsistentADError(reason), innerException)
		{
			this.reason = reason;
		}

		protected InconsistentADException(SerializationInfo info, StreamingContext context) : base(info, context)
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
