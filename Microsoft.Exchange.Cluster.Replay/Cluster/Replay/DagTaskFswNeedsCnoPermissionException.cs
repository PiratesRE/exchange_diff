using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskFswNeedsCnoPermissionException : DagTaskServerException
	{
		public DagTaskFswNeedsCnoPermissionException(string fswPath, string accountName) : base(ReplayStrings.DagTaskFswNeedsCnoPermissionException(fswPath, accountName))
		{
			this.fswPath = fswPath;
			this.accountName = accountName;
		}

		public DagTaskFswNeedsCnoPermissionException(string fswPath, string accountName, Exception innerException) : base(ReplayStrings.DagTaskFswNeedsCnoPermissionException(fswPath, accountName), innerException)
		{
			this.fswPath = fswPath;
			this.accountName = accountName;
		}

		protected DagTaskFswNeedsCnoPermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fswPath = (string)info.GetValue("fswPath", typeof(string));
			this.accountName = (string)info.GetValue("accountName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fswPath", this.fswPath);
			info.AddValue("accountName", this.accountName);
		}

		public string FswPath
		{
			get
			{
				return this.fswPath;
			}
		}

		public string AccountName
		{
			get
			{
				return this.accountName;
			}
		}

		private readonly string fswPath;

		private readonly string accountName;
	}
}
