using System;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	internal interface IPixel<TValue> where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
	{
		float Luminance { get; }

		float Intensity { get; }

		int Bands { get; }

		TValue this[int band]
		{
			get;
		}

		int CompareByIntensity(IPixel<TValue> another);

		bool Equals(object other);

		int GetHashCode();
	}
}
