using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	[Serializable]
	public class ScopeSettings : ExchangeDataObject
	{
		public ScopeSettings()
		{
		}

		internal ScopeSettings(ExchangeADServerSettings settings)
		{
			base.PropertyBag.Clear();
			if (settings != null)
			{
				this.ForestViewEnabled = settings.ForestViewEnabled;
				this.OrganizationalUnit = settings.OrganizationalUnit;
			}
			else
			{
				this.ForestViewEnabled = true;
			}
			base.ResetChangeTracking();
		}

		internal override ObjectSchema Schema
		{
			get
			{
				return ScopeSettings.schema;
			}
		}

		public bool ForestViewEnabled
		{
			get
			{
				return (bool)(this[ScopeSettingsSchema.ForestViewEnabled] ?? false);
			}
			set
			{
				this[ScopeSettingsSchema.ForestViewEnabled] = value;
			}
		}

		public bool DomainViewEnabled
		{
			get
			{
				return !this.ForestViewEnabled;
			}
			set
			{
				this.ForestViewEnabled = !value;
			}
		}

		public virtual string OrganizationalUnit
		{
			get
			{
				return (string)this[ScopeSettingsSchema.OrganizationalUnit];
			}
			set
			{
				this[ScopeSettingsSchema.OrganizationalUnit] = value;
			}
		}

		public string ScopingDescription
		{
			get
			{
				if (!this.ForestViewEnabled)
				{
					return this.OrganizationalUnit;
				}
				return Strings.EntireForest;
			}
		}

		public override ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (this.DomainViewEnabled && string.IsNullOrEmpty(this.OrganizationalUnit))
			{
				list.Add(new PropertyValidationError(Strings.ErrorOrganizationalUnitEmpty, new ADPropertyDefinition("OrganizationalUnit", ExchangeObjectVersion.Exchange2003, typeof(string), "Dummy Property", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null), this));
			}
			return list.ToArray();
		}

		private static ScopeSettingsSchema schema = ObjectSchema.GetInstance<ScopeSettingsSchema>();
	}
}
