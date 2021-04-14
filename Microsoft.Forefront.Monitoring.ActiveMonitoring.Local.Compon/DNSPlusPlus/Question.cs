using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class Question
	{
		public int RequestID { get; private set; }

		public RecordType QueryType { get; private set; }

		public RecordClass QueryClass { get; private set; }

		public string Domain { get; private set; }

		public int ProcessResponse(byte[] message, int position)
		{
			int num = position;
			this.Domain = DnsHelper.ReadDomain(message, ref num);
			this.QueryType = (RecordType)DnsHelper.GetUShort(message, num);
			num += 2;
			this.QueryClass = (RecordClass)DnsHelper.GetUShort(message, num);
			num += 2;
			return num;
		}

		public byte[] GetQueryBytes(int id, string queryName, RecordType queryType, RecordClass queryClass)
		{
			int num = 18 + queryName.Length;
			byte[] array = new byte[num];
			int num2 = 0;
			array[num2++] = (byte)(id >> 8);
			array[num2++] = (byte)(id & 255);
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = 1;
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = 0;
			string[] array2 = queryName.Trim().Split(new char[]
			{
				'.'
			});
			foreach (string text in array2)
			{
				array[num2++] = (byte)text.Length;
				foreach (char c in text)
				{
					array[num2++] = (byte)c;
				}
			}
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = (byte)queryType;
			array[num2++] = 0;
			array[num2++] = (byte)queryClass;
			this.RequestID = id;
			this.QueryType = queryType;
			this.QueryClass = queryClass;
			this.Domain = queryName;
			return array;
		}
	}
}
