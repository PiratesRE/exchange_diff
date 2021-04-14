using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveIdConnectivityWCFExceptionThrown : LocalizedException
	{
		public LiveIdConnectivityWCFExceptionThrown(string e) : base(Strings.messageLiveIdConnectivityWCFExceptionThrown(e))
		{
			this.e = e;
		}

		public LiveIdConnectivityWCFExceptionThrown(string e, Exception innerException) : base(Strings.messageLiveIdConnectivityWCFExceptionThrown(e), innerException)
		{
			this.e = e;
		}

		protected LiveIdConnectivityWCFExceptionThrown(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.e = (string)info.GetValue("e", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("e", this.e);
		}

		public string E
		{
			get
			{
				return this.e;
			}
		}

		private readonly string e;
	}
}
