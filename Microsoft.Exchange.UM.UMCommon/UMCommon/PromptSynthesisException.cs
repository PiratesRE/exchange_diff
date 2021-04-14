using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PromptSynthesisException : LocalizedException
	{
		public PromptSynthesisException(string info) : base(Strings.PromptSynthesisException(info))
		{
			this.info = info;
		}

		public PromptSynthesisException(string info, Exception innerException) : base(Strings.PromptSynthesisException(info), innerException)
		{
			this.info = info;
		}

		protected PromptSynthesisException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.info = (string)info.GetValue("info", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("info", this.info);
		}

		public string Info
		{
			get
			{
				return this.info;
			}
		}

		private readonly string info;
	}
}
