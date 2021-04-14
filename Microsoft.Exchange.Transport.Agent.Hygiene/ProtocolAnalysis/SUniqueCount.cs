using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport.Agent.Hygiene;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	[Serializable]
	internal class SUniqueCount
	{
		public SUniqueCount()
		{
			this.tbl = new ushort[SUniqueCount.nTBL];
			this.Reset();
		}

		public void Merge(SUniqueCount source)
		{
			for (int i = 0; i < source.entries; i++)
			{
				this.AddHash(source.tbl[i]);
			}
		}

		public void Reset()
		{
			Array.Clear(this.tbl, 0, this.tbl.Length);
			this.entries = 0;
		}

		public void AddItem(string itemName)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(itemName);
			ushort ushort16HashCode = PrivateHashAlgorithm1.GetUShort16HashCode(bytes);
			this.AddHash(ushort16HashCode);
		}

		public int Count()
		{
			return this.entries;
		}

		private void AddHash(ushort h)
		{
			bool flag = false;
			int num = -1;
			if (this.entries == this.tbl.Length)
			{
				flag = true;
			}
			else if (this.entries == 0)
			{
				num = 0;
			}
			else
			{
				int num2 = 0;
				int num3 = this.entries - 1;
				while (!flag && num == -1)
				{
					if (h < this.tbl[num2])
					{
						num = num2;
					}
					else if (h > this.tbl[num3])
					{
						num = num3 + 1;
					}
					else if (h == this.tbl[num2] || h == this.tbl[num3])
					{
						flag = true;
					}
					else
					{
						int num4 = (num2 + num3) / 2;
						if (h > this.tbl[num4])
						{
							if (num4 != num2)
							{
								num2 = num4;
							}
							else
							{
								num = num2 + 1;
							}
						}
						else if (h < this.tbl[num4])
						{
							if (num4 != num3)
							{
								num3 = num4;
							}
							else
							{
								num = num3;
							}
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			if (!flag && num == -1)
			{
				throw new LocalizedException(AgentStrings.FailedToFindInsertionPoint);
			}
			if (!flag && this.entries < this.tbl.Length)
			{
				for (int i = this.entries - 1; i >= num; i--)
				{
					this.tbl[i + 1] = this.tbl[i];
				}
				this.tbl[num] = h;
				this.entries++;
			}
		}

		private static int nTBL = 200;

		private ushort[] tbl;

		private int entries;
	}
}
