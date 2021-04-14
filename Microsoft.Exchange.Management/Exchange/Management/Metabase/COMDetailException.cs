using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class COMDetailException : DataSourceOperationException
	{
		public COMDetailException(string errorMsg, string directoryEntry, string detailMessage) : base(Strings.COMDetailException(errorMsg, directoryEntry, detailMessage))
		{
			this.errorMsg = errorMsg;
			this.directoryEntry = directoryEntry;
			this.detailMessage = detailMessage;
		}

		public COMDetailException(string errorMsg, string directoryEntry, string detailMessage, Exception innerException) : base(Strings.COMDetailException(errorMsg, directoryEntry, detailMessage), innerException)
		{
			this.errorMsg = errorMsg;
			this.directoryEntry = directoryEntry;
			this.detailMessage = detailMessage;
		}

		protected COMDetailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
			this.directoryEntry = (string)info.GetValue("directoryEntry", typeof(string));
			this.detailMessage = (string)info.GetValue("detailMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMsg", this.errorMsg);
			info.AddValue("directoryEntry", this.directoryEntry);
			info.AddValue("detailMessage", this.detailMessage);
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		public string DirectoryEntry
		{
			get
			{
				return this.directoryEntry;
			}
		}

		public string DetailMessage
		{
			get
			{
				return this.detailMessage;
			}
		}

		private readonly string errorMsg;

		private readonly string directoryEntry;

		private readonly string detailMessage;
	}
}
