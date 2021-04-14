using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotOpenRunspaceException : LocalizedException
	{
		public CouldNotOpenRunspaceException(Exception e) : base(HybridStrings.HybridCouldNotOpenRunspaceException(e))
		{
			this.e = e;
		}

		public CouldNotOpenRunspaceException(Exception e, Exception innerException) : base(HybridStrings.HybridCouldNotOpenRunspaceException(e), innerException)
		{
			this.e = e;
		}

		protected CouldNotOpenRunspaceException(SerializationInfo info, StreamingContext context) : base(info, context)
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
