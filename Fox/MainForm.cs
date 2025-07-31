using Nox.Win32.Controls;
using Nox.Win32.Controls.Base.Super;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox;

internal class MainForm(Nox.Korekuta Collector)
    : Nox.Win32.Forms.Base.Super.FreeWindow(Collector)
{
    private IGame Game = null!;

    protected override void Initialize()
    {
        base.Initialize();

        Width = 400;
        Height = 800;
        Padding = new Padding(8);

        this.AddToCollection<Panel>(new XPanel(this.Collector, "Content")
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle
        });

        Game
    }
}