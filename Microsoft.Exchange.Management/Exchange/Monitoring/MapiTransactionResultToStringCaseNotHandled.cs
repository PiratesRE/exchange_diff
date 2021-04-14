using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiTransactionResultToStringCaseNotHandled : LocalizedException
	{
		public MapiTransactionResultToStringCaseNotHandled(MapiTransactionResultEnum result) : base(Strings.MapiTransactionResultCaseNotHandled(result))
		{
			this.result = result;
		}

		public MapiTransactionResultToStringCaseNotHandled(MapiTransactionResultEnum result, Exception innerException) : base(Strings.MapiTransactionResultCaseNotHandled(result), innerException)
		{
			this.result = result;
		}

		protected MapiTransactionResultToStringCaseNotHandled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.result = (MapiTransactionResultEnum)info.GetValue("result", typeof(MapiTransactionResultEnum));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("result", this.result);
		}

		public MapiTransactionResultEnum Result
		{
			get
			{
				return this.result;
			}
		}

		private readonly MapiTransactionResultEnum result;
	}
}
