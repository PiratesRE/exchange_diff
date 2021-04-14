using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CompliancePolicyCountExceedsLimitException : LocalizedException
	{
		public CompliancePolicyCountExceedsLimitException(int limit) : base(Strings.CompliancePolicyCountExceedsLimit(limit))
		{
			this.limit = limit;
		}

		public CompliancePolicyCountExceedsLimitException(int limit, Exception innerException) : base(Strings.CompliancePolicyCountExceedsLimit(limit), innerException)
		{
			this.limit = limit;
		}

		protected CompliancePolicyCountExceedsLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.limit = (int)info.GetValue("limit", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("limit", this.limit);
		}

		public int Limit
		{
			get
			{
				return this.limit;
			}
		}

		private readonly int limit;
	}
}
