using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTimeZoneException : LocalizedException
	{
		public InvalidTimeZoneException(string tzKeyName) : base(Strings.InvalidTimeZone(tzKeyName))
		{
			this.tzKeyName = tzKeyName;
		}

		public InvalidTimeZoneException(string tzKeyName, Exception innerException) : base(Strings.InvalidTimeZone(tzKeyName), innerException)
		{
			this.tzKeyName = tzKeyName;
		}

		protected InvalidTimeZoneException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tzKeyName = (string)info.GetValue("tzKeyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tzKeyName", this.tzKeyName);
		}

		public string TzKeyName
		{
			get
			{
				return this.tzKeyName;
			}
		}

		private readonly string tzKeyName;
	}
}
