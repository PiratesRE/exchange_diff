using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotCreateOnPremisesSessionException : LocalizedException
	{
		public CouldNotCreateOnPremisesSessionException(Exception e) : base(HybridStrings.HybridCouldNotCreateOnPremisesSessionException(e))
		{
			this.e = e;
		}

		public CouldNotCreateOnPremisesSessionException(Exception e, Exception innerException) : base(HybridStrings.HybridCouldNotCreateOnPremisesSessionException(e), innerException)
		{
			this.e = e;
		}

		protected CouldNotCreateOnPremisesSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
