using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooManyResultsException : LocalizedException
	{
		public TooManyResultsException(int count) : base(Strings.ErrorAlsTooManyLogResults(count))
		{
			this.count = count;
		}

		public TooManyResultsException(int count, Exception innerException) : base(Strings.ErrorAlsTooManyLogResults(count), innerException)
		{
			this.count = count;
		}

		protected TooManyResultsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.count = (int)info.GetValue("count", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("count", this.count);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private readonly int count;
	}
}
