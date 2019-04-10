using HtmlAgilityPack;
using shortid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.Shortcodes;

namespace JupiterNebula.Website.Shortcodes.TabPanel
{
    public class TabPanel : IShortcode
    {
        private string CreateTabContentIdPrefix() => $"TabPanel__{ShortId.Generate()}";

        public IShortcodeResult Execute(KeyValuePair<string, string>[] args, string content, IDocument document, IExecutionContext context)
        {
            var contentHtml = new HtmlDocument();
            contentHtml.LoadHtml(content);

            var tabListElement = contentHtml.DocumentNode?.SelectSingleNode("/ul");
            var tabElements = tabListElement?.SelectNodes("./li");

            if (tabElements is null)
            {
                return context.GetShortcodeResult(content);
            }

            string idPrefix = CreateTabContentIdPrefix();
            int idSuffix = 0;

            tabListElement.AddClass("nav nav-tabs");
            tabListElement.SetAttributeValue("role", "tablist");
            tabListElement.Id = idPrefix;

            var tabContentWrapElement = contentHtml.CreateElement("div");
            tabContentWrapElement.AddClass("tab-content");

            foreach (var tabElement in tabElements)
            {
                var tabContentElement = tabElement.SelectSingleNode("./div");

                tabContentElement.Id = $"{idPrefix}-{idSuffix++}";
                tabContentElement.AddClass("tab-pane fade");
                tabContentElement.SetAttributeValue("role", "tabpanel");
                tabContentElement.SetAttributeValue("aria-labelledby", tabContentElement.Id + "-tab");

                tabElement.RemoveChild(tabContentElement);
                tabContentWrapElement.AppendChild(tabContentElement);

                var anchorElementInnerHtml = tabElement.InnerHtml;
                tabElement.InnerHtml = string.Empty;
                tabElement.AddClass("nav-item");

                var anchorElement = contentHtml.CreateElement("a");
                anchorElement.AddClass("nav-link");
                anchorElement.SetAttributeValue("data-toggle", "tab");
                anchorElement.SetAttributeValue("href", "#" + tabContentElement.Id);
                anchorElement.SetAttributeValue("role", "tab");
                anchorElement.SetAttributeValue("aria-controls", tabContentElement.Id);
                anchorElement.SetAttributeValue("aria-selected", "false");
                anchorElement.InnerHtml = anchorElementInnerHtml;
                anchorElement.Id = tabContentElement.Id + "-tab";

                tabElement.AppendChild(anchorElement);
            }

            var firstTabElement = tabElements.FirstOrDefault()?.FirstChild;
            firstTabElement?.AddClass("active");
            firstTabElement?.SetAttributeValue("aria-selected", "true");
            tabContentWrapElement.FirstChild?.AddClass("show active");

            if (tabContentWrapElement.HasChildNodes)
            {
                contentHtml.DocumentNode.AppendChild(tabContentWrapElement);
            }

            return context.GetShortcodeResult(contentHtml.DocumentNode.OuterHtml);
        }
    }
}