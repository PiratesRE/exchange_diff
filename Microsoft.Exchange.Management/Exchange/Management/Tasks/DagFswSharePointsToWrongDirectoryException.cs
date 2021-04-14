using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswSharePointsToWrongDirectoryException : LocalizedException
	{
		public DagFswSharePointsToWrongDirectoryException(string share, string server, string currentdirectory, string witnessdirectory) : base(Strings.DagFswSharePointsToWrongDirectoryException(share, server, currentdirectory, witnessdirectory))
		{
			this.share = share;
			this.server = server;
			this.currentdirectory = currentdirectory;
			this.witnessdirectory = witnessdirectory;
		}

		public DagFswSharePointsToWrongDirectoryException(string share, string server, string currentdirectory, string witnessdirectory, Exception innerException) : base(Strings.DagFswSharePointsToWrongDirectoryException(share, server, currentdirectory, witnessdirectory), innerException)
		{
			this.share = share;
			this.server = server;
			this.currentdirectory = currentdirectory;
			this.witnessdirectory = witnessdirectory;
		}

		protected DagFswSharePointsToWrongDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.share = (string)info.GetValue("share", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
			this.currentdirectory = (string)info.GetValue("currentdirectory", typeof(string));
			this.witnessdirectory = (string)info.GetValue("witnessdirectory", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("share", this.share);
			info.AddValue("server", this.server);
			info.AddValue("currentdirectory", this.currentdirectory);
			info.AddValue("witnessdirectory", this.witnessdirectory);
		}

		public string Share
		{
			get
			{
				return this.share;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Currentdirectory
		{
			get
			{
				return this.currentdirectory;
			}
		}

		public string Witnessdirectory
		{
			get
			{
				return this.witnessdirectory;
			}
		}

		private readonly string share;

		private readonly string server;

		private readonly string currentdirectory;

		private readonly string witnessdirectory;
	}
}
