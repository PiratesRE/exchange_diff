using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_INSTANCE_INFO : IEquatable<JET_INSTANCE_INFO>
	{
		internal JET_INSTANCE_INFO()
		{
		}

		internal JET_INSTANCE_INFO(JET_INSTANCE instance, string instanceName, string[] databases)
		{
			this.hInstanceId = instance;
			this.szInstanceName = instanceName;
			if (databases == null)
			{
				this.cDatabases = 0;
				this.databases = null;
				return;
			}
			this.cDatabases = databases.Length;
			this.databases = new ReadOnlyCollection<string>(databases);
		}

		public JET_INSTANCE hInstanceId { get; private set; }

		public string szInstanceName { get; private set; }

		public int cDatabases { get; private set; }

		public IList<string> szDatabaseFileName
		{
			get
			{
				return this.databases;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_INSTANCE_INFO)obj);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INSTANCE_INFO({0})", new object[]
			{
				this.szInstanceName
			});
		}

		public override int GetHashCode()
		{
			int num = this.hInstanceId.GetHashCode() ^ (this.szInstanceName ?? string.Empty).GetHashCode() ^ this.cDatabases << 20;
			for (int i = 0; i < this.cDatabases; i++)
			{
				num ^= this.szDatabaseFileName[i].GetHashCode();
			}
			return num;
		}

		public bool Equals(JET_INSTANCE_INFO other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.hInstanceId != other.hInstanceId || this.szInstanceName != other.szInstanceName || this.cDatabases != other.cDatabases)
			{
				return false;
			}
			for (int i = 0; i < this.cDatabases; i++)
			{
				if (this.szDatabaseFileName[i] != other.szDatabaseFileName[i])
				{
					return false;
				}
			}
			return true;
		}

		internal unsafe void SetFromNativeAscii(NATIVE_INSTANCE_INFO native)
		{
			this.hInstanceId = new JET_INSTANCE
			{
				Value = native.hInstanceId
			};
			this.szInstanceName = Marshal.PtrToStringAnsi(native.szInstanceName);
			this.cDatabases = (int)native.cDatabases;
			string[] array = new string[this.cDatabases];
			for (int i = 0; i < this.cDatabases; i++)
			{
				array[i] = Marshal.PtrToStringAnsi(native.szDatabaseFileName[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
			}
			this.databases = new ReadOnlyCollection<string>(array);
		}

		internal unsafe void SetFromNativeUnicode(NATIVE_INSTANCE_INFO native)
		{
			this.hInstanceId = new JET_INSTANCE
			{
				Value = native.hInstanceId
			};
			this.szInstanceName = Marshal.PtrToStringUni(native.szInstanceName);
			this.cDatabases = (int)native.cDatabases;
			string[] array = new string[this.cDatabases];
			for (int i = 0; i < this.cDatabases; i++)
			{
				array[i] = Marshal.PtrToStringUni(native.szDatabaseFileName[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
			}
			this.databases = new ReadOnlyCollection<string>(array);
		}

		private ReadOnlyCollection<string> databases;
	}
}
