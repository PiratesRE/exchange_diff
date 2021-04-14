using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_RSTMAP : IContentEquatable<JET_RSTMAP>, IDeepCloneable<JET_RSTMAP>
	{
		public string szDatabaseName
		{
			[DebuggerStepThrough]
			get
			{
				return this.databaseName;
			}
			set
			{
				this.databaseName = value;
			}
		}

		public string szNewDatabaseName
		{
			[DebuggerStepThrough]
			get
			{
				return this.newDatabaseName;
			}
			set
			{
				this.newDatabaseName = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_RSTINFO(szDatabaseName={0},szNewDatabaseName={1})", new object[]
			{
				this.szDatabaseName,
				this.szNewDatabaseName
			});
		}

		public bool ContentEquals(JET_RSTMAP other)
		{
			return other != null && string.Equals(this.szDatabaseName, other.szDatabaseName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.szNewDatabaseName, other.szNewDatabaseName, StringComparison.OrdinalIgnoreCase);
		}

		public JET_RSTMAP DeepClone()
		{
			return (JET_RSTMAP)base.MemberwiseClone();
		}

		internal NATIVE_RSTMAP GetNativeRstmap()
		{
			return new NATIVE_RSTMAP
			{
				szDatabaseName = LibraryHelpers.MarshalStringToHGlobalUni(this.szDatabaseName),
				szNewDatabaseName = LibraryHelpers.MarshalStringToHGlobalUni(this.szNewDatabaseName)
			};
		}

		private string databaseName;

		private string newDatabaseName;
	}
}
