﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UMReportAudioMetricsAverageCountersType
	{
		public AudioMetricsAverageType NMOS;

		public AudioMetricsAverageType NMOSDegradation;

		public AudioMetricsAverageType Jitter;

		public AudioMetricsAverageType PercentPacketLoss;

		public AudioMetricsAverageType RoundTrip;

		public AudioMetricsAverageType BurstLossDuration;

		public long TotalAudioQualityCallsSampled;
	}
}
