﻿using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Http.Utility;
using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNetWebFormsRestConsumer
{
    public partial class DQL : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Global.GetRepository() == null) Response.Redirect("Default.aspx");
        }


        private void UpdateGrid(String qualifier)
        {
            Repository repository = Global.GetRepository();
            Feed<RestDocument> feed = repository.ExecuteDQL<RestDocument>("select * from " + qualifier,
                new FeedGetOptions() { Inline = true, Links = true });
            List<RestDocument> docs = feed == null? null : ObjectUtil.getFeedAsList<RestDocument>(feed);

            List<SimpleDocumentProperties> lst = new List<SimpleDocumentProperties>();
            foreach (RestDocument doc in docs)
            {
                SimpleDocumentProperties sdp = new SimpleDocumentProperties();
                sdp.Id = doc.getAttributeValue("r_object_id").ToString();
                sdp.Name = doc.getAttributeValue("object_name").ToString();
                sdp.Subject = doc.getAttributeValue("subject").ToString();
                sdp.Title = doc.getAttributeValue("title").ToString();
                String folderId = doc.getRepeatingString("i_folder_id", 0);
                Folder folder = repository.getObjectById<Folder>(folderId);
                String folderPath = folder.getRepeatingString("r_folder_path", 0);
                sdp.FolderPath = folderPath;
                lst.Add(sdp);
            }
            gridView.DataSource = lst;
            gridView.DataBind();
        }



        protected void gridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Set the edit index.
            gridView.EditIndex = e.NewEditIndex;
            //Bind data to the GridView control.
            UpdateGrid(null);
        }

        protected void gridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //Reset the edit index.
            gridView.EditIndex = -1;
            //Bind data to the GridView control.
            UpdateGrid(null);
        }

        protected void gridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            //Reset the edit index.
            gridView.EditIndex = -1;
        }

        protected void btnExecuteQuery_Click(object sender, EventArgs e)
        {
            
            String qualifier = txtQuery.Text;
            if (qualifier.StartsWith("select"))
            {
                qualifier = qualifier.Substring(qualifier.ToLower().IndexOf("from ")+5);
            }
            UpdateGrid(qualifier);
        }
    }

    class SimpleDocumentProperties
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Subject { get; set; }
        public String Title { get; set; }
        public String FolderPath { get; set; }

    }
}