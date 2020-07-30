using dev_web_api.BusinessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class hardware : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        protected void Page_Init(object sender, EventArgs e)
        {
            ddlAgents.DataSource = monitorDb.GetEnabledAgents();
            ddlAgents.DataValueField = "AgentId";
            ddlAgents.DataTextField = "ScreenName";
            ddlAgents.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var agentResource = monitorDb.GetAgentResource(
                                                Convert.ToInt32(ddlAgents.SelectedValue));
            if (agentResource != null)
            {
                treeView1.Visible = (agentResource != null);
                lblEmptyData.Visible = (agentResource == null);
                if (agentResource != null)
                {
                    LoadJsonToTreeView(treeView1, agentResource.StableDeviceJson);
                }
                treeView1.ExpandAll();
                litDate.Text = $"Last Updated: {agentResource.LastUpdatedDate} UTC";
            }
            else
            {
                treeView1.Visible = false;
                lblError.Visible = true;
                lblError.Text = "Unable to get the server Data";
            }

        }

        /// <summary>
        /// From: http://huseyint.com/2013/12/Have-your-JSON-TreeView-and-unit-test-it-too/
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="json"></param>
        private void LoadJsonToTreeView(TreeView treeView, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }
            treeView.Nodes.Clear();
            var @object = JObject.Parse(json);
            @object.Remove("softwares");
            AddObjectNodes(@object, "Hardware", treeView.Nodes);
        }

        void AddObjectNodes(JObject @object, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            foreach (var property in @object.Properties())
            {
                AddTokenNodes(property.Value, property.Name, node.ChildNodes);
            }
        }

        void AddArrayNodes(JArray array, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            for (var i = 0; i < array.Count; i++)
            {
                AddTokenNodes(array[i], string.Format("[{0}]", i), node.ChildNodes);
            }
        }

        void AddTokenNodes(JToken token, string name, TreeNodeCollection parent)
        {
            if (token is JValue)
            {
                parent.Add(new TreeNode(string.Format("{0}: {1}", name, ((JValue)token).Value)));
            }
            else if (token is JArray)
            {
                AddArrayNodes((JArray)token, name, parent);
            }
            else if (token is JObject)
            {
                AddObjectNodes((JObject)token, name, parent);
            }
        }

        protected void ddlAgents_SelectedIndexChanged(object sender, EventArgs e)
        {

            LoadData();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }
    }
}