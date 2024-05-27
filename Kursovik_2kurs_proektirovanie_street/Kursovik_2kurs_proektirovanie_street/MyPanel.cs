using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursovik_2kurs_proektirovanie_street
{
    public class MyPanel : Panel
    {
        public MyPanel()
        {
            // Включение двойной буферизации для предотвращения мерцания
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
    }
}
