using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeTracingProviderInstallException : LocalizedException
	{
		public ExchangeTracingProviderInstallException(int errorHresult) : base(Strings.ExchangeTracingProviderInstallFailed(errorHresult))
		{
			this.errorHresult = errorHresult;
		}

		public ExchangeTracingProviderInstallException(int errorHresult, Exception innerException) : base(Strings.ExchangeTracingProviderInstallFailed(errorHresult), innerException)
		{
			this.errorHresult = errorHresult;
		}

		protected ExchangeTracingProviderInstallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorHresult = (int)info.GetValue("errorHresult", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorHresult", this.errorHresult);
		}

		public int ErrorHresult
		{
			get
			{
				return this.errorHresult;
			}
		}

		private readonly int errorHresult;
	}
}
