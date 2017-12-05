using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace ncBehaviouralTargeting.Library.BackofficeSections
{
    [PluginController("ncFootprint")]
    [Tree("ncbt", "ncbt", "Footprint", iconClosed: "icon-library")]
    public class FootprintTreeController : TreeController
    {
        public const string MainRoute = "/ncbt/ncbt";

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            // Check if we're rendering the root node's children
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                var treeNodes = new List<SectionTreeNode>
                {
                    new SectionTreeNode
                    {
                        Id = "segmentoverview",
                        Title = "Segments",
                        Icon = "icon-legal",
                        Route = string.Format("{0}/base/{1}", MainRoute, "segmentoverview"),
                        HasChildren = false
                    },
                    new SectionTreeNode
                    {
                        Id = "actionoverview",
                        Title = "Actions",
                        Icon = "icon-flash",
                        Route = string.Format("{0}/base/{1}", MainRoute, "actionoverview"),
                        HasChildren = false
                    }
                };

                if (umbraco.helper.GetCurrentUmbracoUser().UserType.Alias.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
                {
                    treeNodes.Add(new SectionTreeNode
                    {
                        Id = "propertyoverview",
                        Title = "Properties",
                        Icon = "icon-tag",
                        Route = string.Format("{0}/base/{1}", MainRoute, "propertyoverview"),
                        HasChildren = false
                    });

                    treeNodes.Add(new SectionTreeNode
                    {
                        Id = "settingsoverview",
                        Title = "Settings",
                        Icon = "icon-wrench",
                        Route = string.Format("{0}/base/{1}", MainRoute, "settingsoverview"),
                        HasChildren = false
                    });
                }

                treeNodes.Add(new SectionTreeNode
                {
                    Id = "queryoverview",
                    Title = "Visitors",
                    Icon = "icon-search",
                    Route = $"{MainRoute}/base/{"queryoverview"}",
                    HasChildren = false
                });

                return ReturnTreeNodes(treeNodes, queryStrings);
            }

            return new TreeNodeCollection();
        }

        private TreeNodeCollection ReturnTreeNodes(IEnumerable<SectionTreeNode> treeNodes, FormDataCollection queryStrings)
        {
            var tree = new TreeNodeCollection();
            foreach (var item in treeNodes)
            {
                var nodeToAdd = CreateTreeNode(item.Id, null, queryStrings, item.Title, item.Icon, item.HasChildren, item.Route);
                tree.Add(nodeToAdd);
            }
            return tree;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == "segmentoverview")
            {
                menu.Items.Add<ActionRefresh>("Reload");
                return menu;
            }

            return menu;
        }
    }
}
