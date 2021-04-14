using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArgumentDuplicatedException : DiagnosticArgumentException
	{
		public ArgumentDuplicatedException(string msg) : base(DiagnosticsResources.ArgumentDuplicated(msg))
		{
			this.msg = msg;
		}

		public ArgumentDuplicatedException(string msg, Exception innerException) : base(DiagnosticsResources.ArgumentDuplicated(msg), innerException)
		{
			this.msg = msg;
		}

		protected ArgumentDuplicatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}
