using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncSeedConfigNotSupportedException : TransientException
	{
		public IncSeedConfigNotSupportedException(string field) : base(ReplayStrings.IncSeedConfigNotSupportedError(field))
		{
			this.field = field;
		}

		public IncSeedConfigNotSupportedException(string field, Exception innerException) : base(ReplayStrings.IncSeedConfigNotSupportedError(field), innerException)
		{
			this.field = field;
		}

		protected IncSeedConfigNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.field = (string)info.GetValue("field", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("field", this.field);
		}

		public string Field
		{
			get
			{
				return this.field;
			}
		}

		private readonly string field;
	}
}
