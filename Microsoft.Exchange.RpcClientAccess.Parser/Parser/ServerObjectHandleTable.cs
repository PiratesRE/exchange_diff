using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ServerObjectHandleTable
	{
		public int LastIndex
		{
			get
			{
				return this.serverObjectHandleList.Count - 1;
			}
		}

		public int HighestIndexAccessed
		{
			get
			{
				return this.highestIndexAccessed;
			}
			set
			{
				this.highestIndexAccessed = value;
			}
		}

		public ServerObjectHandle this[int key]
		{
			get
			{
				if (key > this.LastIndex || key < 0)
				{
					throw new BufferParseException("Invalid ServerObjectHandleTable Index");
				}
				this.AccessIndex(key);
				return this.serverObjectHandleList[key];
			}
			set
			{
				if (key > this.LastIndex || key < 0)
				{
					throw new BufferParseException("Invalid ServerObjectHandleTable Index");
				}
				this.AccessIndex(key);
				this.serverObjectHandleList[key] = value;
			}
		}

		public ServerObjectHandleTable()
		{
			this.serverObjectHandleList = new List<ServerObjectHandle>(255);
			for (int i = 0; i < 255; i++)
			{
				this.serverObjectHandleList.Add(ServerObjectHandle.None);
			}
			this.highestIndexAccessed = -1;
		}

		private ServerObjectHandleTable(Reader reader)
		{
			int num = (int)(reader.Length - reader.Position);
			int num2 = num / 4;
			if (num2 > 256)
			{
				num2 = 256;
			}
			this.serverObjectHandleList = new List<ServerObjectHandle>(num2);
			for (int i = 0; i < num2; i++)
			{
				this.serverObjectHandleList.Add(ServerObjectHandle.Parse(reader));
			}
			this.highestIndexAccessed = -1;
		}

		public static ServerObjectHandleTable Parse(Reader reader)
		{
			return new ServerObjectHandleTable(reader);
		}

		internal void Serialize(Writer writer)
		{
			for (int i = 0; i <= this.highestIndexAccessed; i++)
			{
				this.serverObjectHandleList[i].Serialize(writer);
			}
		}

		public void MarkLastHandle()
		{
			this.highestIndexAccessed = this.LastIndex;
		}

		public void AccessIndex(int index)
		{
			if (index > this.LastIndex)
			{
				throw new BufferParseException(string.Format("Invalid handle table index. Index = {0}. LastIndex = {1}.", index, this.LastIndex));
			}
			if (index > this.highestIndexAccessed)
			{
				this.highestIndexAccessed = index;
			}
		}

		private IList<ServerObjectHandle> serverObjectHandleList;

		private int highestIndexAccessed;
	}
}
