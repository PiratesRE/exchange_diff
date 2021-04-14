using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringCouldNotFindHubServersException : MonitoringADConfigException
	{
		public MonitoringCouldNotFindHubServersException(string siteName, string adError) : base(ReplayStrings.MonitoringCouldNotFindHubServersException(siteName, adError))
		{
			this.siteName = siteName;
			this.adError = adError;
		}

		public MonitoringCouldNotFindHubServersException(string siteName, string adError, Exception innerException) : base(ReplayStrings.MonitoringCouldNotFindHubServersException(siteName, adError), innerException)
		{
			this.siteName = siteName;
			this.adError = adError;
		}

		protected MonitoringCouldNotFindHubServersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.siteName = (string)info.GetValue("siteName", typeof(string));
			this.adError = (string)info.GetValue("adError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("siteName", this.siteName);
			info.AddValue("adError", this.adError);
		}

		public string SiteName
		{
			get
			{
				return this.siteName;
			}
		}

		public string AdError
		{
			get
			{
				return this.adError;
			}
		}

		private readonly string siteName;

		private readonly string adError;
	}
}
