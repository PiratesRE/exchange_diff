using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OfflineOperationTimedOutException : LocalizedException
	{
		public OfflineOperationTimedOutException(string objectName, int count, int secondsTimeout) : base(Strings.OfflineOperationTimedOutException(objectName, count, secondsTimeout))
		{
			this.objectName = objectName;
			this.count = count;
			this.secondsTimeout = secondsTimeout;
		}

		public OfflineOperationTimedOutException(string objectName, int count, int secondsTimeout, Exception innerException) : base(Strings.OfflineOperationTimedOutException(objectName, count, secondsTimeout), innerException)
		{
			this.objectName = objectName;
			this.count = count;
			this.secondsTimeout = secondsTimeout;
		}

		protected OfflineOperationTimedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectName = (string)info.GetValue("objectName", typeof(string));
			this.count = (int)info.GetValue("count", typeof(int));
			this.secondsTimeout = (int)info.GetValue("secondsTimeout", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectName", this.objectName);
			info.AddValue("count", this.count);
			info.AddValue("secondsTimeout", this.secondsTimeout);
		}

		public string ObjectName
		{
			get
			{
				return this.objectName;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public int SecondsTimeout
		{
			get
			{
				return this.secondsTimeout;
			}
		}

		private readonly string objectName;

		private readonly int count;

		private readonly int secondsTimeout;
	}
}
