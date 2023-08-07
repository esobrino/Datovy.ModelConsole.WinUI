using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.PointOfService;
using Windows.Foundation;

using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace ModelConsole.Graphics.GLibrary
{

   public class GlHandle : GlObject, IGlObject
   {
      public const double DEFAULT_HANDLE_LENGTH = 10.0;

      private Ellipse _ellipse { get; set; }

      private IGlObject _object = null;
      private Shape _shape = null;
      public object Tag
      {
         get { return _shape; }
         set
         {
            _shape = value as Shape;
            _object = (_shape == null) ? null : _shape.Tag as IGlObject;
         }
      }

      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; } = 0;

      private bool _hidden = true;
      public bool Hidden
      {
         get { return _hidden; } 
         set
         {
            _hidden = value;
            if (_ellipse != null)
            {
               _ellipse.Visibility = value ?
                  Visibility.Collapsed : Visibility.Visible;
            }
         }
      }

      public GlHandle() : base(null)
      {
         m_Instance = _ellipse;
      }

      /// <summary>
      /// Move Object to a relative position using given delta values.
      /// </summary>
      /// <param name="delta">DX and DY distance to move</param>
      public virtual void DeltaMove(Point? delta = null)
      {
         if (_object != null)
         {
            _object.DeltaMove(delta);
         }
      }

      /// <summary>
      /// Manage pointer event.
      /// </summary>
      /// <param name="poinerEvent"></param>
      public virtual void PointerEvent(
         GlPointerEvent poinerEvent, PointerPoint point = null)
      {
         if (poinerEvent == GlPointerEvent.Enter)
         {
            Context.SetPointerHandle(_shape, point);
         }
         else
         {
            Context.SetPointerHandle(null);
         }
      }

      public void Draw(GlContext context, double x, double y)
      {
         var d = DEFAULT_HANDLE_LENGTH;
         if (_ellipse == null)
         {
            Context = context;
            var el = _ellipse == null ? new Ellipse() : _ellipse;
            el.Fill = new SolidColorBrush(Colors.WhiteSmoke);
            el.Stroke = new SolidColorBrush(Colors.Gray);
            el.StrokeThickness = 1;
            el.Width = d;
            el.Height = d;

            _ellipse = el;
            _ellipse.Tag = this;
            context.Instance.Children.Add(_ellipse);
         }

         SetPosition(x - d/2.0, y - d/2.0);
      }

      public void SetPosition(double x, double y)
      {
         X = x; 
         Y = y;
         Canvas.SetLeft(_ellipse, x);
         Canvas.SetTop(_ellipse, y);
         Hidden = false;
      }
   }

}
