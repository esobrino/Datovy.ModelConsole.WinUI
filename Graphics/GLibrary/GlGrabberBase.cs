﻿using System;
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
using SkiaSharp;

namespace ModelConsole.Graphics.GLibrary
{

   /// <summary>
   /// Handles allow to select and drag/move the object around.  Handles are 
   /// represented as circles.
   /// </summary>
   public class GlGrabberBase<T> : GlObject, IGlGrabber
   {
      public const double DEFAULT_HANDLE_LENGTH = 14.0;

      protected T _grabber;
      protected Shape _grabberShape;

      /// <summary>
      /// _objec and _objectShape are the thing we are handling or gripping...
      /// </summary>
      protected GlObject _object = null;
      protected Shape _objectShape = null;
      public object Tag
      {
         get { return _objectShape; }
         set
         {
            _objectShape = value as Shape;
            _object = (_objectShape == null) ?
               null : _objectShape.Tag as GlObject;
         }
      }

      public T NativeInstance
      {
         get { return _grabber; }
      }

      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; } = 0;

      protected bool _hidden = true;
      public bool Hidden
      {
         get { return _hidden; }
         set
         {
            _hidden = value;
            if (_grabber != null)
            {
               _grabberShape.Visibility = value ?
                  Visibility.Collapsed : Visibility.Visible;
            }
         }
      }

      protected GlBoundingBox _boundingBox = null;
      public GlBoundingBox BoundingBox
      {
         get
         {
            if (_boundingBox == null)
            {
               var d = DEFAULT_HANDLE_LENGTH / 2.0;
               _boundingBox = new GlBoundingBox(X - d, Y - d, X + d, Y + d);
            }
            return _boundingBox;
         }
      }

      public GlGrabberBase() : base(null)
      {
         m_Instance = _grabber;
      }

      /// <summary>
      /// Move Object to a relative position using given delta values.
      /// </summary>
      /// <param name="delta">DX and DY distance to move</param>
      public override void DeltaMove(Point? delta = null)
      {
         if (_object != null)
         {
            _object.DeltaMove(delta);
         }
      }

      /// <summary>
      /// Is Given point inside the Shape?
      /// </summary>
      /// <param name="point"></param>
      /// <returns>true is returned if it is, else false</returns>
      public bool PointInShape(Point point)
      {
         var bb = BoundingBox;
         return bb.PointInShape(point);
      }

      /// <summary>
      /// Manage pointer event.
      /// </summary>
      /// <param name="poinerEvent"></param>
      public override void PointerEvent(
         GlPointerEvent poinerEvent, PointerPoint point = null)
      {
         if (poinerEvent == GlPointerEvent.Enter)
         {
            Context.SetPointerHandle(_objectShape, point);
         }
         else
         {
            Context.SetPointerHandle(null);
         }
      }

      public override void Reshape(object node)
      {
      }

      /// <summary>
      /// Return true if node is near enough to this node.
      /// </summary>
      /// <param name="node"></param>
      /// <returns></returns>
      public bool Snapped(GlGrip node)
      {
         if (node == null)
         {
            return false;
         }
         var d = DEFAULT_HANDLE_LENGTH / 2.0;
         Point mp = new Point(node.X, node.Y);
         return PointInShape(mp);
      }

      /// <summary>
      /// Greate grabber shape
      /// </summary>
      /// <param name="context"></param>
      public void Create(GlContext context)
      {
         var d = DEFAULT_HANDLE_LENGTH;
         if (_grabberShape == null)
         {
            Context = context;
            _grabber = (T)Activator.CreateInstance(typeof(T));
            _grabberShape = _grabber as Shape;

            _grabberShape.Fill = new SolidColorBrush(Colors.WhiteSmoke);
            _grabberShape.Stroke = new SolidColorBrush(Colors.Gray);
            _grabberShape.StrokeThickness = 1;
            _grabberShape.Width = d;
            _grabberShape.Height = d;
            _grabberShape.Visibility = Visibility.Collapsed;

            _grabberShape.Tag = this;
            Instance = _grabber;
            context.Instance.Children.Add(_grabberShape);
         }
      }

      /// <summary>
      /// Draw grabber shape.
      /// </summary>
      /// <param name="context"></param>
      /// <param name="x"></param>
      /// <param name="y"></param>
      public void Draw(GlContext context, double x, double y)
      {
         var d = DEFAULT_HANDLE_LENGTH;
         Create(context);

         X = x;
         Y = y;
         SetPosition(x - d / 2.0, y - d / 2.0);
      }

      /// <summary>
      /// Set grabber position.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      public void SetPosition(double x, double y)
      {
         Canvas.SetLeft(_grabberShape, x);
         Canvas.SetTop(_grabberShape, y);
         Hidden = false;
      }
   }

}
