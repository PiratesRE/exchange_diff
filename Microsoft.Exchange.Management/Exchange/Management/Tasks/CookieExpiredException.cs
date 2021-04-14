using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CookieExpiredException : LocalizedException
	{
		public CookieExpiredException(Guid oldDc, Guid newDc) : base(Strings.CookieExpiredException(oldDc, newDc))
		{
			this.oldDc = oldDc;
			this.newDc = newDc;
		}

		public CookieExpiredException(Guid oldDc, Guid newDc, Exception innerException) : base(Strings.CookieExpiredException(oldDc, newDc), innerException)
		{
			this.oldDc = oldDc;
			this.newDc = newDc;
		}

		protected CookieExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.oldDc = (Guid)info.GetValue("oldDc", typeof(Guid));
			this.newDc = (Guid)info.GetValue("newDc", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("oldDc", this.oldDc);
			info.AddValue("newDc", this.newDc);
		}

		public Guid OldDc
		{
			get
			{
				return this.oldDc;
			}
		}

		public Guid NewDc
		{
			get
			{
				return this.newDc;
			}
		}

		private readonly Guid oldDc;

		private readonly Guid newDc;
	}
}
