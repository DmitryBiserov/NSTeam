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

            if (linksList.Count == 0) throw new Exception("В проекте отсутствуют подгруженные связи");

            List <Workset> worksets
            = new FilteredWorksetCollector(doc)
            .Where(i => i.Name.Contains("01_Связанные RVT-файлы"))
            .ToList();

            if (worksets.Count == 0) throw new Exception("В проекте отсутствуют рабочие наборы");

            //Из отфильтрованного по имени списка рабочих наборов берем первый элемент. В этот рабочий набор мы будем помещать обнаруженные в модели связи
            Workset worksetp = worksets.First();

            foreach (RevitLinkInstance e in linksList)
            {

                Parameter wsParam = e.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                if (wsParam == null) throw new Exception("Не смог определить рабочий набор для связи");

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Распределить элементы модели по рабочим наборам");
                    wsParam.Set(worksetp.Id.IntegerValue);
                    tx.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}
