using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using shortid;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.Shortcodes;

namespace JupiterNebula.Wyam.Shortcodes.TabBlock
{
    public class TabBlock : IShortcode
    {
        private static string CreateBaseId() => $"{nameof(TabBlock)}__{ShortId.Generate(true, false)}";
        private static string CreateTabId(string baseId, int tabNumber) => $"{baseId}-{tabNumber}";
        private static string CreateTabLinkId(string tabId) => tabId + "-link";
        private static string CreateTabPaneId(string tabId) => tabId + "-pane";

        #region Tab List
        private static XElement CreateTabLink(XNode labelNode, bool active, string tabId)
        {
            var linkElement = new XElement("a", labelNode);
            var tabPaneId = CreateTabPaneId(tabId);

            linkElement.SetAttributeValue("class", $"nav-link{(active ? " active" : string.Empty)}");
            linkElement.SetAttributeValue("data-toggle", "tab");
            linkElement.SetAttributeValue("aria-selected", XmlConvert.ToString(active));
            linkElement.SetAttributeValue("aria-controls", tabPaneId);
            linkElement.SetAttributeValue("href", '#' + tabPaneId);
            linkElement.SetAttributeValue("id", CreateTabLinkId(tabId));

            return linkElement;
        }

        private static XElement CreateTab(XNode labelNode, bool active, string tabId)
        {
            var tabElement = new XElement("li", CreateTabLink(labelNode, active, tabId));
            tabElement.SetAttributeValue("class", "nav-item");

            return tabElement;
        }

        private static XElement CreateTabList(IEnumerable<XElement> shortcodeTabs, 
            Func<XElement, XNode> labelSelector, string baseId)
        {
            int tabCount = 0;
            var tabList =  new XElement("ul", 
                shortcodeTabs.Select(n => labelSelector(n))
                             .Select(n => CreateTab(n, tabCount == 0, CreateTabId(baseId, tabCount++))));
            
            tabList.SetAttributeValue("class", "nav nav-tabs");
            tabList.SetAttributeValue("role", "tablist");

            return tabList;
        }
        #endregion

        #region Tab Content
        private static XElement CreateTabPane(IEnumerable<XNode> contentNodes, bool active, string tabId)
        {
            var tabPane = new XElement("div", contentNodes);

            tabPane.SetAttributeValue("class", $"tab-pane{(active ? " show active" : string.Empty)}");
            tabPane.SetAttributeValue("role", "tabpanel");
            tabPane.SetAttributeValue("aria-labelledby", CreateTabLinkId(tabId));
            tabPane.SetAttributeValue("id", CreateTabPaneId(tabId));

            return tabPane;
        }

        private static XElement CreateTabPaneContainer(IEnumerable<XElement> shortcodeTabs, 
            Func<XElement, IEnumerable<XNode>> contentSelector, string baseId)
        {
            int tabCount = 0;

            var tabContent = new XElement("div", 
                shortcodeTabs.Select(contentSelector)
                             .Select(n => CreateTabPane(n, tabCount == 0, CreateTabId(baseId, tabCount++))));

            tabContent.SetAttributeValue("class", "tab-content");

            return tabContent;
        }
        #endregion

        private static XElement CreateTabBlock(IEnumerable<XElement> shortcodeTabs, 
            Func<XElement, XNode> tabLabelSelector, Func<XElement, IEnumerable<XNode>> tabContentSelector)
        {
            var baseId = CreateBaseId();

            var tabBlock = new XElement("div", 
                CreateTabList(shortcodeTabs, tabLabelSelector, baseId),
                CreateTabPaneContainer(shortcodeTabs, tabContentSelector, baseId));

            tabBlock.SetAttributeValue("class", "tab-block");
            tabBlock.SetAttributeValue("id", baseId);

            return tabBlock;
        }

        public IShortcodeResult Execute(KeyValuePair<string, string>[] args, string content,
            IDocument document, IExecutionContext context)
        {
            var contentElement = XElement.Parse(content);
            var tabBlock = CreateTabBlock(contentElement.Elements("li"),
                nameof => nameof.FirstNode, n => n.Nodes().Skip(1));
            
            return context.GetShortcodeResult(tabBlock.ToString(SaveOptions.DisableFormatting));
        }
    }
}