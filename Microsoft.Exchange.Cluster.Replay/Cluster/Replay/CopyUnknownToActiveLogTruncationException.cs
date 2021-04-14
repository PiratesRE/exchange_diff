using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CopyUnknownToActiveLogTruncationException : LogTruncationException
	{
		public CopyUnknownToActiveLogTruncationException(string db, string activeNode, string targetNode, uint hresult) : base(ReplayStrings.CopyUnknownToActiveLogTruncationException(db, activeNode, targetNode, hresult))
		{
			this.db = db;
			this.activeNode = activeNode;
			this.targetNode = targetNode;
			this.hresult = hresult;
		}

		public CopyUnknownToActiveLogTruncationException(string db, string activeNode, string targetNode, uint hresult, Exception innerException) : base(ReplayStrings.CopyUnknownToActiveLogTruncationException(db, activeNode, targetNode, hresult), innerException)
		{
			this.db = db;
			this.activeNode = activeNode;
			this.targetNode = targetNode;
			this.hresult = hresult;
		}

		protected CopyUnknownToActiveLogTruncationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (string)info.GetValue("db", typeof(string));
			this.activeNode = (string)info.GetValue("activeNode", typeof(string));
			this.targetNode = (string)info.GetValue("targetNode", typeof(string));
			this.hresult = (uint)info.GetValue("hresult", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
			info.AddValue("activeNode", this.activeNode);
			info.AddValue("targetNode", this.targetNode);
			info.AddValue("hresult", this.hresult);
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		public string ActiveNode
		{
			get
			{
				return this.activeNode;
			}
		}

		public string TargetNode
		{
			get
			{
				return this.targetNode;
			}
		}

		public uint Hresult
		{
			get
			{
				return this.hresult;
			}
		}

		private readonly string db;

		private readonly string activeNode;

		private readonly string targetNode;

		private readonly uint hresult;
	}
}
