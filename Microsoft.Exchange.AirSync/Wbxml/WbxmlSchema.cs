using System;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal abstract class WbxmlSchema
	{
		public WbxmlSchema()
		{
			throw new WbxmlException("WbxmlSchema should never be created.  Create one of the derived classes.");
		}

		protected WbxmlSchema(int airSyncVersion)
		{
			this.version = airSyncVersion;
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public abstract string GetName(int tag);

		public abstract string GetNameSpace(int tag);

		public abstract int GetTag(string nameSpace, string name);

		public abstract bool IsTagSecure(int tag);

		public abstract bool IsTagAnOpaqueBlob(int tag);

		private int version;
	}
}
