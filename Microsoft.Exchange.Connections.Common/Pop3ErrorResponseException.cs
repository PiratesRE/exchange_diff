using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3ErrorResponseException : LocalizedException
	{
		public Pop3ErrorResponseException(string command, string response) : base(CXStrings.Pop3ErrorResponseMsg(command, response))
		{
			this.command = command;
			this.response = response;
		}

		public Pop3ErrorResponseException(string command, string response, Exception innerException) : base(CXStrings.Pop3ErrorResponseMsg(command, response), innerException)
		{
			this.command = command;
			this.response = response;
		}

		protected Pop3ErrorResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.command = (string)info.GetValue("command", typeof(string));
			this.response = (string)info.GetValue("response", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("command", this.command);
			info.AddValue("response", this.response);
		}

		public string Command
		{
			get
			{
				return this.command;
			}
		}

		public string Response
		{
			get
			{
				return this.response;
			}
		}

		private readonly string command;

		private readonly string response;
	}
}
