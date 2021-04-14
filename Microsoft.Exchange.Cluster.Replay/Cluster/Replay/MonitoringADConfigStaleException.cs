using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringADConfigStaleException : MonitoringADConfigException
	{
		public MonitoringADConfigStaleException(string age, string maxTTL, string lastError) : base(ReplayStrings.MonitoringADConfigStaleException(age, maxTTL, lastError))
		{
			this.age = age;
			this.maxTTL = maxTTL;
			this.lastError = lastError;
		}

		public MonitoringADConfigStaleException(string age, string maxTTL, string lastError, Exception innerException) : base(ReplayStrings.MonitoringADConfigStaleException(age, maxTTL, lastError), innerException)
		{
			this.age = age;
			this.maxTTL = maxTTL;
			this.lastError = lastError;
		}

		protected MonitoringADConfigStaleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.age = (string)info.GetValue("age", typeof(string));
			this.maxTTL = (string)info.GetValue("maxTTL", typeof(string));
			this.lastError = (string)info.GetValue("lastError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("age", this.age);
			info.AddValue("maxTTL", this.maxTTL);
			info.AddValue("lastError", this.lastError);
		}

		public string Age
		{
			get
			{
				return this.age;
			}
		}

		public string MaxTTL
		{
			get
			{
				return this.maxTTL;
			}
		}

		public string LastError
		{
			get
			{
				return this.lastError;
			}
		}

		private readonly string age;

		private readonly string maxTTL;

		private readonly string lastError;
	}
}
