using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExClusTransientException : TransientException
	{
		public ExClusTransientException(string funName) : base(CommonStrings.ExClusTransientException(funName))
		{
			this.funName = funName;
		}

		public ExClusTransientException(string funName, Exception innerException) : base(CommonStrings.ExClusTransientException(funName), innerException)
		{
			this.funName = funName;
		}

		protected ExClusTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.funName = (string)info.GetValue("funName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("funName", this.funName);
		}

		public string FunName
		{
			get
			{
				return this.funName;
			}
		}

		private readonly string funName;
	}
}
