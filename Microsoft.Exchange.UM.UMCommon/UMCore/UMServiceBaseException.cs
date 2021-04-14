using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMServiceBaseException : LocalizedException
	{
		public UMServiceBaseException(string exceptionText) : base(Strings.UMServiceBaseException(exceptionText))
		{
			this.exceptionText = exceptionText;
		}

		public UMServiceBaseException(string exceptionText, Exception innerException) : base(Strings.UMServiceBaseException(exceptionText), innerException)
		{
			this.exceptionText = exceptionText;
		}

		protected UMServiceBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exceptionText = (string)info.GetValue("exceptionText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exceptionText", this.exceptionText);
		}

		public string ExceptionText
		{
			get
			{
				return this.exceptionText;
			}
		}

		private readonly string exceptionText;
	}
}
