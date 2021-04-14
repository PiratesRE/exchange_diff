using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(SecurityPrincipalPickerObject))]
	public class SecurityPrincipalPickerObject : BaseRow
	{
		public SecurityPrincipalPickerObject(ExtendedSecurityPrincipal securityPrincipal) : base(securityPrincipal)
		{
			this.ExtendedSecurityPrincipal = securityPrincipal;
		}

		private ExtendedSecurityPrincipal ExtendedSecurityPrincipal { get; set; }

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.ExtendedSecurityPrincipal.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string InFolder
		{
			get
			{
				return this.ExtendedSecurityPrincipal.InFolder;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.ExtendedSecurityPrincipal.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SpriteId
		{
			get
			{
				return Icons.FromEnum(this.ExtendedSecurityPrincipal.Type);
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
				return Icons.GenerateIconAltText(this.ExtendedSecurityPrincipal.Type);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
