using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SecurityPrincipalRow : AdObjectResolverRow
	{
		public SecurityPrincipalRow(ADRawEntry aDRawEntry) : base(aDRawEntry)
		{
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base.ADRawEntry[ADObjectSchema.Name];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public override string DisplayName
		{
			get
			{
				string text = (string)base.ADRawEntry[ExtendedSecurityPrincipalSchema.DisplayName];
				if (string.IsNullOrEmpty(text))
				{
					text = this.Name;
				}
				return text;
			}
		}

		[DataMember]
		public string SpriteId
		{
			get
			{
				return Icons.FromEnum((SecurityPrincipalType)base.ADRawEntry[ExtendedSecurityPrincipalSchema.SecurityPrincipalTypes]);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string IconAltText
		{
			get
			{
				return Icons.GenerateIconAltText((SecurityPrincipalType)base.ADRawEntry[ExtendedSecurityPrincipalSchema.SecurityPrincipalTypes]);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ExtendedSecurityPrincipalSchema.DisplayName,
			ADObjectSchema.ObjectClass,
			ExtendedSecurityPrincipalSchema.SecurityPrincipalTypes
		}.ToArray();
	}
}
