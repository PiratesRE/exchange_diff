using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class RUSMarshal
	{
		public static object[] MarshalWholeObject(ADObject recipient, IRecipientSession session, string[] allAttributes)
		{
			PropertyDefinition[] array = null;
			PropertyDefinition[] propDefs = RUSMarshal.LdapAttributesToPropertyDefinitions(allAttributes, recipient, out array);
			if (recipient.ObjectState == ObjectState.New)
			{
				recipient.StampPersistableDefaultValues();
			}
			object[] array2 = RUSMarshal.MarshalObject(propDefs, recipient);
			if (array.Length > 0 && recipient.ObjectState != ObjectState.New && recipient.ObjectState != ObjectState.Deleted)
			{
				bool useGlobalCatalog = session.UseGlobalCatalog;
				bool useConfigNC = session.UseConfigNC;
				session.UseConfigNC = false;
				if (string.Equals(recipient.Id.DescendantDN(1).Rdn.UnescapedName, "Configuration", StringComparison.OrdinalIgnoreCase))
				{
					ADObjectId configurationNamingContext = ADSession.GetConfigurationNamingContext(session.DomainController ?? recipient.OriginatingServer, session.NetworkCredential);
					if (recipient.DistinguishedName.EndsWith(configurationNamingContext.DistinguishedName, StringComparison.OrdinalIgnoreCase))
					{
						session.UseConfigNC = true;
					}
				}
				session.UseGlobalCatalog = false;
				ADRawEntry adrawEntry = null;
				try
				{
					adrawEntry = session.ReadADRawEntry((ADObjectId)recipient.Identity, array);
					if (adrawEntry == null)
					{
						throw new RusOperationException(DirectoryStrings.ExceptionObjectNotFound);
					}
				}
				finally
				{
					session.UseGlobalCatalog = useGlobalCatalog;
					session.UseConfigNC = useConfigNC;
				}
				object[] array3 = RUSMarshal.MarshalObject(array, adrawEntry);
				if (array3.Length > 0)
				{
					int num = array2.Length;
					Array.Resize<object>(ref array2, num + array3.Length);
					array3.CopyTo(array2, num);
				}
			}
			return array2;
		}

		internal static object PolicyTypeToGuids(PolicyType[] policies)
		{
			object[] array = new object[policies.Length];
			int num = 0;
			for (int i = 0; i < policies.Length; i++)
			{
				switch (policies[i])
				{
				case PolicyType.AddressLists:
					array[num] = "372bcee2-2ed8-4e3a-80df-1fcca462162a";
					break;
				case PolicyType.ProxyAddresses:
					array[num] = "4615aa41-ab4d-42a2-9bef-5829327f89b0";
					break;
				default:
					throw new NotImplementedException();
				}
				num++;
			}
			return array;
		}

		internal static object MarshalAttribute(ADPropertyDefinition property, object value)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (value == null)
			{
				return null;
			}
			object[] array;
			if (!property.IsMultivalued)
			{
				array = new object[2];
				array[0] = property.LdapDisplayName;
				if (property.IsBinary)
				{
					byte[] array2 = ADValueConvertor.ConvertValueToBinary(value, property.FormatProvider);
					array[1] = HexConverter.ByteArrayToHexString(array2);
				}
				else
				{
					ADObjectId adobjectId = value as ADObjectId;
					if (adobjectId != null)
					{
						array[1] = adobjectId.ToDNString();
					}
					else
					{
						array[1] = ADValueConvertor.ConvertValueToString(value, property.FormatProvider);
					}
				}
			}
			else
			{
				int num = 0;
				ArrayList arrayList = new ArrayList();
				IEnumerable enumerable = (IEnumerable)value;
				foreach (object obj in enumerable)
				{
					if (property.IsBinary)
					{
						byte[] array3 = ADValueConvertor.ConvertValueToBinary(obj, property.FormatProvider);
						arrayList.Add(HexConverter.ByteArrayToHexString(array3));
					}
					else
					{
						ADObjectId adobjectId2 = obj as ADObjectId;
						string value2;
						if (adobjectId2 != null)
						{
							value2 = adobjectId2.ToDNString();
						}
						else
						{
							value2 = ADValueConvertor.ConvertValueToString(obj, property.FormatProvider);
						}
						arrayList.Add(value2);
					}
					num++;
				}
				if (num == 0)
				{
					return null;
				}
				array = new object[num + 1];
				array[0] = property.LdapDisplayName;
				arrayList.CopyTo(array, 1);
			}
			return array;
		}

		internal static object UnMarshalAttribute(ADPropertyDefinition property, object[] values)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length < 1)
			{
				return null;
			}
			if (property.IsBinary)
			{
				byte[][] array = new byte[values.Length][];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = HexConverter.HexStringToByteArray((string)values[i]);
				}
				values = array;
			}
			return ADValueConvertor.GetValueFromDirectoryAttributeValues(property, values);
		}

		internal static object[] MarshalObject(PropertyDefinition[] propDefs, ADRawEntry recipient)
		{
			ArrayList arrayList = new ArrayList();
			int i = 0;
			while (i < propDefs.Length)
			{
				object obj = null;
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propDefs[i];
				if (adpropertyDefinition.GetterDelegate == null)
				{
					if (recipient.propertyBag.TryGetField(adpropertyDefinition, ref obj))
					{
						goto IL_3F;
					}
				}
				else
				{
					obj = recipient.propertyBag[adpropertyDefinition];
					if (obj != null)
					{
						goto IL_3F;
					}
				}
				IL_55:
				i++;
				continue;
				IL_3F:
				object obj2 = RUSMarshal.MarshalAttribute(adpropertyDefinition, obj);
				if (obj2 != null)
				{
					arrayList.Add(obj2);
					goto IL_55;
				}
				goto IL_55;
			}
			object[] array = new object[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		internal static ADRecipient UnMarshalObject(object rusResult, ADObject originalRecipient)
		{
			if (rusResult == null || !(rusResult is object[]))
			{
				return null;
			}
			SortedList sortedList = new SortedList(new CaseInsensitiveComparer(CultureInfo.InvariantCulture));
			foreach (object[] array2 in (object[])rusResult)
			{
				string[] array3 = new string[array2.Length - 1];
				Array.Copy(array2, 1, array3, 0, array2.Length - 1);
				sortedList.Add(array2[0], array3);
			}
			string[] array4 = new string[sortedList.Count];
			sortedList.Keys.CopyTo(array4, 0);
			ADRecipient adrecipient = new ADRecipient();
			List<ADPropertyDefinition> list = new List<ADPropertyDefinition>((ADPropertyDefinition[])RUSMarshal.LdapAttributesToPropertyDefinitions(RUSMarshal.SchemaInstance, array4));
			List<object> list2 = new List<object>(list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(RUSMarshal.UnMarshalAttribute(list[j], (object[])sortedList[list[j].LdapDisplayName]));
			}
			int k = list.Count - 1;
			while (k >= 0)
			{
				if (list[k] != ADRecipientSchema.WindowsEmailAddress)
				{
					goto IL_171;
				}
				SmtpAddress smtpAddress = (SmtpAddress)list2[k];
				if (((SmtpAddress)list2[k]).Length <= 256)
				{
					goto IL_171;
				}
				list.RemoveAt(k);
				list2.RemoveAt(k);
				ExTraceGlobals.RecipientUpdateServiceTracer.TraceDebug<string>((long)adrecipient.GetHashCode(), "RecipientUpdateService:: A Windows Email Address exceeding 256 characters for recipient \"{0}\" is generated and discarded", adrecipient.ToString());
				IL_286:
				k--;
				continue;
				IL_171:
				if (!list[k].IsMultivalued)
				{
					goto IL_286;
				}
				int num = (list2[k] == null) ? 0 : ((MultiValuedPropertyBase)list2[k]).Count;
				int num2 = (originalRecipient[list[k]] == null) ? 0 : ((MultiValuedPropertyBase)originalRecipient[list[k]]).Count;
				bool flag = num != num2;
				if (!flag && num != 0)
				{
					object[] array5 = (object[])RUSMarshal.MarshalAttribute(list[k], list2[k]);
					object[] array6 = (object[])RUSMarshal.MarshalAttribute(list[k], originalRecipient[list[k]]);
					for (int l = 1; l < array6.Length; l++)
					{
						if (-1 == Array.IndexOf<object>(array5, array6[l]))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					list.RemoveAt(k);
					list2.RemoveAt(k);
					ExTraceGlobals.RecipientUpdateServiceTracer.TraceDebug<string>((long)adrecipient.GetHashCode(), "RecipientUpdateService:: the value of 'msExchPoliciesIncluded' property of recipient \"{0}\" does NOT change at all.", adrecipient.ToString());
					goto IL_286;
				}
				goto IL_286;
			}
			adrecipient.SetProperties(list.ToArray(), list2.ToArray());
			return adrecipient;
		}

		internal static ADObjectSchema Schema()
		{
			return ADRecipientProperties.Instance;
		}

		internal static PropertyDefinition[] LdapAttributesToPropertyDefinitions(ADObjectSchema schema, string[] attributes)
		{
			PropertyDefinition[] array = new ADPropertyDefinition[attributes.Length];
			string[] array2 = new string[attributes.Length];
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < attributes.Length; i++)
			{
				ADPropertyDefinition adpropDefByLdapDisplayName = schema.GetADPropDefByLdapDisplayName(attributes[i]);
				if (adpropDefByLdapDisplayName != null)
				{
					array[num] = adpropDefByLdapDisplayName;
					num++;
				}
				else
				{
					array2[num2] = attributes[i];
					num2++;
				}
			}
			PropertyDefinition[] array3 = new ADPropertyDefinition[num];
			Array.Copy(array, array3, num);
			return array3;
		}

		internal static PropertyDefinition[] LdapAttributesToPropertyDefinitions(string[] attributes, ADObject recipient, out PropertyDefinition[] nonADRecipientPropDefs)
		{
			return RUSMarshal.LdapAttributesToPropertyDefinitions(RUSMarshal.SchemaInstance, attributes, recipient, out nonADRecipientPropDefs);
		}

		internal static PropertyDefinition[] LdapAttributesToPropertyDefinitions(ADObjectSchema schema, string[] attributes, ADObject recipient, out PropertyDefinition[] nonADRecipientPropDefs)
		{
			PropertyDefinition[] array = new ADPropertyDefinition[attributes.Length];
			PropertyDefinition[] array2 = new ADPropertyDefinition[attributes.Length];
			int num = 0;
			int num2 = 0;
			foreach (string text in attributes)
			{
				if (!string.IsNullOrEmpty(text))
				{
					bool flag = false;
					bool flag2 = false;
					object defaultValue = null;
					string[] array3 = text.Split(new char[]
					{
						':'
					}, 2);
					if (string.IsNullOrEmpty(array3[0]))
					{
						throw new ArgumentException(DirectoryStrings.ExArgumentException("attributes", text), "attributes");
					}
					if (array3.Length > 1)
					{
						if (array3[1].Equals("mv", StringComparison.OrdinalIgnoreCase))
						{
							flag2 = true;
						}
						else if (array3[1].Equals("binary", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
						else
						{
							if (!array3[1].Equals("mv-binary", StringComparison.OrdinalIgnoreCase))
							{
								throw new ArgumentException(DirectoryStrings.ExArgumentException("attributes", text), "attributes");
							}
							flag2 = true;
							flag = true;
						}
					}
					else
					{
						defaultValue = string.Empty;
					}
					ADPropertyDefinition adpropDefByLdapDisplayName = schema.GetADPropDefByLdapDisplayName(array3[0]);
					if (adpropDefByLdapDisplayName != null)
					{
						array[num] = adpropDefByLdapDisplayName;
						if (((ADPropertyDefinition)array[num]).IsBinary != flag || ((ADPropertyDefinition)array[num]).IsMultivalued != flag2)
						{
							throw new RusOperationException(DirectoryStrings.ExceptionSchemaMismatch(array3[0], ((ADPropertyDefinition)array[num]).IsBinary, ((ADPropertyDefinition)array[num]).IsMultivalued, flag, flag2));
						}
						num++;
					}
					else
					{
						Type type;
						if (!RUSMarshal.TryFindFilterOnlyDefinitionType(recipient, array3[0], out type))
						{
							type = (flag ? typeof(byte[]) : typeof(string));
						}
						ADPropertyDefinition adpropertyDefinition = new ADPropertyDefinition(array3[0], ExchangeObjectVersion.Exchange2003, type, array3[0], (flag ? ADPropertyDefinitionFlags.Binary : ADPropertyDefinitionFlags.None) | (flag2 ? ADPropertyDefinitionFlags.MultiValued : ADPropertyDefinitionFlags.None), defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
						array2[num2] = adpropertyDefinition;
						num2++;
					}
				}
			}
			PropertyDefinition[] array4 = new ADPropertyDefinition[num];
			Array.Copy(array, array4, num);
			nonADRecipientPropDefs = new ADPropertyDefinition[num2];
			Array.Copy(array2, nonADRecipientPropDefs, num2);
			return array4;
		}

		private static bool TryFindFilterOnlyDefinitionType(ADObject recipient, string attributeName, out Type type)
		{
			ADPropertyDefinition filterOnlyADPropDefByLdapDisplayName = recipient.Schema.GetFilterOnlyADPropDefByLdapDisplayName(attributeName);
			if (filterOnlyADPropDefByLdapDisplayName != null)
			{
				type = filterOnlyADPropDefByLdapDisplayName.Type;
				return true;
			}
			type = null;
			return false;
		}

		internal static string[] ObjectArrayToStringArray(object[] obj)
		{
			string[] array = new string[obj.Length];
			obj.CopyTo(array, 0);
			return array;
		}

		private const string RUS_CALCULATE_PROXIES = "4615aa41-ab4d-42a2-9bef-5829327f89b0";

		private const string RUS_CALCULATE_ADDRESSLISTS = "372bcee2-2ed8-4e3a-80df-1fcca462162a";

		internal static readonly ADObjectSchema SchemaInstance = RUSMarshal.Schema();
	}
}
