using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PipelineHazardDetector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void DisplayWithHazardsOnClick(object sender, RoutedEventArgs e) {
            String instructionSequence = InstructionSequence.Text;
            App.ParseInstructions(instructionSequence, 1);
            //MessageBox.Show("Display With Hazards: " + instructionSequence);
        }

        private void DisplayWithoutForwardingOnClick(object sender, RoutedEventArgs e) {
            String instructionSequence = InstructionSequence.Text;
            App.ParseInstructions(instructionSequence, 2);
            //TextBox textBox = new TextBox();
            //textBox.Text = "Test";
            //PipelineDisplay.Children.Add(textBox);
            //MessageBox.Show("Display Without Forwarding: " + instructionSequence);
        }

        private void DisplayWithForwardingOnClick(object sender, RoutedEventArgs e) {
            String instructionSequence = InstructionSequence.Text;
            App.ParseInstructions(instructionSequence, 3);
            //MessageBox.Show("Display With Forwarding: " + instructionSequence);
        }

    }
}
