using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringCouldNotFindDagException : MonitoringADConfigException
	{
		public MonitoringCouldNotFindDagException(string dagName, string adError) : base(ReplayStrings.MonitoringCouldNotFindDagException(dagName, adError))
		{
			this.dagName = dagName;
			this.adError = adError;
		}

		public MonitoringCouldNotFindDagException(string dagName, string adError, Exception innerException) : base(ReplayStrings.MonitoringCouldNotFindDagException(dagName, adError), innerException)
		{
			this.dagName = dagName;
			this.adError = adError;
		}

		protected MonitoringCouldNotFindDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.adError = (string)info.GetValue("adError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
			info.AddValue("adError", this.adError);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		public string AdError
		{
			get
			{
				return this.adError;
			}
		}

		private readonly string dagName;

		private readonly string adError;
	}
}
