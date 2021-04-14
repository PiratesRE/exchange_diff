using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FastTransientDocumentException : OperationFailedException
	{
		public FastTransientDocumentException(string msg) : base(Strings.FastCannotProcessDocument(msg))
		{
			this.msg = msg;
		}

		public FastTransientDocumentException(string msg, Exception innerException) : base(Strings.FastCannotProcessDocument(msg), innerException)
		{
			this.msg = msg;
		}

		protected FastTransientDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
