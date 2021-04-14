using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidCrossTenantIdFormatException : LocalizedException
	{
		public InvalidCrossTenantIdFormatException(string str) : base(DirectoryStrings.InvalidCrossTenantIdFormat(str))
		{
			this.str = str;
		}

		public InvalidCrossTenantIdFormatException(string str, Exception innerException) : base(DirectoryStrings.InvalidCrossTenantIdFormat(str), innerException)
		{
			this.str = str;
		}

		protected InvalidCrossTenantIdFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.str = (string)info.GetValue("str", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("str", this.str);
		}

		public string Str
		{
			get
			{
				return this.str;
			}
		}

		private readonly string str;
	}
}
