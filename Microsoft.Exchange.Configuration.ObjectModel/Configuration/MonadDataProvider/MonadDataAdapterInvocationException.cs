using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[Serializable]
	internal class MonadDataAdapterInvocationException : ExCmdletInvocationException
	{
		internal MonadDataAdapterInvocationException(ErrorRecord errorRecord, string commandText) : base(errorRecord)
		{
			this.commandText = commandText;
		}

		protected MonadDataAdapterInvocationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public override string Message
		{
			get
			{
				if (this.ErrorRecord != null && this.ErrorRecord.Exception != null && !string.IsNullOrEmpty(this.ErrorRecord.Exception.Message) && !string.IsNullOrEmpty(this.commandText))
				{
					return Strings.InvocationExceptionDescription(this.ErrorRecord.Exception.Message, this.commandText).ToString();
				}
				if (!string.IsNullOrEmpty(this.commandText))
				{
					return Strings.InvocationExceptionDescriptionWithoutError(this.commandText).ToString();
				}
				return base.Message;
			}
		}

		private string commandText;
	}
}
