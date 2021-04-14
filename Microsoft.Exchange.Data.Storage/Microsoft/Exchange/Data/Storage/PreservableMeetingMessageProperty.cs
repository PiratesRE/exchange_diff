using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PreservableMeetingMessageProperty
	{
		static PreservableMeetingMessageProperty()
		{
			PreservableMeetingMessageProperty.CreatePreservableProperty(InternalSchema.Location, new ShouldPreservePropertyDelegate(PreservableMeetingMessageProperty.PreserveLocationTest));
			PreservableMeetingMessageProperty.CreatePreservableProperty(InternalSchema.Subject, new ShouldPreservePropertyDelegate(PreservableMeetingMessageProperty.IsRumTest));
			PreservableMeetingMessageProperty.CreatePreservableProperty(InternalSchema.SubjectPrefix, new ShouldPreservePropertyDelegate(PreservableMeetingMessageProperty.IsRumTest));
			PreservableMeetingMessageProperty.CreatePreservableProperty(InternalSchema.NormalizedSubject, new ShouldPreservePropertyDelegate(PreservableMeetingMessageProperty.IsRumTest));
		}

		private PreservableMeetingMessageProperty(StorePropertyDefinition propDef, ShouldPreservePropertyDelegate shouldPreserveTest, CopyPropertyDelegate copyMethod)
		{
			this.PropertyDefinition = propDef;
			this.ShouldPreserve = shouldPreserveTest;
			this.CopyProperty = copyMethod;
			PreservableMeetingMessageProperty.InstanceFromPropertyDefinition[propDef] = this;
		}

		private static PreservableMeetingMessageProperty CreatePreservableProperty(StorePropertyDefinition propDef, ShouldPreservePropertyDelegate shouldPreserveTest, CopyPropertyDelegate copyMethod)
		{
			return new PreservableMeetingMessageProperty(propDef, shouldPreserveTest, copyMethod);
		}

		private static PreservableMeetingMessageProperty CreatePreservableProperty(StorePropertyDefinition propDef, ShouldPreservePropertyDelegate shouldPreserveTest)
		{
			return PreservableMeetingMessageProperty.CreatePreservableProperty(propDef, shouldPreserveTest, new CopyPropertyDelegate(PreservableMeetingMessageProperty.DefaultCopy));
		}

		public static IEnumerable<PreservableMeetingMessageProperty> PreservableProperties
		{
			get
			{
				return PreservableMeetingMessageProperty.InstanceFromPropertyDefinition.Values;
			}
		}

		public static IEnumerable<StorePropertyDefinition> PreservablePropertyDefinitions
		{
			get
			{
				return new List<StorePropertyDefinition>(PreservableMeetingMessageProperty.InstanceFromPropertyDefinition.Keys);
			}
		}

		public static void CopyPreserving(PreservablePropertyContext context)
		{
			List<PreservableMeetingMessageProperty> list = new List<PreservableMeetingMessageProperty>();
			foreach (PreservableMeetingMessageProperty preservableMeetingMessageProperty in PreservableMeetingMessageProperty.PreservableProperties)
			{
				if (!preservableMeetingMessageProperty.ShouldPreserve(context))
				{
					list.Add(preservableMeetingMessageProperty);
				}
			}
			foreach (PreservableMeetingMessageProperty preservableMeetingMessageProperty2 in list)
			{
				preservableMeetingMessageProperty2.Copy(context);
			}
		}

		private static void DefaultCopy(PreservablePropertyContext context, StorePropertyDefinition prop)
		{
			CalendarItemBase.CopyPropertiesTo(context.MeetingRequest, context.CalendarItem, new PropertyDefinition[]
			{
				prop
			});
		}

		private static bool IsRumTest(PreservablePropertyContext context)
		{
			return context.MeetingRequest.IsRepairUpdateMessage;
		}

		private static bool PreserveLocationTest(PreservablePropertyContext context)
		{
			return context.MeetingRequest.IsRepairUpdateMessage && (context.OrganizerHighlights & ChangeHighlightProperties.Location) == ChangeHighlightProperties.None && PreservableMeetingMessageProperty.IsLocationConsistent(context);
		}

		private static bool IsLocationConsistent(PreservablePropertyContext context)
		{
			return context.CalendarItem.Location.Contains(context.MeetingRequest.Location);
		}

		public static Dictionary<StorePropertyDefinition, PreservableMeetingMessageProperty> InstanceFromPropertyDefinition
		{
			get
			{
				return PreservableMeetingMessageProperty.propertiesDictionary;
			}
		}

		private void Copy(PreservablePropertyContext context)
		{
			this.CopyProperty(context, this.PropertyDefinition);
		}

		public ShouldPreservePropertyDelegate ShouldPreserve { get; private set; }

		private StorePropertyDefinition PropertyDefinition { get; set; }

		private CopyPropertyDelegate CopyProperty { get; set; }

		private static Dictionary<StorePropertyDefinition, PreservableMeetingMessageProperty> propertiesDictionary = new Dictionary<StorePropertyDefinition, PreservableMeetingMessageProperty>();
	}
}
