﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConsole.Model.GLibrary
{

   public class GlModel
   {
      public List<GlObject> Items { get; set; } = new List<GlObject>();

      public GlObject Add(object instance)
      {
         GlObject o = new GlObject(instance);
         Items.Add(o);
         return o;
      }
   }

}
