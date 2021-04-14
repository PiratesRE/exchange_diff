using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlInclude(typeof(PublicFolder))]
	[XmlInclude(typeof(Device))]
	[XmlInclude(typeof(KeyGroup))]
	[XmlInclude(typeof(ServicePrincipal))]
	[XmlInclude(typeof(User))]
	[XmlInclude(typeof(Subscription))]
	[XmlInclude(typeof(SubscribedPlan))]
	[XmlInclude(typeof(Group))]
	[XmlInclude(typeof(ForeignPrincipal))]
	[XmlInclude(typeof(Contact))]
	[XmlInclude(typeof(Company))]
	[XmlInclude(typeof(Account))]
	[Serializable]
	public abstract class DirectoryObject
	{
		internal static DirectoryObjectClass GetObjectClass(DirectoryObjectClassAddressList restriction)
		{
			switch (restriction)
			{
			case DirectoryObjectClassAddressList.Contact:
				return DirectoryObjectClass.Contact;
			case DirectoryObjectClassAddressList.Group:
				return DirectoryObjectClass.Group;
			case DirectoryObjectClassAddressList.User:
				return DirectoryObjectClass.User;
			default:
			{
				string message = string.Format(CultureInfo.CurrentCulture, "The value '{0}' is invalid.", new object[]
				{
					restriction.ToString()
				});
				throw new ArgumentException(message, "restriction");
			}
			}
		}

		internal static DirectoryObjectClass GetObjectClass(DirectoryObjectClassPerson restriction)
		{
			switch (restriction)
			{
			case DirectoryObjectClassPerson.Contact:
				return DirectoryObjectClass.Contact;
			case DirectoryObjectClassPerson.User:
				return DirectoryObjectClass.User;
			default:
			{
				string message = string.Format(CultureInfo.CurrentCulture, "The value '{0}' is invalid.", new object[]
				{
					restriction.ToString()
				});
				throw new ArgumentException(message, "restriction");
			}
			}
		}

		internal static DirectoryProperty GetDirectoryProperty(object nonDirectoryProperty)
		{
			if (nonDirectoryProperty == null)
			{
				return null;
			}
			if (nonDirectoryProperty is AttributeSet[])
			{
				DirectoryPropertyAttributeSet directoryPropertyAttributeSet = new DirectoryPropertyAttributeSet();
				directoryPropertyAttributeSet.SetValues((AttributeSet[])nonDirectoryProperty);
				return directoryPropertyAttributeSet;
			}
			string message = string.Format(CultureInfo.CurrentCulture, "The value '{0}' is invalid.", new object[]
			{
				nonDirectoryProperty.GetType()
			});
			throw new ArgumentException(message, "nonDirectoryProperty");
		}

		internal abstract void ForEachProperty(IPropertyProcessor processor);

		public DirectoryObject()
		{
			this.allField = false;
			this.deletedField = false;
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool All
		{
			get
			{
				return this.allField;
			}
			set
			{
				this.allField = value;
			}
		}

		[XmlAttribute]
		public string ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		[XmlAttribute]
		public string ObjectId
		{
			get
			{
				return this.objectIdField;
			}
			set
			{
				this.objectIdField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool Deleted
		{
			get
			{
				return this.deletedField;
			}
			set
			{
				this.deletedField = value;
			}
		}

		private bool allField;

		private string contextIdField;

		private string objectIdField;

		private bool deletedField;
	}
}
