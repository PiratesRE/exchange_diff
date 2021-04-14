using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PropValuesDataContext : DataContext
	{
		public PropValuesDataContext(PropValueData[][] pvdaa)
		{
			this.pvdaa = pvdaa;
		}

		public PropValuesDataContext(PropValueData[] pvda)
		{
			this.pvdaa = new PropValueData[][]
			{
				pvda
			};
		}

		public override string ToString()
		{
			string arg;
			if (this.pvdaa == null || this.pvdaa.Length == 0)
			{
				arg = string.Empty;
			}
			else if (this.pvdaa.Length == 1)
			{
				arg = CommonUtils.ConcatEntries<PropValueData>(this.pvdaa[0], null);
			}
			else
			{
				arg = CommonUtils.ConcatEntries<PropValueData[]>(this.pvdaa, (PropValueData[] pvda) => CommonUtils.ConcatEntries<PropValueData>(this.pvdaa[0], null));
			}
			return string.Format("PropValues: {0}", arg);
		}

		private PropValueData[][] pvdaa;
	}
}
