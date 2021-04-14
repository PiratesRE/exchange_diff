using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadMetricStorage
	{
		public LoadMetricStorage()
		{
			this.values = new ConcurrentDictionary<LoadMetric, long>();
		}

		public LoadMetricStorage(LoadMetricStorage metricStorage) : this()
		{
			foreach (LoadMetric metric in metricStorage.Metrics)
			{
				this[metric] = metricStorage[metric];
			}
		}

		public IEnumerable<LoadMetric> Metrics
		{
			get
			{
				return this.values.Keys;
			}
		}

		public long this[LoadMetric metric]
		{
			get
			{
				long result;
				if (!this.values.TryGetValue(metric, out result))
				{
					result = 0L;
				}
				return result;
			}
			set
			{
				this.values[metric] = value;
			}
		}

		public static LoadMetricStorage operator +(LoadMetricStorage left, LoadMetricStorage right)
		{
			AnchorUtil.ThrowOnNullArgument(left, "left");
			AnchorUtil.ThrowOnNullArgument(right, "right");
			LoadMetricStorage loadMetricStorage = new LoadMetricStorage();
			foreach (LoadMetric metric in left.values.Keys)
			{
				loadMetricStorage[metric] = left[metric];
			}
			foreach (LoadMetric metric2 in right.values.Keys)
			{
				loadMetricStorage[metric2] += right[metric2];
			}
			return loadMetricStorage;
		}

		public static LoadMetricStorage operator -(LoadMetricStorage left, LoadMetricStorage right)
		{
			LoadMetricStorage result = new LoadMetricStorage();
			foreach (LoadMetric metric in left.values.Keys)
			{
				result[metric] = left[metric];
			}
			foreach (LoadMetric metric2 in from key in right.values.Keys
			where result.values.ContainsKey(key)
			select key)
			{
				result[metric2] -= right[metric2];
			}
			return result;
		}

		public void ConvertFromSerializationFormat()
		{
			foreach (LoadMetric loadMetric in this.Metrics)
			{
				long value;
				this.values.TryRemove(loadMetric, out value);
				foreach (LoadMetric loadMetric2 in LoadMetricRepository.DefaultMetrics)
				{
					if (loadMetric.Name.Equals(loadMetric2.Name))
					{
						this[loadMetric2] = value;
					}
				}
				BandData bandData = loadMetric as BandData;
				if (bandData != null)
				{
					this[bandData.Band] = value;
				}
				else
				{
					Band band = loadMetric as Band;
					if (band != null)
					{
						this[band] = value;
					}
				}
			}
		}

		public void ConvertToSerializationMetrics(bool convertBandToBandData)
		{
			foreach (LoadMetric loadMetric in this.Metrics)
			{
				long num;
				this.values.TryRemove(loadMetric, out num);
				Band band = loadMetric as Band;
				LoadMetric metric;
				if (band != null)
				{
					if (convertBandToBandData)
					{
						metric = new BandData(band)
						{
							TotalWeight = (int)num
						};
					}
					else
					{
						metric = band;
					}
				}
				else
				{
					metric = new LoadMetric(loadMetric.Name, loadMetric.IsSize);
				}
				this[metric] = num;
			}
		}

		public BandData GetBandData(Band band)
		{
			Band left = this.Metrics.OfType<Band>().FirstOrDefault((Band bd) => object.Equals(band, bd));
			BandData bandData = new BandData(band)
			{
				TotalWeight = 0
			};
			if (left == null)
			{
				return bandData;
			}
			bandData.TotalWeight = (int)this[band];
			return bandData;
		}

		public ByteQuantifiedSize GetSizeMetric(LoadMetric sizeMetric)
		{
			long num;
			if (!this.values.TryGetValue(sizeMetric, out num) || num <= 0L)
			{
				return ByteQuantifiedSize.Zero;
			}
			return sizeMetric.ToByteQuantifiedSize(num);
		}

		public bool SupportsAdditional(LoadMetricStorage incomingLoad, out LoadMetric exceededMetric, out long requestedUnits, out long availableUnits)
		{
			AnchorUtil.ThrowOnNullArgument(incomingLoad, "incomingLoad");
			exceededMetric = null;
			requestedUnits = 0L;
			availableUnits = 0L;
			foreach (LoadMetric loadMetric in this.values.Keys)
			{
				if (this[loadMetric] < incomingLoad[loadMetric] || this[loadMetric] == 0L)
				{
					exceededMetric = loadMetric;
					availableUnits = this[loadMetric];
					requestedUnits = incomingLoad[loadMetric];
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			IEnumerable<string> enumerable = this.values.Select(delegate(KeyValuePair<LoadMetric, long> kvp)
			{
				LoadMetric key = kvp.Key;
				return key.GetValueString(kvp.Value);
			});
			string arg = string.Join(",", enumerable);
			return string.Format("{{{0}}}", arg);
		}

		[DataMember]
		private readonly ConcurrentDictionary<LoadMetric, long> values;
	}
}
