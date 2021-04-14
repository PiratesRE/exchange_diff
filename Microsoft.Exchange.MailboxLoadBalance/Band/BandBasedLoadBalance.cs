using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	internal class BandBasedLoadBalance : ILoadBalance
	{
		public BandBasedLoadBalance(IList<Band> bands, ILogger logger, ILoadBalanceSettings settings)
		{
			this.settings = settings;
			this.logger = logger;
			this.bands = bands;
		}

		public IEnumerable<BandMailboxRebalanceData> BalanceForest(LoadContainer forest)
		{
			IEnumerable<BandMailboxRebalanceData> result;
			try
			{
				this.logger.Log(MigrationEventType.Information, "Starting a new load balancing procedure using the band algorithm.", new object[0]);
				this.results = new List<BandMailboxRebalanceData>();
				this.logger.Log(MigrationEventType.Verbose, "Using {0} bands for balancing.", new object[]
				{
					this.bands.Count
				});
				this.logger.Log(MigrationEventType.Verbose, "Calculating total database weight in the forest.", new object[0]);
				DatabaseWeightAggregator databaseWeightAggregator = new DatabaseWeightAggregator();
				forest.Accept(databaseWeightAggregator);
				this.logger.Log(MigrationEventType.Verbose, "Forest has a '{0}' total database weight.", new object[]
				{
					databaseWeightAggregator.TotalWeight
				});
				this.totalDataSelectedToMove = new ByteQuantifiedSize(0UL);
				foreach (Band band in this.bands)
				{
					BandDataAggregator bandDataAggregator = new BandDataAggregator(band);
					forest.Accept(bandDataAggregator);
					this.BalanceBand(band, (double)databaseWeightAggregator.TotalWeight, bandDataAggregator.BandData.ToArray<BandData>());
				}
				ByteQuantifiedSize byteQuantifiedSize = ByteQuantifiedSize.FromGB((ulong)this.settings.MaximumBatchSizeGb);
				ByteQuantifiedSize totalBatchSize = this.GetTotalBatchSize();
				if (totalBatchSize > byteQuantifiedSize)
				{
					this.logger.LogWarning("Total selected size is {0}, but we're only allowed to rebalance {1}. Reducing batch.", new object[]
					{
						totalBatchSize,
						byteQuantifiedSize
					});
					IBatchSizeReducer batchSizeReducer = BatchSizeReducerFactory.Instance.GetBatchSizeReducer(byteQuantifiedSize, totalBatchSize, this.logger);
					result = batchSizeReducer.ReduceBatchSize(this.results);
				}
				else
				{
					result = this.results;
				}
			}
			finally
			{
				this.logger.Log(MigrationEventType.Information, "Finished load balancing procedure using the band algorithm. {0} rebalancing entries created.", new object[]
				{
					this.results.Count
				});
			}
			return result;
		}

		private ByteQuantifiedSize GetTotalBatchSize()
		{
			ByteQuantifiedSize byteQuantifiedSize = ByteQuantifiedSize.Zero;
			foreach (BandMailboxRebalanceData bandMailboxRebalanceData in this.results)
			{
				foreach (Band band in this.bands)
				{
					long value = bandMailboxRebalanceData.RebalanceInformation[band];
					byteQuantifiedSize += band.ToByteQuantifiedSize(value);
				}
			}
			return byteQuantifiedSize;
		}

		private void BalanceBand(Band band, double totalDatabaseWeight, BandData[] bandData)
		{
			this.logger.Log(MigrationEventType.Information, "Balancing band '{0}' with '{1}' data entries.", new object[]
			{
				band,
				bandData.Count<BandData>()
			});
			double moveUnitsPerWeight = (double)bandData.Sum((BandData data) => data.TotalWeight) / totalDatabaseWeight;
			double num = (band.Profile == Band.BandProfile.SizeBased) ? band.MailboxSizeWeightFactor : 0.0;
			double deviation = (double)this.settings.WeightDeviationPercent / 100.0;
			ByteQuantifiedSize byteQuantifiedSize = ByteQuantifiedSize.FromGB((ulong)this.settings.MaximumAmountOfDataPerRoundGb);
			this.logger.Log(MigrationEventType.Verbose, "Moving with a deviation of '{0}', a minimum delta of '{1}' and a maximum of '{2}' per database.", new object[]
			{
				deviation,
				num,
				byteQuantifiedSize
			});
			List<BandData> list = (from data in bandData
			orderby this.GetBandDelta(moveUnitsPerWeight, deviation, data) descending
			select data).ToList<BandData>();
			foreach (BandData bandData2 in list)
			{
				double num2 = this.GetBandDelta(moveUnitsPerWeight, deviation, bandData2);
				this.logger.Log(MigrationEventType.Verbose, "Attempting to balance band {0} for database {1}, current delta is {2}.", new object[]
				{
					band,
					bandData2.Database,
					num2
				});
				LoadMetric instance = PhysicalSize.Instance;
				if (num2 <= num)
				{
					this.logger.Log(MigrationEventType.Information, "Not balancing band {0} for database {1} because delta {2} is either less than the minimum of {3} or database has no more available space ({4}). We're done.", new object[]
					{
						band,
						bandData2.Database,
						num2,
						num,
						bandData2.Database.AvailableCapacity.GetSizeMetric(instance)
					});
					break;
				}
				foreach (BandData bandData3 in from data in bandData
				orderby data.TotalWeight
				select data)
				{
					if (!bandData3.Database.CanAcceptBalancingLoad)
					{
						this.logger.Log(MigrationEventType.Verbose, "Database {0} can not be used as target because it can't take LB load.", new object[]
						{
							bandData3.Database
						});
					}
					else
					{
						double num3 = this.GetBandDelta(moveUnitsPerWeight, 0.0 - deviation, bandData3);
						this.logger.Log(MigrationEventType.Verbose, "Trying to place weight into {0} (current delta: {1}).", new object[]
						{
							bandData3.Database,
							num3
						});
						ByteQuantifiedSize sizeMetric = bandData3.Database.AvailableCapacity.GetSizeMetric(instance);
						if (0.0 - num3 > sizeMetric.ToMB())
						{
							this.logger.Log(MigrationEventType.Verbose, "Target delta of {0} is larger than the {1} available space in the database, adjusting.", new object[]
							{
								num3,
								sizeMetric
							});
							num3 = 0.0 - sizeMetric.ToMB();
							this.logger.Log(MigrationEventType.Verbose, "New target delta is {0}.", new object[]
							{
								num3
							});
						}
						if (num3 >= 0.0)
						{
							this.logger.Log(MigrationEventType.Verbose, "Target database is above the threshold, skipping as a target.", new object[0]);
						}
						else
						{
							ByteQuantifiedSize sizeMetric2 = bandData3.Database.CommittedLoad.GetSizeMetric(instance);
							ByteQuantifiedSize byteQuantifiedSize2;
							if (sizeMetric2 > byteQuantifiedSize)
							{
								byteQuantifiedSize2 = ByteQuantifiedSize.Zero;
							}
							else
							{
								byteQuantifiedSize2 = byteQuantifiedSize - sizeMetric2;
							}
							int num4 = (int)Math.Floor(byteQuantifiedSize2.ToMB() / band.MailboxSizeWeightFactor);
							if (num4 <= 0)
							{
								this.logger.Log(MigrationEventType.Verbose, "Target database committed load is {0} which is over the limit of {1}, skipping as a target.", new object[]
								{
									sizeMetric2,
									byteQuantifiedSize
								});
							}
							else
							{
								int num5 = (int)Math.Min(Math.Round(Math.Min(Math.Abs(num2), Math.Abs(num3))), (double)num4);
								this.logger.Log(MigrationEventType.Verbose, "Projected to move {0} units out of {1} and into {2}", new object[]
								{
									num5,
									bandData2.Database,
									bandData3.Database
								});
								if (num5 > 0)
								{
									ByteQuantifiedSize value = ByteQuantifiedSize.FromMB((ulong)((double)num5 * band.MailboxSizeWeightFactor));
									LoadMetricStorage loadMetricStorage = new LoadMetricStorage();
									loadMetricStorage[band] = (long)num5;
									BandMailboxRebalanceData item = new BandMailboxRebalanceData(bandData2.Database, bandData3.Database, loadMetricStorage);
									bandData3.TotalWeight += num5;
									LoadMetricStorage committedLoad;
									LoadMetric metric;
									(committedLoad = bandData3.Database.CommittedLoad)[metric = instance] = committedLoad[metric] + (long)value.ToBytes();
									this.totalDataSelectedToMove += value;
									bandData2.TotalWeight -= num5;
									this.results.Add(item);
									num2 -= (double)num5;
								}
								if (num2 <= num)
								{
									break;
								}
							}
						}
					}
				}
			}
		}

		private double GetBandDelta(double weightAverage, double deviation, BandData band)
		{
			LoadContainer database = band.Database;
			double num = Math.Round(weightAverage * (double)database.RelativeLoadWeight * (1.0 + deviation));
			return (double)band.TotalWeight - num;
		}

		private readonly ILoadBalanceSettings settings;

		private readonly ILogger logger;

		private readonly IList<Band> bands;

		private List<BandMailboxRebalanceData> results;

		private ByteQuantifiedSize totalDataSelectedToMove;
	}
}
