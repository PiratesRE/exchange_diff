using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ResubmitRequestId : ObjectId
	{
		public ResubmitRequestId(long requestIdentity)
		{
			this.requestIdentity = requestIdentity;
		}

		public long ResubmitRequestRowId
		{
			get
			{
				return this.requestIdentity;
			}
		}

		public static ResubmitRequestId Parse(string identity)
		{
			return new ResubmitRequestId(long.Parse(identity));
		}

		public static bool TryParse(string identity, out ResubmitRequestId resultIdentity)
		{
			long num;
			if (long.TryParse(identity, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				resultIdentity = new ResubmitRequestId(num);
				return true;
			}
			resultIdentity = null;
			return false;
		}

		public override byte[] GetBytes()
		{
			return new byte[0];
		}

		public override string ToString()
		{
			return this.ResubmitRequestRowId.ToString(CultureInfo.InvariantCulture);
		}

		private readonly long requestIdentity;
	}
}
