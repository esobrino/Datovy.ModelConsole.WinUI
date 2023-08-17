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
using System.Runtime.CompilerServices;
using ModelConsole.Controls;

namespace ModelConsole.Graphics.GLibrary
{

   /// <summary>
   /// Model Graphics Context
   /// </summary>
   public class GlContext
   {
      // Dictionary to maintain information about each active pointer. 
      // An entry is added during PointerPressed/PointerEntered events and
      // removed during
      // PointerReleased/PointerCaptureLost/PointerCanceled/PointerExited events
      private Dictionary<uint, Microsoft.UI.Xaml.Input.Pointer> _pointers;

      public static double DefaultRoundCorderRadious = 10;
      public static double DefaultTextPanelPadding = 4;

      private Shape _currentShape = null;
      private PointerPoint _pointerPoint = null;

      private GlGrip _grip = new GlGrip();
      private GlHandle _handle = new GlHandle();
      private IGlGrabber _grabber = null;

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
         _canvas.PointerExited += Canvas_PointerExited;
         _canvas.PointerCanceled += Canvas_PointerCanceled;
         _canvas.PointerCaptureLost += Canvas_PointerCaptureLost;

         _pointers = new Dictionary<uint, Pointer>();
      }

      public void ResetGrabber()
      {
         if (_grabber != null)
         {
            _grabber.Hidden = true;
            _grabber.Tag = null;
         }
         _handle.Hidden = true;
         _handle.Tag = null;
         _grabber = null;
      }

      /// <summary>
      /// Set Pointer Handler.
      /// </summary>
      /// <param name="point"></param>
      public void SetPointerHandle(Shape item, PointerPoint point = null)
      {
         if (point == null || item == null)
         {
            ResetGrabber();
         }
         else
         {
            IGlGrip grip = item.Tag as IGlGrip;

            var node = grip.GetGripNode(point.Position);
            if (node != null && _grabber != node)
            {
               if (_handle != null)
               {
                  _handle.Hidden = true;
                  _handle.Tag = null;
               }

               _grip = node;
               _grabber = node;
               _grabber.Tag = item;
            }
            else if (_grabber == null)
            {
               if (_grip != null)
               {
                  _grip.Hidden = true;
                  _grip.Tag = null;
                  _grip = null;
               }
               _grabber = _handle;
            }

            _grabber.Draw(this, point.Position.X, point.Position.Y);
            _grabber.Tag = item;
         }
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
         e.Handled = true;

         if (_currentShape != null)
         {
            _currentShape.Opacity = 1;
            _currentShape = null;
         }

         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         var pt = e.GetCurrentPoint(null);

         // check if the pointer is in dictionary
         if (!_pointers.ContainsKey(pt.PointerId))
         {
            _pointers[pt.PointerId] = e.Pointer;
         }

         if (s != null)
         {
            s.CapturePointer(e.Pointer);
            _currentShape = s;
            _pointerPoint = pt;

            s.Opacity = .5;

            var o = s.Tag as GlObject;
            o.PointerEvent(GlPointerEvent.Enter, pt);
         }
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
         e.Handled = true;
         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         PointerPoint pt = e.GetCurrentPoint(null);

         // check if the pointer is in dictionary
         if (!_pointers.ContainsKey(pt.PointerId))
         {
            _pointers[pt.PointerId] = e.Pointer;
         }

         if (s != null)
         {
            if (_pointers.Count == 0)
            {
               s.Opacity = 1;
            }
         }
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
         e.Handled = true;

         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         if (s != null)
         {
            PointerPoint pt = e.GetCurrentPoint(null);

            var o = s.Tag as GlObject;
            if (o != null)
            {
               o.PointerEvent(GlPointerEvent.Enter, pt);

               var tag = _currentShape == null ? 
                  null : _currentShape.Tag as GlObject;
               if (tag == null)
               {
                  return;
               }

               if (_currentShape != null && o.Guid != tag.Guid)
               {
                  return;
               }

               Point delta = new Point();
               delta.X = pt.Position.X - _pointerPoint.Position.X;
               delta.Y = pt.Position.Y - _pointerPoint.Position.Y;

               o.DeltaMove(delta);

               _pointerPoint = pt;
               _currentShape = s;

               return;
            }
         }
      }

      /// <summary>
      /// Release Shape
      /// </summary>
      private void ReleaseShape()
      {
         if (_currentShape != null)
         {
            _currentShape.Opacity = 1;
         }
         _currentShape = null;
         SetPointerHandle(null);
      }

      /// <summary>
      /// Pointer Release
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerRelease(
         object sender, PointerRoutedEventArgs e)
      {
         e.Handled = true;

         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         PointerPoint pt = e.GetCurrentPoint(null);

         // check if the pointer is in dictionary
         if (!_pointers.ContainsKey(pt.PointerId))
         {
            _pointers[pt.PointerId] = null;
            _pointers.Remove(pt.PointerId);
         }

         if (s != null)
         {
            s.Opacity = 1;
            s.ReleasePointerCapture(e.Pointer);
         }

         ReleaseShape();
      }

      /// <summary>
      /// Pointer Exited
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerExited(
         object sender, PointerRoutedEventArgs e)
      {
         e.Handled = true;

         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         PointerPoint pt = e.GetCurrentPoint(null);

         // check if the pointer is in dictionary
         if (!_pointers.ContainsKey(pt.PointerId))
         {
            _pointers[pt.PointerId] = null;
            _pointers.Remove(pt.PointerId);
         }

         if (s != null)
         {
            if (_pointers.Count == 0)
            {
               s.Opacity = 1;
            }
         }

         ReleaseShape();
      }

      /// <summary>
      /// Pointer Canceled
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerCanceled(
         object sender, PointerRoutedEventArgs e)
      {
         e.Handled = true;

         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         PointerPoint pt = e.GetCurrentPoint(null);

         // check if the pointer is in dictionary
         if (!_pointers.ContainsKey(pt.PointerId))
         {
            _pointers[pt.PointerId] = null;
            _pointers.Remove(pt.PointerId);
         }

         if (s != null)
         {
            s.ReleasePointerCapture(e.Pointer);

            if (_pointers.Count == 0)
            {
               s.Opacity = 1;
            }
         }

         ReleaseShape();
      }

      /// <summary>
      /// Pointer Capture Lost
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Canvas_PointerCaptureLost(
         object sender, PointerRoutedEventArgs e)
      {
         e.Handled = true;

         var s = e.OriginalSource as Shape;
         if (s == null)
         {
            s = _currentShape;
         }

         PointerPoint pt = e.GetCurrentPoint(null);

         // check if the pointer is in dictionary
         if (!_pointers.ContainsKey(pt.PointerId))
         {
            _pointers[pt.PointerId] = null;
            _pointers.Remove(pt.PointerId);
         }

         if (s != null)
         {
            s.ReleasePointerCapture(e.Pointer);

            if (_pointers.Count == 0)
            {
               s.Opacity = 1;
            }
         }

         ReleaseShape();
      }

   }

}
