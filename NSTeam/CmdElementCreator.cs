/*
    Данный блок автоматически прописывает в элементы модели их автора и дату/время создания

    Срабатывает при начале синхронизации.
    Проставляет штамп в виде "Автор | Дата-Время" в текстовое поле, имя которого задано в параметре nameParam 
    
    Модель должна иметь этот параметр во всех своих элементах.
    Имя для параметра по умолчанию: "Создано"

    
    Доделать:
    - запуск при синхронизации

    - сделать обработку имени пользователя для НЕ файла-хранилища

    - данные прописываются только для тех элементов, для которых можно получить спецификацию материалов - надо для всех. Переделать GetAllModelElements
  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace NSTeam
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class CmdElementCreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //Получение документа
            Document doc =  commandData.Application.ActiveUIDocument.Document;

            //Получение имени пользователя Windows, объявление переменных
            string username = Environment.UserName; //переменная для имени пользователя + получение имени пользователя Windows

            string nameParam = "Создано"; //имя параметра для штампа Пользователь | Дата-Время
            string createStamp = ""; //штамп для записи в поле
            string eIdString = ""; //параметр для id элемента в string (для поиска, принадлежит ли элемент занятому пользователем рабочему набору) 
            bool ownerItsMe = false; //параметр для флага, что данный элемент находится в занятом пользователем рабочем наборе
            

            //Получение всех "геометрических" элементов модели
            var elementsList = GetAllModelElements(doc);
            
            //Получение всех элементов, которые принадлежат к занятым пользователем рабочим наборам в данной сессии
            List<ElementId> checkoutelementsList = CheckoutAll(doc).ToList();
                        
            //Получение текущего штампа даты-времени, с учетом культуры ru
            DateTime localDate = DateTime.Now;
            CultureInfo culture = new CultureInfo("ru-RU");

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Запись создателей элементов");

                foreach (Element e in elementsList)
                {
                    Parameter createParam = e.LookupParameter(nameParam);
                    if (createParam == null) new Exception("Нет параметра " + nameParam + " в проекте");
                    createStamp = createParam.AsString();

                    if (createStamp == null || createStamp == "")//Если значение поля в элементе пустое...
                    {
                        eIdString = e.Id.ToString();

                        foreach (var l in checkoutelementsList)
                        {
                            ownerItsMe = l.ToString().Contains(eIdString);
                            if (ownerItsMe)
                            {
                                createStamp = username + " | ";
                                break; //вываливаемся из перебора вариантов, как только находим совпадение с текущим
                            }
                        }

                        createParam.Set(createStamp + localDate.ToString(culture));//...то записываем в него имя пользователя Windows и дату в формате культуры ru

                    }

                }
                t.Commit();
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Метод, собирающий id всех элементов, которые принадлежат текущему пользователю (через рабочие наборы и WorksharingUtils)
        /// </summary>
        /// <param name="doc">Документ</param>
        /// <returns></returns>
        public ICollection<ElementId> CheckoutAll(Document doc)
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
        IList<Element> GetAllModelElements(Document doc)
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
  
    }
}
