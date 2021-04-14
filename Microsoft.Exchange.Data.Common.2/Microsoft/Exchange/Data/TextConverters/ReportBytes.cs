using System;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ReportBytes : IReportBytes
	{
		internal ReportBytes() : this(0, 0)
		{
		}

		internal ReportBytes(int expansionSizeLimit, int expansionSizeMultiple)
		{
			if (expansionSizeLimit > 0 && expansionSizeMultiple > 0)
			{
				this.expansionSizeLimit = expansionSizeLimit;
				this.expansionSizeMultiple = expansionSizeMultiple;
			}
			else
			{
				if (!ReportBytes.isReadFromConfiguration)
				{
					CtsConfigurationSetting simpleConfigurationSetting = ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "ExpansionSizeLimit");
					if (simpleConfigurationSetting != null)
					{
						ReportBytes.configExpansionSizeLimit = ApplicationServices.ParseIntegerSetting(simpleConfigurationSetting, 524288, 1, true);
					}
					else
					{
						ReportBytes.configExpansionSizeLimit = 524288;
					}
					simpleConfigurationSetting = ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "ExpansionSizeMultiple");
					if (simpleConfigurationSetting != null)
					{
						ReportBytes.configExpansionSizeMultiple = ApplicationServices.ParseIntegerSetting(simpleConfigurationSetting, 10, 5, false);
					}
					else
					{
						ReportBytes.configExpansionSizeMultiple = 10;
					}
					ReportBytes.isReadFromConfiguration = true;
				}
				this.expansionSizeLimit = ReportBytes.configExpansionSizeLimit;
				this.expansionSizeMultiple = ReportBytes.configExpansionSizeMultiple;
			}
			this.bytesRead = 0L;
			this.bytesWritten = 0L;
		}

		public void ReportBytesRead(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.bytesRead += (long)count;
			this.CheckBytes();
		}

		public void ReportBytesWritten(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.bytesWritten += (long)count;
			this.CheckBytes();
		}

		private void CheckBytes()
		{
			if (this.bytesRead == 0L)
			{
				return;
			}
			if (this.bytesRead < 0L)
			{
				throw new InvalidOperationException("ReportBytes.bytesRead < 0");
			}
			if (this.bytesWritten < 0L)
			{
				throw new InvalidOperationException("ReportBytes.bytesWritten < 0");
			}
			if (this.bytesWritten > (long)this.expansionSizeLimit && this.bytesWritten / this.bytesRead > (long)this.expansionSizeMultiple)
			{
				throw new TextConvertersException(TextConvertersStrings.DocumentGrowingExcessively(this.expansionSizeMultiple));
			}
		}

		private const int DefaultExpansionSizeLimit = 524288;

		private const int DefaultExpansionSizeMultiple = 10;

		private static bool isReadFromConfiguration = false;

		private static int configExpansionSizeLimit = -1;

		private static int configExpansionSizeMultiple = -1;

		private int expansionSizeLimit;

		private int expansionSizeMultiple;

		private long bytesRead;

		private long bytesWritten;
	}
}
