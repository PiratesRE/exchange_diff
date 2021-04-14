using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DocumentValidationException : OperationFailedException
	{
		public DocumentValidationException(string msg) : base(Strings.DocumentValidationFailure(msg))
		{
			this.msg = msg;
		}

		public DocumentValidationException(string msg, Exception innerException) : base(Strings.DocumentValidationFailure(msg), innerException)
		{
			this.msg = msg;
		}

		protected DocumentValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
