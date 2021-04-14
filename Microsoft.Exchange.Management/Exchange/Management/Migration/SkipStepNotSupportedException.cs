using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SkipStepNotSupportedException : LocalizedException
	{
		public SkipStepNotSupportedException(string step) : base(Strings.MigrationSkipStepNotSupported(step))
		{
			this.step = step;
		}

		public SkipStepNotSupportedException(string step, Exception innerException) : base(Strings.MigrationSkipStepNotSupported(step), innerException)
		{
			this.step = step;
		}

		protected SkipStepNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.step = (string)info.GetValue("step", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("step", this.step);
		}

		public string Step
		{
			get
			{
				return this.step;
			}
		}

		private readonly string step;
	}
}
