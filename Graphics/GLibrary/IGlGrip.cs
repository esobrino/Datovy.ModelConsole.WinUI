using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;

namespace ModelConsole.Graphics.GLibrary
{

   public interface IGlGrip
   {
      public List<GlGrip> GetGripNodes();
      public GlGrip GetGripNode(Point point);
   }

}
