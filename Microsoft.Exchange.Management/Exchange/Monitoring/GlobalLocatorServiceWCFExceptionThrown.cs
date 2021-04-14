using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GlobalLocatorServiceWCFExceptionThrown : LocalizedException
	{
		public GlobalLocatorServiceWCFExceptionThrown(string e) : base(Strings.messageGlobalLocatorServiceWCFExceptionThrown(e))
		{
			this.e = e;
		}

		public GlobalLocatorServiceWCFExceptionThrown(string e, Exception innerException) : base(Strings.messageGlobalLocatorServiceWCFExceptionThrown(e), innerException)
		{
			this.e = e;
		}

		protected GlobalLocatorServiceWCFExceptionThrown(SerializationInfo info, StreamingContext context) : base(info, context)
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
