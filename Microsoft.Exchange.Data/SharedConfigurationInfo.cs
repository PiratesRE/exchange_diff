using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class SharedConfigurationInfo
	{
		public SharedConfigurationInfo(ServerVersion currentVersion, string programId, string offerId)
		{
			if (currentVersion == null)
			{
				throw new ArgumentNullException("currentVersion");
			}
			if (string.IsNullOrEmpty(programId))
			{
				throw new ArgumentNullException("programId");
			}
			if (programId.Contains("-"))
			{
				throw new ArgumentException("programId cannot contain -");
			}
			if (string.IsNullOrEmpty(offerId))
			{
				throw new ArgumentNullException("offerId");
			}
			if (offerId.Contains("-"))
			{
				throw new ArgumentException("offerId cannot contain -");
			}
			this.currentVersion = currentVersion;
			this.programId = programId;
			this.offerId = offerId;
		}

		public ServerVersion CurrentVersion
		{
			get
			{
				return this.currentVersion;
			}
		}

		public string ProgramId
		{
			get
			{
				return this.programId;
			}
		}

		public string OfferId
		{
			get
			{
				return this.offerId;
			}
		}

		public static SharedConfigurationInfo FromInstalledVersion(string programId, string offerId)
		{
			return new SharedConfigurationInfo(ServerVersion.InstalledVersion, programId, offerId);
		}

		public bool Equals(SharedConfigurationInfo value)
		{
			return object.ReferenceEquals(this, value) || (value != null && this.currentVersion.Equals(value.currentVersion) && this.programId.Equals(value.programId, StringComparison.OrdinalIgnoreCase) && this.offerId.Equals(value.offerId, StringComparison.OrdinalIgnoreCase));
		}

		public override bool Equals(object comparand)
		{
			SharedConfigurationInfo sharedConfigurationInfo = comparand as SharedConfigurationInfo;
			return sharedConfigurationInfo != null && this.Equals(sharedConfigurationInfo);
		}

		public static bool operator ==(SharedConfigurationInfo left, SharedConfigurationInfo right)
		{
			if (left != null)
			{
				return left.Equals(right);
			}
			return right == null;
		}

		public static bool operator !=(SharedConfigurationInfo left, SharedConfigurationInfo right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public string GetPrefix()
		{
			return this.GetPrefix(true);
		}

		private string GetPrefix(bool useWildcard)
		{
			return string.Join("_", new string[]
			{
				this.currentVersion.Major.ToString(),
				this.currentVersion.Minor.ToString(),
				this.currentVersion.Build.ToString(),
				useWildcard ? "*" : this.currentVersion.Revision.ToString()
			});
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.cachedToStringValue))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.GetPrefix(false));
				stringBuilder.Append("-");
				stringBuilder.Append(this.programId);
				stringBuilder.Append("-");
				stringBuilder.Append(this.offerId);
				if (!string.IsNullOrEmpty(this.extension))
				{
					stringBuilder.Append("-");
					stringBuilder.Append(this.extension);
				}
				this.cachedToStringValue = stringBuilder.ToString();
			}
			return this.cachedToStringValue;
		}

		public static SharedConfigurationInfo Parse(string strValue)
		{
			SharedConfigurationInfo result;
			if (!SharedConfigurationInfo.TryParse(strValue, out result))
			{
				throw new ArgumentException("strValue: " + strValue);
			}
			return result;
		}

		public static bool TryParse(string strValue, out SharedConfigurationInfo sharedConfigInfo)
		{
			sharedConfigInfo = null;
			if (string.IsNullOrEmpty(strValue))
			{
				return false;
			}
			string[] array = strValue.Split(new string[]
			{
				"-"
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length < 3)
			{
				return false;
			}
			int i = 0;
			string[] array2 = array[i].Split(new string[]
			{
				"_"
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array2 == null || array2.Length < 4)
			{
				return false;
			}
			ServerVersion serverVersion = new ServerVersion(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]));
			i++;
			if (string.IsNullOrEmpty(array[i]) || string.IsNullOrEmpty(array[i + 1]))
			{
				return false;
			}
			string text = array[i];
			string text2 = array[i + 1];
			i += 2;
			sharedConfigInfo = new SharedConfigurationInfo(serverVersion, text, text2);
			string text3 = null;
			while (i < array.Length)
			{
				text3 = string.Join("-", new string[]
				{
					text3,
					array[i]
				});
				i++;
			}
			sharedConfigInfo.extension = text3;
			return true;
		}

		private const int MinimumNumberOfComponents = 3;

		public const string InnerSeparator = "_";

		public const string OuterSeparator = "-";

		public const string CurrentTypeHeader = "C";

		private ServerVersion currentVersion;

		private string programId;

		private string offerId;

		private string extension;

		private string cachedToStringValue;
	}
}
