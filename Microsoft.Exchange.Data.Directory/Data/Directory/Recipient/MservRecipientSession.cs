using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.MSN.Hotmail.Mserv;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MservRecipientSession : IDisposable
	{
		public MservRecipientSession(bool isReadOnly = true)
		{
			this.isReadOnly = isReadOnly;
			this.mservEndpointConfig = ((MservRecipientSession.mservEndPoint != null) ? MservRecipientSession.mservEndPoint : ConfigBase<AdDriverConfigSchema>.GetConfig<string>("MservEndpoint"));
			this.mservClient = this.BuildMservClient(this.mservEndpointConfig);
		}

		public ADRawEntry FindADRawEntryByPuid(ulong puid, IEnumerable<MServPropertyDefinition> properties)
		{
			return this.LookUpByKey(MservRecord.KeyFromPuid(puid), properties);
		}

		public ADRawEntry FindADRawEntryByEmailAddress(string emailAddress, IEnumerable<MServPropertyDefinition> properties)
		{
			return this.LookUpByKey(emailAddress, properties);
		}

		private ADRawEntry LookUpByKey(string key, IEnumerable<MServPropertyDefinition> properties)
		{
			List<MservRecord> list = new List<MservRecord>(4);
			string text;
			MservRecord mservRecord = this.Read(key, 0, out text);
			if (mservRecord != null)
			{
				list.Add(mservRecord);
				if (!string.IsNullOrEmpty(text))
				{
					MservRecord item = new MservRecord(text, mservRecord.ResourceId, null, mservRecord.Key, mservRecord.Flags);
					list.Add(item);
				}
			}
			if (mservRecord != null)
			{
				string text2;
				MservRecord mservRecord2 = this.Read((mservRecord == null) ? key : mservRecord.Key, 7, out text2);
				if (mservRecord2 != null)
				{
					list.Add(mservRecord2);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			ulong puid;
			if (!list[0].TryGetPuid(out puid))
			{
				return null;
			}
			return this.ADRawEntryFromMservRecords(puid, list, properties, this.mservEndpointConfig, new List<ValidationError>());
		}

		private MservRecord Read(string key, byte resourceId, out string aliasKey)
		{
			bool flag = false;
			bool flag2 = this.ReadFromMaster;
			aliasKey = null;
			do
			{
				try
				{
					string key2 = key;
					string value;
					byte flags;
					string text;
					this.mservClient.ReadRecord(key, resourceId, flag2, ref value, ref flags, ref text);
					if (!string.IsNullOrEmpty(text))
					{
						key2 = text;
						aliasKey = key;
					}
					return new MservRecord(key2, resourceId, value, null, flags);
				}
				catch (MservClientException ex)
				{
					if (ex.ResponseCode == 500)
					{
						flag = (this.RetryReadsOnMaster && !flag2);
						flag2 = true;
					}
				}
			}
			while (flag);
			return null;
		}

		private void SwapRecords(string key, MservRecord record1, MservRecord record2)
		{
			this.mservClient.SwapRecords(key, record1.ResourceId, record1.Value, record2.ResourceId, record2.Value);
		}

		private ADRawEntry ADRawEntryFromMservRecords(ulong puid, IList<MservRecord> mservRecords, IEnumerable<MServPropertyDefinition> properties, string originatingServerName, List<ValidationError> errors)
		{
			ADObjectId adobjectIdFromPuid = ConsumerIdentityHelper.GetADObjectIdFromPuid(puid);
			ADPropertyBag adpropertyBag = new ADPropertyBag(this.isReadOnly, 16);
			adpropertyBag.SetField(ADObjectSchema.Id, adobjectIdFromPuid);
			adpropertyBag.SetField(MServRecipientSchema.Puid, puid);
			foreach (MservRecord mservRecord in mservRecords)
			{
				object value = mservRecord;
				ProviderPropertyDefinition providerPropertyDefinition;
				switch (mservRecord.ResourceId)
				{
				case 0:
					if (mservRecord.SourceKey == null)
					{
						providerPropertyDefinition = MServRecipientSchema.MservPrimaryRecord;
					}
					else
					{
						providerPropertyDefinition = MServRecipientSchema.MservEmailAddressesRecord;
						value = new MultiValuedProperty<MservRecord>
						{
							mservRecord
						};
					}
					break;
				case 1:
					providerPropertyDefinition = MServRecipientSchema.MservSoftDeletedPrimaryRecord;
					break;
				case 2:
				case 3:
				case 5:
				case 6:
					goto IL_D0;
				case 4:
					providerPropertyDefinition = MServRecipientSchema.MservCalendarRecord;
					break;
				case 7:
					providerPropertyDefinition = MServRecipientSchema.MservSecondaryRecord;
					break;
				case 8:
					providerPropertyDefinition = MServRecipientSchema.MservSoftDeletedCalendarRecord;
					break;
				default:
					goto IL_D0;
				}
				PropertyValidationError propertyValidationError = providerPropertyDefinition.ValidateValue(value, true);
				if (propertyValidationError != null)
				{
					errors.Add(propertyValidationError);
				}
				adpropertyBag.SetField(providerPropertyDefinition, value);
				continue;
				IL_D0:
				throw new NotSupportedException("Unexpected record received:" + mservRecord.ToString());
			}
			ADRawEntry adrawEntry = new ADRawEntry(adpropertyBag);
			adrawEntry.OriginatingServer = originatingServerName;
			adrawEntry.WhenReadUTC = new DateTime?(DateTime.UtcNow);
			adrawEntry.IsCached = false;
			adrawEntry.ValidateRead(errors, properties);
			adrawEntry.ResetChangeTracking(true);
			return adrawEntry;
		}

		public void Save(ADRawEntry instanceToSave)
		{
			bool flag;
			List<MservChange> list = this.BuildChangeList(instanceToSave, out flag);
			if (flag)
			{
				this.SwapRecords(list[0], list[1]);
				return;
			}
			foreach (MservChange mservChange in list)
			{
				MservRecord newValue = mservChange.NewValue;
				MservRecord oldValue = mservChange.OldValue;
				if (newValue != null && oldValue == null)
				{
					if (newValue.SourceKey == null)
					{
						this.mservClient.AddRecord(newValue.Key, newValue.ResourceId, newValue.Value, newValue.Flags);
					}
					else
					{
						this.mservClient.AddAlias(newValue.Key, newValue.SourceKey, newValue.ResourceId);
					}
				}
				else if (newValue != null && oldValue != null)
				{
					this.mservClient.UpdateExistingRecord(newValue.Key, newValue.ResourceId, newValue.Value, oldValue.Value, newValue.Flags, newValue.UpdatedFlagsMask, oldValue.Flags, newValue.UpdatedFlagsMask);
				}
				else if (oldValue.SourceKey == null)
				{
					this.mservClient.DeleteRecord(oldValue.Key, oldValue.ResourceId, oldValue.Value, oldValue.Flags, byte.MaxValue);
				}
				else
				{
					this.mservClient.DeleteAlias(oldValue.Key, oldValue.SourceKey, oldValue.ResourceId);
				}
			}
		}

		private void SwapRecords(MservChange record1, MservChange record2)
		{
			this.mservClient.SwapRecords(record1.OldValue.Key, record1.OldValue.ResourceId, record1.OldValue.Value, record2.OldValue.ResourceId, record2.OldValue.Value);
		}

		private List<MservChange> BuildChangeList(ADRawEntry instanceToSave, out bool isSwap)
		{
			List<MservChange> list = new List<MservChange>();
			isSwap = false;
			foreach (PropertyDefinition propertyDefinition in MservRecipientSession.MServRecipientSchema.AllProperties)
			{
				MServPropertyDefinition mservPropertyDefinition = (MServPropertyDefinition)propertyDefinition;
				if (!mservPropertyDefinition.IsCalculated && (instanceToSave.propertyBag.IsChanged(mservPropertyDefinition) || mservPropertyDefinition.IsMultivalued) && !mservPropertyDefinition.IsTaskPopulated)
				{
					object obj = null;
					instanceToSave.propertyBag.TryGetField(mservPropertyDefinition, ref obj);
					if (!mservPropertyDefinition.IsMultivalued || (obj != null && ((MultiValuedPropertyBase)obj).Changed))
					{
						ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "MservRecipientSession::Save - updating {0}", mservPropertyDefinition.Name);
						if (!mservPropertyDefinition.IsMultivalued)
						{
							object obj2;
							instanceToSave.propertyBag.TryGetOriginalValue(mservPropertyDefinition, out obj2);
							MservRecord mservRecord = (MservRecord)obj;
							MservRecord mservRecord2 = (MservRecord)obj2;
							if (mservRecord != null && mservRecord.IsEmpty)
							{
								mservRecord = null;
							}
							list.Add(new MservChange(mservRecord, mservRecord2));
							if (mservRecord != null && mservRecord2 != null && mservRecord.ResourceId != mservRecord2.ResourceId)
							{
								isSwap = true;
								if (mservRecord.Value != mservRecord2.Value)
								{
									throw new MservOperationException(DirectoryStrings.SwapShouldNotChangeValues(mservRecord2.Value, mservRecord2.ResourceId, mservRecord.Value, mservRecord.ResourceId));
								}
							}
						}
						else
						{
							MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)obj;
							if (multiValuedPropertyBase.WasCleared)
							{
								throw new MservOperationException(DirectoryStrings.NoResetOrAssignedMvp);
							}
							foreach (object obj3 in multiValuedPropertyBase.Removed)
							{
								list.Add(new MservChange(null, (MservRecord)obj3));
							}
							foreach (object obj4 in multiValuedPropertyBase.Added)
							{
								list.Add(new MservChange((MservRecord)obj4, null));
							}
						}
					}
				}
			}
			if (isSwap)
			{
				if (list.Count != 2)
				{
					throw new MservOperationException(DirectoryStrings.BadSwapOperationCount(list.Count));
				}
				if (list[0].OldValue.ResourceId != list[1].NewValue.ResourceId || list[0].NewValue.ResourceId != list[1].OldValue.ResourceId)
				{
					throw new MservOperationException(DirectoryStrings.BadSwapResourceIds(list[0].OldValue.ResourceId, list[0].NewValue.ResourceId, list[1].OldValue.ResourceId, list[1].NewValue.ResourceId));
				}
			}
			return list;
		}

		private MservClient BuildMservClient(string pservIP)
		{
			return new MservClient(new MservClientSettings(35, IPAddress.Parse(pservIP))
			{
				ShortSocketTimeout = MservRecipientSession.ShortSocketTimeout,
				LongSocketTimeout = MservRecipientSession.LongSocketTimeout
			});
		}

		public void Dispose()
		{
			this.mservClient.Dispose();
			this.mservClient = null;
		}

		private const byte AppId = 35;

		private MservClient mservClient;

		private readonly bool isReadOnly = true;

		public bool RetryReadsOnMaster = true;

		public bool ReadFromMaster;

		private static TimeSpan ShortSocketTimeout = TimeSpan.FromSeconds(10.0);

		private static TimeSpan LongSocketTimeout = TimeSpan.FromSeconds(10.0);

		private static string mservEndPoint = null;

		private string mservEndpointConfig;

		private static MServRecipientSchema MServRecipientSchema = new MServRecipientSchema();
	}
}
