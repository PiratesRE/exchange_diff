using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis
{
	internal abstract class AnalysisMember : IXmlSerializable
	{
		public AnalysisMember(Func<AnalysisMember> parent, ConcurrencyType runAs, Analysis analysis, IEnumerable<Feature> features)
		{
			this.parent = new Lazy<AnalysisMember>(delegate()
			{
				if (parent != null)
				{
					return parent();
				}
				return analysis.RootAnalysisMember;
			}, true);
			this.RunAs = runAs;
			this.Analysis = analysis;
			this.features = new List<Feature>(features);
			this.featuresHaveBeenInherited = false;
			this.startTimeTicks = default(DateTime).Ticks;
			this.stopTimeTicks = default(DateTime).Ticks;
		}

		public AnalysisMember Parent
		{
			get
			{
				return this.parent.Value;
			}
		}

		public ConcurrencyType RunAs { get; private set; }

		public Analysis Analysis { get; private set; }

		public abstract Type ValueType { get; }

		public ExDateTime StartTime
		{
			get
			{
				long ticks = Thread.VolatileRead(ref this.startTimeTicks);
				return new ExDateTime(ExTimeZone.UtcTimeZone, ticks);
			}
			protected set
			{
				long utcTicks = value.UtcTicks;
				Interlocked.Exchange(ref this.startTimeTicks, utcTicks);
			}
		}

		public ExDateTime StopTime
		{
			get
			{
				long ticks = Thread.VolatileRead(ref this.stopTimeTicks);
				return new ExDateTime(ExTimeZone.UtcTimeZone, ticks);
			}
			protected set
			{
				long utcTicks = value.UtcTicks;
				Interlocked.Exchange(ref this.stopTimeTicks, utcTicks);
			}
		}

		public IEnumerable<Feature> Features
		{
			get
			{
				IEnumerable<Feature> result;
				lock (this.features)
				{
					if (this.featuresHaveBeenInherited)
					{
						result = this.features;
					}
					else
					{
						this.InheritFeatures();
						this.CombineModes();
						this.CombineRoles();
						this.featuresHaveBeenInherited = true;
						result = this.features;
					}
				}
				return result;
			}
		}

		public string Name
		{
			get
			{
				return this.Analysis.GetAnalysisMemberName(this);
			}
		}

		public abstract void Start();

		public abstract IEnumerable<Result> GetResults();

		public IEnumerable<AnalysisMember> AncestorsAndSelf()
		{
			for (AnalysisMember current = this; current != null; current = current.Parent)
			{
				yield return current;
			}
			yield break;
		}

		private void InheritFeatures()
		{
			if (this.Parent == null)
			{
				return;
			}
			using (IEnumerator<Feature> enumerator = (from y in this.Parent.Features
			where y.IsInheritable
			select y).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Feature feature = enumerator.Current;
					if (!feature.AllowsMultiple)
					{
						if (!(from x in this.features
						where x.GetType() == feature.GetType()
						select x).Any<Feature>())
						{
							this.features.Add(feature);
						}
					}
					else
					{
						this.features.Add(feature);
					}
				}
			}
		}

		private void CombineModes()
		{
			AppliesToModeFeature[] array = this.features.OfType<AppliesToModeFeature>().ToArray<AppliesToModeFeature>();
			if (array.Length > 0)
			{
				foreach (AppliesToModeFeature item in array)
				{
					this.features.Remove(item);
				}
				SetupMode setupMode = SetupMode.None;
				foreach (AppliesToModeFeature appliesToModeFeature in array)
				{
					setupMode |= appliesToModeFeature.Mode;
				}
				this.features.Add(new AppliesToModeFeature(setupMode));
			}
		}

		private void CombineRoles()
		{
			AppliesToRoleFeature[] array = this.features.OfType<AppliesToRoleFeature>().ToArray<AppliesToRoleFeature>();
			if (array.Length > 0)
			{
				foreach (AppliesToRoleFeature item in array)
				{
					this.features.Remove(item);
				}
				SetupRole setupRole = SetupRole.None;
				foreach (AppliesToRoleFeature appliesToRoleFeature in array)
				{
					setupRole |= appliesToRoleFeature.Role;
				}
				this.features.Add(new AppliesToRoleFeature(setupRole));
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException("Read from XML is not supported.");
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement((this is Rule) ? "rule" : "setting");
			writer.WriteStartAttribute("name");
			writer.WriteValue(this.Name);
			writer.WriteEndAttribute();
			writer.WriteStartAttribute("type");
			writer.WriteValue(this.ValueType.FullName);
			writer.WriteEndAttribute();
			writer.WriteStartAttribute("starttime");
			writer.WriteValue(this.StartTime.ToString());
			writer.WriteEndAttribute();
			writer.WriteStartAttribute("stoptime");
			writer.WriteValue(this.StopTime.ToString());
			writer.WriteEndAttribute();
			writer.WriteStartElement("features");
			foreach (Feature feature in this.Features)
			{
				writer.WriteStartElement("feature");
				IXmlSerializable xmlSerializable = feature as IXmlSerializable;
				if (xmlSerializable != null)
				{
					xmlSerializable.WriteXml(writer);
				}
				else
				{
					writer.WriteValue(feature.ToString());
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("results");
			foreach (Result result in this.GetResults())
			{
				writer.WriteStartElement("result");
				if (result.HasException)
				{
					writer.WriteStartElement("exception");
					writer.WriteValue(result.Exception.ToString());
				}
				else
				{
					writer.WriteStartElement("value");
					IXmlSerializable xmlSerializable2 = result.ValueAsObject as IXmlSerializable;
					if (xmlSerializable2 != null)
					{
						xmlSerializable2.WriteXml(writer);
					}
					else
					{
						object valueAsObject = result.ValueAsObject;
						if (valueAsObject != null)
						{
							writer.WriteValue(result.ValueAsObject.ToString());
						}
					}
				}
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private Lazy<AnalysisMember> parent;

		private List<Feature> features;

		private bool featuresHaveBeenInherited;

		private long startTimeTicks;

		private long stopTimeTicks;
	}
}
