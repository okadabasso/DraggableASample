using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DraggableApp.Controls
{
    public class SampleControl : UserControl
    {
        //依存関係プロパティ
        public static readonly DependencyProperty ProductCodeProperty =
                  DependencyProperty.Register("ProductCode",
                                               typeof(string),
                                               typeof(SampleControl),
                                               new FrameworkPropertyMetadata("ProductCode", new PropertyChangedCallback(OnProductCodeChanged)));

        //外部に公開するプロパティ
        public string ProductCode
        {
            get { return (string)GetValue(ProductCodeProperty); }
            set { SetValue(ProductCodeProperty, value); }
        }
        public TextBox ProductCodeTextBox
        { get; set; }

        public SampleControl()
        {
            ProductCodeTextBox = new TextBox();
            ProductCodeTextBox.Height = 30;
            ProductCodeTextBox.Width = 192;
            ProductCodeTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            ProductCodeTextBox.Foreground = new SolidColorBrush(Colors.Black);

            this.AddChild(ProductCodeTextBox);
        }
        //コールバックイベントの処理
        private static void OnProductCodeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (SampleControl)obj;

            control.ProductCodeTextBox.Text = (obj != null) ? control.ProductCode : control.ProductCodeTextBox.Text;
        }
    }
}
