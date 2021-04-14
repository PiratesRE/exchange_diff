using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoveStartAfterDateRangeException : RecipientTaskException
	{
		public MoveStartAfterDateRangeException(int days) : base(Strings.ErrorStartAfter(days))
		{
			this.days = days;
		}

		public MoveStartAfterDateRangeException(int days, Exception innerException) : base(Strings.ErrorStartAfter(days), innerException)
		{
			this.days = days;
		}

		protected MoveStartAfterDateRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.days = (int)info.GetValue("days", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("days", this.days);
		}

		public int Days
		{
			get
			{
				return this.days;
			}
		}

		private readonly int days;
	}
}
