using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;

namespace NSTeam
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class CmdToWorksets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<RevitLinkInstance> linksList = new FilteredElementCollector(doc).WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_RvtLinks)
            .Cast<RevitLinkInstance>()
            .ToList();

            List<Workset> worksets
            = new FilteredWorksetCollector(doc)
            .Where(i => i.Name.Contains("01_Связанные RVT-файлы"))
            .ToList();

            string workset = worksets.First().Name.ToString();
            Workset worksetp = worksets.First();

            foreach (RevitLinkInstance e in linksList)
            {

                Parameter wsparam = e.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                if (wsparam == null) return;

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Распределить по рабочим наборам");
                    wsparam.Set(worksetp.Id.IntegerValue);
                    tx.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}
