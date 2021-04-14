using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class LogTransactionInformationCollector
	{
		public void AddLogTransactionInformationBlock(ILogTransactionInformation logTransactionInformationBlock)
		{
			bool flag = false;
			switch (logTransactionInformationBlock.Type())
			{
			case 2:
				if (this.logTransactionInformationIdentityBlock == null)
				{
					this.logTransactionInformationIdentityBlock = logTransactionInformationBlock;
				}
				break;
			case 3:
				if (this.logTransactionInformationOperationBlock == null)
				{
					this.logTransactionInformationOperationBlock = logTransactionInformationBlock;
				}
				break;
			case 4:
				if (this.logTransactionInformationOperationBlock == null)
				{
					this.logTransactionInformationOperationBlock = logTransactionInformationBlock;
				}
				break;
			case 5:
				if (this.logTransactionInformationOperationBlock == null)
				{
					this.logTransactionInformationOperationBlock = logTransactionInformationBlock;
				}
				break;
			default:
			{
				LogTransactionInformationCollector.Counter counter;
				if (!this.perLogTransactionInformationBlockTypeCounter.TryGetValue(logTransactionInformationBlock.Type(), out counter))
				{
					counter = new LogTransactionInformationCollector.Counter
					{
						Value = 1
					};
					this.perLogTransactionInformationBlockTypeCounter.Add(logTransactionInformationBlock.Type(), counter);
				}
				else
				{
					counter.Value++;
				}
				flag = true;
				break;
			}
			}
			int num = logTransactionInformationBlock.Serialize(null, 0);
			if (LogTransactionInformationCollector.AvailableBufferLength <= this.usedBufferLength + num)
			{
				flag = false;
				this.shouldComputeDigest = true;
			}
			else
			{
				this.usedBufferLength += num;
			}
			if (flag)
			{
				this.logTransactionInformationList.AddLast(logTransactionInformationBlock);
			}
		}

		public int Serialize(byte[] buffer, int offset)
		{
			int num = offset;
			if (buffer != null)
			{
				buffer[offset] = LogTransactionInformationCollector.Version;
			}
			offset++;
			if (this.shouldComputeDigest)
			{
				int num2 = offset;
				if (this.logTransactionInformationIdentityBlock != null)
				{
					num2 += this.logTransactionInformationIdentityBlock.Serialize(null, num2);
					if (num2 > LogTransactionInformationCollector.AvailableBufferLength)
					{
						return offset - num;
					}
					if (buffer != null)
					{
						offset += this.logTransactionInformationIdentityBlock.Serialize(buffer, offset);
					}
					else
					{
						offset = num2;
					}
				}
				if (this.logTransactionInformationOperationBlock != null)
				{
					num2 += this.logTransactionInformationOperationBlock.Serialize(null, num2);
					if (num2 > LogTransactionInformationCollector.AvailableBufferLength)
					{
						return offset - num;
					}
					if (buffer != null)
					{
						offset += this.logTransactionInformationOperationBlock.Serialize(buffer, offset);
					}
					else
					{
						offset = num2;
					}
				}
				ILogTransactionInformation logTransactionInformation = new LogTransactionInformationDigest(this.perLogTransactionInformationBlockTypeCounter);
				num2 += logTransactionInformation.Serialize(null, num2);
				if (num2 > LogTransactionInformationCollector.AvailableBufferLength)
				{
					return offset - num;
				}
				if (buffer != null)
				{
					offset += logTransactionInformation.Serialize(buffer, offset);
				}
				else
				{
					offset = num2;
				}
				using (LinkedList<ILogTransactionInformation>.Enumerator enumerator = this.logTransactionInformationList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ILogTransactionInformation logTransactionInformation2 = enumerator.Current;
						num2 += logTransactionInformation2.Serialize(null, num2);
						if (num2 > LogTransactionInformationCollector.AvailableBufferLength)
						{
							return offset - num;
						}
						if (buffer != null)
						{
							offset += logTransactionInformation2.Serialize(buffer, offset);
						}
						else
						{
							offset = num2;
						}
					}
					goto IL_1A1;
				}
			}
			if (this.logTransactionInformationIdentityBlock != null)
			{
				offset += this.logTransactionInformationIdentityBlock.Serialize(buffer, offset);
			}
			if (this.logTransactionInformationOperationBlock != null)
			{
				offset += this.logTransactionInformationOperationBlock.Serialize(buffer, offset);
			}
			foreach (ILogTransactionInformation logTransactionInformation3 in this.logTransactionInformationList)
			{
				offset += logTransactionInformation3.Serialize(buffer, offset);
			}
			IL_1A1:
			return offset - num;
		}

		public static readonly byte Version = 0;

		public static readonly int AvailableBufferLength = 70;

		private bool shouldComputeDigest;

		private ILogTransactionInformation logTransactionInformationIdentityBlock;

		private ILogTransactionInformation logTransactionInformationOperationBlock;

		private LinkedList<ILogTransactionInformation> logTransactionInformationList = new LinkedList<ILogTransactionInformation>();

		private int usedBufferLength;

		private Dictionary<byte, LogTransactionInformationCollector.Counter> perLogTransactionInformationBlockTypeCounter = new Dictionary<byte, LogTransactionInformationCollector.Counter>();

		internal class Counter
		{
			public int Value { get; set; }
		}
	}
}
