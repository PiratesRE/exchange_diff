using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Inference.Ranking
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTrendingModel : IRankingModel
	{
		public ConversationTrendingModel()
		{
			this.featuresAndWeights = ConversationTrendingModel.DefaultFeaturesAndWeights;
			this.dependencies = this.GetDependencies();
		}

		public HashSet<PropertyDefinition> Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		public double Rank(object item)
		{
			double num = 0.0;
			foreach (Tuple<ConversationFeature, double> tuple in this.featuresAndWeights)
			{
				double num2 = tuple.Item1.FeatureValue(item) * tuple.Item2;
				num += num2;
			}
			return num;
		}

		private HashSet<PropertyDefinition> GetDependencies()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (Tuple<ConversationFeature, double> tuple in this.featuresAndWeights)
			{
				foreach (PropertyDefinition propertyDefinition in tuple.Item1.SupportingProperties)
				{
					ApplicationAggregatedProperty item = (ApplicationAggregatedProperty)propertyDefinition;
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		private static readonly int DefaultReplyCountFeatureWeight = 1;

		private static readonly int DefaultTotalItemLikesFeatureWeight = 1;

		private static readonly int DefaultConversationLikesFeatureWeight = 1;

		private static readonly int DefaultDirectParticipantsFeatureWeight = 5;

		private static readonly int DefaultConversationModifiedInLastHourFeatureWeight = 10;

		private static readonly int DefaultConversationModifiedInLastDayFeatureWeight = 5;

		private static readonly int DefaultConversationModifiedInLastWeekFeatureWeight = 1;

		private static readonly ConversationFeature ReplyCountFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.ItemCount
		}, (IStorePropertyBag conversation) => Math.Log((double)conversation.GetValueOrDefault<int>(AggregatedConversationSchema.ItemCount, 1), 2.0));

		private static readonly ConversationFeature DirectParticipantsFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.DirectParticipants
		}, (IStorePropertyBag conversation) => Math.Log((double)(1 + conversation.GetValueOrDefault<Participant[]>(AggregatedConversationSchema.DirectParticipants, null).Length), 2.0));

		private static readonly ConversationFeature TotalItemLikesFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.TotalItemLikes
		}, (IStorePropertyBag conversation) => Math.Log((double)(1 + conversation.GetValueOrDefault<int>(AggregatedConversationSchema.TotalItemLikes, 0)), 2.0));

		private static readonly ConversationFeature ConversationLikesFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.ConversationLikes
		}, (IStorePropertyBag conversation) => Math.Log((double)(1 + conversation.GetValueOrDefault<int>(AggregatedConversationSchema.ConversationLikes, 0)), 2.0));

		private static readonly ConversationFeature ConversationModifiedInLastHourFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.LastDeliveryTime
		}, (IStorePropertyBag conversation) => (double)((ExDateTime.Now - conversation.GetValueOrDefault<ExDateTime>(AggregatedConversationSchema.LastDeliveryTime, ExDateTime.MinValue) <= TimeSpan.FromHours(1.0)) ? 1 : 0));

		private static readonly ConversationFeature ConversationModifiedInLastDayFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.LastDeliveryTime
		}, (IStorePropertyBag conversation) => (double)((ExDateTime.Now - conversation.GetValueOrDefault<ExDateTime>(AggregatedConversationSchema.LastDeliveryTime, ExDateTime.MinValue) <= TimeSpan.FromDays(1.0)) ? 1 : 0));

		private static readonly ConversationFeature ConversationModifiedInLastWeekFeature = new ConversationFeature(new List<PropertyDefinition>
		{
			AggregatedConversationSchema.LastDeliveryTime
		}, (IStorePropertyBag conversation) => (double)((ExDateTime.Now - conversation.GetValueOrDefault<ExDateTime>(AggregatedConversationSchema.LastDeliveryTime, ExDateTime.MinValue) <= TimeSpan.FromDays(7.0)) ? 1 : 0));

		private static readonly Tuple<ConversationFeature, double>[] DefaultFeaturesAndWeights = new Tuple<ConversationFeature, double>[]
		{
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.ReplyCountFeature, (double)ConversationTrendingModel.DefaultReplyCountFeatureWeight),
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.TotalItemLikesFeature, (double)ConversationTrendingModel.DefaultTotalItemLikesFeatureWeight),
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.ConversationLikesFeature, (double)ConversationTrendingModel.DefaultConversationLikesFeatureWeight),
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.DirectParticipantsFeature, (double)ConversationTrendingModel.DefaultDirectParticipantsFeatureWeight),
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.ConversationModifiedInLastHourFeature, (double)ConversationTrendingModel.DefaultConversationModifiedInLastHourFeatureWeight),
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.ConversationModifiedInLastDayFeature, (double)ConversationTrendingModel.DefaultConversationModifiedInLastDayFeatureWeight),
			new Tuple<ConversationFeature, double>(ConversationTrendingModel.ConversationModifiedInLastWeekFeature, (double)ConversationTrendingModel.DefaultConversationModifiedInLastWeekFeatureWeight)
		};

		private readonly Tuple<ConversationFeature, double>[] featuresAndWeights;

		private readonly HashSet<PropertyDefinition> dependencies;
	}
}
