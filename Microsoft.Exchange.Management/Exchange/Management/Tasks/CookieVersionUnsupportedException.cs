using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CookieVersionUnsupportedException : LocalizedException
	{
		public CookieVersionUnsupportedException(int version) : base(Strings.CookieVersionUnsupportedException(version))
		{
			this.version = version;
		}

		public CookieVersionUnsupportedException(int version, Exception innerException) : base(Strings.CookieVersionUnsupportedException(version), innerException)
		{
			this.version = version;
		}

		protected CookieVersionUnsupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.version = (int)info.GetValue("version", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("version", this.version);
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		private readonly int version;
	}
}
