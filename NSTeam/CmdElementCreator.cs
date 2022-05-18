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
using Autodesk.Revit.DB.Events;

namespace NSTeam
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class CmdElementCreator : IExternalCommand
    {
       
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
            //Получение документа
           
            TaskDialog.Show("info", "2");
            try
            {
                MtdHandler.Execute(commandData.Application);
                TaskDialog.Show("info", "3");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("info", "4");
                message = ex.Message;
                return Result.Failed;
            }
        }

           
           

    }
}
