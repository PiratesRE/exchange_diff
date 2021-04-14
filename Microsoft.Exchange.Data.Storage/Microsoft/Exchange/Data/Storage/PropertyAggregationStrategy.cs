using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class PropertyAggregationStrategy
	{
		protected PropertyAggregationStrategy(params StorePropertyDefinition[] propertyDefinitions)
		{
			List<PropertyDependency> list = new List<PropertyDependency>(propertyDefinitions.Length);
			foreach (StorePropertyDefinition storePropertyDefinition in propertyDefinitions)
			{
				SmartPropertyDefinition smartPropertyDefinition = storePropertyDefinition as SmartPropertyDefinition;
				NativeStorePropertyDefinition nativeStorePropertyDefinition = storePropertyDefinition as NativeStorePropertyDefinition;
				if (smartPropertyDefinition != null)
				{
					list.AddRange(smartPropertyDefinition.Dependencies);
				}
				else if (nativeStorePropertyDefinition != null)
				{
					list.Add(new PropertyDependency(nativeStorePropertyDefinition, PropertyDependencyType.NeedForRead));
				}
			}
			this.dependencies = list.ToArray();
		}

		public PropertyDependency[] Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		public void Aggregate(PropertyDefinition aggregatedProperty, PropertyAggregationContext context, PropertyBag target)
		{
			Util.ThrowOnNullArgument(aggregatedProperty, "aggregatedProperty");
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(target, "target");
			PropertyAggregationStrategy.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Aggregating property {0}", aggregatedProperty.Name);
			object obj;
			if (this.TryAggregate(context, out obj))
			{
				this.Trace(obj, aggregatedProperty, context);
				target[aggregatedProperty] = obj;
				return;
			}
			PropertyAggregationStrategy.Tracer.TraceDebug<string>((long)this.GetHashCode(), "No value returned for property {0}", aggregatedProperty.Name);
		}

		protected abstract bool TryAggregate(PropertyAggregationContext context, out object value);

		private static string ObjectValueToString(object value)
		{
			string text = value as string;
			if (text != null)
			{
				return text;
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				return BitConverter.ToString(array);
			}
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("{");
				foreach (object value2 in enumerable)
				{
					if (stringBuilder.Length > 1)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(value2);
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}
			return value.ToString();
		}

		private static string GetPropertyValuePair(PropertyDefinition property, object value)
		{
			return PropertyAggregationStrategy.GetPropertyValuePair(property, PropertyAggregationStrategy.ObjectValueToString(value));
		}

		private static string GetPropertyValuePair(PropertyDefinition property, string value)
		{
			return string.Concat(new string[]
			{
				"[",
				property.Name,
				":",
				value,
				"]"
			});
		}

		private void Trace(object aggregatedValue, PropertyDefinition aggregatedProperty, PropertyAggregationContext context)
		{
			if (PropertyAggregationStrategy.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("{");
				bool flag = true;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					foreach (PropertyDependency propertyDependency in this.Dependencies)
					{
						object obj = storePropertyBag.TryGetProperty(propertyDependency.Property);
						if (obj != null)
						{
							PropertyError propertyError = obj as PropertyError;
							if (propertyError != null)
							{
								if (propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
								{
									stringBuilder.Append(PropertyAggregationStrategy.GetPropertyValuePair(propertyDependency.Property, "ERROR:" + propertyError.PropertyErrorCode.ToString()));
								}
							}
							else
							{
								stringBuilder.Append(PropertyAggregationStrategy.GetPropertyValuePair(propertyDependency.Property, obj));
							}
						}
					}
				}
				stringBuilder.Append("}");
				PropertyAggregationStrategy.Tracer.TraceDebug<string, StringBuilder>((long)this.GetHashCode(), "Aggregated={0} Sources={1}", PropertyAggregationStrategy.GetPropertyValuePair(aggregatedProperty, aggregatedValue), stringBuilder);
			}
		}

		public static readonly PropertyAggregationStrategy None = new PropertyAggregationStrategy.NoAggregation(Array<StorePropertyDefinition>.Empty);

		public static readonly PropertyAggregationStrategy EntryIdsProperty = new PropertyAggregationStrategy.EntryIdsAggregation();

		public static readonly PropertyAggregationStrategy ItemClassesProperty = new PropertyAggregationStrategy.ItemClassesAggregation();

		private static readonly Trace Tracer = ExTraceGlobals.AggregationTracer;

		private readonly PropertyDependency[] dependencies;

		private sealed class NoAggregation : PropertyAggregationStrategy
		{
			public NoAggregation(StorePropertyDefinition[] dependencies) : base(dependencies)
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = null;
				return false;
			}
		}

		internal sealed class CreationTimeAggregation : PropertyAggregationStrategy
		{
			public CreationTimeAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.CreationTime
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				ExDateTime exDateTime = ExDateTime.MinValue;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					ExDateTime valueOrDefault = storePropertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
					if (valueOrDefault > exDateTime)
					{
						exDateTime = valueOrDefault;
					}
				}
				value = exDateTime;
				return true;
			}
		}

		internal sealed class SingleValuePropertyAggregation : PropertyAggregationStrategy
		{
			public SingleValuePropertyAggregation(SelectionStrategy selectionStrategy) : base(selectionStrategy.Dependencies)
			{
				this.selectionStrategy = selectionStrategy;
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				IStorePropertyBag storePropertyBag = null;
				foreach (IStorePropertyBag storePropertyBag2 in context.Sources)
				{
					if (this.selectionStrategy.IsSelectable(storePropertyBag2) && (storePropertyBag == null || this.selectionStrategy.HasPriority(storePropertyBag2, storePropertyBag)))
					{
						storePropertyBag = storePropertyBag2;
					}
				}
				if (storePropertyBag != null)
				{
					value = this.selectionStrategy.GetValue(storePropertyBag);
				}
				else
				{
					value = null;
				}
				return value != null;
			}

			private readonly SelectionStrategy selectionStrategy;
		}

		internal sealed class EntryIdsAggregation : PropertyAggregationStrategy
		{
			public EntryIdsAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.EntryId
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<StoreObjectId> list = new List<StoreObjectId>(context.Sources.Count);
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					byte[] valueOrDefault = storePropertyBag.GetValueOrDefault<byte[]>(InternalSchema.EntryId, null);
					if (valueOrDefault != null)
					{
						StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Unknown);
						if (storeObjectId != null)
						{
							list.Add(storeObjectId);
						}
					}
				}
				value = list.ToArray();
				return true;
			}
		}

		internal sealed class ItemClassesAggregation : PropertyAggregationStrategy
		{
			public ItemClassesAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.ItemClass
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, StoreObjectType.Message.ToString());
					hashSet.Add(valueOrDefault);
				}
				value = hashSet.ToArray<string>();
				return true;
			}
		}
	}
}
