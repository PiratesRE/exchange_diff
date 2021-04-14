using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal abstract class MockObjectCreator
	{
		internal abstract IList<string> GetProperties(string fullName);

		protected abstract void FillProperties(Type type, PSObject psObject, object dummyObject, IList<string> properties);

		internal virtual object CreateDummyObject(Type type)
		{
			return Activator.CreateInstance(type, true);
		}

		internal virtual object TranslateToMockObject(Type type, PSObject psObject)
		{
			object obj = this.CreateDummyObject(type);
			IList<string> properties = this.GetProperties(type.FullName);
			this.FillProperties(type, psObject, obj, properties);
			return obj;
		}

		protected static object GetPropertyBasedOnDefinition(PropertyDefinition definition, object propertyValue)
		{
			if (!(propertyValue is PSObject) || !(((PSObject)propertyValue).BaseObject is IList))
			{
				return MockObjectCreator.GetSingleProperty(propertyValue, definition.Type);
			}
			IList list = ((PSObject)propertyValue).BaseObject as IList;
			if (definition.Type == typeof(ScheduleInterval[]))
			{
				List<ScheduleInterval> list2 = new List<ScheduleInterval>();
				foreach (object prop in list)
				{
					list2.Add((ScheduleInterval)MockObjectCreator.GetSingleProperty(prop, typeof(ScheduleInterval)));
				}
				return list2.ToArray();
			}
			MultiValuedPropertyBase multiValuedPropertyBase = null;
			if (definition.Type == typeof(ProxyAddress))
			{
				multiValuedPropertyBase = new ProxyAddressCollection();
			}
			else if (definition.Type == typeof(ApprovedApplicationCollection))
			{
				multiValuedPropertyBase = new ApprovedApplicationCollection();
			}
			else
			{
				if (definition.Type.FullName == "Microsoft.Exchange.Management.RecipientTasks.MailboxRights[]")
				{
					if (MockObjectCreator.mailboxRightsEnum == null)
					{
						string assemblyFile = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Management.Recipient.dll");
						MockObjectCreator.mailboxRightsEnum = Assembly.LoadFrom(assemblyFile).GetType("Microsoft.Exchange.Management.RecipientTasks.MailboxRights");
					}
					Array array = Array.CreateInstance(MockObjectCreator.mailboxRightsEnum, list.Count);
					int num = 0;
					foreach (object prop2 in list)
					{
						array.SetValue(MockObjectCreator.GetSingleProperty(prop2, MockObjectCreator.mailboxRightsEnum), num);
						num++;
					}
					return array;
				}
				multiValuedPropertyBase = new MultiValuedProperty<object>();
			}
			foreach (object prop3 in list)
			{
				multiValuedPropertyBase.Add(MockObjectCreator.GetSingleProperty(prop3, definition.Type));
			}
			return multiValuedPropertyBase;
		}

		protected static object GetSingleProperty(object prop, Type type)
		{
			if (prop == null)
			{
				return null;
			}
			object obj = null;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				obj = MockObjectCreator.GetSingleProperty(prop, type.GetGenericArguments()[0]);
			}
			else if (type == typeof(ADObjectId) && prop is PSObject)
			{
				obj = new ADObjectId(((PSObject)prop).Members["DistinguishedName"].Value.ToString(), new Guid(((PSObject)prop).Members["ObjectGuid"].Value.ToString()));
			}
			else if (type == typeof(EnhancedTimeSpan))
			{
				obj = EnhancedTimeSpan.Parse(prop.ToString());
			}
			else if (type == typeof(Unlimited<EnhancedTimeSpan>))
			{
				obj = Unlimited<EnhancedTimeSpan>.Parse(prop.ToString());
			}
			else if (type == typeof(ByteQuantifiedSize))
			{
				obj = ByteQuantifiedSize.Parse(prop.ToString());
			}
			else if (type == typeof(Unlimited<ByteQuantifiedSize>))
			{
				obj = Unlimited<ByteQuantifiedSize>.Parse(prop.ToString());
			}
			else if (type == typeof(Unlimited<int>))
			{
				obj = Unlimited<int>.Parse(prop.ToString());
			}
			else if (type == typeof(ProxyAddress))
			{
				obj = ProxyAddress.Parse(prop.ToString());
			}
			else if (type == typeof(SmtpAddress))
			{
				obj = new SmtpAddress(prop.ToString());
			}
			else if (type == typeof(SmtpDomain))
			{
				obj = SmtpDomain.Parse(prop.ToString());
			}
			else if (type == typeof(CountryInfo))
			{
				obj = CountryInfo.Parse(prop.ToString());
			}
			else if (type == typeof(SharingPolicyDomain))
			{
				obj = SharingPolicyDomain.Parse(prop.ToString());
			}
			else if (type == typeof(ApprovedApplication))
			{
				obj = ApprovedApplication.Parse(prop.ToString());
			}
			else if (type == typeof(SmtpDomainWithSubdomains))
			{
				obj = SmtpDomainWithSubdomains.Parse(prop.ToString());
			}
			else if (type == typeof(UMLanguage))
			{
				obj = UMLanguage.Parse(prop.ToString());
			}
			else if (type == typeof(UMSmartHost))
			{
				obj = UMSmartHost.Parse(prop.ToString());
			}
			else if (type == typeof(ScheduleInterval))
			{
				obj = ScheduleInterval.Parse(prop.ToString());
			}
			else if (type == typeof(NumberFormat))
			{
				obj = NumberFormat.Parse(prop.ToString());
			}
			else if (type == typeof(DialGroupEntry))
			{
				obj = DialGroupEntry.Parse(prop.ToString());
			}
			else if (type == typeof(CustomMenuKeyMapping))
			{
				obj = CustomMenuKeyMapping.Parse(prop.ToString());
			}
			else if (type == typeof(HolidaySchedule))
			{
				obj = HolidaySchedule.Parse(prop.ToString());
			}
			else if (type == typeof(UMTimeZone))
			{
				obj = UMTimeZone.Parse(prop.ToString());
			}
			else if (type == typeof(ServerVersion))
			{
				obj = ServerVersion.ParseFromSerialNumber(prop.ToString());
			}
			else if (type == typeof(X509Certificate2))
			{
				obj = new X509Certificate2(((PSObject)prop).Members["RawData"].Value as byte[]);
			}
			else if (type == typeof(LocalizedString))
			{
				obj = new LocalizedString(prop.ToString());
			}
			else if (type == typeof(ExchangeObjectVersion))
			{
				obj = ExchangeObjectVersion.Parse(prop.ToString());
			}
			else if (type == typeof(bool))
			{
				obj = bool.Parse(prop.ToString());
			}
			else if (type == typeof(SecurityPrincipalIdParameter))
			{
				obj = new SecurityPrincipalIdParameter(prop.ToString());
			}
			else if (type == typeof(ActiveDirectoryAccessRule))
			{
				obj = (prop as ActiveDirectoryAccessRule);
			}
			else if (type == typeof(ObjectId))
			{
				string text = prop.ToString();
				if (!ADObjectId.IsValidDistinguishedName(text) && text.Contains("/"))
				{
					text = MockObjectCreator.ConvertDNFromTreeStructure(text);
				}
				obj = new ADObjectId(text);
			}
			else if (type.IsEnum)
			{
				try
				{
					obj = Enum.Parse(type, prop.ToString());
				}
				catch (ArgumentException)
				{
					obj = Enum.GetValues(type).GetValue(0);
				}
			}
			return obj ?? prop;
		}

		private static string ConvertDNFromTreeStructure(string treeDN)
		{
			if (string.IsNullOrEmpty(treeDN))
			{
				throw new ArgumentException("The input param of tree structure identity should not be null.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = treeDN.Split(new char[]
			{
				'/'
			});
			if (array.Length > 2)
			{
				string text = array[1];
				if (text == "Domain Controllers" || text == "Microsoft Exchange Security Groups")
				{
					MockObjectCreator.BuildCNPartOfDN(array, stringBuilder, array.Length - 1, 1);
					MockObjectCreator.BuildOUPartOfDN(text, stringBuilder);
				}
				else if (text == "Microsoft Exchange Hosted Organizations")
				{
					MockObjectCreator.BuildCNPartOfDN(array, stringBuilder, array.Length - 1, 2);
					MockObjectCreator.BuildOUPartOfDN(array[2], stringBuilder);
					MockObjectCreator.BuildOUPartOfDN(text, stringBuilder);
				}
				else
				{
					MockObjectCreator.BuildCNPartOfDN(array, stringBuilder, array.Length - 1, 0);
				}
				MockObjectCreator.BuildDCPartOfDN(array[0].Split(new char[]
				{
					'.'
				}), stringBuilder);
				return stringBuilder.ToString().TrimEnd(new char[]
				{
					','
				});
			}
			return treeDN;
		}

		private static void BuildCNPartOfDN(string[] treeItems, StringBuilder dnBuilder, int toIndex, int fromIndex)
		{
			for (int i = toIndex; i > fromIndex; i--)
			{
				dnBuilder.Append("CN=");
				dnBuilder.Append(treeItems[i]);
				dnBuilder.Append(',');
			}
		}

		private static void BuildDCPartOfDN(string[] treeItems, StringBuilder dnBuilder)
		{
			foreach (string value in treeItems)
			{
				dnBuilder.Append("DC=");
				dnBuilder.Append(value);
				dnBuilder.Append(',');
			}
		}

		private static void BuildOUPartOfDN(string ouPart, StringBuilder dnBuilder)
		{
			dnBuilder.Append("OU=");
			dnBuilder.Append(ouPart);
			dnBuilder.Append(',');
		}

		private const string PrefixForCN = "CN=";

		private const string PrefixForDC = "DC=";

		private const string PrefixForOU = "OU=";

		private const string DomainControllers = "Domain Controllers";

		private const string SecurityGroups = "Microsoft Exchange Security Groups";

		private const string HostedOrganizations = "Microsoft Exchange Hosted Organizations";

		private static Type mailboxRightsEnum;
	}
}
