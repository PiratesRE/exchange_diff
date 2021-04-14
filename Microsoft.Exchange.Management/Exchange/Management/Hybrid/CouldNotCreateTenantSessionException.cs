using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotCreateTenantSessionException : LocalizedException
	{
		public CouldNotCreateTenantSessionException(Exception e) : base(HybridStrings.HybridCouldNotCreateTenantSessionException(e))
		{
			this.e = e;
		}

		public CouldNotCreateTenantSessionException(Exception e, Exception innerException) : base(HybridStrings.HybridCouldNotCreateTenantSessionException(e), innerException)
		{
			this.e = e;
		}

		protected CouldNotCreateTenantSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.e = (Exception)info.GetValue("e", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("e", this.e);
		}

		public Exception E
		{
			get
			{
				return this.e;
			}
		}

		private readonly Exception e;
	}
}
