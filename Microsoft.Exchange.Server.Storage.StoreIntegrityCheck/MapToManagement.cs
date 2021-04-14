using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class MapToManagement : Attribute
	{
		public MapToManagement(string mapName = null, bool skip = false)
		{
			this.skip = skip;
			this.mapName = (mapName ?? string.Empty);
		}

		public bool Skip
		{
			get
			{
				return this.skip;
			}
		}

		public string MapName
		{
			get
			{
				return this.mapName;
			}
		}

		private readonly bool skip;

		private readonly string mapName;
	}
}
