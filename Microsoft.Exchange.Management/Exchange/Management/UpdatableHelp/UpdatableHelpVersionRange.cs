using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	internal class UpdatableHelpVersionRange
	{
		internal UpdatableHelpVersionRange(string versionRange, int revision, string culturesUpdated, string cabinetUrl)
		{
			string[] array = versionRange.Split(new char[]
			{
				'-'
			});
			if (array == null || array.Length < 1 || array.Length > 2)
			{
				throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInvalidVersionNumberErrorID, UpdatableHelpStrings.UpdateInvalidVersionNumber(new LocalizedString(versionRange)), ErrorCategory.InvalidData, null, null);
			}
			this.StartVersion = new UpdatableHelpVersion(array[0].Trim());
			this.EndVersion = ((array.Length == 2) ? new UpdatableHelpVersion(array[1].Trim()) : this.StartVersion);
			if (this.CompareVersions(this.StartVersion, this.EndVersion) > 0)
			{
				UpdatableHelpVersion startVersion = this.StartVersion;
				this.StartVersion = this.EndVersion;
				this.EndVersion = startVersion;
			}
			this.HelpRevision = revision;
			this.CabinetUrl = cabinetUrl;
			string[] array2 = culturesUpdated.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			if (array2 != null)
			{
				foreach (string text in array2)
				{
					list.Add(text.Trim().ToLower());
				}
			}
			this.CulturesAffected = list.ToArray();
		}

		internal UpdatableHelpVersion StartVersion { get; private set; }

		internal UpdatableHelpVersion EndVersion { get; private set; }

		internal int HelpRevision { get; private set; }

		internal string[] CulturesAffected { get; private set; }

		internal string CabinetUrl { get; private set; }

		internal bool IsInRangeAndNewerThan(UpdatableHelpVersion currentVersion, int newestRevisionFound)
		{
			return this.HelpRevision > newestRevisionFound && this.CompareVersions(currentVersion, this.StartVersion) >= 0 && this.CompareVersions(currentVersion, this.EndVersion) <= 0;
		}

		private int CompareVersions(UpdatableHelpVersion v1, UpdatableHelpVersion v2)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num = v1.ElementIntValues[i] - v2.ElementIntValues[i];
				if (num != 0)
				{
					break;
				}
			}
			return num;
		}
	}
}
