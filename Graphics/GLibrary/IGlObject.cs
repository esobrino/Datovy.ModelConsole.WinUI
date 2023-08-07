using Microsoft.UI.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ModelConsole.Graphics.GLibrary
{

   public interface IGlObject
   {
      string Guid { get; }
      void DeltaMove(Point? current);
      void PointerEvent(GlPointerEvent pointerEvent, PointerPoint point = null);
   }

}
