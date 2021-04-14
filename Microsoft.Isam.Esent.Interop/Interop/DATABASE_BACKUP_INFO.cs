using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
	public class DATABASE_BACKUP_INFO
	{
		public string wszDatabaseDisplayName { get; set; }

		public string[] wszDatabaseStreams { get; set; }

		public Guid guidDatabase { get; set; }

		public int ulIconIndexDatabase { get; set; }

		public DatabaseBackupInfoFlags fDatabaseFlags { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "DATABASE_BACKUP_INFO({0}:{1}:{2})", new object[]
			{
				this.wszDatabaseDisplayName,
				(this.wszDatabaseStreams == null) ? string.Empty : this.wszDatabaseStreams[0],
				this.fDatabaseFlags
			});
		}

		internal NATIVE_DATABASE_BACKUP_INFO GetNativeDatabaseBackupInfo()
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			for (int i = 0; i < this.wszDatabaseStreams.Length; i++)
			{
				stringBuilder.Append(this.wszDatabaseStreams[i]);
				stringBuilder.Append('\0');
			}
			stringBuilder.Append('\0');
			string text = stringBuilder.ToString();
			return new NATIVE_DATABASE_BACKUP_INFO
			{
				cwDatabaseStreams = (uint)(text.Length + 1),
				fDatabaseFlags = (uint)this.fDatabaseFlags,
				guidDatabase = this.guidDatabase,
				ulIconIndexDatabase = (uint)this.ulIconIndexDatabase,
				wszDatabaseDisplayName = Marshal.StringToHGlobalUni(this.wszDatabaseDisplayName),
				wszDatabaseStreams = Marshal.StringToHGlobalUni(text)
			};
		}
	}
}
