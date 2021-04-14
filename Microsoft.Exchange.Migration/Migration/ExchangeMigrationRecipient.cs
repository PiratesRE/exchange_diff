using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal abstract class ExchangeMigrationRecipient : IMigrationSerializable
	{
		protected ExchangeMigrationRecipient(MigrationUserRecipientType recipientType)
		{
			this.Properties = new Dictionary<PropTag, object>(ExchangeMigrationRecipient.AllRecipientProperties.Length);
			this.RecipientType = recipientType;
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return this.GetPropertyValue<MigrationUserRecipientType>(PropTag.DisplayType);
			}
			private set
			{
				this.SetPropertyValue(PropTag.DisplayType, (int)value);
			}
		}

		public virtual string Identifier
		{
			get
			{
				foreach (PropTag key in new PropTag[]
				{
					PropTag.SmtpAddress,
					PropTag.EmailAddress
				})
				{
					object obj;
					if (this.Properties.TryGetValue(key, out obj))
					{
						string text = obj as string;
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
				return null;
			}
		}

		public abstract HashSet<PropTag> SupportedProperties { get; }

		public abstract HashSet<PropTag> RequiredProperties { get; }

		public bool DoesADObjectExist
		{
			get
			{
				return this.doesADObjectExist;
			}
			set
			{
				this.doesADObjectExist = value;
			}
		}

		PropertyDefinition[] IMigrationSerializable.PropertyDefinitions
		{
			get
			{
				return ExchangeMigrationRecipient.ExchangeMigrationRecipientPropertyDefinitions;
			}
		}

		public bool TryGetPropertyValue<T>(PropTag proptag, out T value)
		{
			object obj;
			if (this.Properties.TryGetValue(proptag, out obj))
			{
				try
				{
					value = (T)((object)obj);
					return true;
				}
				catch (InvalidCastException)
				{
				}
			}
			value = default(T);
			return false;
		}

		public T GetPropertyValue<T>(PropTag proptag)
		{
			return (T)((object)this.Properties[proptag]);
		}

		public void SetPropertyValue(PropTag proptag, object value)
		{
			this.Properties[proptag] = value;
			this.UpdateDependentProperties(proptag);
		}

		public virtual bool TryValidateRequiredProperties(out LocalizedString errorMessage)
		{
			foreach (PropTag propTag in this.RequiredProperties)
			{
				object obj;
				if (!this.Properties.TryGetValue(propTag, out obj))
				{
					errorMessage = ServerStrings.MigrationNSPIMissingRequiredField(propTag);
					return false;
				}
			}
			errorMessage = LocalizedString.Empty;
			return true;
		}

		public override string ToString()
		{
			return this.RecipientType + ":" + this.Identifier;
		}

		public virtual void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			PersistableDictionary persistableDictionary = new PersistableDictionary();
			foreach (KeyValuePair<PropTag, object> keyValuePair in this.Properties)
			{
				persistableDictionary.Add((long)((ulong)keyValuePair.Key), keyValuePair.Value);
			}
			MigrationHelper.SetDictionaryProperty(message, MigrationBatchMessageSchema.MigrationJobItemExchangeRecipientProperties, persistableDictionary);
			message[MigrationBatchMessageSchema.MigrationJobItemADObjectExists] = this.DoesADObjectExist;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("ExchangeMigrationRecipient");
			foreach (PropTag propTag in this.Properties.Keys)
			{
				string expandedName = "PropTag" + propTag.ToString();
				object obj = this.Properties[propTag];
				object[] array = obj as object[];
				if (array != null)
				{
					XElement xelement2 = new XElement(expandedName);
					foreach (object obj2 in array)
					{
						xelement2.Add(new XElement("Element", new object[]
						{
							new XAttribute("type", obj2.GetType()),
							obj2
						}));
					}
					xelement.Add(xelement2);
				}
				else
				{
					xelement.Add(new XElement(expandedName, new object[]
					{
						new XAttribute("type", obj.GetType()),
						obj
					}));
				}
			}
			return xelement;
		}

		public virtual bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			DictionaryBase dictionaryProperty = MigrationHelper.GetDictionaryProperty(message, MigrationBatchMessageSchema.MigrationJobItemExchangeRecipientProperties, false);
			if (dictionaryProperty == null)
			{
				return false;
			}
			foreach (object obj in dictionaryProperty)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				try
				{
					long num = (long)dictionaryEntry.Key;
					PropTag propTag = (PropTag)num;
					if (this.IsPropertySupported(propTag))
					{
						this.Properties[propTag] = dictionaryEntry.Value;
					}
					else
					{
						MigrationLogger.Log(MigrationEventType.Warning, "ExchangeMigrationRecipient.ReadFromMessageItem found an unsupported PropTag '{0}' msgid: '{1}'- versioning issue?", new object[]
						{
							propTag,
							message.Id
						});
					}
				}
				catch (InvalidCastException innerException)
				{
					throw new InvalidDataException("Invalid Exchange Recipient Properties. Message ID: " + message.Id, innerException);
				}
			}
			this.DoesADObjectExist = message.GetValueOrDefault<bool>(MigrationBatchMessageSchema.MigrationJobItemADObjectExists, false);
			return true;
		}

		internal static bool TryCreate(MigrationUserRecipientType recipientType, out ExchangeMigrationRecipient recipient, bool isPAW = true)
		{
			switch (recipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				recipient = new ExchangeMigrationMailUserRecipient();
				return true;
			case MigrationUserRecipientType.Contact:
				recipient = new ExchangeMigrationMailContactRecipient();
				return true;
			case MigrationUserRecipientType.Group:
				if (isPAW)
				{
					recipient = new ExchangeMigrationGroupRecipient();
				}
				else
				{
					recipient = new LegacyExchangeMigrationGroupRecipient();
				}
				return true;
			case MigrationUserRecipientType.Mailuser:
				recipient = new ExchangeMigrationMailEnabledUserRecipient();
				return true;
			}
			recipient = null;
			return false;
		}

		internal static ExchangeMigrationRecipient Create(IMigrationStoreObject message, MigrationUserRecipientType recipientType, bool isPAW = true)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			ExchangeMigrationRecipient exchangeMigrationRecipient;
			if (!ExchangeMigrationRecipient.TryCreate(recipientType, out exchangeMigrationRecipient, isPAW))
			{
				return null;
			}
			if (exchangeMigrationRecipient.ReadFromMessageItem(message))
			{
				return exchangeMigrationRecipient;
			}
			MigrationUtil.AssertOrThrow(isPAW, "Pre-PAW we expect to always find a recipient because it is discovered at job-item creation time.", new object[0]);
			return null;
		}

		internal bool IsPropertySupported(PropTag propTag)
		{
			return this.SupportedProperties.Contains(propTag);
		}

		internal bool TryGetProxyAddresses(out string[] proxyAddresses)
		{
			bool flag = this.TryGetPropertyValue<string[]>((PropTag)2148470815U, out proxyAddresses);
			if (proxyAddresses != null)
			{
				List<string> list = new List<string>(proxyAddresses.Length);
				foreach (string text in proxyAddresses)
				{
					if (!string.IsNullOrEmpty(text) && !text.StartsWith("EUM:", StringComparison.OrdinalIgnoreCase) && (!text.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase) || SmtpAddress.IsValidSmtpAddress(text.Substring(5))))
					{
						list.Add(text);
					}
				}
				if (list.Count < proxyAddresses.Length)
				{
					proxyAddresses = list.ToArray();
				}
			}
			if (!ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("MigrationSourceMailboxLegacyExchangeDNStampingEnabled"))
			{
				return flag;
			}
			string text2;
			bool flag2 = this.TryGetPropertyValue<string>(PropTag.EmailAddress, out text2);
			if (!flag && !flag2)
			{
				return false;
			}
			if (!flag2)
			{
				return flag;
			}
			text2 = "X500:" + text2;
			if (proxyAddresses != null)
			{
				foreach (string a in proxyAddresses)
				{
					if (string.Equals(a, text2, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				Array.Resize<string>(ref proxyAddresses, proxyAddresses.Length + 1);
			}
			else
			{
				proxyAddresses = new string[1];
			}
			proxyAddresses[proxyAddresses.Length - 1] = text2;
			return true;
		}

		internal bool IsPropertyRequired(PropTag propTag)
		{
			return this.RequiredProperties.Contains(propTag);
		}

		protected virtual void UpdateDependentProperties(PropTag proptag)
		{
		}

		public const PropTag PropTagHomeMDB = (PropTag)2147876895U;

		public const PropTag PropTagPublicDelegates = (PropTag)2148864031U;

		public const PropTag PropTagManager = (PropTag)2147811359U;

		public const PropTag PropTagRoomCapacity = (PropTag)134676483U;

		public const PropTag PropTagUMSpokenName = (PropTag)2361524482U;

		public const PropTag PropTagManagedBy = (PropTag)2148270111U;

		public const PropTag PropTagTargetAddress = (PropTag)2148597791U;

		public const PropTag PropTagCountryName = (PropTag)2154364959U;

		public const PropTag PropTagUserCulture = (PropTag)2359230495U;

		public const PropTag PropTagNickname = PropTag.Account;

		public static readonly long[] AllRecipientProperties = new long[]
		{
			956301315L,
			956628995L,
			975372319L,
			973602847L,
			974520351L,
			974651423L,
			805371935L,
			805503007L,
			973471775L,
			973733919L,
			974913567L,
			974716959L,
			972947487L,
			974192671L,
			974585887L,
			2148470815L,
			973668383L,
			975765535L,
			975634463L,
			975699999L,
			975831071L,
			805568543L,
			2147876895L,
			2147811359L,
			2148864031L,
			134676483L,
			2361524482L,
			2148270111L,
			2154364959L,
			2359230495L,
			973078559L
		};

		public static readonly PropertyDefinition[] ExchangeMigrationRecipientPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemExchangeRecipientIndex,
				MigrationBatchMessageSchema.MigrationJobItemExchangeMsExchHomeServerName,
				MigrationBatchMessageSchema.MigrationJobItemExchangeRecipientProperties,
				MigrationBatchMessageSchema.MigrationJobItemLastProvisionedMemberIndex,
				MigrationBatchMessageSchema.MigrationJobItemADObjectExists
			},
			LegacyExchangeMigrationGroupRecipient.GroupPropertyDefinitions
		});

		protected readonly Dictionary<PropTag, object> Properties;

		private bool doesADObjectExist;
	}
}
