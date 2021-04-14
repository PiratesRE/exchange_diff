using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("objectId")]
	public class ExtensionProperty : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ExtensionProperty CreateExtensionProperty(string objectId, Collection<string> targetObjects)
		{
			ExtensionProperty extensionProperty = new ExtensionProperty();
			extensionProperty.objectId = objectId;
			if (targetObjects == null)
			{
				throw new ArgumentNullException("targetObjects");
			}
			extensionProperty.targetObjects = targetObjects;
			return extensionProperty;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string dataType
		{
			get
			{
				return this._dataType;
			}
			set
			{
				this._dataType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> targetObjects
		{
			get
			{
				return this._targetObjects;
			}
			set
			{
				this._targetObjects = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _name;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _dataType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _targetObjects = new Collection<string>();
	}
}
