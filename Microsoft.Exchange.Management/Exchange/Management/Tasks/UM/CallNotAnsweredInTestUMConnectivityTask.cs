using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CallNotAnsweredInTestUMConnectivityTask : LocalizedException
	{
		public CallNotAnsweredInTestUMConnectivityTask(string timeout) : base(Strings.CallNotAnsweredInTestUMConnectivityTask(timeout))
		{
			this.timeout = timeout;
		}

		public CallNotAnsweredInTestUMConnectivityTask(string timeout, Exception innerException) : base(Strings.CallNotAnsweredInTestUMConnectivityTask(timeout), innerException)
		{
			this.timeout = timeout;
		}

		protected CallNotAnsweredInTestUMConnectivityTask(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.timeout = (string)info.GetValue("timeout", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("timeout", this.timeout);
		}

		public string Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		private readonly string timeout;
	}
}
