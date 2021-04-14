using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDurationException : LocalizedException
	{
		public InvalidDurationException(string minDuration, string maxDuration) : base(Strings.InvalidDuration(minDuration, maxDuration))
		{
			this.minDuration = minDuration;
			this.maxDuration = maxDuration;
		}

		public InvalidDurationException(string minDuration, string maxDuration, Exception innerException) : base(Strings.InvalidDuration(minDuration, maxDuration), innerException)
		{
			this.minDuration = minDuration;
			this.maxDuration = maxDuration;
		}

		protected InvalidDurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.minDuration = (string)info.GetValue("minDuration", typeof(string));
			this.maxDuration = (string)info.GetValue("maxDuration", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("minDuration", this.minDuration);
			info.AddValue("maxDuration", this.maxDuration);
		}

		public string MinDuration
		{
			get
			{
				return this.minDuration;
			}
		}

		public string MaxDuration
		{
			get
			{
				return this.maxDuration;
			}
		}

		private readonly string minDuration;

		private readonly string maxDuration;
	}
}
