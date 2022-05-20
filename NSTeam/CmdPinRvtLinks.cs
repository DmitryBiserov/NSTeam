using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace NSTeam
{
    //тоже вставляем строчку траназакции
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class CmdPinRvtLinks : IExternalCommand
    {
        //добавили интерфейс, реагирующий на нажатие кнопки на Ленте
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Обращаемся к открытой модели
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Получаем содержание проекта в переменную doc. Для макроса, привязанного к приложению, чтобы получить доступ к содержимому проекта таков путь


            //Создаем Коллектор col, собирающий все элементы, что есть в doc.
            FilteredElementCollector col = new FilteredElementCollector(doc);

            //В коллекторе col ищем категорию OST_RvtLinks - RVT-связи (Название категории определяем с помощью плагина Revit LookUp)
            col.OfCategory(BuiltInCategory.OST_RvtLinks);

            //чтобы с этим можно было работать как со списком, преобразуем содержимое коллектора в список
            List<Element> rvtLinkList = col.ToList();

            //rvtLinkList.Cast<RevitLinkInstance>();//Варианты по упрощению от Зуева, пока оставим

            //заготовка строки для информационного окошка
            string rvtLinks = "";

            //открываем транзакцию, чтобы можно было записать в Pinned наше значение (Урок 11 Зуева)
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Прикалывание связей");//это относится к транзакции

                //пробегаемся по списку RVT-связей. У нас тут Elements в списке. У сущностей Elements свои методы и параметры, нам же нужны параметры как для RevitLinkInstance (Урок 10 Зуева)
                foreach (Element curLink in rvtLinkList)
                {

                    RevitLinkInstance rLinks = curLink as RevitLinkInstance;// ВАЖНО. Переопеределение класса с помощью as, чтобы получить доступ к параметрам сущности как к RevitLinkInstance
                    if (rLinks == null) continue;//такую конструкцию всегда нужно проверять на null, на случай если выше что-то пошло не так. 
                                                 //если обрабатываемое значение действительно null, то пропускаем эту итерацию (continue) и переходим к следующему элементу списка

                    rLinks.Pinned = true;//закрепляем связи. Pinned - параметр RVT-связи
                    bool pinStatus = rLinks.Pinned;//параметр для информационного окошка. Состояние связи (приколото - true/не приколото - false)
                    string pinStatusString = "";
                    if (pinStatus)
                    {
                        pinStatusString = "Закреплена";
                    }
                    else
                    {
                        pinStatusString = "Не закреплена";
                    }

                    rvtLinks = rvtLinks + " " + curLink.Name + " - " + pinStatusString;//формируем строку для информационного окошка

                }
                t.Commit();//это относится к закрытию транзакции
            }

            wpfReport reportMsg = new wpfReport(rvtLinks);
            reportMsg.ShowDialog();
            //TaskDialog.Show("Отчет по приколотым связям", rvtLinks);//выводим информационное окошко
            return Result.Succeeded;
        }
    }
}
