using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FsmConfigurationException : LocalizedException
	{
		public FsmConfigurationException(string exceptionText) : base(Strings.FsmConfigurationException(exceptionText))
		{
			this.exceptionText = exceptionText;
		}

		public FsmConfigurationException(string exceptionText, Exception innerException) : base(Strings.FsmConfigurationException(exceptionText), innerException)
		{
			this.exceptionText = exceptionText;
		}

		protected FsmConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
