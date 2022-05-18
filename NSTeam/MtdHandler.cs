using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace NSTeam
{
    public static class MtdHandler 
    {

        /// <summary>
        /// Метод, собирающий id всех элементов, которые принадлежат текущему пользователю (через рабочие наборы и WorksharingUtils)
        /// </summary>
        /// <param name="doc">Документ</param>
        /// <returns></returns>
        private static ICollection<ElementId> CheckoutAll(Document doc)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc);

            ICollection<ElementId> elements = collector.WhereElementIsNotElementType().ToElementIds(); //WhereElementIsCurveDriven()

            ICollection<ElementId> checkoutelements = WorksharingUtils.CheckoutElements(doc, elements);


            return checkoutelements;
        }

        /// <summary>
        /// Метод для получения всех элементов в модели (в данной редакции только тех, на которые можно получить спецификацию материалов)
        /// </summary>
        /// <param name="doc">Документ</param>
        /// <returns></returns>
        public static IList<Element> GetAllModelElements(Document doc)
        {
            List<Element> elements = new List<Element>();

            FilteredElementCollector collector
            = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType();

            foreach (Element e in collector)
            {
                if (null != e.Category && e.Category.HasMaterialQuantities)
                {
                    elements.Add(e);
                }
            }
            return elements;
        }

       

        public static void Execute(UIApplication uiapp)
        {
            Document doc = uiapp.ActiveUIDocument.Document;
            //Получение имени пользователя Windows, объявление переменных
            string username = Environment.UserName; //переменная для имени пользователя + получение имени пользователя Windows

            string nameParam = "Создано"; //имя параметра для штампа Пользователь | Дата-Время
            string createStamp = ""; //штамп для записи в поле
            string eIdString = ""; //параметр для id элемента в string (для поиска, принадлежит ли элемент занятому пользователем рабочему набору) 
            bool ownerItsMe = false; //параметр для флага, что данный элемент находится в занятом пользователем рабочем наборе

            TaskDialog.Show("info", "5");
            TaskDialog.Show("info", "g");

            string docName = doc.Title;

            TaskDialog.Show("info", "e" + docName);

            //Получение текущего штампа даты-времени, с учетом культуры ru
            DateTime localDate = DateTime.Now;
            CultureInfo culture = new CultureInfo("ru-RU");
            TaskDialog.Show("info", "6 " + localDate.ToString() + username);

            //Получение всех "геометрических" элементов модели
            var elementsList = GetAllModelElements(doc);


            //Получение всех элементов, которые принадлежат к занятым пользователем рабочим наборам в данной сессии
            List<ElementId> checkoutelementsList = CheckoutAll(doc).ToList();



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Запись создателей элементов");
                TaskDialog.Show("info", "7");
                foreach (Element e in elementsList)
                {
                    Parameter createParam = e.LookupParameter(nameParam);
                    if (createParam == null) new Exception("Нет параметра " + nameParam + " в проекте");
                    createStamp = createParam.AsString();
                    TaskDialog.Show("info", "8");
                    if (createStamp == null || createStamp == "")//Если значение поля в элементе пустое...
                    {
                        eIdString = e.Id.ToString();
                        TaskDialog.Show("info", "9");
                        foreach (var l in checkoutelementsList)
                        {
                            ownerItsMe = l.ToString().Contains(eIdString);
                            if (ownerItsMe)
                            {
                                createStamp = username + " | ";
                                break; //вываливаемся из перебора вариантов, как только находим совпадение с текущим
                            }
                        }
                        TaskDialog.Show("info", "10");
                        createParam.Set(createStamp + localDate.ToString(culture));//...то записываем в него имя пользователя Windows и дату в формате культуры ru

                    }

                }
                t.Commit();
            }
        }

        //public static string GetName()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
