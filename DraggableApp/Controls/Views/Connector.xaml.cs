using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DraggableApp.Controls.Views
{
    /// <summary>
    /// Interaction logic for Connector
    /// </summary>
    public partial class Connector : UserControl
    {
        public Connector()
        {
            InitializeComponent();
        }
        private void Thumb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Thumb_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var line = (e.Source as Thumb).DataContext as LineGeometry;
            if (line.StartPoint.X == line.EndPoint.X)
            {
                Cursor = Cursors.SizeWE;
            }
            if (line.StartPoint.Y == line.EndPoint.Y)
            {
                Cursor = Cursors.SizeNS;
            }

        }

        private void Thumb_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;

        }
    }
}
