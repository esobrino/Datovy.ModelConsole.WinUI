﻿using ModelConsole.Model.GLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Primitives
{

   /// <summary>
   /// This primitive is drawn as a line connecting two panels.
   /// </summary>
   public class Connector
   {

      public void RegisterObject(GlObject _object)
      {
         Table table = _object.Instance as Table;
         if (table != null)
         {

         }
      }

   }

}
