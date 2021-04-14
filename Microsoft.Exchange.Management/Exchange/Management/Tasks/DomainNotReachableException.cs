using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DomainNotReachableException : LocalizedException
	{
		public DomainNotReachableException(string dom, string taskName) : base(Strings.DomainNotReachableException(dom, taskName))
		{
			this.dom = dom;
			this.taskName = taskName;
		}

		public DomainNotReachableException(string dom, string taskName, Exception innerException) : base(Strings.DomainNotReachableException(dom, taskName), innerException)
		{
			this.dom = dom;
			this.taskName = taskName;
		}

		protected DomainNotReachableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dom = (string)info.GetValue("dom", typeof(string));
			this.taskName = (string)info.GetValue("taskName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dom", this.dom);
			info.AddValue("taskName", this.taskName);
		}

		public string Dom
		{
			get
			{
				return this.dom;
			}
		}

		public string TaskName
		{
			get
			{
				return this.taskName;
			}
		}

		private readonly string dom;

		private readonly string taskName;
	}
}
