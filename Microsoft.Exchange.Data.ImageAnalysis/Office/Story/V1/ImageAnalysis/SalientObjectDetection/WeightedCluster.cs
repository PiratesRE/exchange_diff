using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.CommonMath;

namespace Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection
{
	[DataContract]
	[Serializable]
	internal class WeightedCluster<T>
	{
		public WeightedCluster(Func<T, Box2D> getOutline, Func<T, float> getDensity)
		{
			this.getOutline = getOutline;
			this.getDensity = getDensity;
			this.Reset();
		}

		public WeightedCluster(Func<T, Box2D> getOutline, Func<T, float> getDensity, IEnumerable<T> clusteredRegions) : this(getOutline, getDensity)
		{
			this.Compute(clusteredRegions);
		}

		public WeightedCluster(Func<T, Box2D> getOutline, Func<T, float> getDensity, params T[] clusteredRegions) : this(getOutline, getDensity)
		{
			this.Compute(clusteredRegions);
		}

		public float OutlineDensity
		{
			get
			{
				if (this.ElementCount <= 0)
				{
					return 0f;
				}
				return this.Mass / this.Outline.Area;
			}
		}

		[DataMember]
		public float MaximumDensity { get; private set; }

		[DataMember]
		public Box2D Outline { get; private set; }

		[DataMember]
		public Vector2 Center { get; private set; }

		public Vector2 GeometricalCenter
		{
			get
			{
				return this.Outline.Center;
			}
		}

		[DataMember]
		public float Mass { get; private set; }

		[DataMember]
		public int ElementCount { get; private set; }

		public void Reset()
		{
			this.MaximumDensity = 0f;
			this.Mass = 0f;
			this.Outline = Box2D.Null;
			this.Center = Vector2.Zero;
			this.ElementCount = 0;
		}

		public void Add(T region)
		{
			if (this.getDensity == null || this.getOutline == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "This cluster is locked and can not accept new elements. You can only access properties on it or combine it with another cluster.", new object[0]));
			}
			float num = this.getDensity(region);
			if (num > 0f)
			{
				Box2D box = this.getOutline(region);
				float num2 = num * box.Area;
				this.Center = (this.Center * this.Mass + box.Center * num2) / (this.Mass + num2);
				this.Mass += num2;
				this.Outline += box;
				this.MaximumDensity = Math.Max(this.MaximumDensity, num);
				this.ElementCount++;
			}
		}

		public void Compute(IEnumerable<T> clusteredRegions)
		{
			if (clusteredRegions == null)
			{
				throw new ArgumentNullException("clusteredRegions");
			}
			foreach (T region in clusteredRegions)
			{
				this.Add(region);
			}
		}

		public WeightedCluster<T> Lock()
		{
			this.getDensity = null;
			this.getOutline = null;
			return this;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} center at {1} of mass {2} with {3} elements.", new object[]
			{
				this.Outline,
				this.Center,
				this.Mass,
				this.ElementCount
			});
		}

		[NonSerialized]
		private Func<T, Box2D> getOutline;

		[NonSerialized]
		private Func<T, float> getDensity;
	}
}
