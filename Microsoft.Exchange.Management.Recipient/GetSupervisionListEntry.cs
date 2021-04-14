using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SupervisionListEntry")]
	public sealed class GetSupervisionListEntry : GetRecipientObjectTask<RecipientIdParameter, ADRecipient>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true)]
		public override RecipientIdParameter Identity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter]
		public string Tag
		{
			get
			{
				return (string)base.Fields["Tag"];
			}
			set
			{
				base.Fields["Tag"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			List<SupervisionListEntry> list = new List<SupervisionListEntry>();
			ADRecipient dataObject = (ADRecipient)base.GetDataObject(this.Identity);
			this.GetADRecipientEntries(dataObject, false, list);
			this.GetADRecipientEntries(dataObject, true, list);
			this.GetExternalAddressEntries(dataObject, list);
			this.WriteResult<SupervisionListEntry>(list);
			TaskLogger.LogExit();
		}

		private void GetADRecipientEntries(ADRecipient dataObject, bool isGroup, List<SupervisionListEntry> results)
		{
			MultiValuedProperty<ADObjectIdWithString> multiValuedProperty = isGroup ? ((MultiValuedProperty<ADObjectIdWithString>)dataObject[ADRecipientSchema.DLSupervisionList]) : ((MultiValuedProperty<ADObjectIdWithString>)dataObject[ADRecipientSchema.InternalRecipientSupervisionList]);
			SupervisionRecipientType recipientType = isGroup ? SupervisionRecipientType.DistributionGroup : SupervisionRecipientType.IndividualRecipient;
			ADObjectId[] array = new ADObjectId[multiValuedProperty.Count];
			for (int i = 0; i < multiValuedProperty.Count; i++)
			{
				array[i] = multiValuedProperty[i].ObjectIdValue;
			}
			Result<ADRawEntry>[] array2 = base.TenantGlobalCatalogSession.ReadMultiple(array, new PropertyDefinition[]
			{
				ADObjectSchema.RawName
			});
			for (int j = 0; j < multiValuedProperty.Count; j++)
			{
				ADObjectIdWithString adobjectIdWithString = multiValuedProperty[j];
				SupervisionListEntryConstraint supervisionListEntryConstraint = new SupervisionListEntryConstraint(false);
				if (supervisionListEntryConstraint.Validate(adobjectIdWithString, null, null) == null)
				{
					string[] array3 = adobjectIdWithString.StringValue.Split(new char[]
					{
						SupervisionListEntryConstraint.Delimiter
					});
					string entryName = (string)array2[j].Data[ADObjectSchema.Name];
					foreach (string text in array3)
					{
						if (this.Tag == null || this.Tag.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							SupervisionListEntry item = null;
							try
							{
								item = new SupervisionListEntry(entryName, text, recipientType);
							}
							catch (ArgumentNullException exception)
							{
								base.WriteError(exception, (ErrorCategory)1000, null);
							}
							results.Add(item);
						}
					}
				}
			}
		}

		private void GetExternalAddressEntries(ADRecipient dataObject, List<SupervisionListEntry> results)
		{
			MultiValuedProperty<ADObjectIdWithString> multiValuedProperty = (MultiValuedProperty<ADObjectIdWithString>)dataObject[ADRecipientSchema.OneOffSupervisionList];
			SupervisionRecipientType recipientType = SupervisionRecipientType.ExternalAddress;
			foreach (ADObjectIdWithString adobjectIdWithString in multiValuedProperty)
			{
				SupervisionListEntryConstraint supervisionListEntryConstraint = new SupervisionListEntryConstraint(true);
				if (supervisionListEntryConstraint.Validate(adobjectIdWithString, null, null) == null)
				{
					string[] array = adobjectIdWithString.StringValue.Split(new char[]
					{
						SupervisionListEntryConstraint.Delimiter
					});
					SmtpAddress smtpAddress = new SmtpAddress(array[array.Length - 1]);
					for (int i = 0; i < array.Length - 1; i++)
					{
						string text = array[i];
						if (this.Tag == null || this.Tag.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							SupervisionListEntry item = new SupervisionListEntry(smtpAddress.ToString(), text, recipientType);
							results.Add(item);
						}
					}
				}
			}
		}
	}
}
