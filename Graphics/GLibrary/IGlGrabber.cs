using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;

namespace ModelConsole.Graphics.GLibrary
{

   public interface IGlGrabber
   {
      bool Hidden { get; set; }
      object Tag { get; set; }
      void Draw(GlContext context, double x, double y);
      void DeltaMove(Point? point);
      void Move(Point? point);
   }

}
