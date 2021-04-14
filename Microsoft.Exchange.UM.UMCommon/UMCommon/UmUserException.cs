using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UmUserException : LocalizedException
	{
		public UmUserException(string exceptionText) : base(Strings.UmUserException(exceptionText))
		{
			this.exceptionText = exceptionText;
		}

		public UmUserException(string exceptionText, Exception innerException) : base(Strings.UmUserException(exceptionText), innerException)
		{
			this.exceptionText = exceptionText;
		}

		protected UmUserException(SerializationInfo info, StreamingContext context) : base(info, context)
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
