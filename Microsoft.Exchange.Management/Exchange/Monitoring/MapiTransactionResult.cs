using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MapiTransactionResult
	{
		public MapiTransactionResult()
		{
		}

		public MapiTransactionResult(MapiTransactionResultEnum result)
		{
			this.result = result;
		}

		public MapiTransactionResultEnum Value
		{
			get
			{
				return this.result;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			switch (this.result)
			{
			case MapiTransactionResultEnum.Undefined:
				text = Strings.MapiTransactionResultUndefined;
				break;
			case MapiTransactionResultEnum.Success:
				text = Strings.MapiTransactionResultSuccess;
				break;
			case MapiTransactionResultEnum.Failure:
				text = Strings.MapiTransactionResultFailure;
				break;
			case MapiTransactionResultEnum.MdbMoved:
				text = Strings.MapiTransactionResultMdbMoved;
				break;
			case MapiTransactionResultEnum.StoreNotRunning:
				text = Strings.MapiTransactionResultFailure;
				break;
			default:
				throw new MapiTransactionResultToStringCaseNotHandled(this.result);
			}
			return text;
		}

		private MapiTransactionResultEnum result;
	}
}
