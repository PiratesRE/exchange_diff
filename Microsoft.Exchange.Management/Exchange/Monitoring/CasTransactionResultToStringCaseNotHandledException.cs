using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasTransactionResultToStringCaseNotHandledException : LocalizedException
	{
		public CasTransactionResultToStringCaseNotHandledException(CasTransactionResultEnum result) : base(Strings.CasTransactionResultCaseNotHandled(result))
		{
			this.result = result;
		}

		public CasTransactionResultToStringCaseNotHandledException(CasTransactionResultEnum result, Exception innerException) : base(Strings.CasTransactionResultCaseNotHandled(result), innerException)
		{
			this.result = result;
		}

		protected CasTransactionResultToStringCaseNotHandledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.result = (CasTransactionResultEnum)info.GetValue("result", typeof(CasTransactionResultEnum));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("result", this.result);
		}

		public CasTransactionResultEnum Result
		{
			get
			{
				return this.result;
			}
		}

		private readonly CasTransactionResultEnum result;
	}
}
