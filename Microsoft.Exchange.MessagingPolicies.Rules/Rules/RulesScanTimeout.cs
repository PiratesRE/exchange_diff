using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RulesScanTimeout
	{
		internal IDictionary<string, uint> ScanVelocities { get; private set; }

		internal TimeSpan MinTimeout { get; set; }

		internal RulesScanTimeout(IDictionary<string, uint> scanVelocities, int minFipsTimeoutInMilliseconds)
		{
			this.ScanVelocities = scanVelocities;
			if (!this.ScanVelocities.ContainsKey(RulesScanTimeout.DefaultVelocityKey))
			{
				this.ScanVelocities[RulesScanTimeout.DefaultVelocityKey] = RulesScanTimeout.DefaultVelocityValue;
			}
			this.MinTimeout = TimeSpan.FromMilliseconds((double)minFipsTimeoutInMilliseconds);
		}

		internal TimeSpan GetTimeout(double bodyLength, List<KeyValuePair<string, double>> attachmentNamesAndLengths)
		{
			double num = 0.0;
			num += bodyLength / (this.ScanVelocities[RulesScanTimeout.DefaultVelocityKey] * 1024U);
			if (attachmentNamesAndLengths != null)
			{
				foreach (KeyValuePair<string, double> keyValuePair in attachmentNamesAndLengths)
				{
					string key = RulesScanTimeout.DefaultVelocityKey;
					if (!string.IsNullOrEmpty(keyValuePair.Key))
					{
						key = TransportUtils.GetFileExtension(keyValuePair.Key);
					}
					uint num2;
					if (!this.ScanVelocities.TryGetValue(key, out num2))
					{
						num2 = this.ScanVelocities[RulesScanTimeout.DefaultVelocityKey];
					}
					double value = keyValuePair.Value;
					num += value / (num2 * 1024U);
				}
			}
			if ((double)((int)num) <= this.MinTimeout.TotalSeconds)
			{
				return this.MinTimeout;
			}
			return TimeSpan.FromSeconds((double)((int)num));
		}

		internal TimeSpan GetTimeout(Stream bodyStream, List<KeyValuePair<string, Stream>> attachmentNamesAndStreams)
		{
			double bodyLength = 0.0;
			if (bodyStream != null)
			{
				try
				{
					bodyLength = (double)bodyStream.Length;
				}
				catch (NotSupportedException)
				{
				}
			}
			List<KeyValuePair<string, double>> list = null;
			if (attachmentNamesAndStreams != null && attachmentNamesAndStreams.Count > 0)
			{
				list = new List<KeyValuePair<string, double>>();
				foreach (KeyValuePair<string, Stream> keyValuePair in attachmentNamesAndStreams)
				{
					Stream value = keyValuePair.Value;
					if (value != null)
					{
						try
						{
							long length = value.Length;
							list.Add(new KeyValuePair<string, double>(keyValuePair.Key, (double)length));
						}
						catch (NotSupportedException)
						{
						}
					}
				}
			}
			return this.GetTimeout(bodyLength, list);
		}

		internal static readonly string DefaultVelocityKey = ".";

		internal static readonly uint DefaultVelocityValue = 30U;
	}
}
