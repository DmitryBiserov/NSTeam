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

            //Получаем путь к центральному файлу
            var modelPath = GetCentralServerPath(doc);
            //конвертируем в строку
            string docName = modelPath.ToString();
            //индекс последней точки в пути файла, чтобы отсечь расширение .rvt
            int lastSign = docName.LastIndexOf('.');

            //индекс последнего входждения разделителя полей "_"
            int startSign = docName.LastIndexOf('_');
            //длина части с именем раздела
            int lengthSign = lastSign - startSign;
            //выделяем подстроку с именем раздела начиная с последнего "_" и длиной до знака точки
            string type = docName.Substring(startSign, lengthSign);

            //Получили имя нужного рабочего набора
            string wsName = GetWorksetName(type);


            List<RevitLinkInstance> linksList = new FilteredElementCollector(doc).WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_RvtLinks)
            .Cast<RevitLinkInstance>()
            .ToList();

            List<Grid> gridsList = new FilteredElementCollector(doc).WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_Grids)
            .Cast<Grid>()
            .ToList();

            List<Level> levelsList = new FilteredElementCollector(doc).WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_Levels)
            .Cast<Level>()
            .ToList();

            List<Workset> gridLevelWorksets
            = new FilteredWorksetCollector(doc)
           .Where(i => i.Name.Contains(wsName)).ToList();

            Workset gridLevWorkset = gridLevelWorksets.First();


            List<Workset> worksets
            = new FilteredWorksetCollector(doc)
            .Where(i => i.Name.Contains("01_Связанные RVT-файлы"))
            .ToList();

            if (worksets.Count == 0) throw new Exception("В проекте отсутствует рабочий набор 01_Связанные RVT-файлы");

            string workset = worksets.First().Name.ToString();

            //Из отфильтрованного по имени списка рабочих наборов берем первый элемент. В этот рабочий набор мы будем помещать обнаруженные в модели связи
            Workset worksetp = worksets.First();
            int resultCountLinks = 0;
            int resultCountGrids = 0;
            int resultCountLevels = 0;

            if (linksList.Count != 0)
            {
                foreach (RevitLinkInstance e in linksList)
                {
                    Parameter wsParam = e.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                    if (wsParam == null) throw new Exception("Не смог определить рабочий набор для связи");

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Распределить связи по рабочим наборам");
                        wsParam.Set(worksetp.Id.IntegerValue);
                        tx.Commit();
                        resultCountLinks++;
                    }
                }
            }

            foreach (Grid g in gridsList)
            {
                Parameter gridParam = g.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                if (gridParam == null) throw new Exception("Не смог определить рабочий набор для оси");

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Распределить оси по рабочим наборам");
                    gridParam.Set(gridLevWorkset.Id.IntegerValue);
                    g.Pinned = true;
                    tx.Commit();
                    resultCountGrids++;
                }

            }

            foreach (Level l in levelsList)
            {
                Parameter levelParam = l.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                if (levelParam == null) throw new Exception("Не смог определить рабочий набор для уровня");

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Распределить уровни по рабочим наборам");
                    levelParam.Set(gridLevWorkset.Id.IntegerValue);
                    l.Pinned = true;
                    tx.Commit();
                    resultCountLevels++;   
                }

            }

            TaskDialog.Show("Info", "Готово! Обработано... "+ "\nRVT-связей: " + resultCountLinks + "\nОcей:" + resultCountGrids + "\nУровней: " + resultCountLevels);
            return Result.Succeeded;
        }

        //получение пути к центральному хранилищу, независимо от того, расположен ли он на Revit-server или нет. 
        //Избавляет от проблем с суффиксом пользователя при получении имени файла
        private string GetCentralServerPath(Document doc)
        {
            var modelPath = doc.GetWorksharingCentralModelPath();
            var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

            return centralServerPath;
        }

        //метод для определения имени рабочего набора для осей/уровней по суффиксу имени файла
        public string GetWorksetName(string docName)
        {
            string wsName = "";
            switch (docName)
            {
                case "_АР":
                    return wsName = "00_Общие уровни и сетки_АР";
                case "_КР":
                    return wsName = "00_Общие уровни и сетки_КР";
                case "_ОВ":
                    return wsName = "00_Общие уровни и сетки_ОВ";
                case "_ВК":
                    return wsName = "00_Общие уровни и сетки_ВК";
                case "_ТХ":
                    return wsName = "00_Общие уровни и сетки_ТХ";
                case "_ПТ":
                    return wsName = "00_Общие уровни и сетки_ПТ";
                case "_ЭОМ":
                    return wsName = "00_Общие уровни и сетки_ЭОМ";
                case "_СБ":
                    return wsName = "00_Общие уровни и сетки_СБ";
                case "_СВ":
                    return wsName = "00_Общие уровни и сетки_СВ";

                default:
                    return wsName = "Общие уровни и сетки";
            }
        }
    }
}
