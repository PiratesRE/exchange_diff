using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplicationNotCompleteException : ADOperationException
	{
		public ReplicationNotCompleteException(string forestName, string dcName) : base(DirectoryStrings.ReplicationNotComplete(forestName, dcName))
		{
			this.forestName = forestName;
			this.dcName = dcName;
		}

		public ReplicationNotCompleteException(string forestName, string dcName, Exception innerException) : base(DirectoryStrings.ReplicationNotComplete(forestName, dcName), innerException)
		{
			this.forestName = forestName;
			this.dcName = dcName;
		}

		protected ReplicationNotCompleteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.forestName = (string)info.GetValue("forestName", typeof(string));
			this.dcName = (string)info.GetValue("dcName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("forestName", this.forestName);
			info.AddValue("dcName", this.dcName);
		}

		public string ForestName
		{
			get
			{
				return this.forestName;
			}
		}

		public string DcName
		{
			get
			{
				return this.dcName;
			}
		}

		private readonly string forestName;

		private readonly string dcName;
	}
}
