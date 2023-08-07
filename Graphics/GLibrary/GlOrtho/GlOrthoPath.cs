using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

using Model.Data;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using Windows.Devices.Bluetooth.Advertisement;
using Microsoft.UI;
using Microsoft.UI.Input;
using System.Reflection;

namespace ModelConsole.Graphics.GLibrary.GlOrtho
{
   /// <summary>
   /// Helper class to draw orthogonal lines with rounded edges.
   /// </summary>
   public class GlOrthoPath : GlObject, IGlObject
   {
      protected Path _path = new Path();
      protected GlOrthoPathShape _orthoShape;

      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; } = 0;

      public Path Path
      {
         get { return _path; }
      }

      public GlOrthoPath() : base(null)
      {
         m_Instance = _path;
         _path.Stroke = new SolidColorBrush(Colors.Black);
         _path.StrokeThickness = 1;
      }

      /// <summary>
      /// Move Object to a relative position using given delta values.
      /// </summary>
      /// <param name="delta">DX and DY distance to move</param>
      public virtual void DeltaMove(Point? delta = null)
      {
         // move object
         if (delta.HasValue)
         {
            X = X + delta.Value.X;
            Y = Y + delta.Value.Y;
         }

         Canvas.SetLeft(_path, X);
         Canvas.SetTop(_path, Y);
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
            Context.SetPointerHandle(_path, point);
         }
         else
         {
            Context.SetPointerHandle(null);
         }
      }

      /// <summary>
      /// Get path for orthogonal rounded edges lines from a point to another.
      /// Generally this will be the starting point for drawing orthogonal 
      /// rounded edges lines that connects two shapes.  The line will start or
      /// ends from the bounding box of 2 shapes (such as a rectangle) and 
      /// therefore the side of each source and target is calculated to
      /// figure out the best way to draw the ortho-lines.
      /// </summary>
      /// <param name="x1">start x</param>
      /// <param name="y1">start y</param>
      /// <param name="x2">end x</param>
      /// <param name="y2">end y</param>
      /// <param name="side">in what direction the path will steer?
      /// (default: right)</param>
      public GlOrthoPathShape GetPath(
         double x1, double y1, double x2, double y2, GlSide side = GlSide.Right)
      {
         // set the original path position
         X = x1;
         Y = y1;

         // move x1,y1 and x2, y2 to the origin
         x1 = 0;
         y1 = 0;
         x2 = x2 - X;
         y2 = y2 - Y;

         // prepare the path
         GlDirection direction;
         GlOrthoPathBuilder path = new GlOrthoPathBuilder();
         if (side == GlSide.Left || side == GlSide.Right)
         {
            // always keep x1 lower than x2
            if (x2 < x1)
            {
               GlObject.Swap(ref x1, ref y1, ref x2, ref y2);
            }

            // always move to the right since x1 is always lower then x2
            side = GlSide.Right;
            direction = y1 < y2 ? GlDirection.Up : GlDirection.Down;
         }
         else
         {
            // always keep y1 lower than y2
            if (y2 < y1)
            {
               GlObject.Swap(ref x1, ref y1, ref x2, ref y2);
            }
            side = GlSide.Bottom;
            direction = x1 < x2 ? GlDirection.Right : GlDirection.Left;
         }

         // what is the distance to the inflection point to steer direction?
         path.MiddleLength = (
            side == GlSide.Right ? x2 - x1 : y2 - y1) / 2.0;

         // add line from source shape into the path
         path.AddLine(x1, y1, side, direction);

         // in what direction the path will go?
         double x3, y3;
         if (direction == GlDirection.Up)
         {
            x3 = path.NextPoint.X;
            y3 = y2;
         }
         else if (direction == GlDirection.Down)
         {
            x3 = path.NextPoint.X;
            y3 = y2;
         }
         else
         {
            y3 = path.NextPoint.Y;
            double al = Math.Abs(path.NextPoint.X - x2);
            x3 = x2;
         }

         // add next lines
         path.AddLine(x3, y3, direction);
         path.AddLine(x2, y2);

         _orthoShape = path.Shape;
         return path.Shape;
      }

      /// <summary>
      /// Draw Path and add it to the Canvas.
      /// </summary>
      /// <param name="context">the drawing context</param>
      public void Draw(GlContext context)
      {
         Context = context;

         var geometry = new PathGeometry();
         var figure = new PathFigure();

         foreach (var i in _orthoShape.Items)
         {
            if (i.Type == GlOrthoPathItemType.Line)
            {
               var lgeo = new LineSegment();
               lgeo.Point = i.Point2;
               if (figure.Segments.Count == 0)
               {
                  figure.StartPoint = i.Point1;
                  figure.Segments.Add(lgeo);
               }
               else
               {
                  figure.Segments.Add(lgeo);
               }
            }
            else
            {
               var bgeo = new BezierSegment();
               bgeo.Point1 = i.Point1;
               bgeo.Point2 = i.Point2;
               bgeo.Point3 = i.Point3;
               figure.Segments.Add(bgeo);
            }
         }

         geometry.Figures.Add(figure);
         _path.Data = geometry;
         _path.Tag = this;

         // move the path to the originally given position
         DeltaMove();

         context.Instance.Children.Add(_path);
      }

      /// <summary>
      /// Draw orthogonal lines.
      /// </summary>
      /// <param name="context">drawing canvas and context</param>
      /// <param name="x1">start x</param>
      /// <param name="y1">start y</param>
      /// <param name="x2">end x</param>
      /// <param name="y2">end y</param>
      /// <returns>orthogonal lines instance is returned</returns>
      public static GlOrthoPath Draw(GlContext context,
         double x1, double y1, double x2, double y2, GlSide side = GlSide.Right)
      {
         GlOrthoPath shape = new GlOrthoPath();
         shape.GetPath(x1, y1, x2, y2, side);
         shape.Draw(context);

         return shape;
      }

   }

}
