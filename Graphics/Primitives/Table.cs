using System.Collections.Generic;

using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

using Model.Data;
using ModelConsole.Graphics.GLibrary;

namespace ModelConsole.Graphics.Primitives
{

   public class Panel : GlBoxInfo
   {
   }

   /// <summary>
   /// Table Primitive
   /// </summary>
   public class Table : GlRectangle, IGlObject
   {
      private double _leftPadding = 66;
      private double _rightPadding = 24;

      private double _bannerHeight;

      private TableInfo _table;

      private StackPanel _rowsPanel = new StackPanel();
      public List<TableRowPanel> Rows { get; set; } = new List<TableRowPanel>();

      private double _column1Width;
      private double _column2Width;
      private double _column3Width;

      /// <summary>
      /// Table class initialization using table information
      /// </summary>
      /// <param name="table">table information</param>
      public Table(GlFrame frame, double x, double y,
         double bannerHeight, TableInfo table) : base()
      {
         X = x;
         Y = y;
         _bannerHeight = bannerHeight;
         CornerRadius = GlFrame.DefaultRoundCorderRadious;

         SetTable(table);
      }

      /// <summary>
      /// Move Object to a relative position using given delta values.
      /// </summary>
      /// <param name="delta">DX and DY distance to move if null the object
      /// will move to the current X,Y position</param>
      public override void DeltaMove(Point? delta = null)
      {
         base.DeltaMove(delta);
         Canvas.SetLeft(_rowsPanel, X);
         Canvas.SetTop(_rowsPanel, Y + _bannerHeight);
      }

      /// <summary>
      /// Set Table.
      /// </summary>
      /// <param name="table">table information</param>
      public void SetTable(TableInfo table)
      {
         _table = table;
         _table.Copy(table);
         AddColumns(table.Columns);
      }

      /// <summary>
      /// Draw Banner Text as schema and table names.
      /// </summary>
      public void DrawBannerText(GlFrame frame)
      {
         AddBanner(
            frame, this, _table.SchemaName + "::" + _table.TableName);
      }

      /// <summary>
      /// Set table Banner Text.
      /// </summary>
      /// <param name="schemaName">schema name</param>
      /// <param name="tableName">table name</param>
      public void DrawBannerText(
         GlFrame frame, string schemaName, string tableName)
      {
         _table.SchemaName = schemaName;
         _table.TableName = tableName;

         DrawBannerText(frame);
      }

      /// <summary>
      /// Add Columns
      /// </summary>
      /// <param name="columns">columns list to add</param>
      public void AddColumns(List<ColumnInfo> columns)
      {
         GlTextBox b = new GlTextBox();

         double heigth = b.FontSize + GlFrame.DefaultTextPanelPadding;
         double maxLength = 0;
         double maxTypeLength = 0;
         Size size;

         foreach (var i in columns)
         {
            b.Text = i.ColumnName;
            size = b.GetDesiredSize();
            if (size.Width > maxLength)
            {
               maxLength = size.Width;
            }

            b.Text = TableRowPanel.GetDataType(i);
            size = b.GetDesiredSize();
            if (size.Width > maxTypeLength)
            {
               maxTypeLength = size.Width;
            }
         }

         double x = X;
         double y = Y + _bannerHeight + CornerRadius +
            GlFrame.DefaultTextPanelPadding / 2.0;

         maxLength += 10;
         _column1Width = _leftPadding;
         _column2Width = maxLength;
         _column3Width = maxTypeLength;

         foreach (var c in columns)
         {
            var p = new TableRowPanel();

            p.Instance.Orientation = Orientation.Horizontal;
            p.X = x + 1;
            p.Y = y;

            p.Width = maxLength;
            p.Height = heigth + GlFrame.DefaultTextPanelPadding * 2;

            p.Column = c;

            p.SetSize();
            p.SetSize(_column1Width, _column2Width, _column3Width);

            Rows.Add(p);
            y += p.Height;
         }

         _rowsPanel.Children.Clear();
      }

      /// <summary>
      /// Draw Table baesd on set info and columns...
      /// </summary>
      public void DrawTable(GlFrame frame)
      {
         bool everyOther = true;
         double height = _bannerHeight + CornerRadius * 2;

         foreach (var i in Rows)
         {
            // fill everyother
            everyOther = !everyOther;

            i.SetBackground(
               everyOther ? Microsoft.UI.Colors.WhiteSmoke : 
                  Microsoft.UI.Colors.White);

            // draw constraint
            string constraint = null;
            if (i.Column.IsKey)
            {
               constraint += "PK";
            }
            if (i.Column.IsForeignKey)
            {
               constraint += constraint == null ? "" : ", ";
               constraint += "FK";
            }

            if (constraint != null)
            {
               i.ConstraintText.Text = constraint;
            }

            // draw column name text
            i.Text.Text = i.Column.ColumnName;

            // draw data type text
            i.DataTypeText.Text = TableRowPanel.GetDataType(i.Column);

            // add panel padding to show space around text-blocks
            i.Instance.Padding = new Thickness(
               10, GlFrame.DefaultTextPanelPadding, 
               10, GlFrame.DefaultTextPanelPadding);

            // add row panel to rows-panel 
            _rowsPanel.Children.Add(i.NativeInstance);
            height += i.Height;
         }

         Width = _column1Width + _column2Width + _column3Width + 22;
         Height = height + 40;

         SetInstance(X, Y, Width, Height, GlFrame.DefaultRoundCorderRadious);

         NativeInstance.Tag = this;
         frame.Instance.Children.Add(NativeInstance);
         frame.Instance.Children.Add(_rowsPanel);

         DeltaMove();
         DrawBannerText(frame, _table.SchemaName, _table.TableName);
      }

      /// <summary>
      /// Draw Table based on given info and columns.
      /// </summary>
      /// <param name="frame">frame</param>
      /// <param name="x">x lower-left</param>
      /// <param name="y">y lower-left</param>
      /// <param name="bannerHeight">top banner height</param>
      /// <param name="table"></param>
      /// <returns></returns>
      public static Table DrawTable(GlFrame frame, float x, float y,
         float bannerHeight, TableInfo table)
      {
         Table t = new Table(frame, x, y, bannerHeight, table);
         t.DrawTable(frame);
         return t;
      }

   }

}
