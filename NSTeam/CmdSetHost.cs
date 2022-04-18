/*
 * Created by SharpDevelop.
 * User: Dmitrii Biserov
 * Date: 25.03.2022
 * Time: 21:37
 * 
 * Универсальный шаблон для записи данных из одного параметра в другой параметр одного семейства. 
 * В данной реализации предназначен для записи типа Подосновы (Host) в определенный параметр Дверей (OST_Doors)
 * Имя параметру в который записываем задается в строке 30 (setParamName)
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace NSTeam
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CmdSetHost : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получаем доступ к проекту
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Сюда вписываем, в какой параметр записываем данные. Проверьте в Revit LookUp сам элемент(его категорию), сам параметр (и то, что он не null), и тип данных этого параметра (string, double, int)
            string setParamName = "NS_Стена-основа_Тип";

            //из модели отфильтровываем конкретные эелементы, здесь по категории (категорию элемента смотрим в Revit LookUp)
            List<FamilyInstance> doorTypes = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()//если IsNot - то выбор экземпляров, если просто Is - то выбор типоразмеров
            .OfCategory(BuiltInCategory.OST_Doors)// Get Categories
            .Cast<FamilyInstance>().ToList();//что писать в угловых скобках смотрим для конкретного типа элемента в Revit LookUp

            //Упрощенный вариант записи фильтра, фактически тоже самое, что выше
            //FilteredElementCollector col = new FilteredElementCollector(doc);
            //col.WhereElementIsNotElementType();
            // //col.OfClass(typeof(FamilyInstance));// вариант поиска вместо поиска по категории, тут по идее по имени класса как-то можно найти с помощью этой строки
            //col.OfCategory(BuiltInCategory.OST_Doors);

            //List<FamilyInstance> rebarTypes = col.Cast<FamilyInstance>().ToList();

            string doorTypesNames = "";//заготовка текста для информационного окошка
            int resultCount = 0;//счетчик числа обработанных семейств

            //Для записи нам нужно открыть транзакцию
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Запись типа подосновы в двери");

                //Перебираем двери в нашем списке
                foreach (FamilyInstance curType in doorTypes)
                {
                    //Мы сначала получили дверь как Element (и параметы унаследованные от Элемент), чтобы попасть к параметрам собственно двери, мы с помощью as меняем тип элемента на то, как он обозначен в Revit LookUp
                    //FamilyInstance door = curType as FamilyInstance;//обе эти строки кода мы в итоге заменили методом Cast и поправили foreach (FamilyInstance вместо Element)
                    //if(door == null) continue; после приемов с as всегда нужно опроверять, не затесался ли null в выборку, иначе все вылетит

                    //Поиск параметра и проверка, что параметр не пуст
                    Parameter setParam = curType.LookupParameter(setParamName);

                    //Ниже - вместо return введен throw (Урок 13, 1:06:00), он формирует вываливание из программы не молчаливое,
                    //а с сообщением об ошибке, текст ошибки будет выведен под кнопкой Подробнее. Можно было бы TaskDialog, но его не всегда можно использовать он менее универсален
                    if (setParam == null) throw new Exception("В записываемом элементе (ID: " + curType.Id + ") нет указанного параметра: " + setParamName);


                    string setParamValue = setParam.AsString();//Чтение значения параметра. Тут нужно изменять, если у вас другой тип данных в записываемом параметре, в зависимости типа данных параметра

                    //текст для информационного окошка в конце программы
                    doorTypesNames = doorTypesNames + "\n " + curType.Name + curType.Host.Name;

                    string containFilter = "Wall-Ret_300Con";

                    if (curType.Host.Name != containFilter)
                    {
                        //Запись параметра с помощью функции Set, из двери мы получили и записываем в нее следующие данные curType.Host.Name - ТекущаяДверь.Тип-Подосновы-(стены).Имя-подосновы
                        setParam.Set(curType.Host.Name);
                        //счетчик числа обработанных семейств
                        resultCount++;
                    }
                    else
                    {
                        continue;
                    }
                }
                //закрытие транзакции
                t.Commit();

            }
            //информационное окошко в конце программы
            TaskDialog.Show("Info", "Обработано " + resultCount + " экземпляра(ов) семейств");
            return Result.Succeeded;//для реализации Result Execute
        }

    }
}
