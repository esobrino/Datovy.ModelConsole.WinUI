using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI;
using Microsoft.UI.Input;
using Windows.Foundation;

namespace ModelConsole.Graphics.GLibrary
{

   /// <summary>
   /// Model Graphics Context
   /// </summary>
   public class GlContext
   {

      public static double DefaultRoundCorderRadious = 10;
      public static double DefaultTextPanelPadding = 4;

      private IGlObject _pointerObject = null;
      private PointerPoint _pointerPoint = null;

      private Canvas _canvas;
      public Canvas Instance
      {
         get { return _canvas; }
      }

      public GlContext(Canvas canvas)
      {
         _canvas = canvas;
         _canvas.PointerPressed += Canvas_PointerPressed;
         _canvas.PointerReleased += Canvas_PointerRelease;
         _canvas.PointerMoved += Canvas_PointerMoved;
         _canvas.PointerEntered += Canvas_PointerEntered;
      }

      /// <summary>
      /// When user press the pointer over a graphic instance this function gets
      /// called and e.OriginalSource will identify the object.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerPressed(
         object sender, PointerRoutedEventArgs e)
      {
         _pointerPoint = e.GetCurrentPoint(null);
         _pointerObject = null;
      }

      /// <summary>
      /// On Pointer Move if there is an object already clicked then move the
      /// object to a new position based on the moved distance.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerMoved(
         object sender, PointerRoutedEventArgs e)
      {
         var ptr = e.Pointer;
         var p = e.GetCurrentPoint(null);
         var s = e.OriginalSource as Shape;
         if (s != null && _pointerPoint != null)
         {
            var o = s.Tag as IGlObject;
            if (o != null)
            {
               if (_pointerObject != null && o.Guid != _pointerObject.Guid)
               {
                  return;
               }

               _pointerObject = o;
               Point delta = new Point();
               delta.X = p.Position.X - _pointerPoint.Position.X;
               delta.Y = p.Position.Y - _pointerPoint.Position.Y;
               o.DeltaMove(delta);
               _pointerPoint = p;
               e.Handled = true;
               return;
            }
         }
         e.Handled = false;
      }

      private void Canvas_PointerRelease(
         object sender, PointerRoutedEventArgs e)
      {
         _pointerPoint = null;
      }

      /// <summary>
      /// When user move the pointer over a graphic instance this function gets
      /// called and e.OriginalSource will identify the object.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerEntered(
         object sender, PointerRoutedEventArgs e)
      {

      }

   }

}
