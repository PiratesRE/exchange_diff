using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class FeatureSet : IEnumerable<Feature>, IEnumerable
	{
		private FeatureSet(IEnumerable<Feature> features, Func<FeatureSet> parent = null)
		{
			this.parent = parent;
			if (features == null)
			{
				throw new ArgumentNullException("features");
			}
			if (features.Any((Feature x) => x == null))
			{
				throw new ArgumentException(Strings.CannotAddNullFeature, "features");
			}
			if ((from x in features
			group x by x.GetType()).Any((IGrouping<Type, Feature> x) => x.Count<Feature>() > 1))
			{
				throw new ArgumentException(Strings.CanOnlyHaveOneFeatureOfEachType, "features");
			}
			this.features = new List<Feature>(features);
		}

		public bool HasFeature<T>() where T : Feature
		{
			return this.features.Any((Feature x) => x.GetType() == typeof(T)) || (this.parent != null && this.parent().HasFeature<T>());
		}

		public T GetFeature<T>() where T : Feature
		{
			T t = (T)((object)this.features.FirstOrDefault((Feature x) => x.GetType() == typeof(T)));
			if (t != null)
			{
				return t;
			}
			if (this.parent == null)
			{
				throw new Exception(Strings.FeatureMissing(typeof(T).Name));
			}
			return this.parent().GetFeature<T>();
		}

		public IEnumerator<Feature> GetEnumerator()
		{
			foreach (Feature feature2 in this.features)
			{
				yield return feature2;
			}
			if (this.parent != null)
			{
				using (IEnumerator<Feature> enumerator2 = this.parent().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Feature feature = enumerator2.Current;
						if (!this.features.Any((Feature x) => x.GetType() == feature.GetType()))
						{
							yield return feature;
						}
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly Func<FeatureSet> parent;

		private readonly List<Feature> features;

		public abstract class Builder
		{
			protected FeatureSet BuildFeatureSet(IEnumerable<Feature> features, params Feature[] explicitFeatures)
			{
				return new FeatureSet(features.Union(explicitFeatures), null);
			}

			protected FeatureSet BuildFeatureSet(Func<FeatureSet> parent, IEnumerable<Feature> features, params Feature[] explicitFeatures)
			{
				return new FeatureSet(features.Union(explicitFeatures), null);
			}
		}
	}
}
