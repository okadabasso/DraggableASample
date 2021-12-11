using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

namespace DraggableApp.Views
{
    /// <summary>
    /// Interaction logic for LineDragView
    /// </summary>
    public partial class LineDragView : UserControl
    {
        public LineDragView()
        {
            InitializeComponent();
        }

        private void Thumb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Thumb_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var line = (e.Source as Thumb).DataContext as LineGeometry;
            if(line.StartPoint.X == line.EndPoint.X)
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
