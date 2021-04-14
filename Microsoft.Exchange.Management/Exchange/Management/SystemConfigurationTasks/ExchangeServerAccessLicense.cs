using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class ExchangeServerAccessLicense : ExchangeServerAccessLicenseBase
	{
		public ExchangeServerAccessLicense(ExchangeServerAccessLicense.ServerVersionMajor versionMajor, ExchangeServerAccessLicense.AccessLicenseType accessLicense, ExchangeServerAccessLicense.UnitLabelType unitLabel) : this(versionMajor, accessLicense, unitLabel, ExchangeServerAccessLicense.TabulationMethodType.Net)
		{
		}

		public ExchangeServerAccessLicense(ExchangeServerAccessLicense.ServerVersionMajor versionMajor, ExchangeServerAccessLicense.AccessLicenseType accessLicense, ExchangeServerAccessLicense.UnitLabelType unitLabel, ExchangeServerAccessLicense.TabulationMethodType tabulationMethod)
		{
			this.VersionMajor = versionMajor;
			this.AccessLicense = accessLicense;
			this.UnitLabel = unitLabel;
			this.TabulationMethod = tabulationMethod;
			this.InternalInitialize();
		}

		public string ProductName { get; private set; }

		public ExchangeServerAccessLicense.UnitLabelType UnitLabel { get; private set; }

		public ExchangeServerAccessLicense.TabulationMethodType TabulationMethod { get; private set; }

		internal ExchangeServerAccessLicense.ServerVersionMajor VersionMajor { get; private set; }

		internal ExchangeServerAccessLicense.AccessLicenseType AccessLicense { get; private set; }

		public static bool TryParse(string licenseName, out ExchangeServerAccessLicense license)
		{
			license = null;
			if (string.IsNullOrEmpty(licenseName))
			{
				return false;
			}
			string[] array = licenseName.Split(new char[]
			{
				' '
			});
			if (array.Length != 5)
			{
				return false;
			}
			int num = 0;
			string productName = string.Format("{0} {1} {2}", array[num++], array[num++], array[num++]);
			KeyValuePair<ExchangeServerAccessLicense.ServerVersionMajor, string> keyValuePair = ExchangeServerAccessLicense.VersionMajorProductNameMap.SingleOrDefault((KeyValuePair<ExchangeServerAccessLicense.ServerVersionMajor, string> x) => x.Value.Equals(productName, StringComparison.InvariantCultureIgnoreCase));
			if (keyValuePair.Equals(default(KeyValuePair<ExchangeServerAccessLicense.ServerVersionMajor, string>)))
			{
				return false;
			}
			ExchangeServerAccessLicense.ServerVersionMajor key = keyValuePair.Key;
			ExchangeServerAccessLicense.AccessLicenseType accessLicense;
			if (!ExchangeServerAccessLicense.TryStringToEnum<ExchangeServerAccessLicense.AccessLicenseType>(array[num++], false, out accessLicense))
			{
				return false;
			}
			string text = array[num++];
			ExchangeServerAccessLicense.UnitLabelType unitLabelType;
			if (text.Equals("Edition", StringComparison.InvariantCultureIgnoreCase))
			{
				unitLabelType = ExchangeServerAccessLicense.UnitLabelType.Server;
			}
			else
			{
				if (!ExchangeServerAccessLicense.TryStringToEnum<ExchangeServerAccessLicense.UnitLabelType>(text, false, out unitLabelType))
				{
					return false;
				}
				if (unitLabelType == ExchangeServerAccessLicense.UnitLabelType.Server)
				{
					return false;
				}
			}
			license = new ExchangeServerAccessLicense(key, accessLicense, unitLabelType);
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ProductName: ");
			stringBuilder.AppendLine(this.ProductName);
			stringBuilder.Append("VersionMajor: ");
			stringBuilder.AppendLine(this.VersionMajor.ToString());
			stringBuilder.Append("AccessLicense: ");
			stringBuilder.AppendLine(this.AccessLicense.ToString());
			stringBuilder.Append("UnitLabel: ");
			stringBuilder.AppendLine(this.UnitLabel.ToString());
			stringBuilder.Append("TabulationMethod: ");
			stringBuilder.AppendLine(this.TabulationMethod.ToString());
			return stringBuilder.ToString();
		}

		internal static bool TryStringToEnum<TEnum>(string input, bool isUnderlyingNumber, out TEnum output) where TEnum : struct
		{
			output = default(TEnum);
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			try
			{
				if (!Enum.TryParse<TEnum>(input, true, out output))
				{
					return false;
				}
				int num;
				bool flag = int.TryParse(input, out num);
				if (isUnderlyingNumber)
				{
					if (!flag)
					{
						return false;
					}
					if (!Enum.IsDefined(typeof(TEnum), output))
					{
						return false;
					}
				}
				else if (flag)
				{
					return false;
				}
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}

		private void InternalInitialize()
		{
			this.ProductName = ExchangeServerAccessLicense.VersionMajorProductNameMap[this.VersionMajor];
			base.LicenseName = string.Format("{0} {1} {2}", this.ProductName, this.AccessLicense, (this.UnitLabel == ExchangeServerAccessLicense.UnitLabelType.Server) ? "Edition" : this.UnitLabel.ToString());
		}

		private const string Edition = "Edition";

		private const int TokenLength = 5;

		private static readonly Dictionary<ExchangeServerAccessLicense.ServerVersionMajor, string> VersionMajorProductNameMap = new Dictionary<ExchangeServerAccessLicense.ServerVersionMajor, string>
		{
			{
				ExchangeServerAccessLicense.ServerVersionMajor.E15,
				"Exchange Server 2013"
			}
		};

		public enum ServerVersionMajor
		{
			E15 = 15
		}

		public enum AccessLicenseType
		{
			Standard,
			Enterprise
		}

		public enum UnitLabelType
		{
			Server,
			CAL
		}

		public enum TabulationMethodType
		{
			Net
		}
	}
}
