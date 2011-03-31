using System;
using System.Web.UI;
using N2.Web.UI.WebControls;
using N2.Definitions;

namespace N2.Details
{
	/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.UrlSelector"/> web control as editor/url selector.</summary>
	/// <example>
	/// [N2.Details.EditableUrl("Url to page or document", 50)]
	/// public virtual string PageOrDocumentUrl
	/// {
	///     get { return (string)GetDetail("PageOrDocumentUrl"); }
	///		set { SetDetail("PageOrDocumentUrl", value); }
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableUrlAttribute : AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
	{
		private UrlSelectorMode openingMode = UrlSelectorMode.Items;
		private UrlSelectorMode availableModes = UrlSelectorMode.All;

		public EditableUrlAttribute()
			: this(null, 30)
		{
		}

		/// <summary>Initializes a new instance of the EditableUrlAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUrlAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		/// <summary>Defines whether files or content items are available to be picked</summary>
		public UrlSelectorMode AvailableModes
		{
			get { return availableModes; }
			set { availableModes = value; }
		}

		/// <summary>Defines whether files or content items are first shown when picking an url.</summary>
		public UrlSelectorMode OpeningMode
		{
			get { return openingMode; }
			set { openingMode = value; }
		}

		/// <summary>Defines whether the managementUrls should be stored as app- or server relative.</summary>
		public UrlRelativityMode RelativeTo { get; set; }

		public override void UpdateItem(ContainableContext context)
		{
			UrlSelector selector = (UrlSelector)context.Control;
			context.SetValue(RelativeTo == UrlRelativityMode.Absolute ? selector.Url : N2.Web.Url.ToRelative(selector.Url));
		}

		public override void UpdateEditor(ContainableContext context)
		{
			UrlSelector selector = (UrlSelector)context.Control;
			selector.Url = context.GetValue<string>();
		}

		protected override Control AddEditor(Control container)
		{
			UrlSelector selector = new UrlSelector();
			selector.ID = this.Name;
			selector.AvailableModes = AvailableModes;
			selector.DefaultMode = OpeningMode;

			container.Controls.Add(selector);

			return selector;
		}

		#region IRelativityTransformer Members

		public RelativityMode RelativeWhen { get; set; }

		string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
		{
			return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
		}

		#endregion

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			writer.Write(item[propertyName]);
		}

		#endregion
	}
}
