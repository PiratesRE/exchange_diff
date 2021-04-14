using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class VersionControl
	{
		internal VersionControl(bool isCheckedOut, string checkedOutTo, int versionId)
		{
			this.isCheckedOut = isCheckedOut;
			this.checkedOutTo = checkedOutTo;
			this.versionId = versionId;
		}

		public override bool Equals(object obj)
		{
			VersionControl versionControl = obj as VersionControl;
			return versionControl != null && (this.CheckedOutTo == versionControl.CheckedOutTo && this.IsCheckedOut == versionControl.IsCheckedOut) && this.TipVersion == versionControl.TipVersion;
		}

		public override int GetHashCode()
		{
			return this.CheckedOutTo.GetHashCode() + this.TipVersion.GetHashCode();
		}

		public bool IsCheckedOut
		{
			get
			{
				return this.isCheckedOut;
			}
		}

		public string CheckedOutTo
		{
			get
			{
				return this.checkedOutTo;
			}
		}

		public int TipVersion
		{
			get
			{
				return this.versionId;
			}
		}

		private readonly bool isCheckedOut;

		private readonly string checkedOutTo;

		private readonly int versionId;
	}
}
