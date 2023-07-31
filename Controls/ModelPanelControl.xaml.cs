using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using ModelConsole.Graphics.GLibrary;
using ModelConsole.Graphics.Primitives;
using Model.Test;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModelConsole.Controls
{
   public sealed partial class ModelPanelControl : UserControl
   {
      private GlFrame _frame;
      public ModelPanelControl()
      {
         this.InitializeComponent();
         _frame = new GlFrame(ModelCanvas);

         DrawRectangle();
      }

      public void DrawRectangle()
      {
         //GlRectangle r = GlRectangle.Draw(_frame, 10, 10, 300, 600, 10);
         //GlRectangle.AddBanner(_frame, r, "THIS IS THE TITLE");

         GlModel model = new GlModel();

         var e1 = Data_Table_Entity.GetPersonTable();
         var t1 = Table.DrawTable(_frame, 10, 80, 40, e1);
         t1.SetBackground(Colors.LightYellow);
         model.Add(t1);

         var e2 = Data_Table_Entity.GetPersonNameTable();
         var t2 = Table.DrawTable(_frame, 500, 80, 40, e2);
         t2.SetBackground(Colors.Honeydew);
         model.Add(t2);

         GlOrthoLineShape.Draw(_frame, 10, 300, 400, 600, 
            GlSide.Right, GlDirection.Down);
      }

   }
}
