using System;
using Microsoft.Exchange.Management.Analysis.Builders;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal static class FeatureBuilderExtensions
	{
		public static T Mode<T>(this T builder, SetupMode modes) where T : IFeatureBuilder
		{
			builder.AddFeature(new AppliesToModeFeature(modes));
			return builder;
		}

		public static T Role<T>(this T builder, SetupRole roles) where T : IFeatureBuilder
		{
			builder.AddFeature(new AppliesToRoleFeature(roles));
			return builder;
		}

		public static T Error<T>(this T builder) where T : IRuleFeatureBuilder
		{
			builder.AddFeature(new RuleTypeFeature(RuleType.Error));
			return builder;
		}

		public static T Warning<T>(this T builder) where T : IRuleFeatureBuilder
		{
			builder.AddFeature(new RuleTypeFeature(RuleType.Warning));
			return builder;
		}

		public static T Info<T>(this T builder) where T : IRuleFeatureBuilder
		{
			builder.AddFeature(new RuleTypeFeature(RuleType.Info));
			return builder;
		}

		public static T Message<T>(this T builder, Func<Result, string> textFunction) where T : IRuleFeatureBuilder
		{
			builder.AddFeature(new MessageFeature(textFunction));
			return builder;
		}
	}
}
