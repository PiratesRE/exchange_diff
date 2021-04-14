using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbRemountSkippedSinceMasterChanged : AmDbActionException
	{
		public AmDbRemountSkippedSinceMasterChanged(string dbName, string currentActive, string prevActive) : base(ReplayStrings.AmDbRemountSkippedSinceMasterChanged(dbName, currentActive, prevActive))
		{
			this.dbName = dbName;
			this.currentActive = currentActive;
			this.prevActive = prevActive;
		}

		public AmDbRemountSkippedSinceMasterChanged(string dbName, string currentActive, string prevActive, Exception innerException) : base(ReplayStrings.AmDbRemountSkippedSinceMasterChanged(dbName, currentActive, prevActive), innerException)
		{
			this.dbName = dbName;
			this.currentActive = currentActive;
			this.prevActive = prevActive;
		}

		protected AmDbRemountSkippedSinceMasterChanged(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.currentActive = (string)info.GetValue("currentActive", typeof(string));
			this.prevActive = (string)info.GetValue("prevActive", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("currentActive", this.currentActive);
			info.AddValue("prevActive", this.prevActive);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string CurrentActive
		{
			get
			{
				return this.currentActive;
			}
		}

		public string PrevActive
		{
			get
			{
				return this.prevActive;
			}
		}

		private readonly string dbName;

		private readonly string currentActive;

		private readonly string prevActive;
	}
}
