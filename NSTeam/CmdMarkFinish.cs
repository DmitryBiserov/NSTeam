/*
    Данный блок копирует содержимое полей отделки полов/потолков/стен из ключевой спецификации 
	в поля помещений, из которых берутся значения для марок отделки полов/потолков
    
	Если что, из ключевой спецификации в марку просто так перенести данные нельзя (по крайней мере для 2019-й версии Revit)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace NSTeam
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	internal class CmdMarkFinish : IExternalCommand
	{		
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			Document doc = commandData.Application.ActiveUIDocument.Document;

			List<Room> roomsList = new FilteredElementCollector(doc)
			.OfCategory(BuiltInCategory.OST_Rooms)
			.Cast<Room>()
			.ToList();

			using (Transaction t = new Transaction(doc))
			{
				t.Start("Заполнение марок полов, стен и потолков");

				foreach (Element e in roomsList)
				{
					Parameter floorParamIn = e.LookupParameter("NS_Отделка тип пола");
					if (floorParamIn == null) new Exception("Нет параметра\"NS_Отделка тип пола\" в проекте");
					Parameter ceilingParamIn = e.LookupParameter("NS_Отделка тип потолка");
					if (ceilingParamIn == null) new Exception("Нет параметра\"NS_Отделка тип потолка\" в проекте");
					Parameter wallParamIn = e.LookupParameter("NS_Отделка тип стен");
					if (wallParamIn == null) new Exception("Нет параметра\"NS_Отделка тип стен\" в проекте");

					Parameter floorParamOut = e.LookupParameter("Отделка пола");
					if (floorParamOut == null) new Exception("Нет параметра\"Отделка пола\" в проекте");
					Parameter ceilingParamOut = e.LookupParameter("Отделка потолка");
					if (ceilingParamOut == null) new Exception("Нет параметра\"Отделка потолка\" в проекте");
					Parameter wallParamOut = e.LookupParameter("Отделка стен");
					if (wallParamOut == null) new Exception("Нет параметра\"Отделка стен\" в проекте");

					if(floorParamIn.AsString() != null)
					{ floorParamOut.Set(floorParamIn.AsString()); }
					if (ceilingParamIn.AsString() != null)
					{ ceilingParamOut.Set(ceilingParamIn.AsString()); }
					if (wallParamIn.AsString() != null)
					{ wallParamOut.Set(wallParamIn.AsString()); }

				}


				t.Commit();
			}
			TaskDialog.Show("Готово!", "Обработано: " + roomsList.Count + " помещений ");
			return Result.Succeeded;
		}
		
	}
    		
}
