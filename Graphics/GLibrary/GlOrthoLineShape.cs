using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

using Model.Data;
using ModelConsole.Graphics.GLibrary;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using Windows.Devices.Bluetooth.Advertisement;
using Microsoft.UI;

namespace ModelConsole.Graphics.GLibrary
{

   public enum GlOrthoLineItemType
   {
      Line = 0,
      Arc = 1
   }

   public class GlOrthoLineItem
   {
      public Point Point1 { get; set; }
      public Point Point2 { get; set; }
      public Point Point3 { get; set; }
      public GlOrthoLineItemType Type { get; set; }

      public GlOrthoLineItem(
         Point p1, Point p2)
      {
         Point1 = p1;
         Point2 = p2;
         Type = GlOrthoLineItemType.Line;
      }

      public GlOrthoLineItem(
         Point p1, Point p2, Point p3)
      {
         Point1 = p1;
         Point2 = p2;
         Point3 = p3;
         Type = GlOrthoLineItemType.Arc;
      }
   }

   public class GlOrthoLinePath
   {
      public List<GlOrthoLineItem> Path { get; set; } = 
         new List<GlOrthoLineItem>();
   }

   /// <summary>
   /// Helper to define orthogonal line path with rounded-edges by traversing
   /// through it.
   /// </summary>
   public class GlOrthoLineBuilder
   {
      protected double _edgeRadius = 10;
      protected double _boxMinLength = 30;

      private Point _nextPoint;
      public GlOrthoLinePath Shape { get; set; } = 
         new GlOrthoLinePath();

      public Point NextPoint
      {
         get { return _nextPoint; }
      }

      public void AddLine(
         double x1, double y1, GlSide side, GlDirection direction)
      {
         double l = _boxMinLength - _edgeRadius;
         double x2 = x1, y2 = y1;

         // draw starting line from existing shape
         switch(side)
         {
            case GlSide.Left:
               x2 = x1 - l;
               break;
            case GlSide.Right:
               x2 = x1 + l;
               break;
            case GlSide.Top:
               y2 = y1 + l;
               break;
            case GlSide.Bottom:
               y2 = y1 - l;
               break;
         }

         // perpendicular direction
         double x3 = x2, y3 = y2;
         switch(direction)
         {
            case GlDirection.Left:
               x3 = x2 - _edgeRadius;
               y3 = y2 - _edgeRadius;
               break;
            case GlDirection.Right:
               x3 = x2 + _edgeRadius;
               y3 = y2 + _edgeRadius;
               break;
            case GlDirection.Up:
               x3 = x2 + _edgeRadius;
               y3 = y2 + _edgeRadius;
               break;
            case GlDirection.Down:
               x3 = x2 - _edgeRadius;
               y3 = y2 - _edgeRadius;
               break;
         }

         // draw line
         _nextPoint = new Point(x3, y3);
         var p1 = new Point(x1, y1);
         var p2 = new Point(x2, y2);
         var s1 = new GlOrthoLineItem(p1, p2);
         Shape.Path.Add(s1);

         // draw arc
         var p3 = new Point(x3, y3);
         var p4 = new Point(x2, y2);
         var s2 = new GlOrthoLineItem(p2, p3, p4);
         Shape.Path.Add(s2);
      }

      public void AddLine(double x1, double y1, GlDirection direction)
      {
         // perpendicular direction
         double x3 = x1, y3 = y1;
         switch (direction)
         {
            case GlDirection.Left:
               x3 = x1 - _edgeRadius;
               y3 = y1 - _edgeRadius;
               break;
            case GlDirection.Right:
               x3 = x1 + _edgeRadius;
               y3 = y1 - _edgeRadius;
               break;
            case GlDirection.Up:
               x3 = x1 + _edgeRadius;
               y3 = y1 + _edgeRadius;
               break;
            case GlDirection.Down:
               x3 = x1 - _edgeRadius;
               y3 = y1 - _edgeRadius;
               break;
         }

         // draw a line
         var p1 = _nextPoint;
         var p2 = new Point(x1, y3);
         var s2 = new GlOrthoLineItem(p1, p2);
         Shape.Path.Add(s2);

         // draw an arc
         var p3 = new Point(x3, y1);
         var p4 = new Point(x3, y3);
         var s1 = new GlOrthoLineItem(p2, p3, p4);
         Shape.Path.Add(s1);

         _nextPoint = p3;
      }

      public void AddLine(double x1, double y1)
      {
         // draw a line
         var p1 = _nextPoint;
         var p2 = new Point(x1, y1);
         var s1 = new GlOrthoLineItem(p1, p2);
         Shape.Path.Add(s1);
      }
   }

   /// <summary>
   /// Helper class to draw orthogonal lines with rounded edges.
   /// </summary>
   public class GlOrthoLineShape
   {
      protected Path _path = new Path();
      protected GlOrthoLinePath _orthoShape;

      public Path Path
      {
         get { return _path; }
      }

      public GlOrthoLineShape()
      {
         _path.Stroke = new SolidColorBrush(Colors.Black);
         _path.StrokeThickness = 1;
      }

      /// <summary>
      /// Add orthogonal rounded edges lines from a point to another.
      /// Generally this will be the starting point for drawing orthogonal 
      /// rounded edges lines that connects two shapes.  The line will start or
      /// ends from the bounding box of 2 shapes (such as a rectangle) and 
      /// therefore the side of each source and target should be provided to
      /// figure out the best way to draw the ortho-lines.
      /// </summary>
      /// <param name="x1">start x</param>
      /// <param name="y1">start y</param>
      /// <param name="x2">end x</param>
      /// <param name="y2">end y</param>
      /// <param name="side1">side 1 (default: Right)</param>
      /// <param name="side2">side 2 (default: Left)</param>
      public GlOrthoLinePath AddBox(double x1, double y1, double x2, double y2,
         GlSide side1 = GlSide.Right, GlDirection side2 = GlDirection.Left)
      {
         GlOrthoLineBuilder path = new GlOrthoLineBuilder();
         GlDirection direction;

         // in what direction the path will go?
         if (x1 > x2)
         {
            direction = GlDirection.Left;
         }
         else if (x2 > x1)
         {
            direction = GlDirection.Right;
         }
         else if (y1 > y2)
         {
            direction = GlDirection.Up;
         }
         else
         {
            direction = GlDirection.Down;
         }

         // add line from source shape into the path
         path.AddLine(x1, y1, side1, direction);

         // in what direction the path will go?
         double x3, y3;
         if (direction == GlDirection.Left || direction == GlDirection.Right)
         {
            x3 = path.NextPoint.X;
            double al = Math.Abs(path.NextPoint.Y - y2);
            y3 = y2;// path.NextPoint.Y > y2 ? 
               //path.NextPoint.Y - al : path.NextPoint.Y + al;
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
      /// <param name="frame"></param>
      public void Draw(GlFrame frame)
      {
         var geometry = new PathGeometry();
         var figure = new PathFigure();
         foreach(var i in _orthoShape.Path)
         {
            if (i.Type == GlOrthoLineItemType.Line)
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
               double x1, y1;
               if (i.Point1.X > i.Point2.X)
               {
                  if (i.Point1.Y > i.Point2.Y)
                  {
                     x1 = i.Point1.X;
                     y1 = i.Point1.Y;
                  }
                  else
                  {
                     x1 = i.Point1.X;
                     y1 = i.Point2.Y;
                  }
               }
               else
               {
                  if (i.Point1.Y > i.Point2.Y)
                  {
                     x1 = i.Point2.X;
                     y1 = i.Point1.Y;
                  }
                  else
                  {
                     x1 = i.Point1.X;
                     y1 = i.Point2.Y;
                  }
               }

               Point p3 = new Point(x1, y1);
               
               var bgeo = new BezierSegment();
               bgeo.Point1 = i.Point1;
               bgeo.Point2 = p3;
               bgeo.Point3 = i.Point2;
               figure.Segments.Add(bgeo);
            }
         }
         geometry.Figures.Add(figure);
         _path.Data = geometry;

         frame.Instance.Children.Add(_path);
      }

      /// <summary>
      /// Draw orthogonal lines.
      /// </summary>
      /// <param name="frame">drawing canvas and context</param>
      /// <param name="x1">start x</param>
      /// <param name="y1">start y</param>
      /// <param name="x2">end x</param>
      /// <param name="y2">end y</param>
      /// <param name="side1">side 1 (default: Right)</param>
      /// <param name="side2">side 2 (default: Left)</param>
      /// <returns>orthogonal lines instance is returned</returns>
      public static GlOrthoLineShape Draw(GlFrame frame,
         double x1, double y1, double x2, double y2,
         GlSide side1 = GlSide.Right, GlDirection side2 = GlDirection.Left)
      {
         GlOrthoLineShape shape = new GlOrthoLineShape();
         shape.AddBox(x1, y1, x2, y2, side1, side2);
         shape.Draw(frame);

         Canvas.SetLeft(shape.Path, x1);
         Canvas.SetTop(shape.Path, y1);

         return shape;
      }

   }

}
