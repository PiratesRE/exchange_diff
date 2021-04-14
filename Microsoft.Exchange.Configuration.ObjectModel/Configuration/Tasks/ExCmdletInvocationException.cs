using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	internal abstract class ExCmdletInvocationException : CmdletInvocationException
	{
		internal ExCmdletInvocationException(ErrorRecord errorRecord) : base((errorRecord.Exception != null) ? errorRecord.Exception.Message : null, errorRecord.Exception)
		{
			this.errorRecord = errorRecord;
		}

		protected ExCmdletInvocationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorRecord = (ErrorRecord)info.GetValue("errorRecord", typeof(ErrorRecord));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorRecord", this.ErrorRecord);
		}

		public override string Message
		{
			get
			{
				if (this.ErrorRecord != null && this.ErrorRecord.Exception != null)
				{
					return this.ErrorRecord.Exception.Message;
				}
				return base.Message;
			}
		}

		public override ErrorRecord ErrorRecord
		{
			get
			{
				return this.errorRecord;
			}
		}

		protected ErrorRecord errorRecord;
	}
}
