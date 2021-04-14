using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DelegatedServerErrorException : LocalizedException
	{
		public DelegatedServerErrorException(Exception ex) : base(Strings.DelegatedServerErrorException(ex))
		{
			this.ex = ex;
		}

		public DelegatedServerErrorException(Exception ex, Exception innerException) : base(Strings.DelegatedServerErrorException(ex), innerException)
		{
			this.ex = ex;
		}

		protected DelegatedServerErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ex", this.ex);
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly Exception ex;
	}
}
