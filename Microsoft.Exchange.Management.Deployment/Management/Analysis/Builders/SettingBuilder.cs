using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class SettingBuilder<T, TParent> : ISettingFeatureBuilder, IFeatureBuilder
	{
		public SettingBuilder(SettingBuildContext<T> context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public Setting<T> SetValue(Func<Result<TParent>, Result<T>> setFunction)
		{
			this.context.SetFunction = delegate(Result x)
			{
				IEnumerable<Result<T>> result;
				try
				{
					result = new Result<T>[]
					{
						setFunction((Result<TParent>)x)
					};
				}
				catch (Exception exception)
				{
					result = new Result<T>[]
					{
						new Result<T>(exception)
					};
				}
				return result;
			};
			return (Setting<T>)this.context.Construct();
		}

		public Setting<T> SetMultipleValues(Func<Result<TParent>, IEnumerable<Result<T>>> setFunction)
		{
			this.context.SetFunction = delegate(Result x)
			{
				IEnumerable<Result<T>> result;
				try
				{
					result = setFunction((Result<TParent>)x);
				}
				catch (Exception exception)
				{
					result = new Result<T>[]
					{
						new Result<T>(exception)
					};
				}
				return result;
			};
			return (Setting<T>)this.context.Construct();
		}

		void IFeatureBuilder.AddFeature(Feature feature)
		{
			((IFeatureBuilder)this.context).AddFeature(feature);
		}

		private SettingBuildContext<T> context;
	}
}
