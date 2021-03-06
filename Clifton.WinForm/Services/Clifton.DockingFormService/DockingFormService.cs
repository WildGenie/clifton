﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Clifton.Core.ExtensionMethods;
using Clifton.Core.ModuleManagement;
using Clifton.Core.ServiceManagement;
using Clifton.WinForm.ServiceInterfaces;

namespace Clifton.DockingFormService
{
    public class DockingFormModule : IModule
    {
        public void InitializeServices(IServiceManager serviceManager)
        {
            serviceManager.RegisterSingleton<IDockingFormService, DockingFormService>();
        }
    }

    public class DockingFormService : ServiceBase, IDockingFormService
    {
        public event EventHandler<ContentLoadedEventArgs> ContentLoaded;
        public event EventHandler<EventArgs> ActiveDocumentChanged;
        public event EventHandler<EventArgs> DocumentClosing;

        public Panel DockPanel { get { return dockPanel; } }
        public List<IDockDocument> Documents { get { return dockPanel.DocumentsToArray().Cast<IDockDocument>().ToList(); } }

        protected DockPanel dockPanel;
        protected VS2015LightTheme theme = new VS2015LightTheme();

        public Form CreateMainForm()
        {
            Form form = new BaseForm();
            dockPanel = new DockPanel();
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.Theme = theme;
            form.Controls.Add(dockPanel);

            dockPanel.ActiveDocumentChanged += (sndr, args) => ActiveDocumentChanged.Fire(dockPanel.ActiveDocument);

            return form;
        }

        public Control CreateDocument(WinForm.ServiceInterfaces.DockState dockState, string tabText, string metadata = "")
        {
            DockContent dockContent = new GenericDockContent(metadata);
            dockContent.DockAreas = DockAreas.Float | DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Document;
            dockContent.TabText = tabText;
            dockContent.Show(dockPanel, (WeifenLuo.WinFormsUI.Docking.DockState)dockState);
            dockContent.FormClosing += (sndr, args) => DocumentClosing.Fire(dockContent, EventArgs.Empty);

            return dockContent;
        }

        public Control CreateDocument(Control pane, WinForm.ServiceInterfaces.DockAlignment dockAlignment, string tabText, string metadata = "", double portion = 0.25)
        {
            DockContent dockContent = new GenericDockContent(metadata);
            dockContent.DockAreas = DockAreas.Float | DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Document;
            dockContent.TabText = tabText;
            dockContent.Show(((DockContent)pane).Pane, (WeifenLuo.WinFormsUI.Docking.DockAlignment)dockAlignment, portion);

            return dockContent;
        }

        public void SetActiveDocument(IDockDocument document)
        {
            ((DockContent)document).Activate();
        }

        public void SaveLayout(string filename)
        {
            dockPanel.SaveAsXml(filename);      // layout.xml
        }

        public void LoadLayout(string filename)
        {
            CloseAllDocuments();
            dockPanel.LoadFromXml(filename, new DeserializeDockContent(GetContentFromPersistString));
            LoadApplicationContent();
        }

        protected IDockContent GetContentFromPersistString(string persistString)
        {
            string metadata = persistString.RightOf(',').Trim();
            GenericDockContent content = new GenericDockContent();
            content.Metadata = metadata;

            return content;
        }

        protected void LoadApplicationContent()
        {
            foreach (DockContent document in dockPanel.Contents)
            {
                ContentLoaded.Fire(this, new ContentLoadedEventArgs() { DockContent = document, Metadata = ((GenericDockContent)document).Metadata });
            }

            //foreach (var window in dockPanel.FloatWindows.ToList())
            //{
            //    window.Dispose();
            //}

            //foreach (DockContent doc in dockPanel.Contents.ToList())
            //{
            //    doc.DockHandler.DockPanel = null;
            //    doc.Close();
            //}
        }

        protected void CloseAllDocuments()
        {
            // Close all documents
            foreach (IDockContent document in dockPanel.DocumentsToArray())
            {
                // IMPORANT: dispose all panes.
                document.DockHandler.DockPanel = null;
                document.DockHandler.Close();
            }

            // IMPORTANT: dispose all float windows.
            foreach (var window in dockPanel.FloatWindows.ToList())
            {
                window.Dispose();
            }

            foreach (DockContent doc in dockPanel.Contents.ToList())
            {
                doc.DockHandler.DockPanel = null;
                doc.Close();
            }
        }
    }
}
