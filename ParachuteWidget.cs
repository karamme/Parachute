using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Widgets;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Parachute
{
    public class AssemblyPriority : GH_AssemblyPriority
    {
        public AssemblyPriority() : base() { }

        public override GH_LoadingInstruction PriorityLoad()
        {
            GH_Canvas.WidgetListCreated += new GH_Canvas.WidgetListCreatedEventHandler(AddParachuteWidget);

            return GH_LoadingInstruction.Proceed;
        }

        private void AddParachuteWidget(object sender, GH_CanvasWidgetListEventArgs e)
        {
            ParachuteWidget parachuteWidget = new ParachuteWidget();
            e.AddWidget(parachuteWidget);
        }
    }

    public class ParachuteWidget : GH_Widget
    {
        public ParachuteWidget() : base()
        {
            GH_SettingsServer settings = new GH_SettingsServer("parachute", true);

            if (settings.ConstainsEntry("Ignored"))
            {
                Ignored = new List<string>(settings.GetValue("Ignored", "").Split(','));
            }
            else
            {
                string ignored_defaults = "Panel,Scribble,Sketch,Number Slider,Button,Boolean Toggle";
                settings.SetValue("IsActive", m_visible);
                settings.SetValue("Ignored", ignored_defaults);
                settings.WritePersistentSettings();
                Ignored = new List<string>(ignored_defaults.Split(','));
            }

            Grasshopper.Instances.CanvasCreated += CanvasCreated;
        }

        private bool m_visible = true;

        public override bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                GH_SettingsServer settings = new GH_SettingsServer("parachute", true);
                if (value == true)
                {
                    m_visible = true;
                    Ignored = new List<string>(settings.GetValue("Ignored", "").Split(','));
                    RefreshParachute();
                }
                else
                {
                    m_visible = false;
                    ClearParachute();
                }
                settings.SetValue("IsActive", m_visible);
                settings.WritePersistentSettings();
            }
        }

        public static List<string> Ignored = new List<string>();

        public override string Name
        {
            get { return "Parachute"; }
        }

        public override string Description
        {
            get { return "Display names above components"; }
        }

        public override Bitmap Icon_24x24
        {
            get
            {
                return Properties.Resources.parachute_24;
            }
        }

        public override bool Contains(Point pt_control, PointF pt_canvas)
        {
            return false;
        }

        public override void Render(GH_Canvas Canvas)
        {
            GH_SettingsServer settings = new GH_SettingsServer("parachute", true);
            m_visible = settings.GetValue("IsActive", false);
        }

        private void CanvasCreated(GH_Canvas canvas)
        {
            Grasshopper.Instances.ActiveCanvas.DocumentChanged += DocumentChanged;
        }

        private void DocumentChanged(GH_Canvas canvas, GH_CanvasDocumentChangedEventArgs e)
        {
            if (Visible)
            {
                CheckParachute();
            }
            else
            {
                ClearParachute();
            }
        }

        private void ObjectsAdded(Object sender, GH_DocObjectEventArgs e)
        {
            RefreshParachute();
        }

        private void CheckParachute()
        {
            GH_Document document = Grasshopper.Instances.ActiveCanvas.Document;
            if (document == null) return;
            List<IGH_DocumentObject> documentObjects = new List<IGH_DocumentObject>(document.Objects);
            if (documentObjects.Any(o => o.Description == "ParachuteGroup"))
            {
                document.ObjectsAdded -= new GH_Document.ObjectsAddedEventHandler(ObjectsAdded);
                document.ObjectsAdded += new GH_Document.ObjectsAddedEventHandler(ObjectsAdded);
            }
            else
            {
                RefreshParachute();
            }   
        }

        private void ClearParachute()
        {
            GH_Document document = Grasshopper.Instances.ActiveCanvas.Document;
            if (document == null) return;
            document.ObjectsAdded -= new GH_Document.ObjectsAddedEventHandler(ObjectsAdded);
            List<IGH_DocumentObject> documentObjects = new List<IGH_DocumentObject>(document.Objects);
            foreach (IGH_DocumentObject obj in documentObjects)
            {
                if (obj.GetType() == typeof(Grasshopper.Kernel.Special.GH_Group))
                {
                    if (obj.Description == "ParachuteGroup")
                    {
                        document.RemoveObject(obj, false);
                    }
                }
            }
        }

        private void RefreshParachute()
        {
            GH_Document document = Grasshopper.Instances.ActiveCanvas.Document;
            if (document == null) return;
            document.ObjectsAdded -= new GH_Document.ObjectsAddedEventHandler(ObjectsAdded);
            List<IGH_DocumentObject> documentObjects = new List<IGH_DocumentObject>(document.Objects);
            foreach (IGH_DocumentObject obj in documentObjects)
            {
                if (obj.GetType() == typeof(Grasshopper.Kernel.Special.GH_Group))
                {
                    if (obj.Description == "ParachuteGroup")
                    {
                        document.RemoveObject(obj, false);
                    }
                }
                else if (!Ignored.Contains(obj.Name))
                {
                    Grasshopper.Kernel.Special.GH_Group paragroup = new Grasshopper.Kernel.Special.GH_Group
                    {
                        Colour = Color.FromArgb(0, 0, 0, 0),
                        Description = "ParachuteGroup",
                        NickName = obj.Name
                    };
                    paragroup.AddObject(obj.InstanceGuid);
                    document.AddObject(paragroup, false);
                }
            }
            document.ObjectsAdded += new GH_Document.ObjectsAddedEventHandler(ObjectsAdded);
        }
    }
}
