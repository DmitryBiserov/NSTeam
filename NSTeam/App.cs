using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NSTeam
{
    //Этот текст тоже из С# Developer Revit-а
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class App : IExternalApplication
    {


        // define a method that will create our tab and button
        static void AddRibbonPanel(UIControlledApplication application)
        {
            // Create a custom ribbon tab
            String tabName = "Луч-АР";
            application.CreateRibbonTab(tabName);

            // Add a new ribbon panel
            RibbonPanel ribbonPanelBIM = application.CreateRibbonPanel(tabName, "BIM");
            RibbonPanel ribbonPanelAR = application.CreateRibbonPanel(tabName, "АР");
            RibbonPanel ribbonPanelInfo = application.CreateRibbonPanel(tabName, "Помощь");

            // Get dll assembly path
            string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // create push button for CurveTotalLength
            PushButtonData b1Data = new PushButtonData(
                "PinAllRVTLinks",
                "Прикрепить" + System.Environment.NewLine + "  связи  ",
                thisAssemblyPath,
                "NSTeam.CmdPinRvtLinks");

            PushButton pb1 = ribbonPanelBIM.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Связи. \nКоманда позволяет прикрепить все RVT-связи, имеющиеся в документе";
            BitmapImage pb1ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32PinUp.png"));
            BitmapImage pb1ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16PinUp.png"));
            pb1.LargeImage = pb1ImageLarge;
            pb1.Image = pb1ImageSmall;
            pb1.ToolTipImage = pb1ImageLarge;

            PushButtonData bToWorkSetData = new PushButtonData(
                "ToWorkset",
                "Распределить" + System.Environment.NewLine + "  по рабочим наборам  ",
                thisAssemblyPath,
                "NSTeam.CmdExcelExport");

            PushButton pbWorkSet = ribbonPanelBIM.AddItem(bToWorkSetData) as PushButton;
            pbWorkSet.ToolTip = "Модель. \nРаспределить элементы модели по рабочим наборам. \nСвязи, Уровни и сетки осей";
            BitmapImage pbWorkSetImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32ToWorkSet.png"));
            BitmapImage pbWorkSetImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16ToWorkSet.png"));
            pbWorkSet.LargeImage = pbWorkSetImageLarge;
            pbWorkSet.Image = pbWorkSetImageSmall;

            PushButtonData b2Data = new PushButtonData(
            "RoomListTypeFloor",
            "Заполнить помещения" + System.Environment.NewLine + "  по типам пола  ",
            thisAssemblyPath,
            "NSTeam.CmdRoomListTypeFloor");

            PushButton pb2 = ribbonPanelAR.AddItem(b2Data) as PushButton;
            pb2.ToolTip = "Полы. \nЗаполнить номера помещений (параметр: NS_Номера помещений_по типу пола) для экспликации полов (О_АР_Экспликация полов). \nРаздел Экспликация полов в стандарте АР";
            BitmapImage pb2ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32FlrTp.png"));
            BitmapImage pb2ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16FlrTp.png"));
            pb2.LargeImage = pb2ImageLarge;
            pb2.Image = pb2ImageSmall;

            PushButtonData b3Data = new PushButtonData(
            "RoomListScreedType",
            "Заполнить помещения" + System.Environment.NewLine + "  по типам стяжки  ",
            thisAssemblyPath,
            "NSTeam.CmdRoomListScreedType");

            PushButton pb3 = ribbonPanelAR.AddItem(b3Data) as PushButton;
            pb3.ToolTip = "Полы. \nЗаполнить номера помещений (параметр: NS_Номера помещений по типу стяжки) для экспликации стяжек (О_АР_Экспликация полов_Стяжка). \nРаздел Экспликация полов в стандарте АР";
            BitmapImage pb3ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32ScrTp.png"));
            BitmapImage pb3ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16ScrTp.png"));
            pb3.LargeImage = pb3ImageLarge;
            pb3.Image = pb3ImageSmall;

            PushButtonData b4Data = new PushButtonData(
            "SetHost",
            "Заполнить" + System.Environment.NewLine + "  тип подосновы  ",
            thisAssemblyPath,
            "NSTeam.CmdSetHost");

            PushButton pb4 = ribbonPanelAR.AddItem(b4Data) as PushButton;
            pb4.ToolTip = "Перемычки. \nЗаполнить тип стены-подосновы у дверей и окон (параметр: NS_Стена-основа_Тип) для последующей обработки перемычек. \nРаздел Перемычки в стандарте АР";
            BitmapImage pb4ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32SetHost.png"));
            BitmapImage pb4ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16SetHost.png"));
            pb4.LargeImage = pb4ImageLarge;
            pb4.Image = pb4ImageSmall;

            PushButtonData b5Data = new PushButtonData(
            "WebInfoPortal",
            "Открыть" + System.Environment.NewLine + "  справочный портал  ",
            thisAssemblyPath,
            "NSTeam.CmdWebInfoPortal");

            PushButton pb5 = ribbonPanelInfo.AddItem(b5Data) as PushButton;
            pb5.ToolTip = "Web. \nОткрыть справочный портал N-Systems/Луч";
            BitmapImage pb5ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32NSPortal.png"));
            BitmapImage pb5ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16NSPortal.png"));
            pb5.LargeImage = pb5ImageLarge;
            pb5.Image = pb5ImageSmall;

            PushButtonData b6Data = new PushButtonData(
            "WebStandartPortal",
            "Открыть" + System.Environment.NewLine + "  BIM-стандарт  ",
            thisAssemblyPath,
            "NSTeam.CmdWebStandartPortal");

            PushButton pb6 = ribbonPanelInfo.AddItem(b6Data) as PushButton;
            pb6.ToolTip = "Web. \nОткрыть BIM-стандарт АР";
            BitmapImage pb6ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32NSStand.png"));
            BitmapImage pb6ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16NSStand.png"));
            BitmapImage Tb6ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ToolTipGifs/TlTpGif120NSStand.gif"));
            pb6.LargeImage = pb6ImageLarge;
            pb6.Image = pb6ImageSmall;
            pb6.ToolTipImage = Tb6ImageLarge;

            PushButtonData b7Data = new PushButtonData(
            "WebClassPortal",
            "Открыть" + System.Environment.NewLine + "  классификаторы  ",
            thisAssemblyPath,
            "NSTeam.CmdWebClassPortal");

            PushButton pb7 = ribbonPanelInfo.AddItem(b7Data) as PushButton;
            pb7.ToolTip = "Web. \nОткрыть страницу с классификаторами";
            BitmapImage pb7ImageLarge = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesLarge/BtnImg32NSClass.png"));
            BitmapImage pb7ImageSmall = new BitmapImage(new Uri("pack://application:,,,/NSTeam;component/Resources/ImagesSmall/BtnImg16NSClass.png"));
            pb7.LargeImage = pb7ImageLarge;
            pb7.Image = pb7ImageSmall;
            
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            AddRibbonPanel(application);
            return Result.Succeeded;
        }
    }
}
