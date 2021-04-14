using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidPerfCounterException : LocalizedException
	{
		public InvalidPerfCounterException(string counterName) : base(Strings.InvalidPerfCounterException(counterName))
		{
			this.counterName = counterName;
		}

		public InvalidPerfCounterException(string counterName, Exception innerException) : base(Strings.InvalidPerfCounterException(counterName), innerException)
		{
			this.counterName = counterName;
		}

		protected InvalidPerfCounterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.counterName = (string)info.GetValue("counterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("counterName", this.counterName);
		}

		public string CounterName
		{
			get
			{
				return this.counterName;
			}
		}

		private readonly string counterName;
	}
}
