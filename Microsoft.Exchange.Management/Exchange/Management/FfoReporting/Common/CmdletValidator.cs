using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	internal sealed class CmdletValidator : Attribute
	{
		static CmdletValidator()
		{
			CmdletValidator.validationFunctions.Add("ValidateDlpPolicy", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateDlpPolicy(args.Property, args.Task, args.ConfigSession);
			});
			CmdletValidator.validationFunctions.Add("ValidateDomain", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateDomain(args.Property, args.Task, args.ConfigSession);
			});
			CmdletValidator.validationFunctions.Add("ValidateOrganization", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateOrganization(args.Property, args.Task);
			});
			CmdletValidator.validationFunctions.Add("ValidateRequiredField", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateRequiredField(args.Property, args.Task);
			});
			CmdletValidator.validationFunctions.Add("ValidateTransportRule", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateTransportRule(args.Property, args.Task, args.ConfigSession);
			});
			CmdletValidator.validationFunctions.Add("ScrubDlp", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ScrubDlp(args.Property, args.Task, (Schema.EventTypes)validator.parameters[0]);
			});
			CmdletValidator.validationFunctions.Add("ValidateIntRange", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateIntRange(args.Property, args.Task, (int)validator.parameters[0], (int)validator.parameters[1]);
			});
			CmdletValidator.validationFunctions.Add("ValidateEmailAddress", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateEmailAddress(args.Property, args.Task, null, (CmdletValidator.EmailAddress)validator.parameters[0], (CmdletValidator.WildcardValidationOptions)validator.parameters[1], CmdletValidator.EmailAcceptedDomainOptions.SkipVerify);
			});
			CmdletValidator.validationFunctions.Add("ValidateEmailAddressWithDomain", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateEmailAddress(args.Property, args.Task, args.ConfigSession, (CmdletValidator.EmailAddress)validator.parameters[0], (CmdletValidator.WildcardValidationOptions)validator.parameters[1], (CmdletValidator.EmailAcceptedDomainOptions)validator.parameters[2]);
			});
			CmdletValidator.validationFunctions.Add("ValidateEnum", delegate(CmdletValidator validator, CmdletValidator.CmdletValidatorArgs args)
			{
				validator.ValidateEnum(args.Property, args.Task, (Type)validator.parameters[0], (validator.parameters.Length > 1) ? Convert.ToUInt64(validator.parameters[1]) : ulong.MaxValue);
			});
		}

		internal CmdletValidator(string method, params object[] parameters)
		{
			this.methodName = method;
			this.parameters = parameters;
			this.ValidatorType = CmdletValidator.ValidatorTypes.Preprocessing;
		}

		public CmdletValidator.ValidatorTypes ValidatorType { get; set; }

		public Strings.IDs ErrorMessage
		{
			get
			{
				if (this.errorMessageId != null)
				{
					return this.errorMessageId.Value;
				}
				throw new InvalidOperationException("ErrorMessage must be set before accessing it.");
			}
			set
			{
				this.errorMessageId = new Strings.IDs?(value);
			}
		}

		internal void Validate(CmdletValidator.CmdletValidatorArgs args)
		{
			CmdletValidator.ValidationDelegate validationDelegate = CmdletValidator.validationFunctions[this.methodName];
			validationDelegate(this, args);
		}

		internal void ValidateOrganization(PropertyInfo property, object task)
		{
			if (!(property.GetValue(task, null) is OrganizationIdParameter))
			{
				Type type = task.GetType();
				OrganizationId organizationId = type.GetProperty("CurrentOrganizationId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(task, null) as OrganizationId;
				if (organizationId == null || organizationId.OrganizationalUnit == null)
				{
					this.ThrowError(property);
				}
				OrganizationIdParameter value = new OrganizationIdParameter(organizationId.OrganizationalUnit.Name);
				property.SetValue(task, value, null);
			}
		}

		internal void ValidateEnum(PropertyInfo property, object task, Type enumType, ulong enumerationSubset)
		{
			IList<string> list;
			if (this.TryGetValues<string>(property, task, out list))
			{
				int num;
				if (list.Count == 1 && int.TryParse(list[0], out num))
				{
					this.ThrowError(property);
				}
				string value = string.Join(",", list);
				try
				{
					if (!string.IsNullOrEmpty(value))
					{
						ulong num2 = Convert.ToUInt64(Enum.Parse(enumType, value, true));
						ulong num3 = ~enumerationSubset;
						if ((num3 & num2) != 0UL)
						{
							this.ThrowError(property);
						}
					}
					return;
				}
				catch (ArgumentException)
				{
					this.ThrowError(property);
					return;
				}
			}
			ParameterAttribute[] array = (ParameterAttribute[])property.GetCustomAttributes(typeof(ParameterAttribute), false);
			if (array != null)
			{
				if (!array.Any((ParameterAttribute a) => a.Mandatory) && array.Count<ParameterAttribute>() != 0)
				{
					return;
				}
			}
			this.ThrowError(property);
		}

		internal void ValidateIntRange(PropertyInfo property, object task, int min, int max)
		{
			int num = (int)property.GetValue(task, null);
			if (min > num || max < num)
			{
				this.ThrowError(property);
			}
		}

		internal void ValidateEmailAddress(PropertyInfo property, object task, IConfigDataProvider configSession, CmdletValidator.EmailAddress addressType, CmdletValidator.WildcardValidationOptions wildcardOptions, CmdletValidator.EmailAcceptedDomainOptions domainOptions = CmdletValidator.EmailAcceptedDomainOptions.SkipVerify)
		{
			IList<string> list;
			if (this.TryGetValues<string>(property, task, out list))
			{
				LocalizedString message = (addressType == CmdletValidator.EmailAddress.Sender) ? Strings.InvalidSenderAddress : Strings.InvalidRecipientAddress;
				IEnumerable<string> source = new string[0];
				if (domainOptions == CmdletValidator.EmailAcceptedDomainOptions.Verify)
				{
					AcceptedDomainIdParameter acceptedDomainIdParameter = AcceptedDomainIdParameter.Parse("*");
					source = from domain in acceptedDomainIdParameter.GetObjects<AcceptedDomain>(null, configSession)
					select domain.DomainName.Domain.ToLower();
				}
				foreach (string address in list)
				{
					bool flag2;
					bool flag = Schema.Utilities.ValidateEmailAddress(address, out flag2);
					if (flag && flag2)
					{
						flag &= (wildcardOptions == CmdletValidator.WildcardValidationOptions.Allow);
						if (flag && list.Count > 1)
						{
							message = ((addressType == CmdletValidator.EmailAddress.Sender) ? Strings.CannotCombineWildcardSenderAddress : Strings.CannotCombineWildcardRecipientAddress);
							flag = false;
						}
					}
					if (flag && domainOptions == CmdletValidator.EmailAcceptedDomainOptions.Verify)
					{
						SmtpAddress smtpAddress = new SmtpAddress(address);
						if (!source.Contains(smtpAddress.Domain, StringComparer.InvariantCultureIgnoreCase))
						{
							flag = false;
							message = Strings.EmailAddressNotInAcceptedDomain(address);
						}
					}
					if (!flag)
					{
						if (this.errorMessageId == null)
						{
							throw new InvalidExpressionException(message);
						}
						this.ThrowError(property);
					}
				}
			}
		}

		internal void ScrubDlp(PropertyInfo property, object task, Schema.EventTypes eventTypes)
		{
			Task task2 = task as Task;
			IList<string> collection;
			if ((task2 == null || !Schema.Utilities.HasDlpRole(task2)) && this.TryGetValues<string>(property, task, out collection))
			{
				List<string> list = new List<string>(collection);
				if (list.Count == 0)
				{
					list.AddRange(Schema.Utilities.Split(eventTypes));
				}
				else
				{
					Schema.Utilities.RemoveDlpEventTypes(list);
				}
				if (list.Count == 0)
				{
					this.ThrowError(property);
				}
				property.SetValue(task, new MultiValuedProperty<string>(list), null);
			}
		}

		internal void ValidateRequiredField(PropertyInfo property, object task)
		{
			object value = property.GetValue(task, null);
			if (value is IList)
			{
				IList list = (IList)value;
				if (list.Count > 0)
				{
					return;
				}
			}
			else if (property.PropertyType.IsValueType)
			{
				object obj = Activator.CreateInstance(property.PropertyType);
				if (!value.Equals(obj))
				{
					return;
				}
			}
			else if (value != null)
			{
				return;
			}
			throw new InvalidExpressionException(Strings.RequiredReportingParameter(property.Name));
		}

		internal void ValidateDomain(PropertyInfo property, object task, IConfigDataProvider configSession)
		{
			IList<Fqdn> list;
			if (this.TryGetValues<Fqdn>(property, task, out list) && list.Count > 0)
			{
				if (configSession == null)
				{
					throw new NullReferenceException("ValidateDomain requires an IConfigDataProvider");
				}
				AcceptedDomainIdParameter acceptedDomainIdParameter = AcceptedDomainIdParameter.Parse("*");
				HashSet<string> acceptedDomains = new HashSet<string>(from domain in acceptedDomainIdParameter.GetObjects<AcceptedDomain>(null, configSession)
				select domain.DomainName.Domain.ToLower());
				if (!list.All((Fqdn domain) => acceptedDomains.Contains(domain.Domain.ToLower())))
				{
					this.ThrowError(property);
				}
			}
		}

		internal void ValidateDlpPolicy(PropertyInfo property, object task, IConfigDataProvider configSession)
		{
			IList<string> list;
			if (!DatacenterRegistry.IsForefrontForOffice() && this.TryGetValues<string>(property, task, out list) && list.Count > 0)
			{
				if (configSession == null)
				{
					throw new NullReferenceException("ValidateDlpPolicy requires an IConfigDataProvider");
				}
				HashSet<string> installedDlp = new HashSet<string>(from dlp in DlpUtils.GetInstalledTenantDlpPolicies(configSession)
				select dlp.Name.ToLower());
				if (!list.All((string dlp) => installedDlp.Contains(dlp.ToLower())))
				{
					this.ThrowError(property);
				}
			}
		}

		internal void ValidateTransportRule(PropertyInfo property, object task, IConfigDataProvider configSession)
		{
			IList<string> list;
			if (this.TryGetValues<string>(property, task, out list) && list.Count > 0)
			{
				if (configSession == null)
				{
					throw new NullReferenceException("ValidateTransportRule requires an IConfigDataProvider");
				}
				HashSet<string> installedRules = new HashSet<string>(from rule in DlpUtils.GetTransportRules(configSession, (Rule rule) => true)
				select rule.Name.ToLower());
				if (!list.All((string rule) => installedRules.Contains(rule.ToLower())))
				{
					this.ThrowError(property);
				}
			}
		}

		private void ThrowError(PropertyInfo property)
		{
			if (this.errorMessageId != null)
			{
				throw new InvalidExpressionException(Strings.GetLocalizedString(this.errorMessageId.Value));
			}
			throw new NullReferenceException(string.Format("The error message is not defined for the property {0}.", property.Name));
		}

		private bool TryGetValues<TType>(PropertyInfo property, object task, out IList<TType> values)
		{
			object value = property.GetValue(task, null);
			if (value is IList<TType>)
			{
				values = (IList<TType>)value;
			}
			else
			{
				if (value == null)
				{
					values = null;
					return false;
				}
				values = new List<TType>
				{
					(TType)((object)value)
				};
			}
			return true;
		}

		private static readonly Dictionary<string, CmdletValidator.ValidationDelegate> validationFunctions = new Dictionary<string, CmdletValidator.ValidationDelegate>();

		private readonly string methodName;

		private object[] parameters;

		private Strings.IDs? errorMessageId;

		internal static class Methods
		{
			internal const string DlpPolicy = "ValidateDlpPolicy";

			internal const string Domain = "ValidateDomain";

			internal const string EmailAddress = "ValidateEmailAddress";

			internal const string EmailAddressWithDomain = "ValidateEmailAddressWithDomain";

			internal const string Enum = "ValidateEnum";

			internal const string IntRange = "ValidateIntRange";

			internal const string Organization = "ValidateOrganization";

			internal const string Required = "ValidateRequiredField";

			internal const string ScrubDlp = "ScrubDlp";

			internal const string TransportRule = "ValidateTransportRule";
		}

		internal enum ValidatorTypes
		{
			Preprocessing,
			Postprocessing,
			PostprocessingWithConfigSession
		}

		internal enum EmailAddress
		{
			Sender,
			Recipient
		}

		internal enum WildcardValidationOptions
		{
			Allow,
			Disallow
		}

		internal enum EmailAcceptedDomainOptions
		{
			Verify,
			SkipVerify
		}

		private delegate void ValidationDelegate(CmdletValidator instance, CmdletValidator.CmdletValidatorArgs args);

		internal class CmdletValidatorArgs
		{
			internal CmdletValidatorArgs(PropertyInfo property, object task, Func<IConfigDataProvider> configDataProviderFunction = null)
			{
				this.Property = property;
				this.Task = task;
				this.configDataProviderFunction = configDataProviderFunction;
			}

			internal PropertyInfo Property { get; private set; }

			internal object Task { get; private set; }

			internal IConfigDataProvider ConfigSession
			{
				get
				{
					if (this.configDataProviderFunction == null)
					{
						return null;
					}
					return this.configDataProviderFunction();
				}
			}

			private Func<IConfigDataProvider> configDataProviderFunction;
		}
	}
}
