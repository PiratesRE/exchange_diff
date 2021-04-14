using System;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner;

namespace Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection
{
	[DataContract]
	[Serializable]
	internal class SalientObject
	{
		[DataMember]
		public RegionInfo<ArgbPixel, byte, LabTile> Region { get; set; }

		[DataMember]
		public float SaliencePortion { get; set; }

		[DataMember]
		public bool IsPrimary { get; set; }
	}
}
