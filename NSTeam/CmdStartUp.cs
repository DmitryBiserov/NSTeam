using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using static NSTeam.CmdElementCreator;

namespace NSTeam
{
    internal class CmdStartUp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentSynchronizingWithCentral += new EventHandler<DocumentSynchronizingWithCentralEventArgs>(OnSyncCentralStart);
            return Result.Succeeded;
        }

        public void OnSyncCentralStart(object sender, DocumentSynchronizingWithCentralEventArgs e)
        {
            TaskDialog.Show("info", "1");
            CmdElementCreator obj = new CmdElementCreator();
        }
    }
}
