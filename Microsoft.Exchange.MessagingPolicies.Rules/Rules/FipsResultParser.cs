using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class FipsResultParser
	{
		public static IEnumerable<DiscoveredDataClassification> ParseDataClassifications(FilteringResults filteringResults, ITracer tracer)
		{
			if (filteringResults == null)
			{
				return new List<DiscoveredDataClassification>();
			}
			Dictionary<string, DiscoveredDataClassification> dictionary = new Dictionary<string, DiscoveredDataClassification>();
			foreach (ResultsExtensions.DetectionTuple detectionTuple in ResultsExtensions.GetDetectionTuples(filteringResults, 16))
			{
				int id = detectionTuple.Stream.Id;
				string name = detectionTuple.Stream.Name;
				string topLevelSourceName = FipsResultParser.GetTopLevelSourceName(detectionTuple.Stream, filteringResults.Streams);
				foreach (ClassificationClass classificationClass in detectionTuple.Detection.Classes)
				{
					DiscoveredDataClassification discoveredDataClassification;
					if (!dictionary.ContainsKey(classificationClass.Id))
					{
						discoveredDataClassification = new DiscoveredDataClassification(classificationClass.Id, classificationClass.Name, FipsResultParser.GetRecommendedMinimumConfidence(classificationClass), null);
						dictionary.Add(discoveredDataClassification.Id, discoveredDataClassification);
					}
					else
					{
						discoveredDataClassification = dictionary[classificationClass.Id];
						discoveredDataClassification.RecommendedMinimumConfidence = FipsResultParser.GetRecommendedMinimumConfidence(classificationClass);
					}
					DataClassificationSourceInfo dataClassificationSourceInfo = new DataClassificationSourceInfo(id, name, topLevelSourceName, classificationClass.Count, (uint)classificationClass.Weight);
					discoveredDataClassification.MatchingSourceInfos.Add(dataClassificationSourceInfo);
					foreach (string text in classificationClass.GetGroupIdentifiers())
					{
						bool flag = true;
						foreach (string text2 in classificationClass.GetSourcesInGroup(text))
						{
							try
							{
								int[] array = text2.Split(new char[]
								{
									','
								}).Select(new Func<string, int>(int.Parse)).ToArray<int>();
								if (array.Length != 2)
								{
									tracer.TraceError("Bad detection offset format returned by FIPS {0}", new object[]
									{
										text2
									});
									return new List<DiscoveredDataClassification>();
								}
								DataClassificationMatchLocation item = new DataClassificationMatchLocation(array[0], array[1], dataClassificationSourceInfo);
								if (flag)
								{
									dataClassificationSourceInfo.Locations.Add(item);
									flag = false;
								}
								else
								{
									dataClassificationSourceInfo.Locations.Last<DataClassificationMatchLocation>().SecondaryLocations.Add(item);
								}
							}
							catch (FormatException)
							{
								tracer.TraceError("Bad detection offset format returned by FIPS {0}", new object[]
								{
									text2
								});
								return new List<DiscoveredDataClassification>();
							}
							catch (OverflowException)
							{
								tracer.TraceError("Bad detection offset format returned by FIPS {0}", new object[]
								{
									text2
								});
								return new List<DiscoveredDataClassification>();
							}
						}
					}
				}
			}
			return dictionary.Values;
		}

		public static IEnumerable<DiscoveredDataClassification> UnionDiscoveredDataClassificationsFromDistinctStreams(IEnumerable<DiscoveredDataClassification> left, IEnumerable<DiscoveredDataClassification> right)
		{
			if (left == null && right == null)
			{
				return null;
			}
			if (left == null)
			{
				return right;
			}
			if (right == null)
			{
				return left;
			}
			Dictionary<string, DiscoveredDataClassification> dictionary = new Dictionary<string, DiscoveredDataClassification>();
			foreach (DiscoveredDataClassification discoveredDataClassification in left)
			{
				dictionary.Add(discoveredDataClassification.Id, discoveredDataClassification);
			}
			foreach (DiscoveredDataClassification discoveredDataClassification2 in right)
			{
				if (!dictionary.ContainsKey(discoveredDataClassification2.Id))
				{
					dictionary.Add(discoveredDataClassification2.Id, discoveredDataClassification2);
				}
				else
				{
					DiscoveredDataClassification discoveredDataClassification3 = dictionary[discoveredDataClassification2.Id];
					discoveredDataClassification3.MatchingSourceInfos.AddRange(discoveredDataClassification2.MatchingSourceInfos);
				}
			}
			return dictionary.Values;
		}

		public static IEnumerable<DiscoveredDataClassification> DeleteClassifications(IEnumerable<DiscoveredDataClassification> classifications, List<string> sourceNames, bool keepGivenDeleteOthers = false)
		{
			if (classifications == null)
			{
				return classifications;
			}
			if (sourceNames != null)
			{
				List<DiscoveredDataClassification> list = new List<DiscoveredDataClassification>();
				foreach (DiscoveredDataClassification item in classifications)
				{
					list.Add(item);
				}
				for (int i = list.Count - 1; i >= 0; i--)
				{
					DiscoveredDataClassification discoveredDataClassification = list[i];
					for (int j = discoveredDataClassification.MatchingSourceInfos.Count - 1; j >= 0; j--)
					{
						DataClassificationSourceInfo dataClassificationSourceInfo = discoveredDataClassification.MatchingSourceInfos[j];
						bool flag = false;
						if (sourceNames.Contains(dataClassificationSourceInfo.TopLevelSourceName))
						{
							if (!keepGivenDeleteOthers)
							{
								flag = true;
							}
						}
						else if (keepGivenDeleteOthers)
						{
							flag = true;
						}
						if (flag)
						{
							discoveredDataClassification.MatchingSourceInfos.RemoveAt(j);
						}
					}
					if (discoveredDataClassification.MatchingSourceInfos == null || discoveredDataClassification.MatchingSourceInfos.Count == 0)
					{
						list.RemoveAt(i);
					}
				}
				return list;
			}
			if (keepGivenDeleteOthers)
			{
				return null;
			}
			return classifications;
		}

		private static uint GetRecommendedMinimumConfidence(ClassificationClass classification)
		{
			object obj;
			bool flag = classification.Properties.TryGetValue(DiscoveredDataClassification.RecommendedWeightKey, out obj);
			if (!flag || obj == null)
			{
				return 0U;
			}
			uint result;
			try
			{
				result = Convert.ToUInt32(obj);
			}
			catch (OverflowException)
			{
				result = 0U;
			}
			return result;
		}

		private static string GetTopLevelSourceName(StreamIdentity currentStream, IList<StreamIdentity> allStreams)
		{
			while (currentStream.ParentId > 0)
			{
				currentStream = allStreams[currentStream.ParentId];
			}
			return currentStream.Name;
		}
	}
}
