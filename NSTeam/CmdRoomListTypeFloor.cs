/*
 * Created by SharpDevelop.
 * User: selez
 * Date: 03.04.2022
 * Time: 16:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
    class CmdRoomListTypeFloor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            FormFloorSettings formFloor = new FormFloorSettings();
            formFloor.ShowDialog();

            if (formFloor.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            string inputParam = formFloor.floorInputParam;
            string outputParam = "NS_Номера помещений_по типу пола";

            //Получаем список помещений
            List<Room> roomsList = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .Where(i => i.LookupParameter("Стадия").AsValueString() != "Существующие")
            .Cast<Room>()
            .ToList();

            //вводим словарь, позднее (не тут) определим, что Ключ - тип пола, Значение - номера помещений
            Dictionary<string, string> roomFloor = new Dictionary<string, string>();


            List<string> floor = new List<string>();

            foreach (Room r in roomsList)
            {
                //Вводим переменную для номера текущего обрабатываемого помещения в виде текста
                string curRoomNumber = r.Number.ToString();

                //для каждого помещения из списка помещений ищем параметр Отделки пола
                Parameter floorParam = r.LookupParameter(inputParam);
                if (floorParam == null) throw new Exception("В проекте отсутствует параметр: " + inputParam);


                //читаем параметр Отделки пола
                string floorAtRoom = GetParameterValue(floorParam);  // floorParam.AsValueString();
                //if (floorAtRoom == null)
                //{
                //    floorAtRoom = floorParam.AsString();//в некоторых параметрах в наших проектах значение парметра может быть просто в string, вводим для надежности
                //}

                if (floorAtRoom == null)
                {
                    throw new Exception("Не задан параметр отделки: " + inputParam + " В помещении: " + curRoomNumber);
                }



                //floor.Add(floorParam.AsValueString()); - так бы мы добавляли в список полов значение параметра Отделки пола
                //List<string> fd = floor.Distinct().ToList(); - так бы мы оставляли в этом списке только уникальные значения



                //Если наш словарь содержит в Ключах считанную для текущего пом-ия Отделку пола, то
                if (roomFloor.ContainsKey(floorAtRoom))
                {
                    //string value = roomFloor[floorAtRoom]; - так бы мы получали значение типа пола по ключу из словаря
                    roomFloor[floorAtRoom] += ", " + curRoomNumber;//то берем то, что уже есть в ячейке значений и дописываем ткущий номер пом-ия
                }
                else
                {
                    roomFloor.Add(floorAtRoom, curRoomNumber);//если такого типа пола в словаре еще нет - то создаем в словаре новую пару ключ-значение (тип пола, номер помещения)
                }

                //ключ для помещения равен типу отделки floorAtRoom
            }
            foreach (Room r in roomsList)
            {

                //для каждого помещения из списка помещений ищем параметр Отделки пола
                Parameter floorParam = r.LookupParameter(inputParam);
                Parameter roomsFloorParam = r.LookupParameter(outputParam);
                if (roomsFloorParam == null) throw new Exception("В проекте отсутствует параметр: " + outputParam);

                //читаем параметр Отделки пола
                string floorAtRoom = GetParameterValue(floorParam);

                if (roomFloor.ContainsKey(floorAtRoom))
                {
                    using (Transaction t = new Transaction(doc))
                    {
                        t.Start("Проставить типы полов");
                        r.LookupParameter(outputParam).Set(roomFloor[floorAtRoom]);
                        t.Commit();
                    }
                }
                else
                {
                    continue;

                }
            }
            TaskDialog.Show("Готово!", "Обработано: " + roomsList.Count + " помещений ");
            return Result.Succeeded;//для реализации Result Execute
        }

        public string GetParameterValue(Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return parameter.AsString();
                case StorageType.Double:
                    return parameter.AsValueString();
                case StorageType.ElementId:
                    return parameter.AsElementId().IntegerValue.ToString();
                case StorageType.Integer:
                    return parameter.AsValueString();
                //case StorageType.None:
                // return parameter.AsValueString();
                default:
                    return "-";
            }
        }


    }
}