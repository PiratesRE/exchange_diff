using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal sealed class SupervisionMaps
	{
		public SupervisionMaps(ADRawEntry entry, IList<string> requiredTags)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (requiredTags == null)
			{
				throw new ArgumentNullException("requiredTags");
			}
			ExTraceGlobals.SupervisionTracer.TraceDebug(0L, "Creating supervision maps for {0}", new object[]
			{
				entry[ADObjectSchema.DistinguishedName]
			});
			this.internalRecipientSupervisionMap = new Dictionary<string, List<ADObjectId>>(requiredTags.Count, StringComparer.OrdinalIgnoreCase);
			this.dlSupervisionMap = new Dictionary<string, List<ADObjectId>>(requiredTags.Count, StringComparer.OrdinalIgnoreCase);
			this.oneOffSupervisionMap = new Dictionary<string, List<SmtpAddress>>(requiredTags.Count, StringComparer.OrdinalIgnoreCase);
			foreach (string key in requiredTags)
			{
				this.internalRecipientSupervisionMap.Add(key, new List<ADObjectId>());
				this.dlSupervisionMap.Add(key, new List<ADObjectId>());
				this.oneOffSupervisionMap.Add(key, new List<SmtpAddress>());
			}
			this.ConstructRecipientSupervisionMap(entry, true);
			this.ConstructRecipientSupervisionMap(entry, false);
			this.ConstructOneOffSupervisionMap(entry);
			ExTraceGlobals.SupervisionTracer.TraceDebug(0L, "Supervision maps for {0} created", new object[]
			{
				entry[ADObjectSchema.DistinguishedName]
			});
		}

		public Dictionary<string, List<ADObjectId>> InternalRecipientSupervisionMap
		{
			get
			{
				return this.internalRecipientSupervisionMap;
			}
		}

		public Dictionary<string, List<ADObjectId>> DlSupervisionMap
		{
			get
			{
				return this.dlSupervisionMap;
			}
		}

		public Dictionary<string, List<SmtpAddress>> OneOffSupervisionMap
		{
			get
			{
				return this.oneOffSupervisionMap;
			}
		}

		private void ConstructRecipientSupervisionMap(ADRawEntry entry, bool internalRecipient)
		{
			MultiValuedProperty<ADObjectIdWithString> multiValuedProperty;
			Dictionary<string, List<ADObjectId>> dictionary;
			if (internalRecipient)
			{
				multiValuedProperty = (MultiValuedProperty<ADObjectIdWithString>)entry[ADRecipientSchema.InternalRecipientSupervisionList];
				dictionary = this.internalRecipientSupervisionMap;
			}
			else
			{
				multiValuedProperty = (MultiValuedProperty<ADObjectIdWithString>)entry[ADRecipientSchema.DLSupervisionList];
				dictionary = this.dlSupervisionMap;
			}
			SupervisionListEntryConstraint supervisionListEntryConstraint = new SupervisionListEntryConstraint(false);
			foreach (ADObjectIdWithString adobjectIdWithString in multiValuedProperty)
			{
				PropertyConstraintViolationError propertyConstraintViolationError = supervisionListEntryConstraint.Validate(adobjectIdWithString, null, null);
				if (propertyConstraintViolationError != null)
				{
					ExTraceGlobals.SupervisionTracer.TraceDebug<string, ADObjectIdWithString, LocalizedString>(0L, "Ignoring {0} supervision list entry {1} due to validation error {2}", internalRecipient ? "internal recipient" : "DL", adobjectIdWithString, propertyConstraintViolationError.Description);
				}
				else
				{
					string[] array = adobjectIdWithString.StringValue.Split(new char[]
					{
						SupervisionListEntryConstraint.Delimiter
					});
					foreach (string text in array)
					{
						if (!text.Equals(string.Empty) && dictionary.ContainsKey(text))
						{
							List<ADObjectId> list = dictionary[text];
							if (!list.Contains(adobjectIdWithString.ObjectIdValue))
							{
								list.Add(adobjectIdWithString.ObjectIdValue);
							}
						}
					}
				}
			}
		}

		private void ConstructOneOffSupervisionMap(ADRawEntry entry)
		{
			MultiValuedProperty<ADObjectIdWithString> multiValuedProperty = (MultiValuedProperty<ADObjectIdWithString>)entry[ADRecipientSchema.OneOffSupervisionList];
			SupervisionListEntryConstraint supervisionListEntryConstraint = new SupervisionListEntryConstraint(true);
			foreach (ADObjectIdWithString adobjectIdWithString in multiValuedProperty)
			{
				PropertyConstraintViolationError propertyConstraintViolationError = supervisionListEntryConstraint.Validate(adobjectIdWithString, null, null);
				if (propertyConstraintViolationError != null)
				{
					ExTraceGlobals.SupervisionTracer.TraceDebug<ADObjectIdWithString, LocalizedString>(0L, "Ignoring one off supervision list entry {0} due to validation error {1}", adobjectIdWithString, propertyConstraintViolationError.Description);
				}
				else
				{
					string[] array = adobjectIdWithString.StringValue.Split(new char[]
					{
						SupervisionListEntryConstraint.Delimiter
					});
					SmtpAddress item = new SmtpAddress(array[array.Length - 1]);
					for (int i = 0; i < array.Length - 1; i++)
					{
						string text = array[i];
						if (!text.Equals(string.Empty) && this.oneOffSupervisionMap.ContainsKey(text))
						{
							List<SmtpAddress> list = this.oneOffSupervisionMap[text];
							if (!list.Contains(item))
							{
								list.Add(item);
							}
						}
					}
				}
			}
		}

		private Dictionary<string, List<ADObjectId>> internalRecipientSupervisionMap;

		private Dictionary<string, List<ADObjectId>> dlSupervisionMap;

		private Dictionary<string, List<SmtpAddress>> oneOffSupervisionMap;
	}
}
