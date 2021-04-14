using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal class OpenMessageInstance
	{
		public OpenMessageInstance(OpenMessageStates.OpenMessageState state, DataRow dataRow)
		{
			this.state = state;
			this.dataRow = dataRow;
			this.referenced = true;
			this.current = true;
			this.tentative = false;
		}

		public bool Referenced
		{
			get
			{
				return this.referenced;
			}
		}

		public bool Current
		{
			get
			{
				return this.current;
			}
			internal set
			{
				this.current = value;
			}
		}

		public bool Tentative
		{
			get
			{
				return this.tentative;
			}
			internal set
			{
				this.tentative = value;
			}
		}

		public bool Moved
		{
			get
			{
				return this.moved;
			}
			internal set
			{
				this.moved = value;
			}
		}

		public bool Deleted
		{
			get
			{
				return this.deleted;
			}
			internal set
			{
				this.deleted = value;
			}
		}

		public DataRow DataRow
		{
			get
			{
				return this.dataRow;
			}
		}

		public OpenMessageStates.OpenMessageState State
		{
			get
			{
				return this.state;
			}
		}

		public void MarkUnreferenced()
		{
			this.referenced = false;
		}

		private OpenMessageStates.OpenMessageState state;

		private DataRow dataRow;

		private bool referenced;

		private bool current;

		private bool tentative;

		private bool moved;

		private bool deleted;
	}
}
