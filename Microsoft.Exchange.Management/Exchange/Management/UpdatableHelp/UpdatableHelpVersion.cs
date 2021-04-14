using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	internal class UpdatableHelpVersion
	{
		internal UpdatableHelpVersion(string version)
		{
			this.NormalizedVersionNumber = string.Empty;
			this.ElementIntValues = new int[4];
			string[] array = new string[4];
			string[] array2 = version.Split(new char[]
			{
				'.'
			});
			if (array2.Length == 4)
			{
				for (int i = 0; i < 4; i++)
				{
					string s = array2[i];
					if (!int.TryParse(s, out this.ElementIntValues[i]) || this.ElementIntValues[i] >= UpdatableHelpVersion.ElementUpperBound[i])
					{
						throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInvalidVersionNumberErrorID, UpdatableHelpStrings.UpdateInvalidVersionNumber(new LocalizedString(version)), ErrorCategory.InvalidData, null, null);
					}
					array[i] = this.ElementIntValues[i].ToString().PadLeft(UpdatableHelpVersion.ElementDigits[i], '0');
				}
				this.NormalizedVersionNumber = string.Join(".", array);
				return;
			}
			throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInvalidVersionNumberErrorID, UpdatableHelpStrings.UpdateInvalidVersionNumber(new LocalizedString(version)), ErrorCategory.InvalidData, null, null);
		}

		internal int MajorVersion
		{
			get
			{
				return this.ElementIntValues[0];
			}
		}

		internal int MinorVersion
		{
			get
			{
				return this.ElementIntValues[1];
			}
		}

		internal int BuildNumber
		{
			get
			{
				return this.ElementIntValues[2];
			}
		}

		internal int DotBuildNumber
		{
			get
			{
				return this.ElementIntValues[3];
			}
		}

		internal string NormalizedVersionNumber { get; private set; }

		internal string NormalizedVersionNumberWithRevision(int revision)
		{
			return this.NormalizedVersionNumber + "." + revision.ToString();
		}

		internal const int NumVersionElements = 4;

		internal static int[] ElementUpperBound = new int[]
		{
			100,
			100,
			10000,
			1000
		};

		internal static int[] ElementDigits = new int[]
		{
			2,
			2,
			4,
			3
		};

		internal int[] ElementIntValues;
	}
}
