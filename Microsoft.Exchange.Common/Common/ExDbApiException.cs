using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExDbApiException : LocalizedException
	{
		public ExDbApiException(Win32Exception ex) : base(CommonStrings.ExDbApiException(ex))
		{
			this.ex = ex;
		}

		public ExDbApiException(Win32Exception ex, Exception innerException) : base(CommonStrings.ExDbApiException(ex), innerException)
		{
			this.ex = ex;
		}

		protected ExDbApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ex = (Win32Exception)info.GetValue("ex", typeof(Win32Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ex", this.ex);
		}

		public Win32Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly Win32Exception ex;
	}
}
