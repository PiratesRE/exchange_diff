using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.HA
{
	public struct MINSTANCE_BACKUP_INFO
	{
		public JET_INSTANCE Instance
		{
			get
			{
				return this.<backing_store>Instance;
			}
			set
			{
				this.<backing_store>Instance = value;
			}
		}

		public string Name
		{
			get
			{
				return this.<backing_store>Name;
			}
			set
			{
				this.<backing_store>Name = value;
			}
		}

		public MDATABASE_BACKUP_INFO[] Databases
		{
			get
			{
				return this.<backing_store>Databases;
			}
			set
			{
				this.<backing_store>Databases = value;
			}
		}

		private JET_INSTANCE <backing_store>Instance;

		private string <backing_store>Name;

		private MDATABASE_BACKUP_INFO[] <backing_store>Databases;
	}
}
