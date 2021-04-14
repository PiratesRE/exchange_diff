using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnknownAssistantException : LocalizedException
	{
		public UnknownAssistantException(string assistantName) : base(Strings.descUnknownAssistant(assistantName))
		{
			this.assistantName = assistantName;
		}

		public UnknownAssistantException(string assistantName, Exception innerException) : base(Strings.descUnknownAssistant(assistantName), innerException)
		{
			this.assistantName = assistantName;
		}

		protected UnknownAssistantException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.assistantName = (string)info.GetValue("assistantName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("assistantName", this.assistantName);
		}

		public string AssistantName
		{
			get
			{
				return this.assistantName;
			}
		}

		private readonly string assistantName;
	}
}
