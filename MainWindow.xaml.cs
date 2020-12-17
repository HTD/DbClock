using System.Windows;
using System.Windows.Threading;

namespace DbClock {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        public MainWindow() {
            InitializeComponent();
            var movement = new Movement();
            movement.Tick += (s, e) => Clock.Time = movement.Interpolated;
        }

        private void Menu_Exit(object sender, RoutedEventArgs e) {
            Close();
        }

    }

}