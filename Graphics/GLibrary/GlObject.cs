﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConsole.Graphics.GLibrary
{

   public class GlObject : GlObjectInfo
   {
      protected object m_Instance { get; set; } = null;
      public object Instance
      {
         get { return m_Instance; }
      }

      public GlContext Context { get; set; } = null;

      public GlObject(object instance)
      {
         m_Instance = instance;
      }

      /// <summary>
      /// Swap cordinates.
      /// </summary>
      /// <param name="x1">start x</param>
      /// <param name="y1">start y</param>
      /// <param name="x2">end x</param>
      /// <param name="y2">end y</param>
      public static void Swap(
         ref double x1, ref double y1, ref double x2, ref double y2)
      {
         double xt = x2;
         x2 = x1;
         x1 = xt;

         xt = y2;
         y2 = y1;
         y1 = xt;
      }

   }

}
