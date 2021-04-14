using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Serializable]
	public class PageConfigurableProfile : ITableCentricConfigurable, IHasPermission
	{
		[DDIMandatoryValue]
		public string Name { get; set; }

		public Type[] LambdaExpressionHelper
		{
			get
			{
				return this.lambdaExpressionHelper;
			}
			set
			{
				if (value != this.lambdaExpressionHelper)
				{
					this.lambdaExpressionHelper = value;
					if (this.LambdaExpressionHelper != null)
					{
						foreach (Type type in this.LambdaExpressionHelper)
						{
							ExpressionParser.EnrolPredefinedTypes(type);
						}
					}
				}
			}
		}

		public DataObjectProfileList DataObjectProfiles
		{
			get
			{
				return this.dataObjectProfiles;
			}
			set
			{
				this.dataObjectProfiles = value;
			}
		}

		public ReaderTaskProfileList ReaderTaskProfiles
		{
			get
			{
				return this.readerTaskProfileList;
			}
			set
			{
				this.readerTaskProfileList = value;
			}
		}

		public SaverTaskProfileList SaverTaskProfiles
		{
			get
			{
				return this.saverTaskProfiles;
			}
			set
			{
				this.saverTaskProfiles = value;
			}
		}

		[DDIMandatoryValue]
		public ColumnProfileList ColumnProfiles
		{
			get
			{
				return this.columnProfiles;
			}
			set
			{
				this.columnProfiles = value;
			}
		}

		public PageToDataObjectsList PageToDataObjectsMapping
		{
			get
			{
				return this.pageToDataObjectsMapping;
			}
			set
			{
				this.pageToDataObjectsMapping = value;
			}
		}

		public List<ReaderTaskProfile> BuildReaderTaskProfile()
		{
			return this.ReaderTaskProfiles;
		}

		public List<SaverTaskProfile> BuildSaverTaskProfile()
		{
			return this.SaverTaskProfiles;
		}

		public List<DataObjectProfile> BuildDataObjectProfile()
		{
			return this.DataObjectProfiles;
		}

		public List<ColumnProfile> BuildColumnProfile()
		{
			return this.ColumnProfiles;
		}

		public Dictionary<string, List<string>> BuildPageToDataObjectsMapping()
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (string key in this.PageToDataObjectsMapping.Keys)
			{
				dictionary[key] = this.PageToDataObjectsMapping[key].ToList<string>();
			}
			return dictionary;
		}

		[DefaultValue(true)]
		public bool EnableUICustomization
		{
			get
			{
				return this.enableUICustomization;
			}
			set
			{
				this.enableUICustomization = value;
			}
		}

		public bool CanEnableUICustomization()
		{
			return this.EnableUICustomization;
		}

		public bool HasPermission()
		{
			bool result = false;
			foreach (ReaderTaskProfile readerTaskProfile in this.readerTaskProfileList)
			{
				if (readerTaskProfile.HasPermission())
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private Type[] lambdaExpressionHelper;

		private DataObjectProfileList dataObjectProfiles = new DataObjectProfileList();

		private ReaderTaskProfileList readerTaskProfileList = new ReaderTaskProfileList();

		private SaverTaskProfileList saverTaskProfiles = new SaverTaskProfileList();

		private ColumnProfileList columnProfiles = new ColumnProfileList();

		private PageToDataObjectsList pageToDataObjectsMapping = new PageToDataObjectsList();

		private bool enableUICustomization = true;
	}
}
