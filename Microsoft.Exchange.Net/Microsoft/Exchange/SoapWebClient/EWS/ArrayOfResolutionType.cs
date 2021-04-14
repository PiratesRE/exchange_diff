﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ArrayOfResolutionType
	{
		[XmlElement("Resolution")]
		public ResolutionType[] Resolution;

		[XmlAttribute]
		public int IndexedPagingOffset;

		[XmlIgnore]
		public bool IndexedPagingOffsetSpecified;

		[XmlAttribute]
		public int NumeratorOffset;

		[XmlIgnore]
		public bool NumeratorOffsetSpecified;

		[XmlAttribute]
		public int AbsoluteDenominator;

		[XmlIgnore]
		public bool AbsoluteDenominatorSpecified;

		[XmlAttribute]
		public bool IncludesLastItemInRange;

		[XmlIgnore]
		public bool IncludesLastItemInRangeSpecified;

		[XmlAttribute]
		public int TotalItemsInView;

		[XmlIgnore]
		public bool TotalItemsInViewSpecified;
	}
}
