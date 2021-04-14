using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class WorkItemNotFoundException : LocalizedException
	{
		public WorkItemNotFoundException(string workitemId) : base(Strings.WorkItemNotFoundException(workitemId))
		{
			this.workitemId = workitemId;
		}

		public WorkItemNotFoundException(string workitemId, Exception innerException) : base(Strings.WorkItemNotFoundException(workitemId), innerException)
		{
			this.workitemId = workitemId;
		}

		protected WorkItemNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.workitemId = (string)info.GetValue("workitemId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("workitemId", this.workitemId);
		}

		public string WorkitemId
		{
			get
			{
				return this.workitemId;
			}
		}

		private readonly string workitemId;
	}
}
